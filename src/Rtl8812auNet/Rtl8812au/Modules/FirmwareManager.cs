using System.Diagnostics;
using Rtl8812auNet.Rtl8812au.PredefinedData;

namespace Rtl8812auNet.Rtl8812au.Modules;

public class FirmwareManager
{
    private static readonly Firmware _firmware = new();
    private readonly RtlUsbAdapter _device;

    public FirmwareManager(RtlUsbAdapter device)
    {
        _device = device;
    }

    public void FirmwareDownload8812()
    {
        bool rtStatus = true;
        u8 write_fw = 0;
        //var pHalData = adapterState.HalData;

        var pFirmwareBuf = _firmware.Data;
        var FirmwareLen = (uint)_firmware.Data.Length;

        var firmwareVersion = _firmware.GET_FIRMWARE_HDR_VERSION_8812();
        var firmwareSubVersion = _firmware.GET_FIRMWARE_HDR_SUB_VER_8812();
        var firmwareSignature = _firmware.GET_FIRMWARE_HDR_SIGNATURE_8812();

        RTW_INFO($"FirmwareDownload8812: fw_ver={firmwareVersion} fw_subver={firmwareSubVersion} sig=0x{firmwareSignature:X}");

        if (_firmware.IS_FW_HEADER_EXIST_8812())
        {
            /* Shift 32 bytes for FW header */
            pFirmwareBuf = pFirmwareBuf.Slice(32);
            FirmwareLen -= 32;
        }

        /* Suggested by Filen. If 8051 is running in RAM code, driver should inform Fw to reset by itself, */
        /* or it will cause download Fw fail. 2010.02.01. by tynli. */
        if ((_device.rtw_read8(REG_MCUFWDL) & BIT7) != 0)
        {
            /* 8051 RAM code */
            _device.rtw_write8(REG_MCUFWDL, 0x00);
            _device._8051Reset8812();
        }

        _FWDownloadEnable_8812(true);
        var fwdl_start_time = Stopwatch.StartNew();
        while ((write_fw++ < 3 || (fwdl_start_time.ElapsedMilliseconds) < 500))
        {
            /* reset FWDL chksum */
            _device.rtw_write8(REG_MCUFWDL, (byte)(_device.rtw_read8(REG_MCUFWDL) | FWDL_ChkSum_rpt));

            rtStatus = WriteFW8812(pFirmwareBuf, FirmwareLen);
            if (rtStatus != true)
            {
                continue;
            }

            rtStatus = polling_fwdl_chksum(5, 50);
            if (rtStatus == true)
            {
                break;
            }
        }

        _FWDownloadEnable_8812(false);
        if (true != rtStatus)
        {
            return;
        }

        rtStatus = _FWFreeToGo8812(10, 200);
        if (true != rtStatus)
        {
            return;
        }

        InitializeFirmwareVars8812();
    }

    private void InitializeFirmwareVars8812()
    {
        /* Init H2C cmd. */
        _device.rtw_write8(REG_HMETFR, 0x0f);
    }

    private bool polling_fwdl_chksum(uint min_cnt, uint timeout_ms)
    {
        bool ret = false;
        uint value32;
        var start = Stopwatch.StartNew();
        uint cnt = 0;

        /* polling CheckSum report */
        do
        {
            cnt++;
            value32 = _device.rtw_read32(REG_MCUFWDL);
            if ((value32 & Firmware.FWDL_ChkSum_rpt) != 0)
            {
                break;
            }
        } while (start.ElapsedMilliseconds < timeout_ms || cnt < min_cnt);

        if (!((value32 & Firmware.FWDL_ChkSum_rpt) != 0))
        {
            return false;
        }

        return true;
    }

    private bool _FWFreeToGo8812(u32 min_cnt, u32 timeout_ms)
    {
        bool ret = false;
        u32 value32;
        u32 cnt = 0;

        value32 = _device.rtw_read32(REG_MCUFWDL);
        value32 |= MCUFWDL_RDY;
        value32 = (u32)(value32 & ~WINTINI_RDY);
        _device.rtw_write32(REG_MCUFWDL, value32);

        _device._8051Reset8812();

        var start = Stopwatch.StartNew();
        /*  polling for FW ready */
        do
        {
            cnt++;
            value32 = _device.rtw_read32(REG_MCUFWDL);
            if ((value32 & WINTINI_RDY) != 0)
            {
                break;
            }

        } while ((start.ElapsedMilliseconds) < timeout_ms || cnt < min_cnt);

        if (!((value32 & WINTINI_RDY) != 0))
        {
            goto exit;
        }

        //if (rtw_fwdl_test_trigger_wintint_rdy_fail())
        //{
        //    goto exit;
        //}

        ret = true;

        exit:
        RTW_INFO($"_FWFreeToGo8812: Polling FW ready {(ret ? "OK" : "Fail")}! ({cnt}), REG_MCUFWDL:0x{value32:X8}");

        return ret;
    }

