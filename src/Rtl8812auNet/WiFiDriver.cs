using LibUsbDotNet.LibUsb;
using Microsoft.Extensions.Logging;
using Rtl8812auNet.LibUsbDotNet;
using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet;

public class WiFiDriver : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<WiFiDriver> _logger;
    private readonly UsbContext _usbContext;

    public WiFiDriver(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<WiFiDriver>();

        _logger.LogTrace("Creating UsbContext");
        _usbContext = new UsbContext();
        _usbContext.StartHandlingEvents();
        _usbContext.SetDebugLevel(global::LibUsbDotNet.LogLevel.Info);
        _logger.LogTrace("LibUsbDotNet initialised");
    }

    public List<IUsbDevice> GetUsbDevices()
    {
        return _usbContext.FindAll(SearchPredicate).ToList();
    }

    public Rtl8812aDevice CreateRtlDevice(IUsbDevice usbDevice)
    {
        _logger.LogInformation("Creating Rtl8812aDevice");
        var usb = new LibUsbRtlUsbDevice((UsbDevice)usbDevice);
        var rtlAdapter = new RtlUsbAdapter(usb, _loggerFactory.CreateLogger<RtlUsbAdapter>());
        return new Rtl8812aDevice(rtlAdapter, _loggerFactory.CreateLogger<Rtl8812aDevice>());
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