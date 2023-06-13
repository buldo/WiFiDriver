using System.Collections.Generic;
using System.IO;
using System.Threading.Channels;

using WiFiDriver.App.Rtl8812au;

using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public static void PHY_SetBBReg8812(
        PADAPTER Adapter,
        u16 RegAddr,
        u32 BitMask,
        u32 Data
    )
    {
        u32 OriginalValue, BitShift;

        if (BitMask != bMaskDWord)
        {
            /* if not "double word" write */
            OriginalValue = rtw_read32(Adapter, RegAddr);
            BitShift = PHY_CalculateBitShift(BitMask);
            Data = ((OriginalValue) & (~BitMask)) | (((Data << (int)BitShift)) & BitMask);
        }

        rtw_write32(Adapter, RegAddr, Data);

        /* RTW_INFO("BBW MASK=0x%x Addr[0x%x]=0x%x\n", BitMask, RegAddr, Data); */
    }

    static u32 PHY_CalculateBitShift(u32 BitMask)
    {
        int i;

        for (i = 0; i <= 31; i++)
        {
            if (((BitMask >> i) & 0x1) == 1)
                break;
        }

        return (u32)i;
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
        rf_path path = 0;

        /* RTW_INFO("==>PHY_SetTxPowerLevel8812()\n"); */

        for (path = (u8)rf_path.RF_PATH_A; (byte)path < pHalData.NumTotalRFPath; ++path)
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
        u32 PowerLevel, writeData;
        u16 writeOffset;

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

        UsbHalInit.phy_set_bb_reg(Adapter, writeOffset, 0xffffff, writeData);
    }

    static u8 phy_get_tx_power_index(PADAPTER pAdapter, rf_path RFPath, MGN_RATE Rate, channel_width BandWidth,
        u8 Channel)
    {
        return rtw_hal_get_tx_power_index(pAdapter, RFPath, Rate, BandWidth, Channel, null);
    }

    static u8 rtw_hal_get_tx_power_index(
        PADAPTER padapter,
        rf_path rfpath,
        MGN_RATE rate,
        channel_width bandwidth,
        u8 channel,
        txpwr_idx_comp tic)
    {
        return padapter.hal_func.get_tx_power_index_handler(padapter, rfpath, rate, bandwidth, channel, tic);
    }

    public static u8 PHY_GetTxPowerIndex_8812A(
        PADAPTER pAdapter,
        rf_path RFPath,
        MGN_RATE Rate,
        channel_width BandWidth,
        u8 Channel,
        txpwr_idx_comp? tic
    )
    {
        //PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        //hal_spec_t hal_spec = GET_HAL_SPEC(pAdapter);
        //s16 power_idx;
        //u8 base_idx = 0;
        //s8 by_rate_diff = 0, limit = 0, tpt_offset = 0, extra_bias = 0;
        //RF_TX_NUM ntx_idx = phy_get_current_tx_num(pAdapter, Rate);
        //BOOLEAN bIn24G = false;

        //base_idx = PHY_GetTxPowerIndexBase(
        //    pAdapter,
        //    RFPath,
        //    Rate,
        //    ntx_idx,
        //    BandWidth,
        //    Channel,
        //    bIn24G);

        //by_rate_diff = PHY_GetTxPowerByRate(
        //    pAdapter,
        //    (u8)((!bIn24G ? 1 : 0)),
        //    RFPath,
        //    Rate);


        //limit = PHY_GetTxPowerLimit(
        //    pAdapter,
        //    null,
        //    (u8)((!bIn24G ? 1:0)),
        //    pHalData.current_channel_bw,
        //    RFPath,
        //    Rate,
        //    ntx_idx,
        //    pHalData.current_channel);

        //tpt_offset = PHY_GetTxPowerTrackingOffset(pAdapter, RFPath, Rate);

        //if (tic != null)
        //{
        //    tic.ntx_idx = ntx_idx;
        //    tic.@base = base_idx;
        //    tic.by_rate = by_rate_diff;
        //    tic.limit = limit;
        //    tic.tpt = tpt_offset;
        //    tic.ebias = extra_bias;
        //}

        //by_rate_diff = by_rate_diff > limit ? limit : by_rate_diff;
        //power_idx = (s16)(base_idx + by_rate_diff + tpt_offset + extra_bias + transmit_power_boost);

        //if (transmit_power_override != 0)
        //{
        //    power_idx = transmit_power_override;
        //}

        //if (power_idx < 1)
        //{
        //    power_idx = 1;
        //}

        //if (power_idx < 0)
        //{
        //    power_idx = 0;
        //}
        //else if (power_idx > hal_spec.txgi_max)
        //{
        //    power_idx = hal_spec.txgi_max;
        //}

        //return (byte)power_idx;
        return 42;
    }


    static void phy_set_tx_power_level_by_path(PADAPTER Adapter, u8 channel, rf_path path)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        BOOLEAN bIsIn24G = (pHalData.current_band_type == BAND_TYPE.BAND_ON_2_4G);
        bool under_survey_ch = phy_check_under_survey_ch(Adapter);


        /* if ( pMgntInfo.RegNByteAccess == 0 ) */
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

    static void phy_set_tx_power_index_by_rate_section(
        PADAPTER pAdapter,
        rf_path RFPath,
        u8 Channel,
        RATE_SECTION RateSection)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);

        if (RateSection >= RATE_SECTION.RATE_SECTION_NUM)
        {
            throw new Exception("RateSection >= RATE_SECTION.RATE_SECTION_NUM");
        }

        if (RateSection == RATE_SECTION.CCK && pHalData.current_band_type != BAND_TYPE.BAND_ON_2_4G)
            goto exit;

        PHY_SetTxPowerIndexByRateArray(
            pAdapter,
            RFPath,
            pHalData.current_channel_bw,
            Channel,
            rates_by_sections[(int)RateSection].rates,
            rates_by_sections[(int)RateSection].rate_num);

        exit:
        return;
    }

    static void PHY_SetTxPowerIndexByRateArray(
        PADAPTER pAdapter,
        rf_path RFPath,
        channel_width BandWidth,
        u8 Channel,
        MGN_RATE[] Rates,
        u8 RateArraySize
    )
    {
        u32 powerIndex = 0;
        int i = 0;

        for (i = 0; i < RateArraySize; ++i)
        {
            powerIndex = phy_get_tx_power_index(pAdapter, RFPath, Rates[i], BandWidth, Channel);

            PHY_SetTxPowerIndex(pAdapter, powerIndex, RFPath, Rates[i]);
        }
    }

    static void PHY_SetTxPowerIndex(PADAPTER pAdapter, u32 PowerIndex, rf_path RFPath, MGN_RATE Rate)
    {
        rtw_hal_set_tx_power_index(pAdapter, PowerIndex, RFPath, Rate);
    }

    static void rtw_hal_set_tx_power_index(PADAPTER padapter, u32 powerindex, rf_path rfpath, MGN_RATE rate)
    {
        padapter.hal_func.set_tx_power_index_handler(padapter, powerindex, rfpath, rate);
    }

    static bool phy_check_under_survey_ch(_adapter adapter)
    {

        var dvobj = adapter_to_dvobj(adapter);
        _adapter iface = adapter;
        //mlme_ext_priv mlmeext;
        //bool ret = false;

        //mlmeext = iface.mlmeextpriv;

        //    /* check scan state */
        //if (mlmeext_scan_state(mlmeext) != SCAN_DISABLE
        //    && mlmeext_scan_state(mlmeext) != SCAN_COMPLETE
        //    && mlmeext_scan_state(mlmeext) != SCAN_BACKING_OP)
        //{
        //    ret = _TRUE;
        //}
        //else if (mlmeext_scan_state(mlmeext) == SCAN_BACKING_OP
        //         && !mlmeext_chk_scan_backop_flags(mlmeext, SS_BACKOP_TX_RESUME))
        //{
        //    ret = true;
        //}


        //return ret;

        return true;
    }

    static void phy_SwChnl8812(PADAPTER pAdapter)
    {

        HAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        u8 channelToSW = pHalData.current_channel;
        var bandwidthToSw = pHalData.current_channel_bw;

        if (phy_SwBand8812(pAdapter, channelToSW) == false)
        {
            RTW_INFO("error Chnl %d !", channelToSW);
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

        for (rf_path eRFPath = 0; (byte)eRFPath < pHalData.NumTotalRFPath; eRFPath++)
        {
            /* RF_MOD_AG */
            if (36 <= channelToSW && channelToSW <= 80)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (15 <= channelToSW && channelToSW <= 35)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (82 <= channelToSW && channelToSW <= 140)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x301); /* 5'b01101); */
            }
            else if (140 < channelToSW)
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x501); /* 5'b10101); */
            }
            else
            {
                phy_set_rf_reg(pAdapter, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x000); /* 5'b00000); */
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

    static BOOLEAN phy_SwBand8812(PADAPTER pAdapter, u8 channelToSW)
    {
        u8 u1Btmp;
        BOOLEAN ret_value = true;
        BAND_TYPE Band = BAND_TYPE.BAND_ON_5G;
        BAND_TYPE BandToSW;

        u1Btmp = rtw_read8(pAdapter, REG_CCK_CHECK_8812);
        if ((u1Btmp & BIT7) != 0)
        {
            Band = BAND_TYPE.BAND_ON_5G;
        }
        else
        {
            Band = BAND_TYPE.BAND_ON_2_4G;
        }

        /* Use current channel to judge Band Type and switch Band if need. */
        if (channelToSW > 14)
        {
            BandToSW = BAND_TYPE.BAND_ON_5G;
        }
        else
            BandToSW = BAND_TYPE.BAND_ON_2_4G;

        if (BandToSW != Band)
        {
            PHY_SwitchWirelessBand8812(pAdapter, BandToSW);
        }

        return ret_value;
    }

    static void phy_set_rf_reg(_adapter Adapter, rf_path eRFPath, u16 RegAddr, u32 BitMask, u32 Data) =>
        rtw_hal_write_rfreg((Adapter), (eRFPath), (RegAddr), (BitMask), (Data));

    static void rtw_hal_write_rfreg(_adapter padapter, rf_path eRFPath, u16 RegAddr, u32 BitMask, u32 Data)
    {
        if (padapter.hal_func.write_rfreg != null)
        {
            padapter.hal_func.write_rfreg(padapter, eRFPath, RegAddr, BitMask, Data);
        }
    }

    public static void PHY_SetRFReg8812(PADAPTER Adapter, rf_path eRFPath, u32 RegAddr, u32 BitMask, u32 Data)
    {

        if (BitMask == 0)
            return;

        /* RF data is 20 bits only */
        if (BitMask != bLSSIWrite_data_Jaguar)
        {
            u32 Original_Value, BitShift;
            Original_Value = phy_RFSerialRead(Adapter, eRFPath, RegAddr);
            BitShift = PHY_CalculateBitShift(BitMask);
            Data = ((Original_Value) & (~BitMask)) | (Data << (int)BitShift);
        }

        phy_RFSerialWrite(Adapter, eRFPath, RegAddr, Data);
    }

    static u32 phy_RFSerialRead(PADAPTER Adapter, rf_path eRFPath, u32 Offset)
    {
        u32 retValue = 0;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        BB_REGISTER_DEFINITION_T pPhyReg = pHalData.PHYRegDef[eRFPath];
        BOOLEAN bIsPIMode = false;



        /* <20120809, Kordan> CCA OFF(when entering), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(Adapter)))
            phy_set_bb_reg(Adapter, rCCAonSec_Jaguar, 0x8, 1);

        Offset &= 0xff;

        if (eRFPath == rf_path.RF_PATH_A)
        {
            bIsPIMode = phy_query_bb_reg(Adapter, 0xC00, 0x4) != 0;
        }
        else if (eRFPath == rf_path.RF_PATH_B)
        {
            bIsPIMode = phy_query_bb_reg(Adapter, 0xE00, 0x4) != 0;
        }

        phy_set_bb_reg(Adapter, (ushort)pPhyReg.rfHSSIPara2, bHSSIRead_addr_Jaguar, Offset);

        if (IS_VENDOR_8812A_C_CUT(Adapter))
        {
            Thread.Sleep(20);
        }

        if (bIsPIMode)
        {
            retValue = phy_query_bb_reg(Adapter, pPhyReg.rfLSSIReadBackPi, rRead_data_Jaguar);
            /* RTW_INFO("[PI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.rfLSSIReadBackPi, retValue); */
        }
        else
        {
            retValue = phy_query_bb_reg(Adapter, pPhyReg.rfLSSIReadBack, rRead_data_Jaguar);
            /* RTW_INFO("[SI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.rfLSSIReadBack, retValue); */
        }

        /* <20120809, Kordan> CCA ON(when exiting), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(Adapter)))
        {
            phy_set_bb_reg(Adapter, rCCAonSec_Jaguar, 0x8, 0);
        }


        return retValue;
    }

    public static u32 phy_query_bb_reg(_adapter Adapter, u16 RegAddr, u32 BitMask) =>
        rtw_hal_read_bbreg((Adapter), (RegAddr), (BitMask));

    public static u32 rtw_hal_read_bbreg(_adapter padapter, u16 RegAddr, u32 BitMask)
    {
        u32 data = 0;
        if (padapter.hal_func.read_bbreg != null)
        {
            data = padapter.hal_func.read_bbreg(padapter, RegAddr, BitMask);
        }
        return data;
    }

    public static u32 PHY_QueryBBReg8812(PADAPTER    Adapter,u16         RegAddr,u32         BitMask)
    {
        u32 ReturnValue = 0, OriginalValue, BitShift;

        /* RTW_INFO("--.PHY_QueryBBReg8812(): RegAddr(%#x), BitMask(%#x)\n", RegAddr, BitMask); */

        OriginalValue = rtw_read32(Adapter, RegAddr);
        BitShift = PHY_CalculateBitShift(BitMask);
        ReturnValue = (OriginalValue & BitMask) >> (int)BitShift;

        /* RTW_INFO("BBR MASK=0x%x Addr[0x%x]=0x%x\n", BitMask, RegAddr, OriginalValue); */
        return ReturnValue;
    }


    static void phy_RFSerialWrite(PADAPTER Adapter, rf_path eRFPath, u32 Offset, u32 Data)
    {
        u32 DataAndAddr = 0;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        BB_REGISTER_DEFINITION_T pPhyReg = pHalData.PHYRegDef[eRFPath];

        Offset &= 0xff;

        /* Shadow Update */
        /* PHY_RFShadowWrite(Adapter, eRFPath, Offset, Data); */

        /* Put write addr in [27:20]  and write data in [19:00] */
        DataAndAddr = ((Offset << 20) | (Data & 0x000fffff)) & 0x0fffffff;

        /* Write Operation */
        /* TODO: Dynamically determine whether using PI or SI to write RF registers. */
        phy_set_bb_reg(Adapter, (ushort)pPhyReg.rf3wireOffset, bMaskDWord, DataAndAddr);
        /* RTW_INFO("RFW-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.rf3wireOffset, DataAndAddr); */

    }

    static void phy_SetRegBW_8812(PADAPTER Adapter, channel_width CurrentBW)
    {
        u16 RegRfMod_BW, u2tmp = 0;
        RegRfMod_BW = rtw_read16(Adapter, REG_WMAC_TRXPTCL_CTL);

        switch (CurrentBW)
        {
            case channel_width.CHANNEL_WIDTH_20:
                rtw_write16(Adapter, REG_WMAC_TRXPTCL_CTL, (ushort)(RegRfMod_BW & 0xFE7F)); /* BIT 7 = 0, BIT 8 = 0 */
                break;

            case channel_width.CHANNEL_WIDTH_40:
                u2tmp = (ushort)(RegRfMod_BW | BIT7);
                rtw_write16(Adapter, REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFEFF)); /* BIT 7 = 1, BIT 8 = 0 */
                break;

            case channel_width.CHANNEL_WIDTH_80:
                u2tmp = (ushort)(RegRfMod_BW | BIT8);
                rtw_write16(Adapter, REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFF7F)); /* BIT 7 = 0, BIT 8 = 1 */
                break;

            default:
                RTW_INFO($"phy_PostSetBWMode8812():	unknown Bandwidth: {CurrentBW}");
                break;
        }
    }

    static byte phy_GetSecondaryChnl_8812(PADAPTER Adapter)
    {
        VHT_DATA_SC SCSettingOf40 = 0, SCSettingOf20 = 0;
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* RTW_INFO("SCMapping: Case: pHalData.current_channel_bw %d, pHalData.nCur80MhzPrimeSC %d, pHalData.nCur40MhzPrimeSC %d\n",pHalData.current_channel_bw,pHalData.nCur80MhzPrimeSC,pHalData.nCur40MhzPrimeSC); */
        if (pHalData.current_channel_bw == channel_width.CHANNEL_WIDTH_80)
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
        else if (pHalData.current_channel_bw == channel_width.CHANNEL_WIDTH_40)
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

    static void phy_PostSetBwMode8812(PADAPTER Adapter)
    {
        byte SubChnlNum = 0;
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

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 6;
                else
                {
                    if (pHalData.rf_type == rf_type.RF_2T2R)
                        L1pkVal = 7;
                    else
                        L1pkVal = 8;
                }

                phy_set_bb_reg(Adapter, rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x6 */

                if (SubChnlNum == (byte)VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ)
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

    static void PHY_RF6052SetBandwidth8812(PADAPTER Adapter, channel_width Bandwidth) /* 20M or 40M */
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        switch (Bandwidth)
        {
            case channel_width.CHANNEL_WIDTH_20:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 20MHz\n"); */
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                break;

            case channel_width.CHANNEL_WIDTH_40:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 40MHz\n"); */
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                break;

            case channel_width.CHANNEL_WIDTH_80:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 80MHz\n"); */
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                phy_set_rf_reg(Adapter, rf_path.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                break;

            default:
                RTW_INFO("PHY_RF6052SetBandwidth8812(): unknown Bandwidth: %#X\n", Bandwidth);
                break;
        }
    }

    static void phy_FixSpur_8812A(PADAPTER pAdapter, channel_width Bandwidth, u8 Channel)
    {
        /* C cut Item12 ADC FIFO CLOCK */
        if (IS_VENDOR_8812A_C_CUT(pAdapter))
        {
            if (Bandwidth == channel_width.CHANNEL_WIDTH_40 && Channel == 11)
                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0xC00, 0x3); /* 0x8AC[11:10] = 2'b11 */
            else
                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0xC00, 0x2); /* 0x8AC[11:10] = 2'b10 */

            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == channel_width.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
            {

                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 2'b11 */
                phy_set_bb_reg(pAdapter, rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */

            }
            else if (Bandwidth == channel_width.CHANNEL_WIDTH_40 &&
                     Channel == 11)
            {

                phy_set_bb_reg(pAdapter, rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */

            }
            else if (Bandwidth != channel_width.CHANNEL_WIDTH_80)
            {

                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 2'b10	 */
                phy_set_bb_reg(pAdapter, rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8C4[30] = 0 */

            }
        }
        else
        {
            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == channel_width.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 11 */
            else if (Channel <= 14) /* 2.4G only */
                phy_set_bb_reg(pAdapter, rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 10 */
        }

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
    public static void odm_clear_txpowertracking_state(dm_struct dm_void)
    {

//        dm_struct dm = dm_void;
//        _hal_rf_ rf = dm.rf_table;
//        u8 p = 0;
//        dm_rf_calibration_struct cali_info = dm.rf_calibrate_info;

//        cali_info.bb_swing_idx_cck_base = cali_info.default_cck_index;
//        cali_info.bb_swing_idx_cck = cali_info.default_cck_index;
//        dm.rf_calibrate_info.CCK_index = 0;

//        for (p = RF_PATH_A; p < MAX_RF_PATH; ++p)
//        {
//            cali_info.bb_swing_idx_ofdm_base[p]
//                = cali_info.default_ofdm_index;
//            cali_info.bb_swing_idx_ofdm[p] = cali_info.default_ofdm_index;
//            cali_info.OFDM_index[p] = cali_info.default_ofdm_index;

//            cali_info.power_index_offset[p] = 0;
//            cali_info.delta_power_index[p] = 0;
//            cali_info.delta_power_index_last[p] = 0;

//            /* Initial Mix mode power tracking*/
//            cali_info.absolute_ofdm_swing_idx[p] = 0;
//            cali_info.remnant_ofdm_swing_idx[p] = 0;
//            cali_info.kfree_offset[p] = 0;
//        }

///* Initial Mix mode power tracking*/
//        cali_info.modify_tx_agc_flag_path_a = false;
//        cali_info.modify_tx_agc_flag_path_b = false;
//        cali_info.modify_tx_agc_flag_path_c = false;
//        cali_info.modify_tx_agc_flag_path_d = false;
//        cali_info.remnant_cck_swing_idx = 0;
//        cali_info.thermal_value = rf.eeprom_thermal;
//        cali_info.modify_tx_agc_value_cck = 0;
//        cali_info.modify_tx_agc_value_ofdm = 0;
    }

    static void halrf_iqk_trigger(dm_struct dm_void, bool is_recovery)
    {

        dm_struct dm = dm_void;
        //dm_iqk_info iqk_info = dm.IQK_info;
        //dm_dpk_info dpk_info = dm.dpk_info;
        //_hal_rf_ rf = dm.rf_table;
        //u64 start_time;


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

        //if (!(rf.rf_supportability & HAL_RF_IQK))
        //{
        //    return;
        //}

        //if (iqk_info.rfk_forbidden)
        //{
        //    return;
        //}

        if (!dm.rf_calibrate_info.is_iqk_in_progress)
        {
            dm.rf_calibrate_info.is_iqk_in_progress = true;
            //start_time = odm_get_current_time(dm);
            //{
                phy_iq_calibrate_8812a(dm, is_recovery);
            //}

            //dm.rf_calibrate_info.iqk_progressing_time = odm_get_progressing_time(dm, start_time);
            dm.rf_calibrate_info.is_iqk_in_progress = false;
        }
        else
        {
            //RF_DBG(dm, DBG_RF_IQK, "== Return the IQK CMD, because RFKs in Progress ==\n");
        }
    }

    static void phy_iq_calibrate_8812a(dm_struct dm_void, bool is_recovery)
    {
        dm_struct dm = dm_void;
        dm_rf_calibration_struct cali_info = (dm.rf_calibrate_info);
        u32 counter = 0;

        //if (dm.fw_offload_ability & PHYDM_RF_IQK_OFFLOAD)
        //{
        //    _phy_iq_calibrate_by_fw_8812a(dm);
        //    phydm_iqk_wait(dm, 500);
        //    //{
        //    //    if (dm.rf_calibrate_info.is_iqk_in_progress)
        //    //        RF_DBG(dm, DBG_RF_IQK, "== FW IQK TIMEOUT (Still in progress after 500ms) ==\n");
        //    //}
        //}
        //else
        //{
        //    _phy_iq_calibrate_8812a(dm, dm.channel);
        //}
    }

    static void _phy_iq_calibrate_8812a(dm_struct dm, u8 channel)
    {

        //u32 MACBB_backup[MACBB_REG_NUM], AFE_backup[AFE_REG_NUM] = { 0 }, RFA_backup[RF_REG_NUM] = { 0 }, RFB_backup[RF_REG_NUM] = { 0 };
        //u32 backup_macbb_reg[MACBB_REG_NUM] = { 0x520, 0x550, 0x808, 0xa04, 0x90c, 0xc00, 0xe00, 0x838, 0x82c };
        //u32 backup_afe_reg[AFE_REG_NUM] = {0xc5c, 0xc60, 0xc64, 0xc68, 0xcb0, 0xcb4,
        //    0xe5c, 0xe60, 0xe64, 0xe68, 0xeb0, 0xeb4};
        //u32 reg_c1b8, reg_e1b8;
        //u32 backup_rf_reg[RF_REG_NUM] = { 0x65, 0x8f, 0x0 };
        //u8 chnl_idx = odm_get_right_chnl_place_for_iqk(channel);

        //_iqk_backup_mac_bb_8812a(dm, MACBB_backup, backup_macbb_reg, MACBB_REG_NUM);
        //odm_set_bb_reg(dm, R_0x82c, BIT(31), 0x1);
        //reg_c1b8 = odm_read_4byte(dm, 0xcb8);
        //reg_e1b8 = odm_read_4byte(dm, 0xeb8);
        //odm_set_bb_reg(dm, R_0x82c, BIT(31), 0x0);
        //_iqk_backup_afe_8812a(dm, AFE_backup, backup_afe_reg, AFE_REG_NUM);
        //_iqk_backup_rf_8812a(dm, RFA_backup, RFB_backup, backup_rf_reg, RF_REG_NUM);

        //_iqk_configure_mac_8812a(dm);
        //_iqk_tx_8812a(dm, chnl_idx);
        //_iqk_restore_rf_8812a(dm, RF_PATH_A, backup_rf_reg, RFA_backup, RF_REG_NUM);
        //_iqk_restore_rf_8812a(dm, RF_PATH_B, backup_rf_reg, RFB_backup, RF_REG_NUM);

        //_iqk_restore_afe_8812a(dm, AFE_backup, backup_afe_reg, AFE_REG_NUM);
        //odm_set_bb_reg(dm, R_0x82c, BIT(31), 0x1);
        //odm_write_4byte(dm, 0xcb8, reg_c1b8);
        //odm_write_4byte(dm, 0xeb8, reg_e1b8);
        //odm_set_bb_reg(dm, R_0x82c, BIT(31), 0x0);
        //_iqk_restore_mac_bb_8812a(dm, MACBB_backup, backup_macbb_reg, MACBB_REG_NUM);
    }


    public static void PHY_SetTxPowerIndex_8812A(PADAPTER Adapter, u32 PowerIndex, rf_path RFPath, MGN_RATE Rate)
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* <20120928, Kordan> A workaround in 8812A/8821A testchip, to fix the bug of odd Tx power indexes. */

        if (RFPath == rf_path.RF_PATH_A)
        {
            switch (Rate)
            {
                case MGN_RATE.MGN_1M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_2M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_5_5M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_11M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_6M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_9M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_12M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_18M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_24M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_36M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_48M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_54M:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS10:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS11:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS12:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS13:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS14:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS15:
                    phy_set_bb_reg(Adapter, rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT2SS_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT2SS_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte3, PowerIndex);
                    break;

                default:
                    RTW_INFO("Invalid Rate!!\n");
                    break;
            }
        }
        else if (RFPath == rf_path.RF_PATH_B)
        {
            switch (Rate)
            {
                case MGN_RATE.MGN_1M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_2M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_5_5M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_11M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_6M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_9M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_12M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_18M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_24M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_36M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_48M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_54M:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS10:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS11:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_MCS12:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS13:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS14:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_MCS15:
                    phy_set_bb_reg(Adapter, rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT1SS_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT1SS_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS0:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS1:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT2SS_MCS2:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS3:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS4:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS5:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte3, PowerIndex);
                    break;

                case MGN_RATE.MGN_VHT2SS_MCS6:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte0, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS7:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte1, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS8:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte2, PowerIndex);
                    break;
                case MGN_RATE.MGN_VHT2SS_MCS9:
                    phy_set_bb_reg(Adapter, rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte3, PowerIndex);
                    break;

                default:
                    RTW_INFO("Invalid Rate!!");
                    break;
            }
        }
        else
        {
            RTW_INFO("Invalid RFPath!!");
        }
    }
}