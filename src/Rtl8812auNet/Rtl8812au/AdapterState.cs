namespace Rtl8812auNet.Rtl8812au;

public class AdapterState
{
    public AdapterState(RtlUsbAdapter rtlUsbDevice, hal_com_data halData)
    {
        Device = rtlUsbDevice;
        HalData = halData;
    }

    public hal_com_data HalData { get; }

    public RtlUsbAdapter Device { get; }
}