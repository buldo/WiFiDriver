using System.Buffers.Binary;
using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet;

public class RtlUsbAdapter
{
    private readonly IRtlUsbDevice _usbDevice;

    public RtlUsbAdapter(IRtlUsbDevice usbDevice)
    {
        _usbDevice = usbDevice;
    }

    public IRtlUsbDevice UsbDevice => _usbDevice;

    public void efuse_OneByteRead(UInt16 addr, out byte data)
    {
        /* -----------------e-fuse reg ctrl --------------------------------- */
        /* address			 */
        var addressBytes = new byte[2];
        BinaryPrimitives.TryWriteUInt16LittleEndian(addressBytes, addr);
        rtw_write8(EFUSE_CTRL + 1, addressBytes[0]);
        var tmpRead = rtw_read8(EFUSE_CTRL + 2);
        var secondAddr = (addressBytes[1] & 0x03) | (tmpRead & 0xFC);
        rtw_write8(EFUSE_CTRL + (2), (byte)secondAddr);

        /* Write8(pAdapterState, EFUSE_CTRL+3,  0x72); */
        /* read cmd	 */
        /* Write bit 32 0 */
        var readbyte = rtw_read8(EFUSE_CTRL + 3);
        rtw_write8(EFUSE_CTRL + 3, (byte)(readbyte & 0x7f));

        UInt32 tmpidx = 0;
        while ((0x80 & rtw_read8(EFUSE_CTRL + (3))) == 0 && (tmpidx < 1000))
        {
            Thread.Sleep(1);
            tmpidx++;
        }

        if (tmpidx < 100)
        {
            data = rtw_read8(EFUSE_CTRL);
        }
        else
        {
            data = 0xff;
            //RTW_INFO("%s: [ERROR] addr=0x%x bResult=%d time out 1s !!!\n", __FUNCTION__, addr, bResult);
            //RTW_INFO("%s: [ERROR] EFUSE_CTRL =0x%08x !!!\n", __FUNCTION__, rtw_read32(pAdapterState, EFUSE_CTRL));
        }
    }

    public void phy_set_bb_reg(u16 regAddr, u32 bitMask, u32 data) => PHY_SetBBReg8812(regAddr, bitMask, data);

    public void PHY_SetBBReg8812(
        u16 regAddr,
        u32 bitMask,
        u32 dataOriginal)
    {
        u32 data = dataOriginal;
        if (bitMask != bMaskDWord)
        {
            /* if not "double word" write */
            var OriginalValue = rtw_read32(regAddr);
            var BitShift = PHY_CalculateBitShift(bitMask);
            data = ((OriginalValue) & (~bitMask)) | (((dataOriginal << (int)BitShift)) & bitMask);
        }

        rtw_write32(regAddr, data);

        /* RTW_INFO("BBW MASK=0x%x Addr[0x%x]=0x%x\n", BitMask, RegAddr, Data); */
    }


    public void ReadEFuseByte(UInt16 _offset, byte[] pbuf)
    {
        u32 value32;
        u8 readbyte;
        u16 retry;

        /* Write Address */
        rtw_write8(EFUSE_CTRL + 1, (byte)(_offset & 0xff));
        readbyte = rtw_read8(EFUSE_CTRL + 2);
        rtw_write8(EFUSE_CTRL + 2, (byte)(((_offset >> 8) & 0x03) | (readbyte & 0xfc)));

        /* Write bit 32 0 */
        readbyte = rtw_read8(EFUSE_CTRL + 3);
        rtw_write8(EFUSE_CTRL + 3, (byte)(readbyte & 0x7f));

        /* Check bit 32 read-ready */
        retry = 0;
        value32 = rtw_read32(EFUSE_CTRL);
        /* while(!(((value32 >> 24) & 0xff) & 0x80)  && (retry<10)) */
        while ((((value32 >> 24) & 0xff) & 0x80) == 0 && (retry < 10000))
        {
            value32 = rtw_read32(EFUSE_CTRL);
            retry++;
        }

        /* 20100205 Joseph: Add delay suggested by SD1 Victor. */
        /* This fix the problem that Efuse read error in high temperature condition. */
        /* Designer says that there shall be some delay after ready bit is set, or the */
        /* result will always stay on last data we read. */
        Thread.Sleep(50);
        value32 = rtw_read32(EFUSE_CTRL);

        pbuf[0] = (u8)(value32 & 0xff);
    }


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