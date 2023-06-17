namespace Rtl8812auNet.Rtl8812au;

public static class rtw_cmd
{
    public static bool rtw_setopmode_cmd(_adapter adapter)
    {
        return setopmode_hdl(adapter);
    }
}