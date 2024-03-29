﻿using Microsoft.Extensions.Logging;
using Rtl8812auNet.Rtl8812au.Enumerations;
using Rtl8812auNet.Rtl8812au.Models;
using Rtl8812auNet.Rtl8812au.PredefinedData;

namespace Rtl8812auNet.Rtl8812au.Modules;

internal class HalModule
{
    private readonly RtlUsbAdapter _device;
    private readonly RadioManagementModule _radioManagementModule;
    private readonly EepromManager _eepromManager;
    private readonly ILogger<HalModule> _logger;
    private readonly ILogger<FirmwareManager> _firmwareManagerLogger;
    private readonly bool _usbTxAggMode = true;
    private readonly byte _usbTxAggDescNum = 0x01; // adjust value for OQT Overflow issue 0x3; only 4 bits
    private readonly RX_AGG_MODE _rxAggMode = RX_AGG_MODE.RX_AGG_USB;
    private readonly byte _rxAggDmaTimeout = 0x6; /* 6, absolute time = 34ms/(2^6) */
    private readonly byte _rxAggDmaSize = 16; /* uint: 128b, 0x0A = 10 = MAX_RX_DMA_BUFFER_SIZE/2/pHalData.UsbBulkOutSize */
    private readonly UInt32[] _intrMask = new UInt32[3]; // TODO: Check where is set

    private bool _macPwrCtrlOn;

    public HalModule(
        RtlUsbAdapter device,
        RadioManagementModule radioManagementModule,
        EepromManager eepromManager,
        ILogger<HalModule> logger,
        ILogger<FirmwareManager> firmwareManagerLogger)
    {
        _device = device;
        _radioManagementModule = radioManagementModule;
        _eepromManager = eepromManager;
        _logger = logger;
        _firmwareManagerLogger = firmwareManagerLogger;
    }

    public bool rtw_hal_init(SelectedChannel selectedChannel)
    {
        var status = rtl8812au_hal_init();

        if (status)
        {
            _radioManagementModule.init_hw_mlme_ext(selectedChannel);
            _radioManagementModule.SetMonitorMode();
        }
        else
        {
            _logger.LogError("rtw_hal_init: fail");
        }

        return status;
    }

