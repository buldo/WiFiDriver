namespace Rtl8812auNet.Abstractions;

public interface IRtlEndpoint
{
    public RtlEndpointType Type { get; }

    public RtlEndpointDirection Direction { get; }

    public int GetUsbEndpointNum();
}