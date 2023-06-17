namespace Rtl8812auNet.Rtl8812au;

public static class os_intfs
{
    public static bool netdev_open(_adapter padapter, InitChannel initChannel)
    {
        bool status;

        RTW_INFO($"_netdev_open , bup={padapter.up}");

        if (padapter.up == false)
        {
            status = rtw_hal_init(padapter, initChannel);
            if (status == false)
            {
                goto netdev_open_error;
            }

            if (status == false)
            {
                RTW_INFO("Initialize driver software resource Failed!\n");
                goto netdev_open_error;
            }

            padapter.up = true;
        }

        return true;

        netdev_open_error:

        padapter.up = false;

        RTW_INFO($"-871x_drv - drv_open fail, bup={padapter.up}");

        return false;
    }
}