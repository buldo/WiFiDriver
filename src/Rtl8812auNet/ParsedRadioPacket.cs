using Rtl8812auNet.Rtl8812au.Models;

namespace Rtl8812auNet;

public class ParsedRadioPacket
{
    public byte[] UsbBulkTransfer { get; set; }

    public byte[] Data { get; set; } = Array.Empty<byte>();
    public rx_pkt_attrib Attr { get; set; }
}