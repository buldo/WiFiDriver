using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class _adapter
{
    public dvobj_priv dvobj { get; set; }

    public registry_priv registrypriv { get; } = new ();

    public hal_com_data HalData { get; } = new();

    public IRtlUsbDevice Device { get; set; }
    public bool up { get; set; }
    public hw_port hw_port { get; set; }
    public NAPI_STATE napi_state { get; set; }
}