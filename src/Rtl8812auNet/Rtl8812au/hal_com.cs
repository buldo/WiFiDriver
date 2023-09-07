namespace Rtl8812auNet.Rtl8812au;

public static class hal_com
{
    public static void hw_var_rcr_config(AdapterState adapterState, u32 rcr)
    {
        adapterState.Device.rtw_write32(REG_RCR, rcr);
    }
}