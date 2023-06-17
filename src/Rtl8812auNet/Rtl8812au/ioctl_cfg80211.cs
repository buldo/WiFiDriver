namespace WiFiDriver.App.Rtl8812au;

public static class ioctl_cfg80211
{
    public static void cfg80211_rtw_change_iface(_adapter padapter, InitChannel initChannel)
    {
        nl80211_iftype type = nl80211_iftype.NL80211_IFTYPE_MONITOR;

        RTW_INFO($"cfg80211_rtw_change_iface type={type}, hw_port:{padapter.hw_port}");

        RTW_INFO("cfg80211_rtw_change_iface call netdev_open");
        if (netdev_open(padapter, initChannel) == false)
        {
            throw new Exception("cfg80211_rtw_change_iface failed netdev_open");
        }

        if (_FAIL == rtw_pwr_wakeup(padapter))
        {
            RTW_INFO("cfg80211_rtw_change_iface call rtw_pwr_wakeup fail");
            throw new Exception("cfg80211_rtw_change_iface call rtw_pwr_wakeup fail");
        }

        nl80211_iftype old_type = nl80211_iftype.NL80211_IFTYPE_STATION;
        RTW_INFO("cfg80211_rtw_change_iface old_iftype=%d, new_iftype=%d", old_type, type);

        LeaveAllPowerSaveMode(padapter);

        // Looks like it do nothing in this case
        NDIS_802_11_NETWORK_INFRASTRUCTURE networkType = NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor;
        //if (rtw_set_802_11_infrastructure_mode(padapter, networkType, 0, NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Infrastructure) == false)
        //{
        //    throw new Exception();
        //}

        rtw_setopmode_cmd(padapter, RTW_CMDF.RTW_CMDF_WAIT_ACK);
        RTW_INFO($"cfg80211_rtw_change_iface end");
    }

    public static int cfg80211_rtw_set_monitor_channel(_adapter padapter, InitChannel chandef)
    {
        set_channel_bwmode(padapter, chandef.cur_channel, chandef.cur_ch_offset, chandef.cur_bwmode);
        return 0;
    }

    public static void rtw_cfg80211_init_wiphy(_adapter padapter)
    {
//        u8 rf_type;

//        ieee80211_supported_band band;
//        wireless_dev pwdev = padapter.rtw_wdev;
//        wiphy wiphy = pwdev.wiphy;

//        //rtw_hal_get_hwreg(padapter, HW_VAR_RF_TYPE, (u8*)(&rf_type));
//        rf_type = 2;

//        RTW_INFO($"rtw_cfg80211_init_wiphy:rf_type={rf_type}");

//        // TRUE
//        //if (IsSupported24G(padapter->registrypriv.wireless_mode))
//        {
//            //band = wiphy.bands[NL80211_BAND_2GHZ];
//            //if (band)
//            {

//                rtw_cfg80211_init_ht_capab(padapter, band.ht_cap, BAND_ON_2_4G, rf_type);

//            }
//        }

//        // TRUE
//        //if (is_supported_5g(padapter->registrypriv.wireless_mode))
//        {
//            //band = wiphy.bands[NL80211_BAND_5GHZ];
//            //if (band)
//            {
//                rtw_cfg80211_init_ht_capab(padapter, band.ht_cap, BAND_ON_5G, rf_type);
//                rtw_cfg80211_init_vht_capab(padapter, band.vht_cap, BAND_ON_5G, rf_type);
//            }
//        }

///* copy mac_addr to wiphy */
//        //_rtw_memcpy(wiphy->perm_addr, adapter_mac_addr(padapter), ETH_ALEN);
    }
}