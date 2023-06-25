using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly IRtlUsbDevice _usbDevice;
    private readonly AdapterState _adapterState;
    private Task _readTask;

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
            cur_channel = 8
        });

        SetMonitorChannel(_adapterState, new InitChannel()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        _readTask = Task.Run(() => _usbDevice.InfinityRead());
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
}

public static class FrameParsing
{
    public static void recvbuf2recvframe(AdapterState _adapterState, byte[] pskb)
    {
        u8 pkt_cnt = 0;
        int pkt_offset;
        u8[] pphy_status = null;
        recv_frame precvframe = null;

        rx_pkt_attrib pattrib = null;
        HAL_DATA_TYPE pHalData = _adapterState.HalData;
        //recv_priv precvpriv = _adapterState.recvpriv;
        //_queue pfree_recv_queue = precvpriv.free_recv_queue;

        s32 transfer_len = (s32)pskb.Length;
        Memory<byte> pbuf = pskb;

        do
        {
            precvframe = new recv_frame();
            rtl8812_query_rx_desc_status(precvframe, pbuf.Span);

            pattrib = precvframe.hdr.attrib;

            if (((pattrib.crc_err) || (pattrib.icv_err)))
            {
                RTW_INFO("%s: RX Warning! crc_err=%d icv_err=%d, skip!\n", pattrib.crc_err, pattrib.icv_err);
                return;
                //rtw_free_recvframe(precvframe, pfree_recv_queue);
                //goto _exit_recvbuf2recvframe;
            }

            pkt_offset = RXDESC_SIZE + pattrib.drvinfo_sz + pattrib.shift_sz + pattrib.pkt_len;

            if ((pattrib.pkt_len <= 0) || (pkt_offset > transfer_len))
            {
                //RTW_INFO("%s()-%d: RX Warning!,pkt_len<=0 or pkt_offset> transfer_len\n", __FUNCTION__, __LINE__);
                return;
            }

            //if (check_fwstate(_adapterState.mlmepriv, WIFI_MONITOR_STATE) == false)
            //    if ((pattrib.pkt_rpt_type == NORMAL_RX) && rtw_hal_rcr_check(padapter, RCR_APPFCS))
            //    {
            //        pattrib.pkt_len -= IEEE80211_FCS_LEN;
            //    }

            if (rtw_os_alloc_recvframe(_adapterState, precvframe, pbuf.Slice(pattrib.shift_sz + pattrib.drvinfo_sz + RXDESC_SIZE).Span) == false)
            {
                //rtw_free_recvframe(precvframe, pfree_recv_queue);

                goto _exit_recvbuf2recvframe;
            }

            recvframe_put(precvframe, pattrib.pkt_len);

            if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.NORMAL_RX) /* Normal rx packet */
            {
                pre_recv_entry(precvframe, pattrib.physt ? (pbuf.Slice(RXDESC_OFFSET)) : null);
            }
            else
            {
                if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.C2H_PACKET)
                {
                    //rtw_hal_c2h_pkt_pre_hdl(_adapterState, precvframe.hdr.rx_data, pattrib.pkt_len);
                }
                else if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.HIS_REPORT)
                {
                }

