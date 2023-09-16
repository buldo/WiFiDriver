using System.Buffers.Binary;
using System.Text;

using Rtl8812auNet.Rtl8812au.Enumerations;
using Rtl8812auNet.Rtl8812au.Models;

namespace Rtl8812auNet.Rtl8812au.Modules;

public class EepromManager
{
    private const ushort EFUSE_MAP_LEN_JAGUAR = 512;
    private const byte EFUSE_WIFI = 0;
    private const UInt32 EFUSE_MAX_SECTION_JAGUAR = 64;
    private const UInt32 EFUSE_MAX_WORD_UNIT = 4;
    private const UInt32 EFUSE_REAL_CONTENT_LEN_JAGUAR = 512;
    private const UInt16 RTL_EEPROM_ID = 0x8129;
    private const byte EEPROM_DEFAULT_VERSION = 0;
    private const byte EEPROM_VERSION_8812 = 0xC4;
    private const UInt32 HWSET_MAX_SIZE_JAGUAR = 512;
    private const UInt16 EEPROM_TX_PWR_INX_8812 = 0x10;
    private const byte EEPROM_RF_BOARD_OPTION_8812 = 0xC1;
    private const byte EEPROM_XTAL_8812 = 0xB9;
    private const byte EEPROM_DEFAULT_CRYSTAL_CAP_8812 = 0x20;
    private const byte EEPROM_Default_ThermalMeter_8812 = 0x18;
    private const int EEPROM_PA_TYPE_8812AU = 0xBC;
    private const int EEPROM_LNA_TYPE_2G_8812AU = 0xBD;
    private const int EEPROM_LNA_TYPE_5G_8812AU = 0xBF;
    private const byte EEPROM_RFE_OPTION_8812 = 0xCA;

    private readonly RtlUsbAdapter _device;
    private readonly byte[] efuse_eeprom_data = new byte[1024]; /*92C:256bytes, 88E:512bytes, we use union set (512bytes)*/
    private readonly byte EEPROMVersion;
    private readonly byte EEPROMRegulatory;
    private bool EEPROMBluetoothCoexist;
    private byte eeprom_thermal_meter;
    private byte PAType_2G;
    private byte PAType_5G;
    private byte LNAType_2G;
    private byte LNAType_5G;
    private bool external_pa_5g;
    private bool external_lna_5g;

    public EepromManager(RtlUsbAdapter device)
    {
        _device = device;
        (Version, RfType, NumTotalRfPath) = read_chip_version_8812a(device);

        /* Initialize general global value */
        // Looks like WTF
        if (RfType == RfType.RF_1T1R)
        {
            NumTotalRfPath = 1;
        }
        else
        {
            NumTotalRfPath = 2;
        }

        hal_InitPGData_8812A();
        Hal_EfuseParseIDCode8812A();
        EEPROMVersion = Hal_ReadPROMVersion8812A(_device, efuse_eeprom_data);
        EEPROMRegulatory = Hal_ReadTxPowerInfo8812A(_device, efuse_eeprom_data);

        /*  */
        /* Read Bluetooth co-exist and initialize */
        /*  */
        Hal_EfuseParseBTCoexistInfo8812A();

        Hal_EfuseParseXtal_8812A();
        Hal_ReadThermalMeter_8812A();

        Hal_ReadAmplifierType_8812A();
        Hal_ReadRFEType_8812A();

        // EEPROMUsbSwitch not used in our code
        //pHalData.EEPROMUsbSwitch = ReadUsbModeSwitch8812AU(pHalData.efuse_eeprom_data, pHalData.AutoloadFailFlag);
        //RTW_INFO("Usb Switch: %d", pHalData.EEPROMUsbSwitch);

        /* 2013/04/15 MH Add for different board type recognize. */
        hal_ReadUsbType_8812AU();
    }

    public HalVersion Version { get; }

    public RfType RfType { get; private set; }

    public byte NumTotalRfPath { get; }

    public byte crystal_cap { get; private set; }

    public UInt16 TypeGPA { get; private set; }

