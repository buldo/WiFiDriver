using Android.Hardware.Usb;
using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.AndroidDemo.RtlUsb
{
    internal class RtlUsbDevice : IRtlUsbDevice
    {
        private readonly UsbDevice _usbDevice;
        private readonly UsbDeviceConnection _usbDeviceConnection;

        public RtlUsbDevice(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection)
        {
            _usbDevice = usbDevice;
            _usbDeviceConnection = usbDeviceConnection;
        }
        public void InfinityRead()
        {
            var ep = GetInEp();

            var buffer = new byte[4096];
            while (true)
            {
                var readed = _usbDeviceConnection.BulkTransfer(ep, buffer, buffer.Length, 1000);
                if (readed != 0)
                {
                    Console.WriteLine($"BULK {readed}");
                }
            }
        }

        private UsbEndpoint GetInEp()
        {
            var iface = _usbDevice.GetConfiguration(0).GetInterface(0);

            for (int i = 0; i < iface.EndpointCount; i++)
            {
                var ep = iface.GetEndpoint(i);
                if (ep.Direction == UsbAddressing.In && ep.Type == UsbAddressing.XferBulk)
                {
                    return ep;
                }
            }

            throw new Exception("Read EP not found");
        }

        public int Speed { get; } = 3;
        public void WriteBytes(ushort register, Span<byte> data)
        {
            _usbDeviceConnection.ControlTransfer(
                (UsbAddressing)0x40,
                5,
                register,
                0,
                data.ToArray(),
                data.Length,
                500);
        }

        public ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount)
        {
            var buffer = new byte[bytesCount];
            _usbDeviceConnection.ControlTransfer(
                (UsbAddressing)0xC0,
                5,
                register,
                0,
                buffer,
                buffer.Length,
                500);

            return buffer;
        }

        public List<IRtlEndpoint> GetEndpoints()
        {
            var configuration = _usbDevice.GetConfiguration(0);
            var iface = configuration.GetInterface(0);

            var ret = new List<IRtlEndpoint>();
            for (int i = 0; i < iface.EndpointCount; i++)
            {
                var ep = iface.GetEndpoint(i);
                RtlEndpointType type = ep.Type switch
                {
                    UsbAddressing.XferBulk => RtlEndpointType.Bulk,
                    UsbAddressing.XferInterrupt => RtlEndpointType.Interrupt,
                    UsbAddressing.XferIsochronous => RtlEndpointType.Isochronous,
                    _ => RtlEndpointType.Control
                };

                var rtlEp = new RtlEndpoint()
                {
                    Direction = ep.Direction == UsbAddressing.In ? RtlEndpointDirection.In : RtlEndpointDirection.Out,
                    Type = type
                };
                ret.Add(rtlEp);
            }

            return ret;
        }
    }

    public class RtlEndpoint : IRtlEndpoint
    {
        public RtlEndpointType Type { get; set; }
        public RtlEndpointDirection Direction { get; set; }
    }
}
