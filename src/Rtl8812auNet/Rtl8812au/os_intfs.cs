namespace Rtl8812auNet.Rtl8812au;

public static class os_intfs
{
    public static bool netdev_open(_adapter padapter, InitChannel initChannel)
    {
        pwrctrl_priv pwrctrlpriv = adapter_to_pwrctl(padapter);

        if (pwrctrlpriv.bInSuspend)
        {
            RTW_INFO(" [WARN] OPEN FAILED bInSuspend = false");
            return false;
        }

        return _netdev_open(padapter, initChannel);
    }

    static bool _netdev_open(_adapter padapter, InitChannel initChannel)
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

            if (padapter.napi_state == NAPI_STATE.NAPI_DISABLE)
            {
                padapter.napi_state = NAPI_STATE.NAPI_ENABLE;
            }

            rtw_cfg80211_init_wiphy(padapter);
            padapter.up = true;
        }

        return true;

        netdev_open_error:

        padapter.up = false;

        RTW_INFO($"-871x_drv - drv_open fail, bup={padapter.up}");

        return false;
    }
}