namespace Rtl8812auNet.Rtl8812au;

public static class hal_com_phycfg
{
    public static u8 PHY_GetTxPowerIndexBase(
            PADAPTER pAdapter,
            rf_path RFPath,
            MGN_RATE Rate,
            RF_TX_NUM ntx_idx,
            channel_width BandWidth,
            u8 Channel,
            BOOLEAN bIn24G
        )
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        dm_struct pDM_Odm = pHalData.odmpriv;
        u8 i = 0; /* default set to 1S */
        u8 txPower = 0;
//        u8 chnlIdx = (u8)(Channel - 1);

//        if (HAL_IsLegalChannel(pAdapter, Channel) == false)
//        {
//            chnlIdx = 0;
//            RTW_INFO("Illegal channel!!\n");
//        }


//        bIn24G = phy_GetChnlIndex(Channel, chnlIdx);


//        if (bIn24G)
//        {
//            if (IS_CCK_RATE(Rate))
//            {
//                /* CCK-nTX */
//                txPower = pHalData.Index24G_CCK_Base[RFPath][chnlIdx];
//                txPower += pHalData.CCK_24G_Diff[RFPath][RF_1TX];
//                if (ntx_idx >= RF_2TX)
//                {
//                    txPower += pHalData.CCK_24G_Diff[RFPath][RF_2TX];
//                }

//                if (ntx_idx >= RF_3TX)
//                {
//                    txPower += pHalData.CCK_24G_Diff[RFPath][RF_3TX];
//                }

//                if (ntx_idx >= RF_4TX)
//                {
//                    txPower += pHalData.CCK_24G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

//            txPower = pHalData.Index24G_BW40_Base[RFPath][chnlIdx];

///* OFDM-nTX */
//            if ((MGN_RATE.MGN_6M <= Rate && Rate <= MGN_RATE.MGN_54M) && !IS_CCK_RATE(Rate))
//            {
//                txPower += pHalData.OFDM_24G_Diff[RFPath][RF_1TX];
//                if (ntx_idx >= RF_2TX)
//                {
//                    txPower += pHalData.OFDM_24G_Diff[RFPath][RF_2TX];
//                }

//                if (ntx_idx >= RF_3TX)
//                {
//                    txPower += pHalData.OFDM_24G_Diff[RFPath][RF_3TX];
//                }

//                if (ntx_idx >= RF_4TX)
//                {
//                    txPower += pHalData.OFDM_24G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

///* BW20-nS */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_20)
//            {
//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_24G_Diff[RFPath][RF_1TX];
//                }

//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_24G_Diff[RFPath][RF_2TX];
//                }

//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_24G_Diff[RFPath][RF_3TX];
//                }

//                if ((MGN_RATE.MGN_MCS24 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_24G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

///* BW40-nS */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_40)
//            {
//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_1TX];
//                }

//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_2TX];
//                }

//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_3TX];
//                }

//                if ((MGN_RATE.MGN_MCS24 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

///* Willis suggest adopt BW 40M power index while in BW 80 mode */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_80)
//            {
//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_1TX];
//                }

//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_2TX];
//                }

//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_3TX];
//                }

//                if ((MGN_RATE.MGN_MCS24 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_24G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }
//        }

//        else
//        {
//            if (Rate >= MGN_RATE.MGN_6M)
//            {
//                txPower = pHalData.Index5G_BW40_Base[RFPath][chnlIdx];
//            }
//            else
//            {
//                RTW_INFO("===>PHY_GetTxPowerIndexBase: INVALID Rate(0x%02x).\n", Rate);
//                goto exit;
//            }

//            /* OFDM-nTX */
//            if ((MGN_RATE.MGN_6M <= Rate && Rate <= MGN_RATE.MGN_54M) && !IS_CCK_RATE(Rate))
//            {
//                txPower += pHalData.OFDM_5G_Diff[RFPath][RF_1TX];
//                if (ntx_idx >= RF_2TX)
//                    txPower += pHalData.OFDM_5G_Diff[RFPath][RF_2TX];
//                if (ntx_idx >= RF_3TX)
//                    txPower += pHalData.OFDM_5G_Diff[RFPath][RF_3TX];
//                if (ntx_idx >= RF_4TX)
//                    txPower += pHalData.OFDM_5G_Diff[RFPath][RF_4TX];
//                goto exit;
//            }

//            /* BW20-nS */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_20)
//            {
//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_5G_Diff[RFPath][RF_1TX];
//                }
//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_5G_Diff[RFPath][RF_2TX];
//                }
//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_5G_Diff[RFPath][RF_3TX];
//                }
//                if ((MGN_RATE.MGN_MCS24 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW20_5G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

//            /* BW40-nS */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_40)
//            {
//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_5G_Diff[RFPath][RF_1TX];
//                }

//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_5G_Diff[RFPath][RF_2TX];
//                }

//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_5G_Diff[RFPath][RF_3TX];
//                }
//                if ((MGN_RATE.MGN_MCS24 <= Rate && Rate <= MGN_RATE.MGN_MCS31) || (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW40_5G_Diff[RFPath][RF_4TX];
//                }
//                goto exit;
//            }

//            /* BW80-nS */
//            if (BandWidth == channel_width.CHANNEL_WIDTH_80)
//            {
//                /* get 80MHz cch index */
//                for (i = 0; i < CENTER_CH_5G_80M_NUM; ++i)
//                {
//                    if (center_ch_5g_80m[i] == Channel)
//                    {
//                        chnlIdx = i;
//                        break;
//                    }
//                }

//                if (i >= CENTER_CH_5G_80M_NUM)
//                {
//                    if (rtw_mp_mode_check(pAdapter) == false)
//                        throw new Exception();

//                    txPower = 0;
//                    goto exit;
//                }

//                txPower = pHalData.Index5G_BW80_Base[RFPath,chnlIdx];

//                if ((MGN_RATE.MGN_MCS0 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT1SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += +pHalData.BW80_5G_Diff[RFPath, RF_1TX];
//                }
//                if ((MGN_RATE.MGN_MCS8 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT2SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW80_5G_Diff[RFPath, RF_2TX];
//                }
//                if ((MGN_RATE.MGN_MCS16 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT3SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW80_5G_Diff[RFPath, RF_3TX];
//                }
//                if ((MGN_RATE.MGN_MCS23 <= Rate && Rate <= MGN_RATE.MGN_MCS31) ||
//                    (MGN_RATE.MGN_VHT4SS_MCS0 <= Rate && Rate <= MGN_RATE.MGN_VHT4SS_MCS9))
//                {
//                    txPower += pHalData.BW80_5G_Diff[RFPath,RF_4TX];
//                }
//                goto exit;
//            }

//            /* TODO: BW160-nS */
//            throw new Exception();
//        }


//        exit:
        return txPower;
    }

    //public static s8 PHY_GetTxPowerByRate(PADAPTER    pAdapter,u8          Band, rf_path    RFPath, MGN_RATE          Rate)
    //{
    //    if (!phy_is_tx_power_by_rate_needed(pAdapter))
    //        return 0;

    //    return _PHY_GetTxPowerByRate(pAdapter, Band, RFPath, Rate);
    //}
}