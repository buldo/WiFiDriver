using System.Net;

using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Rtl8812auNet.AndroidDemo.Platforms.Android;
using Rtl8812auNet.AndroidDemo.RtlUsb;
using Rtl8812auNet.Rtl8812au;
using Application = Android.App.Application;

namespace Rtl8812auNet.AndroidDemo;

public partial class MainPage : ContentPage
{
    private static string ACTION_USB_PERMISSION = "zone.bld.receiverapp.USB_PERMISSION";
    private readonly MyBroadcastReceiver _broadcastReceiver;

    public MainPage()
    {
        InitializeComponent();
        _broadcastReceiver = new MyBroadcastReceiver(this);
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        var context = Android.App.Application.Context;
        var usbManager = (UsbManager)context.GetSystemService(Android.Content.Context.UsbService);

        var (_, device) = usbManager.DeviceList.FirstOrDefault(pair => pair.Value.ManufacturerName == "Realtek");
        if (device != null)
        {
            if (usbManager.HasPermission(device))
            {
                StatusLabel.Text = $"Device found:{device.DeviceName}" + System.Environment.NewLine + "Starting...";
                StartService(device);
            }
            else
            {
                var pi = PendingIntent.GetBroadcast(
                    Android.App.Application.Context,
                    0,
                    new Intent(ACTION_USB_PERMISSION),
                    PendingIntentFlags.Immutable);
                _broadcastReceiver.Dev = device;
                var filter = new IntentFilter(ACTION_USB_PERMISSION);
                context.RegisterReceiver(_broadcastReceiver, filter);

                StatusLabel.Text = $"Device found:{device.DeviceName}" + System.Environment.NewLine + "Requesting permissions...";
                usbManager.RequestPermission(device, pi);
            }
        }
        else
        {
            StatusLabel.Text = "No RTL8812AU device found";
        }

        Console.WriteLine("READY");
    }

    private void StartService(UsbDevice device)
    {
        StatusLabel.Text = $"Device found:{device.DeviceName}" + System.Environment.NewLine + "Starting...";
        AndroidServiceManager.Device = device;
        var context = Android.App.Application.Context;
        var usbManager = (UsbManager)context.GetSystemService(Android.Content.Context.UsbService);
        AndroidServiceManager.Connection = usbManager.OpenDevice(device);
        AndroidServiceManager.StartWfbService();
    }

    public class MyBroadcastReceiver : BroadcastReceiver
    {
        private readonly MainPage _parent;

        public MyBroadcastReceiver(MainPage parent)
        {
            _parent = parent;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            _parent.StatusLabel.Text = $"Device found:{Dev.DeviceName}" + System.Environment.NewLine + "Starting...";
            _parent.StartService(Dev);
        }

        public UsbDevice Dev { get; set; }
    }
}