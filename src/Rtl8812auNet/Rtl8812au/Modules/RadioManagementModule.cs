namespace Rtl8812auNet.Rtl8812au.Modules;

public class RadioManagementModule
{
    private readonly RtlUsbAdapter _device;
    private readonly RfPowerManagementModule _rfPowerManagement;

    public RadioManagementModule(
        RtlUsbAdapter device,
        RfPowerManagementModule rfPowerManagement)
    {
        _device = device;
        _rfPowerManagement = rfPowerManagement;
    }

    public void init_hw_mlme_ext(hal_com_data pHalData, InitChannel pmlmeext)
    {
        /* Modify to make sure first time change channel(band) would be done properly */
        pHalData.current_channel = 0;
        pHalData.current_channel_bw = ChannelWidth.CHANNEL_WIDTH_MAX;
        pHalData.current_band_type = BandType.BAND_MAX;

        /* set_opmode_cmd(padapter, infra_client_with_mlme); */ /* removed */
        Set_HW_VAR_ENABLE_RX_BAR(true);
        set_channel_bwmode(
            pHalData,
            pmlmeext.cur_channel,
            pmlmeext.cur_ch_offset,
            pmlmeext.cur_bwmode);
    }

    public void setopmode_hdl(HwPort HwPort)
    {
        SetMonitorMode(HwPort);
    }

    private void SetMonitorMode(HwPort HwPort)
    {
        rtw_hal_set_msr(HwPort, _HW_STATE_NOLINK_);
        hw_var_set_monitor();
    }

    private void rtw_hal_set_msr(HwPort HwPort, u8 net_type)
    {
        switch (HwPort)
        {
            case HwPort.HW_PORT0:
                /*REG_CR - BIT[17:16]-Network Type for port 0*/
                var val8 = (byte)(_device.rtw_read8(MSR) & 0x0C);
                val8 |= net_type;
                _device.rtw_write8(MSR, val8);
                break;
            //case HwPort.HW_PORT1:
            //    /*REG_CR - BIT[19:18]-Network Type for port 1*/
            //    val8 = rtw_read8(adapterState, MSR) & 0x03;
            //    val8 |= net_type << 2;
            //    rtw_write8(adapterState, MSR, val8);
            //    break;

            default:
                throw new NotImplementedException();
                break;
        }
    }

    void hw_var_set_monitor()
    {
        u32 rcr_bits;
        u16 value_rxfltmap2;

        /* Receive all type */
        rcr_bits = RCR_AAP | RCR_APM | RCR_AM | RCR_AB | RCR_APWRMGT | RCR_ADF | RCR_ACF | RCR_AMF | RCR_APP_PHYST_RXFF;

        /* Append FCS */
        rcr_bits |= RCR_APPFCS;

        //rtw_hal_get_hwreg(adapterState, HW_VAR_RCR, pHalData.rcr_backup);
        hw_var_rcr_config(rcr_bits);

        /* Receive all data frames */
        value_rxfltmap2 = 0xFFFF;
        _device.rtw_write16(REG_RXFLTMAP2, value_rxfltmap2);
    }

    public void hw_var_rcr_config(u32 rcr)
    {
        _device.rtw_write32(REG_RCR, rcr);
    }

    private void Set_HW_VAR_ENABLE_RX_BAR(bool val)
    {
        if (val)
        {
            /* enable RX BAR */
            u32 val16 = _device.rtw_read16(REG_RXFLTMAP1);

            val16 |= BIT8;
            _device.rtw_write16(REG_RXFLTMAP1, (u16)val16);
        }
        else
        {
            /* disable RX BAR */
            u32 val16 = _device.rtw_read16(REG_RXFLTMAP1);

            val16 &= NotBIT8;
            _device.rtw_write16(REG_RXFLTMAP1, (u16)val16);
        }
        RTW_INFO($"[HW_VAR_ENABLE_RX_BAR] 0x{REG_RXFLTMAP1:X4}=0x{_device.rtw_read16(REG_RXFLTMAP1):X4}");
    }


