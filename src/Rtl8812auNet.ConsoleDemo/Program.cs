using Microsoft.Extensions.Logging;
using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.ConsoleDemo;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var loggerFactory = LoggerFactory.Create(builder =>
            builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());

        using var driver = new WiFiDriver(loggerFactory);
        var devices = driver.GetUsbDevices();
        var device = devices.First();

        var rtlDevice = driver.CreateRtlDevice(device);

        rtlDevice.Init(
            PacketProcessor,
            new()
            {
                ChannelWidth = ChannelWidth.CHANNEL_WIDTH_20,
                ChannelOffset = 0,
                Channel = 140,
                //Channel = 36
            });

        Console.ReadLine();
        Console.WriteLine("End");
    }

    private static Task PacketProcessor(ParsedRadioPacket packet)
    {
        Console.WriteLine($"Packet received. Data len: {packet.Data.Length}");

        return Task.CompletedTask;
    }
}