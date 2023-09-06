namespace Rtl8812auNet.Rtl8812au;

public class ParsedRadioPacket
{
    public byte[] UsbBulkTransfer { get; set; }

    public byte[] Data { get; set; } = Array.Empty<byte>();
    public rx_pkt_attrib Attr { get; set; }
}