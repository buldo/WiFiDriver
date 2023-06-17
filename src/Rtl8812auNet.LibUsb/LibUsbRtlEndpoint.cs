using LibUsbDotNet.Info;

using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.LibUsb;

public class LibUsbRtlEndpoint : IRtlEndpoint
{
    private const byte USB_ENDPOINT_NUMBER_MASK = 0x0f;

    private readonly UsbEndpointInfo _usbEndpointInfo;

    public LibUsbRtlEndpoint(UsbEndpointInfo usbEndpointInfo)
    {
        _usbEndpointInfo = usbEndpointInfo;
    }

    public RtlEndpointType Type => (RtlEndpointType)(_usbEndpointInfo.Attributes & 0x3);

    public RtlEndpointDirection Direction => (RtlEndpointDirection)(_usbEndpointInfo.EndpointAddress & (0b1000_0000));
    public int GetUsbEndpointNum()
    {
        return _usbEndpointInfo.EndpointAddress & USB_ENDPOINT_NUMBER_MASK;
    }
}