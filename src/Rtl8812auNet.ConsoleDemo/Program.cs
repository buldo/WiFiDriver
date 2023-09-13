using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet.ConsoleDemo;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        using var driver = new WiFiDriver();
        var devices = driver.GetUsbDevices();
        var device = devices.First();

        var rtlDevice = driver.CreateRtlDevice(device);

        rtlDevice.Init(PacketProcessor);

        Console.ReadLine();
        Console.WriteLine("End");
    }

    private static Task PacketProcessor(ParsedRadioPacket packet)
    {
        Console.WriteLine($"Packet received. Data len: {packet.Data.Length}");

        return Task.CompletedTask;
    }
}