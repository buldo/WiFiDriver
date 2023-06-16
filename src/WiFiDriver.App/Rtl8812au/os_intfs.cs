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
        //wireless_dev wdev = padapter.rtw_wdev;
        pwrctrl_priv pwrctrlpriv = adapter_to_pwrctl(padapter);

        RTW_INFO($"_netdev_open , bup={padapter.up}");
        padapter.netif_up = true;

        if (padapter.up == false)
        {
            status = rtw_hal_init(padapter, initChannel);
            if (status == false)
            {
                goto netdev_open_error;
            }

            //RTW_INFO($"MAC Address = {pnetdev.dev_addr}");

            //status = rtw_start_drv_threads(padapter);
            if (status == false)
            {
                RTW_INFO("Initialize driver software resource Failed!\n");
                goto netdev_open_error;
            }

            if (padapter.napi_state == NAPI_STATE.NAPI_DISABLE)
            {
               // napi_enable(padapter.napi);
                padapter.napi_state = NAPI_STATE.NAPI_ENABLE;
            }

            // rtw_intf_start(padapter); // looks like only READ from RECV_BULK_IN_ADDR 0x80

            rtw_cfg80211_init_wiphy(padapter);
            //rtw_cfg80211_init_wdev_data(padapter);
            //rtw_led_control(padapter, LED_CTL_NO_LINK);
            padapter.up = true;
            pwrctrlpriv.bips_processing = false;
        }
        padapter.net_closed = false;

        //_set_timer(adapter_to_dvobj(padapter).dynamic_chk_timer, 2000);

        //rtw_netif_carrier_on(pnetdev); /* call this func when rtw_joinbss_event_callback return success */
        //rtw_netif_wake_queue(pnetdev);
        //netdev_br_init(pnetdev);

        netdev_open_normal_process:

        //RTW_INFO("-871x_drv - drv_open, bup=%d\n", padapter->bup);

        return true;

        netdev_open_error:

        padapter.up = false;

        //if (padapter.napi_state == NAPI_STATE.NAPI_ENABLE)
        //{
        //    napi_disable(padapter.napi);
        //    padapter.napi_state = NAPI_STATE.NAPI_DISABLE;
        //}

        //rtw_netif_carrier_off(pnetdev);
        //rtw_netif_stop_queue(pnetdev);

        RTW_INFO($"-871x_drv - drv_open fail, bup={padapter.up}");

        return false;
    }
}