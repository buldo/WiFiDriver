using Microsoft.Extensions.Logging;
using Rtl8812auNet.Rtl8812au.Enumerations;
using Rtl8812auNet.Rtl8812au.Models;
using Rtl8812auNet.Rtl8812au.Modules;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly RtlUsbAdapter _device;
    private readonly ILogger _logger;
    private readonly FrameParser _frameParser = new();
    private readonly RadioManagementModule _radioManagement;

    private Task _readTask;
    private Task _parseTask;
    private readonly HalModule _halModule;
    private Func<ParsedRadioPacket, Task> _packetProcessor;

    public Rtl8812aDevice(RtlUsbAdapter device, ILogger<Rtl8812aDevice> logger)
    {
        _device = device;
        _logger = logger;

        var eepromManager = new EepromManager(device);
        _radioManagement = new RadioManagementModule(HwPort.HW_PORT0, device, eepromManager, _logger);
        _halModule = new HalModule(_device, _radioManagement, eepromManager);
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
        _radioManagement.set_channel_bwmode(channel.Channel, channel.ChannelOffset, channel.ChannelWidth);
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
        var status = _halModule.rtw_hal_init(selectedChannel);
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