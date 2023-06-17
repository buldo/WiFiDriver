using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class _adapter
{
    public bool isprimary { get; set; }
    public dvobj_priv dvobj { get; set; }
    public IFACE_ID iface_id { get; set; }
    public ADAPTER_TYPE adapter_type { get; set; }

    public registry_priv registrypriv { get; } = new ();

    public hal_com_data HalData { get; } = new();

    public hal_ops hal_func { get; } = new();
    public IRtlUsbDevice Device { get; set; }
    public bool netif_up { get; set; }
    public bool up { get; set; }
    public bool net_closed { get; set; }
    public hw_port hw_port { get; set; }
    public NAPI_STATE napi_state { get; set; }
}