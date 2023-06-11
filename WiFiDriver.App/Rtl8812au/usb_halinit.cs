using Microsoft.Win32;
using System.Buffers.Binary;
using System.IO;
using System.Xml.Linq;

using WiFiDriver.App.Rtl8812au;

namespace WiFiDriver.App.Rtl8812au;

public static class usb_halinit
{
    const byte EFUSE_ACCESS_ON_JAGUAR = 0x69;
    const byte EFUSE_ACCESS_OFF_JAGUAR = 0x00;
    private static readonly byte BOOT_FROM_EEPROM = BIT(4);
    private static readonly byte EEPROM_EN = BIT(5);
    private const UInt32 HWSET_MAX_SIZE_JAGUAR = 512;
    private const byte EFUSE_WIFI = 0;
    private const UInt16 EEPROM_TX_PWR_INX_8812 = 0x10;
    private const ushort EFUSE_MAP_LEN_JAGUAR = 512;
    private const UInt32 EFUSE_MAX_SECTION_JAGUAR = 64;
    private const UInt32 EFUSE_MAX_WORD_UNIT_JAGUAR = 4;
    private const UInt32 EFUSE_MAX_WORD_UNIT = 4;
    private const UInt32 EFUSE_REAL_CONTENT_LEN_JAGUAR = 512;
    private const UInt16 RTL_EEPROM_ID = 0x8129;
    private const byte EEPROM_Default_Version = 0;
    private const byte EEPROM_VERSION_8812 = 0xC4;
    private const byte EEPROM_XTAL_8812 = 0xB9;
    private const byte EEPROM_Default_CrystalCap_8812 = 0x20;

    private static u8[] center_ch_5g_all = new byte[CENTER_CH_5G_ALL_NUM]
    {
        15, 16, 17, 18,
        20, 24, 28, 32,
/* G00 */36, 38, 40,
        42,
/* G01 */44, 46, 48,
        /* 50, */
/* G02 */52, 54, 56,
        58,
/* G03 */60, 62, 64,
        68, 72, 76, 80,
        84, 88, 92, 96,
/* G04 */100, 102, 104,
        106,
/* G05 */108, 110, 112,
        /* 114, */
/* G06 */116, 118, 120,
        122,
/* G07 */124, 126, 128,
/* G08 */132, 134, 136,
        138,
/* G09 */140, 142, 144,
/* G10 */149, 151, 153,
        155,
/* G11 */157, 159, 161,
        /* 163, */
/* G12 */165, 167, 169,
        171,
/* G13 */173, 175, 177
    };

    private static u8[] center_ch_5g_80m = new byte[CENTER_CH_5G_80M_NUM]
    {
/* G00 ~ G01*/42,
/* G02 ~ G03*/58,
/* G04 ~ G05*/106,
/* G06 ~ G07*/122,
/* G08 ~ G09*/138,
/* G10 ~ G11*/155,
/* G12 ~ G13*/171
    };

    public static void rtl8812au_interface_configure(_adapter padapter)
    {
        var pHalData = GET_HAL_DATA(padapter);

        var pdvobjpriv = adapter_to_dvobj(padapter);

        if (IS_SUPER_SPEED_USB(padapter))
        {
            pHalData.UsbBulkOutSize = USB_SUPER_SPEED_BULK_SIZE; /* 1024 bytes */
        }
        else if (IS_HIGH_SPEED_USB(padapter))
        {
            pHalData.UsbBulkOutSize = USB_HIGH_SPEED_BULK_SIZE; /* 512 bytes */
        }
        else
        {
            pHalData.UsbBulkOutSize = USB_FULL_SPEED_BULK_SIZE; /* 64 bytes */
        }

        pHalData.interfaceIndex = pdvobjpriv.InterfaceNumber;

//# ifdef CONFIG_USB_TX_AGGREGATION
//        pHalData.UsbTxAggMode = 1;
//        pHalData.UsbTxAggDescNum = 6; /* only 4 bits */

//        if (IS_HARDWARE_TYPE_8812AU(padapter)) /* page added for Jaguar */
//            pHalData.UsbTxAggDescNum = 0x01; /* adjust value for OQT  Overflow issue */ /* 0x3;	 */ /* only 4 bits */
//#endif

//# ifdef CONFIG_USB_RX_AGGREGATION
//        if (IS_HARDWARE_TYPE_8812AU(padapter))
//            pHalData.rxagg_mode = RX_AGG_USB;
//        else
//            pHalData.rxagg_mode = RX_AGG_USB; /* todo: change to USB_RX_AGG_DMA */
//        pHalData.rxagg_usb_size = 8; /* unit: 512b */
//        pHalData.rxagg_usb_timeout = 0x6;
//        pHalData.rxagg_dma_size = 16; /* uint: 128b, 0x0A = 10 = MAX_RX_DMA_BUFFER_SIZE/2/pHalData.UsbBulkOutSize */
//        pHalData.rxagg_dma_timeout = 0x6; /* 6, absolute time = 34ms/(2^6) */

//        if (IS_SUPER_SPEED_USB(padapter))
//        {
//            pHalData.rxagg_usb_size = 0x7;
//            pHalData.rxagg_usb_timeout = 0x1a;
//        }
//        else
//        {
//            /* the setting to reduce RX FIFO overflow on USB2.0 and increase rx throughput */

//# ifdef CONFIG_PREALLOC_RX_SKB_BUFFER
//            u32 remainder = 0;
//            u8 quotient = 0;

//            remainder = MAX_RECVBUF_SZ % (4 * 1024);
//            quotient = (u8)(MAX_RECVBUF_SZ >> 12);

//            if (quotient > 5)
//            {
//                pHalData.rxagg_usb_size = 0x5;
//                pHalData.rxagg_usb_timeout = 0x20;
//            }
//            else
//            {
//                if (remainder >= 2048)
//                {
//                    pHalData.rxagg_usb_size = quotient;
//                    pHalData.rxagg_usb_timeout = 0x10;
//                }
//                else
//                {
//                    pHalData.rxagg_usb_size = (quotient - 1);
//                    pHalData.rxagg_usb_timeout = 0x10;
//                }
//            }

//#else /* !CONFIG_PREALLOC_RX_SKB_BUFFER */
//            pHalData.rxagg_usb_size = 0x5;
//            pHalData.rxagg_usb_timeout = 0x20;
//#endif /* CONFIG_PREALLOC_RX_SKB_BUFFER */

//        }

//#endif /* CONFIG_USB_RX_AGGREGATION */

        HalUsbSetQueuePipeMapping8812AUsb(padapter, pdvobjpriv.RtNumInPipes, pdvobjpriv.RtNumOutPipes);

    }

    static bool HalUsbSetQueuePipeMapping8812AUsb(PADAPTER pAdapter, u8 NumInPipe, u8 NumOutPipe)
    {
        var pHalData = GET_HAL_DATA(pAdapter);
        bool result = false;

        var pregistrypriv = pAdapter.registrypriv;
        var bWIFICfg = (pregistrypriv.wifi_spec) ? true : false;

        _ConfigChipOutEP_8812(pAdapter, NumOutPipe);

        /* Normal chip with one IN and one OUT doesn't have interrupt IN EP. */
        if (1 == pHalData.OutEpNumber)
        {
            if (1 != NumInPipe)
            {
                return result;
            }
        }

        /* All config other than above support one Bulk IN and one Interrupt IN. */
        /* if(2 != NumInPipe){ */
        /*	return result; */
        /* } */

        if (NumOutPipe == 4)
        {
            result = true;
            {
                // TODO:
                throw new NotImplementedException("_FourOutPipeMapping88212AU");
                //_FourOutPipeMapping88212AU(pAdapter, bWIFICfg);
            }
        }
        else
        {
            // TODO:
            throw new NotImplementedException("Hal_MappingOutPipe");
            //result = Hal_MappingOutPipe(pAdapter, NumOutPipe);
        }

        return result;

    }

    public static u8 ReadAdapterInfo8812AU(PADAPTER Adapter)
    {
        /* Read all content in Efuse/EEPROM. */
        Hal_ReadPROMContent_8812A(Adapter);

        /* We need to define the RF type after all PROM value is recognized. */
        ReadRFType8812A(Adapter);

        return _SUCCESS;
    }

    private static void ReadRFType8812A(PADAPTER padapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);

        pHalData.rf_chip = RF_CHIP_E.RF_6052;

        //if (IsSupported24G(padapter.registrypriv.wireless_mode) && is_supported_5g(padapter.registrypriv.wireless_mode))
        {
            pHalData.BandSet = BAND_TYPE.BAND_ON_BOTH;
        }
        // else if (is_supported_5g(padapter.registrypriv.wireless_mode))
        // {
        //     pHalData.BandSet = BAND_ON_5G;
        // }
        // else
        // {
        //     pHalData.BandSet = BAND_ON_2_4G;
        // }


    }

    static void Hal_ReadPROMContent_8812A(PADAPTER Adapter
    )
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u8 eeValue;

        /* check system boot selection */
        eeValue = rtw_read8(Adapter, REG_9346CR);
        pHalData.EepromOrEfuse = (eeValue & BOOT_FROM_EEPROM) != 0 ? true : false;
        pHalData.bautoload_fail_flag = (eeValue & EEPROM_EN) != 0 ? false : true;

        RTW_INFO("Boot from %s, Autoload %s !", (pHalData.EepromOrEfuse ? "EEPROM" : "EFUSE"),
            (pHalData.bautoload_fail_flag ? "Fail" : "OK"));

        /* pHalData.EEType = IS_BOOT_FROM_EEPROM(Adapter) ? EEPROM_93C46 : EEPROM_BOOT_EFUSE; */

        InitAdapterVariablesByPROM_8812AU(Adapter);
    }

    private static void InitAdapterVariablesByPROM_8812AU(PADAPTER Adapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        hal_InitPGData_8812A(Adapter, pHalData.efuse_eeprom_data);

        Hal_EfuseParseIDCode8812A(Adapter, pHalData.efuse_eeprom_data);

        Hal_ReadPROMVersion8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        hal_ReadIDs_8812AU(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        hal_config_macaddr(Adapter, pHalData.bautoload_fail_flag);
        Hal_ReadTxPowerInfo8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        Hal_ReadBoardType8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);

        /*  */
        /* Read Bluetooth co-exist and initialize */
        /*  */
        Hal_EfuseParseBTCoexistInfo8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);

        Hal_ReadChannelPlan8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        Hal_EfuseParseXtal_8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        Hal_ReadThermalMeter_8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        Hal_ReadRemoteWakeup_8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        // Fully disabled via CONFIG_ANTENNA_DIVERSITY
        //Hal_ReadAntennaDiversity8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);


        Hal_ReadAmplifierType_8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        Hal_ReadRFEType_8812A(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);


        hal_ReadUsbModeSwitch_8812AU(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);
        hal_CustomizeByCustomerID_8812AU(Adapter);

        ReadLEDSetting_8812AU(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);

        /* 2013/04/15 MH Add for different board type recognize. */
        hal_ReadUsbType_8812AU(Adapter, pHalData.efuse_eeprom_data, pHalData.bautoload_fail_flag);


        /* set coex. ant info once efuse parsing is done */
        rtw_btcoex_set_ant_info(Adapter);
    }

    static void rtw_btcoex_set_ant_info(PADAPTER padapter)
    {
            rtw_btcoex_wifionly_AntInfoSetting(padapter);
    }
    static void rtw_btcoex_wifionly_AntInfoSetting(PADAPTER padapter)
    {
        hal_btcoex_wifionly_AntInfoSetting(padapter);
    }

    static void hal_btcoex_wifionly_AntInfoSetting(PADAPTER padapter)
    {
        //wifi_only_cfg        pwifionlycfg = GLBtCoexistWifiOnly;
        //wifi_only_haldata    pwifionly_haldata = pwifionlycfg.haldata_info;
        //HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);

        //pwifionly_haldata.efuse_pg_antnum = pHalData.EEPROMBluetoothAntNum;
        //pwifionly_haldata.efuse_pg_antpath = pHalData.ant_path;
        //pwifionly_haldata.rfe_type = pHalData.rfe_type;
        //pwifionly_haldata.ant_div_cfg = pHalData.AntDivCfg;
    }


