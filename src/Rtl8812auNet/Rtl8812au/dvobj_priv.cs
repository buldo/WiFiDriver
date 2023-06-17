using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class dvobj_priv
{
    private const int CONFIG_IFACE_NUMBER = 1;
    public _adapter[] padapters { get; } =new _adapter[CONFIG_IFACE_NUMBER]; /*IFACE_ID_MAX*/

    public byte iface_nums { get; set; } /* total number of ifaces used runtime */

    public pwrctrl_priv pwrctl_priv { get; }= new pwrctrl_priv();

    /*-------- below is for USB INTERFACE --------*/

    public u8 usb_speed { get; set; } /* 1.1, 2.0 or 3.0 */
    public u8 nr_endpoint { get; set; }
    public u8 RtNumInPipes { get; set; }
    public u8 RtNumOutPipes { get; set; }
    public int[] ep_num { get; }= new int[6]; /* endpoint number */
}