using Android.App;
using Android.Content;
using Android.OS;
using Bld.WlanUtils;
using Java.Util.Logging;

using Microsoft.Extensions.Logging;
using Rtl8812auNet.AndroidDemo.RtlUsb;
using Rtl8812auNet.Rtl8812au;
using Rtl8812auNet.Rtl8812au.Models;
using ChannelWidth = Rtl8812auNet.Rtl8812au.Enumerations.ChannelWidth;

namespace Rtl8812auNet.AndroidDemo.Platforms.Android;

[Service]
public class DriverBackgroundService : Service
{
    private WiFiDriver _driver;
    private Rtl8812aDevice _device;
    private ILogger<DriverBackgroundService> _logger;
    private WlanChannel _channel = Channels.Ch036;


    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        var loggerFactory = IPlatformApplication.Current.Services.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<DriverBackgroundService>();
        _driver = new WiFiDriver(loggerFactory);
        _device = _driver.CreateRtlDevice(new RtlUsbDevice(AndroidServiceManager.Device, AndroidServiceManager.Connection, loggerFactory.CreateLogger<RtlUsbDevice>()));
        _device.Init(PacketProcessor, CreateCurrentChannel());
        _device.SetMonitorChannel(CreateCurrentChannel());

        return StartCommandResult.Sticky;
    }

    private Task PacketProcessor(ParsedRadioPacket arg)
    {
        _logger.LogDebug("Received");
        return Task.CompletedTask;
    }

    private SelectedChannel CreateCurrentChannel()
    {
        return new SelectedChannel
        {
            Channel = (byte)_channel.ChannelNumber,
            ChannelOffset = 0,
            ChannelWidth = ChannelWidth.CHANNEL_WIDTH_20
        };
    }
}