namespace WiFiDriver.App.Rtl8812au;

public static class rtl8812a_phycfg
{
    public static void PHY_SetSwChnlBWMode8812(
        PADAPTER Adapter,
        u8 channel,
        channel_width Bandwidth, u8 Offset40,
        u8 Offset80)
    {
        /* RTW_INFO("%s()===>\n",__FUNCTION__); */

        PHY_HandleSwChnlAndSetBW8812(Adapter, true, true, channel, Bandwidth, Offset40, Offset80, channel);

        /* RTW_INFO("<==%s()\n",__FUNCTION__); */
    }

    static void PHY_HandleSwChnlAndSetBW8812(
        PADAPTER Adapter,
        BOOLEAN bSwitchChannel,
        BOOLEAN bSetBandWidth,
        u8 ChannelNum,
        channel_width ChnlWidth,
        u8 ChnlOffsetOf40MHz,
        u8 ChnlOffsetOf80MHz,
        u8 CenterFrequencyIndex1
    )
    {
        PADAPTER pDefAdapter = Adapter;
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pDefAdapter);
        u8 tmpChannel = pHalData.current_channel;
        channel_width tmpBW = pHalData.current_channel_bw;
        u8 tmpnCur40MhzPrimeSC = pHalData.nCur40MhzPrimeSC;
        u8 tmpnCur80MhzPrimeSC = pHalData.nCur80MhzPrimeSC;
        u8 tmpCenterFrequencyIndex1 = pHalData.CurrentCenterFrequencyIndex1;

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
        phy_SwChnlAndSetBwMode8812(Adapter);
    }

    static void phy_SwChnlAndSetBwMode8812(PADAPTER Adapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* RTW_INFO("phy_SwChnlAndSetBwMode8812(): bSwChnl %d, bSetChnlBW %d\n", pHalData.bSwChnl, pHalData.bSetChnlBW); */
        //if (Adapter.bNotifyChannelChange) {
        //    RTW_INFO("[%s] bSwChnl=%d, ch=%d, bSetChnlBW=%d, bw=%d\n",
        //        __FUNCTION__,
        //        pHalData.bSwChnl,
        //        pHalData.current_channel,
        //        pHalData.bSetChnlBW,
        //        pHalData.current_channel_bw);
        //}

        //if (RTW_CANNOT_RUN(Adapter))
        //    return;


        if (pHalData.bSwChnl)
        {
            phy_SwChnl8812(Adapter);
            pHalData.bSwChnl = false;
        }

        if (pHalData.bSetChnlBW)
        {
            phy_PostSetBwMode8812(Adapter);
            pHalData.bSetChnlBW = false;
        }

        odm_clear_txpowertracking_state(pHalData.odmpriv);
        PHY_SetTxPowerLevel8812(Adapter, pHalData.current_channel);

        phy_InitRssiTRSW(Adapter);

        if ((pHalData.bNeedIQK == true))
        {

            /*phy_iq_calibrate_8812a(Adapter, _FALSE);*/
            halrf_iqk_trigger(pHalData.odmpriv, false);
        }

        pHalData.bNeedIQK = false;
    }

    /* <20130207, Kordan> The variales initialized here are used in odm_LNAPowerControl(). */
    private static void phy_InitRssiTRSW(PADAPTER pAdapter)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);

        var pDM_Odm = pHalData.odmpriv;
        u8 channel = pHalData.current_channel;

        if (pHalData.rfe_type == 3)
        {

            if (channel <= 14)
            {
                pDM_Odm.rssi_trsw_h = 70; /* Unit: percentage(%) */
                pDM_Odm.rssi_trsw_iso = 25;
            }
            else
            {
                pDM_Odm.rssi_trsw_h = 80;
                pDM_Odm.rssi_trsw_iso = 25;
            }

            pDM_Odm.rssi_trsw_l = pDM_Odm.rssi_trsw_h - pDM_Odm.rssi_trsw_iso - 10;
        }
    }

    static void PHY_SetTxPowerLevel8812(PADAPTER Adapter, u8 Channel)
    {

        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        u8 path = 0;

        /* RTW_INFO("==>PHY_SetTxPowerLevel8812()\n"); */

        for (path = (u8)rf_path.RF_PATH_A; path < pHalData.NumTotalRFPath; ++path)
        {
            phy_set_tx_power_level_by_path(Adapter, Channel, path);
            PHY_TxPowerTrainingByPath_8812(Adapter, pHalData.current_channel_bw, Channel, (rf_path)path);
        }

        /* RTW_INFO("<==PHY_SetTxPowerLevel8812()\n"); */
    }

    static void PHY_TxPowerTrainingByPath_8812(PADAPTER Adapter, channel_width BandWidth, u8 Channel, rf_path RfPath)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        u8 i;
        u32 PowerLevel, writeData, writeOffset;

        if ((u8)RfPath >= pHalData.NumTotalRFPath)
        {
            return;
        }

        writeData = 0;
        if (RfPath == rf_path.RF_PATH_A)
        {
            PowerLevel = phy_get_tx_power_index(Adapter, rf_path.RF_PATH_A, MGN_RATE.MGN_MCS7, BandWidth, Channel);
            writeOffset = rA_TxPwrTraing_Jaguar;
        }
        else
        {
            PowerLevel = phy_get_tx_power_index(Adapter, rf_path.RF_PATH_B, MGN_RATE.MGN_MCS7, BandWidth, Channel);
            writeOffset = rB_TxPwrTraing_Jaguar;
        }

        for (i = 0; i < 3; i++)
        {
            if (i == 0)
                PowerLevel = PowerLevel - 10;
            else if (i == 1)
                PowerLevel = PowerLevel - 8;
            else
                PowerLevel = PowerLevel - 6;
            writeData |= (((PowerLevel > 2) ? (PowerLevel) : 2) << (i * 8));
        }

        phy_set_bb_reg(Adapter, writeOffset, 0xffffff, writeData);
    }

    static void phy_set_tx_power_level_by_path(PADAPTER Adapter, u8 channel, u8 path)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        BOOLEAN bIsIn24G = (pHalData.current_band_type == BAND_TYPE.BAND_ON_2_4G);
        bool under_survey_ch = phy_check_under_survey_ch(Adapter);


        /* if ( pMgntInfo->RegNByteAccess == 0 ) */
        {
            if (bIsIn24G)
            {
                phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.CCK);
            }

            phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.OFDM);

            if (!under_survey_ch)
            {
                phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.HT_MCS0_MCS7);
                phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.VHT_1SSMCS0_1SSMCS9);

                if (pHalData.NumTotalRFPath >= 2)
                {
                    phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.HT_MCS8_MCS15);
                    phy_set_tx_power_index_by_rate_section(Adapter, path, channel, RATE_SECTION.VHT_2SSMCS0_2SSMCS9);
                }
            }
        }
    }

    static void phy_SwChnl8812(PADAPTER pAdapter)
    {

        HAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        u8 channelToSW = pHalData.current_channel;
        var bandwidthToSw = pHalData.current_channel_bw;

        if (phy_SwBand8812(pAdapter, channelToSW) == false)
        {
            RTW_INFO("error Chnl %d !\n", channelToSW);
        }

        /* <20130313, Kordan> Sample code to demonstrate how to configure AGC_TAB_DIFF.(Disabled by now) */

        if (pHalData.rf_chip == RF_CHIP_E.RF_PSEUDO_11N)
        {
            RTW_INFO("phy_SwChnl8812: return for PSEUDO\n");
            return;
        }

        /* RTW_INFO("[BW:CHNL], phy_SwChnl8812(), switch to channel %d !!\n", channelToSW); */

        /* fc_area		 */
        if (36 <= channelToSW && channelToSW <= 48)
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (15 <= channelToSW && channelToSW <= 35)
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (50 <= channelToSW && channelToSW <= 80)
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x453);
        }
        else if (82 <= channelToSW && channelToSW <= 116)
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x452);
        }
        else if (118 <= channelToSW)
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x412);
        }
        else
        {
            phy_set_bb_reg(pAdapter, rFc_area_Jaguar, 0x1ffe0000, 0x96a);
        }

        for (var eRFPath = 0; eRFPath < pHalData.NumTotalRFPath; eRFPath++)
        {
            /* RF_MOD_AG */
            if (36 <= channelToSW && channelToSW <= 80)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8, 0x101); /* 5'b00101); */
            }
            else if (15 <= channelToSW && channelToSW <= 35)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8, 0x101); /* 5'b00101); */
            }
            else if (82 <= channelToSW && channelToSW <= 140)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8, 0x301); /* 5'b01101); */
            }
            else if (140 < channelToSW)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8, 0x501); /* 5'b10101); */
            }
            else
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8, 0x000); /* 5'b00000); */
            }

            /* <20121109, Kordan> A workaround for 8812A only. */
            phy_FixSpur_8812A(pAdapter, pHalData.current_channel_bw, channelToSW);
            phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, bMaskByte0, channelToSW);
        }

        /*only for 8812A mp mode*/
        if ((pHalData.LNAType_5G == 0x00) && pAdapter.registrypriv.mp_mode == 1)
        {
            throw new NotImplementedException();
            //phy_SpurCalibration_8812A(pAdapter, channelToSW, bandwidthToSw);
        }
    }

    static void phy_PostSetBwMode8812(PADAPTER Adapter)
    {
        u8 SubChnlNum = 0;
        u8 L1pkVal = 0, reg_837 = 0;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);


        /* 3 Set Reg668 BW */
        phy_SetRegBW_8812(Adapter, pHalData.current_channel_bw);

        /* 3 Set Reg483 */
        SubChnlNum = phy_GetSecondaryChnl_8812(Adapter);
        rtw_write8(Adapter, REG_DATA_SC_8812, SubChnlNum);

        if (pHalData.rf_chip == RF_CHIP_E.RF_PSEUDO_11N)
        {
            RTW_INFO("phy_PostSetBwMode8812: return for PSEUDO\n");
            return;
        }

        reg_837 = rtw_read8(Adapter, rBWIndication_Jaguar + 3);
        /* 3 Set Reg848 Reg864 Reg8AC Reg8C4 RegA00 */
        switch (pHalData.current_channel_bw)
        {
            case channel_width.CHANNEL_WIDTH_20:
                phy_set_bb_reg(Adapter, rRFMOD_Jaguar, 0x003003C3, 0x00300200); /* 0x8ac[21,20,9:6,1,0]=8'b11100000 */
                phy_set_bb_reg(Adapter, rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */

                if (pHalData.rf_type == rf_type.RF_2T2R)
                {
                    phy_set_bb_reg(Adapter, rL1PeakTH_Jaguar, 0x03C00000, 7); /* 2R 0x848[25:22] = 0x7 */
                }
                else
                {
                    phy_set_bb_reg(Adapter, rL1PeakTH_Jaguar, 0x03C00000, 8); /* 1R 0x848[25:22] = 0x8 */
                }

                break;

            case channel_width.CHANNEL_WIDTH_40:
                phy_set_bb_reg(Adapter, rRFMOD_Jaguar, 0x003003C3, 0x00300201); /* 0x8ac[21,20,9:6,1,0]=8'b11100000		 */
                phy_set_bb_reg(Adapter, rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */
                phy_set_bb_reg(Adapter, rRFMOD_Jaguar, 0x3C, SubChnlNum);
                phy_set_bb_reg(Adapter, rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) !=0)
                    L1pkVal = 6;
                else
                {
                    if (pHalData.rf_type == RF_2T2R)
                        L1pkVal = 7;
                    else
                        L1pkVal = 8;
                }

                phy_set_bb_reg(Adapter, rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x6 */

                if (SubChnlNum == VHT_DATA_SC_20_UPPER_OF_80MHZ)
                {
                    phy_set_bb_reg(Adapter, rCCK_System_Jaguar, bCCK_System_Jaguar, 1);
                }
                else
                {
                    phy_set_bb_reg(Adapter, rCCK_System_Jaguar, bCCK_System_Jaguar, 0);
                }
                break;

            case channel_width.CHANNEL_WIDTH_80:
                phy_set_bb_reg(Adapter, rRFMOD_Jaguar, 0x003003C3, 0x00300202); /* 0x8ac[21,20,9:6,1,0]=8'b11100010 */
                phy_set_bb_reg(Adapter, rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8c4[30] = 1 */
                phy_set_bb_reg(Adapter, rRFMOD_Jaguar, 0x3C, SubChnlNum);
                phy_set_bb_reg(Adapter, rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 5;
                else
                {
                    if (pHalData.rf_type == rf_type.RF_2T2R)
                    {
                        L1pkVal = 6;
                    }
                    else
                    {
                        L1pkVal = 7;
                    }
                }

                phy_set_bb_reg(Adapter, rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x5 */

                break;

            default:
                RTW_INFO("phy_PostSetBWMode8812():	unknown Bandwidth: %#X\n", pHalData.current_channel_bw);
                break;
        }

        /* <20121109, Kordan> A workaround for 8812A only. */
        phy_FixSpur_8812A(Adapter, pHalData.current_channel_bw, pHalData.current_channel);

        /* RTW_INFO("phy_PostSetBwMode8812(): Reg483: %x\n", rtw_read8(Adapter, 0x483)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg668: %x\n", rtw_read32(Adapter, 0x668)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg8AC: %x\n", phy_query_bb_reg(Adapter, rRFMOD_Jaguar, 0xffffffff)); */

        /* 3 Set RF related register */
        PHY_RF6052SetBandwidth8812(Adapter, pHalData.current_channel_bw);
    }

    /*@ **********************************************************************
 * <20121113, Kordan> This function should be called when tx_agc changed.
 * Otherwise the previous compensation is gone, because we record the
 * delta of temperature between two TxPowerTracking watch dogs.
 *
 * NOTE: If Tx BB swing or Tx scaling is varified during run-time, still
 * need to call this function.
 * **********************************************************************
 */
    static void odm_clear_txpowertracking_state(dm_struct dm_void)
    {

        dm_struct dm = dm_void;
        _hal_rf_ rf = dm.rf_table;
        u8 p = 0;
        dm_rf_calibration_struct cali_info = dm.rf_calibrate_info;

        cali_info.bb_swing_idx_cck_base = cali_info.default_cck_index;
        cali_info.bb_swing_idx_cck = cali_info.default_cck_index;
        dm.rf_calibrate_info.CCK_index = 0;

        for (p = RF_PATH_A; p < MAX_RF_PATH; ++p)
        {
            cali_info.bb_swing_idx_ofdm_base[p]
                = cali_info.default_ofdm_index;
            cali_info.bb_swing_idx_ofdm[p] = cali_info.default_ofdm_index;
            cali_info.OFDM_index[p] = cali_info.default_ofdm_index;

            cali_info.power_index_offset[p] = 0;
            cali_info.delta_power_index[p] = 0;
            cali_info.delta_power_index_last[p] = 0;

            /* Initial Mix mode power tracking*/
            cali_info.absolute_ofdm_swing_idx[p] = 0;
            cali_info.remnant_ofdm_swing_idx[p] = 0;
            cali_info.kfree_offset[p] = 0;
        }

/* Initial Mix mode power tracking*/
        cali_info.modify_tx_agc_flag_path_a = false;
        cali_info.modify_tx_agc_flag_path_b = false;
        cali_info.modify_tx_agc_flag_path_c = false;
        cali_info.modify_tx_agc_flag_path_d = false;
        cali_info.remnant_cck_swing_idx = 0;
        cali_info.thermal_value = rf.eeprom_thermal;
        cali_info.modify_tx_agc_value_cck = 0;
        cali_info.modify_tx_agc_value_ofdm = 0;
    }

    static void halrf_iqk_trigger(dm_struct dm_void, bool is_recovery)
    {

        dm_struct dm = dm_void;
        dm_iqk_info iqk_info = dm.IQK_info;
        dm_dpk_info dpk_info = dm.dpk_info;
        _hal_rf_ rf = dm.rf_table;
        u64 start_time;


        //if (dm.mp_mode &&
        //    rf.is_con_tx &&
        //    rf.is_single_tone &&
        //    rf.is_carrier_suppresion)
        //    if (*dm.mp_mode &&
        //        ((*rf.is_con_tx ||
        //          *rf.is_single_tone ||
        //          *rf.is_carrier_suppresion)))
        //    {
        //        return;
        //    }

        if (!(rf.rf_supportability & HAL_RF_IQK))
        {
            return;
        }

        if (iqk_info.rfk_forbidden)
        {
            return;
        }

        if (!dm.rf_calibrate_info.is_iqk_in_progress)
        {
            dm.rf_calibrate_info.is_iqk_in_progress = true;
            start_time = odm_get_current_time(dm);
            {
                phy_iq_calibrate_8812a(dm, is_recovery);
            }

            dm.rf_calibrate_info.iqk_progressing_time = odm_get_progressing_time(dm, start_time);
            dm.rf_calibrate_info.is_iqk_in_progress = false;
        }
        else
        {
            //RF_DBG(dm, DBG_RF_IQK, "== Return the IQK CMD, because RFKs in Progress ==\n");
        }
    }
}