    private bool check_positive(UInt32 condition1, UInt32 condition2, UInt32 condition4)
    {
        var originalBoardType = _eepromManager.GetBoardType();

        uint boardType =
            ((originalBoardType & BIT4) >>> 4) << 0 | /* _GLNA*/
            ((originalBoardType & BIT3) >>> 3) << 1 | /* _GPA*/
            ((originalBoardType & BIT7) >>> 7) << 2 | /* _ALNA*/
            ((originalBoardType & BIT6) >>> 6) << 3 | /* _APA */
            ((originalBoardType & BIT2) >>> 2) << 4 | /* _BT*/
            ((originalBoardType & BIT1) >>> 1) << 5 | /* _NGFF*/
            ((originalBoardType & BIT5) >>> 5) << 6;  /* _TRSWT*/


        UInt32 cond1 = condition1;
        UInt32 cond2 = condition2;
        UInt32 cond4 = condition4;

        uint cut_version_for_para = (_eepromManager.Version.IS_A_CUT()) ? (uint)15 : (uint)_eepromManager.Version.CUTVersion;
        uint pkg_type_for_para = (byte)15;

        UInt32 driver1 = cut_version_for_para << 24 |
                      ((uint)RTL871X_HCI_TYPE.RTW_USB & 0xF0) << 16 |
                      pkg_type_for_para << 12 |
                      ((uint)RTL871X_HCI_TYPE.RTW_USB & 0x0F) << 8 |
                      boardType;

        UInt32 driver2 =
            ((uint)_eepromManager.TypeGLNA & 0xFF) << 0 |
            ((uint)_eepromManager.TypeGPA & 0xFF) << 8 |
            ((uint)_eepromManager.TypeALNA & 0xFF) << 16 |
            ((uint)_eepromManager.TypeAPA & 0xFF) << 24;

        UInt32 driver4 =
            ((uint)_eepromManager.TypeGLNA & 0xFF00) >> 8 |
            ((uint)_eepromManager.TypeGPA & 0xFF00) |
            ((uint)_eepromManager.TypeALNA & 0xFF00) << 8 |
            ((uint)_eepromManager.TypeAPA & 0xFF00) << 16;

        /*============== value Defined Check ===============*/
        /*QFN type [15:12] and cut version [27:24] need to do value check*/

        if (((cond1 & 0x0000F000) != 0) && ((cond1 & 0x0000F000) != (driver1 & 0x0000F000)))
        {
            return false;
        }

        if (((cond1 & 0x0F000000) != 0) && ((cond1 & 0x0F000000) != (driver1 & 0x0F000000)))
        {
            return false;
        }

        /*=============== Bit Defined Check ================*/
        /* We don't care [31:28] */

        cond1 &= 0x00FF0FFF;
        driver1 &= 0x00FF0FFF;

        if ((cond1 & driver1) == cond1)
        {
            UInt32 bit_mask = 0;

            if ((cond1 & 0x0F) == 0) /* board_type is DONTCARE*/
            {
                return true;
            }

            if ((cond1 & BIT0) != 0) /*GLNA*/
            {
                bit_mask |= 0x000000FF;
            }
            if ((cond1 & BIT1) != 0) /*GPA*/
            {
                bit_mask |= 0x0000FF00;
            }
            if ((cond1 & BIT2) != 0) /*ALNA*/
            {
                bit_mask |= 0x00FF0000;
            }
            if ((cond1 & BIT3) != 0) /*APA*/
            {
                bit_mask |= 0xFF000000;
            }

            if (((cond2 & bit_mask) == (driver2 & bit_mask)) &&
                ((cond4 & bit_mask) == (driver4 & bit_mask))) /* board_type of each RF path is matched*/
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    private bool rtl8812au_hal_init()
    {
        // Check if MAC has already power on. by tynli. 2011.05.27.
        var value8 = _device.rtw_read8(REG_SYS_CLKR + 1);
        var regCr = _device.rtw_read8(REG_CR);
        _logger.LogInformation("power-on :REG_SYS_CLKR 0x09=0x{value8:X}. REG_CR 0x100=0x{regCr:X}", value8, regCr);
        if ((value8 & BIT3) != 0 && (regCr != 0 && regCr != 0xEA))
        {
            /* pHalData.bMACFuncEnable = TRUE; */
            _logger.LogInformation("MAC has already power on");
        }
        else
        {
            /* pHalData.bMACFuncEnable = FALSE; */
            /* Set FwPSState to ALL_ON mode to prevent from the I/O be return because of 32k */
            /* state which is set before sleep under wowlan mode. 2012.01.04. by tynli. */
            /* pHalData.FwPSState = FW_PS_STATE_ALL_ON_88E; */
            _logger.LogInformation("MAC has not been powered on yet");
        }

        _device.rtw_write8(REG_RF_CTRL, 5);
        _device.rtw_write8(REG_RF_CTRL, 7);
        _device.rtw_write8(REG_RF_B_CTRL_8812, 5);
        _device.rtw_write8(REG_RF_B_CTRL_8812, 7);

        // If HW didn't go through a complete de-initial procedure,
        // it probably occurs some problem for double initial procedure.
        // Like "CONFIG_DEINIT_BEFORE_INIT" in 92du chip
        _device.rtl8812au_hw_reset();

        var initPowerOnStatus = InitPowerOn();
        if (initPowerOnStatus == false)
        {
            return false;
        }

        // ATTENTION!! BOUNDARY size depends on wifi_spec aka WMM or not WMM
        var initLltTable8812AStatus = InitLLTTable8812A(TX_PAGE_BOUNDARY_8812);
        if (initLltTable8812AStatus == false)
        {
            return false;
        }

        _InitHardwareDropIncorrectBulkOut_8812A();

        var fwManager = new FirmwareManager(_device, _firmwareManagerLogger);
        fwManager.FirmwareDownload8812();

        PHY_MACConfig8812();

        _InitQueueReservedPage_8812AUsb();
        _InitTxBufferBoundary_8812AUsb();

        _InitQueuePriority_8812AUsb();
        _InitPageBoundary_8812AUsb();

        _InitTransferPageSize_8812AUsb();

        // Get Rx PHY status in order to report RSSI and others.
        _InitDriverInfoSize_8812A(DRVINFO_SZ);

        _InitInterrupt_8812AU();
        _InitNetworkType_8812A(); /* set msr	 */
        _InitWMACSetting_8812A();
        _InitAdaptiveCtrl_8812AUsb();
        _InitEDCA_8812AUsb();

        _InitRetryFunction_8812A();
        init_UsbAggregationSetting_8812A();

        _InitBeaconParameters_8812A();
        _InitBeaconMaxError_8812A();

        _InitBurstPktLen(); // added by page. 20110919

        // Init CR MACTXEN, MACRXEN after setting RxFF boundary REG_TRXFF_BNDY to patch
        // Hw bug which Hw initials RxFF boundry size to a value which is larger than the real Rx buffer size in 88E.
        // 2011.08.05. by tynli.
        value8 = _device.rtw_read8(REG_CR);
        _device.rtw_write8(REG_CR, (byte)(value8 | MACTXEN | MACRXEN));

        _device.rtw_write16(REG_PKT_VO_VI_LIFE_TIME, 0x0400); /* unit: 256us. 256ms */
        _device.rtw_write16(REG_PKT_BE_BK_LIFE_TIME, 0x0400); /* unit: 256us. 256ms */

        var bbConfig8812Status = PHY_BBConfig8812();
        if (bbConfig8812Status == false)
        {
            return false;
        }

        PHY_RF6052_Config_8812();

        if (_eepromManager.RfType == RfType.RF_1T1R)
        {
            PHY_BB8812_Config_1T();
        }

        if (registry_priv.rf_config == RfType.RF_1T2R)
        {
            _device.phy_set_bb_reg(rTxPath_Jaguar, bMaskLWord, 0x1111);
        }


        if (registry_priv.channel <= 14)
        {
            _radioManagementModule.PHY_SwitchWirelessBand8812(BandType.BAND_ON_2_4G);
        }
        else
        {
            _radioManagementModule.PHY_SwitchWirelessBand8812(BandType.BAND_ON_5G);
        }

        _radioManagementModule.rtw_hal_set_chnl_bw(
            registry_priv.channel,
            ChannelWidth.CHANNEL_WIDTH_20,
            HAL_PRIME_CHNL_OFFSET_DONT_CARE,
            HAL_PRIME_CHNL_OFFSET_DONT_CARE);


        // HW SEQ CTRL
        // set 0x0 to 0xFF by tynli. Default enable HW SEQ NUM.
        _device.rtw_write8(REG_HWSEQ_CTRL, 0xFF);


        // Disable BAR, suggested by Scott
        // 2010.04.09 add by hpfan
        _device.rtw_write32(REG_BAR_MODE_CTRL, 0x0201ffff);

        if (registry_priv.wifi_spec)
        {
            _device.rtw_write16(REG_FAST_EDCA_CTRL, 0);
        }

        // Nav limit , suggest by scott
        _device.rtw_write8(0x652, 0x0);


        /* 0x4c6[3] 1: RTS BW = Data BW */
        /* 0: RTS BW depends on CCA / secondary CCA result. */
        _device.rtw_write8(REG_QUEUE_CTRL, (byte)(_device.rtw_read8(REG_QUEUE_CTRL) & 0xF7));

        /* enable Tx report. */
        _device.rtw_write8(REG_FWHW_TXQ_CTRL + 1, 0x0F);

        /* Suggested by SD1 pisa. Added by tynli. 2011.10.21. */
        _device.rtw_write8(REG_EARLY_MODE_CONTROL_8812 + 3, 0x01); /* Pretx_en, for WEP/TKIP SEC */

        /* tynli_test_tx_report. */
        _device.rtw_write16(REG_TX_RPT_TIME, 0x3DF0);

        /* Reset USB mode switch setting */
        _device.rtw_write8(REG_SDIO_CTRL_8812, 0x0);
        _device.rtw_write8(REG_ACLK_MON, 0x0);

        _device.rtw_write8(REG_USB_HRPWM, 0);

        // TODO:
        ///* ack for xmit mgmt frames. */
        _device.rtw_write32(REG_FWHW_TXQ_CTRL, _device.rtw_read32(REG_FWHW_TXQ_CTRL) | BIT12);

        return true;
    }

    private static UInt32 _NPQ(UInt32 x) => ((x) & 0xFF);
    private static UInt32 _HPQ(UInt32 x) => ((x) & 0xFF);
    private static UInt32 _LPQ(UInt32 x) => (((x) & 0xFF) << 8);
    private static UInt32 _PUBQ(UInt32 x) => (((x) & 0xFF) << 16);
    private static UInt32 LD_RQPN() => BIT31;
    private static UInt16 _TXDMA_HIQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 14);
    private static UInt16 _TXDMA_MGQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 12);
    private static UInt16 _TXDMA_BKQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 10);
    private static UInt16 _TXDMA_BEQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 8);
    private static UInt16 _TXDMA_VIQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 6);
    private static UInt16 _TXDMA_VOQ_MAP(UInt16 x) => (UInt16)(((x) & 0x3) << 4);
    private static byte _PSTX(byte x) => (byte)((x) << 4);
    private static UInt32 _NETTYPE(UInt32 x) => (((x) & 0x3) << 16);
    private static UInt16 BIT_LRL(UInt16 x) => (UInt16)(((x) & BIT_MASK_LRL) << BIT_SHIFT_LRL);
    private static UInt16 BIT_SRL(UInt16 x) => (UInt16)(((x) & BIT_MASK_SRL) << BIT_SHIFT_SRL);
    private static UInt16 _SPEC_SIFS_CCK(UInt16 x) => (UInt16)((x) & 0xFF);
    private static UInt16 _SPEC_SIFS_OFDM(UInt16 x) => (UInt16)(((x) & 0xFF) << 8);

    private void PHY_BB8812_Config_1T()
    {
        /* BB OFDM RX Path_A */
        _device.phy_set_bb_reg(rRxPath_Jaguar, bRxPath_Jaguar, 0x11);
        /* BB OFDM TX Path_A */
        _device.phy_set_bb_reg(rTxPath_Jaguar, bMaskLWord, 0x1111);
        /* BB CCK R/Rx Path_A */
        _device.phy_set_bb_reg(rCCK_RX_Jaguar, bCCK_RX_Jaguar, 0x0);
        /* MCS support */
        _device.phy_set_bb_reg(0x8bc, 0xc0000060, 0x4);
        /* RF Path_B HSSI OFF */
        _device.phy_set_bb_reg(0xe00, 0xf, 0x4);
        /* RF Path_B Power Down */
        _device.phy_set_bb_reg(0xe90, bMaskDWord, 0);
        /* ADDA Path_B OFF */
        _device.phy_set_bb_reg(0xe60, bMaskDWord, 0);
        _device.phy_set_bb_reg(0xe64, bMaskDWord, 0);
    }

    public void PHY_RF6052_Config_8812()
    {
        /*  */
        /* Config BB and RF */
        /*  */
        phy_RF6052_Config_ParaFile_8812();
    }

    private void phy_RF6052_Config_ParaFile_8812()
    {
        RfPath eRFPath;

        for (eRFPath = 0; (byte)eRFPath < _eepromManager.NumTotalRfPath; eRFPath++)
        {
            /*----Initialize RF fom connfiguration file----*/
            switch (eRFPath)
            {
                case RfPath.RF_PATH_A:
                    odm_config_rf_with_header_file(odm_rf_config_type.CONFIG_RF_RADIO, eRFPath);
                    break;
                case RfPath.RF_PATH_B:
                    odm_config_rf_with_header_file(odm_rf_config_type.CONFIG_RF_RADIO, eRFPath);
                    break;
                default:
                    break;
            }
        }
    }

    private void odm_config_rf_with_header_file(odm_rf_config_type config_type, RfPath e_rf_path)
    {
        if (config_type == odm_rf_config_type.CONFIG_RF_RADIO)
        {
            if (e_rf_path == RfPath.RF_PATH_A)
            {
                //READ_AND_CONFIG_MP(8812a, _radioa);
                odm_read_and_config_mp_8812a_radioa();
            }
            else if (e_rf_path == RfPath.RF_PATH_B)
            {
                //READ_AND_CONFIG_MP(8812a, _radiob);
                odm_read_and_config_mp_8812a_radiob();
            }
        }
    }

    private void odm_read_and_config_mp_8812a_radiob()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        var array_len = halhwimg8812a_rf.array_mp_8812a_radiob.Length;
        UInt32[] array = halhwimg8812a_rf.array_mp_8812a_radiob;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >>> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        // PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        // PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_rf_radio_b_8812a(v1, v2);
                }
            }

            i = i + 2;
        }
    }

    private void odm_read_and_config_mp_8812a_radioa()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        var array_len = halhwimg8812a_rf.array_mp_8812a_radioa.Length;
        UInt32[] array = halhwimg8812a_rf.array_mp_8812a_radioa;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >>> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_rf_radio_a_8812a(v1, v2);
                }
            }

            i = i + 2;
        }
    }

    private void odm_config_rf_radio_a_8812a(UInt32 addr, UInt32 data)
    {
        UInt32 content = 0x1000; /* RF_Content: radioa_txt */
        UInt32 maskfor_phy_set = (UInt32)(content & 0xE000);

        odm_config_rf_reg_8812a(addr, data, RfPath.RF_PATH_A, (UInt16)(addr | maskfor_phy_set));
    }

    private void odm_config_rf_radio_b_8812a(UInt32 addr, UInt32 data)
    {
        UInt32 content = 0x1001; /* RF_Content: radiob_txt */
        UInt32 maskfor_phy_set = (UInt32)(content & 0xE000);

        odm_config_rf_reg_8812a(addr, data, RfPath.RF_PATH_B, (UInt16)(addr | maskfor_phy_set));
    }

    private void odm_config_rf_reg_8812a(UInt32 addr, UInt32 data, RfPath RF_PATH, UInt16 reg_addr)
    {
        if (addr == 0xfe || addr == 0xffe)
        {
            ODM_sleep_ms(50);
        }
        else
        {
            odm_set_rf_reg(RF_PATH, reg_addr, RFREGOFFSETMASK, data);
            /* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }
    }

    private void odm_set_rf_reg(RfPath e_rf_path, UInt16 reg_addr, UInt32 bit_mask, UInt32 data)
    {
        _radioManagementModule.phy_set_rf_reg(e_rf_path, reg_addr, bit_mask, data);
    }

    private bool PHY_BBConfig8812()
    {
        /* tangw check start 20120412 */
        /* . APLL_EN,,APLL_320_GATEB,APLL_320BIAS,  auto config by hw fsm after pfsm_go (0x4 bit 8) set */
        uint TmpU1B = _device.rtw_read8(REG_SYS_FUNC_EN);

        TmpU1B |= FEN_USBA;

        _device.rtw_write8(REG_SYS_FUNC_EN, (byte)TmpU1B);

        _device.rtw_write8(REG_SYS_FUNC_EN, (byte)(TmpU1B | FEN_BB_GLB_RSTn | FEN_BBRSTB)); /* same with 8812 */
        /* 6. 0x1f[7:0] = 0x07 PathA RF Power On */
        _device.rtw_write8(REG_RF_CTRL, 0x07); /* RF_SDMRSTB,RF_RSTB,RF_EN same with 8723a */
        /* 7.  PathB RF Power On */
        _device.rtw_write8(REG_OPT_CTRL_8812 + 2, 0x7); /* RF_SDMRSTB,RF_RSTB,RF_EN same with 8723a */
        /* tangw check end 20120412 */


        /*  */
        /* Config BB and AGC */
        /*  */
        var rtStatus = phy_BB8812_Config_ParaFile();

        hal_set_crystal_cap(_eepromManager.crystal_cap);

        return rtStatus;
    }

    private void hal_set_crystal_cap(byte crystal_cap)
    {
        crystal_cap = (byte)(crystal_cap & 0x3F);

        /* write 0x2C[30:25] = 0x2C[24:19] = CrystalCap */
        _device.phy_set_bb_reg(REG_MAC_PHY_CTRL, 0x7FF80000u, (byte)(crystal_cap | (crystal_cap << 6)));
    }

    private bool phy_BB8812_Config_ParaFile()
    {
        bool rtStatus = odm_config_bb_with_header_file(odm_bb_config_type.CONFIG_BB_PHY_REG);

        /* Read PHY_REG.TXT BB INIT!! */

        if (rtStatus != true)
        {
            _logger.LogInformation("phy_BB8812_Config_ParaFile: CONFIG_BB_PHY_REG Fail!!");
            goto phy_BB_Config_ParaFile_Fail;
        }

        rtStatus = odm_config_bb_with_header_file(odm_bb_config_type.CONFIG_BB_AGC_TAB);

        if (rtStatus != true)
        {
            _logger.LogInformation("phy_BB8812_Config_ParaFile CONFIG_BB_AGC_TAB Fail!!");
        }

        phy_BB_Config_ParaFile_Fail:

        return rtStatus;
    }

    private bool odm_config_bb_with_header_file(odm_bb_config_type config_type)
    {
        bool result = true;

        /* @1 AP doesn't use PHYDM initialization in these ICs */

        if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG)
        {
            //READ_AND_CONFIG_MP(8812a, _phy_reg);
            odm_read_and_config_mp_8812a_phy_reg();
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB)
        {
            //READ_AND_CONFIG_MP(8812a, _agc_tab);
            odm_read_and_config_mp_8812a_agc_tab();
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG_PG)
        {
            throw new NotImplementedException("odm_bb_config_type.CONFIG_BB_PHY_REG_PG");
            // READ_AND_CONFIG_MP(8812a, _phy_reg_pg);
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG_MP)
        {
            //READ_AND_CONFIG_MP(8812a, _phy_reg_mp);
            odm_read_and_config_mp_8812a_phy_reg_mp();
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB_DIFF)
        {
            throw new NotImplementedException("odm_bb_config_type.CONFIG_BB_AGC_TAB_DIFF");
            //dm.fw_offload_ability &= ~PHYDM_PHY_PARAM_OFFLOAD;
            ///*@AGC_TAB DIFF dont support FW offload*/
            //if ((dm.channel >= 36) && (dm.channel <= 64))
            //{
            //    AGC_DIFF_CONFIG_MP(8812a, lb);
            //}
            //else if (*dm.channel >= 100)
            //{
            //    AGC_DIFF_CONFIG_MP(8812a, hb);
            //}
        }

        // TODO:
        //if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG || config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB)
        //{
        //    if (dm.fw_offload_ability & PHYDM_PHY_PARAM_OFFLOAD)
        //    {
        //        result = phydm_set_reg_by_fw(dm, PHYDM_HALMAC_CMD_END, 0, 0, 0, (RfPath)0,0);
        //        PHYDM_DBG(dm, ODM_COMP_INIT, "phy param offload end!result = %d", result);
        //    }
        //}

        return result;
    }

    private void odm_read_and_config_mp_8812a_agc_tab()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        UInt32 array_len = (UInt32)array_mp_8812a_agc_tab.Length;
        UInt32[] array = array_mp_8812a_agc_tab;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_bb_agc_8812a(v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }

    private void odm_config_bb_agc_8812a(UInt32 addr, UInt32 bitmask, UInt32 data)
    {
        odm_set_bb_reg(addr, bitmask, data);
        /* Add 1us delay between BB/RF register setting. */
        ODM_delay_us(1);
    }

    private void odm_read_and_config_mp_8812a_phy_reg_mp()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        UInt32 array_len = (UInt32)array_mp_8812a_phy_reg_mp.Length;
        UInt32[] array = array_mp_8812a_phy_reg_mp;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_bb_phy_8812a(v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }

    private void odm_read_and_config_mp_8812a_phy_reg()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        int array_len = array_mp_8812a_phy_reg.Length;
        UInt32[] array = array_mp_8812a_phy_reg;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    _logger.LogDebug($"SEND_TO {v1:X4}");
                    odm_config_bb_phy_8812a(v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }


    private void odm_config_bb_phy_8812a(UInt32 addr, UInt32 bitmask, UInt32 data)
    {
        if (addr == 0xfe)
        {
            ODM_sleep_ms(50);
        }
        else if (addr == 0xfd)
        {
            ODM_delay_ms(5);
        }
        else if (addr == 0xfc)
        {
            ODM_delay_ms(1);
        }
        else if (addr == 0xfb)
        {
            ODM_delay_us(50);
        }
        else if (addr == 0xfa)
        {
            ODM_delay_us(5);
        }
        else if (addr == 0xf9)
        {
            ODM_delay_us(1);
        }
        else
        {
            odm_set_bb_reg(addr, bitmask, data);
            /* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }
    }

    private void odm_set_bb_reg(UInt32 reg_addr, UInt32 bit_mask, UInt32 data)
    {
        _device.phy_set_bb_reg((UInt16)reg_addr, bit_mask, data);
    }


    static void ODM_delay_ms(int ms)
    {
        Thread.Sleep(ms);
    }

    static void ODM_sleep_ms(int ms)
    {
        Thread.Sleep(ms);
    }

    static void ODM_delay_us(long us)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long v = (us * System.Diagnostics.Stopwatch.Frequency) / 1000000;
        while (sw.ElapsedTicks < v)
        {
        }
    }

    private void _InitBurstPktLen()
    {
        byte speedvalue, provalue, temp;

        _device.rtw_write8(0xf050, 0x01); /* usb3 rx interval */
        _device.rtw_write16(REG_RXDMA_STATUS, 0x7400); /* burset lenght=4, set 0x3400 for burset length=2 */
        _device.rtw_write8(0x289, 0xf5); /* for rxdma control */

        /* 0x456 = 0x70, sugguested by Zhilin */
        _device.rtw_write8(REG_AMPDU_MAX_TIME_8812, 0x70);

        _device.rtw_write32(REG_AMPDU_MAX_LENGTH_8812, 0xffffffff);
        _device.rtw_write8(REG_USTIME_TSF, 0x50);
        _device.rtw_write8(REG_USTIME_EDCA, 0x50);

        speedvalue = _device.rtw_read8(0xff); /* check device operation speed: SS 0xff bit7 */

        if ((speedvalue & BIT7) != 0)
        {
            /* USB2/1.1 Mode */
            temp = _device.rtw_read8(0xfe17);
            if (((temp >> 4) & 0x03) == 0)
            {
                provalue = _device.rtw_read8(REG_RXDMA_PRO_8812);
                _device.rtw_write8(REG_RXDMA_PRO_8812,
                    (byte)((provalue | BIT4 | BIT3 | BIT2 | BIT1) & (NotBIT5))); /* set burst pkt len=512B */
            }
            else
            {
                provalue = _device.rtw_read8(REG_RXDMA_PRO_8812);
                _device.rtw_write8(REG_RXDMA_PRO_8812,
                    (byte)((provalue | BIT5 | BIT3 | BIT2 | BIT1) & (NotBIT4))); /* set burst pkt len=64B */
            }
        }
        else
        {
            /* USB3 Mode */
            provalue = _device.rtw_read8(REG_RXDMA_PRO_8812);
            _device.rtw_write8(REG_RXDMA_PRO_8812,
                //((provalue | BIT3 | BIT2 | BIT1) & (~(BIT5 | BIT4)))); /* set burst pkt len=1k */
                (byte)((provalue | BIT3 | BIT2 | BIT1) & (0b1100_1111))); /* set burst pkt len=1k */

            _device.rtw_write8(0xf008, (byte)(_device.rtw_read8(0xf008) & 0xE7));
        }

        temp = _device.rtw_read8(REG_SYS_FUNC_EN);
        _device.rtw_write8(REG_SYS_FUNC_EN, (byte)(temp & (NotBIT10))); /* reset 8051 */

        _device.rtw_write8(REG_HT_SINGLE_AMPDU_8812,
            (byte)(_device.rtw_read8(REG_HT_SINGLE_AMPDU_8812) | BIT7)); /* enable single pkt ampdu */
        _device.rtw_write8(REG_RX_PKT_LIMIT, 0x18); /* for VHT packet length 11K */

        _device.rtw_write8(REG_PIFS, 0x00);

        _device.rtw_write16(REG_MAX_AGGR_NUM, 0x1f1f);
        _device.rtw_write8(REG_FWHW_TXQ_CTRL, (byte)(_device.rtw_read8(REG_FWHW_TXQ_CTRL) & (NotBIT7)));

        // AMPDUBurstMode is always false
        //if (pHalData.AMPDUBurstMode)
        //{
        //    adapterState.Device.rtw_write8(REG_AMPDU_BURST_MODE_8812, 0x5F);
        //}

        _device.rtw_write8(0x1c,
            (byte)(_device.rtw_read8(0x1c) | BIT5 | BIT6)); /* to prevent mac is reseted by bus. 20111208, by Page */

        /* ARFB table 9 for 11ac 5G 2SS */
        _device.rtw_write32(REG_ARFR0_8812, 0x00000010);
        _device.rtw_write32(REG_ARFR0_8812 + 4, 0xfffff000);

        /* ARFB table 10 for 11ac 5G 1SS */
        _device.rtw_write32(REG_ARFR1_8812, 0x00000010);
        _device.rtw_write32(REG_ARFR1_8812 + 4, 0x003ff000);

        /* ARFB table 11 for 11ac 24G 1SS */
        _device.rtw_write32(REG_ARFR2_8812, 0x00000015);
        _device.rtw_write32(REG_ARFR2_8812 + 4, 0x003ff000);
        /* ARFB table 12 for 11ac 24G 2SS */
        _device.rtw_write32(REG_ARFR3_8812, 0x00000015);
        _device.rtw_write32(REG_ARFR3_8812 + 4, 0xffcff000);
    }

    private void _InitBeaconMaxError_8812A()
    {
        _device.rtw_write8(REG_BCN_MAX_ERR, 0xFF);
    }

    private void _InitBeaconParameters_8812A()
    {
        var val8 = DIS_TSF_UDT;
        var val16 = (UInt16)(val8 | (val8 << 8)); /* port0 and port1 */

        _device.rtw_write16(REG_BCN_CTRL, val16);

        /* TBTT setup time */
        _device.rtw_write8(REG_TBTT_PROHIBIT, TBTT_PROHIBIT_SETUP_TIME);

        /* TBTT hold time: 0x540[19:8] */
        _device.rtw_write8(REG_TBTT_PROHIBIT + 1, TBTT_PROHIBIT_HOLD_TIME_STOP_BCN & 0xFF);
        _device.rtw_write8(REG_TBTT_PROHIBIT + 2,
            (byte)((_device.rtw_read8(REG_TBTT_PROHIBIT + 2) & 0xF0) | (TBTT_PROHIBIT_HOLD_TIME_STOP_BCN >> 8)));

        _device.rtw_write8(REG_DRVERLYINT, DRIVER_EARLY_INT_TIME_8812); /* 5ms */
        _device.rtw_write8(REG_BCNDMATIM, BCN_DMA_ATIME_INT_TIME_8812); /* 2ms */

        /* Suggested by designer timchen. Change beacon AIFS to the largest number */
        /* beacause test chip does not contension before sending beacon. by tynli. 2009.11.03 */
        _device.rtw_write16(REG_BCNTCFG, 0x4413);
    }

    private void init_UsbAggregationSetting_8812A()
    {
        ///* Tx aggregation setting */
        usb_AggSettingTxUpdate_8812A();

        ///* Rx aggregation setting */
        usb_AggSettingRxUpdate_8812A();
    }

    private void usb_AggSettingTxUpdate_8812A()
    {
        if (_usbTxAggMode)
        {
            UInt32 value32 = _device.rtw_read32(REG_TDECTRL);
            value32 = value32 & ~(BLK_DESC_NUM_MASK << BLK_DESC_NUM_SHIFT);
            value32 |= ((_usbTxAggDescNum & BLK_DESC_NUM_MASK) << BLK_DESC_NUM_SHIFT);

            _device.rtw_write32(REG_DWBCN0_CTRL_8812, value32);
            //if (IS_HARDWARE_TYPE_8821U(adapterState))   /* page added for Jaguar */
            //    rtw_write8(adapterState, REG_DWBCN1_CTRL_8812, pHalData._usbTxAggDescNum << 1);
        }
    }

    private void usb_AggSettingRxUpdate_8812A()
    {
        uint valueDMA = _device.rtw_read8(REG_TRXDMA_CTRL);
        switch (_rxAggMode)
        {
            case RX_AGG_MODE.RX_AGG_DMA:
                valueDMA |= RXDMA_AGG_EN;
                /* 2012/10/26 MH For TX through start rate temp fix. */
            {
                UInt16 temp;

                /* Adjust DMA page and thresh. */
                temp = (UInt16)(_rxAggDmaSize | (_rxAggDmaTimeout << 8));
                _device.rtw_write16(REG_RXDMA_AGG_PG_TH, temp);
                _device.rtw_write8(REG_RXDMA_AGG_PG_TH + 3,
                    (byte)BIT7); /* for dma agg , 0x280[31]GBIT_RXDMA_AGG_OLD_MOD, set 1 */
            }
                break;
            case RX_AGG_MODE.RX_AGG_USB:
                valueDMA |= RXDMA_AGG_EN;
            {
                UInt16 temp;

                /* Adjust DMA page and thresh. */
                temp = (UInt16)(_device.rxagg_usb_size | (_device.rxagg_usb_timeout << 8));
                _device.rtw_write16(REG_RXDMA_AGG_PG_TH, temp);
            }
                break;
            case RX_AGG_MODE.RX_AGG_MIX:
            case RX_AGG_MODE.RX_AGG_DISABLE:
            default:
                /* TODO: */
                break;
        }

        _device.rtw_write8(REG_TRXDMA_CTRL, (byte)valueDMA);
    }


    private void _InitRetryFunction_8812A()
    {
        uint value8;

        value8 = _device.rtw_read8(REG_FWHW_TXQ_CTRL);
        value8 |= EN_AMPDU_RTY_NEW;
        _device.rtw_write8(REG_FWHW_TXQ_CTRL, (byte)value8);

        /* Set ACK timeout */
        /* rtw_write8(adapterState, REG_ACKTO, 0x40);  */ /* masked by page for BCM IOT issue temporally */
        _device.rtw_write8(REG_ACKTO, 0x80);
    }

    private void _InitEDCA_8812AUsb()
    {
        /* Set Spec SIFS (used in NAV) */
        _device.rtw_write16(REG_SPEC_SIFS, 0x100a);
        _device.rtw_write16(REG_MAC_SPEC_SIFS, 0x100a);

        /* Set SIFS for CCK */
        _device.rtw_write16(REG_SIFS_CTX, 0x100a);

        /* Set SIFS for OFDM */
        _device.rtw_write16(REG_SIFS_TRX, 0x100a);

        /* TXOP */
        _device.rtw_write32(REG_EDCA_BE_PARAM, 0x005EA42B);
        _device.rtw_write32(REG_EDCA_BK_PARAM, 0x0000A44F);
        _device.rtw_write32(REG_EDCA_VI_PARAM, 0x005EA324);
        _device.rtw_write32(REG_EDCA_VO_PARAM, 0x002FA226);

        /* 0x50 for 80MHz clock */
        _device.rtw_write8(REG_USTIME_TSF, 0x50);
        _device.rtw_write8(REG_USTIME_EDCA, 0x50);
    }

    private void _InitAdaptiveCtrl_8812AUsb()
    {
        /* Response Rate Set */
        UInt32 value32 = _device.rtw_read32(REG_RRSR);
        value32 &= NotRATE_BITMAP_ALL;

        value32 |= RATE_RRSR_WITHOUT_CCK;
        value32 |= RATE_RRSR_CCK_ONLY_1M;
        _device.rtw_write32(REG_RRSR, value32);

        /* CF-END Threshold */
        /* m_spIoBase.rtw_write8(REG_CFEND_TH, 0x1); */

        /* SIFS (used in NAV) */
        UInt16 value16 = (UInt16)(_SPEC_SIFS_CCK(0x10) | _SPEC_SIFS_OFDM(0x10));
        _device.rtw_write16(REG_SPEC_SIFS, value16);

        /* Retry Limit */
        value16 = (UInt16)(BIT_LRL(RL_VAL_STA) | BIT_SRL(RL_VAL_STA));
        _device.rtw_write16(REG_RETRY_LIMIT, value16);
    }

    private void _InitWMACSetting_8812A()
    {
        /* rcr = AAP | APM | AM | AB | APP_ICV | ADF | AMF | APP_FCS | HTC_LOC_CTRL | APP_MIC | APP_PHYSTS; */
        UInt32 rcr = RCR_APM |
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

        _radioManagementModule.hw_var_rcr_config(rcr);

        /* Accept all multicast address */
        _device.rtw_write32(REG_MAR, 0xFFFFFFFF);
        _device.rtw_write32(REG_MAR + 4, 0xFFFFFFFF);

        uint value16 = BIT10 | BIT5;
        _device.rtw_write16(REG_RXFLTMAP1, (UInt16)value16);
    }

    private void _InitNetworkType_8812A()
    {
        var value32 = _device.rtw_read32(REG_CR);
        /* TODO: use the other function to set network type */
        value32 = (value32 & ~MASK_NETTYPE) | _NETTYPE(NT_LINK_AP);

        _device.rtw_write32(REG_CR, value32);
    }

    private void _InitInterrupt_8812AU()
    {
        /* HIMR */
        _device.rtw_write32(REG_HIMR0_8812, _intrMask[0] & 0xFFFFFFFF);
        _device.rtw_write32(REG_HIMR1_8812, _intrMask[1] & 0xFFFFFFFF);
    }

    private void _InitDriverInfoSize_8812A(byte drvInfoSize)
    {
        _device.rtw_write8(REG_RX_DRVINFO_SZ, drvInfoSize);
    }

    private void _InitTransferPageSize_8812AUsb()
    {
        byte value8 = _PSTX(PBP_512);
        _device.rtw_write8(REG_PBP, value8);
    }

    private void _InitPageBoundary_8812AUsb()
    {
        _device.rtw_write16((REG_TRXFF_BNDY + 2), RX_DMA_BOUNDARY_8812);
    }

    private void _InitQueueReservedPage_8812AUsb()
    {
        UInt32 numHQ = 0;
        UInt32 numLQ = 0;
        UInt32 numNQ = 0;
        UInt32 value32;
        byte value8;
        bool bWiFiConfig = registry_priv.wifi_spec;

        if (!bWiFiConfig)
        {
            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_HQ))
            {
                numHQ = NORMAL_PAGE_NUM_HPQ_8812;
            }

            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_LQ))
            {
                numLQ = NORMAL_PAGE_NUM_LPQ_8812;
            }

            /* NOTE: This step shall be proceed before writting REG_RQPN.		 */
            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_NQ))
            {
                numNQ = NORMAL_PAGE_NUM_NPQ_8812;
            }
        }
        else
        {
            /* WMM		 */
            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_HQ))
            {
                numHQ = WMM_NORMAL_PAGE_NUM_HPQ_8812;
            }

            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_LQ))
            {
                numLQ = WMM_NORMAL_PAGE_NUM_LPQ_8812;
            }

            /* NOTE: This step shall be proceed before writting REG_RQPN.		 */
            if (_device.OutEpQueueSel.HasFlag(TxSele.TX_SELE_NQ))
            {
                numNQ = WMM_NORMAL_PAGE_NUM_NPQ_8812;
            }
        }

        UInt32 numPubQ = TX_TOTAL_PAGE_NUMBER_8812 - numHQ - numLQ - numNQ;

        value8 = (byte)_NPQ(numNQ);
        _device.rtw_write8(REG_RQPN_NPQ, value8);

        /* TX DMA */
        value32 = _HPQ(numHQ) | _LPQ(numLQ) | _PUBQ(numPubQ) | LD_RQPN();
        _device.rtw_write32(REG_RQPN, value32);
    }


    private void PHY_MACConfig8812()
    {
        odm_read_and_config_mp_8812a_mac_reg();
    }

    private bool InitPowerOn()
    {
        if (_macPwrCtrlOn)
        {
            return true;
        }

        if (!HalPwrSeqCmdParsing(PowerSequences.Rtl8812_NIC_ENABLE_FLOW))
        {
            _logger.LogError("InitPowerOn: run power on flow fail");
            return false;
        }

        /* Enable MAC DMA/WMAC/SCHEDULE/SEC block */
        /* Set CR bit10 to enable 32k calibration. Suggested by SD1 Gimmy. Added by tynli. 2011.08.31. */
        _device.rtw_write16(REG_CR, 0x00); /* suggseted by zhouzhou, by page, 20111230 */
        UInt16 u2btmp = _device.rtw_read16(REG_CR);
        u2btmp |= (ushort)(
            CrBit.HCI_TXDMA_EN |
            CrBit.HCI_RXDMA_EN |
            CrBit.TXDMA_EN |
            CrBit.RXDMA_EN |
            CrBit.PROTOCOL_EN |
            CrBit.SCHEDULE_EN |
            CrBit.ENSEC |
            CrBit.CALTMR_EN);
        _device.rtw_write16(REG_CR, u2btmp);

        _macPwrCtrlOn = true;
        return true;
    }

    private bool HalPwrSeqCmdParsing(WlanPowerConfig[] PwrSeqCmd)
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
            switch (PwrCfgCmd.Command)
            {
                case PwrCmd.PWR_CMD_READ:
                    break;

                case PwrCmd.PWR_CMD_WRITE:
                {
                    var offset = PwrCfgCmd.Offset;
                    /* Read the value from system register */
                    var currentOffsetValue = _device.Read8(offset);

                    currentOffsetValue = (byte)(currentOffsetValue & unchecked((byte)(~PwrCfgCmd.Mask)));
                    currentOffsetValue = (byte)(currentOffsetValue | ((PwrCfgCmd.Value) & (PwrCfgCmd.Mask)));

                    /* Write the value back to sytem register */
                    _device.Write8(offset, currentOffsetValue);
                }
                    break;

                case PwrCmd.PWR_CMD_POLLING:

                {
                    var bPollingBit = false;
                    var offset = (PwrCfgCmd.Offset);
                    UInt32 maxPollingCnt = 5000;
                    bool flag = false;

                    maxPollingCnt = 5000;

                    do
                    {
                        var value = _device.Read8(offset);

                        value = (byte)(value & PwrCfgCmd.Mask);
                        if (value == ((PwrCfgCmd.Value) & PwrCfgCmd.Mask))
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

                                _device.Write8(0x04, (byte)(_device.Read8(0x04) | BIT3));
                                _device.Write8(0x04, (byte)(_device.Read8(0x04) & NotBIT3));

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
                    if (PwrCfgCmd.Value == (byte)PWRSEQ_DELAY_UNIT.PWRSEQ_DELAY_US)
                    {
                        Thread.Sleep((PwrCfgCmd.Offset));
                    }
                    else
                    {
                        Thread.Sleep((PwrCfgCmd.Offset) * 1000);
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

            AryIdx++; /* Add Array Index */
        } while (true);

        return true;
    }

    private bool InitLLTTable8812A(byte txpktbuf_bndy)
    {
        bool status;
        for (uint i = 0; i < (txpktbuf_bndy - 1); i++)
        {
            status = _LLTWrite_8812A(i, i + 1);
            if (true != status)
            {
                return false;
            }
        }

        /* end of list */
        status = _LLTWrite_8812A((uint)(txpktbuf_bndy - 1), 0xFF);
        if (status == false)
        {
            return false;
        }

        /* Make the other pages as ring buffer */
        /* This ring buffer is used as beacon buffer if we config this MAC as two MAC transfer. */
        /* Otherwise used as local loopback buffer. */
        UInt32 Last_Entry_Of_TxPktBuf = LAST_ENTRY_OF_TX_PKT_BUFFER_8812;
        for (uint i = txpktbuf_bndy; i < Last_Entry_Of_TxPktBuf; i++)
        {
            status = _LLTWrite_8812A(i, (i + 1));
            if (status == false)
            {
                return false;
            }
        }

        /* Let last entry point to the start entry of ring buffer */
        status = _LLTWrite_8812A(Last_Entry_Of_TxPktBuf, txpktbuf_bndy);
        if (status == false)
        {
            return false;
        }

        return true;
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

    private bool _LLTWrite_8812A(UInt32 address, UInt32 data)
    {
        bool status = true;
        Int32 count = 0;
        UInt32 value = _LLT_INIT_ADDR(address) | _LLT_INIT_DATA(data) | _LLT_OP(_LLT_WRITE_ACCESS);

        _device.rtw_write32(REG_LLT_INIT, value);

        /* polling */
        do
        {
            value = _device.rtw_read32(REG_LLT_INIT);
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

    private void _InitHardwareDropIncorrectBulkOut_8812A()
    {
        var DROP_DATA_EN = BIT9;
        UInt32 value32 = _device.rtw_read32(REG_TXDMA_OFFSET_CHK);
        value32 |= DROP_DATA_EN;
        _device.rtw_write32(REG_TXDMA_OFFSET_CHK, value32);
    }

    private void odm_read_and_config_mp_8812a_mac_reg()
    {
        UInt32 i = 0;
        byte c_cond;
        bool is_matched = true, is_skipped = false;
        var array_len = HalHwImg8812aMac.Mp8812aMacReg.Length;
        var array = HalHwImg8812aMac.Mp8812aMacReg;

        UInt32 pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            var v1 = array[i];
            var v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30)) != 0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31) != 0)
                {
                    /* positive condition*/
                    c_cond = (byte)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30) != 0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(pre_v1, pre_v2, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    ushort addr = (UInt16)v1;
                    byte data = (byte)v2;
                    odm_write_1byte(addr, data);
                }
            }

            i = i + 2;
        }
    }

    private void odm_write_1byte(UInt16 reg_addr, byte data)
    {
        _device.rtw_write8(reg_addr, data);
    }

    private void _InitTxBufferBoundary_8812AUsb()
    {
        byte txPageBoundary8812 = TX_PAGE_BOUNDARY_8812;

        _device.rtw_write8(REG_BCNQ_BDNY, txPageBoundary8812);
        _device.rtw_write8(REG_MGQ_BDNY, txPageBoundary8812);
        _device.rtw_write8(REG_WMAC_LBK_BF_HD, txPageBoundary8812);
        _device.rtw_write8(REG_TRXFF_BNDY, txPageBoundary8812);
        _device.rtw_write8(REG_TDECTRL + 1, txPageBoundary8812);
    }

    private void _InitQueuePriority_8812AUsb()
    {
        switch (_device.OutEpNumber)
        {
            case 2:
                _InitNormalChipTwoOutEpPriority_8812AUsb();
                break;
            case 3:
                _InitNormalChipThreeOutEpPriority_8812AUsb();
                break;
            case 4:
                _InitNormalChipFourOutEpPriority_8812AUsb();
                break;
            default:
                _logger.LogError("_InitQueuePriority_8812AUsb(): Shall not reach here!\n");
                break;
        }
    }

    private void _InitNormalChipTwoOutEpPriority_8812AUsb()
    {
        UInt16 valueHi;
        UInt16 valueLow;

        switch (_device.OutEpQueueSel)
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

        UInt16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;
        if (!registry_priv.wifi_spec)
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

        _InitNormalChipRegPriority_8812AUsb(beQ, bkQ, viQ, voQ, mgtQ, hiQ);
    }

    private void _InitNormalChipThreeOutEpPriority_8812AUsb()
    {
        UInt16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;

        if (!registry_priv.wifi_spec)
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

        _InitNormalChipRegPriority_8812AUsb(beQ, bkQ, viQ, voQ, mgtQ, hiQ);
    }

    private void _InitNormalChipFourOutEpPriority_8812AUsb()
    {
        UInt16 beQ, bkQ, viQ, voQ, mgtQ, hiQ;

        if (!registry_priv.wifi_spec)
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

        _InitNormalChipRegPriority_8812AUsb(beQ, bkQ, viQ, voQ, mgtQ, hiQ);
        init_hi_queue_config_8812a_usb();
    }

    private void _InitNormalChipRegPriority_8812AUsb(
        UInt16 beQ,
        UInt16 bkQ,
        UInt16 viQ,
        UInt16 voQ,
        UInt16 mgtQ,
        UInt16 hiQ
    )
    {
        UInt16 value16 = (UInt16)(_device.rtw_read16(REG_TRXDMA_CTRL) & 0x7);

        value16 = (UInt16)(value16 |
                        _TXDMA_BEQ_MAP(beQ) | _TXDMA_BKQ_MAP(bkQ) |
                        _TXDMA_VIQ_MAP(viQ) | _TXDMA_VOQ_MAP(voQ) |
                        _TXDMA_MGQ_MAP(mgtQ) | _TXDMA_HIQ_MAP(hiQ));

        _device.rtw_write16(REG_TRXDMA_CTRL, value16);
    }

    private void init_hi_queue_config_8812a_usb()
    {
        /* Packet in Hi Queue Tx immediately (No constraint for ATIM Period)*/
        _device.rtw_write8(REG_HIQ_NO_LMT_EN, 0xFF);
    }

}