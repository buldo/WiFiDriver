namespace Rtl8812auNet.Abstractions;

[Flags]
public enum RtlEndpointDirection : byte
{
    /// <summary>
    ///  In: device-to-host
    /// </summary>
    In = 0x80,

    /// <summary>
    ///  Out: host-to-device
    /// </summary>
    Out = 0,

}