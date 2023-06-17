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

    public static u8[] center_ch_5g_all = new byte[CENTER_CH_5G_ALL_NUM]
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
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);

        dvobj_priv pdvobjpriv = adapter_to_dvobj(padapter);

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

        pHalData.UsbTxAggMode = true;
        pHalData.UsbTxAggDescNum = 6; /* only 4 bits */
        pHalData.UsbTxAggDescNum = 0x01; /* adjust value for OQT  Overflow issue */ /* 0x3;	 */ /* only 4 bits */
        pHalData.rxagg_mode = RX_AGG_MODE.RX_AGG_USB;
        pHalData.rxagg_usb_size = 8; /* unit: 512b */
        pHalData.rxagg_usb_timeout = 0x6;
        pHalData.rxagg_dma_size = 16; /* uint: 128b, 0x0A = 10 = MAX_RX_DMA_BUFFER_SIZE/2/pHalData.UsbBulkOutSize */
        pHalData.rxagg_dma_timeout = 0x6; /* 6, absolute time = 34ms/(2^6) */

        if (IS_SUPER_SPEED_USB(padapter))
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
            result = Hal_MappingOutPipe(pAdapter, NumOutPipe);
        }

        return result;

    }

    /// <remarks>
    /// NumOutPipe == 3 for tenda
    /// </remarks>>
    static BOOLEAN Hal_MappingOutPipe(PADAPTER pAdapter, u8 NumOutPipe)
    {
        registry_priv pregistrypriv = pAdapter.registrypriv;

        BOOLEAN bWIFICfg = (pregistrypriv.wifi_spec) ? true : false;

        BOOLEAN result = true;

        switch (NumOutPipe)
        {
            //    case 2:
            //        _TwoOutPipeMapping(pAdapter, bWIFICfg);
            //        break;
            case 3:
            case 4:
            case 5:
            case 6:
                _ThreeOutPipeMapping(pAdapter, bWIFICfg);
                break;
            //    case 1:
            //        _OneOutPipeMapping(pAdapter);
            //        break;
            default:
                result = false;
                break;
        }

        return result;

    }

    static void _ThreeOutPipeMapping(PADAPTER pAdapter, BOOLEAN bWIFICfg)
    {

        dvobj_priv pdvobjpriv = adapter_to_dvobj(pAdapter);

        if (bWIFICfg)
        {
            /* for WMM */

            /*	BK, 	BE, 	VI, 	VO, 	BCN,	CMD,MGT,HIGH,HCCA  */
            /* {  1, 	2, 	1, 	0, 	0, 	0, 	0, 	0, 		0	}; */
            /* 0:H, 1:N, 2:L */

            pdvobjpriv.Queue2Pipe[0] = pdvobjpriv.RtOutPipe[0]; /* VO */
            pdvobjpriv.Queue2Pipe[1] = pdvobjpriv.RtOutPipe[1]; /* VI */
            pdvobjpriv.Queue2Pipe[2] = pdvobjpriv.RtOutPipe[2]; /* BE */
            pdvobjpriv.Queue2Pipe[3] = pdvobjpriv.RtOutPipe[1]; /* BK */

            pdvobjpriv.Queue2Pipe[4] = pdvobjpriv.RtOutPipe[0]; /* BCN */
            pdvobjpriv.Queue2Pipe[5] = pdvobjpriv.RtOutPipe[0]; /* MGT */
            pdvobjpriv.Queue2Pipe[6] = pdvobjpriv.RtOutPipe[0]; /* HIGH */
            pdvobjpriv.Queue2Pipe[7] = pdvobjpriv.RtOutPipe[0]; /* TXCMD */

        }
        else
        {
            /* typical setting */


            /*	BK, 	BE, 	VI, 	VO, 	BCN,	CMD,MGT,HIGH,HCCA  */
            /* {  2, 	2, 	1, 	0, 	0, 	0, 	0, 	0, 		0	};			 */
            /* 0:H, 1:N, 2:L */

            pdvobjpriv.Queue2Pipe[0] = pdvobjpriv.RtOutPipe[0]; /* VO */
            pdvobjpriv.Queue2Pipe[1] = pdvobjpriv.RtOutPipe[1]; /* VI */
            pdvobjpriv.Queue2Pipe[2] = pdvobjpriv.RtOutPipe[2]; /* BE */
            pdvobjpriv.Queue2Pipe[3] = pdvobjpriv.RtOutPipe[2]; /* BK */

            pdvobjpriv.Queue2Pipe[4] = pdvobjpriv.RtOutPipe[0]; /* BCN */
            pdvobjpriv.Queue2Pipe[5] = pdvobjpriv.RtOutPipe[0]; /* MGT */
            pdvobjpriv.Queue2Pipe[6] = pdvobjpriv.RtOutPipe[0]; /* HIGH */
            pdvobjpriv.Queue2Pipe[7] = pdvobjpriv.RtOutPipe[0]; /* TXCMD	 */
        }

    }

    public static void ReadAdapterInfo8812AU(PADAPTER Adapter)
    {
        /* Read all content in Efuse/EEPROM. */
        Hal_ReadPROMContent_8812A(Adapter);
    }

    static void Hal_ReadPROMContent_8812A(PADAPTER Adapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u8 eeValue;

        /* check system boot selection */
        eeValue = rtw_read8(Adapter, REG_9346CR);
        pHalData.EepromOrEfuse = (eeValue & BOOT_FROM_EEPROM) != 0 ? true : false;
        pHalData.bautoload_fail_flag = (eeValue & EEPROM_EN) != 0 ? false : true;

        RTW_INFO(
            $"Boot from {(pHalData.EepromOrEfuse ? "EEPROM" : "EFUSE")}, Autoload {(pHalData.bautoload_fail_flag ? "Fail" : "OK")} !");

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

    static void ReadLEDSetting_8812AU(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
    {
//#ifdef CONFIG_RTW_LED
//        struct led_priv *pledpriv = adapter_to_led(Adapter);

//# ifdef CONFIG_RTW_SW_LED
//        pledpriv.bRegUseLed = true;
//#else /* HW LED */
//        pledpriv.LedStrategy = HW_LED;
//#endif /* CONFIG_RTW_SW_LED */
//#endif
    }

    static void hal_CustomizeByCustomerID_8812AU(PADAPTER pAdapter)
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
        //            /* pHalData.bAutoConnectEnable = false; */
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

    private static void hal_ReadUsbModeSwitch_8812AU(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
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

    private static void Hal_ReadRFEType_8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
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

    static void Hal_ReadChannelPlan8812A(PADAPTER padapter, u8[] hwinfo, BOOLEAN AutoLoadFail)
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
////		force_hw_chplan = true;
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

    static void Hal_ReadThermalMeter_8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
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
            pHalData.eeprom_thermal_meter = 0xFF;
        }

        /* pHalData.ThermalMeter[0] = pHalData.eeprom_thermal_meter;	 */
        RTW_INFO("ThermalMeter = 0x%x\n", pHalData.eeprom_thermal_meter);
    }

    static void Hal_EfuseParseBTCoexistInfo8812A(PADAPTER Adapter, u8[] hwinfo, BOOLEAN AutoLoadFail)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

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

    static void Hal_ReadBoardType8812A(PADAPTER Adapter, u8[] PROMContent, BOOLEAN AutoloadFail)
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

    private static void hal_load_txpwr_info(_adapter adapter, TxPowerInfo24G pwr_info_2g, TxPowerInfo5G pwr_info_5g,
        u8[] pg_data)
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

    private static void hal_load_pg_txpwr_info(_adapter adapter, TxPowerInfo24G pwr_info_2g, TxPowerInfo5G pwr_info_5g,
        u8[] pg_data, BOOLEAN AutoLoadFail)
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

        if (hal_chk_pg_txpwr_info_2g(adapter, pwr_info_2g) &&
            hal_chk_pg_txpwr_info_5g(adapter, pwr_info_5g))
        {
            goto exit;
        }

        end_parse:
        txpwr_src++;
        if (txpwr_src < PG_TXPWR_SRC_NUM)
            goto select_src;

        if (hal_chk_pg_txpwr_info_2g(adapter, pwr_info_2g) || hal_chk_pg_txpwr_info_5g(adapter, pwr_info_5g))
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

        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
                continue;
            for (group = 0; group < MAX_CHNL_GROUP_5G; group++)
                if (IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path, group]))
                {
                    return false;
                }

            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
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

    //static u8 map_read8(map_t map, u16 offset)
    //{
    //    map_seg_t seg;
    //    u8 val = map.init_value;
    //    int i;

    //    if (offset + 1 > map.len)
    //    {
    //        throw new Exception("WTF");
    //        goto exit;
    //    }

    //    for (i = 0; i < map.seg_num; i++)
    //    {
    //        seg = map.segs + i;
    //        if (seg.sa + seg.len <= offset || seg.sa >= offset + 1)
    //        {
    //            continue;
    //        }

    //        val = * (seg.c + offset - seg.sa);
    //        break;
    //    }

    //    exit:
    //    return val;
    //}

    static u8 map_read8(map_t map, u16 offset)
    {
        map_seg_t seg = map.segs[0];
        u8 val = map.init_value;
        int i;

        if (offset + 1 > map.len)
        {
            throw new Exception("WTF");
            goto exit;
        }

        val = seg.c[offset - seg.sa];

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

    static string[] _pg_txpwr_src_str =
    {
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
            RTW_INFO($"{rf_path_char(path)}[hal_load_pg_txpwr_info_path_5g] eaddr:{offset}");
        }

        for (group = 0; group < MAX_CHNL_GROUP_5G; group++)
        {
            if (HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
            {
                tmp_base = map_read8(txpwr_map, offset);
                if (!IS_PG_TXPWR_BASE_INVALID(hal_spec, tmp_base)
                    && IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexBW40_Base[path, group])
                   )
                {
                    pwr_info.IndexBW40_Base[path, group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO(
                            $"[{rf_path_char(path)}] 5G G{group} BW40-1S base:{tmp_base} from {pg_txpwr_src_str(txpwr_src)}");
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
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO(
                                $"[{rf_path_char(path)}] 5G BW20-{tx_idx + 1} diff:{tmp_diff} from {pg_txpwr_src_str(txpwr_src)}");
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.OFDM_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO(
                                $"[{rf_path_char(path)}] 5G OFDM-{tx_idx + 1} diff:{tmp_diff} from {pg_txpwr_src_str(txpwr_src)}");
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
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW40_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G BW40-%dS diff:%d from %s\n",
                                rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G BW20-%dS diff:%d from %s\n",
                                rf_path_char(path), tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
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
                pwr_info.OFDM_Diff[path, 1] = tmp_diff;
                if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                {
                    RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), 2,
                        tmp_diff, pg_txpwr_src_str(txpwr_src));
                }
            }

            if (HAL_SPEC_CHK_TX_CNT(hal_spec, 2))
            {
                tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, 2]))
                {
                    pwr_info.OFDM_Diff[path, 2] = tmp_diff;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path),
                            3, tmp_diff, pg_txpwr_src_str(txpwr_src));
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
                pwr_info.OFDM_Diff[path, 3] = tmp_diff;
                if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                {
                    RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G OFDM-%dT diff:%d from %s\n", rf_path_char(path), 4,
                        tmp_diff, pg_txpwr_src_str(txpwr_src));
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
                        RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G BW80-%dS diff:%d from %s\n", rf_path_char(path),
                            tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                }

                tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW160_Diff[path, tx_idx])
                   )
                {
                    pwr_info.BW160_Diff[path, tx_idx] = tmp_diff;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO("[hal_load_pg_txpwr_info_path_5g] 5G BW160-%dS diff:%d from %s\n", rf_path_char(path),
                            tx_idx + 1, tmp_diff, pg_txpwr_src_str(txpwr_src));
                    }
                }
            }

            offset++;
        }

        if (offset != pg_offset + PG_TXPWR_1PATH_BYTE_NUM_5G)
        {
            RTW_ERR(
                $"[hal_load_pg_txpwr_info_path_5g] parse {offset - pg_offset} bytes != {PG_TXPWR_1PATH_BYTE_NUM_5G}");
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
            RTW_INFO($"hal_load_pg_txpwr_info_path_2g [{rf_path_char(path)}] offset:0x{offset:H3}\n");
        }

        for (group = 0; group < MAX_CHNL_GROUP_24G; group++)
        {
            if (HAL_SPEC_CHK_RF_PATH_2G(hal_spec, path))
            {
                tmp_base = map_read8(txpwr_map, offset);
                if (!IS_PG_TXPWR_BASE_INVALID(hal_spec, tmp_base) &&
                    IS_PG_TXPWR_BASE_INVALID(hal_spec, pwr_info.IndexCCK_Base[path, group])
                   )
                {
                    pwr_info.IndexCCK_Base[path, group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO(
                            $"[{rf_path_char(path)}] 2G G{group:2} CCK-1T base:{tmp_base} from {pg_txpwr_src_str(txpwr_src)}");
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
                    pwr_info.IndexBW40_Base[path, group] = tmp_base;
                    if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                    {
                        RTW_INFO(
                            $"[{rf_path_char(path)}] 2G G{group:2} BW40-1S base:{tmp_base} from {pg_txpwr_src_str(txpwr_src)}");
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
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) &&
                        IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G BW20-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) &&
                        IS_PG_TXPWR_DIFF_INVALID(pwr_info.OFDM_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.OFDM_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G OFDM-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
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
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff) &&
                        IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW40_Diff[path, tx_idx]))
                    {
                        pwr_info.BW40_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G BW40-%dS diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
                        }

                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.BW20_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.BW20_Diff[path, tx_idx] = tmp_diff;
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
                            RTW_INFO("[%c] 2G OFDM-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
                        }
                    }

                    tmp_diff = PG_TXPWR_LSB_DIFF_TO_S8BIT(val);
                    if (!IS_PG_TXPWR_DIFF_INVALID(tmp_diff)
                        && IS_PG_TXPWR_DIFF_INVALID(pwr_info.CCK_Diff[path, tx_idx])
                       )
                    {
                        pwr_info.CCK_Diff[path, tx_idx] = tmp_diff;
                        if (LOAD_PG_TXPWR_WARN_COND(txpwr_src))
                        {
                            RTW_INFO("[%c] 2G CCK-%dT diff:%d from %s\n", rf_path_char(path), tx_idx + 1, tmp_diff,
                                pg_txpwr_src_str(txpwr_src));
                        }
                    }
                }

                offset++;
            }
        }

        if (offset != pg_offset + PG_TXPWR_1PATH_BYTE_NUM_2G)
        {
            RTW_ERR($"hal_load_pg_txpwr_info_path_2g parse {offset - pg_offset} bytes != {PG_TXPWR_1PATH_BYTE_NUM_2G}");
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
        for (path = 0; path < MAX_RF_PATH; path++)
        {
            for (group = 0; group < MAX_CHNL_GROUP_5G; group++)
            {
                pwr_info.IndexBW40_Base[path, group] = PG_TXPWR_INVALID_BASE;
            }

            for (tx_idx = 0; tx_idx < MAX_TX_COUNT; tx_idx++)
            {
                pwr_info.OFDM_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW20_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW40_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW80_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
                pwr_info.BW160_Diff[path, tx_idx] = PG_TXPWR_INVALID_DIFF;
            }
        }

        for (path = 0; path < MAX_RF_PATH; path++)
        {
            if (!HAL_SPEC_CHK_RF_PATH_5G(hal_spec, path))
                break;
            /* dummy diff */
            pwr_info.BW40_Diff[path, 0] = 0; /* 5G BW40-1S */
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
            RTW_WARN($"rtw_get_ch_group invalid channel:{ch}");
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

        RTW_INFO($"pHalData.TypeGPA = 0x{pHalData.TypeGPA}");
        RTW_INFO($"pHalData.TypeAPA = 0x{pHalData.TypeAPA}");
        RTW_INFO($"pHalData.TypeGLNA = 0x{pHalData.TypeGLNA}");
        RTW_INFO($"pHalData.TypeALNA = 0x{pHalData.TypeALNA}");
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
                pHalData.ExternalPA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_PA) != 0;
                pHalData.ExternalLNA_2G = (GetRegAmplifierType2G(Adapter) & ODM_BOARD_EXT_LNA) != 0;
            }

            if (GetRegAmplifierType5G(Adapter) == 0)
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
                pHalData.external_pa_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_PA_5G) != 0;
                pHalData.external_lna_5g = (GetRegAmplifierType5G(Adapter) & ODM_BOARD_EXT_LNA_5G) != 0;
            }
        }
        else
        {
            pHalData.ExternalPA_2G = false;
            pHalData.external_pa_5g = true;
            pHalData.ExternalLNA_2G = false;
            pHalData.external_lna_5g = true;

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

        RTW_INFO($"pHalData.PAType_2G is 0x{pHalData.PAType_2G:X}, pHalData.ExternalPA_2G = {pHalData.ExternalPA_2G}");
        RTW_INFO(
            $"pHalData.PAType_5G is 0x{pHalData.PAType_5G:X}, pHalData.external_pa_5g = {pHalData.external_pa_5g}");
        RTW_INFO(
            $"pHalData.LNAType_2G is 0x{pHalData.LNAType_2G:X}, pHalData.ExternalLNA_2G = {pHalData.ExternalLNA_2G}");
        RTW_INFO(
            $"pHalData.LNAType_5G is 0x{pHalData.LNAType_5G:X}, pHalData.external_lna_5g = {pHalData.external_lna_5g}");
    }

    private static void Hal_EfuseParseXtal_8812A(PADAPTER pAdapter, u8[] hwinfo, bool AutoLoadFail)
    {
        var pHalData = GET_HAL_DATA(pAdapter);

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
//         if (hw_addr && rtw_check_invalid_mac_address(hw_addr, true) == false) {
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
        EEPROMId = BinaryPrimitives.ReadUInt16LittleEndian(hwinfo.AsSpan(0, 2));
        if (EEPROMId != RTL_EEPROM_ID)
        {
            RTW_INFO($"EEPROM ID(0x{EEPROMId:X}) is invalid!!");
            pHalData.bautoload_fail_flag = true;
        }
        else
            pHalData.bautoload_fail_flag = false;

        RTW_INFO($"EEPROM ID=0x{EEPROMId}");
    }

    static void hal_InitPGData_8812A(PADAPTER padapter, u8[] PROMContent)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        u32 i;

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
            /* pHalData.AutoloadFailFlag = true; */
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
                    pHalData.bautoload_fail_flag = false;
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

        rtw_dump_cur_efuse(pAdapter);
    }

    static void rtw_dump_cur_efuse(PADAPTER padapter)
    {
        HAL_DATA_TYPE hal_data = GET_HAL_DATA(padapter);

        var mapsize = EFUSE_GetEfuseDefinition(padapter, EFUSE_WIFI, EFUSE_DEF_TYPE.TYPE_EFUSE_MAP_LEN);

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

    static int EFUSE_GetEfuseDefinition(PADAPTER pAdapter, u8 efuseType, EFUSE_DEF_TYPE type)
    {
        return rtl8812_EFUSE_GetEfuseDefinition(pAdapter, efuseType, type);
    }

    static void Efuse_ReadAllMap(_adapter adapter, byte efuseType, byte[] Efuse)
    {
        EfusePowerSwitch8812A(adapter, false, true);
        efuse_ReadEFuse(adapter, efuseType, 0, EFUSE_MAP_LEN_JAGUAR, Efuse);
        EfusePowerSwitch8812A(adapter, false, false);
    }

    private static void efuse_ReadEFuse(_adapter adapter, byte efuseType, UInt16 _offset, UInt16 _size_byte,
        byte[] pbuf)
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
                        eFuseWord[offset][i] = (ushort)(rtemp8[0] & 0xff);


                        if (eFuse_Addr >= EFUSE_REAL_CONTENT_LEN_JAGUAR)
                            break;

                        /* RTPRINT(FEEPROM, EFUSE_READ_ALL, ("Addr=%d", eFuse_Addr)); */
                        ReadEFuseByte(adapter, eFuse_Addr, rtemp8);
                        eFuse_Addr++;

                        efuse_utilized++;
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
        RTW_INFO($"Hal_EfuseReadEFuse8812A: eFuse_Addr offset(0x{eFuse_Addr:X}) !!");
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

    static void ReadEFuseByte(_adapter Adapter, UInt16 _offset, byte[] pbuf)
    {
        u32 value32;
        u8 readbyte;
        u16 retry;

        /* Write Address */
        rtw_write8(Adapter, EFUSE_CTRL + 1, (byte)(_offset & 0xff));
        readbyte = rtw_read8(Adapter, EFUSE_CTRL + 2);
        rtw_write8(Adapter, EFUSE_CTRL + 2, (byte)(((_offset >> 8) & 0x03) | (readbyte & 0xfc)));

        /* Write bit 32 0 */
        readbyte = rtw_read8(Adapter, EFUSE_CTRL + 3);
        rtw_write8(Adapter, EFUSE_CTRL + 3, (byte)(readbyte & 0x7f));

        /* Check bit 32 read-ready */
        retry = 0;
        value32 = rtw_read32(Adapter, EFUSE_CTRL);
        /* while(!(((value32 >> 24) & 0xff) & 0x80)  && (retry<10)) */
        while ((((value32 >> 24) & 0xff) & 0x80) == 0 && (retry < 10000))
        {
            value32 = rtw_read32(Adapter, EFUSE_CTRL);
            retry++;
        }

        /* 20100205 Joseph: Add delay suggested by SD1 Victor. */
        /* This fix the problem that Efuse read error in high temperature condition. */
        /* Designer says that there shall be some delay after ready bit is set, or the */
        /* result will always stay on last data we read. */
        Thread.Sleep(50);
        value32 = rtw_read32(Adapter, EFUSE_CTRL);

        pbuf[0] = (u8)(value32 & 0xff);

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

    public static bool rtl8812au_hal_init(PADAPTER Adapter)
    {
        u8 value8 = 0, u1bRegCR;
        u16 value16;
        u8 txpktbuf_bndy;
        bool status = false;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        pwrctrl_priv pwrctrlpriv = adapter_to_pwrctl(Adapter);
        registry_priv pregistrypriv = Adapter.registrypriv;

        rt_rf_power_state eRfPowerStateToSet;

        // Check if MAC has already power on. by tynli. 2011.05.27.
        value8 = rtw_read8(Adapter, REG_SYS_CLKR + 1);
        u1bRegCR = rtw_read8(Adapter, REG_CR);
        RTW_INFO(" power-on :REG_SYS_CLKR 0x09=0x%02x. REG_CR 0x100=0x%02x.\n", value8, u1bRegCR);
        if ((value8 & BIT3) != 0 && (u1bRegCR != 0 && u1bRegCR != 0xEA))
        {
            /* pHalData.bMACFuncEnable = TRUE; */
            RTW_INFO(" MAC has already power on.\n");
        }
        else
        {
            /* pHalData.bMACFuncEnable = FALSE; */
            /* Set FwPSState to ALL_ON mode to prevent from the I/O be return because of 32k */
            /* state which is set before sleep under wowlan mode. 2012.01.04. by tynli. */
            /* pHalData.FwPSState = FW_PS_STATE_ALL_ON_88E; */
            RTW_INFO(" MAC has not been powered on yet.\n");
        }

        rtw_write8(Adapter, REG_RF_CTRL, 5);
        rtw_write8(Adapter, REG_RF_CTRL, 7);
        rtw_write8(Adapter, REG_RF_B_CTRL_8812, 5);
        rtw_write8(Adapter, REG_RF_B_CTRL_8812, 7);

        // If HW didn't go through a complete de-initial procedure,
        // it probably occurs some problem for double initial procedure.
        // Like "CONFIG_DEINIT_BEFORE_INIT" in 92du chip
        rtl8812au_hw_reset(Adapter);

        status = rtw_hal_power_on(Adapter);
        if (status == false)
        {
            goto exit;
        }

        if (!pregistrypriv.wifi_spec)
        {
            txpktbuf_bndy = TX_PAGE_BOUNDARY_8812;
        }
        else
        {
            throw new NotImplementedException();
            /* for WMM */
            //txpktbuf_bndy = WMM_NORMAL_TX_PAGE_BOUNDARY_8812;
        }

        status = InitLLTTable8812A(Adapter, txpktbuf_bndy);
        if (status == false)
        {
            goto exit;
        }

        _InitHardwareDropIncorrectBulkOut_8812A(Adapter);

        //if (pHalData.bRDGEnable)
        //{
        //    _InitRDGSetting_8812A(Adapter);
        //}


        FirmwareDownload8812(Adapter, false);

        if (pwrctrlpriv.reg_rfoff == true)
        {
            pwrctrlpriv.rf_pwrstate = rt_rf_power_state.rf_off;
        }

        status = PHY_MACConfig8812(Adapter);
        if (status == false)
        {
            goto exit;
        }


        _InitQueueReservedPage_8812AUsb(Adapter);
        _InitTxBufferBoundary_8812AUsb(Adapter);

        _InitQueuePriority_8812AUsb(Adapter);
        _InitPageBoundary_8812AUsb(Adapter);

        _InitTransferPageSize_8812AUsb(Adapter);

        // Get Rx PHY status in order to report RSSI and others.
        _InitDriverInfoSize_8812A(Adapter, DRVINFO_SZ);

        _InitInterrupt_8812AU(Adapter);
        _InitNetworkType_8812A(Adapter); /* set msr	 */
        _InitWMACSetting_8812A(Adapter);
        _InitAdaptiveCtrl_8812AUsb(Adapter);
        _InitEDCA_8812AUsb(Adapter);

        _InitRetryFunction_8812A(Adapter);
        init_UsbAggregationSetting_8812A(Adapter);

        _InitBeaconParameters_8812A(Adapter);
        _InitBeaconMaxError_8812A(Adapter, true);

        _InitBurstPktLen(Adapter); // added by page. 20110919

        // Init CR MACTXEN, MACRXEN after setting RxFF boundary REG_TRXFF_BNDY to patch
        // Hw bug which Hw initials RxFF boundry size to a value which is larger than the real Rx buffer size in 88E.
        // 2011.08.05. by tynli.
        value8 = rtw_read8(Adapter, REG_CR);
        rtw_write8(Adapter, REG_CR, (byte)(value8 | MACTXEN | MACRXEN));

        rtw_write16(Adapter, REG_PKT_VO_VI_LIFE_TIME, 0x0400);  /* unit: 256us. 256ms */
        rtw_write16(Adapter, REG_PKT_BE_BK_LIFE_TIME, 0x0400);	/* unit: 256us. 256ms */

        status = PHY_BBConfig8812(Adapter);
        if (status == false)
        {
            goto exit;
        }

        PHY_RFConfig8812(Adapter);

        if (pHalData.rf_type == rf_type.RF_1T1R)
        {
            PHY_BB8812_Config_1T(Adapter);
        }

        if (Adapter.registrypriv.rf_config == rf_type.RF_1T2R)
        {
            phy_set_bb_reg(Adapter, rTxPath_Jaguar, bMaskLWord, 0x1111);
        }


        if (Adapter.registrypriv.channel <= 14)
        {
            PHY_SwitchWirelessBand8812(Adapter, BAND_TYPE.BAND_ON_2_4G);
        }
        else
        {
            PHY_SwitchWirelessBand8812(Adapter, BAND_TYPE.BAND_ON_5G);
        }

        rtw_hal_set_chnl_bw(Adapter, Adapter.registrypriv.channel, channel_width.CHANNEL_WIDTH_20,
            HAL_PRIME_CHNL_OFFSET_DONT_CARE,
            HAL_PRIME_CHNL_OFFSET_DONT_CARE);


        invalidate_cam_all(Adapter);

        // HW SEQ CTRL
        // set 0x0 to 0xFF by tynli. Default enable HW SEQ NUM.
        rtw_write8(Adapter, REG_HWSEQ_CTRL, 0xFF);


        // Disable BAR, suggested by Scott
        // 2010.04.09 add by hpfan
        rtw_write32(Adapter, REG_BAR_MODE_CTRL, 0x0201ffff);

        if (pregistrypriv.wifi_spec)
        {
            rtw_write16(Adapter, REG_FAST_EDCA_CTRL, 0);
        }

        // Nav limit , suggest by scott
        rtw_write8(Adapter, 0x652, 0x0);

        rtl8812_InitHalDm(Adapter);

        {
            /*  */
            /* 2010/08/11 MH Merge from 8192SE for Minicard init. We need to confirm current radio status */
            /* and then decide to enable RF or not.!!!??? For Selective suspend mode. We may not */
            /* call init_adapter. May cause some problem?? */
            /*  */
            /* Fix the bug that Hw/Sw radio off before S3/S4, the RF off action will not be executed */
            /* in MgntActSet_RF_State() after wake up, because the value of pHalData.eRFPowerState */
            /* is the same as eRfOff, we should change it to eRfOn after we config RF parameters. */
            /* Added by tynli. 2010.03.30. */
            pwrctrlpriv.rf_pwrstate = rt_rf_power_state.rf_on;

            /* 0x4c6[3] 1: RTS BW = Data BW */
            /* 0: RTS BW depends on CCA / secondary CCA result. */
            rtw_write8(Adapter, REG_QUEUE_CTRL, (byte)(rtw_read8(Adapter, REG_QUEUE_CTRL) & 0xF7));

            /* enable Tx report. */
            rtw_write8(Adapter, REG_FWHW_TXQ_CTRL + 1, 0x0F);

            /* Suggested by SD1 pisa. Added by tynli. 2011.10.21. */
            rtw_write8(Adapter, REG_EARLY_MODE_CONTROL_8812 + 3, 0x01); /* Pretx_en, for WEP/TKIP SEC */

            /* tynli_test_tx_report. */
            rtw_write16(Adapter, REG_TX_RPT_TIME, 0x3DF0);

            /* Reset USB mode switch setting */
            rtw_write8(Adapter, REG_SDIO_CTRL_8812, 0x0);
            rtw_write8(Adapter, REG_ACLK_MON, 0x0);


            /* 2010/08/26 MH Merge from 8192CE. */
            if (pwrctrlpriv.rf_pwrstate == rt_rf_power_state.rf_on)
            {
                /*		if(IS_HARDWARE_TYPE_8812AU(Adapter))
                        {
                #if (RTL8812A_SUPPORT == 1)
                            pHalData.bNeedIQK = _TRUE;
                            if(pHalData.bIQKInitialized)
                                PHY_IQCalibrate_8812A(Adapter, _TRUE);
                            else
                            {
                                PHY_IQCalibrate_8812A(Adapter, _FALSE);
                                pHalData.bIQKInitialized = _TRUE;
                            }
                #endif
                        }*/
                /* odm_txpowertracking_check(&pHalData.odmpriv ); */
                /* PHY_LCCalibrate_8812A(Adapter); */
            }
        }

        rtw_write8(Adapter, REG_USB_HRPWM, 0);

        // TODO:
        ///* ack for xmit mgmt frames. */
        rtw_write32(Adapter, REG_FWHW_TXQ_CTRL, rtw_read32(Adapter, REG_FWHW_TXQ_CTRL) | BIT12);
        exit:

        return status;
    }

    public static void phy_set_bb_reg(_adapter Adapter, u16 RegAddr, u32 BitMask, u32 Data) =>
        PHY_SetBBReg8812(Adapter, RegAddr, BitMask, Data);

    static void rtl8812_InitHalDm(PADAPTER Adapter)
    {
        dm_InitGPIOSetting(Adapter);
        rtw_phydm_init(Adapter);
        /* Adapter.fix_rate = 0xFF; */
    }

    static void rtw_phydm_init(_adapter adapter)
    {
        var hal_data = GET_HAL_DATA(adapter);
        var phydm = (hal_data.odmpriv);
        init_phydm_info(adapter);
        odm_dm_init(phydm);
    }

    static void odm_dm_init(dm_struct dm)
    {
        halrf_init(dm);
        phydm_supportability_init(dm);
        phydm_common_info_self_init(dm);
        phydm_rx_phy_status_init(dm);
        //phydm_dig_init(dm);
        //phydm_cck_pd_init(dm);
        //phydm_env_monitor_init(dm);
        //phydm_adaptivity_init(dm);
        //phydm_ra_info_init(dm);
        //phydm_rssi_monitor_init(dm);
        //phydm_cfo_tracking_init(dm);
        //phydm_rf_init(dm);
        //phydm_dc_cancellation(dm);
        //phydm_psd_init(dm);
    }

    static void phydm_rx_phy_status_init(dm_struct dm)
    {
        //odm_phy_dbg_info dbg = dm.phy_dbg_info;

        //dbg.show_phy_sts_all_pkt = 0;
        //dbg.show_phy_sts_max_cnt = 1;
        //dbg.show_phy_sts_cnt = 0;

        //phydm_avg_phystatus_init(dm);
    }

    static void phydm_common_info_self_init(dm_struct dm)
    {
        //u32 reg_tmp = 0;
        //u32 mask_tmp = 0;

        ///*@BB IP Generation*/
        //if (dm.support_ic_type & ODM_IC_JGR3_SERIES)
        //    dm.ic_ip_series = PHYDM_IC_JGR3;
        //else if (dm.support_ic_type & ODM_IC_11AC_SERIES)
        //    dm.ic_ip_series = PHYDM_IC_AC;
        //else if (dm.support_ic_type & ODM_IC_11N_SERIES)
        //    dm.ic_ip_series = PHYDM_IC_N;

        ///*@BB phy-status Generation*/
        //if (dm.support_ic_type & PHYSTS_3RD_TYPE_IC)
        //    dm.ic_phy_sts_type = PHYDM_PHYSTS_TYPE_3;
        //else if (dm.support_ic_type & PHYSTS_2ND_TYPE_IC)
        //    dm.ic_phy_sts_type = PHYDM_PHYSTS_TYPE_2;
        //else
        //    dm.ic_phy_sts_type = PHYDM_PHYSTS_TYPE_1;

        //phydm_init_cck_setting(dm);

        //reg_tmp = ODM_REG(BB_RX_PATH, dm);
        //mask_tmp = ODM_BIT(BB_RX_PATH, dm);
        //dm.rf_path_rx_enable = (u8)odm_get_bb_reg(dm, reg_tmp, mask_tmp);

        //phydm_init_soft_ml_setting(dm);

        //dm.phydm_sys_up_time = 0;

        //if (dm.support_ic_type & ODM_IC_1SS)
        //    dm.num_rf_path = 1;
        //else if (dm.support_ic_type & ODM_IC_2SS)
        //    dm.num_rf_path = 2;
        //else if (dm.support_ic_type & ODM_IC_4SS)
        //    dm.num_rf_path = 4;
        //else
        //    dm.num_rf_path = 1;

        //phydm_trx_antenna_setting_init(dm, dm.num_rf_path);

        //dm.tx_rate = 0xFF;
        //dm.rssi_min_by_path = 0xFF;

        //dm.number_linked_client = 0;
        //dm.pre_number_linked_client = 0;
        //dm.number_active_client = 0;
        //dm.pre_number_active_client = 0;

        //dm.last_tx_ok_cnt = 0;
        //dm.last_rx_ok_cnt = 0;
        //dm.tx_tp = 0;
        //dm.rx_tp = 0;
        //dm.total_tp = 0;
        //dm.traffic_load = TRAFFIC_LOW;

        //dm.nbi_set_result = 0;
        //dm.is_init_hw_info_by_rfe = false;
        //dm.pre_dbg_priority = DBGPORT_RELEASE;
        //dm.tp_active_th = 5;
        //dm.disable_phydm_watchdog = 0;

        //dm.u8_dummy = 0xf;
        //dm.u16_dummy = 0xffff;
        //dm.u32_dummy = 0xffffffff;

        //dm.pause_lv_table.lv_cckpd = PHYDM_PAUSE_RELEASE;
        //dm.pause_lv_table.lv_dig = PHYDM_PAUSE_RELEASE;
    }

    static void phydm_supportability_init(dm_struct dm)
    {
        //u64 support_ability;

        //if (dm.mp_mode)
        //{
        //    support_ability = 0;
        //}
        //else
        //{
        //    support_ability = phydm_supportability_init_ce(dm);

        //    /*@[Config Antenna Diversity]*/
        //    if (IS_FUNC_EN(dm.enable_antdiv))
        //        support_ability |= ODM_BB_ANT_DIV;

        //    /*@[Config TXpath Diversity]*/
        //    if (IS_FUNC_EN(dm.enable_pathdiv))
        //        support_ability |= ODM_BB_PATH_DIV;

        //    /*@[Config Adaptive SOML]*/
        //    if (IS_FUNC_EN(dm.en_adap_soml))
        //        support_ability |= ODM_BB_ADAPTIVE_SOML;

        //    /* @[Config Adaptivity]*/
        //    if (IS_FUNC_EN(dm.enable_adaptivity))
        //        support_ability |= ODM_BB_ADAPTIVITY;
        //}

        //odm_cmn_info_init(dm, ODM_CMNINFO_ABILITY, support_ability);
        //PHYDM_DBG(dm, ODM_COMP_INIT, "IC=0x%x, mp=%d, Supportability=0x%llx\n",
        //    dm.support_ic_type, *dm.mp_mode, dm.support_ability);
    }

    static void halrf_init(dm_struct dm)
    {
        //RF_DBG(dm, DBG_RF_INIT, "HALRF_Init\n");
        //halrf_init_debug_setting(dm);
        //if (dm.mp_mode)
        //{
        //    halrf_supportability_init_mp(dm);
        //}
        //else
        //{
        //    halrf_supportability_init(dm);
        //}

        ///*Init all RF funciton*/
        //halrf_aac_check(dm);
        //halrf_dack_trigger(dm);

        //halrf_tssi_init(dm);
    }

    static void init_phydm_info(_adapter adapter)
    {
        PHAL_DATA_TYPE hal_data = GET_HAL_DATA(adapter);
        dm_struct phydm = (hal_data.odmpriv);
        odm_cmn_info_init(phydm, odm_cmninfo.ODM_CMNINFO_FW_VER, hal_data.firmware_version);
        odm_cmn_info_init(phydm, odm_cmninfo.ODM_CMNINFO_FW_SUB_VER, hal_data.firmware_sub_version);
    }

    static void dm_InitGPIOSetting(PADAPTER Adapter)
    {
        //PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        //u8 tmp1byte;

        //tmp1byte = rtw_read8(Adapter, REG_GPIO_MUXCFG);
        //tmp1byte &= (GPIOSEL_GPIO | ~GPIOSEL_ENBT);

        //rtw_write8(Adapter, REG_GPIO_MUXCFG, tmp1byte);
    }

    static void invalidate_cam_all(_adapter padapter)
    {
        //dvobj_priv dvobj = adapter_to_dvobj(padapter);
        //cam_ctl_t cam_ctl = dvobj.cam_ctl;
        //u8 val8 = 0;
        //rtw_hal_set_hwreg(padapter, HW_VAR_CAM_INVALID_ALL, val8);
        //rtw_sec_cam_map_clr_all(cam_ctl.used);
    }

    public static void PHY_SwitchWirelessBand8812(PADAPTER Adapter, BAND_TYPE Band)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        channel_width current_bw = pHalData.current_channel_bw;
        bool eLNA_2g = pHalData.ExternalLNA_2G;

        /* RTW_INFO("==>PHY_SwitchWirelessBand8812() %s\n", ((Band==0)?"2.4G":"5G")); */

        pHalData.current_band_type = (BAND_TYPE)Band;

        if (Band == BAND_TYPE.BAND_ON_2_4G)
        {
            /* 2.4G band */

            phy_set_bb_reg(Adapter, rOFDMCCKEN_Jaguar, bOFDMEN_Jaguar | bCCKEN_Jaguar, 0x03);

            /* <20131128, VincentL> Remove 0x830[3:1] setting when switching 2G/5G, requested by Yn. */
            phy_set_bb_reg(Adapter, rBWIndication_Jaguar, 0x3, 0x1); /* 0x834[1:0] = 0x1 */
            /* set PD_TH_20M for BB Yn user guide R27 */
            phy_set_bb_reg(Adapter, rPwed_TH_Jaguar, BIT13 | BIT14 | BIT15 | BIT16 | BIT17,
                0x17); /* 0x830[17:13]=5'b10111 */


            /* set PWED_TH for BB Yn user guide R29 */

            if (current_bw == channel_width.CHANNEL_WIDTH_20
                && pHalData.rf_type == rf_type.RF_1T1R
                && eLNA_2g == false)
            {
                /* 0x830[3:1]=3'b010 */
                phy_set_bb_reg(Adapter, rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x02);
            }
            else
            {
                /* 0x830[3:1]=3'b100 */
                phy_set_bb_reg(Adapter, rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x04);
            }


            /* AGC table select */
            phy_set_bb_reg(Adapter, rAGC_table_Jaguar, 0x3, 0); /* 0x82C[1:0] = 2b'00 */

            phy_SetRFEReg8812(Adapter, Band);

            /* <20131106, Kordan> Workaround to fix CCK FA for scan issue. */
            /* if( pHalData.bMPMode == FALSE) */

            phy_set_bb_reg(Adapter, rTxPath_Jaguar, 0xf0, 0x1);
            phy_set_bb_reg(Adapter, rCCK_RX_Jaguar, 0x0f000000, 0x1);

            update_tx_basic_rate(Adapter, NETWORK_TYPE.WIRELESS_11BG);

            /* CCK_CHECK_en */
            rtw_write8(Adapter, REG_CCK_CHECK_8812, (byte)(rtw_read8(Adapter, REG_CCK_CHECK_8812) & (NotBIT7)));
        }
        else
        {
            /* 5G band */
            u16 count = 0, reg41A = 0;


            /* CCK_CHECK_en */
            rtw_write8(Adapter, REG_CCK_CHECK_8812, (byte)(rtw_read8(Adapter, REG_CCK_CHECK_8812) | BIT7));

            count = 0;
            reg41A = rtw_read16(Adapter, REG_TXPKT_EMPTY);
            /* RTW_INFO("Reg41A value %d", reg41A); */
            reg41A &= 0x30;
            while ((reg41A != 0x30) && (count < 50))
            {
                Thread.Sleep(50);
                /* RTW_INFO("Delay 50us\n"); */

                reg41A = rtw_read16(Adapter, REG_TXPKT_EMPTY);
                reg41A &= 0x30;
                count++;
                /* RTW_INFO("Reg41A value %d", reg41A); */
            }

            if (count != 0)
                RTW_INFO("PHY_SwitchWirelessBand8812(): Switch to 5G Band. Count = %d reg41A=0x%x\n", count, reg41A);

            /* 2012/02/01, Sinda add registry to switch workaround without long-run verification for scan issue. */
            phy_set_bb_reg(Adapter, rOFDMCCKEN_Jaguar, bOFDMEN_Jaguar | bCCKEN_Jaguar, 0x03);

            /* <20131128, VincentL> Remove 0x830[3:1] setting when switching 2G/5G, requested by Yn. */
            phy_set_bb_reg(Adapter, rBWIndication_Jaguar, 0x3, 0x2); /* 0x834[1:0] = 0x2 */
            /* set PD_TH_20M for BB Yn user guide R27 */
            phy_set_bb_reg(Adapter, rPwed_TH_Jaguar, BIT13 | BIT14 | BIT15 | BIT16 | BIT17,
                0x15); /* 0x830[17:13]=5'b10101 */


            /* set PWED_TH for BB Yn user guide R29 */
            /* 0x830[3:1]=3'b100 */
            phy_set_bb_reg(Adapter, rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x04);

            /* AGC table select */
            phy_set_bb_reg(Adapter, rAGC_table_Jaguar, 0x3, 1); /* 0x82C[1:0] = 2'b00 */

            phy_SetRFEReg8812(Adapter, Band);

            /* <20131106, Kordan> Workaround to fix CCK FA for scan issue. */
            /* if( pHalData.bMPMode == FALSE) */
            phy_set_bb_reg(Adapter, rTxPath_Jaguar, 0xf0, 0x0);
            phy_set_bb_reg(Adapter, rCCK_RX_Jaguar, 0x0f000000, 0xF);

            /* avoid using cck rate in 5G band */
            /* Set RRSR rate table. */
            update_tx_basic_rate(Adapter, NETWORK_TYPE.WIRELESS_11A);


            /* RTW_INFO("==>PHY_SwitchWirelessBand8812() BAND_ON_5G settings OFDM index 0x%x\n", pHalData.OFDM_index[RF_PATH_A]); */
        }

        phy_SetBBSwingByBand_8812A(Adapter, Band);
    }

    static void phy_SetBBSwingByBand_8812A(PADAPTER Adapter, BAND_TYPE Band)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA((Adapter));

        phy_set_bb_reg(Adapter, rA_TxScale_Jaguar, 0xFFE00000,
            phy_get_tx_bb_swing_8812a(Adapter, (BAND_TYPE)Band, rf_path.RF_PATH_A)); /* 0xC1C[31:21] */
        phy_set_bb_reg(Adapter, rB_TxScale_Jaguar, 0xFFE00000,
            phy_get_tx_bb_swing_8812a(Adapter, (BAND_TYPE)Band, rf_path.RF_PATH_B)); /* 0xE1C[31:21] */

        odm_clear_txpowertracking_state(pHalData.odmpriv);
    }

    static u32 phy_get_tx_bb_swing_8812a(PADAPTER Adapter, BAND_TYPE Band, rf_path RFPath)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA((Adapter));
        dm_struct pDM_Odm = pHalData.odmpriv;
        dm_rf_calibration_struct pRFCalibrateInfo = (pDM_Odm.rf_calibrate_info);

        s8 bbSwing_2G = (s8)(-1 * Adapter.registrypriv.TxBBSwing_2G);
        s8 bbSwing_5G = (s8)(-1 * Adapter.registrypriv.TxBBSwing_5G);
        u32 _out = 0x200;
        const s8 AUTO = -1;


        if (pHalData.bautoload_fail_flag)
        {
            if (Band == BAND_TYPE.BAND_ON_2_4G)
            {
                pRFCalibrateInfo.bb_swing_diff_2g = bbSwing_2G;
                if (bbSwing_2G == 0)
                    _out = 0x200; /* 0 dB */
                else if (bbSwing_2G == -3)
                    _out = 0x16A; /* -3 dB */
                else if (bbSwing_2G == -6)
                    _out = 0x101; /* -6 dB */
                else if (bbSwing_2G == -9)
                    _out = 0x0B6; /* -9 dB */
                else
                {
                    if (pHalData.ExternalPA_2G)
                    {
                        pRFCalibrateInfo.bb_swing_diff_2g = -3;
                        _out = 0x16A;
                    }
                    else
                    {
                        pRFCalibrateInfo.bb_swing_diff_2g = 0;
                        _out = 0x200;
                    }
                }
            }
            else if (Band == BAND_TYPE.BAND_ON_5G)
            {
                pRFCalibrateInfo.bb_swing_diff_5g = bbSwing_5G;
                if (bbSwing_5G == 0)
                    _out = 0x200; /* 0 dB */

                else if (bbSwing_5G == -3)
                    _out = 0x16A; /* -3 dB */

                else if (bbSwing_5G == -6)
                    _out = 0x101; /* -6 dB */

                else if (bbSwing_5G == -9)
                    _out = 0x0B6; /* -9 dB */

                else
                {
                    pRFCalibrateInfo.bb_swing_diff_5g = 0;
                    _out = 0x200;
                }
            }
            else
            {
                pRFCalibrateInfo.bb_swing_diff_2g = -3;
                pRFCalibrateInfo.bb_swing_diff_5g = -3;
                _out = 0x16A; /* -3 dB */
            }
        }
        else
        {
            byte swing = 0;
            byte onePathSwing = 0;

            if (Band == BAND_TYPE.BAND_ON_2_4G)
            {
                if (Adapter.registrypriv.TxBBSwing_2G == AUTO)
                {
                    efuse_ShadowRead1Byte(Adapter, EEPROM_TX_BBSWING_2G_8812, out swing);
                    swing = (swing == 0xFF) ? (byte)0x00 : swing;
                }
                else if (bbSwing_2G == 0)
                    swing = 0x00; /* 0 dB */
                else if (bbSwing_2G == -3)
                    swing = 0x05; /* -3 dB */
                else if (bbSwing_2G == -6)
                    swing = 0x0A; /* -6 dB */
                else if (bbSwing_2G == -9)
                    swing = 0xFF; /* -9 dB */
                else
                    swing = 0x00;
            }
            else
            {
                if (Adapter.registrypriv.TxBBSwing_5G == AUTO)
                {
                    efuse_ShadowRead1Byte(Adapter, EEPROM_TX_BBSWING_5G_8812, out swing);
                    swing = (swing == 0xFF) ? (byte)0x00 : swing;
                }
                else if (bbSwing_5G == 0)
                    swing = 0x00; /* 0 dB */
                else if (bbSwing_5G == -3)
                    swing = 0x05; /* -3 dB */
                else if (bbSwing_5G == -6)
                    swing = 0x0A; /* -6 dB */
                else if (bbSwing_5G == -9)
                    swing = 0xFF; /* -9 dB */
                else
                    swing = 0x00;
            }

            if (RFPath == rf_path.RF_PATH_A)
                onePathSwing = (byte)((swing & 0x3) >> 0); /* 0xC6/C7[1:0] */
            else if (RFPath == rf_path.RF_PATH_B)
                onePathSwing = (byte)((swing & 0xC) >> 2); /* 0xC6/C7[3:2] */

            if (onePathSwing == 0x0)
            {
                if (Band == BAND_TYPE.BAND_ON_2_4G)
                    pRFCalibrateInfo.bb_swing_diff_2g = 0;
                else
                    pRFCalibrateInfo.bb_swing_diff_5g = 0;
                _out = 0x200; /* 0 dB */
            }
            else if (onePathSwing == 0x1)
            {
                if (Band == BAND_TYPE.BAND_ON_2_4G)
                    pRFCalibrateInfo.bb_swing_diff_2g = -3;
                else
                    pRFCalibrateInfo.bb_swing_diff_5g = -3;
                _out = 0x16A; /* -3 dB */
            }
            else if (onePathSwing == 0x2)
            {
                if (Band == BAND_TYPE.BAND_ON_2_4G)
                    pRFCalibrateInfo.bb_swing_diff_2g = -6;
                else
                    pRFCalibrateInfo.bb_swing_diff_5g = -6;
                _out = 0x101; /* -6 dB */
            }
            else if (onePathSwing == 0x3)
            {
                if (Band == BAND_TYPE.BAND_ON_2_4G)
                    pRFCalibrateInfo.bb_swing_diff_2g = -9;
                else
                    pRFCalibrateInfo.bb_swing_diff_5g = -9;
                _out = 0x0B6; /* -9 dB */
            }
        }

