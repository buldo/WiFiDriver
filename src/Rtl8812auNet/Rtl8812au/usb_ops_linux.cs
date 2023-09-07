using Rtl8812auNet.Rtl8812au;

namespace Rtl8812auNet.Rtl8812au;

public static class usb_ops_linux
{
    public static List<(rx_pkt_attrib RxAtrib, byte[] Data)> recvbuf2recvframe(byte[] ptr)
    {
        var transfer_len = ptr.Length;
        var pbuf = ptr.AsSpan();
        var pkt_cnt = GET_RX_STATUS_DESC_USB_AGG_PKTNUM_8812(pbuf);
        RTW_INFO($"pkt_cnt == {pkt_cnt}");

        List<(rx_pkt_attrib RxAtrib, byte[] Data)> ret = new();

        do
        {
            var pattrib = rtl8812_query_rx_desc_status(pbuf);

            if ((pattrib.crc_err) || (pattrib.icv_err))
            {
                RTW_INFO($"RX Warning! crc_err={pattrib.crc_err} icv_err={pattrib.icv_err}, skip!");
                break;
            }

            var pkt_offset = RXDESC_SIZE + pattrib.drvinfo_sz + pattrib.shift_sz + pattrib.pkt_len; // this is offset for next package

            if ((pattrib.pkt_len <= 0) || (pkt_offset > transfer_len))
            {
                RTW_WARN("RX Warning!,pkt_len<=0 or pkt_offset> transfer_len");
                break;
            }

            if (pattrib.mfrag == true)
            {
                // !!! We skips this packages because ohd not use fragmentation
                RTW_WARN("mfrag scipping");

                //if (rtw_os_alloc_recvframe(precvframe, pbuf.Slice(pattrib.shift_sz + pattrib.drvinfo_sz + RXDESC_SIZE), pskb) == false)
                //{
                //    return false;
                //}
            }



            // recvframe_put(precvframe, pattrib.pkt_len);
            /* recvframe_pull(precvframe, drvinfo_sz + RXDESC_SIZE); */

            if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.NORMAL_RX) /* Normal rx packet */
            {
                ret.Add((pattrib, pbuf.Slice(pattrib.shift_sz + pattrib.drvinfo_sz + RXDESC_SIZE, pattrib.pkt_len).ToArray()));
                //pre_recv_entry(precvframe, pattrib.physt ? pbuf.Slice(RXDESC_OFFSET) : null);
            }
            else
            {
                /* pkt_rpt_type == TX_REPORT1-CCX, TX_REPORT2-TX RTP,HIS_REPORT-USB HISR RTP */
                if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.C2H_PACKET)
                {
                    RTW_INFO("RX USB C2H_PACKET");
                    //rtw_hal_c2h_pkt_pre_hdl(padapter, precvframe.u.hdr.rx_data, pattrib.pkt_len);
                }
                else if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.HIS_REPORT)
                {
                    RTW_INFO("RX USB HIS_REPORT");
                }
            }

            /* jaguar 8-byte alignment */
            pkt_offset = (u16)_RND8(pkt_offset);
            //pkt_cnt--;

            if (pkt_offset >= pbuf.Length)
            {
                break;
            }
            pbuf = pbuf.Slice(pkt_offset);

            transfer_len -= pkt_offset;
        } while (transfer_len > 0);

        //if (pkt_cnt != 0)
        //{
        //    RTW_WARN($"Unprocessed packets: {pkt_cnt}");
        //}
        RTW_INFO($"{ret.Count} received in frame");

        return ret;
    }

    //private static s32 pre_recv_entry(recv_frame precvframe, u8[] pphy_status)
    //{
    //    if (pphy_status)
    //    {
    //        rx_query_phy_status(precvframe, pphy_status);
    //    }

    //    ret = rtw_recv_entry(precvframe);

    //    exit:
    //    return ret;
    //}

    //private static bool rtw_os_alloc_recvframe(_adapter padapter, recv_frame precvframe, u8[] pdata, _pkt[] pskb)
    //{
    //    bool res = true;
    //    u32 alloc_sz;

    //    rx_pkt_attrib pattrib = precvframe.hdr.attrib;

    //    if (pdata == null)
    //    {
    //        precvframe.hdr.pkt = null;
    //        res = false;
    //        return res;
    //    }

    //    /* Modified by Albert 20101213 */
    //    /* For 8 bytes IP header alignment. */
    //    u8 shift_sz = pattrib.qos ? (u8)6 : (u8)0; /*	Qos data, wireless lan header length is 26 */

    //    u32 skb_len = pattrib.pkt_len;

    //    /* for first fragment packet, driver need allocate 1536+drvinfo_sz+RXDESC_SIZE to defrag packet. */
    //    /* modify alloc_sz for recvive crc error packet by thomas 2011-06-02 */
    //    if ((pattrib.mfrag == true) && (pattrib.frag_num == 0))
    //    {
    //        /* alloc_sz = 1664;	 */ /* 1664 is 128 alignment. */
    //        alloc_sz = (skb_len <= 1650) ? 1664 : (skb_len + 14);
    //    }
    //    else
    //    {
    //        alloc_sz = skb_len;
    //        /*	6 is for IP header 8 bytes alignment in QoS packet case. */
    //        /*	8 is for skb->data 4 bytes alignment. */
    //        alloc_sz += 14;
    //    }

    //    _pkt pkt_copy = rtw_skb_alloc(alloc_sz);

    //    if (pkt_copy)
    //    {
    //        pkt_copy.dev = padapter.pnetdev;
    //        pkt_copy.len = skb_len;
    //        precvframe.hdr.pkt = pkt_copy;
    //        precvframe.hdr.rx_head = pkt_copy->head;
    //        precvframe.hdr.rx_end = pkt_copy->data + alloc_sz;
    //        skb_reserve(pkt_copy, 8 - ((SIZE_PTR)(pkt_copy->data) & 7)); /* force pkt_copy->data at 8-byte alignment address */
    //        skb_reserve(pkt_copy, shift_sz); /* force ip_hdr at 8-byte alignment address according to shift_sz. */
    //        _rtw_memcpy(pkt_copy.data, pdata, skb_len);
    //        precvframe.hdr.rx_data = precvframe.hdr.rx_tail = pkt_copy.data;
    //    }
    //    else
    //    {
    //        if ((pattrib.mfrag == true) && (pattrib.frag_num == 0))
    //        {
    //            RTW_INFO("alloc_skb fail , drop frag frame");
    //            /* rtw_free_recvframe(precvframe, pfree_recv_queue); */
    //            return false;
    //        }

    //        if (pskb == null)
    //        {
    //            return false;
    //        }

    //        precvframe.hdr.pkt = rtw_skb_clone(pskb);
    //        if (precvframe.hdr.pkt)
    //        {
    //            precvframe.hdr.rx_head = precvframe.hdr.rx_data = precvframe.hdr.rx_tail = pdata;
    //            precvframe.hdr.rx_end = pdata + alloc_sz;
    //        }
    //        else
    //        {
    //            RTW_INFO("rtw_skb_clone fail");
    //            /* rtw_free_recvframe(precvframe, pfree_recv_queue); */
    //            /*exit_rtw_os_recv_resource_alloc;*/
    //            res = false;
    //        }
    //    }

    //    return res;
    //}

    private static uint GET_RX_STATUS_DESC_USB_AGG_PKTNUM_8812(Span<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(12), 16, 8);

    private static u32 _RND8(int sz)
    {
        u32 val = (uint)(((sz >> 3) + ((sz & 7) != 0 ? 1 : 0)) << 3);
        return val;
    }

    private static rx_pkt_attrib rtl8812_query_rx_desc_status(Span<byte> pdesc)
    {
        var pattrib = new rx_pkt_attrib();

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
        pattrib.bw = GET_RX_STATUS_DESC_BW_8812(pdesc);

        /* Offset 20 */
        /* pattrib.tsfl=(u8)GET_RX_STATUS_DESC_TSFL_8812(pdesc); */

        return pattrib;
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