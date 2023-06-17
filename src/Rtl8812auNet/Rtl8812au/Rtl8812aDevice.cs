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
        _adapterState = usb_intf.rtw_drv_init(_usbDevice);
    }

    public void Init()
    {

        ioctl_cfg80211.cfg80211_rtw_change_iface(_adapterState, new InitChannel()
        {
            cur_bwmode = channel_width.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        ioctl_cfg80211.cfg80211_rtw_set_monitor_channel(_adapterState, new InitChannel()
        {
            cur_bwmode = channel_width.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        _readTask = Task.Run(() => _usbDevice.InfinityRead());
    }
}