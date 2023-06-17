namespace Rtl8812auNet.Rtl8812au;

public static class rtw_cmd
{
    public static bool rtw_setopmode_cmd(_adapter adapter, RTW_CMDF flags)
    {
        bool res = true;

        /* no need to enqueue, do the cmd hdl directly and free cmd parameter */
        if (true != setopmode_hdl(adapter))
        {
            return false;
        }

        return res;
    }
}