    public UInt16 TypeAPA { get; private set; }

    public UInt16 TypeGLNA { get; private set; }

    public UInt16 TypeALNA { get; private set; }

    public bool ExternalPA_2G { get; private set; }
    public bool ExternalLNA_2G { get; private set; }

    public UInt16 rfe_type { get; private set; }

    public byte GetBoardType()
    {
        /* 1 ======= BoardType: ODM_CMNINFO_BOARD_TYPE ======= */
        uint odm_board_type = ODM_BOARD_DEFAULT;

        if (ExternalLNA_2G)
        {
            odm_board_type |= ODM_BOARD_EXT_LNA;
        }

        if (external_lna_5g)
        {
            odm_board_type |= ODM_BOARD_EXT_LNA_5G;
        }

        if (ExternalPA_2G)
        {
            odm_board_type |= ODM_BOARD_EXT_PA;
        }

        if (external_pa_5g)
        {
            odm_board_type |= ODM_BOARD_EXT_PA_5G;
        }

        if (EEPROMBluetoothCoexist)
        {
            odm_board_type |= ODM_BOARD_BT;
        }

        return (byte)odm_board_type;
    }

    public void efuse_ShadowRead1Byte(UInt16 Offset, out byte Value)
    {
        Value = efuse_eeprom_data[Offset];
    }

    private void Hal_ReadRFEType_8812A()
    {
        if (!_device.AutoloadFailFlag)
        {
            if ((registry_priv.RFE_Type != 64) || 0xFF == efuse_eeprom_data[EEPROM_RFE_OPTION_8812])
            {
                if (registry_priv.RFE_Type != 64)
                {
                    rfe_type = registry_priv.RFE_Type;
                }
                else
                {
                    rfe_type = 0;
                }

            }
            else if ((efuse_eeprom_data[EEPROM_RFE_OPTION_8812] & BIT7) != 0)
            {
                if (external_lna_5g == true || external_lna_5g == null)
                {
                    if (external_pa_5g == true || external_pa_5g == null)
                    {
                        if (ExternalLNA_2G && ExternalPA_2G)
                        {
                            rfe_type = 3;
                        }
                        else
                        {
                            rfe_type = 0;
                        }
                    }
                    else
                    {
                        rfe_type = 2;
                    }
                }
                else
                {
                    rfe_type = 4;
                }
            }
            else
            {
                rfe_type = (ushort)(efuse_eeprom_data[EEPROM_RFE_OPTION_8812] & 0x3F);

                /* 2013/03/19 MH Due to othe customer already use incorrect EFUSE map */
                /* to for their product. We need to add workaround to prevent to modify */
                /* spec and notify all customer to revise the IC 0xca content. After */
                /* discussing with Willis an YN, revise driver code to prevent. */
                if (rfe_type == 4 &&
                    (external_pa_5g == true || ExternalPA_2G == true || external_lna_5g == true || ExternalLNA_2G == true))
                {
                    rfe_type = 0;
                }
            }
        }
        else
        {
            if (registry_priv.RFE_Type != 64)
            {
                rfe_type = registry_priv.RFE_Type;
            }
            else
            {
                rfe_type = 0;
            }
        }

        RTW_INFO($"RFE Type: 0x{rfe_type:X}");
    }

