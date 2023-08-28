using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly IRtlUsbDevice _usbDevice;
    private readonly AdapterState _adapterState;
    private Task _readTask;
    private Task _parseTask;

    public Rtl8812aDevice(IRtlUsbDevice usbDevice)
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
            cur_channel = 140
        });

        SetMonitorChannel(_adapterState, new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140
        });

        _readTask = Task.Run(() => _usbDevice.InfinityRead());
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
        await foreach (var transfer in _usbDevice.BulkTransfersReader.ReadAllAsync())
        {

        }
    }
}

public class recv_frame_hdr
{
    //_list list;
    public sk_buff pkt;

    //_adapter* adapter;

    u8 fragcnt;

    int frame_tag;

    public rx_pkt_attrib attrib;

    public uint len;
    u8[] rx_head;
    public u8[] rx_data;
    u8[] rx_tail;
    u8[] rx_end;

    public byte[] precvbuf;


    /*  */
    //sta_info psta;

    /* for A-MPDU Rx reordering buffer control */
    //struct recv_reorder_ctrl *preorder_ctrl;

//#ifdef CONFIG_WAPI_SUPPORT
//    u8 UserPriority;
//    u8 WapiTempPN[16];
//    u8 WapiSrcAddr[6];
//    u8 bWapiCheckPNInDecrypt;
//    u8 bIsWaiPacket;
//#endif

};

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

public class sk_buff
{
//    /* These two members must be first. */
//    struct sk_buff      *next;
//	struct sk_buff      *prev;

//	ktime_t tstamp;

//    struct sock     *sk;
//	//struct net_device	*dev;
//	struct ifnet *dev;

//	/*
//	 * This is the control buffer. It is free to use for every
//	 * layer. Please put your private variables there. If you
//	 * want to keep them across layers you have to do a skb_clone()
//	 * first. This is owned by whoever has the skb queued ATM.
//	 */
//	char cb[48] __aligned(8);

//    unsigned long _skb_refdst;
//# ifdef CONFIG_XFRM
//    struct sec_path    *sp;
//#endif
public uint len,data_len;
//    u16 mac_len,
//                hdr_len;
//    union {
//		u32 csum;
//    struct {

//            u16 csum_start;
//    u16 csum_offset;
//}
//smbol2;
//	}smbol1;
//u32 priority;
//kmemcheck_bitfield_begin(flags1);
//u8 local_df:1,
//				cloned: 1,
//				ip_summed: 2,
//				nohdr: 1,
//				nfctinfo: 3;
//u8 pkt_type:3,
//				fclone: 2,
//				ipvs_property: 1,
//				peeked: 1,
//				nf_trace: 1;
//kmemcheck_bitfield_end(flags1);
//u16 protocol;

//void(*destructor)(struct sk_buff *skb);
//#if defined(CONFIG_NF_CONNTRACK) || defined(CONFIG_NF_CONNTRACK_MODULE)
//	struct nf_conntrack	*nfct;
//	struct sk_buff		*nfct_reasm;
//#endif
//# ifdef CONFIG_BRIDGE_NETFILTER
//struct nf_bridge_info   *nf_bridge;
//#endif

//int skb_iif;
//# ifdef CONFIG_NET_SCHED
//u16 tc_index;   /* traffic control index */
//# ifdef CONFIG_NET_CLS_ACT
//u16 tc_verd;    /* traffic control verdict */
//#endif
//#endif

//u32 rxhash;

//kmemcheck_bitfield_begin(flags2);
//u16 queue_mapping:16;
//# ifdef CONFIG_IPV6_NDISC_NODETYPE
//u8 ndisc_nodetype:2,
//				deliver_no_wcard: 1;
//#else
//u8 deliver_no_wcard:1;
//#endif
//kmemcheck_bitfield_end(flags2);

///* 0/14 bit hole */

//# ifdef CONFIG_NET_DMA
//dma_cookie_t dma_cookie;
//#endif
//# ifdef CONFIG_NETWORK_SECMARK
//u32 secmark;
//#endif
//union {
//		u32		mark;
//u32 dropcount;
//	}symbol3;

//u16 vlan_tci;

//sk_buff_data_t transport_header;
//sk_buff_data_t network_header;
//sk_buff_data_t mac_header;
///* These elements must be at the end, see alloc_skb() for details.  */
//sk_buff_data_t tail;
//sk_buff_data_t end;
//unsigned char		*head,
//				*data;
//unsigned int		truesize;
//atomic_t users;
};