namespace Rtl8812auNet.Rtl8812au;

public class AdapterState
{
    public AdapterState(DvObj dvObj, HwPort hwPort, RtlUsbAdapter rtlUsbDevice, hal_com_data halData)
    {
        DvObj = dvObj;
        HwPort = hwPort;
        Device = rtlUsbDevice;
        HalData = halData;
    }

    public DvObj DvObj { get; }

    public hal_com_data HalData { get; }

    public RtlUsbAdapter Device { get; }

    public HwPort HwPort { get; }
}