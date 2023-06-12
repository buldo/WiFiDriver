using System.Diagnostics;

namespace WiFiDriver.App.Rtl8812au;

public static class rtw_pwrctrl
{
    public static int rtw_pwr_wakeup(_adapter adapter) => _rtw_pwr_wakeup(adapter, RTW_PWR_STATE_CHK_INTERVAL);

    static int _rtw_pwr_wakeup(_adapter padapter, u32 ips_deffer_ms)
    {
        string caller = "rtw_pwr_wakeup";

        dvobj_priv dvobj = adapter_to_dvobj(padapter);
        pwrctrl_priv pwrpriv = dvobj_to_pwrctl(dvobj);
        int ret = _SUCCESS;
        var start = Stopwatch.StartNew();

        /*RTW_INFO(FUNC_ADPT_FMT "===>\n", FUNC_ADPT_ARG(padapter));*/
        /* for LPS */
        LeaveAllPowerSaveMode(padapter);

        /* IPS still bound with primary adapter */
        //var pmlmepriv = padapter.mlmepriv;

        //if (rtw_time_after(rtw_get_current_time() + rtw_ms_to_systime(ips_deffer_ms), pwrpriv.ips_deny_time))
        //{
        //    pwrpriv.ips_deny_time = rtw_get_current_time() + rtw_ms_to_systime(ips_deffer_ms);
        //}

        //if (pwrpriv.ps_processing)
        //{
        //    RTW_INFO("%s wait ps_processing...\n", __func__);
        //    while (pwrpriv.ps_processing && rtw_get_passing_time_ms(start) <= 3000)
        //        rtw_mdelay_os(10);
        //    if (pwrpriv.ps_processing)
        //        RTW_INFO("%s wait ps_processing timeout\n", __func__);
        //    else
        //        RTW_INFO("%s wait ps_processing done\n", __func__);
        //}

        //if (pwrpriv.bInSuspend)
        //{
        //    RTW_INFO("%s wait bInSuspend...\n", __func__);
        //    while (pwrpriv.bInSuspend
        //           && ((rtw_get_passing_time_ms(start) <= 3000 && !rtw_is_do_late_resume(pwrpriv))
        //               || (rtw_get_passing_time_ms(start) <= 500 && rtw_is_do_late_resume(pwrpriv)))
        //          )
        //        rtw_mdelay_os(10);
        //    if (pwrpriv.bInSuspend)
        //        RTW_INFO("%s wait bInSuspend timeout\n", __func__);
        //    else
        //        RTW_INFO("%s wait bInSuspend done\n", __func__);
        //}

/* System suspend is not allowed to wakeup */
        if ((true == pwrpriv.bInSuspend))
        {
            ret = _FAIL;
            goto exit;
        }


/* TODO: the following checking need to be merged... */
        //if (rtw_is_drv_stopped(padapter)
        //    || !padapter.bup
        //    || !rtw_is_hw_init_completed(padapter)
        //   )
        //{
        //    RTW_INFO("%s: bDriverStopped=%s, bup=%d, hw_init_completed=%u\n"
        //        , caller
        //        , rtw_is_drv_stopped(padapter) ? "True" : "False"
        //        , padapter.bup
        //        , rtw_get_hw_init_completed(padapter));
        //    ret = false;
        //    goto exit;
        //}

        exit:
        //if (rtw_time_after(rtw_get_current_time() + rtw_ms_to_systime(ips_deffer_ms), pwrpriv.ips_deny_time))
        //{
        //    pwrpriv.ips_deny_time = rtw_get_current_time() + rtw_ms_to_systime(ips_deffer_ms);
        //}
/*RTW_INFO(FUNC_ADPT_FMT "<===\n", FUNC_ADPT_ARG(padapter));*/
        return ret;

    }

    /*
 * Description: Leave all power save mode: LPS, FwLPS, IPS if needed.
 * Move code to function by tynli. 2010.03.26.
 *   */
    public static void LeaveAllPowerSaveMode(PADAPTER Adapter)
    {

    dvobj_priv dvobj = adapter_to_dvobj(Adapter);
    u8 enqueue = 0;
    int i;



//	if (rtw_mi_get_assoc_if_num(Adapter))
//{
//    /* connect */
//# ifdef CONFIG_LPS_LCLK
//    enqueue = 1;
//#endif


//#ifdef CONFIG_LPS
//		rtw_lps_ctrl_wk_cmd(Adapter, LPS_CTRL_LEAVE, enqueue);
//#endif

//# ifdef CONFIG_LPS_LCLK
//LPS_Leave_check(Adapter);
//#endif
//	} else
//{
//    if (adapter_to_pwrctl(Adapter).rf_pwrstate == rf_off)
//    {
//# ifdef CONFIG_AUTOSUSPEND
//        if (Adapter.registrypriv.usbss_enable)
//        {
//#if (LINUX_VERSION_CODE >= KERNEL_VERSION(2, 6, 35))
//				usb_disable_autosuspend(adapter_to_dvobj(Adapter).pusbdev);
//#elif (LINUX_VERSION_CODE >= KERNEL_VERSION(2, 6, 22) && LINUX_VERSION_CODE <= KERNEL_VERSION(2, 6, 34))
//				adapter_to_dvobj(Adapter).pusbdev.autosuspend_disabled = Adapter.bDisableAutosuspend;/* autosuspend disabled by the user */
//#endif
//        }
//        else
//#endif
//        {
//#if defined(CONFIG_FWLPS_IN_IPS) || defined(CONFIG_SWLPS_IN_IPS) || (defined(CONFIG_PLATFORM_SPRD) && defined(CONFIG_RTL8188E))
//# ifdef CONFIG_IPS
//				if (_FALSE == ips_leave(Adapter))
//					RTW_INFO("======> ips_leave fail.............\n");
//#endif
//#endif /* CONFIG_SWLPS_IN_IPS || (CONFIG_PLATFORM_SPRD && CONFIG_RTL8188E) */
//        }
//    }
//}

}
}