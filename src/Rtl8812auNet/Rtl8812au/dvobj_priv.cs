using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class dvobj_priv
{
    public dvobj_priv(
        byte rtNumOutPipes,
        byte usbSpeed)
    {
        RtNumOutPipes = rtNumOutPipes;
        usb_speed = usbSpeed;
    }

    public pwrctrl_priv pwrctl_priv { get; }= new();

    /*-------- below is for USB INTERFACE --------*/

    public u8 usb_speed { get; } /* 1.1, 2.0 or 3.0 */
    public u8 RtNumOutPipes { get; }
}