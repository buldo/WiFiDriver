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
        // Look at rtw_hal_ops_check in hal_init.c
        // 11 in  0xC0(REALTEK_USB_VENQT_READ)  0x00FC
        // 13 in  0xC0(REALTEK_USB_VENQT_READ)  0x00F0
        // 15 in  0xC0(REALTEK_USB_VENQT_READ)  0x000A
        // 17 out 0x40(REALTEK_USB_VENQT_WRITE) 0x0000
        // 19 in  0xC0(REALTEK_USB_VENQT_READ)  0x0032
    }
}