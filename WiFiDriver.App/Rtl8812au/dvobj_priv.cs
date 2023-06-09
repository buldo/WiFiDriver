using LibUsbDotNet.LibUsb;
namespace WiFiDriver.App.Rtl8812au;

public class dvobj_priv
{
    private const int CONFIG_IFACE_NUMBER = 1;
    /*-------- below is common data --------*/
    public const CHIP_TYPE chip_type = CHIP_TYPE.RTL8812;
    public HARDWARE_TYPE HardwareType;
    public byte interface_type; /*USB,SDIO,SPI,PCI*/

    bool bSurpriseRemoved;
    public bool bDriverStopped;

    int processing_dev_remove;

    //debug_priv drv_dbg;

    //_mutex hw_init_mutex;
    //   _mutex h2c_fwcmd_mutex;
    //   _mutex setch_mutex;
    //   _mutex setbw_mutex;
    //   _mutex rf_read_reg_mutex;

    byte oper_channel; /* saved channel info when call set_channel_bw */
    byte oper_bwmode;

    byte oper_ch_offset; /* PRIME_CHNL_OFFSET */
    //systime on_oper_ch_time;

    public _adapter[] padapters { get; } =new _adapter[CONFIG_IFACE_NUMBER]; /*IFACE_ID_MAX*/
    public byte iface_nums; /* total number of ifaces used runtime */
    //mi_state iface_state;

    //systime periodic_tsf_update_etime;
    //_timer periodic_tsf_update_end_timer;

    //macid_ctl_t macid_ctl;

    //cam_ctl_t cam_ctl;
    //sec_cam_ent cam_cache[SEC_CAM_ENT_NUM_SW_LIMIT];

    //rf_ctl_t rf_ctl;

    /* For 92D, DMDP have 2 interface. */
    public byte InterfaceNumber;
    public byte NumInterfaces;

    /* In /Out Pipe information */
    public int[] RtInPipe = new int[2];
    public int[] RtOutPipe = new int[4];
    //byte Queue2Pipe[HW_QUEUE_ENTRY]; /* for out pipe mapping */

    byte irq_alloc;
    bool continual_io_error;

    bool disable_func;

    byte xmit_block;
    //_lock xmit_block_lock;

    pwrctrl_priv pwrctl_priv;

    //rtw_traffic_statistics traffic_stat;

//	_thread_hdl_ rtnl_lock_holder;
//    _timer dynamic_chk_timer; /* dynamic/periodic check timer */


    /*-------- below is for USB INTERFACE --------*/

    public u8 usb_speed; /* 1.1, 2.0 or 3.0 */
    public u8 nr_endpoint;
    public u8 RtNumInPipes;
    public u8 RtNumOutPipes;
    public int[] ep_num = new int[6]; /* endpoint number */

    int RegUsbSS;

    //_sema usb_suspend_sema;

    //_mutex usb_vendor_req_mutex;

    public UsbDevice pusbintf;
    public UsbDevice pusbdev;


    byte tpt_mode; /* RTK T/P Testing Mode, 0:default mode */
    u32 edca_be_ul;
    u32 edca_be_dl;

    /* also for RTK T/P Testing Mode */
    u8 scan_deny;

}