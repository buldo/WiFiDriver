using System.Diagnostics;

namespace Rtl8812auNet.Rtl8812au;

public static class rtw_pwrctrl
{
    public static int rtw_pwr_wakeup(_adapter adapter) => _rtw_pwr_wakeup(adapter);

    static int _rtw_pwr_wakeup(_adapter padapter)
    {
        dvobj_priv dvobj = adapter_to_dvobj(padapter);
        int ret = _SUCCESS;
        LeaveAllPowerSaveMode(padapter);
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