static void hal_ReadUsbType_8812AU(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        /* if (IS_HARDWARE_TYPE_8812AU(Adapter) && Adapter.UsbModeMechanism.RegForcedUsbMode == 5) */
        {
            PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

            hal_spec_t hal_spc = GET_HAL_SPEC(Adapter);
            u8 reg_tmp, i, j, antenna = 0, wmode = 0;
            /* Read anenna type from EFUSE 1019/1018 */
            for (i = 0; i < 2; i++)
            {
                /*
                  Check efuse address 1019
                  Check efuse address 1018
                */
                efuse_OneByteRead(Adapter, (ushort)(1019 - i), out reg_tmp);
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
                efuse_OneByteRead(Adapter, (ushort)(1021 - i), out reg_tmp);

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
                pHalData.rf_type = rf_type.RF_1T1R;
                /* UsbModeSwitch_SetUsbModeMechOn(Adapter, FALSE); */
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
                        /* UsbModeSwitch_SetUsbModeMechOn(Adapter, FALSE); */
                        /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VS; */
                        RTW_INFO("%s(): EFUSE_HIDDEN_8812AU_VS");
                    }
                }
                else if (wmode == 2)
                {
                    /* Antenna == 2 WMODE = 2 RTL8812AU-VN 11N only + USB2.0 Mode */
                    /* UsbModeSwitch_SetUsbModeMechOn(Adapter, FALSE); */
                    /* pHalData.EFUSEHidden = EFUSE_HIDDEN_812AU_VN; */
                    RTW_INFO("%s(): EFUSE_HIDDEN_8812AU_VN");

                    var PROTO_CAP_11AC = BIT3;
                    //hal_spc.proto_cap &= ~PROTO_CAP_11AC;
                    hal_spc.proto_cap = (byte)(hal_spc.proto_cap & ~PROTO_CAP_11AC);
                }
            }
        }
    }

    static void ReadLEDSetting_8812AU(PADAPTER    Adapter,u8		[]PROMContent,BOOLEAN     AutoloadFail)
    {
//#ifdef CONFIG_RTW_LED
//        struct led_priv *pledpriv = adapter_to_led(Adapter);

//# ifdef CONFIG_RTW_SW_LED
//        pledpriv.bRegUseLed = _TRUE;
//#else /* HW LED */
//        pledpriv.LedStrategy = HW_LED;
//#endif /* CONFIG_RTW_SW_LED */
//#endif
    }

