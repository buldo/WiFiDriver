namespace Rtl8812auNet.Rtl8812au.Models;

public class DvObj
{
    public DvObj(
        byte outPipesCount,
        byte usbSpeed)
    {
        OutPipesCount = outPipesCount;
        UsbSpeed = usbSpeed;
    }

    public byte UsbSpeed { get; } /* 1.1, 2.0 or 3.0 */
    public byte OutPipesCount { get; }
}