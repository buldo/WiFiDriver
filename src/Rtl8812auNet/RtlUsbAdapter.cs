using System.Buffers.Binary;
using Microsoft.Extensions.Logging;
using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet;

public class RtlUsbAdapter
{
    private const byte BOOT_FROM_EEPROM = (byte)BIT4;
    private const byte EEPROM_EN = (byte)BIT5;

    private readonly IRtlUsbDevice _usbDevice;
    private readonly ILogger<RtlUsbAdapter> _logger;

    public RtlUsbAdapter(
        IRtlUsbDevice usbDevice,
        ILogger<RtlUsbAdapter> logger)
    {
        _usbDevice = usbDevice;
        _logger = logger;
        (OutPipesCount, UsbSpeed) = InitDvObj();

        if (UsbSpeed == RTW_USB_SPEED_3)
        {
            rxagg_usb_size = 0x7;
            rxagg_usb_timeout = 0x1a;
        }
        else
        {
            /* the setting to reduce RX FIFO overflow on USB2.0 and increase rx throughput */
            rxagg_usb_size = 0x5;
            rxagg_usb_timeout = 0x20;
        }

        (OutEpQueueSel, OutEpNumber) = GetChipOutEP8812(OutPipesCount);

        var eeValue = rtw_read8(REG_9346CR);
        EepromOrEfuse = (eeValue & BOOT_FROM_EEPROM) != 0;
        AutoloadFailFlag = (eeValue & EEPROM_EN) == 0;

        _logger.LogInformation($"Boot from {(EepromOrEfuse ? "EEPROM" : "EFUSE")}, Autoload {(AutoloadFailFlag ? "Fail" : "OK")} !");
    }

    public IRtlUsbDevice UsbDevice => _usbDevice;

    public byte UsbSpeed { get; } /* 1.1, 2.0 or 3.0 */

    public byte OutPipesCount { get; }

    public byte rxagg_usb_size { get; }

    public byte rxagg_usb_timeout { get; }

    public TxSele OutEpQueueSel { get; }

    public byte OutEpNumber { get; }

    public bool AutoloadFailFlag { get; set; }
    public bool EepromOrEfuse { get; }

    public void rtl8812au_hw_reset()
    {
        uint reg_val = 0;
        if ((rtw_read8(REG_MCUFWDL) & BIT7) != 0)
        {
            _8051Reset8812();
            rtw_write8(REG_MCUFWDL, 0x00);
            /* before BB reset should do clock gated */
            rtw_write32(rFPGA0_XCD_RFPara,
                rtw_read32(rFPGA0_XCD_RFPara) | (BIT6));
            /* reset BB */
            reg_val = rtw_read8(REG_SYS_FUNC_EN);
            reg_val = (byte)(reg_val & ~(BIT0 | BIT1));
            rtw_write8(REG_SYS_FUNC_EN, (byte)reg_val);
            /* reset RF */
            rtw_write8(REG_RF_CTRL, 0);
            /* reset TRX path */
            rtw_write16(REG_CR, 0);
            /* reset MAC */
            reg_val = rtw_read8(REG_APS_FSMCO + 1);
            reg_val |= BIT1;
            rtw_write8(REG_APS_FSMCO + 1, (byte)reg_val); /* reg0x5[1] ,auto FSM off */

            reg_val = rtw_read8(REG_APS_FSMCO + 1);

            /* check if   reg0x5[1] auto cleared */
            while ((reg_val & BIT1) != 0)
            {
                Thread.Sleep(1);
                reg_val = rtw_read8(REG_APS_FSMCO + 1);
            }

            reg_val |= BIT0;
            rtw_write8(REG_APS_FSMCO + 1, (byte)reg_val); /* reg0x5[0] ,auto FSM on */

            reg_val = rtw_read8(REG_SYS_FUNC_EN + 1);
            reg_val = (byte)(reg_val & ~(BIT4 | BIT7));
            rtw_write8(REG_SYS_FUNC_EN + 1, (byte)reg_val);
            reg_val = rtw_read8(REG_SYS_FUNC_EN + 1);
            reg_val = (byte)(reg_val | BIT4 | BIT7);
            rtw_write8(REG_SYS_FUNC_EN + 1, (byte)reg_val);
        }
    }

