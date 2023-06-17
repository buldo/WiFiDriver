namespace Rtl8812auNet.Rtl8812au;

public class DvObj
{
    public DvObj(
        byte outPipesCount,
        byte usbSpeed)
    {
        OutPipesCount = outPipesCount;
        UsbSpeed = usbSpeed;
    }

    public u8 UsbSpeed { get; } /* 1.1, 2.0 or 3.0 */
    public u8 OutPipesCount { get; }
}