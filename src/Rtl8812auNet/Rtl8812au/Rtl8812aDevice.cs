using Microsoft.Extensions.Logging;
using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au.Modules;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly RtlUsbAdapter _device;
    private readonly ILogger _logger;
    private readonly AdapterState _adapterState;
    private readonly StatefulFrameParser _frameParser = new();
    private readonly RadioManagementModule _radioManagement;

    private Task _readTask;
    private Task _parseTask;
    private readonly HalModule _halModule;
    private Func<ParsedRadioPacket, Task> _packetProcessor;

    public Rtl8812aDevice(RtlUsbAdapter device, ILogger<Rtl8812aDevice> logger)
    {
        _device = device;
        _logger = logger;

        _radioManagement = new RadioManagementModule(HwPort.HW_PORT0, device, _logger);
        _halModule = new HalModule(_device, _radioManagement);

        _adapterState = InitAdapter(_device);
    }

    public void Init(
        Func<ParsedRadioPacket, Task> packetProcessor,
        SelectedChannel channel)
    {
        _packetProcessor = packetProcessor;

        StartWithMonitorMode(channel);
        SetMonitorChannel(channel);

        _readTask = Task.Run(() => _device.UsbDevice.InfinityRead());
        _parseTask = Task.Run(ParseUsbData);
    }

    public void SetMonitorChannel(SelectedChannel channel)
    {
        _radioManagement.set_channel_bwmode(_adapterState.HalData, channel.Channel, channel.ChannelOffset, channel.ChannelWidth);
    }

    private DvObj InitDvObj(RtlUsbAdapter usbInterface)
    {
        u8 numOutPipes = 0;

        foreach (var endpoint in usbInterface.UsbDevice.GetEndpoints())
        {
            var type = endpoint.Type;
            var direction = endpoint.Direction;

            if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.Out)
            {
                numOutPipes++;
            }
        }

        var usbSpeed = usbInterface.UsbDevice.Speed switch
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

        return new DvObj(numOutPipes, usbSpeed);
    }

    private AdapterState InitAdapter(RtlUsbAdapter pusb_intf)
    {
        var dvobj = InitDvObj(_device);

        u8 rxagg_usb_size;
        u8 rxagg_usb_timeout;
        if (dvobj.UsbSpeed == RTW_USB_SPEED_3)
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

        var chipVersionResult = read_chip_version_8812a();
        var chipOut = GetChipOutEP8812(dvobj.OutPipesCount);

        var eeValue = _device.rtw_read8(REG_9346CR);
        var eepromOrEfuse = (eeValue & BOOT_FROM_EEPROM) != 0;
        var autoloadFailFlag = (eeValue & EEPROM_EN) == 0;

        RTW_INFO($"Boot from {(eepromOrEfuse ? "EEPROM" : "EFUSE")}, Autoload {(autoloadFailFlag ? "Fail" : "OK")} !");

        var halData = new hal_com_data(
            chipVersionResult.version_id,
            chipVersionResult.rf_type,
            chipVersionResult.numTotalRfPath)
        {
            rxagg_usb_size = rxagg_usb_size, /* unit: 512b */
            rxagg_usb_timeout = rxagg_usb_timeout,
            OutEpQueueSel = chipOut.OutEpQueueSel,
            OutEpNumber = chipOut.OutEpNumber,
            EepromOrEfuse = eepromOrEfuse,
            AutoloadFailFlag = autoloadFailFlag
        };

        var adapterState = new AdapterState(pusb_intf, halData);

        /* step read efuse/eeprom data and get mac_addr */
        ReadAdapterInfo8812AU(adapterState);

        /* step 5. */
        Init_ODM_ComInfo_8812(halData);

        return adapterState;
    }

    private (TxSele OutEpQueueSel, byte OutEpNumber) GetChipOutEP8812(u8 NumOutPipe)
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

    private (HAL_VERSION version_id, RfType rf_type, byte numTotalRfPath) read_chip_version_8812a()
    {
        u32 value32 = _device.rtw_read32(REG_SYS_CFG);
        RTW_INFO($"read_chip_version_8812a SYS_CFG(0x{REG_SYS_CFG:X})=0x{value32:X8}");

        var versionId = new HAL_VERSION
        {
            RFType = HalRFType.RF_TYPE_2T2R /* RF_2T2R; */
        };

        if (registry_priv.special_rf_path == 1)
        {
            versionId.RFType = HalRFType.RF_TYPE_1T1R; /* RF_1T1R; */
        }

        versionId.CUTVersion = (CutVersion)((value32 & CHIP_VER_RTL_MASK) >> CHIP_VER_RTL_SHIFT); /* IC version (CUT) */
        versionId.CUTVersion += 1;

        /* For multi-function consideration. Added by Roger, 2010.10.06. */

        var (rfType, numTotalRfPath) = versionId.GetRfType();

        RTW_INFO($"rtw_hal_config_rftype RF_Type is {rfType} TotalTxPath is {numTotalRfPath}");

        return(versionId, rfType, numTotalRfPath);
    }

    private void StartWithMonitorMode(SelectedChannel selectedChannel)
    {
        if (NetDevOpen(selectedChannel) == false)
        {
            throw new Exception("StartWithMonitorMode failed NetDevOpen");
        }

        _radioManagement.SetMonitorMode();
    }

    private bool NetDevOpen(SelectedChannel selectedChannel)
    {
        var status = _halModule.rtw_hal_init(_adapterState.HalData, selectedChannel);
        if (status == false)
        {
            return false;
        }

        return true;
    }

    private async Task ParseUsbData()
    {
        await foreach (var transfer in _device.UsbDevice.BulkTransfersReader.ReadAllAsync())
        {
            var packet = _frameParser.ParsedRadioPacket(transfer);
            foreach (var radioPacket in packet)
            {
                try
                {
                    await _packetProcessor(radioPacket);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}