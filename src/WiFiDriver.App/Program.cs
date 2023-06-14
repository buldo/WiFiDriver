﻿// See https://aka.ms/new-console-template for more information
using LibUsbDotNet.LibUsb;
using WiFiDriver.App.Rtl8812au;

namespace WiFiDriver.App;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        using var context = new UsbContext();

        var devices = context.FindAll(SearchPredicate);

        var device = devices.First();

        var rtlDevice = new Rtl8812aDevice((UsbDevice)device);

        rtlDevice.Init();

        Console.ReadLine();
        Console.WriteLine("End");
    }

    private static bool SearchPredicate(IUsbDevice device)
    {
        return Rtl8812aDevices.Devices.Contains(new UsbDeviceVidPid(device.VendorId, device.ProductId));
    }
}