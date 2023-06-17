namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_hal_init
{
    static bool isMonitor = false;
    public static void SetMonitorMode(PADAPTER Adapter, NDIS_802_11_NETWORK_INFRASTRUCTURE val)
    {
        u8 val8;
        var mode = val;


        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        //if (isMonitor == true)
        //{
        //    //throw new NotImplementedException();
        //    ///* reset RCR from backup */
        //    //rtw_hal_set_hwreg(Adapter, HW_VAR_RCR, (u8*)&pHalData.rcr_backup);
        //    //rtw_hal_rcr_set_chk_bssid(Adapter, MLME_ACTION_NONE);
        //    isMonitor = false;
        //}

        if (mode == NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor)
        {
            isMonitor = true;
            /* set net_type */
            Set_MSR(Adapter, _HW_STATE_NOLINK_);
            hw_var_set_monitor(Adapter);
        }
    }

    static void hw_var_set_monitor(PADAPTER Adapter)
    {
        u32 rcr_bits;
        u16 value_rxfltmap2;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        /* Receive all type */
        rcr_bits = RCR_AAP | RCR_APM | RCR_AM | RCR_AB | RCR_APWRMGT | RCR_ADF | RCR_ACF | RCR_AMF | RCR_APP_PHYST_RXFF;

        /* Append FCS */
        rcr_bits |= RCR_APPFCS;

        //rtw_hal_get_hwreg(Adapter, HW_VAR_RCR, pHalData.rcr_backup);
        hw_var_rcr_config(Adapter, rcr_bits);

        /* Receive all data frames */
        value_rxfltmap2 = 0xFFFF;
        rtw_write16(Adapter, REG_RXFLTMAP2, value_rxfltmap2);
}
}