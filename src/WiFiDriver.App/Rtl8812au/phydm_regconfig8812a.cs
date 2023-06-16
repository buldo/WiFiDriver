using System.Runtime.ConstrainedExecution;
using System.Threading.Channels;
using WiFiDriver.App.Rtl8812au;

namespace WiFiDriver.App.Rtl8812au;

public static class phydm_regconfig8812a
{
    static s8 phy_txpwr_ww_lmt_value(_adapter adapter)
    {

        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);

        if (hal_spec.txgi_max == 63)
        {
            return -63;
        }
        else if (hal_spec.txgi_max == 127)
        {
            return -128;
        }

        return -128;
    }

    static void phy_set_tx_power_limit(
        _adapter Adapter,
        string Regulation,
        string Band,
        string Bandwidth,
        string RateSection,
        string ntx,
        string Channel,
        string PowerLimit
    )
    {
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);
        hal_spec_t hal_spec = GET_HAL_SPEC(Adapter);
        BAND_TYPE band = 0;
        channel_width bandwidth = 0;
        u8 tlrs = 0;
        u8 channel = byte.Parse(Channel);
        RF_TX_NUM ntx_idx;
        s8 powerLimit = sbyte.Parse(PowerLimit);
        s8 prevPowerLimit, channelIndex;
        s8 ww_lmt_val = phy_txpwr_ww_lmt_value(Adapter);

        //if (GetU1ByteIntegerFromStringInDecimal(Channel, out channel) == false || GetS1ByteIntegerFromStringInDecimal(PowerLimit, out powerLimit) == false)
        //{
        //    RTW_PRINT($"Illegal index of power limit table [ch {Channel}][val {PowerLimit}]");
        //    return;
        //}

        if (powerLimit != ww_lmt_val)
        {
            if (powerLimit < -hal_spec.txgi_max || powerLimit > hal_spec.txgi_max)
            {
                RTW_PRINT($"Illegal power limit value [ch {Channel}][val {PowerLimit}]");
            }

            if (powerLimit > hal_spec.txgi_max)
            {
                powerLimit = (sbyte)hal_spec.txgi_max;
            }
            else if (powerLimit < -hal_spec.txgi_max)

            {
                powerLimit = (sbyte)(ww_lmt_val + 1);
            }
        }

        if (RateSection == "CCK")
        {
            tlrs = TXPWR_LMT_RS_CCK;
        }
        else if (RateSection == "OFDM")
        {
            tlrs = TXPWR_LMT_RS_OFDM;
        }
        else if (RateSection == ("HT"))
        {
            tlrs = TXPWR_LMT_RS_HT;
        }
        else if (RateSection == "VHT")
        {
            tlrs = TXPWR_LMT_RS_VHT;
        }
        else
        {
            RTW_PRINT($"Wrong rate section:{RateSection}");
            return;
        }

        if (ntx == "1T")
        {
            ntx_idx = RF_TX_NUM.RF_1TX;
        }
        else if (ntx == "2T")
        {
            ntx_idx = RF_TX_NUM.RF_2TX;
        }
        else if (ntx == "3T")
        {
            ntx_idx = RF_TX_NUM.RF_3TX;
        }
        else if (ntx == "4T")
        {
            ntx_idx = RF_TX_NUM.RF_4TX;
        }
        else
        {
            RTW_PRINT($"Wrong tx num:{ntx}");
            return;
        }

        if (Bandwidth == "20M")
        {
            bandwidth = channel_width.CHANNEL_WIDTH_20;
        }
        else if (Bandwidth == "40M")
        {
            bandwidth = channel_width.CHANNEL_WIDTH_40;
        }
        else if (Bandwidth == "80M")
        {
            bandwidth = channel_width.CHANNEL_WIDTH_80;
        }
        else if (Bandwidth == "160M")
        {
            bandwidth = channel_width.CHANNEL_WIDTH_160;
        }
        else
        {
            RTW_PRINT($"unknown bandwidth: {Bandwidth}");
            return;
        }

        if (Band == "2.4G")
        {
            band = BAND_TYPE.BAND_ON_2_4G;
            channelIndex = phy_GetChannelIndexOfTxPowerLimit(BAND_TYPE.BAND_ON_2_4G, channel);

            if (channelIndex == -1)
            {
                RTW_PRINT($"unsupported channel: {channel} at 2.4G");
                return;
            }

            if ((byte)bandwidth >= MAX_2_4G_BANDWIDTH_NUM)
            {
                RTW_PRINT($"unsupported bandwidth: {Bandwidth} at 2.4G");
                return;
            }

            rtw_txpwr_lmt_add(adapter_to_rfctl(Adapter), Regulation, band, bandwidth, tlrs, ntx_idx, (byte)channelIndex,
                powerLimit);
        }

        else if (Band == "5G")
        {
            band = BAND_TYPE.BAND_ON_5G;
            channelIndex = phy_GetChannelIndexOfTxPowerLimit(BAND_TYPE.BAND_ON_5G, channel);

            if (channelIndex == -1)
            {
                RTW_PRINT($"unsupported channel: {channel} at 5G");
                return;
            }

            rtw_txpwr_lmt_add(adapter_to_rfctl(Adapter), Regulation, band, bandwidth, tlrs, ntx_idx, (byte)channelIndex,
                powerLimit);
        }

        else
        {
            RTW_PRINT($"unknown/unsupported band:{Band}");
            return;
        }
    }

    static void rtw_txpwr_lmt_add(rf_ctl_t rfctl, string regd_name, BAND_TYPE band, channel_width bw, u8 tlrs, RF_TX_NUM ntx_idx, u8 ch_idx, s8 lmt)
    {
        rtw_txpwr_lmt_add_with_nlen(rfctl, regd_name, band, bw, tlrs, ntx_idx, ch_idx, lmt);
    }

    static void rtw_txpwr_lmt_add_with_nlen(rf_ctl_t rfctl, string regd_name,  BAND_TYPE band, channel_width bw, u8 tlrs, RF_TX_NUM ntx_idx, u8 ch_idx, s8 lmt)
    {
        //hal_spec_t hal_spec = GET_HAL_SPEC(dvobj_get_primary_adapter(rfctl_to_dvobj(rfctl)));
        //txpwr_lmt_ent ent;
        //_irqL irqL;
        //_list cur,  head;
        //s8 pre_lmt;

        //// search for existed entry
        //head = rfctl.txpwr_lmt_list;
        //cur = get_next(head);
        //while ((rtw_end_of_queue_search(head, cur)) == _FALSE)
        //{
        //    ent = LIST_CONTAINOR(cur, txpwr_lmt_ent, list);
        //    cur = get_next(cur);

        //    if (strlen(ent.regd_name) == nlen
        //        && _rtw_memcmp(ent.regd_name, regd_name, nlen) == _TRUE)
        //        goto chk_lmt_val;
        //}

        ///* alloc new one */
        //ent = (txpwr_lmt_ent*)rtw_zvmalloc(sizeof(txpwr_lmt_ent) + nlen + 1);
        //if (!ent)
        //{
        //    goto release_lock;
        //}

        //_rtw_init_listhead(&ent.list);
        //_rtw_memcpy(ent.regd_name, regd_name, nlen);
        //{
        //    u8 j, k, l, m;

        //    for (j = 0; j < MAX_2_4G_BANDWIDTH_NUM; ++j)
        //    for (k = 0; k < TXPWR_LMT_RS_NUM_2G; ++k)
        //    for (m = 0; m < CENTER_CH_2G_NUM; ++m)
        //    for (l = 0; l < MAX_TX_COUNT; ++l)
        //        ent.lmt_2g[j][k][m][l] = hal_spec.txgi_max;

        //    for (j = 0; j < MAX_5G_BANDWIDTH_NUM; ++j)
        //    for (k = 0; k < TXPWR_LMT_RS_NUM_5G; ++k)
        //    for (m = 0; m < CENTER_CH_5G_ALL_NUM; ++m)
        //    for (l = 0; l < MAX_TX_COUNT; ++l)
        //        ent.lmt_5g[j][k][m][l] = hal_spec.txgi_max;

        //}

        //rtw_list_insert_tail(&ent.list, &rfctl.txpwr_lmt_list);
        //rfctl.txpwr_regd_num++;

        //chk_lmt_val:
        //if (band == BAND_ON_2_4G)
        //{
        //    pre_lmt = ent.lmt_2g[bw][tlrs][ch_idx][ntx_idx];
        //}
        //else if (band == BAND_ON_5G)
        //{
        //    pre_lmt = ent.lmt_5g[bw][tlrs - 1][ch_idx][ntx_idx];
        //}

        ////if (pre_lmt != hal_spec.txgi_max)
        ////    RTW_PRINT("duplicate txpwr_lmt for [%s][%s][%s][%s][%uT][%d]\n"
        ////        , regd_name, band_str(band), ch_width_str(bw), txpwr_lmt_rs_str(tlrs), ntx_idx + 1
        ////        , band == BAND_ON_2_4G ? ch_idx + 1 : center_ch_5g_all[ch_idx]);

        //lmt = rtw_min(pre_lmt, lmt);
        //if (band == BAND_ON_2_4G)
        //{
        //    ent.lmt_2g[bw][tlrs][ch_idx][ntx_idx] = lmt;
        //}
        //else if (band == BAND_ON_5G)
        //{
        //    ent.lmt_5g[bw][tlrs - 1][ch_idx][ntx_idx] = lmt;
        //}
    }

    static s8 phy_GetChannelIndexOfTxPowerLimit(BAND_TYPE Band, u8 Channel)
    {
        s8 channelIndex = -1;
        u8 i = 0;

        if (Band == BAND_TYPE.BAND_ON_2_4G)
        {
            channelIndex = (sbyte)(Channel - 1);
        }
        else if (Band == BAND_TYPE.BAND_ON_5G)
        {
            for (i = 0; i < CENTER_CH_5G_ALL_NUM; ++i)
            {
                if (center_ch_5g_all[i] == Channel)
                {
                    channelIndex = (sbyte)i;
                }
            }
        }
        else
        {
            RTW_PRINT($"Invalid Band {Band} in phy_GetChannelIndexOfTxPowerLimit");
        }

        if (channelIndex == -1)
        {
            RTW_PRINT($"Invalid Channel {Channel} of Band {Band} in phy_GetChannelIndexOfTxPowerLimit");
        }

        return channelIndex;
    }

    public static void odm_config_bb_txpwr_lmt_8812a(
        _adapter dm,
        string regulation,
        string band,
        string bandwidth,
        string rate_section,
        string rf_path,
        string channel,
        string power_limit)
    {
        phy_set_tx_power_limit(dm, regulation, band, bandwidth, rate_section, rf_path, channel, power_limit);
    }

    static void odm_config_rf_reg_8812a(_adapter dm, u32 addr, u32 data,rf_path RF_PATH, u16 reg_addr)
    {
        if (addr == 0xfe || addr == 0xffe)
        {
            ODM_sleep_ms(50);
        }
        else
        {
            odm_set_rf_reg(dm, RF_PATH, reg_addr, RFREGOFFSETMASK, data);
/* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }
    }

    public static void odm_config_rf_radio_a_8812a(_adapter dm, u32 addr, u32 data)
    {
        u32 content = 0x1000; /* RF_Content: radioa_txt */
        u32 maskfor_phy_set = (u32)(content & 0xE000);

        odm_config_rf_reg_8812a(dm, addr, data, rf_path.RF_PATH_A, (u16)(addr | maskfor_phy_set));

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> odm_config_rf_with_header_file: [RadioA] %08X %08X\n", addr, data);
    }

    public static void odm_config_rf_radio_b_8812a(_adapter dm, u32 addr, u32 data)
    {
        u32 content = 0x1001; /* RF_Content: radiob_txt */
        u32 maskfor_phy_set = (u32)(content & 0xE000);

        odm_config_rf_reg_8812a(dm, addr, data, rf_path.RF_PATH_B, (u16)(addr | maskfor_phy_set));

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> odm_config_rf_with_header_file: [RadioB] %08X %08X\n", addr, data);
    }

    public static void odm_config_mac_8812a(_adapter adapter, u16 addr, u8 data)
    {
        odm_write_1byte(adapter, addr, data);
        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "===> odm_config_mac_with_header_file: [MAC_REG] %08X %08X\n",
        //    addr, data);
    }

    public static void odm_config_bb_phy_8812a(_adapter dm, u32 addr, u32 bitmask, u32 data)
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
            odm_set_bb_reg(dm, addr, bitmask, data);
            /* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //	  "===> odm_config_bb_with_header_file: [PHY_REG] %08X %08X\n",
        //	  addr, data);
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

    public static void odm_config_bb_agc_8812a(_adapter dm, u32 addr, u32 bitmask,u32 data)
    {

        odm_set_bb_reg(dm, addr, bitmask, data);
        /* Add 1us delay between BB/RF register setting. */
        ODM_delay_us(1);

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "===> odm_config_bb_with_header_file: [AGC_TAB] %08X %08X\n",
        //    addr, data);
    }
}