/* RTW_INFO("<=== phy_get_tx_bb_swing_8812a, out = 0x%X\n", out); */

        return _out;
    }

    static void phy_SetRFEReg8812(PADAPTER Adapter, BAND_TYPE Band)
    {
        uint u1tmp = 0;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        if (Band == BAND_TYPE.BAND_ON_2_4G)
        {
            switch (pHalData.rfe_type)
            {
                case 0:
                case 2:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    break;
                case 1:
                {
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                }
                    break;
                case 3:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337770);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337770);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, r_ANTSEL_SW_Jaguar, 0x00000303, 0x1);
                    break;
                case 4:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x001);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x001);
                    break;
                case 5:
                    rtw_write8(Adapter, rA_RFE_Pinmux_Jaguar + 2, 0x77);

                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    u1tmp = rtw_read8(Adapter, rA_RFE_Inv_Jaguar + 3);
                    u1tmp &= NotBIT0;
                    rtw_write8(Adapter, rA_RFE_Inv_Jaguar + 3, (byte)(u1tmp));
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    break;
                case 6:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x07772770);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x07772770);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (pHalData.rfe_type)
            {
                case 0:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 1:
                {
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                }
                    break;
                case 2:
                case 4:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 3:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337717);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337717);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    phy_set_bb_reg(Adapter, r_ANTSEL_SW_Jaguar, 0x00000303, 0x1);
                    break;
                case 5:
                    rtw_write8(Adapter, rA_RFE_Pinmux_Jaguar + 2, 0x33);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    u1tmp = rtw_read8(Adapter, rA_RFE_Inv_Jaguar + 3);
                    rtw_write8(Adapter, rA_RFE_Inv_Jaguar + 3, (byte)(u1tmp |= BIT0));
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 6:
                    phy_set_bb_reg(Adapter, rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x07737717);
                    phy_set_bb_reg(Adapter, rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x07737717);
                    phy_set_bb_reg(Adapter, rA_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    phy_set_bb_reg(Adapter, rB_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    break;
                default:
                    break;
            }
        }
    }

    /* Update RRSR and Rate for USERATE */
    static void update_tx_basic_rate(_adapter padapter, NETWORK_TYPE wirelessmode)
    {
        //NDIS_802_11_RATES_EX supported_rates;

        //mlme_ext_priv    pmlmeext = padapter.mlmeextpriv;

        //_rtw_memset(supported_rates, 0, NDIS_802_11_LENGTH_RATES_EX);

        ///* clear B mod if current channel is in 5G band, avoid tx cck rate in 5G band. */
        //if (pmlmeext.cur_channel > 14)
        //    wirelessmode &= ~(NETWORK_TYPE.WIRELESS_11B);

        //if (wirelessmode.HasFlag(NETWORK_TYPE.WIRELESS_11B) && wirelessmode.HasFlag(NETWORK_TYPE.WIRELESS_11B))
        //{
        //    _rtw_memcpy(supported_rates, rtw_basic_rate_cck, 4);
        //}
        //else if (wirelessmode.HasFlag(NETWORK_TYPE.WIRELESS_11B))
        //{
        //    _rtw_memcpy(supported_rates, rtw_basic_rate_mix, 7);
        //}
        //else
        //{
        //    _rtw_memcpy(supported_rates, rtw_basic_rate_ofdm, 3);
        //}

        //if (wirelessmode.HasFlag(NETWORK_TYPE.WIRELESS_11B))
        //{
        //    update_mgnt_tx_rate(padapter, IEEE80211_CCK_RATE_1MB);
        //}
        //else
        //{
        //    update_mgnt_tx_rate(padapter, IEEE80211_OFDM_RATE_6MB);
        //}

        //rtw_hal_set_hwreg(padapter, HW_VAR_BASIC_RATE, supported_rates);
    }

    static void PHY_BB8812_Config_1T(PADAPTER Adapter)
    {
        /* BB OFDM RX Path_A */
        phy_set_bb_reg(Adapter, rRxPath_Jaguar, bRxPath_Jaguar, 0x11);
        /* BB OFDM TX Path_A */
        phy_set_bb_reg(Adapter, rTxPath_Jaguar, bMaskLWord, 0x1111);
        /* BB CCK R/Rx Path_A */
        phy_set_bb_reg(Adapter, rCCK_RX_Jaguar, bCCK_RX_Jaguar, 0x0);
        /* MCS support */
        phy_set_bb_reg(Adapter, 0x8bc, 0xc0000060, 0x4);
        /* RF Path_B HSSI OFF */
        phy_set_bb_reg(Adapter, 0xe00, 0xf, 0x4);
        /* RF Path_B Power Down */
        phy_set_bb_reg(Adapter, 0xe90, bMaskDWord, 0);
        /* ADDA Path_B OFF */
        phy_set_bb_reg(Adapter, 0xe60, bMaskDWord, 0);
        phy_set_bb_reg(Adapter, 0xe64, bMaskDWord, 0);
    }

    static void PHY_RFConfig8812(PADAPTER Adapter)
    {
        PHY_RF6052_Config_8812(Adapter);
    }

    static void PHY_RF6052_Config_8812(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* Initialize general global value */
        if (pHalData.rf_type == rf_type.RF_1T1R)
        {
            pHalData.NumTotalRFPath = 1;
        }
        else
        {
            pHalData.NumTotalRFPath = 2;
        }

        /*  */
        /* Config BB and RF */
        /*  */
        phy_RF6052_Config_ParaFile_8812(Adapter);
    }

    static void phy_RF6052_Config_ParaFile_8812(PADAPTER Adapter)
    {
        rf_path eRFPath;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        for (eRFPath = 0; (byte)eRFPath < pHalData.NumTotalRFPath; eRFPath++)
        {
            /*----Initialize RF fom connfiguration file----*/
            switch (eRFPath)
            {
                case rf_path.RF_PATH_A:
                    odm_config_rf_with_header_file(Adapter, odm_rf_config_type.CONFIG_RF_RADIO, eRFPath);
                    break;
                case rf_path.RF_PATH_B:
                    odm_config_rf_with_header_file(Adapter, odm_rf_config_type.CONFIG_RF_RADIO, eRFPath);
                    break;
                default:
                    break;
            }
        }

        odm_config_rf_with_tx_pwr_track_header_file(pHalData.odmpriv);
    }

    static bool phy_BB8812_Config_ParaFile(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        bool rtStatus = odm_config_bb_with_header_file(Adapter, odm_bb_config_type.CONFIG_BB_PHY_REG);

        /* Read PHY_REG.TXT BB INIT!! */

        if (rtStatus != true)
        {
            RTW_INFO("phy_BB8812_Config_ParaFile: CONFIG_BB_PHY_REG Fail!!");
            goto phy_BB_Config_ParaFile_Fail;
        }

        rtStatus = odm_config_bb_with_header_file(Adapter, odm_bb_config_type.CONFIG_BB_AGC_TAB);

        if (rtStatus != true)
        {
            RTW_INFO("phy_BB8812_Config_ParaFile CONFIG_BB_AGC_TAB Fail!!");
        }

        phy_BB_Config_ParaFile_Fail:

        return rtStatus;
    }


    static bool PHY_BBConfig8812(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        uint TmpU1B = 0;

        phy_InitBBRFRegisterDefinition(Adapter);

        /* tangw check start 20120412 */
        /* . APLL_EN,,APLL_320_GATEB,APLL_320BIAS,  auto config by hw fsm after pfsm_go (0x4 bit 8) set */
        TmpU1B = rtw_read8(Adapter, REG_SYS_FUNC_EN);

        TmpU1B |= FEN_USBA;

        rtw_write8(Adapter, REG_SYS_FUNC_EN, (byte)TmpU1B);

        rtw_write8(Adapter, REG_SYS_FUNC_EN, (byte)(TmpU1B | FEN_BB_GLB_RSTn | FEN_BBRSTB)); /* same with 8812 */
        /* 6. 0x1f[7:0] = 0x07 PathA RF Power On */
        rtw_write8(Adapter, REG_RF_CTRL, 0x07); /* RF_SDMRSTB,RF_RSTB,RF_EN same with 8723a */
        /* 7.  PathB RF Power On */
        rtw_write8(Adapter, REG_OPT_CTRL_8812 + 2, 0x7); /* RF_SDMRSTB,RF_RSTB,RF_EN same with 8723a */
        /* tangw check end 20120412 */


        /*  */
        /* Config BB and AGC */
        /*  */
        var rtStatus = phy_BB8812_Config_ParaFile(Adapter);

        hal_set_crystal_cap(Adapter, pHalData.crystal_cap);

        return rtStatus;
    }

    static void hal_set_crystal_cap(_adapter adapter, u8 crystal_cap)
    {
        crystal_cap = (byte)(crystal_cap & 0x3F);

        /* write 0x2C[30:25] = 0x2C[24:19] = CrystalCap */
        phy_set_bb_reg(adapter, REG_MAC_PHY_CTRL, 0x7FF80000u, (byte)(crystal_cap | (crystal_cap << 6)));
    }

    static void phy_InitBBRFRegisterDefinition(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* RF Interface Sowrtware Control */
        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfintfs =
            rFPGA0_XAB_RFInterfaceSW; /* 16 LSBs if read 32-bit from 0x870 */
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfintfs =
            rFPGA0_XAB_RFInterfaceSW; /* 16 MSBs if read 32-bit from 0x870 (16-bit for 0x872) */

        /* RF Interface Output (and Enable) */
        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfintfo = rFPGA0_XA_RFInterfaceOE; /* 16 LSBs if read 32-bit from 0x860 */
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfintfo = rFPGA0_XB_RFInterfaceOE; /* 16 LSBs if read 32-bit from 0x864 */

        /* RF Interface (Output and)  Enable */
        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfintfe =
            rFPGA0_XA_RFInterfaceOE; /* 16 MSBs if read 32-bit from 0x860 (16-bit for 0x862) */
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfintfe =
            rFPGA0_XB_RFInterfaceOE; /* 16 MSBs if read 32-bit from 0x864 (16-bit for 0x866) */

        pHalData.PHYRegDef[rf_path.RF_PATH_A].rf3wireOffset = rA_LSSIWrite_Jaguar; /* LSSI Parameter */
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rf3wireOffset = rB_LSSIWrite_Jaguar;

        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfHSSIPara2 = rHSSIRead_Jaguar; /* wire control parameter2 */
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfHSSIPara2 = rHSSIRead_Jaguar; /* wire control parameter2 */

        /* Tranceiver Readback LSSI/HSPI mode */
        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfLSSIReadBack = rA_SIRead_Jaguar;
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfLSSIReadBack = rB_SIRead_Jaguar;
        pHalData.PHYRegDef[rf_path.RF_PATH_A].rfLSSIReadBackPi = rA_PIRead_Jaguar;
        pHalData.PHYRegDef[rf_path.RF_PATH_B].rfLSSIReadBackPi = rB_PIRead_Jaguar;
    }

    static void _InitBurstPktLen(PADAPTER Adapter)
    {
        u8 speedvalue, provalue, temp;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);


        /* rtw_write16(Adapter, REG_TRXDMA_CTRL_8195, 0xf5b0); */
        /* rtw_write16(Adapter, REG_TRXDMA_CTRL_8812, 0xf5b4); */
        rtw_write8(Adapter, 0xf050, 0x01); /* usb3 rx interval */
        rtw_write16(Adapter, REG_RXDMA_STATUS, 0x7400); /* burset lenght=4, set 0x3400 for burset length=2 */
        rtw_write8(Adapter, 0x289, 0xf5); /* for rxdma control */
        /* rtw_write8(Adapter, 0x3a, 0x46); */

        /* 0x456 = 0x70, sugguested by Zhilin */
        rtw_write8(Adapter, REG_AMPDU_MAX_TIME_8812, 0x70);

        rtw_write32(Adapter, REG_AMPDU_MAX_LENGTH_8812, 0xffffffff);
        rtw_write8(Adapter, REG_USTIME_TSF, 0x50);
        rtw_write8(Adapter, REG_USTIME_EDCA, 0x50);

        speedvalue = rtw_read8(Adapter, 0xff); /* check device operation speed: SS 0xff bit7 */

        if ((speedvalue & BIT7) != 0)
        {
            /* USB2/1.1 Mode */
            temp = rtw_read8(Adapter, 0xfe17);
            if (((temp >> 4) & 0x03) == 0)
            {
                pHalData.UsbBulkOutSize = USB_HIGH_SPEED_BULK_SIZE;
                provalue = rtw_read8(Adapter, REG_RXDMA_PRO_8812);
                rtw_write8(Adapter, REG_RXDMA_PRO_8812,
                    (byte)((provalue | BIT4 | BIT3 | BIT2 | BIT1) & (NotBIT5))); /* set burst pkt len=512B */
            }
            else
            {
                pHalData.UsbBulkOutSize = 64;
                provalue = rtw_read8(Adapter, REG_RXDMA_PRO_8812);
                rtw_write8(Adapter, REG_RXDMA_PRO_8812,
                    (byte)((provalue | BIT5 | BIT3 | BIT2 | BIT1) & (NotBIT4))); /* set burst pkt len=64B */
            }
        }
        else
        {
            /* USB3 Mode */
            pHalData.UsbBulkOutSize = USB_SUPER_SPEED_BULK_SIZE;
            provalue = rtw_read8(Adapter, REG_RXDMA_PRO_8812);
            rtw_write8(Adapter, REG_RXDMA_PRO_8812,
                //((provalue | BIT3 | BIT2 | BIT1) & (~(BIT5 | BIT4)))); /* set burst pkt len=1k */
                (byte)((provalue | BIT3 | BIT2 | BIT1) & (0b1100_1111))); /* set burst pkt len=1k */

            rtw_write8(Adapter, 0xf008, (byte)(rtw_read8(Adapter, 0xf008) & 0xE7));
        }

        temp = rtw_read8(Adapter, REG_SYS_FUNC_EN);
        rtw_write8(Adapter, REG_SYS_FUNC_EN, (byte)(temp & (NotBIT10))); /* reset 8051 */

        rtw_write8(Adapter, REG_HT_SINGLE_AMPDU_8812,
            (byte)(rtw_read8(Adapter, REG_HT_SINGLE_AMPDU_8812) | BIT7)); /* enable single pkt ampdu */
        rtw_write8(Adapter, REG_RX_PKT_LIMIT, 0x18); /* for VHT packet length 11K */

        rtw_write8(Adapter, REG_PIFS, 0x00);

        rtw_write16(Adapter, REG_MAX_AGGR_NUM, 0x1f1f);
        rtw_write8(Adapter, REG_FWHW_TXQ_CTRL, (byte)(rtw_read8(Adapter, REG_FWHW_TXQ_CTRL) & (NotBIT7)));

        if (pHalData.AMPDUBurstMode)
        {
            rtw_write8(Adapter, REG_AMPDU_BURST_MODE_8812, 0x5F);
        }

        rtw_write8(Adapter, 0x1c,
            (byte)(rtw_read8(Adapter, 0x1c) | BIT5 | BIT6)); /* to prevent mac is reseted by bus. 20111208, by Page */

        /* ARFB table 9 for 11ac 5G 2SS */
        rtw_write32(Adapter, REG_ARFR0_8812, 0x00000010);
        rtw_write32(Adapter, REG_ARFR0_8812 + 4, 0xfffff000);

        /* ARFB table 10 for 11ac 5G 1SS */
        rtw_write32(Adapter, REG_ARFR1_8812, 0x00000010);
        rtw_write32(Adapter, REG_ARFR1_8812 + 4, 0x003ff000);

        /* ARFB table 11 for 11ac 24G 1SS */
        rtw_write32(Adapter, REG_ARFR2_8812, 0x00000015);
        rtw_write32(Adapter, REG_ARFR2_8812 + 4, 0x003ff000);
        /* ARFB table 12 for 11ac 24G 2SS */
        rtw_write32(Adapter, REG_ARFR3_8812, 0x00000015);
        rtw_write32(Adapter, REG_ARFR3_8812 + 4, 0xffcff000);
    }

    static void _InitBeaconMaxError_8812A(PADAPTER Adapter, BOOLEAN InfraMode)
    {
        rtw_write8(Adapter, REG_BCN_MAX_ERR, 0xFF);
    }

    static void _InitBeaconParameters_8812A(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        var val8 = DIS_TSF_UDT;
        var val16 = (u16)(val8 | (val8 << 8)); /* port0 and port1 */

        rtw_write16(Adapter, REG_BCN_CTRL, val16);

        /* TBTT setup time */
        rtw_write8(Adapter, REG_TBTT_PROHIBIT, TBTT_PROHIBIT_SETUP_TIME);

        /* TBTT hold time: 0x540[19:8] */
        rtw_write8(Adapter, REG_TBTT_PROHIBIT + 1, TBTT_PROHIBIT_HOLD_TIME_STOP_BCN & 0xFF);
        rtw_write8(Adapter, REG_TBTT_PROHIBIT + 2,
            (byte)((rtw_read8(Adapter, REG_TBTT_PROHIBIT + 2) & 0xF0) | (TBTT_PROHIBIT_HOLD_TIME_STOP_BCN >> 8)));

        rtw_write8(Adapter, REG_DRVERLYINT, DRIVER_EARLY_INT_TIME_8812); /* 5ms */
        rtw_write8(Adapter, REG_BCNDMATIM, BCN_DMA_ATIME_INT_TIME_8812); /* 2ms */

        /* Suggested by designer timchen. Change beacon AIFS to the largest number */
        /* beacause test chip does not contension before sending beacon. by tynli. 2009.11.03 */
        rtw_write16(Adapter, REG_BCNTCFG, 0x4413);

    }

    static void init_UsbAggregationSetting_8812A(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        ///* Tx aggregation setting */
        usb_AggSettingTxUpdate_8812A(Adapter);

        ///* Rx aggregation setting */
        usb_AggSettingRxUpdate_8812A(Adapter);

        /* 201/12/10 MH Add for USB agg mode dynamic switch. */
        pHalData.UsbRxHighSpeedMode = false;
    }

    static void _InitRetryFunction_8812A(PADAPTER Adapter)
    {
        uint value8;

        value8 = rtw_read8(Adapter, REG_FWHW_TXQ_CTRL);
        value8 |= EN_AMPDU_RTY_NEW;
        rtw_write8(Adapter, REG_FWHW_TXQ_CTRL, (byte)value8);

        /* Set ACK timeout */
        /* rtw_write8(Adapter, REG_ACKTO, 0x40);  */ /* masked by page for BCM IOT issue temporally */
        rtw_write8(Adapter, REG_ACKTO, 0x80);
    }

    static void _InitEDCA_8812AUsb(PADAPTER Adapter)
    {
        /* Set Spec SIFS (used in NAV) */
        rtw_write16(Adapter, REG_SPEC_SIFS, 0x100a);
        rtw_write16(Adapter, REG_MAC_SPEC_SIFS, 0x100a);

        /* Set SIFS for CCK */
        rtw_write16(Adapter, REG_SIFS_CTX, 0x100a);

        /* Set SIFS for OFDM */
        rtw_write16(Adapter, REG_SIFS_TRX, 0x100a);

        /* TXOP */
        rtw_write32(Adapter, REG_EDCA_BE_PARAM, 0x005EA42B);
        rtw_write32(Adapter, REG_EDCA_BK_PARAM, 0x0000A44F);
        rtw_write32(Adapter, REG_EDCA_VI_PARAM, 0x005EA324);
        rtw_write32(Adapter, REG_EDCA_VO_PARAM, 0x002FA226);

        /* 0x50 for 80MHz clock */
        rtw_write8(Adapter, REG_USTIME_TSF, 0x50);
        rtw_write8(Adapter, REG_USTIME_EDCA, 0x50);
    }

    static void _InitAdaptiveCtrl_8812AUsb(PADAPTER Adapter)
    {
        u16 value16;
        u32 value32;

        /* Response Rate Set */
        value32 = rtw_read32(Adapter, REG_RRSR);
        value32 &= NotRATE_BITMAP_ALL;

        if (Adapter.registrypriv.wireless_mode.HasFlag(NETWORK_TYPE.WIRELESS_11B))
            value32 |= RATE_RRSR_CCK_ONLY_1M;
        else
            value32 |= RATE_RRSR_WITHOUT_CCK;

        value32 |= RATE_RRSR_CCK_ONLY_1M;
        rtw_write32(Adapter, REG_RRSR, value32);

        /* CF-END Threshold */
        /* m_spIoBase.rtw_write8(REG_CFEND_TH, 0x1); */

        /* SIFS (used in NAV) */
        value16 = (u16)(_SPEC_SIFS_CCK(0x10) | _SPEC_SIFS_OFDM(0x10));
        rtw_write16(Adapter, REG_SPEC_SIFS, value16);

        /* Retry Limit */
        value16 = (u16)(BIT_LRL(RL_VAL_STA) | BIT_SRL(RL_VAL_STA));
        rtw_write16(Adapter, REG_RETRY_LIMIT, value16);
    }

    public static u16 BIT_LRL(u16 x) => (u16)(((x) & BIT_MASK_LRL) << BIT_SHIFT_LRL);
    public static u16 BIT_SRL(u16 x) => (u16)(((x) & BIT_MASK_SRL) << BIT_SHIFT_SRL);

    public static u16 _SPEC_SIFS_CCK(u16 x) => (u16)((x) & 0xFF);
    public static u16 _SPEC_SIFS_OFDM(u16 x) => (u16)(((x) & 0xFF) << 8);

    static void _InitWMACSetting_8812A(PADAPTER Adapter)
    {
        /* rcr = AAP | APM | AM | AB | APP_ICV | ADF | AMF | APP_FCS | HTC_LOC_CTRL | APP_MIC | APP_PHYSTS; */
        u32 rcr = RCR_APM |
                  RCR_AM |
                  RCR_AB |
                  RCR_CBSSID_DATA |
                  RCR_CBSSID_BCN |
                  RCR_APP_ICV |
                  RCR_AMF |
                  RCR_HTC_LOC_CTRL |
                  RCR_APP_MIC |
                  RCR_APP_PHYST_RXFF |
                  RCR_APPFCS |
                  FORCEACK;

        hw_var_rcr_config(Adapter, rcr);

        /* Accept all multicast address */
        rtw_write32(Adapter, REG_MAR, 0xFFFFFFFF);
        rtw_write32(Adapter, REG_MAR + 4, 0xFFFFFFFF);

        uint value16 = BIT10 | BIT5;
        rtw_write16(Adapter, REG_RXFLTMAP1, (u16)value16);
    }

    static void _InitNetworkType_8812A(PADAPTER Adapter)
    {
        u32 value32;

        value32 = rtw_read32(Adapter, REG_CR);
        /* TODO: use the other function to set network type */
        value32 = (value32 & ~MASK_NETTYPE) | _NETTYPE(NT_LINK_AP);

        rtw_write32(Adapter, REG_CR, value32);
    }

    public static u32 _NETTYPE(u32 x) => (((x) & 0x3) << 16);

    static void _InitInterrupt_8812AU(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u8 usb_opt;

        /* HIMR */
        rtw_write32(Adapter, REG_HIMR0_8812, pHalData.IntrMask[0] & 0xFFFFFFFF);
        rtw_write32(Adapter, REG_HIMR1_8812, pHalData.IntrMask[1] & 0xFFFFFFFF);
    }

    static void _InitDriverInfoSize_8812A(PADAPTER Adapter, u8 drvInfoSize)
    {
        rtw_write8(Adapter, REG_RX_DRVINFO_SZ, drvInfoSize);
    }

    static void _InitTransferPageSize_8812AUsb(PADAPTER Adapter)
    {

        u8 value8;
        value8 = _PSTX(PBP_512);

        rtw_write8(Adapter, REG_PBP, value8);
    }

    static byte _PSTX(byte x) => (byte)((x) << 4);

    static void _InitPageBoundary_8812AUsb(PADAPTER Adapter)
    {
        /* u2Byte 			rxff_bndy; */
        /* u2Byte			Offset; */
        /* BOOLEAN			bSupportRemoteWakeUp; */

        /* Adapter.HalFunc.get_hal_def_var_handler(Adapter, HAL_DEF_WOWLAN , &bSupportRemoteWakeUp); */
        /* RX Page Boundary */
        /* srand(static_cast<unsigned int>(time(NULL)) ); */

        /*	Offset = MAX_RX_DMA_BUFFER_SIZE_8812/256;
         *	rxff_bndy = (Offset*256)-1; */

        rtw_write16(Adapter, (REG_TRXFF_BNDY + 2), RX_DMA_BOUNDARY_8812);
    }

    static void _InitQueuePriority_8812AUsb(PADAPTER Adapter)
    {
        var pHalData = GET_HAL_DATA(Adapter);

        switch (pHalData.OutEpNumber)
        {
            case 2:
                _InitNormalChipTwoOutEpPriority_8812AUsb(Adapter);
                break;
            case 3:
                _InitNormalChipThreeOutEpPriority_8812AUsb(Adapter);
                break;
            case 4:
                _InitNormalChipFourOutEpPriority_8812AUsb(Adapter);
                break;
            default:
                RTW_INFO("_InitQueuePriority_8812AUsb(): Shall not reach here!\n");
                break;
        }
    }

    static void _InitNormalChipFourOutEpPriority_8812AUsb(PADAPTER Adapter)
    {

        registry_priv pregistrypriv = Adapter.registrypriv;
        u16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;

        if (!pregistrypriv.wifi_spec)
        {
            /* typical setting */
            beQ = QUEUE_LOW;
            bkQ = QUEUE_LOW;
            viQ = QUEUE_NORMAL;
            voQ = QUEUE_NORMAL;
            mgtQ = QUEUE_EXTRA;
            hiQ = QUEUE_HIGH;
        }
        else
        {
            /* for WMM */
            beQ = QUEUE_LOW;
            bkQ = QUEUE_NORMAL;
            viQ = QUEUE_NORMAL;
            voQ = QUEUE_HIGH;
            mgtQ = QUEUE_HIGH;
            hiQ = QUEUE_HIGH;
        }

        _InitNormalChipRegPriority_8812AUsb(Adapter, beQ, bkQ, viQ, voQ, mgtQ, hiQ);
        init_hi_queue_config_8812a_usb(Adapter);
    }

    static void init_hi_queue_config_8812a_usb(PADAPTER Adapter)
    {
        /* Packet in Hi Queue Tx immediately (No constraint for ATIM Period)*/
        rtw_write8(Adapter, REG_HIQ_NO_LMT_EN, 0xFF);
    }

    static void _InitNormalChipThreeOutEpPriority_8812AUsb(PADAPTER Adapter)
    {

        registry_priv pregistrypriv = Adapter.registrypriv;
        u16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;

        if (!pregistrypriv.wifi_spec)
        {
            /* typical setting */
            beQ = QUEUE_LOW;
            bkQ = QUEUE_LOW;
            viQ = QUEUE_NORMAL;
            voQ = QUEUE_HIGH;
            mgtQ = QUEUE_HIGH;
            hiQ = QUEUE_HIGH;
        }
        else
        {
            /* for WMM */
            beQ = QUEUE_LOW;
            bkQ = QUEUE_NORMAL;
            viQ = QUEUE_NORMAL;
            voQ = QUEUE_HIGH;
            mgtQ = QUEUE_HIGH;
            hiQ = QUEUE_HIGH;
        }

        _InitNormalChipRegPriority_8812AUsb(Adapter, beQ, bkQ, viQ, voQ, mgtQ, hiQ);
    }

    static void _InitNormalChipTwoOutEpPriority_8812AUsb(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        registry_priv pregistrypriv = Adapter.registrypriv;
        u16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;


        u16 valueHi = 0;
        u16 valueLow = 0;

        switch (pHalData.OutEpQueueSel)
        {
            case (TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ):
                valueHi = QUEUE_HIGH;
                valueLow = QUEUE_LOW;
                break;
            case (TxSele.TX_SELE_NQ | TxSele.TX_SELE_LQ):
                valueHi = QUEUE_NORMAL;
                valueLow = QUEUE_LOW;
                break;
            case (TxSele.TX_SELE_HQ | TxSele.TX_SELE_NQ):
                valueHi = QUEUE_HIGH;
                valueLow = QUEUE_NORMAL;
                break;
            default:
                valueHi = QUEUE_HIGH;
                valueLow = QUEUE_NORMAL;
                break;
        }

        if (!pregistrypriv.wifi_spec)
        {
            beQ = valueLow;
            bkQ = valueLow;
            viQ = valueHi;
            voQ = valueHi;
            mgtQ = valueHi;
            hiQ = valueHi;
        }
        else
        {
            /* for WMM ,CONFIG_OUT_EP_WIFI_MODE */
            beQ = valueLow;
            bkQ = valueHi;
            viQ = valueHi;
            voQ = valueLow;
            mgtQ = valueHi;
            hiQ = valueHi;
        }

        _InitNormalChipRegPriority_8812AUsb(Adapter, beQ, bkQ, viQ, voQ, mgtQ, hiQ);
    }

    static void _InitNormalChipRegPriority_8812AUsb(
        PADAPTER Adapter,
        u16 beQ,
        u16 bkQ,
        u16 viQ,
        u16 voQ,
        u16 mgtQ,
        u16 hiQ
    )
    {
        u16 value16 = (u16)(rtw_read16(Adapter, REG_TRXDMA_CTRL) & 0x7);

        value16 = (u16)(value16 |
                        _TXDMA_BEQ_MAP(beQ) | _TXDMA_BKQ_MAP(bkQ) |
                        _TXDMA_VIQ_MAP(viQ) | _TXDMA_VOQ_MAP(voQ) |
                        _TXDMA_MGQ_MAP(mgtQ) | _TXDMA_HIQ_MAP(hiQ));

        rtw_write16(Adapter, REG_TRXDMA_CTRL, value16);
    }

    public static u16 _TXDMA_CMQ_MAP(u16 x) => (u16)(((x) & 0x3) << 16);
    public static u16 _TXDMA_HIQ_MAP(u16 x) => (u16)(((x) & 0x3) << 14);
    public static u16 _TXDMA_MGQ_MAP(u16 x) => (u16)(((x) & 0x3) << 12);
    public static u16 _TXDMA_BKQ_MAP(u16 x) => (u16)(((x) & 0x3) << 10);
    public static u16 _TXDMA_BEQ_MAP(u16 x) => (u16)(((x) & 0x3) << 8);
    public static u16 _TXDMA_VIQ_MAP(u16 x) => (u16)(((x) & 0x3) << 6);
    public static u16 _TXDMA_VOQ_MAP(u16 x) => (u16)(((x) & 0x3) << 4);

    static void _InitTxBufferBoundary_8812AUsb(PADAPTER Adapter)
    {
        var pregistrypriv = Adapter.registrypriv;
        u8 txpktbuf_bndy;

        txpktbuf_bndy = TX_PAGE_BOUNDARY_8812;

        rtw_write8(Adapter, REG_BCNQ_BDNY, txpktbuf_bndy);
        rtw_write8(Adapter, REG_MGQ_BDNY, txpktbuf_bndy);
        rtw_write8(Adapter, REG_WMAC_LBK_BF_HD, txpktbuf_bndy);
        rtw_write8(Adapter, REG_TRXFF_BNDY, txpktbuf_bndy);
        rtw_write8(Adapter, REG_TDECTRL + 1, txpktbuf_bndy);

    }

    static bool PHY_MACConfig8812(PADAPTER Adapter)
    {
        bool rtStatus = false;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /*  */
        /* Config MAC */
        /*  */
        //rtStatus = phy_ConfigMACWithParaFile(Adapter, PHY_FILE_MAC_REG);
        if (rtStatus == false)
        {
            odm_config_mac_with_header_file(Adapter, pHalData.odmpriv);
            rtStatus = true;
        }

        return rtStatus;
    }

    static bool odm_config_mac_with_header_file(_adapter adapter, dm_struct dm)
    {
        bool result = true;

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "support_platform: 0x%X, support_interface: 0x%X, board_type: 0x%X\n",
        //    dm.support_platform, dm.support_interface, dm.board_type);

/* @1 AP doesn't use PHYDM initialization in these ICs */

        odm_read_and_config_mp_8812a_mac_reg(adapter, dm);

        //if (dm.fw_offload_ability & PHYDM_PHY_PARAM_OFFLOAD)
        //{
        //    result = phydm_set_reg_by_fw(dm,
        //        PHYDM_HALMAC_CMD_END,
        //        0,
        //        0,
        //        0,
        //        (rf_path)0,
        //        0);
        //    //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    //    "mac param offload end!result = %d", result);
        //}

        return result;
    }

    static void _InitHardwareDropIncorrectBulkOut_8812A(PADAPTER Adapter)
    {
        var DROP_DATA_EN = BIT9;
        u32 value32 = rtw_read32(Adapter, REG_TXDMA_OFFSET_CHK);
        value32 |= DROP_DATA_EN;
        rtw_write32(Adapter, REG_TXDMA_OFFSET_CHK, value32);
    }

    private static bool InitLLTTable8812A(PADAPTER padapter, u8 txpktbuf_bndy)
    {
        bool status = false;
        u32 i;
        u32 Last_Entry_Of_TxPktBuf = LAST_ENTRY_OF_TX_PKT_BUFFER_8812;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);

        for (i = 0; i < (txpktbuf_bndy - 1); i++)
        {
            status = _LLTWrite_8812A(padapter, i, i + 1);
            if (true != status)
            {
                return status;
            }
        }

        /* end of list */
        status = _LLTWrite_8812A(padapter, (uint)(txpktbuf_bndy - 1), 0xFF);
        if (true != status)
        {
            return status;
        }

        /* Make the other pages as ring buffer */
        /* This ring buffer is used as beacon buffer if we config this MAC as two MAC transfer. */
        /* Otherwise used as local loopback buffer. */
        for (i = txpktbuf_bndy; i < Last_Entry_Of_TxPktBuf; i++)
        {
            status = _LLTWrite_8812A(padapter, i, (i + 1));
            if (true != status)
            {
                return status;
            }
        }

        /* Let last entry point to the start entry of ring buffer */
        status = _LLTWrite_8812A(padapter, Last_Entry_Of_TxPktBuf, txpktbuf_bndy);
        if (true != status)
        {
            return status;
        }

        return status;
    }

    private static UInt32 _LLT_INIT_DATA(UInt32 x)
    {
        return ((x) & 0xFF);
    }

    private static UInt32 _LLT_INIT_ADDR(UInt32 x)
    {
        return (((x) & 0xFF) << 8);
    }

    private static UInt32 _LLT_OP(UInt32 x)
    {
        return (((x) & 0x3) << 30);
    }

    private static UInt32 _LLT_OP_VALUE(UInt32 x)
    {
        return (((x) >> 30) & 0x3);
    }

    private static bool _LLTWrite_8812A(PADAPTER Adapter, u32 address, u32 data)
    {
        bool status = true;
        s32 count = 0;
        u32 value = _LLT_INIT_ADDR(address) | _LLT_INIT_DATA(data) | _LLT_OP(_LLT_WRITE_ACCESS);

        rtw_write32(Adapter, REG_LLT_INIT, value);

        /* polling */
        do
        {
            value = rtw_read32(Adapter, REG_LLT_INIT);
            if (_LLT_NO_ACTIVE == _LLT_OP_VALUE(value))
            {
                break;
            }

            if (count > POLLING_LLT_THRESHOLD)
            {
                status = false;
                break;
            }

            ++count;
        } while (true);

        return status;
    }

    private static bool rtw_hal_power_on(_adapter padapter)
    {
        return _InitPowerOn_8812AU(padapter);
    }

    public static bool _InitPowerOn_8812AU(_adapter padapter)
    {
        u16 u2btmp = 0;
        u8 u1btmp = 0;
        bool bMacPwrCtrlOn = false;
        /* HW Power on sequence */

        bMacPwrCtrlOn = padapter.HalData.bMacPwrCtrlOn;
        if (bMacPwrCtrlOn == true)
        {
            return true;
        }

        if (!HalPwrSeqCmdParsing(padapter,
                CutMsk.PWR_CUT_ALL_MSK,
                FabMsk.PWR_FAB_ALL_MSK,
                InterfaceMask.PWR_INTF_USB_MSK,
                PowerSequences.Rtl8812_NIC_ENABLE_FLOW))
        {
            RTW_ERR("_InitPowerOn_8812AU: run power on flow fail");
            return false;
        }

        /* Enable MAC DMA/WMAC/SCHEDULE/SEC block */
        /* Set CR bit10 to enable 32k calibration. Suggested by SD1 Gimmy. Added by tynli. 2011.08.31. */
        rtw_write16(padapter, REG_CR, 0x00); /* suggseted by zhouzhou, by page, 20111230 */
        u2btmp = rtw_read16(padapter, REG_CR);
        u2btmp |= (ushort)(
            CrBit.HCI_TXDMA_EN |
            CrBit.HCI_RXDMA_EN |
            CrBit.TXDMA_EN |
            CrBit.RXDMA_EN |
            CrBit.PROTOCOL_EN |
            CrBit.SCHEDULE_EN |
            CrBit.ENSEC |
            CrBit.CALTMR_EN);
        rtw_write16(padapter, REG_CR, u2btmp);

        /* Need remove below furture, suggest by Jackie. */
        /* if 0xF0[24] =1 (LDO), need to set the 0x7C[6] to 1. */

        bMacPwrCtrlOn = true;
        padapter.HalData.bMacPwrCtrlOn = bMacPwrCtrlOn;

        return true;
    }

    static void rtl8812au_hw_reset(_adapter Adapter)
    {
        uint reg_val = 0;
        if ((rtw_read8(Adapter, REG_MCUFWDL) & BIT7) != 0)
        {
            _8051Reset8812(Adapter);
            rtw_write8(Adapter, REG_MCUFWDL, 0x00);
            /* before BB reset should do clock gated */
            rtw_write32(Adapter, rFPGA0_XCD_RFPara,
                rtw_read32(Adapter, rFPGA0_XCD_RFPara) | (BIT6));
            /* reset BB */
            reg_val = rtw_read8(Adapter, REG_SYS_FUNC_EN);
            reg_val = (byte)(reg_val & ~(BIT0 | BIT1));
            rtw_write8(Adapter, REG_SYS_FUNC_EN, (byte)reg_val);
            /* reset RF */
            rtw_write8(Adapter, REG_RF_CTRL, 0);
            /* reset TRX path */
            rtw_write16(Adapter, REG_CR, 0);
            /* reset MAC */
            reg_val = rtw_read8(Adapter, REG_APS_FSMCO + 1);
            reg_val |= BIT1;
            rtw_write8(Adapter, REG_APS_FSMCO + 1, (byte)reg_val); /* reg0x5[1] ,auto FSM off */

            reg_val = rtw_read8(Adapter, REG_APS_FSMCO + 1);

            /* check if   reg0x5[1] auto cleared */
            while ((reg_val & BIT1) != 0)
            {
                Thread.Sleep(1);
                reg_val = rtw_read8(Adapter, REG_APS_FSMCO + 1);
            }

            reg_val |= BIT0;
            rtw_write8(Adapter, REG_APS_FSMCO + 1, (byte)reg_val); /* reg0x5[0] ,auto FSM on */

            reg_val = rtw_read8(Adapter, REG_SYS_FUNC_EN + 1);
            reg_val = (byte)(reg_val & ~(BIT4 | BIT7));
            rtw_write8(Adapter, REG_SYS_FUNC_EN + 1, (byte)reg_val);
            reg_val = rtw_read8(Adapter, REG_SYS_FUNC_EN + 1);
            reg_val = (byte)(reg_val | BIT4 | BIT7);
            rtw_write8(Adapter, REG_SYS_FUNC_EN + 1, (byte)reg_val);
        }
    }

    static void _8051Reset8812(PADAPTER padapter)
    {
        u8 u1bTmp, u1bTmp2;

        /* Reset MCU IO Wrapper- sugggest by SD1-Gimmy */

        u1bTmp2 = rtw_read8(padapter, REG_RSV_CTRL);
        rtw_write8(padapter, REG_RSV_CTRL, (byte)(u1bTmp2 & (NotBIT1)));
        u1bTmp2 = rtw_read8(padapter, REG_RSV_CTRL + 1);
        rtw_write8(padapter, REG_RSV_CTRL + 1, (byte)(u1bTmp2 & (NotBIT3)));


        u1bTmp = rtw_read8(padapter, REG_SYS_FUNC_EN + 1);
        rtw_write8(padapter, REG_SYS_FUNC_EN + 1, (byte)(u1bTmp & (NotBIT2)));

        /* Enable MCU IO Wrapper */

        u1bTmp2 = rtw_read8(padapter, REG_RSV_CTRL);
        rtw_write8(padapter, REG_RSV_CTRL, (byte)(u1bTmp2 & (NotBIT1)));
        u1bTmp2 = rtw_read8(padapter, REG_RSV_CTRL + 1);
        rtw_write8(padapter, REG_RSV_CTRL + 1, (byte)(u1bTmp2 | (BIT3)));


        rtw_write8(padapter, REG_SYS_FUNC_EN + 1, (byte)(u1bTmp | (BIT2)));

        RTW_INFO("=====> _8051Reset8812(): 8051 reset success .");
    }

    static bool HalPwrSeqCmdParsing(
        _adapter adapter,
        CutMsk CutVersion,
        FabMsk FabVersion,
        InterfaceMask InterfaceType,
        WLAN_PWR_CFG[] PwrSeqCmd)
    {
        bool bHWICSupport = false;
        UInt32 AryIdx = 0;
        //UInt16 offset = 0;
        UInt32 pollingCount = 0; /* polling autoload done. */

        do
        {
            var PwrCfgCmd = PwrSeqCmd[AryIdx];

            /* 2 Only Handle the command whose FAB, CUT, and Interface are matched */
            //if ((GET_PWR_CFG_FAB_MASK(PwrCfgCmd) & FabVersion) &&
            //    (GET_PWR_CFG_CUT_MASK(PwrCfgCmd) & CutVersion) &&
            //    (GET_PWR_CFG_INTF_MASK(PwrCfgCmd) & InterfaceType))
            if (((PwrCfgCmd.fab_msk & FabVersion) != 0) &&
                ((PwrCfgCmd.cut_msk & CutVersion) != 0) &&
                ((PwrCfgCmd.interface_msk & InterfaceType) != 0))
            {
                switch (PwrCfgCmd.cmd)
                {
                    case PwrCmd.PWR_CMD_READ:
                        break;

                    case PwrCmd.PWR_CMD_WRITE:
                    {
                        var offset = PwrCfgCmd.offset;
                        /* Read the value from system register */
                        var currentOffsetValue = Read8(adapter, offset);

                        currentOffsetValue = (byte)(currentOffsetValue & unchecked((byte)(~PwrCfgCmd.msk)));
                        currentOffsetValue = (byte)(currentOffsetValue | ((PwrCfgCmd.value) & (PwrCfgCmd.msk)));

                        /* Write the value back to sytem register */
                        Write8(adapter, offset, currentOffsetValue);
                    }
                        break;

                    case PwrCmd.PWR_CMD_POLLING:

                    {
                        var bPollingBit = false;
                        var offset = (PwrCfgCmd.offset);
                        UInt32 maxPollingCnt = 5000;
                        bool flag = false;

                        // HW_VAR_PWR_CMD is undefined. Always 0
                        //     rtw_hal_get_hwreg(HW_VAR_PWR_CMD, &bHWICSupport);
                        // if (bHWICSupport && offset == 0x06)
                        // {
                        //     flag = false;
                        //     maxPollingCnt = 100000;
                        // }
                        // else
                        // {
                        //
                        // }

                        maxPollingCnt = 5000;

                        do
                        {
                            var value = Read8(adapter, offset);

                            value = (byte)(value & PwrCfgCmd.msk);
                            if (value == ((PwrCfgCmd.value) & PwrCfgCmd.msk))
                            {
                                bPollingBit = true;
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }

                            if (pollingCount++ > maxPollingCnt)
                            {
                                // TODO: RTW_ERR("HalPwrSeqCmdParsing: Fail to polling Offset[%#x]=%02x\n", offset, value);

                                /* For PCIE + USB package poll power bit timeout issue only modify 8821AE and 8723BE */
                                if (bHWICSupport && offset == 0x06 && flag == false)
                                {

                                    // TODO: RTW_ERR("[WARNING] PCIE polling(0x%X) timeout(%d), Toggle 0x04[3] and try again.\n", offset, maxPollingCnt);

                                    Write8(adapter, 0x04, (byte)(Read8(adapter, 0x04) | BIT3));
                                    Write8(adapter, 0x04, (byte)(Read8(adapter, 0x04) & NotBIT3));

                                    /* Retry Polling Process one more time */
                                    pollingCount = 0;
                                    flag = true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        } while (!bPollingBit);
                    }

                        break;

                    case PwrCmd.PWR_CMD_DELAY:
                    {
                        if (PwrCfgCmd.value == (byte)PWRSEQ_DELAY_UNIT.PWRSEQ_DELAY_US)
                        {
                            Thread.Sleep((PwrCfgCmd.offset));
                        }
                        else
                        {
                            Thread.Sleep((PwrCfgCmd.offset) * 1000);
                        }
                    }
                        break;

                    case PwrCmd.PWR_CMD_END:
                        /* When this command is parsed, end the process */
                        return true;
                        break;

                    default:
                        break;
                }
            }

            AryIdx++; /* Add Array Index */
        } while (true);

        return true;
    }

    private static bool FirmwareDownload8812(PADAPTER Adapter, BOOLEAN bUsedWoWLANFw)
    {
        bool rtStatus = true;
        u8 write_fw = 0;
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        pwrctrl_priv pwrpriv = adapter_to_pwrctl(Adapter);

        var pFirmware = new RT_FIRMWARE_8812
        {
            szFwBuffer = Firmware.array_mp_8812a_fw_nic,
            ulFwLength = (uint)Firmware.array_mp_8812a_fw_nic.Length
        };

        var pFirmwareBuf = pFirmware.szFwBuffer.AsSpan();
        var FirmwareLen = pFirmware.ulFwLength;
        var pFwHdr = pFirmware.szFwBuffer;

        pHalData.firmware_version = (u16)GET_FIRMWARE_HDR_VERSION_8812(pFwHdr);
        pHalData.firmware_sub_version = (u16)GET_FIRMWARE_HDR_SUB_VER_8812(pFwHdr);
        pHalData.FirmwareSignature = (u16)GET_FIRMWARE_HDR_SIGNATURE_8812(pFwHdr);

        RTW_INFO(
            $"FirmwareDownload8812: fw_ver={pHalData.firmware_version} fw_subver={pHalData.firmware_sub_version} sig=0x{pHalData.FirmwareSignature:X}");

        if (Firmware.IS_FW_HEADER_EXIST_8812(pFwHdr))
        {
            /* Shift 32 bytes for FW header */
            pFirmwareBuf = pFirmwareBuf.Slice(32);
            FirmwareLen -= 32;
        }

        /* Suggested by Filen. If 8051 is running in RAM code, driver should inform Fw to reset by itself, */
        /* or it will cause download Fw fail. 2010.02.01. by tynli. */
        if ((rtw_read8(Adapter, REG_MCUFWDL) & BIT7) != 0)
        {
            /* 8051 RAM code */
            rtw_write8(Adapter, REG_MCUFWDL, 0x00);
            _8051Reset8812(Adapter);
        }

        _FWDownloadEnable_8812(Adapter, true);
        var fwdl_start_time = Stopwatch.StartNew();
        while ((write_fw++ < 3 || (fwdl_start_time.ElapsedMilliseconds) < 500))
        {
            /* reset FWDL chksum */
            rtw_write8(Adapter, REG_MCUFWDL, (byte)(rtw_read8(Adapter, REG_MCUFWDL) | FWDL_ChkSum_rpt));

            rtStatus = _WriteFW_8812(Adapter, pFirmwareBuf, FirmwareLen);
            if (rtStatus != true)
            {
                continue;
            }

            rtStatus = polling_fwdl_chksum(Adapter, 5, 50);
            if (rtStatus == true)
            {
                break;
            }
        }

        _FWDownloadEnable_8812(Adapter, false);
        if (true != rtStatus)
        {
            goto exit;
        }

        rtStatus = _FWFreeToGo8812(Adapter, 10, 200);
        if (true != rtStatus)
        {
            goto exit;
        }

        exit:

        InitializeFirmwareVars8812(Adapter);

        return rtStatus;
    }

    static void InitializeFirmwareVars8812(PADAPTER padapter)
    {
        /* Init H2C cmd. */
        rtw_write8(padapter, REG_HMETFR, 0x0f);
    }

    static bool _FWFreeToGo8812(_adapter adapter, u32 min_cnt, u32 timeout_ms)
    {
        bool ret = false;
        u32 value32;
        u32 cnt = 0;

        value32 = rtw_read32(adapter, REG_MCUFWDL);
        value32 |= MCUFWDL_RDY;
        value32 = (u32)(value32 & ~WINTINI_RDY);
        rtw_write32(adapter, REG_MCUFWDL, value32);

        _8051Reset8812(adapter);

        var start = Stopwatch.StartNew();
        /*  polling for FW ready */
        do
        {
            cnt++;
            value32 = rtw_read32(adapter, REG_MCUFWDL);
            if ((value32 & WINTINI_RDY) != 0)
            {
                break;
            }

        } while ((start.ElapsedMilliseconds) < timeout_ms || cnt < min_cnt);

        if (!((value32 & WINTINI_RDY) != 0))
        {
            goto exit;
        }

        //if (rtw_fwdl_test_trigger_wintint_rdy_fail())
        //{
        //    goto exit;
        //}

        ret = true;

        exit:
        RTW_INFO($"_FWFreeToGo8812: Polling FW ready {(ret ? "OK" : "Fail")}! ({cnt}), REG_MCUFWDL:0x{value32:X8}");

        return ret;
    }

    static bool polling_fwdl_chksum(_adapter adapter, uint min_cnt, uint timeout_ms)
    {
        bool ret = false;
        uint value32;
        var start = Stopwatch.StartNew();
        uint cnt = 0;

        /* polling CheckSum report */
        do
        {
            cnt++;
            value32 = rtw_read32(adapter, REG_MCUFWDL);
            if ((value32 & Firmware.FWDL_ChkSum_rpt) != 0)
            {
                break;
            }
        } while (start.ElapsedMilliseconds < timeout_ms || cnt < min_cnt);

        if (!((value32 & Firmware.FWDL_ChkSum_rpt) != 0))
        {
            return false;
        }

        //if (rtw_fwdl_test_trigger_chksum_fail())
        //{
        //    return false;

        //}

        //RTW_INFO("%s: Checksum report %s! (%u, %dms), REG_MCUFWDL:0x%08x\n", __FUNCTION__
        //    , (ret == _SUCCESS) ? "OK" : "Fail", cnt, rtw_get_passing_time_ms(start), value32);

        return true;
    }

    static bool _WriteFW_8812(_adapter adapter, Span<byte> buffer, UInt32 size)
    {
        const int MAX_DLFW_PAGE_SIZE = 4096; /* @ page : 4k bytes */

        /* Since we need dynamic decide method of dwonload fw, so we call this function to get chip version. */
        bool ret = true;
        Int32 pageNums, remainSize;
        Int32 page;
        int offset;
        var bufferPtr = buffer;

        pageNums = (int)(size / MAX_DLFW_PAGE_SIZE);
        /* RT_ASSERT((pageNums <= 4), ("Page numbers should not greater then 4\n")); */
        remainSize = (int)(size % MAX_DLFW_PAGE_SIZE);

        for (page = 0; page < pageNums; page++)
        {
            offset = page * MAX_DLFW_PAGE_SIZE;
            ret = _PageWrite_8812(adapter, page, bufferPtr.Slice(offset), MAX_DLFW_PAGE_SIZE);

            if (ret == false)
            {
                goto exit;
            }
        }

        if (remainSize != 0)
        {
            offset = pageNums * MAX_DLFW_PAGE_SIZE;
            page = pageNums;
            ret = _PageWrite_8812(adapter, page, bufferPtr.Slice(offset), remainSize);

            if (ret == false)
            {
                goto exit;
            }

        }

        exit:
        return ret;
    }

    static bool _PageWrite_8812(_adapter adapter, int page, Span<byte> buffer, int size)
    {
        byte value8;
        byte u8Page = (byte)(page & 0x07);

        value8 = (byte)((Read8(adapter, (REG_MCUFWDL + 2)) & 0xF8) | u8Page);
        Write8(adapter, (REG_MCUFWDL + 2), value8);

        return _BlockWrite_8812(adapter, buffer, size);
    }

    static bool _BlockWrite_8812(_adapter adapter, Span<byte> buffer, int buffSize)
    {
        const int MAX_REG_BOLCK_SIZE = 196;

        bool ret = true;

        UInt32 blockSize_p1 = 4; /* (Default) Phase #1 : PCI muse use 4-byte write to download FW */
        UInt32 blockSize_p2 = 8; /* Phase #2 : Use 8-byte, if Phase#1 use big size to write FW. */
        UInt32 blockSize_p3 = 1; /* Phase #3 : Use 1-byte, the remnant of FW image. */
        UInt32 blockCount_p1 = 0, blockCount_p2 = 0, blockCount_p3 = 0;
        UInt32 remainSize_p1 = 0, remainSize_p2 = 0;
        //u8			*bufferPtr	= (u8 *)buffer;
        UInt32 i = 0, offset = 0;

        blockSize_p1 = MAX_REG_BOLCK_SIZE;

        /* 3 Phase #1 */
        blockCount_p1 = (UInt32)(buffSize / blockSize_p1);
        remainSize_p1 = (UInt32)(buffSize % blockSize_p1);


        for (i = 0; i < blockCount_p1; i++)
        {
            WriteBytes(adapter, (ushort)(FW_START_ADDRESS + i * blockSize_p1),
                buffer.Slice((int)(i * blockSize_p1), (int)blockSize_p1));
        }

        /* 3 Phase #2 */
        if (remainSize_p1 != 0)
        {
            offset = blockCount_p1 * blockSize_p1;

            blockCount_p2 = remainSize_p1 / blockSize_p2;
            remainSize_p2 = remainSize_p1 % blockSize_p2;

            for (i = 0; i < blockCount_p2; i++)
            {
                WriteBytes(adapter, (ushort)(FW_START_ADDRESS + offset + i * blockSize_p2),
                    buffer.Slice((int)(offset + i * blockSize_p2), (int)blockSize_p2));
            }

        }

        /* 3 Phase #3 */
        if (remainSize_p2 != 0)
        {
            offset = (blockCount_p1 * blockSize_p1) + (blockCount_p2 * blockSize_p2);

            blockCount_p3 = remainSize_p2 / blockSize_p3;


            for (i = 0; i < blockCount_p3; i++)
            {
                Write8(adapter, (ushort)(FW_START_ADDRESS + offset + i), buffer[(int)(offset + i)]);
            }
        }

        return ret;
    }

    static void _FWDownloadEnable_8812(PADAPTER padapter, BOOLEAN enable)
    {
        u8 tmp;

        if (enable)
        {
            /* MCU firmware download enable. */
            tmp = rtw_read8(padapter, REG_MCUFWDL);
            rtw_write8(padapter, REG_MCUFWDL, (byte)(tmp | 0x01));

            /* 8051 reset */
            tmp = rtw_read8(padapter, REG_MCUFWDL + 2);
            rtw_write8(padapter, REG_MCUFWDL + 2, (byte)(tmp & 0xf7));
        }
        else
        {

            /* MCU firmware download disable. */
            tmp = rtw_read8(padapter, REG_MCUFWDL);
            rtw_write8(padapter, REG_MCUFWDL, (byte)(tmp & 0xfe));
        }
    }

    private static UInt32 GET_FIRMWARE_HDR_SIGNATURE_8812(byte[] __FwHdr)
    {
        return LE_BITS_TO_4BYTE(__FwHdr.AsSpan(0, 4), 0, 16);
        /* 92C0: test chip; 92C, 88C0: test chip; 88C1: MP A-cut; 92C1: MP A-cut */
    }

    private static UInt32 GET_FIRMWARE_HDR_VERSION_8812(byte[] __FwHdr)
    {
        return LE_BITS_TO_4BYTE(__FwHdr.AsSpan(4, 4), 0, 16); /* FW Version */
    }

    private static UInt32 GET_FIRMWARE_HDR_SUB_VER_8812(byte[] __FwHdr)
    {
        return LE_BITS_TO_4BYTE(__FwHdr.AsSpan(4, 4), 16, 8); /* FW Subversion, default 0x00 */
    }

    private static UInt32 LE_BITS_TO_4BYTE(Span<byte> __pStart, int __BitOffset, int __BitLen)
    {
        return ((LE_P4BYTE_TO_HOST_4BYTE(__pStart) >> (__BitOffset)) & BIT_LEN_MASK_32(__BitLen));
    }

    private static UInt32 LE_P4BYTE_TO_HOST_4BYTE(Span<byte> __pStart)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(__pStart);
    }

    private static UInt32 BIT_LEN_MASK_32(int __BitLen) => ((u32)(0xFFFFFFFF >> (32 - (__BitLen))));

    public static void read_chip_version_8812a(PADAPTER Adapter)
    {
        u32 value32;
        PHAL_DATA_TYPE pHalData;
        pHalData = GET_HAL_DATA(Adapter);

        value32 = rtw_read32(Adapter, REG_SYS_CFG);
        RTW_INFO($"read_chip_version_8812a SYS_CFG(0x{REG_SYS_CFG:X})=0x{value32:X8}");

        pHalData.version_id.ICType = HAL_IC_TYPE_E.CHIP_8812;

        pHalData.version_id.ChipType =
            ((value32 & RTL_ID) != 0 ? HAL_CHIP_TYPE_E.TEST_CHIP : HAL_CHIP_TYPE_E.NORMAL_CHIP);

        pHalData.version_id.RFType = HAL_RF_TYPE_E.RF_TYPE_2T2R; /* RF_2T2R; */

        if (Adapter.registrypriv.special_rf_path == 1)
            pHalData.version_id.RFType = HAL_RF_TYPE_E.RF_TYPE_1T1R; /* RF_1T1R; */

        pHalData.version_id.VendorType =
            ((value32 & VENDOR_ID) != 0 ? HAL_VENDOR_E.CHIP_VENDOR_UMC : HAL_VENDOR_E.CHIP_VENDOR_TSMC);

        pHalData.version_id.CUTVersion =
            (HAL_CUT_VERSION_E)((value32 & CHIP_VER_RTL_MASK) >> CHIP_VER_RTL_SHIFT); /* IC version (CUT) */
        pHalData.version_id.CUTVersion += 1;

        /* value32 = rtw_read32(Adapter, REG_GPIO_OUTSTS); */
        pHalData.version_id.ROMVer = 0; /* ROM code version. */

        /* For multi-function consideration. Added by Roger, 2010.10.06. */
        pHalData.MultiFunc = RT_MULTI_FUNC.RT_MULTI_FUNC_NONE;
        value32 = rtw_read32(Adapter, REG_MULTI_FUNC_CTRL);
        pHalData.MultiFunc |= ((value32 & WL_FUNC_EN) != 0 ? RT_MULTI_FUNC.RT_MULTI_FUNC_WIFI : 0);
        pHalData.MultiFunc |= ((value32 & BT_FUNC_EN) != 0 ? RT_MULTI_FUNC.RT_MULTI_FUNC_BT : 0);
        pHalData.PolarityCtl = ((value32 & WL_HWPDN_SL) != 0
            ? RT_POLARITY_CTL.RT_POLARITY_HIGH_ACT
            : RT_POLARITY_CTL.RT_POLARITY_LOW_ACT);

        rtw_hal_config_rftype(Adapter);

        //dump_chip_info(pHalData.version_id);
    }

    static void rtw_hal_config_rftype(PADAPTER padapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);

        if (IS_1T1R(pHalData.version_id))
        {
            pHalData.rf_type = rf_type.RF_1T1R;
            pHalData.NumTotalRFPath = 1;
        }
        else if (IS_2T2R(pHalData.version_id))
        {
            pHalData.rf_type = rf_type.RF_2T2R;
            pHalData.NumTotalRFPath = 2;
        }
        else if (IS_1T2R(pHalData.version_id))
        {
            pHalData.rf_type = rf_type.RF_1T2R;
            pHalData.NumTotalRFPath = 2;
        }
        else if (IS_3T3R(pHalData.version_id))
        {
            pHalData.rf_type = rf_type.RF_3T3R;
            pHalData.NumTotalRFPath = 3;
        }
        else if (IS_4T4R(pHalData.version_id))
        {
            pHalData.rf_type = rf_type.RF_4T4R;
            pHalData.NumTotalRFPath = 4;
        }
        else
        {
            pHalData.rf_type = rf_type.RF_1T1R;
            pHalData.NumTotalRFPath = 1;
        }

        RTW_INFO($"rtw_hal_config_rftype RF_Type is {pHalData.rf_type} TotalTxPath is {pHalData.NumTotalRFPath}");
    }

    static void _InitQueueReservedPage_8812AUsb(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        registry_priv pregistrypriv = Adapter.registrypriv;
        u32 numHQ = 0;
        u32 numLQ = 0;
        u32 numNQ = 0;
        u32 numPubQ = 0;
        u32 value32;
        u8 value8;
        BOOLEAN bWiFiConfig = pregistrypriv.wifi_spec;

        if (!bWiFiConfig)
        {
            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_HQ))
            {
                numHQ = NORMAL_PAGE_NUM_HPQ_8812;
            }

            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_LQ))
            {
                numLQ = NORMAL_PAGE_NUM_LPQ_8812;
            }

            /* NOTE: This step shall be proceed before writting REG_RQPN.		 */
            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_NQ))
            {
                numNQ = NORMAL_PAGE_NUM_NPQ_8812;
            }
        }
        else
        {
            /* WMM		 */
            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_HQ))
            {
                numHQ = WMM_NORMAL_PAGE_NUM_HPQ_8812;
            }

            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_LQ))
            {
                numLQ = WMM_NORMAL_PAGE_NUM_LPQ_8812;
            }

            /* NOTE: This step shall be proceed before writting REG_RQPN.		 */
            if (pHalData.OutEpQueueSel.HasFlag(TxSele.TX_SELE_NQ))
            {
                numNQ = WMM_NORMAL_PAGE_NUM_NPQ_8812;
            }
        }

        numPubQ = TX_TOTAL_PAGE_NUMBER_8812 - numHQ - numLQ - numNQ;

        value8 = (u8)_NPQ(numNQ);
        rtw_write8(Adapter, REG_RQPN_NPQ, value8);

