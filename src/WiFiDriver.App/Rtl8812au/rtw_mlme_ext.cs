namespace WiFiDriver.App.Rtl8812au;

public static class rtw_mlme_ext
{
    public static bool setopmode_hdl(_adapter padapter)
    {
        SetMonitorMode(padapter, NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor);
        return true;
    }

    public static bool init_hw_mlme_ext(_adapter padapter, InitChannel pmlmeext)
    {

        //struct mlme_ext_priv *pmlmeext = &padapter.mlmeextpriv;
        HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        bool rx_bar_enble = true;

        /*
         * Sync driver status and hardware setting
         */

        /* Modify to make sure first time change channel(band) would be done properly */
        pHalData.current_channel = 0;
        pHalData.current_channel_bw = channel_width.CHANNEL_WIDTH_MAX;
        pHalData.current_band_type = BAND_TYPE.BAND_MAX;

        /* set_opmode_cmd(padapter, infra_client_with_mlme); */ /* removed */
        Set_HW_VAR_ENABLE_RX_BAR(padapter, rx_bar_enble);
        set_channel_bwmode(padapter, pmlmeext.cur_channel, pmlmeext.cur_ch_offset, pmlmeext.cur_bwmode);

        return true;
    }

    private static void Set_HW_VAR_ENABLE_RX_BAR(_adapter adapter, bool val)
    {
        if (val == true)
        {
            /* enable RX BAR */
            u32 val16 = rtw_read16(adapter, REG_RXFLTMAP1);

            val16 |= BIT8;
            rtw_write16(adapter, REG_RXFLTMAP1, (u16)val16);
        }
        else
        {
            /* disable RX BAR */
            u32 val16 = rtw_read16(adapter, REG_RXFLTMAP1);

            val16 &= NotBIT8;
            rtw_write16(adapter, REG_RXFLTMAP1, (u16)val16);
        }
        RTW_INFO($"[HW_VAR_ENABLE_RX_BAR] 0x{REG_RXFLTMAP1:X4}=0x{rtw_read16(adapter, REG_RXFLTMAP1):X4}");
    }
}