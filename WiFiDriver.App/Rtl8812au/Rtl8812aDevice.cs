using System.Threading;

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;

using WiFiDriver.App.Rtl8812au;


namespace WiFiDriver.App.Rtl8812au;

public class Rtl8812aDevice
{
    private const byte REALTEK_USB_VENQT_READ = 0xC0;
    private const byte REALTEK_USB_VENQT_WRITE = 0x40;
    private const byte REALTEK_USB_VENQT_CMD_REQ = 0x05;
    private const byte REALTEK_USB_VENQT_CMD_IDX = 0x00;
    private const byte REALTEK_USB_IN_INT_EP_IDX = 1;

    private readonly UsbDevice _usbDevice;

    public Rtl8812aDevice(UsbDevice usbDevice)
    {
        _usbDevice = usbDevice;
        var dvobj_priv = usb_intf.usb_dvobj_init(_usbDevice);
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