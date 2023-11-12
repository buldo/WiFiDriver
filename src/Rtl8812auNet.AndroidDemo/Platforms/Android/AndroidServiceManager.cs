using Android.Hardware.Usb;

namespace Rtl8812auNet.AndroidDemo.Platforms.Android;

public static class AndroidServiceManager
{
    public static MainActivity MainActivity { get; set; }

    public static bool IsRunning { get; set; }
    public static UsbDevice Device { get; set; }
    public static UsbDeviceConnection Connection { get; set; }

    public static void StartWfbService()
    {
        if (MainActivity == null)
        {
            return;
        }

        MainActivity.StartService();
    }

    public static void StopWfbService()
    {
        if (MainActivity == null)
        {
            return;
        }
        MainActivity.StopService();
        IsRunning = false;
    }
}