    private void hal_ReadUsbType_8812AU()
    {
        /* if (IS_HARDWARE_TYPE_8812AU(adapterState) && adapterState.UsbModeMechanism.RegForcedUsbMode == 5) */
        {
           byte reg_tmp, i, j, antenna = 0, wmode = 0;
            /* Read anenna type from EFUSE 1019/1018 */
            for (i = 0; i < 2; i++)
            {
                /*
                  Check efuse address 1019
                  Check efuse address 1018
                */
                _device.efuse_OneByteRead((ushort)(1019 - i), out reg_tmp);
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
                _device.efuse_OneByteRead((ushort)(1021 - i), out reg_tmp);

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
                RfType = RfType.RF_1T1R;
                /* UsbModeSwitch_SetUsbModeMechOn(adapterState, FALSE); */
                /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VL; */
                RTW_INFO("%s(): EFUSE_HIDDEN_812AU_VL\n");
            }
            else if (antenna == 2)
            {
                if (wmode == 3)
                {
                    if (efuse_eeprom_data[EEPROM_USB_MODE_8812] == 0x2)
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

    private void Hal_ReadAmplifierType_8812A()
    {
        byte extTypePA_2G_A = (byte)((efuse_eeprom_data[0xBD] & BIT2) >> 2); /* 0xBD[2] */
        byte extTypePA_2G_B = (byte)((efuse_eeprom_data[0xBD] & BIT6) >> 6); /* 0xBD[6] */
        byte extTypePA_5G_A = (byte)((efuse_eeprom_data[0xBF] & BIT2) >> 2); /* 0xBF[2] */
        byte extTypePA_5G_B = (byte)((efuse_eeprom_data[0xBF] & BIT6) >> 6); /* 0xBF[6] */
        byte extTypeLNA_2G_A = (byte)((efuse_eeprom_data[0xBD] & (BIT1 | BIT0)) >> 0); /* 0xBD[1:0] */
        byte extTypeLNA_2G_B = (byte)((efuse_eeprom_data[0xBD] & (BIT5 | BIT4)) >> 4); /* 0xBD[5:4] */
        byte extTypeLNA_5G_A = (byte)((efuse_eeprom_data[0xBF] & (BIT1 | BIT0)) >> 0); /* 0xBF[1:0] */
        byte extTypeLNA_5G_B = (byte)((efuse_eeprom_data[0xBF] & (BIT5 | BIT4)) >> 4); /* 0xBF[5:4] */

        hal_ReadPAType_8812A();

        if ((PAType_2G & (BIT5 | BIT4)) == (BIT5 | BIT4)) /* [2.4G] Path A and B are both extPA */
        {
            TypeGPA = (ushort)(extTypePA_2G_B << 2 | extTypePA_2G_A);
        }

        if ((PAType_5G & (BIT1 | BIT0)) == (BIT1 | BIT0)) /* [5G] Path A and B are both extPA */
        {
            TypeAPA = (ushort)(extTypePA_5G_B << 2 | extTypePA_5G_A);
        }

        if ((LNAType_2G & (BIT7 | BIT3)) == (BIT7 | BIT3)) /* [2.4G] Path A and B are both extLNA */
        {
            TypeGLNA = (ushort)(extTypeLNA_2G_B << 2 | extTypeLNA_2G_A);
        }

        if ((LNAType_5G & (BIT7 | BIT3)) == (BIT7 | BIT3)) /* [5G] Path A and B are both extLNA */
        {
            TypeALNA = (ushort)(extTypeLNA_5G_B << 2 | extTypeLNA_5G_A);
        }

        RTW_INFO($"pHalData.TypeGPA = 0x{TypeGPA:X}");
        RTW_INFO($"pHalData.TypeAPA = 0x{TypeAPA:X}");
        RTW_INFO($"pHalData.TypeGLNA = 0x{TypeGLNA:X}");
        RTW_INFO($"pHalData.TypeALNA = 0x{TypeALNA:X}");
    }

    private void hal_ReadPAType_8812A()
    {
        if (!_device.AutoloadFailFlag)
        {
            if (registry_priv.AmplifierType_2G == 0)
            {
                /* AUTO */
                PAType_2G = efuse_eeprom_data[EEPROM_PA_TYPE_8812AU];
                LNAType_2G = efuse_eeprom_data[EEPROM_LNA_TYPE_2G_8812AU];
                if (PAType_2G == 0xFF)
                {
                    PAType_2G = 0;
                }

                if (LNAType_2G == 0xFF)
                {
                    LNAType_2G = 0;
                }

                ExternalPA_2G = ((PAType_2G & BIT5) != 0 && (PAType_2G & BIT4) != 0);
                ExternalLNA_2G = ((LNAType_2G & BIT7) != 0 && (LNAType_2G & BIT3) != 0);
            }
            else
            {
                ExternalPA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_PA) != 0;
                ExternalLNA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_LNA) != 0;
            }

            if (registry_priv.AmplifierType_5G == 0)
            {
                /* AUTO */
                PAType_5G = efuse_eeprom_data[EEPROM_PA_TYPE_8812AU];
                LNAType_5G = efuse_eeprom_data[EEPROM_LNA_TYPE_5G_8812AU];
                if (PAType_5G == 0xFF)
                {
                    PAType_5G = 0;
                }

                if (LNAType_5G == 0xFF)
                {
                    LNAType_5G = 0;
                }

                external_pa_5g = ((PAType_5G & BIT1) != 0 && (PAType_5G & BIT0) != 0);
                external_lna_5g = ((LNAType_5G & BIT7) != 0 && (LNAType_5G & BIT3) != 0);
            }
            else
            {
                external_pa_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_PA_5G) != 0;
                external_lna_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }
        else
        {
            ExternalPA_2G = false;
            external_pa_5g = true;
            ExternalLNA_2G = false;
            external_lna_5g = true;

            if (registry_priv.AmplifierType_2G == 0)
            {
                /* AUTO */
                ExternalPA_2G = false;
                ExternalLNA_2G = false;
            }
            else
            {
                ExternalPA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_PA) != 0;
                ExternalLNA_2G = (registry_priv.AmplifierType_2G & ODM_BOARD_EXT_LNA) != 0;
            }

            if (registry_priv.AmplifierType_5G == 0)
            {
                /* AUTO */
                external_pa_5g = false;
                external_lna_5g = false;
            }
            else
            {
                external_pa_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_PA_5G) != 0;
                external_lna_5g = (registry_priv.AmplifierType_5G & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }

        RTW_INFO($"pHalData.PAType_2G is 0x{PAType_2G:X}, pHalData.ExternalPA_2G = {ExternalPA_2G}");
        RTW_INFO($"pHalData.PAType_5G is 0x{PAType_5G:X}, pHalData.external_pa_5g = {external_pa_5g}");
        RTW_INFO($"pHalData.LNAType_2G is 0x{LNAType_2G:X}, pHalData.ExternalLNA_2G = {ExternalLNA_2G}");
        RTW_INFO($"pHalData.LNAType_5G is 0x{LNAType_5G:X}, pHalData.external_lna_5g = {external_lna_5g}");
    }

    private void Hal_ReadThermalMeter_8812A()
    {
        /*  */
        /* ThermalMeter from EEPROM */
        /*  */
        if (!_device.AutoloadFailFlag)
        {
            eeprom_thermal_meter = efuse_eeprom_data[EEPROM_THERMAL_METER_8812];
        }
        else
        {
            eeprom_thermal_meter = EEPROM_Default_ThermalMeter_8812;
        }
        /* pHalData.eeprom_thermal_meter = (tempval&0x1f);	 */ /* [4:0] */

        if (eeprom_thermal_meter == 0xff || _device.AutoloadFailFlag)
        {
            eeprom_thermal_meter = 0xFF;
        }

        /* pHalData.ThermalMeter[0] = pHalData.eeprom_thermal_meter;	 */
        RTW_INFO($"ThermalMeter = 0x{eeprom_thermal_meter:X}");
    }

    private void Hal_EfuseParseXtal_8812A()
    {
        if (!_device.AutoloadFailFlag)
        {
            crystal_cap = efuse_eeprom_data[EEPROM_XTAL_8812];
            if (crystal_cap == 0xFF)
            {
                crystal_cap = EEPROM_DEFAULT_CRYSTAL_CAP_8812; /* what value should 8812 set? */
            }
        }
        else
        {
            crystal_cap = EEPROM_DEFAULT_CRYSTAL_CAP_8812;
        }

        RTW_INFO($"crystal_cap: 0x{crystal_cap:X}");
    }

    private void Hal_EfuseParseBTCoexistInfo8812A()
    {
        if (!_device.AutoloadFailFlag)
        {
            var tmp_u8 = efuse_eeprom_data[EEPROM_RF_BOARD_OPTION_8812];
            if (((tmp_u8 & 0xe0) >> 5) == 0x1) /* [7:5] */
            {
                EEPROMBluetoothCoexist = true;
            }
            else
            {
                EEPROMBluetoothCoexist = false;
            }
        }
        else
        {
            EEPROMBluetoothCoexist = false;
        }
    }

    private static byte Hal_ReadTxPowerInfo8812A(RtlUsbAdapter device, byte[] efuse_eeprom_data)
    {
        byte EEPROMRegulatory;
        /* 2010/10/19 MH Add Regulator recognize for CU. */
        if (!device.AutoloadFailFlag)
        {

            if (efuse_eeprom_data[EEPROM_RF_BOARD_OPTION_8812] == 0xFF)
            {
                EEPROMRegulatory = (EEPROM_DEFAULT_BOARD_OPTION & 0x7); /* bit0~2 */
            }
            else
            {
                EEPROMRegulatory = (byte)(efuse_eeprom_data[EEPROM_RF_BOARD_OPTION_8812] & 0x7); /* bit0~2 */
            }

        }
        else
        {
            EEPROMRegulatory = 0;
        }

        RTW_INFO("EEPROMRegulatory = 0x%x", EEPROMRegulatory);

        return EEPROMRegulatory;
    }

    private void hal_InitPGData_8812A()
    {
        UInt32 i;

        if (false == _device.AutoloadFailFlag)
        {
            /* autoload OK. */
            if (_device.EepromOrEfuse)
            {
                /* Read all Content from EEPROM or EFUSE. */
                for (i = 0; i < HWSET_MAX_SIZE_JAGUAR; i += 2)
                {
                    /* value16 = EF2Byte(ReadEEprom(pAdapterState, (u2Byte) (i>>1))); */
                    /* *((UInt16*)(&PROMContent[i])) = value16; */
                }
            }
            else
            {
                /*  */
                /* 2013/03/08 MH Add for 8812A HW limitation, ROM code can only */
                /*  */
                var efuse_content = new byte[4];
                _device.efuse_OneByteRead(0x200, out efuse_content[0]);
                _device.efuse_OneByteRead(0x202, out efuse_content[1]);
                _device.efuse_OneByteRead(0x204, out efuse_content[2]);
                _device.efuse_OneByteRead(0x210, out efuse_content[3]);
                if (efuse_content[0] != 0xFF ||
                    efuse_content[1] != 0xFF ||
                    efuse_content[2] != 0xFF ||
                    efuse_content[3] != 0xFF)
                {
                    /* DbgPrint("Disable FW ofl load\n"); */
                    /* pMgntInfo.RegFWOffload = FALSE; */
                }

                /* Read EFUSE real map to shadow. */
                EFUSE_ShadowMapUpdate(EFUSE_WIFI);
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
            _device.efuse_OneByteRead(0x200, out efuse_content[0]);
            _device.efuse_OneByteRead(0x202, out efuse_content[1]);
            _device.efuse_OneByteRead(0x204, out efuse_content[2]);
            _device.efuse_OneByteRead(0x210, out efuse_content[3]);
            if (efuse_content[0] != 0xFF ||
                efuse_content[1] != 0xFF ||
                efuse_content[2] != 0xFF ||
                efuse_content[3] != 0xFF)
            {
                _device.AutoloadFailFlag = false;
            }

            /* update to default value 0xFF */
            if (!_device.EepromOrEfuse)
            {
                EFUSE_ShadowMapUpdate(EFUSE_WIFI);
            }
        }


        if (IsEfuseTxPowerInfoValid(efuse_eeprom_data) == false)
        {
            throw new NotImplementedException("Hal_readPGDataFromConfigFile");
        }
    }

    private static (HalVersion version_id, RfType rf_type, byte numTotalRfPath) read_chip_version_8812a(RtlUsbAdapter device)
    {
        UInt32 value32 = device.rtw_read32(REG_SYS_CFG);
        RTW_INFO($"read_chip_version_8812a SYS_CFG(0x{REG_SYS_CFG:X})=0x{value32:X8}");

        var versionId = new HalVersion
        {
            RFType = HalRFType.RF_TYPE_2T2R /* RF_2T2R; */
        };

        if (registry_priv.special_rf_path == 1)
        {
            versionId.RFType = HalRFType.RF_TYPE_1T1R; /* RF_1T1R; */
        }

        versionId.CUTVersion = (CutVersion)((value32 & CHIP_VER_RTL_MASK) >> CHIP_VER_RTL_SHIFT); /* IC version (CUT) */
        versionId.CUTVersion += 1;

        /* For multi-function consideration. Added by Roger, 2010.10.06. */

        var (rfType, numTotalRfPath) = versionId.GetRfType();

        RTW_INFO($"rtw_hal_config_rftype RF_Type is {rfType} TotalTxPath is {numTotalRfPath}");

        return (versionId, rfType, numTotalRfPath);
    }

    private void Hal_EfuseParseIDCode8812A()
    {
        UInt16 EEPROMId;


        /* Checl 0x8129 again for making sure autoload status!! */
        EEPROMId = BinaryPrimitives.ReadUInt16LittleEndian(efuse_eeprom_data.AsSpan(0, 2));
        if (EEPROMId != RTL_EEPROM_ID)
        {
            RTW_INFO($"EEPROM ID(0x{EEPROMId:X}) is invalid!!");
            _device.AutoloadFailFlag = true;
        }
        else
        {
            _device.AutoloadFailFlag = false;
        }

        RTW_INFO($"EEPROM ID=0x{EEPROMId}");
    }

    private static byte Hal_ReadPROMVersion8812A(RtlUsbAdapter device, byte[] efuse_eeprom_data)
    {
        byte EEPROMVersion;
        if (device.AutoloadFailFlag)
        {
            EEPROMVersion = EEPROM_DEFAULT_VERSION;
        }
        else
        {
            EEPROMVersion = efuse_eeprom_data[EEPROM_VERSION_8812];


            if (EEPROMVersion == 0xFF)
            {
                EEPROMVersion = EEPROM_DEFAULT_VERSION;
            }
        }

        RTW_INFO("pHalData.EEPROMVersion is 0x%x", EEPROMVersion);
        return EEPROMVersion;
    }

    private void EFUSE_ShadowMapUpdate(byte efuseType)
    {
        if (_device.AutoloadFailFlag)
        {
            for (int i = 0; i < efuse_eeprom_data.Length; i++)
            {
                efuse_eeprom_data[i] = 0xFF;
            }
        }
        else
        {
            Efuse_ReadAllMap(efuseType, efuse_eeprom_data);
        }

        rtw_dump_cur_efuse();
    }

    private void Efuse_ReadAllMap(byte efuseType, byte[] Efuse)
    {
        EfusePowerSwitch8812A(false, true);
        efuse_ReadEFuse(efuseType, 0, EFUSE_MAP_LEN_JAGUAR, Efuse);
        EfusePowerSwitch8812A(false, false);
    }

    private void rtw_dump_cur_efuse()
    {
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
                    $"  {efuse_eeprom_data[i]:X2} {efuse_eeprom_data[i + 1]:X2} {efuse_eeprom_data[i + 2]:X2} {efuse_eeprom_data[i + 3]:X2}");
            }

            Console.WriteLine(builder);
        }
    }

