using System.Buffers.Binary;
using LibUsbDotNet.Main;

namespace WiFiDriver.App.Rtl8812au;

public static class rtw_io
{
    private const byte REALTEK_USB_VENQT_READ = 0xC0;
    private const byte REALTEK_USB_VENQT_WRITE = 0x40;

    public static byte rtw_read8(_adapter adapter, ushort addr)
    {
        return ReadBytes(adapter, addr, 1)[0];
    }

    public static UInt32 rtw_read32(_adapter adapter, ushort address)
    {
        var data = ReadBytes(adapter, address, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(data);
    }

    public static UInt16 rtw_read16(_adapter adapter, ushort address)
    {
        var data = ReadBytes(adapter,address, 2);
        return BinaryPrimitives.ReadUInt16LittleEndian(data);
    }

    private static void rtw_write32(_adapter adapter, ushort address, UInt32 value)
    {
        var data = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(data, value);
        WriteBytes(adapter, address, data);
    }

    public static void rtw_write16(_adapter adapter, ushort address, UInt16 value)
    {
        var data = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(data, value);
        WriteBytes(adapter, address, data);
    }

    public static void rtw_write8(_adapter adapter, ushort address, byte value)
    {
        var data = new byte[1]{value};
        WriteBytes(adapter, address, data);
    }

    private static ReadOnlySpan<byte> ReadBytes(_adapter adapter, ushort register, ushort bytesCount)
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
        var bytesReceived = adapter.Device.ControlTransfer(packet, buffer, 0, bytesCount);
        return buffer.AsSpan(0, bytesReceived);
    }

    private static void WriteBytes(_adapter adapter, ushort register, Span<byte> data)
    {
        var packet = new UsbSetupPacket
        {
            RequestType = REALTEK_USB_VENQT_WRITE,
            Request = 5,
            Index = 0,
            Length = (short)data.Length, // ?? is it read length
            Value = (short)register
        };

        var bytesReceived = adapter.Device.ControlTransfer(packet, data.ToArray(), 0, data.Length);
    }
}