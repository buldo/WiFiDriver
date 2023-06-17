namespace Rtl8812auNet.Abstractions;

[Flags]
public enum RtlEndpointType : byte
{
    /// <summary>
    /// Control endpoint type.
    /// </summary>
    Control,

    /// <summary>
    /// Isochronous endpoint type.
    /// </summary>
    Isochronous,

    /// <summary>
    /// Bulk endpoint type.
    /// </summary>
    Bulk,

    /// <summary>
    /// Interrupt endpoint type.
    /// </summary>
    Interrupt
}