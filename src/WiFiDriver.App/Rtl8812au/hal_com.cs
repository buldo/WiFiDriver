namespace WiFiDriver.App.Rtl8812au;

public static class hal_com
{
    public static void rtw_hal_set_msr(_adapter adapter, u8 net_type)
    {
        u8 val8 = 0;

        switch (adapter.hw_port)
        {
            case hw_port.HW_PORT0:
                /*REG_CR - BIT[17:16]-Network Type for port 0*/
                val8 = (byte)(rtw_read8(adapter, MSR) & 0x0C);
                val8 |= net_type;
                rtw_write8(adapter, MSR, val8);
                break;
            //case hw_port.HW_PORT1:
            //    /*REG_CR - BIT[19:18]-Network Type for port 1*/
            //    val8 = rtw_read8(adapter, MSR) & 0x03;
            //    val8 |= net_type << 2;
            //    rtw_write8(adapter, MSR, val8);
            //    break;

            default:
                throw new NotImplementedException();
                break;
        }
    }

    public static void hw_var_rcr_config(_adapter adapter, u32 rcr)
    {
        rtw_write32(adapter, REG_RCR, rcr);
        GET_HAL_DATA(adapter).ReceiveConfig = rcr;
    }

    public static u32 hw_var_rcr_get(_adapter adapter)
    {
        var v32 = rtw_read32(adapter, REG_RCR);
        GET_HAL_DATA(adapter).ReceiveConfig = v32;
        return v32;
    }

    public static RF_TX_NUM phy_get_current_tx_num(PADAPTER        pAdapter, MGN_RATE Rate)
    {
        RF_TX_NUM tx_num = RF_TX_NUM.RF_1TX;

        if (IS_2T_RATE(Rate))
        {
            tx_num = RF_TX_NUM.RF_2TX;
        }
        else if (IS_3T_RATE(Rate))
        {
            tx_num = RF_TX_NUM.RF_3TX;
        }
        else
        {
            throw new Exception();
        }

        return tx_num;
    }

    static bool IS_2T_RATE(MGN_RATE _rate)   =>(IS_HT2SS_RATE((_rate)) || IS_VHT2SS_RATE((_rate)));
    static bool IS_3T_RATE(MGN_RATE _rate) =>  (IS_HT3SS_RATE((_rate)) || IS_VHT3SS_RATE((_rate)));

    static bool IS_HT3SS_RATE(MGN_RATE _rate) => ((_rate) >= MGN_RATE.MGN_MCS16 && (_rate) <= MGN_RATE.MGN_MCS23);
    static bool IS_HT2SS_RATE(MGN_RATE _rate) => ((_rate) >= MGN_RATE.MGN_MCS8 && (_rate) <= MGN_RATE.MGN_MCS15);

    static bool IS_VHT2SS_RATE(MGN_RATE _rate) => (_rate) >= MGN_RATE.MGN_VHT2SS_MCS0 && (_rate <= MGN_RATE.MGN_VHT2SS_MCS9);
    static bool IS_VHT3SS_RATE(MGN_RATE _rate) => (_rate) >= MGN_RATE.MGN_VHT3SS_MCS0 && (_rate <= MGN_RATE.MGN_VHT3SS_MCS9);
}