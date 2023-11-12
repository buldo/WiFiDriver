using LibUsbDotNet.LibUsb;
using Microsoft.Extensions.Logging;

using Rtl8812auNet.Abstractions;
using Rtl8812auNet.LibUsbDotNet;
using Rtl8812auNet.Rtl8812au;
using Rtl8812auNet.Rtl8812au.Modules;

namespace Rtl8812auNet;

public class WiFiDriver : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly bool _isLibUsbMode;
    private readonly ILogger<WiFiDriver> _logger;
    private readonly UsbContext? _usbContext;

    public WiFiDriver(
        ILoggerFactory loggerFactory,
        bool isLibUsbMode)
    {
        _loggerFactory = loggerFactory;
        _isLibUsbMode = isLibUsbMode;
        _logger = loggerFactory.CreateLogger<WiFiDriver>();

        if (isLibUsbMode)
        {
            _logger.LogTrace("Creating UsbContext");
            _usbContext = new UsbContext();
            _usbContext.StartHandlingEvents();
            _usbContext.SetDebugLevel(global::LibUsbDotNet.LogLevel.Info);
            _logger.LogTrace("LibUsbDotNet initialised");
        }
    }

    public List<IUsbDevice> GetUsbDevices()
    {
        if (_usbContext == null)
        {
            throw new Exception("Not in libusb mode");
        }

        return _usbContext.FindAll(SearchPredicate).ToList();
    }

    public Rtl8812aDevice CreateRtlDevice(IUsbDevice usbDevice)
    {
        _logger.LogInformation("Creating Rtl8812aDevice from IUsbDevice(LibUSB)");
        var usb = new LibUsbRtlUsbDevice(
            (UsbDevice)usbDevice,
            _loggerFactory.CreateLogger<LibUsbRtlUsbDevice>());
        var rtlAdapter = new RtlUsbAdapter(usb, _loggerFactory.CreateLogger<RtlUsbAdapter>());
        return new Rtl8812aDevice(
            rtlAdapter,
            _loggerFactory.CreateLogger<Rtl8812aDevice>(),
            _loggerFactory.CreateLogger<EepromManager>(),
            _loggerFactory.CreateLogger<HalModule>(),
            _loggerFactory.CreateLogger<FrameParser>(),
            _loggerFactory.CreateLogger<FirmwareManager>());
    }

    public Rtl8812aDevice CreateRtlDevice(IRtlUsbDevice usbDevice)
    {
        _logger.LogInformation("Creating Rtl8812aDevice from IRtlUsbDevice");
        var rtlAdapter = new RtlUsbAdapter(usbDevice, _loggerFactory.CreateLogger<RtlUsbAdapter>());
        return new Rtl8812aDevice(
            rtlAdapter,
            _loggerFactory.CreateLogger<Rtl8812aDevice>(),
            _loggerFactory.CreateLogger<EepromManager>(),
            _loggerFactory.CreateLogger<HalModule>(),
            _loggerFactory.CreateLogger<FrameParser>(),
            _loggerFactory.CreateLogger<FirmwareManager>());
    }

    public void Dispose()
    {
        _usbContext?.Dispose();
    }

    private static bool SearchPredicate(IUsbDevice device)
    {
        return Rtl8812aDevices.Devices.Contains(new UsbDeviceVidPid(device.VendorId, device.ProductId));
    }
}