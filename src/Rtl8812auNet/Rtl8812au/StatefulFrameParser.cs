namespace Rtl8812auNet.Rtl8812au;

public class StatefulFrameParser
{
    public List<ParsedRadioPacket> ParsedRadioPacket(byte[] usbTransfer)
    {

        return usb_ops_linux.recvbuf2recvframe(usbTransfer)
            .Select(tuple => new ParsedRadioPacket
            {
                UsbBulkTransfer = usbTransfer,
                Data = tuple.Data,
                Attr = tuple.RxAtrib
            })
            .ToList();
    }
}