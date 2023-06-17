namespace Rtl8812auNet.Rtl8812au;

public static class hal_com
{
    public static void hw_var_rcr_config(AdapterState adapterState, u32 rcr)
    {
        rtw_write32(adapterState, REG_RCR, rcr);
    }
}