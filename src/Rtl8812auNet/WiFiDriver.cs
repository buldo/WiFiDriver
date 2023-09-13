using LibUsbDotNet;
using LibUsbDotNet.LibUsb;

using Rtl8812auNet.LibUsbDotNet;
using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet;

public class WiFiDriver : IDisposable
{
    private readonly UsbContext _usbContext;

    public WiFiDriver()
    {
        _usbContext = new UsbContext();
        _usbContext.StartHandlingEvents();
        _usbContext.SetDebugLevel(LogLevel.Info);
    }

    public List<IUsbDevice> GetUsbDevices()
    {
        return _usbContext.FindAll(SearchPredicate).ToList();
    }

    public Rtl8812aDevice CreateRtlDevice(IUsbDevice usbDevice)
    {
        var usb = new LibUsbRtlUsbDevice((UsbDevice)usbDevice);
        var rtlAdapter = new RtlUsbAdapter(usb);
        return new Rtl8812aDevice(rtlAdapter);
    }

    public void Dispose()
    {
        _usbContext.Dispose();
    }

    private static bool SearchPredicate(IUsbDevice device)
    {
        return Rtl8812aDevices.Devices.Contains(new UsbDeviceVidPid(device.VendorId, device.ProductId));
    }
}