    public void _8051Reset8812()
    {
        byte u1bTmp, u1bTmp2;

        /* Reset MCU IO Wrapper- sugggest by SD1-Gimmy */

        u1bTmp2 = rtw_read8(REG_RSV_CTRL);
        rtw_write8(REG_RSV_CTRL, (byte)(u1bTmp2 & (NotBIT1)));
        u1bTmp2 = rtw_read8(REG_RSV_CTRL + 1);
        rtw_write8(REG_RSV_CTRL + 1, (byte)(u1bTmp2 & (NotBIT3)));


        u1bTmp = rtw_read8(REG_SYS_FUNC_EN + 1);
        rtw_write8(REG_SYS_FUNC_EN + 1, (byte)(u1bTmp & (NotBIT2)));

        /* Enable MCU IO Wrapper */

        u1bTmp2 = rtw_read8(REG_RSV_CTRL);
        rtw_write8(REG_RSV_CTRL, (byte)(u1bTmp2 & (NotBIT1)));
        u1bTmp2 = rtw_read8(REG_RSV_CTRL + 1);
        rtw_write8(REG_RSV_CTRL + 1, (byte)(u1bTmp2 | (BIT3)));


        rtw_write8(REG_SYS_FUNC_EN + 1, (byte)(u1bTmp | (BIT2)));

        RTW_INFO("=====> _8051Reset8812(): 8051 reset success .");
    }

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

    public void phy_set_bb_reg(UInt16 regAddr, UInt32 bitMask, UInt32 data) => PHY_SetBBReg8812(regAddr, bitMask, data);

    private void PHY_SetBBReg8812(
        UInt16 regAddr,
        UInt32 bitMask,
        UInt32 dataOriginal)
    {
        UInt32 data = dataOriginal;
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
        UInt32 value32;
        byte readbyte;
        UInt16 retry;

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

        pbuf[0] = (byte)(value32 & 0xff);
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

    private (byte numOutPipes, byte usbSpeed) InitDvObj()
    {
        byte numOutPipes = 0;

        foreach (var endpoint in UsbDevice.GetEndpoints())
        {
            var type = endpoint.Type;
            var direction = endpoint.Direction;

            if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.Out)
            {
                numOutPipes++;
            }
        }

        var usbSpeed = UsbDevice.Speed switch
        {
            USB_SPEED_LOW => RTW_USB_SPEED_1_1,
            USB_SPEED_FULL => RTW_USB_SPEED_1_1,
            USB_SPEED_HIGH => RTW_USB_SPEED_2,
            USB_SPEED_SUPER => RTW_USB_SPEED_3,
            _ => RTW_USB_SPEED_UNKNOWN
        };

        if (usbSpeed == RTW_USB_SPEED_UNKNOWN)
        {
            RTW_INFO("UNKNOWN USB SPEED MODE, ERROR !!!");
            throw new Exception();
        }

        return (numOutPipes, usbSpeed);
    }

    private (TxSele OutEpQueueSel, byte OutEpNumber) GetChipOutEP8812(byte NumOutPipe)
    {
        TxSele OutEpQueueSel = 0;
        byte OutEpNumber = 0;

        switch (NumOutPipe)
        {
            case 4:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ | TxSele.TX_SELE_EQ;
                OutEpNumber = 4;
                break;
            case 3:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_LQ | TxSele.TX_SELE_NQ;
                OutEpNumber = 3;
                break;
            case 2:
                OutEpQueueSel = TxSele.TX_SELE_HQ | TxSele.TX_SELE_NQ;
                OutEpNumber = 2;
                break;
            case 1:
                OutEpQueueSel = TxSele.TX_SELE_HQ;
                OutEpNumber = 1;
                break;
            default:
                break;
        }

        RTW_INFO($"OutEpQueueSel({OutEpQueueSel}), OutEpNumber({OutEpNumber})");

        return (OutEpQueueSel, OutEpNumber);
    }
}