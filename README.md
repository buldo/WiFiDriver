# WiFiDriver
RTL8812AU userspace driver.  
Yes. It's real. Userspace wifi driver with monitoring support.

# How to use
1. Install WinUSB driver for you adapter via [Zadig](https://zadig.akeo.ie)  
2. Install [nuget package](https://www.nuget.org/packages/Bld.Rtl8812auNet) to your project
3. Write some code:
```csharp
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
```

4. Use (Packet.Net)[https://github.com/dotpcap/packetnet] for parsing frames data(not tested but have to work)