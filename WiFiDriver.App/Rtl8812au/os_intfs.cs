namespace WiFiDriver.App.Rtl8812au;

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
        pwrctrl_priv pwrctrlpriv = adapter_to_pwrctl(padapter);

        RTW_INFO($"_netdev_open , bup={padapter.up}");

        padapter.netif_up = true;

        if (padapter.up == false)
        {
            status = rtw_hal_init(padapter, initChannel);
            if (status == false)
            {
                return false;
            }

            RTW_INFO("MAC Address = ");
            //rtw_led_control(padapter, LED_CTL_NO_LINK);
            padapter.up = true;
            pwrctrlpriv.bips_processing = false;

        }

        padapter.net_closed = false;
        //_set_timer(&adapter_to_dvobj(padapter).dynamic_chk_timer, 2000);
        RTW_INFO($"-871x_drv - drv_open, bup={padapter.up}");
        return true;
    }
}