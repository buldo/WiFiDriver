namespace Rtl8812auNet.Rtl8812au;

public static class rtw_mlme_ext
{
    public static bool setopmode_hdl(AdapterState padapter)
    {
        SetMonitorMode(padapter, NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor);
        return true;
    }

    private static void SetMonitorMode(AdapterState adapterState, NDIS_802_11_NETWORK_INFRASTRUCTURE val)
    {
        if (val == NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor)
        {
            rtw_hal_set_msr(adapterState, _HW_STATE_NOLINK_);
            hw_var_set_monitor(adapterState);
        }
    }

    private static void rtw_hal_set_msr(AdapterState adapterState, u8 net_type)
    {
        switch (adapterState.HwPort)
        {
            case HwPort.HW_PORT0:
                /*REG_CR - BIT[17:16]-Network Type for port 0*/
                var val8 = (byte)(rtw_read8(adapterState, MSR) & 0x0C);
                val8 |= net_type;
                rtw_write8(adapterState, MSR, val8);
                break;
            //case HwPort.HW_PORT1:
            //    /*REG_CR - BIT[19:18]-Network Type for port 1*/
            //    val8 = rtw_read8(adapterState, MSR) & 0x03;
            //    val8 |= net_type << 2;
            //    rtw_write8(adapterState, MSR, val8);
            //    break;

            default:
                throw new NotImplementedException();
                break;
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

    public static bool init_hw_mlme_ext(AdapterState padapter, InitChannel pmlmeext)
    {

        //struct mlme_ext_priv *pmlmeext = &padapter.mlmeextpriv;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        bool rx_bar_enble = true;

        /*
         * Sync driver status and hardware setting
         */

        /* Modify to make sure first time change channel(band) would be done properly */
        pHalData.current_channel = 0;
        pHalData.current_channel_bw = ChannelWidth.CHANNEL_WIDTH_MAX;
        pHalData.current_band_type = BandType.BAND_MAX;

        /* set_opmode_cmd(padapter, infra_client_with_mlme); */ /* removed */
        Set_HW_VAR_ENABLE_RX_BAR(padapter, rx_bar_enble);
        set_channel_bwmode(padapter, pmlmeext.cur_channel, pmlmeext.cur_ch_offset, pmlmeext.cur_bwmode);

        return true;
    }

    private static void Set_HW_VAR_ENABLE_RX_BAR(AdapterState adapterState, bool val)
    {
        if (val == true)
        {
            /* enable RX BAR */
            u32 val16 = rtw_read16(adapterState, REG_RXFLTMAP1);

            val16 |= BIT8;
            rtw_write16(adapterState, REG_RXFLTMAP1, (u16)val16);
        }
        else
        {
            /* disable RX BAR */
            u32 val16 = rtw_read16(adapterState, REG_RXFLTMAP1);

            val16 &= NotBIT8;
            rtw_write16(adapterState, REG_RXFLTMAP1, (u16)val16);
        }
        RTW_INFO($"[HW_VAR_ENABLE_RX_BAR] 0x{REG_RXFLTMAP1:X4}=0x{rtw_read16(adapterState, REG_RXFLTMAP1):X4}");
    }
}