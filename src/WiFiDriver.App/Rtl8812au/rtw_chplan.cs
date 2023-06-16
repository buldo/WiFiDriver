namespace WiFiDriver.App.Rtl8812au;

public static class rtw_chplan
{
    public static u8 init_channel_set(_adapter padapter, u8 ChannelPlan, RT_CHANNEL_INFO[] channel_set)
    {

        registry_priv regsty = adapter_to_regsty(padapter);
        u8 index, chanset_size = 0;
        //bool b5GBand = false;
        //bool b2_4GBand = false;
        //u8 rd_2g = 0, rd_5g = 0;


        ////if (!rtw_is_channel_plan_valid(ChannelPlan))
        ////{
        ////    RTW_ERR("ChannelPlan ID 0x%02X error !!!!!\n", ChannelPlan);
        ////    return chanset_size;
        ////}


        //b2_4GBand = true;
        //b5GBand = true;

        //if (b2_4GBand)
        //{
        //    if (ChannelPlan == RTW_CHPLAN_REALTEK_DEFINE)
        //        rd_2g = RTW_CHANNEL_PLAN_MAP_REALTEK_DEFINE.rd_2g;
        //    else
        //        rd_2g = RTW_ChannelPlanMap[ChannelPlan].rd_2g;

        //    for (index = 0; index < CH_LIST_LEN(RTW_ChannelPlan2G[rd_2g]); index++)
        //    {
        //        if (rtw_regsty_is_excl_chs(regsty, CH_LIST_CH(RTW_ChannelPlan2G[rd_2g], index)) == _TRUE)
        //        {
        //            continue;
        //        }

        //        if (chanset_size >= MAX_CHANNEL_NUM)
        //        {
        //            RTW_WARN($"chset size can't exceed MAX_CHANNEL_NUM({MAX_CHANNEL_NUM})");
        //            break;
        //        }

        //        channel_set[chanset_size].ChannelNum = CH_LIST_CH(RTW_ChannelPlan2G[rd_2g], index);

        //        if (ChannelPlan == RTW_CHPLAN_GLOBAL_DOAMIN
        //            || rd_2g == RTW_RD_2G_GLOBAL
        //           )
        //        {
        //            /* Channel 1~11 is active, and 12~14 is passive */
        //            if (channel_set[chanset_size].ChannelNum >= 1 && channel_set[chanset_size].ChannelNum <= 11)
        //                channel_set[chanset_size].ScanType = SCAN_ACTIVE;
        //            else if ((channel_set[chanset_size].ChannelNum >= 12 && channel_set[chanset_size].ChannelNum <= 14))
        //                channel_set[chanset_size].ScanType = SCAN_PASSIVE;
        //        }
        //        else if (ChannelPlan == RTW_CHPLAN_WORLD_WIDE_13
        //                 || ChannelPlan == RTW_CHPLAN_WORLD_WIDE_5G
        //                 || rd_2g == RTW_RD_2G_WORLD
        //                )
        //        {
        //            /* channel 12~13, passive scan */
        //            if (channel_set[chanset_size].ChannelNum <= 11)
        //                channel_set[chanset_size].ScanType = SCAN_ACTIVE;
        //            else
        //                channel_set[chanset_size].ScanType = SCAN_PASSIVE;
        //        }
        //        else
        //            channel_set[chanset_size].ScanType = SCAN_ACTIVE;

        //        chanset_size++;
        //    }
        //}


        //if (b5GBand)
        //{
        //    if (ChannelPlan == RTW_CHPLAN_REALTEK_DEFINE)
        //    {
        //        rd_5g = RTW_CHANNEL_PLAN_MAP_REALTEK_DEFINE.rd_5g;
        //    }
        //    else
        //    {
        //        rd_5g = RTW_ChannelPlanMap[ChannelPlan].rd_5g;
        //    }

        //    for (index = 0; index < CH_LIST_LEN(RTW_ChannelPlan5G[rd_5g]); index++)
        //    {
        //        if (rtw_regsty_is_excl_chs(regsty, CH_LIST_CH(RTW_ChannelPlan5G[rd_5g], index)) == _TRUE)
        //        {
        //            continue;
        //        }

        //        if (chanset_size >= MAX_CHANNEL_NUM)
        //        {
        //            RTW_WARN($"chset size can't exceed MAX_CHANNEL_NUM({MAX_CHANNEL_NUM})");
        //            break;
        //        }

        //        channel_set[chanset_size].ChannelNum = CH_LIST_CH(RTW_ChannelPlan5G[rd_5g], index);

        //        if ((ChannelPlan == RTW_CHPLAN_WORLD_WIDE_5G) /* all channels passive */
        //            || (rtw_is_5g_band1(channel_set[chanset_size].ChannelNum)
        //                && rtw_rd_5g_band1_passive(rd_5g)) /* band1 passive */
        //            || (rtw_is_5g_band4(channel_set[chanset_size].ChannelNum)
        //                && rtw_rd_5g_band4_passive(rd_5g)) /* band4 passive */
        //            || (rtw_is_dfs_ch(channel_set[chanset_size].ChannelNum)) /* DFS channel(band2, 3) passive */
        //           )
        //        {
        //            channel_set[chanset_size].ScanType = RT_SCAN_TYPE.SCAN_PASSIVE;
        //        }
        //        else
        //        {
        //            channel_set[chanset_size].ScanType = RT_SCAN_TYPE.SCAN_ACTIVE;
        //        }

        //        chanset_size++;
        //    }
        //}

        //if (chanset_size != 0)
        //{
        //    RTW_INFO($" ChannelPlan ID:0x{ChannelPlan:X}, ch num:{chanset_size:X}");
        //}
        //else
        //{
        //    RTW_WARN($"ChannelPlan ID:0x{ChannelPlan:X}, final chset has no channel");
        //}

        return chanset_size;
    }
}