    public void set_channel_bwmode(hal_com_data pHalData, byte channel, byte channel_offset, ChannelWidth bwmode)
    {
        u8 center_ch, chnl_offset80 = HAL_PRIME_CHNL_OFFSET_DONT_CARE;

        //if (padapter.bNotifyChannelChange)
        //{
        //    RTW_INFO("[%s] ch = %d, offset = %d, bwmode = %d\n", __func__, channel, channel_offset, bwmode);
        //}

        center_ch = rtw_get_center_ch(channel, bwmode, channel_offset);
        if (bwmode == ChannelWidth.CHANNEL_WIDTH_80)
        {
            if (center_ch > channel)
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_LOWER;
            }
            else if (center_ch < channel)
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_UPPER;
            }
            else
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_DONT_CARE;
            }
        }

        rtw_hal_set_chnl_bw(pHalData, center_ch, bwmode, channel_offset, chnl_offset80); /* set center channel */
    }


    public void rtw_hal_set_chnl_bw(
        hal_com_data pHalData,
        u8 channel,
        ChannelWidth Bandwidth,
        u8 Offset40,
        u8 Offset80)
    {
        PHY_SetSwChnlBWMode8812(pHalData, channel, Bandwidth, Offset40, Offset80);
    }

    public void PHY_SetSwChnlBWMode8812(
        hal_com_data pHalData,
        u8 channel,
        ChannelWidth Bandwidth,
        u8 Offset40,
        u8 Offset80)
    {
        PHY_HandleSwChnlAndSetBW8812(pHalData, true, true, channel, Bandwidth, Offset40, Offset80, channel);
    }

    public void PHY_SwitchWirelessBand8812(hal_com_data pHalData, BandType Band)
    {
        ChannelWidth current_bw = pHalData.current_channel_bw;
        bool eLNA_2g = pHalData.ExternalLNA_2G;

        /* RTW_INFO("==>PHY_SwitchWirelessBand8812() %s\n", ((Band==0)?"2.4G":"5G")); */

        pHalData.current_band_type = (BandType)Band;

        if (Band == BandType.BAND_ON_2_4G)
        {
            /* 2.4G band */

            _device.phy_set_bb_reg(rOFDMCCKEN_Jaguar, bOFDMEN_Jaguar | bCCKEN_Jaguar, 0x03);

            /* <20131128, VincentL> Remove 0x830[3:1] setting when switching 2G/5G, requested by Yn. */
            _device.phy_set_bb_reg(rBWIndication_Jaguar, 0x3, 0x1); /* 0x834[1:0] = 0x1 */
            /* set PD_TH_20M for BB Yn user guide R27 */
            _device.phy_set_bb_reg(rPwed_TH_Jaguar, BIT13 | BIT14 | BIT15 | BIT16 | BIT17,
                0x17); /* 0x830[17:13]=5'b10111 */


            /* set PWED_TH for BB Yn user guide R29 */

            if (current_bw == ChannelWidth.CHANNEL_WIDTH_20
                && pHalData.rf_type == RfType.RF_1T1R
                && eLNA_2g == false)
            {
                /* 0x830[3:1]=3'b010 */
                _device.phy_set_bb_reg(rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x02);
            }
            else
            {
                /* 0x830[3:1]=3'b100 */
                _device.phy_set_bb_reg(rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x04);
            }


            /* AGC table select */
            _device.phy_set_bb_reg(rAGC_table_Jaguar, 0x3, 0); /* 0x82C[1:0] = 2b'00 */

            phy_SetRFEReg8812(pHalData, Band);

            /* <20131106, Kordan> Workaround to fix CCK FA for scan issue. */
            /* if( pHalData.bMPMode == FALSE) */

            _device.phy_set_bb_reg(rTxPath_Jaguar, 0xf0, 0x1);
            _device.phy_set_bb_reg(rCCK_RX_Jaguar, 0x0f000000, 0x1);

            /* CCK_CHECK_en */
            _device.rtw_write8(REG_CCK_CHECK_8812, (byte)(_device.rtw_read8(REG_CCK_CHECK_8812) & (NotBIT7)));
        }
        else
        {
            /* 5G band */
            u16 count = 0, reg41A = 0;


            /* CCK_CHECK_en */
            _device.rtw_write8(REG_CCK_CHECK_8812, (byte)(_device.rtw_read8(REG_CCK_CHECK_8812) | BIT7));

            count = 0;
            reg41A = _device.rtw_read16(REG_TXPKT_EMPTY);
            /* RTW_INFO("Reg41A value %d", reg41A); */
            reg41A &= 0x30;
            while ((reg41A != 0x30) && (count < 50))
            {
                Thread.Sleep(50);
                /* RTW_INFO("Delay 50us\n"); */

                reg41A = _device.rtw_read16(REG_TXPKT_EMPTY);
                reg41A &= 0x30;
                count++;
                /* RTW_INFO("Reg41A value %d", reg41A); */
            }

            if (count != 0)
                RTW_INFO("PHY_SwitchWirelessBand8812(): Switch to 5G Band. Count = %d reg41A=0x%x\n", count, reg41A);

            /* 2012/02/01, Sinda add registry to switch workaround without long-run verification for scan issue. */
            _device.phy_set_bb_reg(rOFDMCCKEN_Jaguar, bOFDMEN_Jaguar | bCCKEN_Jaguar, 0x03);

            /* <20131128, VincentL> Remove 0x830[3:1] setting when switching 2G/5G, requested by Yn. */
            _device.phy_set_bb_reg(rBWIndication_Jaguar, 0x3, 0x2); /* 0x834[1:0] = 0x2 */
            /* set PD_TH_20M for BB Yn user guide R27 */
            _device.phy_set_bb_reg(rPwed_TH_Jaguar, BIT13 | BIT14 | BIT15 | BIT16 | BIT17,
                0x15); /* 0x830[17:13]=5'b10101 */


            /* set PWED_TH for BB Yn user guide R29 */
            /* 0x830[3:1]=3'b100 */
            _device.phy_set_bb_reg(rPwed_TH_Jaguar, BIT1 | BIT2 | BIT3, 0x04);

            /* AGC table select */
            _device.phy_set_bb_reg(rAGC_table_Jaguar, 0x3, 1); /* 0x82C[1:0] = 2'b00 */

            phy_SetRFEReg8812(pHalData, Band);

            /* <20131106, Kordan> Workaround to fix CCK FA for scan issue. */
            /* if( pHalData.bMPMode == FALSE) */
            _device.phy_set_bb_reg(rTxPath_Jaguar, 0xf0, 0x0);
            _device.phy_set_bb_reg(rCCK_RX_Jaguar, 0x0f000000, 0xF);
        }

        phy_SetBBSwingByBand_8812A(pHalData, Band);
    }

    private static u8 rtw_get_center_ch(u8 channel, ChannelWidth chnl_bw, u8 chnl_offset)
    {
        u8 center_ch = channel;

        if (chnl_bw == ChannelWidth.CHANNEL_WIDTH_80)
        {
            if (channel == 36 || channel == 40 || channel == 44 || channel == 48)
                center_ch = 42;
            else if (channel == 52 || channel == 56 || channel == 60 || channel == 64)
                center_ch = 58;
            else if (channel == 100 || channel == 104 || channel == 108 || channel == 112)
                center_ch = 106;
            else if (channel == 116 || channel == 120 || channel == 124 || channel == 128)
                center_ch = 122;
            else if (channel == 132 || channel == 136 || channel == 140 || channel == 144)
                center_ch = 138;
            else if (channel == 149 || channel == 153 || channel == 157 || channel == 161)
                center_ch = 155;
            else if (channel == 165 || channel == 169 || channel == 173 || channel == 177)
                center_ch = 171;
            else if (channel <= 14)
                center_ch = 7;
        }
        else if (chnl_bw == ChannelWidth.CHANNEL_WIDTH_40)
        {
            if (chnl_offset == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                center_ch = (byte)(channel + 2);
            }
            else
            {
                center_ch = (byte)(channel - 2);
            }
        }
        else if (chnl_bw == ChannelWidth.CHANNEL_WIDTH_20)
            center_ch = channel;
        else
        {
            throw new Exception();
        }

        return center_ch;
    }


    private void phy_SetBBSwingByBand_8812A(hal_com_data pHalData, BandType Band)
    {
        _device.phy_set_bb_reg(rA_TxScale_Jaguar, 0xFFE00000, phy_get_tx_bb_swing_8812a(pHalData, (BandType)Band, RfPath.RF_PATH_A)); /* 0xC1C[31:21] */
        _device.phy_set_bb_reg(rB_TxScale_Jaguar, 0xFFE00000, phy_get_tx_bb_swing_8812a(pHalData, (BandType)Band, RfPath.RF_PATH_B)); /* 0xE1C[31:21] */
    }

    private u32 phy_get_tx_bb_swing_8812a(hal_com_data pHalData, BandType Band, RfPath RFPath)
    {
        s8 bbSwing_2G = (s8)(-1 * registry_priv.TxBBSwing_2G);
        s8 bbSwing_5G = (s8)(-1 * registry_priv.TxBBSwing_5G);
        u32 _out = 0x200;
        const s8 AUTO = -1;

        if (pHalData.AutoloadFailFlag)
        {
            if (Band == BandType.BAND_ON_2_4G)
            {
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
                        _out = 0x16A;
                    }
                    else
                    {
                        _out = 0x200;
                    }
                }
            }
            else if (Band == BandType.BAND_ON_5G)
            {
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
                    _out = 0x200;
                }
            }
            else
            {
                _out = 0x16A; /* -3 dB */
            }
        }
        else
        {
            byte swing = 0;
            byte onePathSwing = 0;

            if (Band == BandType.BAND_ON_2_4G)
            {
                if (registry_priv.TxBBSwing_2G == AUTO)
                {
                    efuse_ShadowRead1Byte(pHalData, EEPROM_TX_BBSWING_2G_8812, out swing);
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
                if (registry_priv.TxBBSwing_5G == AUTO)
                {
                    efuse_ShadowRead1Byte(pHalData, EEPROM_TX_BBSWING_5G_8812, out swing);
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

            if (RFPath == RfPath.RF_PATH_A)
                onePathSwing = (byte)((swing & 0x3) >> 0); /* 0xC6/C7[1:0] */
            else if (RFPath == RfPath.RF_PATH_B)
                onePathSwing = (byte)((swing & 0xC) >> 2); /* 0xC6/C7[3:2] */

            if (onePathSwing == 0x0)
            {
                _out = 0x200; /* 0 dB */
            }
            else if (onePathSwing == 0x1)
            {
                _out = 0x16A; /* -3 dB */
            }
            else if (onePathSwing == 0x2)
            {
                _out = 0x101; /* -6 dB */
            }
            else if (onePathSwing == 0x3)
            {
                _out = 0x0B6; /* -9 dB */
            }
        }

        /* RTW_INFO("<=== phy_get_tx_bb_swing_8812a, out = 0x%X\n", out); */

        return _out;
    }

    private static void efuse_ShadowRead1Byte(
        hal_com_data pHalData,
        u16 Offset,
        out u8 Value)
    {
        Value = pHalData.efuse_eeprom_data[Offset];
    }

    private void PHY_HandleSwChnlAndSetBW8812(
        hal_com_data pHalData,
        BOOLEAN bSwitchChannel,
        BOOLEAN bSetBandWidth,
        u8 ChannelNum,
        ChannelWidth ChnlWidth,
        u8 ChnlOffsetOf40MHz,
        u8 ChnlOffsetOf80MHz,
        u8 CenterFrequencyIndex1
    )
    {
        /* RTW_INFO("=> PHY_HandleSwChnlAndSetBW8812: bSwitchChannel %d, bSetBandWidth %d\n",bSwitchChannel,bSetBandWidth); */

        /* check is swchnl or setbw */
        if (!bSwitchChannel && !bSetBandWidth)
        {
            RTW_INFO("PHY_HandleSwChnlAndSetBW8812:  not switch channel and not set bandwidth\n");
            return;
        }

        /* skip change for channel or bandwidth is the same */
        if (bSwitchChannel)
        {
            if (pHalData.current_channel != ChannelNum)
            {
                pHalData.bSwChnl = true;
            }
        }

        if (bSetBandWidth)
        {
            if (pHalData.bChnlBWInitialized == false)
            {
                pHalData.bChnlBWInitialized = true;
                pHalData.bSetChnlBW = true;
            }
            else if ((pHalData.current_channel_bw != ChnlWidth) ||
                     (pHalData.nCur40MhzPrimeSC != ChnlOffsetOf40MHz) ||
                     (pHalData.nCur80MhzPrimeSC != ChnlOffsetOf80MHz) ||
                     (pHalData.CurrentCenterFrequencyIndex1 != CenterFrequencyIndex1))
            {
                pHalData.bSetChnlBW = true;
            }
        }

        if (!pHalData.bSetChnlBW && !pHalData.bSwChnl && pHalData.bNeedIQK != true)
        {
            /* RTW_INFO("<= PHY_HandleSwChnlAndSetBW8812: bSwChnl %d, bSetChnlBW %d\n",pHalData.bSwChnl,pHalData.bSetChnlBW); */
            return;
        }


        if (pHalData.bSwChnl)
        {
            pHalData.current_channel = ChannelNum;
            pHalData.CurrentCenterFrequencyIndex1 = ChannelNum;
        }


        if (pHalData.bSetChnlBW)
        {
            pHalData.current_channel_bw = ChnlWidth;

            pHalData.nCur40MhzPrimeSC = ChnlOffsetOf40MHz;
            pHalData.nCur80MhzPrimeSC = ChnlOffsetOf80MHz;


            pHalData.CurrentCenterFrequencyIndex1 = CenterFrequencyIndex1;
        }

        /* Switch workitem or set timer to do switch channel or setbandwidth operation */
        phy_SwChnlAndSetBwMode8812(pHalData);
    }

    private void phy_SwChnlAndSetBwMode8812(hal_com_data pHalData)
    {
        if (pHalData.bSwChnl)
        {
            phy_SwChnl8812(pHalData);
            pHalData.bSwChnl = false;
        }

        if (pHalData.bSetChnlBW)
        {
            phy_PostSetBwMode8812(pHalData);
            pHalData.bSetChnlBW = false;
        }

        _rfPowerManagement.PHY_SetTxPowerLevel8812(pHalData, pHalData.current_channel);

        pHalData.bNeedIQK = false;
    }

    private void phy_SwChnl8812(hal_com_data pHalData)
    {
        u8 channelToSW = pHalData.current_channel;

        if (phy_SwBand8812(pHalData, channelToSW) == false)
        {
            RTW_INFO("error Chnl %d !", channelToSW);
        }

        /* RTW_INFO("[BW:CHNL], phy_SwChnl8812(), switch to channel %d !!\n", channelToSW); */

        /* fc_area		 */
        if (36 <= channelToSW && channelToSW <= 48)
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (15 <= channelToSW && channelToSW <= 35)
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (50 <= channelToSW && channelToSW <= 80)
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x453);
        }
        else if (82 <= channelToSW && channelToSW <= 116)
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x452);
        }
        else if (118 <= channelToSW)
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x412);
        }
        else
        {
            _device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x96a);
        }

        for (RfPath eRFPath = 0; (byte)eRFPath < pHalData.NumTotalRFPath; eRFPath++)
        {
            /* RF_MOD_AG */
            if (36 <= channelToSW && channelToSW <= 80)
            {
                phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (15 <= channelToSW && channelToSW <= 35)
            {
                phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (82 <= channelToSW && channelToSW <= 140)
            {
                phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x301); /* 5'b01101); */
            }
            else if (140 < channelToSW)
            {
                phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x501); /* 5'b10101); */
            }
            else
            {
                phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x000); /* 5'b00000); */
            }

            /* <20121109, Kordan> A workaround for 8812A only. */
            phy_FixSpur_8812A(pHalData, pHalData.current_channel_bw, channelToSW);
            phy_set_rf_reg(pHalData, eRFPath, RF_CHNLBW_Jaguar, bMaskByte0, channelToSW);
        }
    }

    private BOOLEAN phy_SwBand8812(hal_com_data pHalData, u8 channelToSW)
    {
        u8 u1Btmp;
        BOOLEAN ret_value = true;
        BandType Band;
        BandType BandToSW;

        u1Btmp = _device.rtw_read8(REG_CCK_CHECK_8812);
        if ((u1Btmp & BIT7) != 0)
        {
            Band = BandType.BAND_ON_5G;
        }
        else
        {
            Band = BandType.BAND_ON_2_4G;
        }

        /* Use current channel to judge Band Type and switch Band if need. */
        if (channelToSW > 14)
        {
            BandToSW = BandType.BAND_ON_5G;
        }
        else
            BandToSW = BandType.BAND_ON_2_4G;

        if (BandToSW != Band)
        {
            PHY_SwitchWirelessBand8812(pHalData, BandToSW);
        }

        return ret_value;
    }

    private void phy_FixSpur_8812A(hal_com_data pHalData, ChannelWidth Bandwidth, u8 Channel)
    {
        /* C cut Item12 ADC FIFO CLOCK */
        if (IS_VENDOR_8812A_C_CUT(pHalData))
        {
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_40 && Channel == 11)
            {
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0xC00, 0x3); /* 0x8AC[11:10] = 2'b11 */
            }
            else
            {
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0xC00, 0x2); /* 0x8AC[11:10] = 2'b10 */
            }

            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
            {

                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 2'b11 */
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */

            }
            else if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_40 &&
                     Channel == 11)
            {
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */
            }
            else if (Bandwidth != ChannelWidth.CHANNEL_WIDTH_80)
            {
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 2'b10	 */
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8C4[30] = 0 */
            }
        }
        else
        {
            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
            {
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 11 */
            }
            else if (Channel <= 14) /* 2.4G only */
            {
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 10 */
            }
        }

    }

    private void phy_SetRFEReg8812(hal_com_data pHalData, BandType Band)
    {
        uint u1tmp = 0;

        if (Band == BandType.BAND_ON_2_4G)
        {
            switch (pHalData.rfe_type)
            {
                case 0:
                case 2:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    break;
                case 1:
                {
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                }
                    break;
                case 3:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337770);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337770);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(r_ANTSEL_SW_Jaguar, 0x00000303, 0x1);
                    break;
                case 4:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x001);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x001);
                    break;
                case 5:
                    _device.rtw_write8(rA_RFE_Pinmux_Jaguar + 2, 0x77);

                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77777777);
                    u1tmp = _device.rtw_read8(rA_RFE_Inv_Jaguar + 3);
                    u1tmp &= NotBIT0;
                    _device.rtw_write8(rA_RFE_Inv_Jaguar + 3, (byte)(u1tmp));
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    break;
                case 6:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x07772770);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x07772770);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
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
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 1:
                {
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337717);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x000);
                }
                    break;
                case 2:
                case 4:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 3:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337717);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x54337717);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    _device.phy_set_bb_reg(r_ANTSEL_SW_Jaguar, 0x00000303, 0x1);
                    break;
                case 5:
                    _device.rtw_write8(rA_RFE_Pinmux_Jaguar + 2, 0x33);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x77337777);
                    u1tmp = _device.rtw_read8(rA_RFE_Inv_Jaguar + 3);
                    _device.rtw_write8(rA_RFE_Inv_Jaguar + 3, (byte)(u1tmp |= BIT0));
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMask_RFEInv_Jaguar, 0x010);
                    break;
                case 6:
                    _device.phy_set_bb_reg(rA_RFE_Pinmux_Jaguar, bMaskDWord, 0x07737717);
                    _device.phy_set_bb_reg(rB_RFE_Pinmux_Jaguar, bMaskDWord, 0x07737717);
                    _device.phy_set_bb_reg(rA_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    _device.phy_set_bb_reg(rB_RFE_Inv_Jaguar, bMaskDWord, 0x00000077);
                    break;
                default:
                    break;
            }
        }
    }

    void phy_PostSetBwMode8812(hal_com_data pHalData)
    {
        u8 L1pkVal = 0, reg_837 = 0;


        /* 3 Set Reg668 BW */
        phy_SetRegBW_8812(pHalData.current_channel_bw);

        /* 3 Set Reg483 */
        var SubChnlNum = phy_GetSecondaryChnl_8812(pHalData);
        _device.rtw_write8(REG_DATA_SC_8812, SubChnlNum);

        reg_837 = _device.rtw_read8(rBWIndication_Jaguar + 3);
        /* 3 Set Reg848 Reg864 Reg8AC Reg8C4 RegA00 */
        switch (pHalData.current_channel_bw)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300200); /* 0x8ac[21,20,9:6,1,0]=8'b11100000 */
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */

                if (pHalData.rf_type == RfType.RF_2T2R)
                {
                    _device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, 7); /* 2R 0x848[25:22] = 0x7 */
                }
                else
                {
                    _device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, 8); /* 1R 0x848[25:22] = 0x8 */
                }

                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300201); /* 0x8ac[21,20,9:6,1,0]=8'b11100000		 */
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x3C, SubChnlNum);
                _device.phy_set_bb_reg(rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 6;
                else
                {
                    if (pHalData.rf_type == RfType.RF_2T2R)
                        L1pkVal = 7;
                    else
                        L1pkVal = 8;
                }

                _device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x6 */

                if (SubChnlNum == (byte)VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ)
                {
                    _device.phy_set_bb_reg(rCCK_System_Jaguar, bCCK_System_Jaguar, 1);
                }
                else
                {
                    _device.phy_set_bb_reg(rCCK_System_Jaguar, bCCK_System_Jaguar, 0);
                }

                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300202); /* 0x8ac[21,20,9:6,1,0]=8'b11100010 */
                _device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8c4[30] = 1 */
                _device.phy_set_bb_reg(rRFMOD_Jaguar, 0x3C, SubChnlNum);
                _device.phy_set_bb_reg(rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 5;
                else
                {
                    if (pHalData.rf_type == RfType.RF_2T2R)
                    {
                        L1pkVal = 6;
                    }
                    else
                    {
                        L1pkVal = 7;
                    }
                }

                _device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x5 */

                break;

            default:
                RTW_INFO("phy_PostSetBWMode8812():	unknown Bandwidth: %#X\n", pHalData.current_channel_bw);
                break;
        }

        /* <20121109, Kordan> A workaround for 8812A only. */
        phy_FixSpur_8812A(pHalData, pHalData.current_channel_bw, pHalData.current_channel);

        /* RTW_INFO("phy_PostSetBwMode8812(): Reg483: %x\n", rtw_read8(adapterState, 0x483)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg668: %x\n", rtw_read32(adapterState, 0x668)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg8AC: %x\n", phy_query_bb_reg(adapterState, rRFMOD_Jaguar, 0xffffffff)); */

        /* 3 Set RF related register */
        PHY_RF6052SetBandwidth8812(pHalData, pHalData.current_channel_bw);
    }

    private void PHY_RF6052SetBandwidth8812(hal_com_data pHalData, ChannelWidth Bandwidth) /* 20M or 40M */
    {
        switch (Bandwidth)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 20MHz\n"); */
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 40MHz\n"); */
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 80MHz\n"); */
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                phy_set_rf_reg(pHalData, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                break;

            default:
                RTW_INFO("PHY_RF6052SetBandwidth8812(): unknown Bandwidth: %#X\n", Bandwidth);
                break;
        }
    }

    public void phy_set_rf_reg(hal_com_data pHalData, RfPath eRFPath, u16 RegAddr, u32 BitMask, u32 Data)
    {
        uint data = Data;
        Console.WriteLine($"RFREG;{(byte)eRFPath};{(uint)RegAddr:X};{BitMask:X};{data:X}");
        if (BitMask == 0)
        {
            return;
        }

        /* RF data is 20 bits only */
        if (BitMask != bLSSIWrite_data_Jaguar)
        {
            u32 Original_Value, BitShift;
            Original_Value = phy_RFSerialRead(pHalData, eRFPath, RegAddr);
            BitShift = PHY_CalculateBitShift(BitMask);
            data = ((Original_Value) & (~BitMask)) | (data << (int)BitShift);
        }

        phy_RFSerialWrite(pHalData, eRFPath, RegAddr, data);
    }

    private u32 phy_RFSerialRead(hal_com_data pHalData, RfPath eRFPath, u32 Offset)
    {
        u32 retValue;
        BbRegisterDefinition pPhyReg = pHalData.PHYRegDef[eRFPath];
        BOOLEAN bIsPIMode = false;

        /* <20120809, Kordan> CCA OFF(when entering), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(pHalData)))
        {
            _device.phy_set_bb_reg(rCCAonSec_Jaguar, 0x8, 1);
        }

        Offset &= 0xff;

        if (eRFPath == RfPath.RF_PATH_A)
        {
            bIsPIMode = phy_query_bb_reg(0xC00, 0x4) != 0;
        }
        else if (eRFPath == RfPath.RF_PATH_B)
        {
            bIsPIMode = phy_query_bb_reg(0xE00, 0x4) != 0;
        }

        _device.phy_set_bb_reg((ushort)pPhyReg.RfHSSIPara2, bHSSIRead_addr_Jaguar, Offset);

        if (IS_VENDOR_8812A_C_CUT(pHalData))
        {
            Thread.Sleep(20);
        }

        if (bIsPIMode)
        {
            retValue = phy_query_bb_reg(pPhyReg.RfLSSIReadBackPi, rRead_data_Jaguar);
            /* RTW_INFO("[PI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.rfLSSIReadBackPi, retValue); */
        }
        else
        {
            retValue = phy_query_bb_reg(pPhyReg.RfLSSIReadBack, rRead_data_Jaguar);
            /* RTW_INFO("[SI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.RfLSSIReadBack, retValue); */
        }

        /* <20120809, Kordan> CCA ON(when exiting), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(pHalData)))
        {
            _device.phy_set_bb_reg(rCCAonSec_Jaguar, 0x8, 0);
        }

        return retValue;
    }

    private u32 phy_query_bb_reg(u16 regAddr, u32 bitMask) =>
        PHY_QueryBBReg8812(regAddr, bitMask);

    private u32 PHY_QueryBBReg8812(u16 regAddr, u32 bitMask)
    {
        u32 ReturnValue, OriginalValue, BitShift;

        /* RTW_INFO("--.PHY_QueryBBReg8812(): RegAddr(%#x), BitMask(%#x)\n", RegAddr, BitMask); */

        OriginalValue = _device.rtw_read32(regAddr);
        BitShift = PHY_CalculateBitShift(bitMask);
        ReturnValue = (OriginalValue & bitMask) >> (int)BitShift;

        /* RTW_INFO("BBR MASK=0x%x Addr[0x%x]=0x%x\n", BitMask, RegAddr, OriginalValue); */
        return ReturnValue;
    }

    private void phy_RFSerialWrite(hal_com_data pHalData, RfPath eRFPath, u32 Offset, u32 Data)
    {
        BbRegisterDefinition pPhyReg = pHalData.PHYRegDef[eRFPath];

        Offset &= 0xff;
        /* Shadow Update */
        /* PHY_RFShadowWrite(adapterState, eRFPath, Offset, Data); */
        /* Put write addr in [27:20]  and write data in [19:00] */
        var dataAndAddr = ((Offset << 20) | (Data & 0x000fffff)) & 0x0fffffff;

        /* Write Operation */
        /* TODO: Dynamically determine whether using PI or SI to write RF registers. */
        _device.phy_set_bb_reg((ushort)pPhyReg.Rf3WireOffset, bMaskDWord, dataAndAddr);
        /* RTW_INFO("RFW-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.Rf3WireOffset, DataAndAddr); */
    }

    private void phy_SetRegBW_8812(ChannelWidth CurrentBW)
    {
        u16 RegRfMod_BW, u2tmp;
        RegRfMod_BW = _device.rtw_read16(REG_WMAC_TRXPTCL_CTL);

        switch (CurrentBW)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                _device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(RegRfMod_BW & 0xFE7F)); /* BIT 7 = 0, BIT 8 = 0 */
                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                u2tmp = (ushort)(RegRfMod_BW | BIT7);
                _device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFEFF)); /* BIT 7 = 1, BIT 8 = 0 */
                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                u2tmp = (ushort)(RegRfMod_BW | BIT8);
                _device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFF7F)); /* BIT 7 = 0, BIT 8 = 1 */
                break;

            default:
                RTW_INFO($"phy_PostSetBWMode8812():	unknown Bandwidth: {CurrentBW}");
                break;
        }
    }

    private byte phy_GetSecondaryChnl_8812(hal_com_data pHalData)
    {
        VHT_DATA_SC SCSettingOf40 = 0, SCSettingOf20 = 0;

        /* RTW_INFO("SCMapping: Case: pHalData.current_channel_bw %d, pHalData.nCur80MhzPrimeSC %d, pHalData.nCur40MhzPrimeSC %d\n",pHalData.current_channel_bw,pHalData.nCur80MhzPrimeSC,pHalData.nCur40MhzPrimeSC); */
        if (pHalData.current_channel_bw == ChannelWidth.CHANNEL_WIDTH_80)
        {
            if (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                SCSettingOf40 = VHT_DATA_SC.VHT_DATA_SC_40_LOWER_OF_80MHZ;
            }
            else if (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER)
            {
                SCSettingOf40 = VHT_DATA_SC.VHT_DATA_SC_40_UPPER_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }

            if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER) &&
                (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWEST_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWER_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPERST_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }
        }
        else if (pHalData.current_channel_bw == ChannelWidth.CHANNEL_WIDTH_40)
        {
            /* RTW_INFO("SCMapping: Case: pHalData.current_channel_bw %d, pHalData.nCur40MhzPrimeSC %d\n",pHalData.current_channel_bw,pHalData.nCur40MhzPrimeSC); */

            if (pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER)
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ;
            }
            else if (pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWER_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }
        }

        /*RTW_INFO("SCMapping: SC Value %x\n", ((SCSettingOf40 << 4) | SCSettingOf20));*/
        return (byte)(((byte)SCSettingOf40 << 4) | (byte)SCSettingOf20);
    }
}