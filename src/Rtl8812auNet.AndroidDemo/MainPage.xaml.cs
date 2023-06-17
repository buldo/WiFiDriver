using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Rtl8812auNet.AndroidDemo.RtlUsb;
using Rtl8812auNet.Rtl8812au;
using Application = Android.App.Application;

namespace Rtl8812auNet.AndroidDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            var context = Android.App.Application.Context;
            var usbManager = (UsbManager)context.GetSystemService(Android.Content.Context.UsbService);
            var dev = usbManager.DeviceList.Single(pair => pair.Value.ManufacturerName == "Realtek").Value;
            var pi = PendingIntent.GetBroadcast(
                Application.Context,
                0,
                new Intent(Context.UsbService),
                PendingIntentFlags.Immutable);
            usbManager.RequestPermission(dev, pi);

            var conn = usbManager.OpenDevice(dev);
            var rtlUsbDevice = new RtlUsbDevice(dev, conn);
            var rtl = new Rtl8812aDevice(rtlUsbDevice);
            rtl.Init();

            Console.WriteLine("READY");
        }
    }
}