static void hal_CustomizeByCustomerID_8812AU(PADAPTER        pAdapter)
    {
        // Looks like all this need for led
        //HAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);

        ///* For customized behavior. */
        //if ((pHalData.EEPROMVID == 0x103C) && (pHalData.EEPROMPID == 0x1629)) /* HP Lite-On for RTL8188CUS Slim Combo. */
        //    pHalData.CustomerID = RT_CID_819x_HP;
        //else if ((pHalData.EEPROMVID == 0x9846) && (pHalData.EEPROMPID == 0x9041))
        //    pHalData.CustomerID = RT_CID_NETGEAR;
        //else if ((pHalData.EEPROMVID == 0x2019) && (pHalData.EEPROMPID == 0x1201))
        //    pHalData.CustomerID = RT_CID_PLANEX;
        //else if ((pHalData.EEPROMVID == 0x0BDA) && (pHalData.EEPROMPID == 0x5088))
        //    pHalData.CustomerID = RT_CID_CC_C;
        //else if ((pHalData.EEPROMVID == 0x0411) && ((pHalData.EEPROMPID == 0x0242) || (pHalData.EEPROMPID == 0x025D)))
        //    pHalData.CustomerID = RT_CID_DNI_BUFFALO;
        //else if (((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3314)) ||
        //    ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x804B)) ||
        //    ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x805B)) ||
        //    ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3315)) ||
        //    ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3316)))
        //    pHalData.CustomerID = RT_CID_DLINK;

        //RTW_INFO("PID= 0x%x, VID=  %x\n", pHalData.EEPROMPID, pHalData.EEPROMVID);

        ///*	Decide CustomerID according to VID/DID or EEPROM */
        //switch (pHalData.EEPROMCustomerID)
        //{
        //    case EEPROM_CID_DEFAULT:
        //        if ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3308))
        //            pHalData.CustomerID = RT_CID_DLINK;
        //        else if ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3309))
        //            pHalData.CustomerID = RT_CID_DLINK;
        //        else if ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x330a))
        //            pHalData.CustomerID = RT_CID_DLINK;
        //        else if ((pHalData.EEPROMVID == 0x0BFF) && (pHalData.EEPROMPID == 0x8160))
        //        {
        //            /* pHalData.bAutoConnectEnable = _FALSE; */
        //            pHalData.CustomerID = RT_CID_CHINA_MOBILE;
        //        }
        //        else if ((pHalData.EEPROMVID == 0x0BDA) && (pHalData.EEPROMPID == 0x5088))
        //            pHalData.CustomerID = RT_CID_CC_C;
        //        else if ((pHalData.EEPROMVID == 0x0846) && (pHalData.EEPROMPID == 0x9052))
        //            pHalData.CustomerID = RT_CID_NETGEAR;
        //        else if ((pHalData.EEPROMVID == 0x0411) && ((pHalData.EEPROMPID == 0x0242) || (pHalData.EEPROMPID == 0x025D)))
        //            pHalData.CustomerID = RT_CID_DNI_BUFFALO;
        //        else if (((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3314)) ||
        //            ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x804B)) ||
        //            ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x805B)) ||
        //            ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3315)) ||
        //            ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3316)))
        //            pHalData.CustomerID = RT_CID_DLINK;
        //        RTW_INFO("PID= 0x%x, VID=  %x\n", pHalData.EEPROMPID, pHalData.EEPROMVID);
        //        break;
        //    case EEPROM_CID_WHQL:
        //        /* padapter.bInHctTest = TRUE; */

        //        /* pMgntInfo.bSupportTurboMode = FALSE; */
        //        /* pMgntInfo.bAutoTurboBy8186 = FALSE; */

        //        /* pMgntInfo.PowerSaveControl.bInactivePs = FALSE; */
        //        /* pMgntInfo.PowerSaveControl.bIPSModeBackup = FALSE; */
        //        /* pMgntInfo.PowerSaveControl.bLeisurePs = FALSE; */
        //        /* pMgntInfo.PowerSaveControl.bLeisurePsModeBackup = FALSE; */
        //        /* pMgntInfo.keepAliveLevel = 0; */

        //        /* padapter.bUnloadDriverwhenS3S4 = FALSE; */
        //        break;
        //    default:
        //        pHalData.CustomerID = RT_CID_DEFAULT;
        //        break;

        //}
        //RTW_INFO("Customer ID: 0x%2x\n", pHalData.CustomerID);

        //hal_CustomizedBehavior_8812AU(pAdapter);
    }


    private static void hal_ReadUsbModeSwitch_8812AU(PADAPTER    Adapter,u8			[]PROMContent, BOOLEAN     AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        if (AutoloadFail)
        {
            pHalData.EEPROMUsbSwitch = false;
        }
        else
            /* check efuse 0x08 bit2 */
        {
            pHalData.EEPROMUsbSwitch = ((PROMContent[EEPROM_USB_MODE_8812] & BIT1) >> 1) != 0;
        }

        RTW_INFO("Usb Switch: %d", pHalData.EEPROMUsbSwitch);
    }


    private static void Hal_ReadRFEType_8812A(PADAPTER    Adapter,u8			[]PROMContent, BOOLEAN     AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        if (!AutoloadFail)
        {
            if ((GetRegRFEType(Adapter) != 64) || 0xFF == PROMContent[EEPROM_RFE_OPTION_8812])
            {
                if (GetRegRFEType(Adapter) != 64)
                    pHalData.rfe_type = GetRegRFEType(Adapter);
                else
                {
                    if (IS_HARDWARE_TYPE_8812AU(Adapter))
                    {
                        pHalData.rfe_type = 0;
                    }

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
                    if (IS_HARDWARE_TYPE_8812AU(Adapter))
                    {
                        pHalData.rfe_type = 0;
                    }

                }
            }
        }
        else
        {
            if (GetRegRFEType(Adapter) != 64)
                pHalData.rfe_type = GetRegRFEType(Adapter);
            else
            {
                if (IS_HARDWARE_TYPE_8812AU(Adapter))
                {
                    pHalData.rfe_type = 0;
                }

            }
        }

        RTW_INFO("RFE Type: 0x%2x\n", pHalData.rfe_type);
    }

    static void Hal_ReadChannelPlan8812A(PADAPTER        padapter,u8				[]hwinfo,BOOLEAN         AutoLoadFail)
    {
        //hal_com_config_channel_plan(
        //    padapter
        //    , hwinfo[EEPROM_COUNTRY_CODE_8812]
        //    , hwinfo[EEPROM_ChannelPlan_8812]
        //    , padapter.registrypriv.alpha2
        //    , padapter.registrypriv.channel_plan
        //    , RTW_CHPLAN_REALTEK_DEFINE
        //    , AutoLoadFail
        //);
    }

    /// <summary>
    /// Use hardware(efuse), driver parameter(registry) and default channel plan
    /// to decide which one should be used.
    /// </summary>
    /// <param name="padapter"></param>
    /// <param name="hw_alpha2">country code from HW (efuse/eeprom/mapfile)</param>
    /// <param name="hw_chplan">
    /// channel plan from HW (efuse/eeprom/mapfile)
    /// BIT[7] software configure mode; 0:Enable, 1:disable
    /// BIT[6:0] Channel Plan
    /// </param>
    /// <param name="sw_alpha2">country code from HW (registry/module param)</param>
    /// <param name="sw_chplan">channel plan from SW (registry/module param)</param>
    /// <param name="def_chplan">channel plan used when HW/SW both invalid</param>
    /// <param name="AutoLoadFail"></param>
    static void hal_com_config_channel_plan(
        PADAPTER padapter,
        string hw_alpha2,
        u8 hw_chplan,
        string sw_alpha2,
        u8 sw_chplan,
        u8 def_chplan,
        BOOLEAN AutoLoadFail
    )
    {

//        rf_ctl_t rfctl = adapter_to_rfctl(padapter);
//        PHAL_DATA_TYPE pHalData;
//        bool force_hw_chplan = false;
//        int chplan = -1;
//        country_chplan country_ent = null, ent;

//        pHalData = GET_HAL_DATA(padapter);

//        /* treat 0xFF as invalid value, bypass hw_chplan & force_hw_chplan parsing */
//        if (hw_chplan == 0xFF)
//            goto chk_hw_country_code;

//        if (AutoLoadFail == true)
//        {
//            goto chk_sw_config;
//        }

////#ifndef CONFIG_FORCE_SW_CHANNEL_PLAN
////	if (hw_chplan & EEPROM_CHANNEL_PLAN_BY_HW_MASK)
////		force_hw_chplan = _TRUE;
////#endif

//        hw_chplan &= (~EEPROM_CHANNEL_PLAN_BY_HW_MASK);

//        chk_hw_country_code:
//        if (hw_alpha2 && !IS_ALPHA2_NO_SPECIFIED(hw_alpha2))
//        {
//            ent = rtw_get_chplan_from_country(hw_alpha2);
//            if (ent)
//            {
//                /* get chplan from hw country code, by pass hw chplan setting */
//                country_ent = ent;
//                chplan = ent.chplan;
//                goto chk_sw_config;
//            }
//            else
//            {
//                RTW_PRINT("%s unsupported hw_alpha2:\"%c%c\"\n", __func__, hw_alpha2[0], hw_alpha2[1]);
//            }
//        }

//        if (rtw_is_channel_plan_valid(hw_chplan))
//            chplan = hw_chplan;
//        else if (force_hw_chplan == true)
//        {
//            RTW_PRINT("%s unsupported hw_chplan:0x%02X\n", hw_chplan);
//            /* hw infomaton invalid, refer to sw information */
//            force_hw_chplan = false;
//        }

//        chk_sw_config:
//        if (force_hw_chplan == true)
//        {
//            goto done;
//        }

//        if (!string.IsNullOrWhiteSpace(sw_alpha2))
//        {
//            ent = rtw_get_chplan_from_country(sw_alpha2);
//            if (ent !=null)
//            {
//                /* get chplan from sw country code, by pass sw chplan setting */
//                country_ent = ent;
//                chplan = ent.chplan;
//                goto done;
//            }
//            else
//            {
//                RTW_PRINT("%s unsupported sw_alpha2:\"%c%c\"\n",  sw_alpha2[0], sw_alpha2[1]);
//            }
//        }

//        if (rtw_is_channel_plan_valid(sw_chplan))
//        {
//            /* cancel hw_alpha2 because chplan is specified by sw_chplan*/
//            country_ent = null;
//            chplan = sw_chplan;
//        }
//        else if (sw_chplan != RTW_CHPLAN_UNSPECIFIED)
//        {
//            RTW_PRINT("%s unsupported sw_chplan:0x%02X", sw_chplan);
//        }

//        done:
//        if (chplan == -1)
//        {
//            RTW_PRINT("%s use def_chplan:0x%02X\n", def_chplan);
//            chplan = def_chplan;
//        }
//        else if (country_ent != null)
//        {
//            RTW_PRINT("%s country code:\"%c%c\" with chplan:0x%02X\n",country_ent.alpha2[0], country_ent.alpha2[1], country_ent.chplan);
//        }
//        else
//            RTW_PRINT("%s chplan:0x%02X\n", chplan);

//        rfctl.country_ent = country_ent;
//        rfctl.ChannelPlan = chplan;
//        pHalData.bDisableSWChannelPlan = force_hw_chplan;
    }


    private static void Hal_ReadRemoteWakeup_8812A(PADAPTER padapter, u8[] hwinfo, BOOLEAN AutoLoadFail)
    {
        pwrctrl_priv pwrctl = adapter_to_pwrctl(padapter);

        if (AutoLoadFail)
        {
            pwrctl.bHWPowerdown = false;
            pwrctl.bSupportRemoteWakeup = false;
        }
        else
        {
            /* decide hw if support remote wakeup function */
            /* if hw supported, 8051 (SIE) will generate WeakUP signal( D+/D- toggle) when autoresume */
            pwrctl.bSupportRemoteWakeup = (hwinfo[EEPROM_USB_OPTIONAL_FUNCTION0] & BIT1) != 0 ? true : false;
            RTW_INFO("%s...bSupportRemoteWakeup(%x)", pwrctl.bSupportRemoteWakeup);
        }
    }

    static void Hal_ReadThermalMeter_8812A(PADAPTER    Adapter,u8			[]PROMContent, BOOLEAN AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
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
            pHalData.odmpriv.rf_calibrate_info.is_apk_thermal_meter_ignore = true;
            pHalData.eeprom_thermal_meter = 0xFF;
        }

        /* pHalData.ThermalMeter[0] = pHalData.eeprom_thermal_meter;	 */
        RTW_INFO("ThermalMeter = 0x%x\n", pHalData.eeprom_thermal_meter);
    }

    static void Hal_EfuseParseBTCoexistInfo8812A(PADAPTER         Adapter,u8				[]hwinfo, BOOLEAN          AutoLoadFail)
    {

        registry_priv regsty = Adapter.registrypriv;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u8 tmp_u8;
        u32 tmp_u32;

            pHalData.EEPROMBluetoothType = BT_RTL8812A;

            if (!AutoLoadFail)
            {
                tmp_u8 = hwinfo[EEPROM_RF_BOARD_OPTION_8812];
                if (((tmp_u8 & 0xe0) >> 5) == 0x1) /* [7:5] */
                    pHalData.EEPROMBluetoothCoexist = true;
                else
                    pHalData.EEPROMBluetoothCoexist = false;

                tmp_u8 = hwinfo[EEPROM_RF_BT_SETTING_8812];
                pHalData.EEPROMBluetoothAntNum = (byte)(tmp_u8 & 0x1); /* bit [0] */
            }
            else
            {
                pHalData.EEPROMBluetoothCoexist = false;
                pHalData.EEPROMBluetoothAntNum = 1; //Ant_x1;
            }
    }

    static void Hal_ReadBoardType8812A(PADAPTER    Adapter, u8[]PROMContent, BOOLEAN     AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

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

    private static void Hal_ReadTxPowerInfo8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoLoadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        TxPowerInfo24G pwrInfo24G = new TxPowerInfo24G();
        TxPowerInfo5G pwrInfo5G = new TxPowerInfo5G();

        hal_load_txpwr_info(Adapter, pwrInfo24G, pwrInfo5G, PROMContent);

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

    private static void hal_load_txpwr_info(_adapter adapter, TxPowerInfo24G pwr_info_2g, TxPowerInfo5G pwr_info_5g, u8[] pg_data)
    {
        HAL_DATA_TYPE hal_data = GET_HAL_DATA(adapter);

        var hal_spec = GET_HAL_SPEC(adapter);
        u8 max_tx_cnt = hal_spec.max_tx_cnt;
        u8 rfpath, ch_idx, group = 0, tx_idx;

        /* load from pg data (or default value) */
        hal_load_pg_txpwr_info(adapter, pwr_info_2g, pwr_info_5g, pg_data, false);

        /* transform to hal_data */
        for (rfpath = 0; rfpath < MAX_RF_PATH; rfpath++)
        {

            if (!HAL_SPEC_CHK_RF_PATH_2G(hal_spec, rfpath))
                goto bypass_2g;

            /* 2.4G base */
            for (ch_idx = 0; ch_idx < CENTER_CH_2G_NUM; ch_idx++)
            {
                u8 cck_group = 0;

                if (rtw_get_ch_group((byte)(ch_idx + 1), ref group, ref cck_group) != BAND_TYPE.BAND_ON_2_4G)
                    continue;

                hal_data.Index24G_CCK_Base[rfpath, ch_idx] = pwr_info_2g.IndexCCK_Base[rfpath, cck_group];
                hal_data.Index24G_BW40_Base[rfpath, ch_idx] = pwr_info_2g.IndexBW40_Base[rfpath, group];
            }

            /* 2.4G diff */
            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
                if (tx_idx >= max_tx_cnt)
                    break;

                hal_data.CCK_24G_Diff[rfpath, tx_idx] =
                    (byte)(pwr_info_2g.CCK_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.OFDM_24G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_2g.OFDM_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.BW20_24G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_2g.BW20_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.BW40_24G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_2g.BW40_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
            }

            bypass_2g: ;


            if (!HAL_SPEC_CHK_RF_PATH_5G(hal_spec, rfpath))
                goto bypass_5g;
            u8 tmp = 0;
/* 5G base */
            for (ch_idx = 0; ch_idx < CENTER_CH_5G_ALL_NUM; ch_idx++)
            {
                if (rtw_get_ch_group(center_ch_5g_all[ch_idx], ref group, ref tmp) != BAND_TYPE.BAND_ON_5G)
                    continue;
                hal_data.Index5G_BW40_Base[rfpath, ch_idx] = pwr_info_5g.IndexBW40_Base[rfpath, group];
            }

            for (ch_idx = 0; ch_idx < CENTER_CH_5G_80M_NUM; ch_idx++)
            {
                u8 upper, lower;

                if (rtw_get_ch_group(center_ch_5g_80m[ch_idx], ref group, ref tmp) != BAND_TYPE.BAND_ON_5G)
                    continue;

                upper = pwr_info_5g.IndexBW40_Base[rfpath, group];
                lower = pwr_info_5g.IndexBW40_Base[rfpath, group + 1];
                hal_data.Index5G_BW80_Base[rfpath, ch_idx] = (byte)((upper + lower) / 2);
            }

/* 5G diff */
            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
                if (tx_idx >= max_tx_cnt)
                    break;

                hal_data.OFDM_5G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_5g.OFDM_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.BW20_5G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_5g.BW20_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.BW40_5G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_5g.BW40_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
                hal_data.BW80_5G_Diff[rfpath, tx_idx] =
                    (sbyte)(pwr_info_5g.BW80_Diff[rfpath, tx_idx] * hal_spec.pg_txgi_diff_factor);
            }

            bypass_5g: ;

        }
    }

    static map_t hal_pg_txpwr_def_info(_adapter adapter)
    {
        u8 interface_type = 0;
        map_t map = null;

        map = MAP_ENT(0xB8, 1, 0xFF, new map_seg_t()
            {
                sa = 0x10,
                len = 82,
                c = new byte[]
                {
                    0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x02, 0xEE, 0xEE, 0xFF, 0xFF,
                    0xFF, 0xFF, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A,
                    0x02, 0xEE, 0xFF, 0xFF, 0xEE, 0xFF, 0x00, 0xEE, 0xFF, 0xFF, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D,
                    0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x02, 0xEE, 0xEE, 0xFF, 0xFF, 0xFF, 0xFF, 0x2A, 0x2A, 0x2A, 0x2A,
                    0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x02, 0xEE, 0xFF, 0xFF, 0xEE, 0xFF,
                    0x00, 0xEE
                }
            }

        );

        return map;
    }

    private static void hal_load_pg_txpwr_info(_adapter adapter, TxPowerInfo24G pwr_info_2g, TxPowerInfo5G pwr_info_5g, u8[] pg_data, BOOLEAN AutoLoadFail)
    {

        var hal_spec = GET_HAL_SPEC(adapter);
        u8 path;
        u16 pg_offset;
        u8 txpwr_src = PG_TXPWR_SRC_PG_DATA;
        map_t pg_data_map = MAP_ENT(184, 1, 0xFF, MAPSEG_PTR_ENT(0x00, 184, pg_data));
        map_t txpwr_map = null;

        /* init with invalid value and some dummy base and diff */
        hal_init_pg_txpwr_info_2g(adapter, pwr_info_2g);
        hal_init_pg_txpwr_info_5g(adapter, pwr_info_5g);

        select_src:
        pg_offset = hal_spec.pg_txpwr_saddr;

        switch (txpwr_src)
        {
            case PG_TXPWR_SRC_PG_DATA:
                txpwr_map = pg_data_map;
                break;
            case PG_TXPWR_SRC_IC_DEF:
                txpwr_map = hal_pg_txpwr_def_info(adapter);
                break;
            case PG_TXPWR_SRC_DEF:
            default:
                txpwr_map = MAP_ENT(0xB8, 1, 0xFF, new map_seg_t()
                {
                    len = 168,
                    sa = 0x88,
                    c = new byte[]
                    {
                        0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x24, 0xEE, 0xEE, 0xEE, 0xEE,
                        0xEE, 0xEE, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A,
                        0x04, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D,
                        0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x24, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x2A, 0x2A, 0x2A, 0x2A,
                        0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x04, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE,
                        0xEE, 0xEE, 0xEE, 0xEE, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x24,
                        0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A,
                        0x2A, 0x2A, 0x2A, 0x2A, 0x04, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x2D, 0x2D,
                        0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x2D, 0x24, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE,
                        0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x2A, 0x04, 0xEE,
                        0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE
                    }
                });
                break;
        }

        if (txpwr_map == null)
        {
            goto end_parse;
        }

        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path) && !HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
                break;
            pg_offset = hal_load_pg_txpwr_info_path_2g(adapter, pwr_info_2g, path, txpwr_src, txpwr_map, pg_offset);
            pg_offset = hal_load_pg_txpwr_info_path_5g(adapter, pwr_info_5g, path, txpwr_src, txpwr_map, pg_offset);
        }

        if (hal_chk_pg_txpwr_info_2g(adapter, pwr_info_2g)&&
            hal_chk_pg_txpwr_info_5g(adapter, pwr_info_5g))
        {
            goto exit;
        }

        end_parse:
        txpwr_src++;
        if (txpwr_src < PG_TXPWR_SRC_NUM)
            goto select_src;

        if (hal_chk_pg_txpwr_info_2g(adapter, pwr_info_2g)|| hal_chk_pg_txpwr_info_5g(adapter, pwr_info_5g))
        {
            throw new Exception();
        }

        exit:

        return;
    }

    static bool hal_chk_band_cap(_adapter adapter, u8 cap)
    {
        return (GET_HAL_SPEC(adapter).band_cap & cap) != 0;
    }

    static bool IS_PG_TXPWR_BASE_INVALID(hal_spec_t hal_spec, byte _base) => ((_base) > hal_spec.txgi_max);

    static bool hal_chk_pg_txpwr_info_2g(_adapter adapter, TxPowerInfo24G pwr_info)
    {
        u8 BAND_CAP_2G = 0;

        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);
        u8 path, group, tx_idx;

        if (pwr_info == null || !hal_chk_band_cap(adapter, BAND_CAP_2G))
            return true;

        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path))
            {
                continue;
            }

            for (group = 0; group < MAX_CHNL_GROUP_24G; group++)
            {
                if (IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexCCK_Base[path, group]) ||
                    IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path, group]))
                {
                    return false;
                }
            }

            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
                if (!HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    continue;
                }

                if (IS_PG_TXPWR_DIFF_INVALID(pwr_info.CCK_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path, tx_idx]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    static bool IS_PG_TXPWR_DIFF_INVALID(sbyte _diff) => ((_diff) > 7 || (_diff) < -8);

    static bool HAL_SPEC_CHK_TX_CNT(hal_spec_t _spec, byte _cnt_idx) => ((_spec).max_tx_cnt > (_cnt_idx));

    static bool hal_chk_pg_txpwr_info_5g(_adapter adapter, TxPowerInfo5G pwr_info)
    {
        u8 BAND_CAP_5G = 1;
        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);
        u8 path, group, tx_idx;

        if (pwr_info == null || !hal_chk_band_cap(adapter, BAND_CAP_5G))
        {
            return true;
        }

        for (path = 0; path<MAX_RF_PATH; path++) {
            if (!HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
                continue;
            for (group = 0; group<MAX_CHNL_GROUP_5G; group++)
                if (IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path,group]))
                {
                    return false;
                }
            for (tx_idx = 0; tx_idx<MAX_TX_COUNT; tx_idx++) {
                if (!HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    continue;
                }
                if (IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW80_Diff[path, tx_idx])
                    || IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW160_Diff[path, tx_idx]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    static u8 map_read8(map_t map, u16 offset)
    {
        map_seg_t seg;
        u8 val = map.init_value;
        int i;

        if (offset + 1 > map.len)
        {
            throw new Exception("WTF");
            goto exit;
        }

        for (i = 0; i<map.seg_num; i++)
        {
            seg = map.segs + i;
            if (seg.sa + seg.len <= offset || seg.sa >= offset + 1)
                continue;

            val = * (seg.c + offset - seg.sa);
            break;
        }

        exit:
        return val;
    }

    static string rf_path_char(uint path) => (((path) >= RF_PATH_MAX) ? "X" : "A" + (path));

    static u8 PG_TXPWR_MSB_DIFF_S4BIT(u8 _pg_v) => (byte)(((_pg_v) & 0xf0) >> 4);
    static u8 PG_TXPWR_LSB_DIFF_S4BIT(u8 _pg_v) => (byte)((_pg_v) & 0x0f);

    static s8 PG_TXPWR_MSB_DIFF_TO_S8BIT(u8 _pg_v) => (sbyte)((PG_TXPWR_MSB_DIFF_S4BIT(_pg_v) & BIT3) != 0
        ? (PG_TXPWR_MSB_DIFF_S4BIT(_pg_v) | 0xF0)
        : PG_TXPWR_MSB_DIFF_S4BIT(_pg_v));

    static s8 PG_TXPWR_LSB_DIFF_TO_S8BIT(u8 _pg_v) => (sbyte)((PG_TXPWR_LSB_DIFF_S4BIT(_pg_v) & BIT3) != 0
        ? (PG_TXPWR_LSB_DIFF_S4BIT(_pg_v) | 0xF0)
        : PG_TXPWR_LSB_DIFF_S4BIT(_pg_v));
    static string[] _pg_txpwr_src_str = {
        "PG_DATA",
        "IC_DEF",
        "DEF",
        "UNKNOWN"
    };

    static string pg_txpwr_src_str(int src) =>
        (((src) >= PG_TXPWR_SRC_NUM) ? _pg_txpwr_src_str[PG_TXPWR_SRC_NUM] : _pg_txpwr_src_str[(src)]);

    static u16 hal_load_pg_txpwr_info_path_5g(_adapter adapter, TxPowerInfo5G pwr_info, byte path, u8 txpwr_src,
        map_t txpwr_map, u16 pg_offset)
    {

        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);
        u16 offset = pg_offset;
        u8 group, tx_idx;
        u8 val;
        u8 tmp_base;
        s8 tmp_diff;

        if (pwr_info == null || !hal_chk_band_cap(adapter, BAND_CAP_5G))

        {
            offset += PG_TXPWR_1PATH_BYTE_NUM_5G;
            goto exit;
        }


        if (DBG_PG_TXPWR_READ)
        {
            RTW_INFO("%s[%c] eaddr:0x%03x\n", rf_path_char(path), offset);
        }

        for (group = 0; group < MAX_CHNL_GROUP_5G; group++)
        {
            if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
            {
                tmp_base = map_read8(txpwr_map, offset);
                if (!IS_PG_TXPWR_BASE_INVALID(hal_spec, tmp_base)
                    && IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path,group])
                   )
                {
                    pwr_info.IndexBW40_Base[path,group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[%c] 5G G%02d BW40-1S base:%u from %s\n", rf_path_char(path), group, tmp_base, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }

            offset++;
        }

        for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
        {
            if (tx_idx == 0)
            {
                if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    val = map_read8(txpwr_map, offset);
                    tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 5G BW20-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.OFDM_Diff[path,tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }
                }

                offset++;
            }
            else
            {
                if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    val = map_read8(txpwr_map, offset);
                    tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.BW40_Diff[path,tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 5G BW40-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 5G BW20-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }
                }

                offset++;
            }
        }

/* OFDM diff 2T ~ 3T */
        if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, 1))
        {
            val = map_read8(txpwr_map, offset);
            tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
            if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, 1]))
            {
                pwr_info.OFDM_Diff[path,1] = tmp_diff;
                if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                {
                    RTW_INFO("[%c] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), 2, tmp_diff, pg_txpwr_src_str(txpwr_src));
                }
            }

            if (HAL_SPEC_CHK_TX_CNT(hal_spec, 2))
            {
                tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path,2]))
                {
                    pwr_info.OFDM_Diff[path, 2] = tmp_diff;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[%c] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), 3, tmp_diff, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }
        }

        offset++;

