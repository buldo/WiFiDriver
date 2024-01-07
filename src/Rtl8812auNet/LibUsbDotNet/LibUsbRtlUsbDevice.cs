#if WINDOWS
#nullable enable
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using Microsoft.Extensions.Logging;
using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.LibUsbDotNet;

public class LibUsbRtlUsbDevice : IRtlUsbDevice
{
    private const byte REALTEK_USB_VENQT_READ = 0xC0;
    private const byte REALTEK_USB_VENQT_WRITE = 0x40;

    private readonly UsbDevice _usbDevice;
    private readonly ILogger<LibUsbRtlUsbDevice> _logger;
    private readonly UsbEndpointReader _reader;

    private BulkDataHandler? _bulkDataHandler;

    public LibUsbRtlUsbDevice(
        UsbDevice usbDevice,
        ILogger<LibUsbRtlUsbDevice> logger)
    {
        _usbDevice = usbDevice;
        _logger = logger;
        _usbDevice.Open();
        _usbDevice.SetConfiguration(1);
        _usbDevice.ClaimInterface(0);

        _reader = _usbDevice.OpenEndpointReader(GetInEp());
    }

    public void SetBulkDataHandler(BulkDataHandler handler)
    {
        _bulkDataHandler = handler;
    }

    public void InfinityRead()
    {
        var readBuffer = new byte[8192 + 1024];
        while (true)
        {
            try
            {
                var result = _reader.Read(readBuffer, 5000, out var len);
                if (result == Error.NotFound)
                {

                }
                else if (result != Error.Success)
                {
                    _logger.LogError("BULK read ERR {result}", result);
                }

                if (len != 0)
                {
                    _bulkDataHandler?.Invoke(readBuffer.AsSpan(0, len));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "_reader.Read error");
            }
        }
    }

    public int Speed => _usbDevice.Speed;

    public ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount)
    {
        var packet = new UsbSetupPacket
        {
            RequestType = REALTEK_USB_VENQT_READ,
            Request = 5,
            Index = 0,
            Length = (short)bytesCount,
            Value = (short)register
        };

        var buffer = new byte[bytesCount];
        var bytesReceived = _usbDevice.ControlTransfer(packet, buffer, 0, bytesCount);
        return buffer.AsSpan(0, bytesReceived);
    }

    public List<IRtlEndpoint> GetEndpoints()
    {
        return _usbDevice
            .Configs[0]
            .Interfaces[0]
            .Endpoints
            .Select(e => (IRtlEndpoint)(new LibUsbRtlEndpoint(e)))
            .ToList();
    }

    public void WriteBytes(ushort register, Span<byte> data)
    {
        var packet = new UsbSetupPacket
        {
            RequestType = REALTEK_USB_VENQT_WRITE,
            Request = 5,
            Index = 0,
            Length = (short)data.Length, // ?? is it read length
            Value = (short)register
        };

        var bytesReceived = _usbDevice.ControlTransfer(packet, data.ToArray(), 0, data.Length);
    }

    private ReadEndpointID GetInEp()
    {
        foreach (var endpoint in _usbDevice.Configs[0].Interfaces[0].Endpoints)
        {
            var type = (EndpointType)(endpoint.Attributes & 0x3);
            var direction = (EndpointDirection)(endpoint.EndpointAddress & (0b1000_0000));
            if (type == EndpointType.Bulk && direction == EndpointDirection.In)
            {

                return (ReadEndpointID)endpoint.EndpointAddress;
            }
        }

        throw new Exception("Read EP not found");
    }
}

#endif