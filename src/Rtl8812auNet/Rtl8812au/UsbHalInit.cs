using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace Rtl8812auNet.Rtl8812au;

public static class UsbHalInit
{
    private const byte BOOT_FROM_EEPROM = (byte)BIT4;
    private const byte EEPROM_EN = (byte)BIT5;
    private const UInt32 HWSET_MAX_SIZE_JAGUAR = 512;
    private const byte EFUSE_WIFI = 0;
    private const UInt16 EEPROM_TX_PWR_INX_8812 = 0x10;
    private const ushort EFUSE_MAP_LEN_JAGUAR = 512;
    private const UInt32 EFUSE_MAX_SECTION_JAGUAR = 64;
    private const UInt32 EFUSE_MAX_WORD_UNIT = 4;
    private const UInt32 EFUSE_REAL_CONTENT_LEN_JAGUAR = 512;
    private const UInt16 RTL_EEPROM_ID = 0x8129;
    private const byte EEPROM_DEFAULT_VERSION = 0;
    private const byte EEPROM_VERSION_8812 = 0xC4;
    private const byte EEPROM_XTAL_8812 = 0xB9;
    private const byte EEPROM_DEFAULT_CRYSTAL_CAP_8812 = 0x20;

    public static void rtl8812au_interface_configure(AdapterState padapter)
    {
        var pHalData = padapter.HalData;

        DvObj pdvobjpriv = padapter.DvObj;

        pHalData.UsbTxAggMode = true;
        pHalData.UsbTxAggDescNum = 6; /* only 4 bits */
        pHalData.UsbTxAggDescNum = 0x01; /* adjust value for OQT  Overflow issue */ /* 0x3;	 */ /* only 4 bits */
        pHalData.rxagg_mode = RX_AGG_MODE.RX_AGG_USB;
        pHalData.rxagg_usb_size = 8; /* unit: 512b */
        pHalData.rxagg_usb_timeout = 0x6;
        pHalData.rxagg_dma_size = 16; /* uint: 128b, 0x0A = 10 = MAX_RX_DMA_BUFFER_SIZE/2/pHalData.UsbBulkOutSize */
        pHalData.rxagg_dma_timeout = 0x6; /* 6, absolute time = 34ms/(2^6) */

        if (padapter.DvObj.UsbSpeed == RTW_USB_SPEED_3)
        {
            pHalData.rxagg_usb_size = 0x7;
            pHalData.rxagg_usb_timeout = 0x1a;
        }
        else
        {
            /* the setting to reduce RX FIFO overflow on USB2.0 and increase rx throughput */
            pHalData.rxagg_usb_size = 0x5;
            pHalData.rxagg_usb_timeout = 0x20;
        }

        var chipOut = GetChipOutEP8812(pdvobjpriv.OutPipesCount);
        pHalData.OutEpQueueSel = chipOut.OutEpQueueSel;
        pHalData.OutEpNumber = chipOut.OutEpNumber;
    }

    public static void ReadAdapterInfo8812AU(AdapterState adapterState)
    {
        /* Read all content in Efuse/EEPROM. */
        Hal_ReadPROMContent_8812A(adapterState);
    }

    static void Hal_ReadPROMContent_8812A(AdapterState adapterState)
    {
        var pHalData = adapterState.HalData;

        /* check system boot selection */
        var eeValue = adapterState.Device.rtw_read8(REG_9346CR);
        pHalData.EepromOrEfuse = (eeValue & BOOT_FROM_EEPROM) != 0;
        pHalData.AutoloadFailFlag = (eeValue & EEPROM_EN) == 0;

        RTW_INFO($"Boot from {(pHalData.EepromOrEfuse ? "EEPROM" : "EFUSE")}, Autoload {(pHalData.AutoloadFailFlag ? "Fail" : "OK")} !");

        InitAdapterVariablesByPROM_8812AU(adapterState);
    }

    private static void InitAdapterVariablesByPROM_8812AU(AdapterState adapterState)
    {
        var pHalData = adapterState.HalData;

        hal_InitPGData_8812A(adapterState);

        Hal_EfuseParseIDCode8812A(adapterState, pHalData.efuse_eeprom_data);

        Hal_ReadPROMVersion8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        Hal_ReadTxPowerInfo8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        Hal_ReadBoardType8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);