    private void EfusePowerSwitch8812A(bool bWrite, bool pwrState)
    {
        UInt16 tmpV16;
        const byte EFUSE_ACCESS_ON_JAGUAR = 0x69;
        const byte EFUSE_ACCESS_OFF_JAGUAR = 0x00;
        if (pwrState)
        {
            _device.rtw_write8(REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_ON_JAGUAR);

            /* 1.2V Power: From VDDON with Power Cut(0x0000h[15]), defualt valid */
            tmpV16 = _device.rtw_read16(REG_SYS_ISO_CTRL);
            if (!((tmpV16 & SysIsoCtrlBits.PWC_EV12V) == SysIsoCtrlBits.PWC_EV12V))
            {
                tmpV16 |= SysIsoCtrlBits.PWC_EV12V;
                /* Write16(pAdapterState,REG_SYS_ISO_CTRL,tmpV16); */
            }

            /* Reset: 0x0000h[28], default valid */
            tmpV16 = _device.rtw_read16(REG_SYS_FUNC_EN);
            if (!((tmpV16 & SysFuncEnBits.FEN_ELDR) == SysFuncEnBits.FEN_ELDR))
            {
                tmpV16 |= SysFuncEnBits.FEN_ELDR;
                _device.rtw_write16(REG_SYS_FUNC_EN, tmpV16);
            }

            /* Clock: Gated(0x0008h[5]) 8M(0x0008h[1]) clock from ANA, default valid */
            tmpV16 = _device.rtw_read16(REG_SYS_CLKR);
            if ((!((tmpV16 & SysClkrBits.LOADER_CLK_EN) == SysClkrBits.LOADER_CLK_EN)) ||
                (!((tmpV16 & SysClkrBits.ANA8M) == SysClkrBits.ANA8M)))
            {
                tmpV16 |= (SysClkrBits.LOADER_CLK_EN | SysClkrBits.ANA8M);
                _device.rtw_write16(REG_SYS_CLKR, tmpV16);
            }

            if (bWrite)
            {
                /* Enable LDO 2.5V before read/write action */
                var tempval = _device.rtw_read8(REG_EFUSE_TEST + 3);
                //tempval &= ~(BIT3 | BIT4 | BIT5 | BIT6);
                //tempval &= (0b1111_0111 & 0b1110_1111 & 0b1101_1111 & 0b1011_1111);
                tempval &= 0b1000_0111;
                tempval |= (VoltageValues.VOLTAGE_V25 << 3);
                tempval |= 0b1000_0000;
                _device.rtw_write8(REG_EFUSE_TEST + 3, tempval);
            }
        }
        else
        {
            _device.rtw_write8(REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_OFF_JAGUAR);

            if (bWrite)
            {
                /* Disable LDO 2.5V after read/write action */
                var tempval = _device.rtw_read8(REG_EFUSE_TEST + 3);
                _device.rtw_write8(REG_EFUSE_TEST + 3, (byte)(tempval & 0x7F));
            }
        }
    }

