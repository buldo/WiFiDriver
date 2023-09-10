namespace Rtl8812auNet.Rtl8812au;

public class AdapterState
{
    public AdapterState(DvObj dvObj, HwPort hwPort, RtlUsbAdapter rtlUsbDevice)
    {
        DvObj = dvObj;
        HwPort = hwPort;
        Device = rtlUsbDevice;
    }

    public DvObj DvObj { get; }

    public hal_com_data HalData { get; } = new();

    public RtlUsbAdapter Device { get; }

    public HwPort HwPort { get; }
}