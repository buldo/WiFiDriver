using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class AdapterState
{
    public DvObj dvobj { get; set; }

    public registry_priv registrypriv { get; } = new ();

    public hal_com_data HalData { get; } = new();

    public IRtlUsbDevice Device { get; init; }

    public bool IsUp { get; set; }

    public HwPort HwPort { get; set; }
}