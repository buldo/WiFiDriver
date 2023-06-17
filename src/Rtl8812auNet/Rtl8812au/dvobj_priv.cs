using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public class dvobj_priv
{
    private const int CONFIG_IFACE_NUMBER = 1;
    /*-------- below is common data --------*/

    public RTL871X_HCI_TYPE interface_type = RTL871X_HCI_TYPE.RTW_USB; /*USB,SDIO,SPI,PCI*/
    public byte oper_channel; /* saved channel info when call set_channel_bw */
    public _adapter[] padapters { get; } =new _adapter[CONFIG_IFACE_NUMBER]; /*IFACE_ID_MAX*/

    public byte iface_nums; /* total number of ifaces used runtime */

    public rf_ctl_t rf_ctl = new rf_ctl_t();

    /* In /Out Pipe information */
    public int[] RtInPipe = new int[2];
    public int[] RtOutPipe = new int[4];
    public int[] Queue2Pipe = new int[HW_QUEUE_ENTRY]; /* for out pipe mapping */

    //_lock xmit_block_lock;

    public pwrctrl_priv pwrctl_priv = new pwrctrl_priv();

    /*-------- below is for USB INTERFACE --------*/

    public u8 usb_speed; /* 1.1, 2.0 or 3.0 */
    public u8 nr_endpoint;
    public u8 RtNumInPipes;
    public u8 RtNumOutPipes;
    public int[] ep_num = new int[6]; /* endpoint number */
}