                //rtw_free_recvframe(precvframe, pfree_recv_queue);
            }

            transfer_len -= pkt_offset;
            precvframe = null;

        } while (transfer_len > 0);

        _exit_recvbuf2recvframe:

        return;
    }

    static bool pre_recv_entry(recv_frame precvframe, Memory<byte>? pphy_status)
    {
        bool ret = true;
        u8[] pbuf = precvframe.hdr.rx_data;
        u8[] pda = wifi_get_ra(pbuf);
        bool ra_is_bmc = IS_MCAST(pda);
        _adapter primary_padapter = precvframe.hdr.adapter;

        if (primary_padapter.registrypriv.mp_mode != 1)
        {
            /* skip unnecessary bmc data frame for primary adapter */
            if (ra_is_bmc == true && GetFrameType(pbuf) == WIFI_DATA_TYPE
                                   && !adapter_allow_bmc_data_rx(precvframe.hdr.adapter)
               )
            {
                rtw_free_recvframe(precvframe, precvframe.hdr.adapter.recvpriv.free_recv_queue);
                goto exit;
            }
        }

        if (pphy_status != null)
        {
            rx_query_phy_status(precvframe, pphy_status);
        }
        ret = rtw_recv_entry(precvframe);

        exit:
        return ret;
    }

    static void recvframe_put(recv_frame precvframe, int sz)
    {
        /* rx_tai += sz; move rx_tail sz bytes  hereafter */

        /* used for append sz bytes from ptr to rx_tail, update rx_tail and return the updated rx_tail to the caller */
        /* after putting, rx_tail must be still larger than rx_end. */
        byte[] prev_rx_tail;

        /* RTW_INFO("recvframe_put: len=%d\n", sz); */

        if (precvframe == null)
        {
            return;
        }

        prev_rx_tail = precvframe.hdr.rx_tail;

        precvframe.hdr.rx_tail += sz;

        if (precvframe.hdr.rx_tail > precvframe.hdr.rx_end)
        {
            precvframe.hdr.rx_tail -= sz;
            return;
        }

        precvframe.hdr.len += sz;
    }

    static bool rtw_os_alloc_recvframe(AdapterState padapter, recv_frame precvframe, Span<byte> pdata)
    {
        bool res = true;

        if (pdata == null)
        {
            precvframe.hdr.pkt = null;
            res = false;
            return res;
        }

        rx_pkt_attrib pattrib = precvframe.hdr.attrib;

        // Modified by Albert 20101213
        // For 8 bytes IP header alignment.
        u8 shift_sz = pattrib.qos ? (u8)6 : (u8)0; /*	Qos data, wireless lan header length is 26 */

        u32 skb_len = pattrib.pkt_len;

        /* for first fragment packet, driver need allocate 1536+drvinfo_sz+RXDESC_SIZE to defrag packet. */
        /* modify alloc_sz for recvive crc error packet by thomas 2011-06-02 */
        u32 alloc_sz;
        if (pattrib.mfrag && (pattrib.frag_num == 0))
        {
            /* alloc_sz = 1664;	 */ /* 1664 is 128 alignment. */
            alloc_sz = (skb_len <= 1650) ? 1664 : (skb_len + 14);
        }
        else
        {
            alloc_sz = skb_len;
            /*	6 is for IP header 8 bytes alignment in QoS packet case. */
            /*	8 is for skb.data 4 bytes alignment. */
            alloc_sz += 14;
        }

        var pkt_copy = new sk_buff();

        if (pkt_copy != null)
        {
            //pkt_copy.dev = padapter.pnetdev;
            pkt_copy.len = skb_len;
            precvframe.hdr.pkt = pkt_copy;
            precvframe.hdr.rx_head = pkt_copy.head;
            precvframe.hdr.rx_end = pkt_copy.data + alloc_sz;
            skb_reserve(pkt_copy, 8 - ((SIZE_PTR)(pkt_copy.data) & 7)); /* force pkt_copy.data at 8-byte alignment address */
            skb_reserve(pkt_copy, shift_sz); /* force ip_hdr at 8-byte alignment address according to shift_sz. */
            _rtw_memcpy(pkt_copy.data, pdata, skb_len);
            precvframe.hdr.rx_data = precvframe.hdr.rx_tail = pkt_copy.data;
        }
        //else
        //{
        //    if ((pattrib.mfrag) && (pattrib.frag_num == 0))
        //    {
        //        RTW_INFO("rtw_os_alloc_recvframe: alloc_skb fail , drop frag frame");
        //        /* rtw_free_recvframe(precvframe, pfree_recv_queue); */
        //        res = false;
        //        goto exit_rtw_os_recv_resource_alloc;
        //    }

        //    if (pskb == null)
        //    {
        //        res = false;
        //        goto exit_rtw_os_recv_resource_alloc;
        //    }

        //    precvframe.hdr.pkt = rtw_skb_clone(pskb);
        //    if (precvframe.hdr.pkt)
        //    {
        //        precvframe.hdr.pkt.dev = padapter.pnetdev;
        //        precvframe.hdr.rx_head = precvframe.hdr.rx_data = precvframe.u.hdr.rx_tail = pdata;
        //        precvframe.hdr.rx_end = pdata + alloc_sz;
        //    }
        //    else
        //    {
        //        RTW_INFO("rtw_os_alloc_recvframe: rtw_skb_clone fail");
        //        /* rtw_free_recvframe(precvframe, pfree_recv_queue); */
        //        /*exit_rtw_os_recv_resource_alloc;*/
        //        res = false;
        //    }
        //}

        exit_rtw_os_recv_resource_alloc:

        return res;

    }

    private static void rtl8812_query_rx_desc_status(recv_frame precvframe, Span<byte> pdesc)
    {
        var pattrib = precvframe.hdr.attrib;

        /* Offset 0 */
        pattrib.pkt_len = GET_RX_STATUS_DESC_PKT_LEN_8812(pdesc); /* (le32_to_cpu(pdesc.rxdw0)&0x00003fff) */
        pattrib.crc_err = GET_RX_STATUS_DESC_CRC32_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 14) & 0x1); */
        pattrib.icv_err = GET_RX_STATUS_DESC_ICV_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 15) & 0x1); */
        pattrib.drvinfo_sz = (u8)(GET_RX_STATUS_DESC_DRVINFO_SIZE_8812(pdesc) * 8); /* ((le32_to_cpu(pdesc.rxdw0) >> 16) & 0xf) * 8; */ /* uint 2^3 = 8 bytes */
        pattrib.encrypt = GET_RX_STATUS_DESC_SECURITY_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 20) & 0x7); */
        pattrib.qos = GET_RX_STATUS_DESC_QOS_8812(pdesc); /* (( le32_to_cpu( pdesc.rxdw0 ) >> 23) & 0x1); */ /* Qos data, wireless lan header length is 26 */
        pattrib.shift_sz = GET_RX_STATUS_DESC_SHIFT_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 24) & 0x3); */
        pattrib.physt = GET_RX_STATUS_DESC_PHY_STATUS_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 26) & 0x1); */
        pattrib.bdecrypted = !GET_RX_STATUS_DESC_SWDEC_8812(pdesc); /* (le32_to_cpu(pdesc.rxdw0) & BIT(27))? 0:1; */

        /* Offset 4 */
        pattrib.priority = GET_RX_STATUS_DESC_TID_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw1) >> 8) & 0xf); */
        pattrib.mdata = GET_RX_STATUS_DESC_MORE_DATA_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw1) >> 26) & 0x1); */
        pattrib.mfrag = GET_RX_STATUS_DESC_MORE_FRAG_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw1) >> 27) & 0x1); */ /* more fragment bit */

        /* Offset 8 */
        pattrib.seq_num = GET_RX_STATUS_DESC_SEQ_8812(pdesc); /* (le32_to_cpu(pdesc.rxdw2) & 0x00000fff); */
        pattrib.frag_num = GET_RX_STATUS_DESC_FRAG_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw2) >> 12) & 0xf); */ /* fragmentation number */

        if (GET_RX_STATUS_DESC_RPT_SEL_8812(pdesc))
        {
            pattrib.pkt_rpt_type = RX_PACKET_TYPE.C2H_PACKET;
        }
        else
        {
            pattrib.pkt_rpt_type = RX_PACKET_TYPE.NORMAL_RX;
        }

        /* Offset 12 */
        pattrib.data_rate = GET_RX_STATUS_DESC_RX_RATE_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw3))&0x7f); */

        /* Offset 16 */
        pattrib.sgi = GET_RX_STATUS_DESC_SPLCP_8812(pdesc);
        pattrib.ldpc = GET_RX_STATUS_DESC_LDPC_8812(pdesc);
        pattrib.stbc = GET_RX_STATUS_DESC_STBC_8812(pdesc);
        pattrib.bw = (u8)GET_RX_STATUS_DESC_BW_8812(pdesc);

        /* Offset 20 */
        /* pattrib.tsfl=(u8)GET_RX_STATUS_DESC_TSFL_8812(pdesc); */
    }

    private static u16 GET_RX_STATUS_DESC_PKT_LEN_8812(Span<byte> __pRxStatusDesc)
        => (u16)LE_BITS_TO_4BYTE(__pRxStatusDesc, 0, 14);

    private static bool GET_RX_STATUS_DESC_CRC32_8812(Span<byte> __pRxStatusDesc)
        => LE_BITS_TO_4BYTE(__pRxStatusDesc, 14, 1) != 0;

    private static bool GET_RX_STATUS_DESC_ICV_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 15, 1) != 0;

    private static u8 GET_RX_STATUS_DESC_DRVINFO_SIZE_8812(Span<byte> __pRxStatusDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxStatusDesc, 16, 4);

    private static u8 GET_RX_STATUS_DESC_SECURITY_8812(Span<byte> __pRxStatusDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxStatusDesc, 20, 3);

    private static bool GET_RX_STATUS_DESC_QOS_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 23, 1) != 0;

    private static u8 GET_RX_STATUS_DESC_SHIFT_8812(Span<byte> __pRxStatusDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxStatusDesc, 24, 2);

    private static bool GET_RX_STATUS_DESC_PHY_STATUS_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 26, 1) != 0;

    private static bool GET_RX_STATUS_DESC_SWDEC_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 27, 1) != 0;

    private static u8 GET_RX_STATUS_DESC_TID_8812(Span<byte> __pRxDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 8, 4);

    private static bool GET_RX_STATUS_DESC_MORE_DATA_8812(Span<byte> __pRxDesc) =>
        LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 26, 1) != 0;

    private static bool GET_RX_STATUS_DESC_MORE_FRAG_8812(Span<byte> __pRxDesc) =>
        LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 27, 1) != 0;

    private static u16 GET_RX_STATUS_DESC_SEQ_8812(Span<byte> __pRxStatusDesc) =>
        (u16)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 0, 12);

    private static u8 GET_RX_STATUS_DESC_FRAG_8812(Span<byte> __pRxStatusDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 12, 4);

    private static bool GET_RX_STATUS_DESC_RPT_SEL_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 28, 1) != 0;

    private static u8 GET_RX_STATUS_DESC_RX_RATE_8812(Span<byte> __pRxStatusDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(12), 0, 7);

    private static u8 GET_RX_STATUS_DESC_SPLCP_8812(Span<byte> __pRxDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 0, 1);

    private static u8 GET_RX_STATUS_DESC_LDPC_8812(Span<byte> __pRxDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 1, 1);

    private static u8 GET_RX_STATUS_DESC_STBC_8812(Span<byte> __pRxDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 2, 1);

    private static u8 GET_RX_STATUS_DESC_BW_8812(Span<byte> __pRxDesc) =>
        (u8)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 4, 2);
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
    u8[] rx_data;
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