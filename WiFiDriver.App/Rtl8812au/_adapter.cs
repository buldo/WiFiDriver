using LibUsbDotNet.LibUsb;

namespace WiFiDriver.App.Rtl8812au;

public class _adapter
{
    public bool isprimary { get; set; }
    public dvobj_priv dvobj { get; set; }
    public IFACE_ID iface_id { get; set; }
    public ADAPTER_TYPE adapter_type { get; set; }

    public registry_priv registrypriv { get; } = new ();

    public hal_com_data HalData { get; } = new();

    public hal_ops hal_func { get; } = new();
    public UsbDevice Device { get; set; }
}