using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class AdapterState
{
    public AdapterState(DvObj dvObj, HwPort hwPort, IRtlUsbDevice rtlUsbDevice)
    {
        DvObj = dvObj;
        HwPort = hwPort;
        Device = rtlUsbDevice;
    }

    public DvObj DvObj { get; }

    public registry_priv registrypriv { get; } = new ();

    public hal_com_data HalData { get; } = new();

    public IRtlUsbDevice Device { get; }

    public HwPort HwPort { get; }
}