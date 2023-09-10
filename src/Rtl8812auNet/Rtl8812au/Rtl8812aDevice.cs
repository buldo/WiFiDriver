using System.Net;
using System.Net.Sockets;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly RtlUsbAdapter _usbDevice;
    private readonly AdapterState _adapterState;
    private readonly StatefulFrameParser _frameParser = new();
    private Task _readTask;
    private Task _parseTask;

    public Rtl8812aDevice(RtlUsbAdapter usbDevice)
    {
        _usbDevice = usbDevice;
        var dvobj = InitDvObj(_usbDevice);
        _adapterState = InitAdapter(dvobj, _usbDevice);
    }

    public void Init()
    {

        StartWithMonitorMode(new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        SetMonitorChannel(_adapterState, new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        _readTask = Task.Run(() => _usbDevice.UsbDevice.InfinityRead());
        _parseTask = Task.Run(() => ParseUsbData());
    }

    private void StartWithMonitorMode(InitChannel initChannel)
    {
        if (NetDevOpen(initChannel) == false)
        {
            throw new Exception("StartWithMonitorMode failed NetDevOpen");
        }

        setopmode_hdl(_adapterState);
    }

    private void SetMonitorChannel(AdapterState padapter, InitChannel chandef)
    {
        set_channel_bwmode(padapter, chandef.cur_channel, chandef.cur_ch_offset, chandef.cur_bwmode);
    }

    private bool NetDevOpen(InitChannel initChannel)
    {
        var status = rtw_hal_init(_adapterState, initChannel);
        if (status == false)
        {
            return false;
        }

        return true;
    }

    private readonly UdpClient _client = new UdpClient();
    private readonly IPEndPoint _address = new IPEndPoint(IPAddress.Parse("172.23.97.127"), 4321);

    private async Task ParseUsbData()
    {
        await foreach (var transfer in _usbDevice.UsbDevice.BulkTransfersReader.ReadAllAsync())
        {
            var packet = _frameParser.ParsedRadioPacket(transfer);
            foreach (var radioPacket in packet)
            {
                //await _client.SendAsync(radioPacket.Data, _address);
                //Console.WriteLine(Convert.ToHexString(radioPacket.Data));
                //Console.WriteLine();
            }
        }
    }
}