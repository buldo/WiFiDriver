using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly IRtlUsbDevice _usbDevice;
    private readonly AdapterState _adapterState;
    private Task _readTask;

    public Rtl8812aDevice(IRtlUsbDevice usbDevice)
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
            cur_channel = 8
        });

        SetMonitorChannel(_adapterState, new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        _readTask = Task.Run(() => _usbDevice.InfinityRead());
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
}