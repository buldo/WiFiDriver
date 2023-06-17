namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_hal_init
{
    public static void SetMonitorMode(AdapterState adapterState, NDIS_802_11_NETWORK_INFRASTRUCTURE val)
    {
        if (val == NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor)
        {
            rtw_hal_set_msr(adapterState, _HW_STATE_NOLINK_);
            hw_var_set_monitor(adapterState);
        }
    }

    static void hw_var_set_monitor(AdapterState adapterState)
    {
        u32 rcr_bits;
        u16 value_rxfltmap2;

        /* Receive all type */
        rcr_bits = RCR_AAP | RCR_APM | RCR_AM | RCR_AB | RCR_APWRMGT | RCR_ADF | RCR_ACF | RCR_AMF | RCR_APP_PHYST_RXFF;

        /* Append FCS */
        rcr_bits |= RCR_APPFCS;

        //rtw_hal_get_hwreg(adapterState, HW_VAR_RCR, pHalData.rcr_backup);
        hw_var_rcr_config(adapterState, rcr_bits);

        /* Receive all data frames */
        value_rxfltmap2 = 0xFFFF;
        rtw_write16(adapterState, REG_RXFLTMAP2, value_rxfltmap2);
    }
}