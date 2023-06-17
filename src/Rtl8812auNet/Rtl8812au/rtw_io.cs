using System.Buffers.Binary;

namespace Rtl8812auNet.Rtl8812au;

public static class rtw_io
{
    public static byte rtw_read8(AdapterState adapterState, ushort addr)
    {
        return ReadBytes(adapterState, addr, 1)[0];
    }

    public static byte Read8(AdapterState adapterState, ushort addr)
    {
        return rtw_read8(adapterState, addr);
    }

    public static UInt32 rtw_read32(AdapterState adapterState, ushort address)
    {
        var data = ReadBytes(adapterState, address, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(data);
    }

    public static UInt16 rtw_read16(AdapterState adapterState, ushort address)
    {
        var data = ReadBytes(adapterState,address, 2);
        return BinaryPrimitives.ReadUInt16LittleEndian(data);
    }

    public static void rtw_write32(AdapterState adapterState, ushort address, UInt32 value)
    {
        var data = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(data, value);
        WriteBytes(adapterState, address, data);
    }

    public static void rtw_write16(AdapterState adapterState, ushort address, UInt16 value)
    {
        var data = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(data, value);
        WriteBytes(adapterState, address, data);
    }

    public static void rtw_write8(AdapterState adapterState, ushort address, byte value)
    {
        var data = new byte[1]{value};
        WriteBytes(adapterState, address, data);
    }

    public static void Write8(AdapterState adapterState, ushort address, byte value)
    {
        rtw_write8(adapterState, address, value);
    }

    private static ReadOnlySpan<byte> ReadBytes(AdapterState adapterState, ushort register, ushort bytesCount)
    {
        return adapterState.Device.ReadBytes(register, bytesCount);
    }

    public static void WriteBytes(AdapterState adapterState, ushort register, Span<byte> data)
    {
        adapterState.Device.WriteBytes(register, data);
    }
}