    private void efuse_ReadEFuse(byte efuseType, UInt16 _offset, UInt16 _size_byte,
        byte[] pbuf)
    {
        if (efuseType == EFUSE_WIFI)
        {
            Hal_EfuseReadEFuse8812A(_offset, _size_byte, pbuf);
        }
        else
        {
            throw new NotImplementedException();
            // hal_ReadEFuse_BT(adapterState, _offset, _size_byte, pbuf, bPseudoTest);
        }
    }

    private void Hal_EfuseReadEFuse8812A(UInt16 _offset, UInt16 _size_byte, byte[] pbuf)
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
        _device.ReadEFuseByte(eFuse_Addr, rtemp8);
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

                _device.ReadEFuseByte(eFuse_Addr, rtemp8);

                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("extended header efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8));	 */

                if ((rtemp8[0] & 0x0F) == 0x0F)
                {
                    eFuse_Addr++;
                    _device.ReadEFuseByte(eFuse_Addr, rtemp8);

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
                        _device.ReadEFuseByte(eFuse_Addr, rtemp8);
                        eFuse_Addr++;
                        eFuseWord[offset][i] = (ushort)(rtemp8[0] & 0xff);


                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;

                        /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d", eFuse_Addr)); */
                        _device.ReadEFuseByte(eFuse_Addr, rtemp8);
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
            _device.ReadEFuseByte(eFuse_Addr, rtemp8);
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
        // TODO: SetHwReg8812AU(HW_VARIABLES.HW_VAR_EFUSE_BYTES, (byte*)&eFuse_Addr);
        RTW_INFO($"Hal_EfuseReadEFuse8812A: eFuse_Addr offset(0x{eFuse_Addr:X}) !!");
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
}