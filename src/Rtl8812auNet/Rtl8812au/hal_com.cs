namespace Rtl8812auNet.Rtl8812au;

public static class hal_com
{
    public static void rtw_hal_set_msr(AdapterState adapterState, u8 net_type)
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

    public static void hw_var_rcr_config(AdapterState adapterState, u32 rcr)
    {
        rtw_write32(adapterState, REG_RCR, rcr);
    }
}