        /*  */
        /* Read Bluetooth co-exist and initialize */
        /*  */
        Hal_EfuseParseBTCoexistInfo8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);

        Hal_EfuseParseXtal_8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        Hal_ReadThermalMeter_8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);

        Hal_ReadAmplifierType_8812A(adapterState, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        Hal_ReadRFEType_8812A(adapterState.HalData, pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);


        pHalData.EEPROMUsbSwitch = ReadUsbModeSwitch8812AU(pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        RTW_INFO("Usb Switch: %d", pHalData.EEPROMUsbSwitch);

        /* 2013/04/15 MH Add for different board type recognize. */
        hal_ReadUsbType_8812AU(adapterState, pHalData.efuse_eeprom_data);
    }

    static void hal_ReadUsbType_8812AU(AdapterState adapterState, u8[] PROMContent)
    {
        /* if (IS_HARDWARE_TYPE_8812AU(adapterState) && adapterState.UsbModeMechanism.RegForcedUsbMode == 5) */
        {
            var pHalData = adapterState.HalData;

            u8 reg_tmp, i, j, antenna = 0, wmode = 0;
            /* Read anenna type from EFUSE 1019/1018 */
            for (i = 0; i < 2; i++)
            {
                /*
                  Check efuse address 1019
                  Check efuse address 1018
                */
                adapterState.Device.efuse_OneByteRead((ushort)(1019 - i), out reg_tmp);
                /*
                  CHeck bit 7-5
                  Check bit 3-1
                */
                if (((reg_tmp >> 5) & 0x7) != 0)
                {
                    antenna = (byte)((reg_tmp >> 5) & 0x7);
                    break;
                }
                else if ((reg_tmp >> 1 & 0x07) != 0)
                {
                    antenna = (byte)((reg_tmp >> 1) & 0x07);
                    break;
                }


            }

            /* Read anenna type from EFUSE 1021/1020 */
            for (i = 0; i < 2; i++)
            {
                /*
                  Check efuse address 1021
                  Check efuse address 1020
                */
                adapterState.Device.efuse_OneByteRead((ushort)(1021 - i), out reg_tmp);

                /* CHeck bit 3-2 */
                if (((reg_tmp >> 2) & 0x3) != 0)
                {
                    wmode = (byte)((reg_tmp >> 2) & 0x3);
                    break;
                }
            }

            RTW_INFO("%s: antenna=%d, wmode=%d", antenna, wmode);
/* Antenna == 1 WMODE = 3 RTL8812AU-VL 11AC + USB2.0 Mode */
            if (antenna == 1)
            {
                /* Config 8812AU as 1*1 mode AC mode. */
                pHalData.rf_type = RfType.RF_1T1R;
                /* UsbModeSwitch_SetUsbModeMechOn(adapterState, FALSE); */
                /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VL; */
                RTW_INFO("%s(): EFUSE_HIDDEN_812AU_VL\n");
            }
            else if (antenna == 2)
            {
                if (wmode == 3)
                {
                    if (PROMContent[EEPROM_USB_MODE_8812] == 0x2)
                    {
                        /* RTL8812AU Normal Mode. No further action. */
                        /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU; */
                        RTW_INFO("%s(): EFUSE_HIDDEN_812AU");
                    }
                    else
                    {
                        /* Antenna == 2 WMODE = 3 RTL8812AU-VS 11AC + USB2.0 Mode */
                        /* Driver will not support USB automatic switch */
                        /* UsbModeSwitch_SetUsbModeMechOn(adapterState, FALSE); */
                        /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VS; */
                        RTW_INFO("%s(): EFUSE_HIDDEN_8812AU_VS");
                    }
                }
                else if (wmode == 2)
                {
                    /* Antenna == 2 WMODE = 2 RTL8812AU-VN 11N only + USB2.0 Mode */
                    /* UsbModeSwitch_SetUsbModeMechOn(adapterState, FALSE); */
                    /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VN; */
                    RTW_INFO("%s(): EFUSE_HIDDEN_8812AU_VN");
                }
            }
        }
    }

    private static bool ReadUsbModeSwitch8812AU(u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        if (AutoloadFail)
        {
            return false;
        }
        else /* check efuse 0x08 bit2 */
        {
            return ((PROMContent[EEPROM_USB_MODE_8812] & BIT1) >> 1) != 0;
        }
    }

    private static void Hal_ReadRFEType_8812A(hal_com_data pHalData, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        if (!AutoloadFail)
        {
            if ((registry_priv.RFE_Type != 64) || 0xFF == PROMContent[EEPROM_RFE_OPTION_8812])
            {
                if (registry_priv.RFE_Type != 64)
                {
                    pHalData.rfe_type = registry_priv.RFE_Type;
                }
                else
                {
                    pHalData.rfe_type = 0;
                }

            }
            else if ((PROMContent[EEPROM_RFE_OPTION_8812] & BIT7) != 0)
            {
                if (pHalData.external_lna_5g == true || pHalData.external_lna_5g == null)
                {
                    if (pHalData.external_pa_5g == true || pHalData.external_pa_5g == null)
                    {
                        if (pHalData.ExternalLNA_2G && pHalData.ExternalPA_2G)
                            pHalData.rfe_type = 3;
                        else
                            pHalData.rfe_type = 0;
                    }
                    else
                        pHalData.rfe_type = 2;
                }
                else
                    pHalData.rfe_type = 4;
            }
            else
            {
                pHalData.rfe_type = (ushort)(PROMContent[EEPROM_RFE_OPTION_8812] & 0x3F);

                /* 2013/03/19 MH Due to othe customer already use incorrect EFUSE map */
                /* to for their product. We need to add workaround to prevent to modify */
                /* spec and notify all customer to revise the IC 0xca content. After */
                /* discussing with Willis an YN, revise driver code to prevent. */
                if (pHalData.rfe_type == 4 &&
                    (pHalData.external_pa_5g == true || pHalData.ExternalPA_2G == true ||
                     pHalData.external_lna_5g == true || pHalData.ExternalLNA_2G == true))
                {
                    pHalData.rfe_type = 0;
                }
            }
        }
        else
        {
            if (registry_priv.RFE_Type != 64)
            {
                pHalData.rfe_type = registry_priv.RFE_Type;
            }
            else
            {
                pHalData.rfe_type = 0;
            }
        }

        RTW_INFO("RFE Type: 0x%2x\n", pHalData.rfe_type);
    }

    static void Hal_ReadThermalMeter_8812A(AdapterState adapterState, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        var pHalData = adapterState.HalData;
        /* u8	tempval; */

        /*  */
        /* ThermalMeter from EEPROM */
        /*  */
        if (!AutoloadFail)
        {
            pHalData.eeprom_thermal_meter = PROMContent[EEPROM_THERMAL_METER_8812];
        }
        else
            pHalData.eeprom_thermal_meter = EEPROM_Default_ThermalMeter_8812;
        /* pHalData.eeprom_thermal_meter = (tempval&0x1f);	 */ /* [4:0] */

        if (pHalData.eeprom_thermal_meter == 0xff || AutoloadFail)
        {
            pHalData.eeprom_thermal_meter = 0xFF;
        }

        /* pHalData.ThermalMeter[0] = pHalData.eeprom_thermal_meter;	 */
        RTW_INFO("ThermalMeter = 0x%x\n", pHalData.eeprom_thermal_meter);
    }

    static void Hal_EfuseParseBTCoexistInfo8812A(AdapterState adapterState, u8[] hwinfo, BOOLEAN AutoLoadFail)
    {
        var pHalData = adapterState.HalData;

        if (!AutoLoadFail)
        {
            var tmp_u8 = hwinfo[EEPROM_RF_BOARD_OPTION_8812];
            if (((tmp_u8 & 0xe0) >> 5) == 0x1) /* [7:5] */
                pHalData.EEPROMBluetoothCoexist = true;
            else
                pHalData.EEPROMBluetoothCoexist = false;
        }
        else
        {
            pHalData.EEPROMBluetoothCoexist = false;
        }
    }

    static void Hal_ReadBoardType8812A(AdapterState adapterState, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        var pHalData = adapterState.HalData;

        if (!AutoloadFail)
        {
            pHalData.InterfaceSel = (byte)((PROMContent[EEPROM_RF_BOARD_OPTION_8812] & 0xE0) >> 5);
            if (PROMContent[EEPROM_RF_BOARD_OPTION_8812] == 0xFF)
            {
                pHalData.InterfaceSel = (EEPROM_DEFAULT_BOARD_OPTION & 0xE0) >> 5;
            }
        }
        else
        {
            pHalData.InterfaceSel = 0;
        }

        RTW_INFO("Board Type: 0x%2x", pHalData.InterfaceSel);

    }

    private static void Hal_ReadTxPowerInfo8812A(AdapterState adapterState, u8[] PROMContent, BOOLEAN AutoLoadFail)
    {
        var pHalData = adapterState.HalData;

        /* 2010/10/19 MH Add Regulator recognize for CU. */
        if (!AutoLoadFail)
        {

            if (PROMContent[EEPROM_RF_BOARD_OPTION_8812] == 0xFF)
            {
                pHalData.EEPROMRegulatory = (EEPROM_DEFAULT_BOARD_OPTION & 0x7); /* bit0~2 */
            }
            else
            {
                pHalData.EEPROMRegulatory = (byte)(PROMContent[EEPROM_RF_BOARD_OPTION_8812] & 0x7); /* bit0~2 */
            }

        }
        else
        {
            pHalData.EEPROMRegulatory = 0;
        }

        RTW_INFO("EEPROMRegulatory = 0x%x", pHalData.EEPROMRegulatory);

    }

    private static void Hal_ReadAmplifierType_8812A(AdapterState adapterState, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        var pHalData = adapterState.HalData;

        u8 extTypePA_2G_A = (byte)((PROMContent[0xBD] & BIT2) >> 2); /* 0xBD[2] */
        u8 extTypePA_2G_B = (byte)((PROMContent[0xBD] & BIT6) >> 6); /* 0xBD[6] */
        u8 extTypePA_5G_A = (byte)((PROMContent[0xBF] & BIT2) >> 2); /* 0xBF[2] */
        u8 extTypePA_5G_B = (byte)((PROMContent[0xBF] & BIT6) >> 6); /* 0xBF[6] */
        u8 extTypeLNA_2G_A = (byte)((PROMContent[0xBD] & (BIT1 | BIT0)) >> 0); /* 0xBD[1:0] */
        u8 extTypeLNA_2G_B = (byte)((PROMContent[0xBD] & (BIT5 | BIT4)) >> 4); /* 0xBD[5:4] */
        u8 extTypeLNA_5G_A = (byte)((PROMContent[0xBF] & (BIT1 | BIT0)) >> 0); /* 0xBF[1:0] */
        u8 extTypeLNA_5G_B = (byte)((PROMContent[0xBF] & (BIT5 | BIT4)) >> 4); /* 0xBF[5:4] */

        hal_ReadPAType_8812A(adapterState, PROMContent, AutoloadFail);

        if ((pHalData.PAType_2G & (BIT5 | BIT4)) == (BIT5 | BIT4)) /* [2.4G] Path A and B are both extPA */
        {
            pHalData.TypeGPA = (ushort)(extTypePA_2G_B << 2 | extTypePA_2G_A);
        }

        if ((pHalData.PAType_5G & (BIT1 | BIT0)) == (BIT1 | BIT0)) /* [5G] Path A and B are both extPA */
        {
            pHalData.TypeAPA = (ushort)(extTypePA_5G_B << 2 | extTypePA_5G_A);
        }

        if ((pHalData.LNAType_2G & (BIT7 | BIT3)) == (BIT7 | BIT3)) /* [2.4G] Path A and B are both extLNA */
        {
            pHalData.TypeGLNA = (ushort)(extTypeLNA_2G_B << 2 | extTypeLNA_2G_A);
        }

        if ((pHalData.LNAType_5G & (BIT7 | BIT3)) == (BIT7 | BIT3)) /* [5G] Path A and B are both extLNA */
        {
            pHalData.TypeALNA = (ushort)(extTypeLNA_5G_B << 2 | extTypeLNA_5G_A);
        }

        RTW_INFO($"pHalData.TypeGPA = 0x{pHalData.TypeGPA}");
        RTW_INFO($"pHalData.TypeAPA = 0x{pHalData.TypeAPA}");
        RTW_INFO($"pHalData.TypeGLNA = 0x{pHalData.TypeGLNA}");
        RTW_INFO($"pHalData.TypeALNA = 0x{pHalData.TypeALNA}");
    }

    private const int EEPROM_PA_TYPE_8812AU = 0xBC;
    private const int EEPROM_LNA_TYPE_2G_8812AU = 0xBD;
    private const int EEPROM_LNA_TYPE_5G_8812AU = 0xBF;

    private static void hal_ReadPAType_8812A(AdapterState adapterState, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        var pHalData = adapterState.HalData;

        if (!AutoloadFail)
        {
            if (registry_priv.AmplifierType_2G == 0)
            {
                /* AUTO */
                pHalData.PAType_2G = PROMContent[EEPROM_PA_TYPE_8812AU];
                pHalData.LNAType_2G = PROMContent[EEPROM_LNA_TYPE_2G_8812AU];
                if (pHalData.PAType_2G == 0xFF)
                {
                    pHalData.PAType_2G = 0;
                }

                if (pHalData.LNAType_2G == 0xFF)
                {
                    pHalData.LNAType_2G = 0;
                }

                pHalData.ExternalPA_2G = ((pHalData.PAType_2G & BIT5) != 0 && (pHalData.PAType_2G & BIT4) != 0);
                pHalData.ExternalLNA_2G = ((pHalData.LNAType_2G & BIT7) != 0 && (pHalData.LNAType_2G & BIT3) != 0);
            }
            else
            {
                pHalData.ExternalPA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_PA) != 0;
                pHalData.ExternalLNA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_LNA) != 0;
            }

            if (registry_priv.AmplifierType_5G == 0)
            {
                /* AUTO */
                pHalData.PAType_5G = PROMContent[EEPROM_PA_TYPE_8812AU];
                pHalData.LNAType_5G = PROMContent[EEPROM_LNA_TYPE_5G_8812AU];
                if (pHalData.PAType_5G == 0xFF)
                {
                    pHalData.PAType_5G = 0;
                }

                if (pHalData.LNAType_5G == 0xFF)
                {
                    pHalData.LNAType_5G = 0;
                }

                pHalData.external_pa_5g = ((pHalData.PAType_5G & BIT1) != 0 && (pHalData.PAType_5G & BIT0) != 0);
                pHalData.external_lna_5g = ((pHalData.LNAType_5G & BIT7) != 0 && (pHalData.LNAType_5G & BIT3) != 0);
            }
            else
            {
                pHalData.external_pa_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_PA_5G) != 0;
                pHalData.external_lna_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }
        else
        {
            pHalData.ExternalPA_2G = false;
            pHalData.external_pa_5g = true;
            pHalData.ExternalLNA_2G = false;
            pHalData.external_lna_5g = true;

            if (registry_priv.AmplifierType_2G == 0)
            {
                /* AUTO */
                pHalData.ExternalPA_2G = false;
                pHalData.ExternalLNA_2G = false;
            }
            else
            {
                pHalData.ExternalPA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_PA) != 0;
                pHalData.ExternalLNA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_LNA) != 0;
            }

            if (registry_priv.AmplifierType_5G == 0)
            {
                /* AUTO */
                pHalData.external_pa_5g = false;
                pHalData.external_lna_5g = false;
            }
            else
            {
                pHalData.external_pa_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_PA_5G) != 0;
                pHalData.external_lna_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }

        RTW_INFO($"pHalData.PAType_2G is 0x{pHalData.PAType_2G:X}, pHalData.ExternalPA_2G = {pHalData.ExternalPA_2G}");
        RTW_INFO(
            $"pHalData.PAType_5G is 0x{pHalData.PAType_5G:X}, pHalData.external_pa_5g = {pHalData.external_pa_5g}");
        RTW_INFO(
            $"pHalData.LNAType_2G is 0x{pHalData.LNAType_2G:X}, pHalData.ExternalLNA_2G = {pHalData.ExternalLNA_2G}");
        RTW_INFO(
            $"pHalData.LNAType_5G is 0x{pHalData.LNAType_5G:X}, pHalData.external_lna_5g = {pHalData.external_lna_5g}");
    }

    private static void Hal_EfuseParseXtal_8812A(AdapterState pAdapterState, u8[] hwinfo, bool AutoLoadFail)
    {
        var pHalData = pAdapterState.HalData;

        if (!AutoLoadFail)
        {
            pHalData.crystal_cap = hwinfo[EEPROM_XTAL_8812];
            if (pHalData.crystal_cap == 0xFF)
                pHalData.crystal_cap = EEPROM_DEFAULT_CRYSTAL_CAP_8812; /* what value should 8812 set? */
        }
        else
        {
            pHalData.crystal_cap = EEPROM_DEFAULT_CRYSTAL_CAP_8812;
        }

        RTW_INFO($"crystal_cap: 0x{pHalData.crystal_cap:X}");
    }

    static void Hal_ReadPROMVersion8812A(AdapterState adapterState, u8[] PROMContent, bool AutoloadFail)
    {
        var pHalData = adapterState.HalData;

        if (AutoloadFail)
        {
            pHalData.EEPROMVersion = EEPROM_DEFAULT_VERSION;
        }
        else
        {
            pHalData.EEPROMVersion = PROMContent[EEPROM_VERSION_8812];


            if (pHalData.EEPROMVersion == 0xFF)
            {
                pHalData.EEPROMVersion = EEPROM_DEFAULT_VERSION;
            }
        }

        RTW_INFO("pHalData.EEPROMVersion is 0x%x", pHalData.EEPROMVersion);
    }

    static void Hal_EfuseParseIDCode8812A(AdapterState padapter, u8[] hwinfo)
    {
        var pHalData = padapter.HalData;
        u16 EEPROMId;


        /* Checl 0x8129 again for making sure autoload status!! */
        EEPROMId = BinaryPrimitives.ReadUInt16LittleEndian(hwinfo.AsSpan(0, 2));
        if (EEPROMId != RTL_EEPROM_ID)
        {
            RTW_INFO($"EEPROM ID(0x{EEPROMId:X}) is invalid!!");
            pHalData.AutoloadFailFlag = true;
        }
        else
            pHalData.AutoloadFailFlag = false;

        RTW_INFO($"EEPROM ID=0x{EEPROMId}");
    }

    static void hal_InitPGData_8812A(AdapterState padapter)
    {
        var pHalData = padapter.HalData;
        u32 i;

        if (false == pHalData.AutoloadFailFlag)
        {
            /* autoload OK. */
            if (is_boot_from_eeprom(padapter))
            {
                /* Read all Content from EEPROM or EFUSE. */
                for (i = 0; i < HWSET_MAX_SIZE_JAGUAR; i += 2)
                {
                    /* value16 = EF2Byte(ReadEEprom(pAdapterState, (u2Byte) (i>>1))); */
                    /* *((u16*)(&PROMContent[i])) = value16; */
                }
            }
            else
            {
                /*  */
                /* 2013/03/08 MH Add for 8812A HW limitation, ROM code can only */
                /*  */
                var efuse_content = new byte[4];
                padapter.Device.efuse_OneByteRead(0x200, out efuse_content[0]);
                padapter.Device.efuse_OneByteRead(0x202, out efuse_content[1]);
                padapter.Device.efuse_OneByteRead(0x204, out efuse_content[2]);
                padapter.Device.efuse_OneByteRead(0x210, out efuse_content[3]);
                if (efuse_content[0] != 0xFF ||
                    efuse_content[1] != 0xFF ||
                    efuse_content[2] != 0xFF ||
                    efuse_content[3] != 0xFF)
                {
                    /* DbgPrint("Disable FW ofl load\n"); */
                    /* pMgntInfo.RegFWOffload = FALSE; */
                }

                /* Read EFUSE real map to shadow. */
                EFUSE_ShadowMapUpdate(padapter, EFUSE_WIFI);
            }
        }
        else
        {
            /* autoload fail */
            /* pHalData.AutoloadFailFlag = true; */
            /*  */
            /* 2013/03/08 MH Add for 8812A HW limitation, ROM code can only */
            /*  */
            var efuse_content = new byte[4];
            padapter.Device.efuse_OneByteRead(0x200, out efuse_content[0]);
            padapter.Device.efuse_OneByteRead(0x202, out efuse_content[1]);
            padapter.Device.efuse_OneByteRead(0x204, out efuse_content[2]);
            padapter.Device.efuse_OneByteRead(0x210, out efuse_content[3]);
            if (efuse_content[0] != 0xFF ||
                efuse_content[1] != 0xFF ||
                efuse_content[2] != 0xFF ||
                efuse_content[3] != 0xFF)
            {
                pHalData.AutoloadFailFlag = false;
            }

            /* update to default value 0xFF */
            if (!is_boot_from_eeprom(padapter))
            {
                EFUSE_ShadowMapUpdate(padapter, EFUSE_WIFI);
            }
        }


        if (IsEfuseTxPowerInfoValid(padapter.HalData.efuse_eeprom_data) == false)
        {
            throw new NotImplementedException("Hal_readPGDataFromConfigFile");
        }

    }

    /// <remarks>
    /// check_phy_efuse_tx_power_info_valid
    /// </remarks>
    private static bool IsEfuseTxPowerInfoValid(byte[] efuseEepromData)
    {
        // Just because single chip support
        UInt16 tx_index_offset = EEPROM_TX_PWR_INX_8812;
        for (int index = 0; index < 11; index++)
        {
            if (efuseEepromData[tx_index_offset + index] == 0xFF)
            {
                return false;
            }
        }

        return true;
    }

    private static void EFUSE_ShadowMapUpdate(AdapterState pAdapterState, byte efuseType)
    {
        var pHalData = pAdapterState.HalData;
        UInt16 mapLen = 0;

        if (pHalData.AutoloadFailFlag == true)
        {
            for (int i = 0; i < pHalData.efuse_eeprom_data.Length; i++)
            {
                pHalData.efuse_eeprom_data[i] = 0xFF;
            }
        }
        else
        {
            Efuse_ReadAllMap(pAdapterState, efuseType, pHalData.efuse_eeprom_data);
        }

        rtw_dump_cur_efuse(pAdapterState);
    }

    static void rtw_dump_cur_efuse(AdapterState padapter)
    {
        var hal_data = padapter.HalData;

        var mapsize = EFUSE_MAP_LEN_JAGUAR;

        var linesCount = mapsize / (4 * 4);
        var lineLength = 4 * 4;
        for (int j = 0; j < linesCount; j++)
        {
            var startIndex = (j * lineLength);
            var builder = new StringBuilder();
            builder.Append($"0x{startIndex:X3}:");
            for (int i = startIndex; i < startIndex + lineLength; i += 4)
            {
                builder.Append(
                    $"  {hal_data.efuse_eeprom_data[i]:X2} {hal_data.efuse_eeprom_data[i + 1]:X2} {hal_data.efuse_eeprom_data[i + 2]:X2} {hal_data.efuse_eeprom_data[i + 3]:X2}");
            }

            Console.WriteLine(builder);
        }
    }

    static void Efuse_ReadAllMap(AdapterState adapterState, byte efuseType, byte[] Efuse)
    {
        EfusePowerSwitch8812A(adapterState, false, true);
        efuse_ReadEFuse(adapterState, efuseType, 0, EFUSE_MAP_LEN_JAGUAR, Efuse);
        EfusePowerSwitch8812A(adapterState, false, false);
    }

    private static void efuse_ReadEFuse(AdapterState adapterState, byte efuseType, UInt16 _offset, UInt16 _size_byte,
        byte[] pbuf)
    {
        if (efuseType == EFUSE_WIFI)
        {
            Hal_EfuseReadEFuse8812A(adapterState, _offset, _size_byte, pbuf);
        }
        else
        {
            throw new NotImplementedException();
            // hal_ReadEFuse_BT(adapterState, _offset, _size_byte, pbuf, bPseudoTest);
        }
    }

    private static void Hal_EfuseReadEFuse8812A(AdapterState adapterState, UInt16 _offset, UInt16 _size_byte, byte[] pbuf)
    {
        byte[] efuseTbl = null;
        byte[] rtemp8 = new byte[1];
        UInt16 eFuse_Addr = 0;
        byte offset, wren;
        UInt16 i, j;
        UInt16[][] eFuseWord = null;
        byte u1temp = 0;

        /*  */
        /* Do NOT excess total size of EFuse table. Added by Roger, 2008.11.10. */
        /*  */
        if ((_offset + _size_byte) > EFUSE_MAP_LEN_JAGUAR)
        {
            /* total E-Fuse table is 512bytes */
            // TODO: RTW_INFO("Hal_EfuseReadEFuse8812A(): Invalid offset(%#x) with read bytes(%#x)!!\n", _offset, _size_byte);
            return;
        }

        efuseTbl = new byte[EFUSE_MAP_LEN_JAGUAR];

        eFuseWord = new ushort[EFUSE_MAX_SECTION_JAGUAR][];
        for (int k = 0; k < eFuseWord.Length; k++)
        {
            eFuseWord[k] = new ushort[EFUSE_MAX_WORD_UNIT];
        }

        /* 0. Refresh efuse init map as all oxFF. */
        for (i = 0; i < EFUSE_MAX_SECTION_JAGUAR; i++)
        {
            for (j = 0; j < EFUSE_MAX_WORD_UNIT; j++)
            {
                eFuseWord[i][j] = 0xFFFF;
            }
        }

        /*  */
        /* 1. Read the first byte to check if efuse is empty!!! */
        /*  */
        /*  */
        adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);
        if (rtemp8[0] != 0xFF)
        {
            /* RTW_INFO("efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8); */
            eFuse_Addr++;
        }
        else
        {
            RTW_INFO($"EFUSE is empty efuse_Addr-{eFuse_Addr} efuse_data={rtemp8}");
            return;
        }


        /*  */
        /* 2. Read real efuse content. Filter PG header and every section data. */
        /*  */
        while ((rtemp8[0] != 0xFF) && (eFuse_Addr < EFUSE_REAL_CONTENT_LEN_JAGUAR))
        {
            /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("efuse_Addr-%d efuse_data=%x\n", eFuse_Addr-1, *rtemp8)); */

            /* Check PG header for section num. */
            if ((rtemp8[0] & 0x1F) == 0x0F)
            {
                /* extended header */
                u1temp = (byte)((rtemp8[0] & 0xE0) >> 5);
                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("extended header u1temp=%x *rtemp&0xE0 0x%x\n", u1temp, *rtemp8 & 0xE0)); */

                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("extended header u1temp=%x\n", u1temp)); */

                adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);

                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("extended header efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8));	 */

                if ((rtemp8[0] & 0x0F) == 0x0F)
                {
                    eFuse_Addr++;
                    adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);

                    if (rtemp8[0] != 0xFF && (eFuse_Addr < EFUSE_REAL_CONTENT_LEN_JAGUAR))
                        eFuse_Addr++;
                    continue;
                }
                else
                {
                    offset = (byte)(((rtemp8[0] & 0xF0) >> 1) | u1temp);
                    wren = (byte)(rtemp8[0] & 0x0F);
                    eFuse_Addr++;
                }
            }
            else
            {
                offset = (byte)((rtemp8[0] >> 4) & 0x0f);
                wren = (byte)(rtemp8[0] & 0x0f);
            }

            if (offset < EFUSE_MAX_SECTION_JAGUAR)
            {
                /* Get word enable value from PG header */
                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Offset-%d Worden=%x\n", offset, wren)); */

                for (i = 0; i < EFUSE_MAX_WORD_UNIT; i++)
                {
                    /* Check word enable condition in the section				 */
                    if (!((wren & 0x01) == 0x01))
                    {
                        /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d\n", eFuse_Addr)); */
                        adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);
                        eFuse_Addr++;
                        eFuseWord[offset][i] = (ushort)(rtemp8[0] & 0xff);


                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;

                        /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d", eFuse_Addr)); */
                        adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);
                        eFuse_Addr++;

                        eFuseWord[offset][i] |= (ushort)(((rtemp8[0]) << 8) & 0xff00);

                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                    }

                    wren >>= 1;

                }
            }
            else
            {
                /* deal with error offset,skip error data		 */
                RTW_PRINT($"invalid offset:0x{offset}");
                for (i = 0; i < EFUSE_MAX_WORD_UNIT; i++)
                {
                    /* Check word enable condition in the section				 */
                    if (!((wren & 0x01) == 0x01))
                    {
                        eFuse_Addr++;
                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                        eFuse_Addr++;
                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                    }
                }
            }

            /* Read next PG header */
            adapterState.Device.ReadEFuseByte(eFuse_Addr, rtemp8);
            /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d rtemp 0x%x\n", eFuse_Addr, *rtemp8)); */

            if (rtemp8[0] != 0xFF && (eFuse_Addr < EFUSE_REAL_CONTENT_LEN_JAGUAR))
            {
                eFuse_Addr++;
            }
        }

        /*  */
        /* 3. Collect 16 sections and 4 word unit into Efuse map. */
        /*  */
        for (i = 0; i < EFUSE_MAX_SECTION_JAGUAR; i++)
        {
            for (j = 0; j < EFUSE_MAX_WORD_UNIT; j++)
            {
                efuseTbl[(i * 8) + (j * 2)] = (byte)(eFuseWord[i][j] & 0xff);
                efuseTbl[(i * 8) + ((j * 2) + 1)] = (byte)((eFuseWord[i][j] >> 8) & 0xff);
            }
        }


        /*  */
        /* 4. Copy from Efuse map to output pointer memory!!! */
        /*  */
        for (i = 0; i < _size_byte; i++)
        {
            pbuf[i] = efuseTbl[_offset + i];
        }

        /*  */
        /* 5. Calculate Efuse utilization. */
        /*  */
        // TODO: SetHwReg8812AU(HW_VARIABLES.HW_VAR_EFUSE_BYTES, (u8*)&eFuse_Addr);
        RTW_INFO($"Hal_EfuseReadEFuse8812A: eFuse_Addr offset(0x{eFuse_Addr:X}) !!");
    }

    private static void EfusePowerSwitch8812A(AdapterState adapterState, bool bWrite, bool pwrState)
    {
        UInt16 tmpV16;
        const byte EFUSE_ACCESS_ON_JAGUAR = 0x69;
        const byte EFUSE_ACCESS_OFF_JAGUAR = 0x00;
        if (pwrState)
        {
            adapterState.Device.rtw_write8(REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_ON_JAGUAR);

            /* 1.2V Power: From VDDON with Power Cut(0x0000h[15]), defualt valid */
            tmpV16 = adapterState.Device.rtw_read16(REG_SYS_ISO_CTRL);
            if (!((tmpV16 & SysIsoCtrlBits.PWC_EV12V) == SysIsoCtrlBits.PWC_EV12V))
            {
                tmpV16 |= SysIsoCtrlBits.PWC_EV12V;
                /* Write16(pAdapterState,REG_SYS_ISO_CTRL,tmpV16); */
            }

            /* Reset: 0x0000h[28], default valid */
            tmpV16 = adapterState.Device.rtw_read16(REG_SYS_FUNC_EN);
            if (!((tmpV16 & SysFuncEnBits.FEN_ELDR) == SysFuncEnBits.FEN_ELDR))
            {
                tmpV16 |= SysFuncEnBits.FEN_ELDR;
                adapterState.Device.rtw_write16(REG_SYS_FUNC_EN, tmpV16);
            }

            /* Clock: Gated(0x0008h[5]) 8M(0x0008h[1]) clock from ANA, default valid */
            tmpV16 = adapterState.Device.rtw_read16(REG_SYS_CLKR);
            if ((!((tmpV16 & SysClkrBits.LOADER_CLK_EN) == SysClkrBits.LOADER_CLK_EN)) ||
                (!((tmpV16 & SysClkrBits.ANA8M) == SysClkrBits.ANA8M)))
            {
                tmpV16 |= (SysClkrBits.LOADER_CLK_EN | SysClkrBits.ANA8M);
                adapterState.Device.rtw_write16(REG_SYS_CLKR, tmpV16);
            }

            if (bWrite)
            {
                /* Enable LDO 2.5V before read/write action */
                var tempval = adapterState.Device.rtw_read8(REG_EFUSE_TEST + 3);
                //tempval &= ~(BIT3 | BIT4 | BIT5 | BIT6);
                //tempval &= (0b1111_0111 & 0b1110_1111 & 0b1101_1111 & 0b1011_1111);
                tempval &= 0b1000_0111;
                tempval |= (VoltageValues.VOLTAGE_V25 << 3);
                tempval |= 0b1000_0000;
                adapterState.Device.rtw_write8(REG_EFUSE_TEST + 3, tempval);
            }
        }
        else
        {
            adapterState.Device.rtw_write8(REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_OFF_JAGUAR);

            if (bWrite)
            {
                /* Disable LDO 2.5V after read/write action */
                var tempval = adapterState.Device.rtw_read8(REG_EFUSE_TEST + 3);
                adapterState.Device.rtw_write8(REG_EFUSE_TEST + 3, (byte)(tempval & 0x7F));
            }
        }

    }

    /// <remarks>
    /// _ConfigChipOutEP_8812
    /// </remarks>
    static (TxSele OutEpQueueSel, byte OutEpNumber) GetChipOutEP8812(u8 NumOutPipe)
    {
        TxSele OutEpQueueSel = 0;
        byte OutEpNumber = 0;

        switch (NumOutPipe)
        {
            case 4:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ | TxSele.TX_SELE_EQ;
                OutEpNumber = 4;
                break;
            case 3:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ;
                OutEpNumber = 3;
                break;
            case 2:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_NQ;
                OutEpNumber = 2;
                break;
            case 1:
                OutEpQueueSel = TxSele.TX_SELE_HQ;
                OutEpNumber = 1;
                break;
            default:
                break;
        }

        RTW_INFO($"OutEpQueueSel({OutEpQueueSel}), OutEpNumber({OutEpNumber})");

        return (OutEpQueueSel,  OutEpNumber);
    }

    private static bool is_boot_from_eeprom(AdapterState adapterState) => (adapterState.HalData.EepromOrEfuse);










    public static UInt32 LE_BITS_TO_4BYTE(Span<byte> __pStart, int __BitOffset, int __BitLen)
    {
        return ((LE_P4BYTE_TO_HOST_4BYTE(__pStart) >> (__BitOffset)) & BIT_LEN_MASK_32(__BitLen));
    }

    private static UInt32 LE_P4BYTE_TO_HOST_4BYTE(Span<byte> __pStart)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(__pStart);
    }

    private static UInt32 BIT_LEN_MASK_32(int __BitLen) => ((u32)(0xFFFFFFFF >> (32 - (__BitLen))));

    public static void read_chip_version_8812a(AdapterState adapterState)
    {
        u32 value32 = adapterState.Device.rtw_read32(REG_SYS_CFG);
        RTW_INFO($"read_chip_version_8812a SYS_CFG(0x{REG_SYS_CFG:X})=0x{value32:X8}");

        var pHalData = adapterState.HalData;
        pHalData.version_id.RFType = HalRFType.RF_TYPE_2T2R; /* RF_2T2R; */

        if (registry_priv.special_rf_path == 1)
        {
            pHalData.version_id.RFType = HalRFType.RF_TYPE_1T1R; /* RF_1T1R; */
        }

        pHalData.version_id.CUTVersion = (CutVersion)((value32 & CHIP_VER_RTL_MASK) >> CHIP_VER_RTL_SHIFT); /* IC version (CUT) */
        pHalData.version_id.CUTVersion += 1;

        /* For multi-function consideration. Added by Roger, 2010.10.06. */
        pHalData.MultiFunc = RT_MULTI_FUNC.RT_MULTI_FUNC_NONE;
        value32 = adapterState.Device.rtw_read32(REG_MULTI_FUNC_CTRL);
        pHalData.MultiFunc |= ((value32 & WL_FUNC_EN) != 0 ? RT_MULTI_FUNC.RT_MULTI_FUNC_WIFI : 0);
        pHalData.MultiFunc |= ((value32 & BT_FUNC_EN) != 0 ? RT_MULTI_FUNC.RT_MULTI_FUNC_BT : 0);

        var (rfType, numTotalRfPath) = GetRfType(pHalData.version_id);
        pHalData.rf_type = rfType;
        pHalData.NumTotalRFPath = numTotalRfPath;
        RTW_INFO($"rtw_hal_config_rftype RF_Type is {pHalData.rf_type} TotalTxPath is {pHalData.NumTotalRFPath}");
        //dump_chip_info(pHalData.version_id);
    }

    static (RfType rf_type, u8 NumTotalRFPath) GetRfType(HAL_VERSION version)
    {
        if (IS_1T1R(version))
        {
            return (RfType.RF_1T1R, 1);
        }

        if (IS_2T2R(version))
        {
            return (RfType.RF_2T2R, 2);
        }

        if (IS_1T2R(version))
        {
            return (RfType.RF_1T2R, 2);
        }

        if (IS_3T3R(version))
        {
            return (RfType.RF_3T3R, 3);
        }

        if (IS_4T4R(version))
        {
            return (RfType.RF_4T4R, 4);
        }


        return (RfType.RF_1T1R, 1);
    }






}