/* TX DMA */
        value32 = _HPQ(numHQ) | _LPQ(numLQ) | _PUBQ(numPubQ) | LD_RQPN();
        rtw_write32(Adapter, REG_RQPN, value32);
    }

    static u32 _NPQ(u32 x) => ((x) & 0xFF);
    static u32 _HPQ(u32 x) => ((x) & 0xFF);
    static u32 _LPQ(u32 x) => (((x) & 0xFF) << 8);
    static u32 _PUBQ(u32 x) => (((x) & 0xFF) << 16);
    static u32 LD_RQPN() => BIT31;

    static void usb_AggSettingTxUpdate_8812A(PADAPTER            Adapter)
    {

        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u32 value32;

        if (Adapter.registrypriv.wifi_spec)
        {
            pHalData.UsbTxAggMode = false;
        }

        if (pHalData.UsbTxAggMode)
        {
            value32 = rtw_read32(Adapter, REG_TDECTRL);
            value32 = value32 & ~(BLK_DESC_NUM_MASK << BLK_DESC_NUM_SHIFT);
            value32 |= ((pHalData.UsbTxAggDescNum & BLK_DESC_NUM_MASK) << BLK_DESC_NUM_SHIFT);

            rtw_write32(Adapter, REG_DWBCN0_CTRL_8812, value32);
            //if (IS_HARDWARE_TYPE_8821U(Adapter))   /* page added for Jaguar */
            //    rtw_write8(Adapter, REG_DWBCN1_CTRL_8812, pHalData.UsbTxAggDescNum << 1);
        }
    }


    static void usb_AggSettingRxUpdate_8812A(PADAPTER Adapter)
    {

        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        uint valueDMA = rtw_read8(Adapter, REG_TRXDMA_CTRL);
        switch (pHalData.rxagg_mode)
        {
            case RX_AGG_MODE.RX_AGG_DMA:
                valueDMA |= RXDMA_AGG_EN;
                /* 2012/10/26 MH For TX through start rate temp fix. */
            {
                u16 temp;

                /* Adjust DMA page and thresh. */
                temp = (u16)(pHalData.rxagg_dma_size | (pHalData.rxagg_dma_timeout << 8));
                rtw_write16(Adapter, REG_RXDMA_AGG_PG_TH, temp);
                rtw_write8(Adapter, REG_RXDMA_AGG_PG_TH + 3, (byte)BIT7); /* for dma agg , 0x280[31]GBIT_RXDMA_AGG_OLD_MOD, set 1 */
            }
                break;
            case RX_AGG_MODE.RX_AGG_USB:
                valueDMA |= RXDMA_AGG_EN;
            {
                u16 temp;

                /* Adjust DMA page and thresh. */
                temp = (u16)(pHalData.rxagg_usb_size | (pHalData.rxagg_usb_timeout << 8));
                rtw_write16(Adapter, REG_RXDMA_AGG_PG_TH, temp);
            }
                break;
            case RX_AGG_MODE.RX_AGG_MIX:
            case RX_AGG_MODE.RX_AGG_DISABLE:
            default:
                /* TODO: */
                break;
        }

        rtw_write8(Adapter, REG_TRXDMA_CTRL, (byte)valueDMA);
    }
}