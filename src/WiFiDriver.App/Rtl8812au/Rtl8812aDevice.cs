using LibUsbDotNet.LibUsb;

namespace WiFiDriver.App.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly UsbDevice _usbDevice;
    private readonly _adapter _adapter;

    public Rtl8812aDevice(UsbDevice usbDevice)
    {
        _usbDevice = usbDevice;
        _usbDevice.Open();
        _adapter = usb_intf.rtw_drv_init(_usbDevice);
    }

    public void Init()
    {

        ioctl_cfg80211.cfg80211_rtw_change_iface(_adapter, new InitChannel()
        {
            cur_bwmode = channel_width.CHANNEL_WIDTH_40,
            cur_ch_offset = 0,
            cur_channel = 46
        });
    }
}