    private void _FWDownloadEnable_8812(bool enable)
    {
        u8 tmp;

        if (enable)
        {
            /* MCU firmware download enable. */
            tmp = _device.rtw_read8(REG_MCUFWDL);
            _device.rtw_write8(REG_MCUFWDL, (byte)(tmp | 0x01));

            /* 8051 reset */
            tmp = _device.rtw_read8(REG_MCUFWDL + 2);
            _device.rtw_write8(REG_MCUFWDL + 2, (byte)(tmp & 0xf7));
        }
        else
        {
            /* MCU firmware download disable. */
            tmp = _device.rtw_read8(REG_MCUFWDL);
            _device.rtw_write8(REG_MCUFWDL, (byte)(tmp & 0xfe));
        }
    }

    private bool WriteFW8812(Span<byte> buffer, UInt32 size)
    {
        const int MAX_DLFW_PAGE_SIZE = 4096; /* @ page : 4k bytes */

        /* Since we need dynamic decide method of dwonload fw, so we call this function to get chip version. */
        bool ret = true;
        Int32 pageNums, remainSize;
        Int32 page;
        int offset;
        var bufferPtr = buffer;

        pageNums = (int)(size / MAX_DLFW_PAGE_SIZE);
        /* RT_ASSERT((pageNums <= 4), ("Page numbers should not greater then 4\n")); */
        remainSize = (int)(size % MAX_DLFW_PAGE_SIZE);

        for (page = 0; page < pageNums; page++)
        {
            offset = page * MAX_DLFW_PAGE_SIZE;
            ret = PageWrite(page, bufferPtr.Slice(offset), MAX_DLFW_PAGE_SIZE);

            if (ret == false)
            {
                goto exit;
            }
        }

        if (remainSize != 0)
        {
            offset = pageNums * MAX_DLFW_PAGE_SIZE;
            page = pageNums;
            ret = PageWrite(page, bufferPtr.Slice(offset), remainSize);

            if (ret == false)
            {
                goto exit;
            }

        }

    exit:
        return ret;
    }

    private bool PageWrite(int page, Span<byte> buffer, int size)
    {
        byte value8;
        byte u8Page = (byte)(page & 0x07);

        value8 = (byte)((_device.Read8((REG_MCUFWDL + 2)) & 0xF8) | u8Page);
        _device.Write8((REG_MCUFWDL + 2), value8);

        return BlockWrite(buffer, size);
    }

    private bool BlockWrite(Span<byte> buffer, int buffSize)
    {
        const int MAX_REG_BOLCK_SIZE = 196;

        bool ret = true;

        UInt32 blockSize_p1 = 4; /* (Default) Phase #1 : PCI muse use 4-byte write to download FW */
        UInt32 blockSize_p2 = 8; /* Phase #2 : Use 8-byte, if Phase#1 use big size to write FW. */
        UInt32 blockSize_p3 = 1; /* Phase #3 : Use 1-byte, the remnant of FW image. */
        UInt32 blockCount_p1 = 0, blockCount_p2 = 0, blockCount_p3 = 0;
        UInt32 remainSize_p1 = 0, remainSize_p2 = 0;
        //u8			*bufferPtr	= (u8 *)buffer;
        UInt32 i = 0, offset = 0;

        blockSize_p1 = MAX_REG_BOLCK_SIZE;

        /* 3 Phase #1 */
        blockCount_p1 = (UInt32)(buffSize / blockSize_p1);
        remainSize_p1 = (UInt32)(buffSize % blockSize_p1);


        for (i = 0; i < blockCount_p1; i++)
        {
            _device.WriteBytes((ushort)(FW_START_ADDRESS + i * blockSize_p1),
                buffer.Slice((int)(i * blockSize_p1), (int)blockSize_p1));
        }

        /* 3 Phase #2 */
        if (remainSize_p1 != 0)
        {
            offset = blockCount_p1 * blockSize_p1;

            blockCount_p2 = remainSize_p1 / blockSize_p2;
            remainSize_p2 = remainSize_p1 % blockSize_p2;

            for (i = 0; i < blockCount_p2; i++)
            {
                _device.WriteBytes((ushort)(FW_START_ADDRESS + offset + i * blockSize_p2),
                    buffer.Slice((int)(offset + i * blockSize_p2), (int)blockSize_p2));
            }

        }

        /* 3 Phase #3 */
        if (remainSize_p2 != 0)
        {
            offset = (blockCount_p1 * blockSize_p1) + (blockCount_p2 * blockSize_p2);

            blockCount_p3 = remainSize_p2 / blockSize_p3;


            for (i = 0; i < blockCount_p3; i++)
            {
                _device.Write8((ushort)(FW_START_ADDRESS + offset + i), buffer[(int)(offset + i)]);
            }
        }

        return ret;
    }
}