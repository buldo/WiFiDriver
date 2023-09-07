using System.Buffers.Binary;
using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet;

public class RtlUsbAdapter
{
    private readonly IRtlUsbDevice _usbDevice;

    public RtlUsbAdapter(IRtlUsbDevice usbDevice)
    {
        _usbDevice = usbDevice;
    }

    public IRtlUsbDevice UsbDevice => _usbDevice;

    public byte rtw_read8(ushort addr)
    {
        return ReadBytes(addr, 1)[0];
    }

    public byte Read8(ushort addr)
    {
        return rtw_read8(addr);
    }

    public UInt32 rtw_read32(ushort address)
    {
        var data = ReadBytes(address, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(data);
    }

    public UInt16 rtw_read16(ushort address)
    {
        var data = ReadBytes(address, 2);
        return BinaryPrimitives.ReadUInt16LittleEndian(data);
    }

    public void rtw_write32(ushort address, UInt32 value)
    {
        var data = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(data, value);
        WriteBytes(address, data);
    }

    public void rtw_write16(ushort address, UInt16 value)
    {
        var data = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(data, value);
        WriteBytes(address, data);
    }

    public void rtw_write8(ushort address, byte value)
    {
        var data = new byte[1] { value };
        WriteBytes(address, data);
    }

    public void Write8(ushort address, byte value)
    {
        rtw_write8(address, value);
    }

    private ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount)
    {
        return _usbDevice.ReadBytes(register, bytesCount);
    }

    public void WriteBytes(ushort register, Span<byte> data)
    {
        _usbDevice.WriteBytes(register, data);
    }
}