namespace Rtl8812auNet.Rtl8812au;

public static class ioctl_cfg80211
{
    public static void cfg80211_rtw_change_iface(AdapterState padapter, InitChannel initChannel)
    {
        nl80211_iftype type = nl80211_iftype.NL80211_IFTYPE_MONITOR;

        RTW_INFO($"cfg80211_rtw_change_iface type={type}, hw_port:{padapter.hw_port}");

        RTW_INFO("cfg80211_rtw_change_iface call netdev_open");
        if (netdev_open(padapter, initChannel) == false)
        {
            throw new Exception("cfg80211_rtw_change_iface failed netdev_open");
        }

        nl80211_iftype old_type = nl80211_iftype.NL80211_IFTYPE_STATION;
        RTW_INFO("cfg80211_rtw_change_iface old_iftype=%d, new_iftype=%d", old_type, type);

        setopmode_hdl(padapter);
        RTW_INFO($"cfg80211_rtw_change_iface end");
    }

    public static void cfg80211_rtw_set_monitor_channel(AdapterState padapter, InitChannel chandef)
    {
        set_channel_bwmode(padapter, chandef.cur_channel, chandef.cur_ch_offset, chandef.cur_bwmode);
    }

    private static bool netdev_open(AdapterState padapter, InitChannel initChannel)
    {
        bool status;

        RTW_INFO($"_netdev_open , bup={padapter.up}");

        if (padapter.up == false)
        {
            status = rtw_hal_init(padapter, initChannel);
            if (status == false)
            {
                goto netdev_open_error;
            }

            if (status == false)
            {
                RTW_INFO("Initialize driver software resource Failed!\n");
                goto netdev_open_error;
            }

            padapter.up = true;
        }

        return true;

        netdev_open_error:

        padapter.up = false;

        RTW_INFO($"-871x_drv - drv_open fail, bup={padapter.up}");

        return false;
    }
}