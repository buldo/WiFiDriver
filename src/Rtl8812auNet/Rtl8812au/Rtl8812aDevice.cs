namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly RtlUsbAdapter _usbDevice;
    private readonly AdapterState _adapterState;
    private readonly StatefulFrameParser _frameParser = new();
    private Task _readTask;
    private Task _parseTask;

    public Rtl8812aDevice(RtlUsbAdapter usbDevice)
    {
        _usbDevice = usbDevice;
        var dvobj = InitDvObj(_usbDevice);
        _adapterState = InitAdapter(dvobj, _usbDevice);
    }

    public void Init()
    {

        StartWithMonitorMode(new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        SetMonitorChannel(_adapterState, new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        _readTask = Task.Run(() => _usbDevice.UsbDevice.InfinityRead());
        _parseTask = Task.Run(() => ParseUsbData());
    }

    private void StartWithMonitorMode(InitChannel initChannel)
    {
        if (NetDevOpen(initChannel) == false)
        {
            throw new Exception("StartWithMonitorMode failed NetDevOpen");
        }

        setopmode_hdl(_adapterState);
    }

    private void SetMonitorChannel(AdapterState padapter, InitChannel chandef)
    {
        set_channel_bwmode(padapter, chandef.cur_channel, chandef.cur_ch_offset, chandef.cur_bwmode);
    }

    private bool NetDevOpen(InitChannel initChannel)
    {
        var status = rtw_hal_init(_adapterState, initChannel);
        if (status == false)
        {
            return false;
        }

        return true;
    }

    private async Task ParseUsbData()
    {
        await foreach (var transfer in _usbDevice.UsbDevice.BulkTransfersReader.ReadAllAsync())
        {
            var packet = _frameParser.ParsedRadioPacket(transfer);
            foreach (var radioPacket in packet)
            {
                Console.WriteLine(Convert.ToHexString(radioPacket.Data));
            }
        }
    }
}

public class rx_pkt_attrib
{
    public u16 pkt_len;
    public bool physt;
    public u8 drvinfo_sz;
    public u8 shift_sz;
    u8 hdrlen; /* the WLAN Header Len */
    u8 to_fr_ds;
    u8 amsdu;
    public bool qos;
    public u8 priority;
    u8 pw_save;
    public bool mdata;
    public u16 seq_num;
    public u8 frag_num;
    public bool mfrag;
    u8 order;
    u8 privacy; /* in frame_ctrl field */
    public bool bdecrypted;
    public u8 encrypt; /* when 0 indicate no encrypt. when non-zero, indicate the encrypt algorith */
    u8 iv_len;
    u8 icv_len;
    public bool crc_err;
    public bool icv_err;

    u16 eth_type;

    //u8 dst[ETH_ALEN];
    //u8 src[ETH_ALEN];
    //u8 ta[ETH_ALEN];
    //u8 ra[ETH_ALEN];
    //u8 bssid[ETH_ALEN];
//# ifdef CONFIG_RTW_MESH
//    u8 msa[ETH_ALEN]; /* mesh sa */
//    u8 mda[ETH_ALEN]; /* mesh da */
//    u8 mesh_ctrl_present;
//    u8 mesh_ctrl_len; /* length of mesh control field */
//#endif

    u8 ack_policy;

    u8 key_index;

    public u8 data_rate;
    u8 ch; /* RX channel */
    public u8 bw;
    public u8 stbc;
    public u8 ldpc;
    public u8 sgi;
    public RX_PACKET_TYPE pkt_rpt_type;
    u32 tsfl;
    //u32 MacIDValidEntry[2]; /* 64 bits present 64 entry. */
    u8 ppdu_cnt;
    u32 free_cnt;       /* free run counter */
    //struct phydm_phyinfo_struct phy_info;

//#ifdef CONFIG_TCP_CSUM_OFFLOAD_RX
//    /* checksum offload realted varaiables */
//    u8 csum_valid;      /* Checksum valid, 0: not check, 1: checked */
//    u8 csum_err;		/* Checksum Error occurs */
//#endif /* CONFIG_TCP_CSUM_OFFLOAD_RX */
}