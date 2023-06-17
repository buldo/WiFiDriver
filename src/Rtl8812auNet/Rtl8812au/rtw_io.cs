using System.Buffers.Binary;

namespace Rtl8812auNet.Rtl8812au;

public static class rtw_io
{
    public static byte rtw_read8(_adapter adapter, ushort addr)
    {
        return ReadBytes(adapter, addr, 1)[0];
    }

    public static byte Read8(_adapter adapter, ushort addr)
    {
        return rtw_read8(adapter, addr);
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

    public static void rtw_write32(_adapter adapter, ushort address, UInt32 value)
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

    public static void Write8(_adapter adapter, ushort address, byte value)
    {
        rtw_write8(adapter, address, value);
    }

    private static ReadOnlySpan<byte> ReadBytes(_adapter adapter, ushort register, ushort bytesCount)
    {
        return adapter.Device.ReadBytes(register, bytesCount);
    }

    public static void WriteBytes(_adapter adapter, ushort register, Span<byte> data)
    {
        adapter.Device.WriteBytes(register, data);
    }
}