/* OFDM diff 4T */
        if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, 3))
        {
            val = map_read8(txpwr_map, offset);
            tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
            if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, 3]))
            {
                pwr_info.OFDM_Diff[path,3] = tmp_diff;
                if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                {
                    RTW_INFO("[%c] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), 4, tmp_diff, pg_txpwr_src_str(txpwr_src));
                }
            }
        }

        offset++;

        for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
        {
            if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
            {
                val = map_read8(txpwr_map, offset);
                tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW80_Diff[path, tx_idx])
                   )
                {
                    pwr_info.BW80_Diff[path, tx_idx] = tmp_diff;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        RTW_INFO("[%c] 5G BW80-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                }

                tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW160_Diff[path, tx_idx])
                   )
                {
                    pwr_info.BW160_Diff[path, tx_idx] = tmp_diff;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[%c] 5G BW160-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }

            offset++;
        }

        if (offset != pg_offset + PG_TXPWR_1PATH_BYTE_NUM_5G)
        {
            RTW_ERR("%s parse %d bytes != %d\n", offset - pg_offset, PG_TXPWR_1PATH_BYTE_NUM_5G);
            throw new Exception("ERRR");
        }

        exit:
        return offset;
    }

    private static bool LOAD_PG_TXPWR_WARN_COND(byte txpwrSrc)
    {
        return true; // Because DBG_PG_TXPWR_READ
    }

    private static ushort hal_load_pg_txpwr_info_path_2g(_adapter adapter, TxPowerInfo24G pwr_info, byte path,
        u8 txpwr_src, map_t txpwr_map, u16 pg_offset)
    {

        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);
        u16 offset = pg_offset;
        u8 group, tx_idx;
        u8 val;
        u8 tmp_base;
        s8 tmp_diff;

        if (pwr_info == null || !hal_chk_band_cap(adapter, BAND_CAP_2G))
        {
            offset += PG_TXPWR_1PATH_BYTE_NUM_2G;
            goto exit;
        }

        if (DBG_PG_TXPWR_READ)
        {
            RTW_INFO("%s [%c] offset:0x%03x\n", rf_path_char(path), offset);
        }

        for (group = 0; group < MAX_CHNL_GROUP_24G; group++)
        {
            if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path))
            {
                tmp_base = map_read8(txpwr_map, offset);
                if (!IS_PG_TXPWR_BASE_INVALID(hal_spec, tmp_base) && IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexCCK_Base[path,group])
                   )
                {
                    pwr_info.IndexCCK_Base[path,group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[%c] 2G G%02d CCK-1T base:%u from %s\n", rf_path_char(path), group, tmp_base, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }

            offset++;
        }

        for (group = 0; group < MAX_CHNL_GROUP_24G - 1; group++)
        {
            if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path))
            {
                tmp_base = map_read8(txpwr_map, offset);
                if (!IS_PG_TXPWR_BASE_INVALID(hal_spec, tmp_base)
                    && IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path, group])
                   )
                {
                    pwr_info.IndexBW40_Base[path,group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[%c] 2G G%02d BW40-1S base:%u from %s\n", rf_path_char(path), group, tmp_base, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }

            offset++;
        }

        for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
        {
            if (tx_idx == 0)
            {
                if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    val = map_read8(txpwr_map, offset);
                    tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path,tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G BW20-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.OFDM_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G OFDM-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }
                }

                offset++;
            }
            else
            {
                if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    val = map_read8(txpwr_map, offset);
                    tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path,tx_idx]))
                    {
                        pwr_info.BW40_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G BW40-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }

                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path,tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                            RTW_INFO("[%c] 2G BW20-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
                    }
                }

                offset++;

                if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path) && HAL_SPEC_CHK_TX_CNT(hal_spec, tx_idx))
                {
                    val = map_read8(txpwr_map, offset);
                    tmp_diff = PG_TXPWR_MSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.OFDM_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G OFDM-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.CCK_Diff[path,tx_idx])
                       )
                    {
                        pwr_info.CCK_Diff[path,tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G CCK-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }
                }

                offset++;
            }
        }

        if (offset != pg_offset + PG_TXPWR_1PATH_BYTE_NUM_2G)
        {
            RTW_ERR("%s parse %d bytes != %d\n",  offset - pg_offset, PG_TXPWR_1PATH_BYTE_NUM_2G);
            throw new Exception();
        }

        exit:
        return offset;
    }

    static void hal_init_pg_txpwr_info_2g(_adapter adapter, TxPowerInfo24G pwr_info)
    {
        var hal_spec = GET_HAL_SPEC(adapter);
        u8 path, group, tx_idx;

        /* init with invalid value */
        for (path = 0; path < MAX_RF_PATH; path++)
        {
            for (group = 0; group < MAX_CHNL_GROUP_24G; group++)
            {
                pwr_info.IndexCCK_Base[path, group] = PG_TXPWR_INVALID_BASE;
                pwr_info.IndexBW40_Base[path, group] = PG_TXPWR_INVALID_BASE;
            }

            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
                pwr_info.CCK_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.OFDM_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW20_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW40_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
            }
        }

        /* init for dummy base and diff */
        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path))
                break;
            /* 2.4G BW40 base has 1 less group than CCK base*/
            pwr_info.IndexBW40_Base[path, MAX_CHNL_GROUP_24G - 1] = 0;

            /* dummy diff */
            pwr_info.CCK_Diff[path, 0] = 0; /* 2.4G CCK-1TX */
            pwr_info.BW40_Diff[path, 0] = 0; /* 2.4G BW40-1S */
        }
    }

    static void hal_init_pg_txpwr_info_5g(_adapter adapter, TxPowerInfo5G pwr_info)
    {
        var hal_spec = GET_HAL_SPEC(adapter);
        u8 path, group, tx_idx;

        /* init with invalid value */
        for (path = 0; path<MAX_RF_PATH; path++) {
            for (group = 0; group < MAX_CHNL_GROUP_5G; group++)
            {
                pwr_info.IndexBW40_Base[path,group] = PG_TXPWR_INVALID_BASE;
            }
            for (tx_idx = 0; tx_idx<MAX_TX_COUNT; tx_idx++)
            {
                pwr_info.OFDM_Diff[path,tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW20_Diff[path,tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW40_Diff[path,tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW80_Diff[path,tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW160_Diff[path,tx_idx] = PG_TXPWR_INVALID_DIFF;
            }
        }

        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
                break;
            /* dummy diff */
            pwr_info.BW40_Diff[path,0] = 0; /* 5G BW40-1S */
        }
    }

    private static BAND_TYPE rtw_get_ch_group(u8 ch, ref u8 group, ref u8 cck_group)
    {
        BAND_TYPE band = BAND_TYPE.BAND_MAX;
        s8 gp = -1, cck_gp = -1;

        if (ch <= 14)
        {
            band = BAND_TYPE.BAND_ON_2_4G;

            if (1 <= ch && ch <= 2)
                gp = 0;
            else if (3 <= ch && ch <= 5)
                gp = 1;
            else if (6 <= ch && ch <= 8)
                gp = 2;
            else if (9 <= ch && ch <= 11)
                gp = 3;
            else if (12 <= ch && ch <= 14)
                gp = 4;
            else
                band = BAND_TYPE.BAND_MAX;

            if (ch == 14)
                cck_gp = 5;
            else
                cck_gp = gp;
        }
        else
        {
            band = BAND_TYPE.BAND_ON_5G;

            if (15 <= ch && ch <= 42)
                gp = 0;
            else if (44 <= ch && ch <= 48)
                gp = 1;
            else if (50 <= ch && ch <= 58)
                gp = 2;
            else if (60 <= ch && ch <= 80)
                gp = 3;
            else if (82 <= ch && ch <= 106)
                gp = 4;
            else if (108 <= ch && ch <= 114)
                gp = 5;
            else if (116 <= ch && ch <= 122)
                gp = 6;
            else if (124 <= ch && ch <= 130)
                gp = 7;
            else if (132 <= ch && ch <= 138)
                gp = 8;
            else if (140 <= ch && ch <= 144)
                gp = 9;
            else if (149 <= ch && ch <= 155)
                gp = 10;
            else if (157 <= ch && ch <= 161)
                gp = 11;
            else if (165 <= ch && ch <= 171)
                gp = 12;
            else if (173 <= ch && ch <= 177)
                gp = 13;
            else
                band = BAND_TYPE.BAND_MAX;
        }

        if (band == BAND_TYPE.BAND_MAX
            || (band == BAND_TYPE.BAND_ON_2_4G && cck_gp == -1)
            || gp == -1
           )
        {
            RTW_WARN("%s invalid channel:%u", "rtw_get_ch_group", ch);
            //rtw_warn_on(1);
            goto exit;
        }

        group = (byte)gp;

        if (band == BAND_TYPE.BAND_ON_2_4G)
        {
            if (cck_gp >= 0)
            {
                cck_group = (byte)cck_gp;
            }
        }

        exit:
        return band;
    }

    private static void Hal_ReadAmplifierType_8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        u8 extTypePA_2G_A = (byte)((PROMContent[0xBD] & BIT2) >> 2); /* 0xBD[2] */
        u8 extTypePA_2G_B = (byte)((PROMContent[0xBD] & BIT6) >> 6); /* 0xBD[6] */
        u8 extTypePA_5G_A = (byte)((PROMContent[0xBF] & BIT2) >> 2); /* 0xBF[2] */
        u8 extTypePA_5G_B = (byte)((PROMContent[0xBF] & BIT6) >> 6); /* 0xBF[6] */
        u8 extTypeLNA_2G_A = (byte)((PROMContent[0xBD] & (BIT1 | BIT0)) >> 0); /* 0xBD[1:0] */
        u8 extTypeLNA_2G_B = (byte)((PROMContent[0xBD] & (BIT5 | BIT4)) >> 4); /* 0xBD[5:4] */
        u8 extTypeLNA_5G_A = (byte)((PROMContent[0xBF] & (BIT1 | BIT0)) >> 0); /* 0xBF[1:0] */
        u8 extTypeLNA_5G_B = (byte)((PROMContent[0xBF] & (BIT5 | BIT4)) >> 4); /* 0xBF[5:4] */

        hal_ReadPAType_8812A(Adapter, PROMContent, AutoloadFail);

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

        RTW_INFO("pHalData.TypeGPA = 0x%X", pHalData.TypeGPA);
        RTW_INFO("pHalData.TypeAPA = 0x%X", pHalData.TypeAPA);
        RTW_INFO("pHalData.TypeGLNA = 0x%X", pHalData.TypeGLNA);
        RTW_INFO("pHalData.TypeALNA = 0x%X", pHalData.TypeALNA);
    }

    private const int EEPROM_PA_TYPE_8812AU = 0xBC;
    private const int EEPROM_LNA_TYPE_2G_8812AU = 0xBD;
    private const int EEPROM_LNA_TYPE_5G_8812AU = 0xBF;

    private static void hal_ReadPAType_8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        if (!AutoloadFail)
        {
            if (GetRegAmplifierType2G(Adapter) == 0)
            {
                /* AUTO */
                pHalData.PAType_2G = PROMContent[EEPROM_PA_TYPE_8812AU];
                pHalData.LNAType_2G = PROMContent[EEPROM_LNA_TYPE_2G_8812AU];
                if (pHalData.PAType_2G == 0xFF)
                    pHalData.PAType_2G = 0;
                if (pHalData.LNAType_2G == 0xFF)
                    pHalData.LNAType_2G = 0;

                pHalData.ExternalPA_2G = ((pHalData.PAType_2G & BIT5) != 0 && (pHalData.PAType_2G & BIT4) != 0);
                pHalData.ExternalLNA_2G = ((pHalData.LNAType_2G & BIT7) != 0 && (pHalData.LNAType_2G & BIT3) != 0);
            }
            else
            {
                pHalData.ExternalPA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_PA) != 0;
                pHalData.ExternalLNA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_LNA) != 0;
            }

            if (GetRegAmplifierType5G(Adapter) == 0)
            {
                /* AUTO */
                pHalData.PAType_5G = PROMContent[EEPROM_PA_TYPE_8812AU];
                pHalData.LNAType_5G = PROMContent[EEPROM_LNA_TYPE_5G_8812AU];
                if (pHalData.PAType_5G == 0xFF)
                    pHalData.PAType_5G = 0;
                if (pHalData.LNAType_5G == 0xFF)
                    pHalData.LNAType_5G = 0;

                pHalData.external_pa_5g = ((pHalData.PAType_5G & BIT1) != 0 && (pHalData.PAType_5G & BIT0) != 0);
                pHalData.external_lna_5g = ((pHalData.LNAType_5G & BIT7) != 0 && (pHalData.LNAType_5G & BIT3) != 0);
            }
            else
            {
                pHalData.external_pa_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_PA_5G) != 0;
                pHalData.external_lna_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }
        else
        {
            pHalData.ExternalPA_2G = false;
            pHalData.external_pa_5g = null;
            pHalData.ExternalLNA_2G = false;
            pHalData.external_lna_5g = null;

            if (GetRegAmplifierType2G(Adapter) == 0)
            {
                /* AUTO */
                pHalData.ExternalPA_2G = false;
                pHalData.ExternalLNA_2G = false;
            }
            else
            {
                pHalData.ExternalPA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_PA) != 0;
                pHalData.ExternalLNA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_LNA) != 0;
            }

            if (GetRegAmplifierType5G(Adapter) == 0)
            {
                /* AUTO */
                pHalData.external_pa_5g = false;
                pHalData.external_lna_5g = false;
            }
            else
            {
                pHalData.external_pa_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_PA_5G) != 0;
                pHalData.external_lna_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }

        RTW_INFO("pHalData.PAType_2G is 0x%x, pHalData.ExternalPA_2G = %d", pHalData.PAType_2G, pHalData.ExternalPA_2G);
        RTW_INFO("pHalData.PAType_5G is 0x%x, pHalData.external_pa_5g = %d", pHalData.PAType_5G,
            pHalData.external_pa_5g);
        RTW_INFO("pHalData.LNAType_2G is 0x%x, pHalData.ExternalLNA_2G = %d", pHalData.LNAType_2G,
            pHalData.ExternalLNA_2G);
        RTW_INFO("pHalData.LNAType_5G is 0x%x, pHalData.external_lna_5g = %d", pHalData.LNAType_5G,
            pHalData.external_lna_5g);
    }

    private static void Hal_EfuseParseXtal_8812A(PADAPTER pAdapter, u8[] hwinfo, bool AutoLoadFail)
    {
        var pHalData = GET_HAL_DATA(pAdapter);

        if (!AutoLoadFail)
        {
            pHalData.crystal_cap = hwinfo[EEPROM_XTAL_8812];
            if (pHalData.crystal_cap == 0xFF)
                pHalData.crystal_cap = EEPROM_Default_CrystalCap_8812; /* what value should 8812 set? */
        }
        else
        {
            pHalData.crystal_cap = EEPROM_Default_CrystalCap_8812;
        }

        RTW_INFO("crystal_cap: 0x%2x", pHalData.crystal_cap);
    }

    private static bool hal_config_macaddr(_adapter adapter, bool autoload_fail)
    {
        // TODO: Looks like not needed
//         var pdvobjpriv = adapter_to_dvobj(adapter);
//
//         HAL_DATA_TYPE* hal_data = GET_HAL_DATA(adapter);
//         u8 addr[ETH_ALEN];
//         int addr_offset = hal_efuse_macaddr_offset(adapter);
//         u8* hw_addr = null;
//         int ret = _SUCCESS;
//
//         if (autoload_fail)
//             goto bypass_hw_pg;
//
//         if (addr_offset != -1)
//             hw_addr = &hal_data.efuse_eeprom_data[addr_offset];
//
// #ifdef CONFIG_EFUSE_CONFIG_FILE
//         /* if the hw_addr is written by efuse file, set to null */
//         if (hal_data.efuse_file_status == EFUSE_FILE_LOADED)
//             hw_addr = null;
// #endif
//
//         if (!hw_addr) {
//             /* try getting hw pg data */
//             if (Hal_GetPhyEfuseMACAddr(adapter, addr) == _SUCCESS)
//                 hw_addr = addr;
//         }
//
//         /* check hw pg data */
//         if (hw_addr && rtw_check_invalid_mac_address(hw_addr, _TRUE) == _FALSE) {
//             _rtw_memcpy(hal_data.EEPROMMACAddr, hw_addr, ETH_ALEN);
//             goto exit;
//         }
//
//         bypass_hw_pg:
//
// # ifdef CONFIG_EFUSE_CONFIG_FILE
// /* check wifi mac file */
//         if (Hal_ReadMACAddrFromFile(adapter, addr) == _SUCCESS)
//         {
//             _rtw_memcpy(hal_data.EEPROMMACAddr, addr, ETH_ALEN);
//             goto exit;
//         }
// #endif
//
//         _rtw_memset(hal_data.EEPROMMACAddr, 0, ETH_ALEN);
//         ret = _FAIL;
//         exit:
//         dev_info(&udev.dev, "88XXau %pM hw_info[%02x]", hw_addr, addr_offset);
        return true;
    }

    static void Hal_ReadPROMVersion8812A(PADAPTER Adapter, u8[] PROMContent, bool AutoloadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        if (AutoloadFail)
        {
            pHalData.EEPROMVersion = EEPROM_Default_Version;
        }
        else
        {
            pHalData.EEPROMVersion = PROMContent[EEPROM_VERSION_8812];


            if (pHalData.EEPROMVersion == 0xFF)
            {
                pHalData.EEPROMVersion = EEPROM_Default_Version;
            }
        }

        RTW_INFO("pHalData.EEPROMVersion is 0x%x", pHalData.EEPROMVersion);
    }

    private static void hal_ReadIDs_8812AU(PADAPTER Adapter, byte[] PROMContent, bool AutoloadFail)
    {
        // TODO: Looks like not needed
        // HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        //
        // if (!AutoloadFail)
        // {
        //     /* VID, PID */
        //     if (IS_HARDWARE_TYPE_8812AU(Adapter))
        //     {
        //         pHalData.EEPROMVID = ReadLE2Byte(&PROMContent[EEPROM_VID_8812AU]);
        //         pHalData.EEPROMPID = ReadLE2Byte(&PROMContent[EEPROM_PID_8812AU]);
        //     }
        //
        //
        //     /* Customer ID, 0x00 and 0xff are reserved for Realtek.		 */
        //     pHalData.EEPROMCustomerID = *(u8*)&PROMContent[EEPROM_CustomID_8812];
        //     pHalData.EEPROMSubCustomerID = EEPROM_Default_SubCustomerID;
        //
        // }
        // else
        // {
        //     pHalData.EEPROMVID = EEPROM_Default_VID;
        //     pHalData.EEPROMPID = EEPROM_Default_PID;
        //
        //     /* Customer ID, 0x00 and 0xff are reserved for Realtek.		 */
        //     pHalData.EEPROMCustomerID = EEPROM_Default_CustomerID;
        //     pHalData.EEPROMSubCustomerID = EEPROM_Default_SubCustomerID;
        //
        // }
        //
        // if ((pHalData.EEPROMVID == 0x050D) && (pHalData.EEPROMPID == 0x1106)) /* SerComm for Belkin. */
        //     pHalData.CustomerID = RT_CID_819x_Sercomm_Belkin;
        // else if ((pHalData.EEPROMVID == 0x0846) && (pHalData.EEPROMPID == 0x9051)) /* SerComm for Netgear. */
        //     pHalData.CustomerID = RT_CID_819x_Sercomm_Netgear;
        // else if ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x330e)) /* add by ylb 20121012 for customer led for alpha */
        //     pHalData.CustomerID = RT_CID_819x_ALPHA_Dlink;
        // else if ((pHalData.EEPROMVID == 0x0B05) && (pHalData.EEPROMPID == 0x17D2)) /* Edimax for ASUS */
        //     pHalData.CustomerID = RT_CID_819x_Edimax_ASUS;
        // else if ((pHalData.EEPROMVID == 0x0846) && (pHalData.EEPROMPID == 0x9052))
        //     pHalData.CustomerID = RT_CID_NETGEAR;
        // else if ((pHalData.EEPROMVID == 0x0411) && ((pHalData.EEPROMPID == 0x0242) || (pHalData.EEPROMPID == 0x025D)))
        //     pHalData.CustomerID = RT_CID_DNI_BUFFALO;
        // else if (((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3314)) ||
        //     ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x804B)) ||
        //     ((pHalData.EEPROMVID == 0x20F4) && (pHalData.EEPROMPID == 0x805B)) ||
        //     ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3315)) ||
        //     ((pHalData.EEPROMVID == 0x2001) && (pHalData.EEPROMPID == 0x3316)))
        //     pHalData.CustomerID = RT_CID_DLINK;
        //
        // RTW_INFO("VID = 0x%04X, PID = 0x%04X\n", pHalData.EEPROMVID, pHalData.EEPROMPID);
        // RTW_INFO("Customer ID: 0x%02X, SubCustomer ID: 0x%02X\n", pHalData.EEPROMCustomerID, pHalData.EEPROMSubCustomerID);
    }

    static void Hal_EfuseParseIDCode8812A(PADAPTER padapter, u8[] hwinfo)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        u16 EEPROMId;


        /* Checl 0x8129 again for making sure autoload status!! */
        EEPROMId = BinaryPrimitives.ReadUInt16LittleEndian(hwinfo);
        if (EEPROMId != RTL_EEPROM_ID)
        {
            RTW_INFO("EEPROM ID(%#x) is invalid!!\n", EEPROMId);
            pHalData.bautoload_fail_flag = true;
        }
        else
            pHalData.bautoload_fail_flag = false;

        RTW_INFO("EEPROM ID=0x%04x\n", EEPROMId);
    }

    static void hal_InitPGData_8812A(PADAPTER padapter, u8[] PROMContent)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        u32 i;
        u16 value16;

        if (false == pHalData.bautoload_fail_flag)
        {
            /* autoload OK. */
            if (is_boot_from_eeprom(padapter))
            {
                /* Read all Content from EEPROM or EFUSE. */
                for (i = 0; i < HWSET_MAX_SIZE_JAGUAR; i += 2)
                {
                    /* value16 = EF2Byte(ReadEEprom(pAdapter, (u2Byte) (i>>1))); */
                    /* *((u16*)(&PROMContent[i])) = value16; */
                }
            }
            else
            {
                /*  */
                /* 2013/03/08 MH Add for 8812A HW limitation, ROM code can only */
                /*  */
                if (IS_HARDWARE_TYPE_8812AU(padapter))
                {
                    var efuse_content = new byte[4];
                    efuse_OneByteRead(padapter, 0x200, out efuse_content[0]);
                    efuse_OneByteRead(padapter, 0x202, out efuse_content[1]);
                    efuse_OneByteRead(padapter, 0x204, out efuse_content[2]);
                    efuse_OneByteRead(padapter, 0x210, out efuse_content[3]);
                    if (efuse_content[0] != 0xFF ||
                        efuse_content[1] != 0xFF ||
                        efuse_content[2] != 0xFF ||
                        efuse_content[3] != 0xFF)
                    {
                        /* DbgPrint("Disable FW ofl load\n"); */
                        /* pMgntInfo.RegFWOffload = FALSE; */
                    }
                }

                /* Read EFUSE real map to shadow. */
                EFUSE_ShadowMapUpdate(padapter, EFUSE_WIFI);
            }
        }
        else
        {
            /* autoload fail */
            /* pHalData.AutoloadFailFlag = _TRUE; */
            /*  */
            /* 2013/03/08 MH Add for 8812A HW limitation, ROM code can only */
            /*  */
            if (IS_HARDWARE_TYPE_8812AU(padapter))
            {
                var efuse_content = new byte[4];
                efuse_OneByteRead(padapter, 0x200, out efuse_content[0]);
                efuse_OneByteRead(padapter, 0x202, out efuse_content[1]);
                efuse_OneByteRead(padapter, 0x204, out efuse_content[2]);
                efuse_OneByteRead(padapter, 0x210, out efuse_content[3]);
                if (efuse_content[0] != 0xFF ||
                    efuse_content[1] != 0xFF ||
                    efuse_content[2] != 0xFF ||
                    efuse_content[3] != 0xFF)
                {
                    /* DbgPrint("Disable FW ofl load\n"); */
                    /* pMgntInfo.RegFWOffload = FALSE; */
                    pHalData.bautoload_fail_flag = false;
                }
                else
                {
                    /* DbgPrint("EFUSE_Read1Byte(pAdapter, (u2Byte)512) = %x\n", EFUSE_Read1Byte(pAdapter, (u2Byte)512)); */
                }
            }

            /* update to default value 0xFF */
            if (!is_boot_from_eeprom(padapter))
            {
                EFUSE_ShadowMapUpdate(padapter, EFUSE_WIFI);
            }
        }


        if (check_phy_efuse_tx_power_info_valid(padapter) == false)
        {
            throw new NotImplementedException("Hal_readPGDataFromConfigFile");
            // TODO: Maybe now not need
            //if (Hal_readPGDataFromConfigFile() != true)
            {
                // TODO: RTW_ERR("invalid phy efuse and read from file fail, will use driver default!!\n");
            }
        }

    }

    private static bool check_phy_efuse_tx_power_info_valid(_adapter adapter)
    {
        var pContent = adapter.HalData.efuse_eeprom_data;
        int index = 0;
        UInt16 tx_index_offset = 0x0000;

        // Just because single chip support
        tx_index_offset = EEPROM_TX_PWR_INX_8812;

        /* TODO: chacking length by ICs */
        for (index = 0; index < 11; index++)
        {
            if (pContent[tx_index_offset + index] == 0xFF)
            {
                return false;
            }
        }

        return true;
    }

    private static void EFUSE_ShadowMapUpdate(PADAPTER pAdapter, byte efuseType)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        UInt16 mapLen = 0;

        if (pHalData.bautoload_fail_flag == true)
        {
            for (int i = 0; i < pHalData.efuse_eeprom_data.Length; i++)
            {
                pHalData.efuse_eeprom_data[i] = 0xFF;
            }
        }
        else
        {
            Efuse_ReadAllMap(pAdapter, efuseType, pHalData.efuse_eeprom_data);
        }

        rtw_mask_map_read(0x00, mapLen, pHalData.efuse_eeprom_data);

        //rtw_dump_cur_efuse(pAdapter);
    }

    static void Efuse_ReadAllMap(_adapter adapter, byte efuseType, byte[] Efuse)
    {
        EfusePowerSwitch8812A(adapter, false, true);
        efuse_ReadEFuse(adapter, efuseType, 0, EFUSE_MAP_LEN_JAGUAR, Efuse);
        EfusePowerSwitch8812A(adapter, false, false);
    }

    private static void efuse_ReadEFuse(_adapter adapter, byte efuseType, UInt16 _offset, UInt16 _size_byte, byte[] pbuf)
    {
        if (efuseType == EFUSE_WIFI)
        {
            Hal_EfuseReadEFuse8812A(adapter, _offset, _size_byte, pbuf);
        }
        else
        {
            throw new NotImplementedException();
            // hal_ReadEFuse_BT(Adapter, _offset, _size_byte, pbuf, bPseudoTest);
        }
    }

    private static void Hal_EfuseReadEFuse8812A(_adapter adapter, UInt16 _offset, UInt16 _size_byte, byte[] pbuf)
    {
        byte[] efuseTbl = null;
        byte[] rtemp8 = new byte[1];
        UInt16 eFuse_Addr = 0;
        byte offset, wren;
        UInt16 i, j;
        UInt16[][] eFuseWord = null;
        UInt16 efuse_utilized = 0;
        byte efuse_usage = 0;
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
        ReadEFuseByte(adapter, eFuse_Addr, rtemp8);
        if (rtemp8[0] != 0xFF)
        {
            efuse_utilized++;
            /* RTW_INFO("efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8); */
            eFuse_Addr++;
        }
        else
        {
            // TODO: RTW_INFO("EFUSE is empty efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8);
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

                ReadEFuseByte(adapter, eFuse_Addr, rtemp8);

                /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("extended header efuse_Addr-%d efuse_data=%x\n", eFuse_Addr, *rtemp8));	 */

                if ((rtemp8[0] & 0x0F) == 0x0F)
                {
                    eFuse_Addr++;
                    ReadEFuseByte(adapter, eFuse_Addr, rtemp8);

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
                        ReadEFuseByte(adapter, eFuse_Addr, rtemp8);
                        eFuse_Addr++;
                        efuse_utilized++;
                        eFuseWord[offset][i] = (byte)(rtemp8[0] & 0xff);


                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;

                        /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d", eFuse_Addr)); */
                        ReadEFuseByte(adapter, eFuse_Addr, rtemp8);
                        eFuse_Addr++;

                        efuse_utilized++;
                        eFuseWord[offset][i] |= (byte)(((rtemp8[0]) << 8) & 0xff00);

                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                    }

                    wren >>= 1;

                }
            }
            else
            {
                /* deal with error offset,skip error data		 */
                // TODO: RTW_PRINT("invalid offset:0x%02x\n", offset);
                for (i = 0; i < EFUSE_MAX_WORD_UNIT; i++)
                {
                    /* Check word enable condition in the section				 */
                    if (!((wren & 0x01) == 0x01))
                    {
                        eFuse_Addr++;
                        efuse_utilized++;
                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                        eFuse_Addr++;
                        efuse_utilized++;
                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;
                    }
                }
            }

            /* Read next PG header */
            ReadEFuseByte(adapter, eFuse_Addr, rtemp8);
            /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d rtemp 0x%x\n", eFuse_Addr, *rtemp8)); */

            if (rtemp8[0] != 0xFF && (eFuse_Addr < EFUSE_REAL_CONTENT_LEN_JAGUAR))
            {
                efuse_utilized++;
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
        efuse_usage = (byte)((eFuse_Addr * 100) / EFUSE_REAL_CONTENT_LEN_JAGUAR);
        // TODO: SetHwReg8812AU(HW_VARIABLES.HW_VAR_EFUSE_BYTES, (u8*)&eFuse_Addr);
        // TODO: RTW_INFO("%s: eFuse_Addr offset(%#x) !!\n", __FUNCTION__, eFuse_Addr);
    }

    private static void rtw_mask_map_read(UInt16 addr, UInt16 cnts, byte[] data)
    {
        UInt16 i = 0;
        // TODO:
        //if (registrypriv.boffefusemask == 0)
        //{

        //    for (i = 0; i < cnts; i++)
        //    {
        //        if (registrypriv.bFileMaskEfuse == true)
        //        {
        //            if (rtw_file_efuse_IsMasked(padapter, addr + i)) /*use file efuse mask.*/
        //                data[i] = 0xff;
        //        }
        //        else
        //        {
        //            /*RTW_INFO(" %s , data[%d] = %x\n", __func__, i, data[i]);*/
        //            if (efuse_IsMasked(padapter, addr + i))
        //            {
        //                data[i] = 0xff;
        //                /*RTW_INFO(" %s ,mask data[%d] = %x\n", __func__, i, data[i]);*/
        //            }
        //        }
        //    }

        //}
    }

    private static void EfusePowerSwitch8812A(_adapter adapter, bool bWrite, bool pwrState)
    {
        UInt16 tmpV16;
        const byte EFUSE_ACCESS_ON_JAGUAR = 0x69;
        const byte EFUSE_ACCESS_OFF_JAGUAR = 0x00;
        if (pwrState)
        {
            rtw_write8(adapter, REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_ON_JAGUAR);

            /* 1.2V Power: From VDDON with Power Cut(0x0000h[15]), defualt valid */
            tmpV16 = rtw_read16(adapter, REG_SYS_ISO_CTRL);
            if (!((tmpV16 & SysIsoCtrlBits.PWC_EV12V) == SysIsoCtrlBits.PWC_EV12V))
            {
                tmpV16 |= SysIsoCtrlBits.PWC_EV12V;
                /* Write16(pAdapter,REG_SYS_ISO_CTRL,tmpV16); */
            }

            /* Reset: 0x0000h[28], default valid */
            tmpV16 = rtw_read16(adapter, REG_SYS_FUNC_EN);
            if (!((tmpV16 & SysFuncEnBits.FEN_ELDR) == SysFuncEnBits.FEN_ELDR))
            {
                tmpV16 |= SysFuncEnBits.FEN_ELDR;
                rtw_write16(adapter, REG_SYS_FUNC_EN, tmpV16);
            }

            /* Clock: Gated(0x0008h[5]) 8M(0x0008h[1]) clock from ANA, default valid */
            tmpV16 = rtw_read16(adapter, REG_SYS_CLKR);
            if ((!((tmpV16 & SysClkrBits.LOADER_CLK_EN) == SysClkrBits.LOADER_CLK_EN)) ||
                (!((tmpV16 & SysClkrBits.ANA8M) == SysClkrBits.ANA8M)))
            {
                tmpV16 |= (SysClkrBits.LOADER_CLK_EN | SysClkrBits.ANA8M);
                rtw_write16(adapter, REG_SYS_CLKR, tmpV16);
            }

            if (bWrite)
            {
                /* Enable LDO 2.5V before read/write action */
                var tempval = rtw_read8(adapter, REG_EFUSE_TEST + 3);
                //tempval &= ~(BIT3 | BIT4 | BIT5 | BIT6);
                //tempval &= (0b1111_0111 & 0b1110_1111 & 0b1101_1111 & 0b1011_1111);
                tempval &= 0b1000_0111;
                tempval |= (VoltageValues.VOLTAGE_V25 << 3);
                tempval |= 0b1000_0000;
                rtw_write8(adapter, REG_EFUSE_TEST + 3, tempval);
            }
        }
        else
        {
            rtw_write8(adapter, REG_EFUSE_BURN_GNT_8812, EFUSE_ACCESS_OFF_JAGUAR);

            if (bWrite)
            {
                /* Disable LDO 2.5V after read/write action */
                var tempval = rtw_read8(adapter, REG_EFUSE_TEST + 3);
                rtw_write8(adapter, REG_EFUSE_TEST + 3, (byte)(tempval & 0x7F));
            }
        }

    }

    static bool efuse_OneByteRead(PADAPTER pAdapter, UInt16 addr, out byte data)
    {
        /* -----------------e-fuse reg ctrl --------------------------------- */
        /* address			 */
        var addressBytes = new byte[2];
        BinaryPrimitives.TryWriteUInt16LittleEndian(addressBytes, addr);
        rtw_write8(pAdapter, EFUSE_CTRL + 1, addressBytes[0]);
        var tmpRead = rtw_read8(pAdapter, EFUSE_CTRL + 2);
        var secondAddr = (addressBytes[1] & 0x03) | (tmpRead & 0xFC);
        rtw_write8(pAdapter, EFUSE_CTRL + (2), (byte)secondAddr);

        /* Write8(pAdapter, EFUSE_CTRL+3,  0x72); */
        /* read cmd	 */
        /* Write bit 32 0 */
        var readbyte = rtw_read8(pAdapter, EFUSE_CTRL + 3);
        rtw_write8(pAdapter, EFUSE_CTRL + 3, (byte)(readbyte & 0x7f));


        UInt32 tmpidx = 0;
        while ((0x80 & rtw_read8(pAdapter, EFUSE_CTRL + (3))) == 0 && (tmpidx < 1000))
        {
            Thread.Sleep(1);
            tmpidx++;
        }

        bool bResult;
        if (tmpidx < 100)
        {
            data = rtw_read8(pAdapter, EFUSE_CTRL);
            bResult = true;
        }
        else
        {
            data = 0xff;
            bResult = false;
            //RTW_INFO("%s: [ERROR] addr=0x%x bResult=%d time out 1s !!!\n", __FUNCTION__, addr, bResult);
            //RTW_INFO("%s: [ERROR] EFUSE_CTRL =0x%08x !!!\n", __FUNCTION__, rtw_read32(pAdapter, EFUSE_CTRL));
        }

        return bResult;
    }

    static void ReadEFuseByte(_adapter adapter, UInt16 _offset, byte[] pbuf)
    {
        UInt32 value32;
        byte readbyte;
        UInt16 retry;
        /* systime start=rtw_get_current_time(); */

        /* Write Address */
        rtw_write8(adapter, EFUSE_CTRL + 1, (byte)(_offset & 0xff));
        readbyte = rtw_read8(adapter, EFUSE_CTRL + (2));
        rtw_write8(adapter, EFUSE_CTRL + (2), (byte)(((_offset >> 8) & 0x03) | (readbyte & 0xfc)));

        /* Write bit 32 0 */
        readbyte = rtw_read8(adapter, EFUSE_CTRL + (3));
        rtw_write8(adapter, EFUSE_CTRL + (3), (byte)(readbyte & 0x7f));

        /* Check bit 32 read-ready */
        retry = 0;
        value32 = rtw_read32(adapter, EFUSE_CTRL + (0));
        /* while(!(((value32 >> 24) & 0xff) & 0x80)  && (retry<10)) */
        while (!((((value32 >> 24) & 0xff) & 0x80) == 0x80) && (retry < 10000))
        {
            value32 = rtw_read32(adapter, EFUSE_CTRL + (0));
            retry++;
        }

        /* 20100205 Joseph: Add delay suggested by SD1 Victor. */
        /* This fix the problem that Efuse read error in high temperature condition. */
        /* Designer says that there shall be some delay after ready bit is set, or the */
        /* result will always stay on last data we read. */
        Thread.Sleep(50);
        value32 = rtw_read32(adapter, EFUSE_CTRL + (0));

        pbuf[0] = (byte)(value32 & 0xff);
        /* RTW_INFO("ReadEFuseByte _offset:%08u, in %d ms\n",_offset ,rtw_get_passing_time_ms(start)); */

    }

    static void _ConfigChipOutEP_8812(PADAPTER pAdapter, u8 NumOutPipe)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);


        pHalData.OutEpQueueSel = 0;
        pHalData.OutEpNumber = 0;

        switch (NumOutPipe)
        {
            case 4:
                pHalData.OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ | TxSele.TX_SELE_EQ;
                pHalData.OutEpNumber = 4;
                break;
            case 3:
                pHalData.OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ;
                pHalData.OutEpNumber = 3;
                break;
            case 2:
                pHalData.OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_NQ;
                pHalData.OutEpNumber = 2;
                break;
            case 1:
                pHalData.OutEpQueueSel = TxSele.TX_SELE_HQ;
                pHalData.OutEpNumber = 1;
                break;
            default:
                break;

        }

        RTW_INFO("%s OutEpQueueSel(0x%02x), OutEpNumber(%d)", "_ConfigChipOutEP_8812", pHalData.OutEpQueueSel,
            pHalData.OutEpNumber);

    }

    private static bool is_boot_from_eeprom(_adapter adapter) => (GET_HAL_DATA(adapter).EepromOrEfuse);

    private static map_seg_t MAPSEG_PTR_ENT(UInt16 _sa, u16 _len, u8[] _p)
    {
        return new map_seg_t()
        {
            sa = _sa,
            len = _len,
            c = _p
        };

    }

    private static map_t MAP_ENT(ushort _len, ushort _seg_num, byte _init_v, map_seg_t _seg)
    {
        var t = new map_t()
        {
            len = _len,
            seg_num = _seg_num,
            init_value = _init_v,
            segs = new map_seg_t[_seg_num]
        };

        t.segs[0] = _seg;
        return t;
    }
}