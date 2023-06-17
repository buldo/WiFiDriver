namespace Rtl8812auNet.Rtl8812au;

public static class usb_ops
{
    public static bool IS_HIGH_SPEED_USB(AdapterState adapterState) =>
        (adapter_to_dvobj(adapterState).UsbSpeed == RTW_USB_SPEED_2);
    public static bool IS_SUPER_SPEED_USB(AdapterState adapterState) =>
        (adapter_to_dvobj(adapterState).UsbSpeed == RTW_USB_SPEED_3);

    public const uint USB_SUPER_SPEED_BULK_SIZE = 1024; /* usb 3.0 */
    public const uint USB_HIGH_SPEED_BULK_SIZE = 512; /* usb 2.0 */
    public const uint USB_FULL_SPEED_BULK_SIZE = 64; /* usb 1.1 */

}