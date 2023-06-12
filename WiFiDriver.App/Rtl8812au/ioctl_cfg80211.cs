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
}