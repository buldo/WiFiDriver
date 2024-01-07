using Microsoft.Extensions.Logging;
using Rtl8812auNet.Rtl8812au.Enumerations;
using Rtl8812auNet.Rtl8812au.Models;

namespace Rtl8812auNet.Rtl8812au;

public class FrameParser
{
    private readonly ILogger<FrameParser> _logger;

    public FrameParser(ILogger<FrameParser> logger)
    {
        _logger = logger;
    }

    public List<ParsedRadioPacket> ParsedRadioPacket(ReadOnlySpan<byte> usbTransfer)
    {

        return recvbuf2recvframe(usbTransfer)
            .Select(tuple => new ParsedRadioPacket
            {
                Data = tuple.Data,
                Attr = tuple.RxAtrib
            })
            .ToList();
    }

    private List<(rx_pkt_attrib RxAtrib, byte[] Data)> recvbuf2recvframe(ReadOnlySpan<byte> ptr)
    {
        var pbuf = ptr;
        var pkt_cnt = GET_RX_STATUS_DESC_USB_AGG_PKTNUM_8812(pbuf);
        _logger.LogInformation($"pkt_cnt == {pkt_cnt}");

        List<(rx_pkt_attrib RxAtrib, byte[] Data)> ret = new();

        do
        {
            var pattrib = rtl8812_query_rx_desc_status(pbuf);

            if ((pattrib.crc_err) || (pattrib.icv_err))
            {
                _logger.LogInformation($"RX Warning! crc_err={pattrib.crc_err} icv_err={pattrib.icv_err}, skip!");
                break;
            }

            var pkt_offset = RXDESC_SIZE + pattrib.drvinfo_sz + pattrib.shift_sz + pattrib.pkt_len; // this is offset for next package

            if ((pattrib.pkt_len <= 0) || (pkt_offset > pbuf.Length))
            {
                _logger.LogWarning(
                    "RX Warning!,pkt_len <= 0 or pkt_offset > transfer_len; pkt_len: {pkt_len}, pkt_offset: {pkt_offset}, transfer_len: {transfer_len}",
                    pattrib.pkt_len, pkt_offset, pbuf.Length);
                break;
            }

            if (pattrib.mfrag)
            {
                // !!! We skips this packages because ohd not use fragmentation
                _logger.LogWarning("mfrag scipping");

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
                    _logger.LogInformation("RX USB C2H_PACKET");
                    //rtw_hal_c2h_pkt_pre_hdl(padapter, precvframe.u.hdr.rx_data, pattrib.pkt_len);
                }
                else if (pattrib.pkt_rpt_type == RX_PACKET_TYPE.HIS_REPORT)
                {
                    _logger.LogInformation("RX USB HIS_REPORT");
                }
            }

            /* jaguar 8-byte alignment */
            pkt_offset = (UInt16)_RND8(pkt_offset);
            //pkt_cnt--;

            if (pkt_offset >= pbuf.Length)
            {
                break;
            }
            pbuf = pbuf.Slice(pkt_offset);
        } while (pbuf.Length > 0);

        //if (pkt_cnt != 0)
        //{
        //    RTW_WARN($"Unprocessed packets: {pkt_cnt}");
        //}
        _logger.LogInformation($"{ret.Count} received in frame");

        return ret;
    }

    private static uint GET_RX_STATUS_DESC_USB_AGG_PKTNUM_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(12), 16, 8);

    private static UInt32 _RND8(int sz)
    {
        UInt32 val = (uint)(((sz >> 3) + ((sz & 7) != 0 ? 1 : 0)) << 3);
        return val;
    }

    private static rx_pkt_attrib rtl8812_query_rx_desc_status(ReadOnlySpan<byte> pdesc)
    {
        var pattrib = new rx_pkt_attrib();

        /* Offset 0 */
        pattrib.pkt_len = GET_RX_STATUS_DESC_PKT_LEN_8812(pdesc); /* (le32_to_cpu(pdesc.rxdw0)&0x00003fff) */
        pattrib.crc_err = GET_RX_STATUS_DESC_CRC32_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 14) & 0x1); */
        pattrib.icv_err = GET_RX_STATUS_DESC_ICV_8812(pdesc); /* ((le32_to_cpu(pdesc.rxdw0) >> 15) & 0x1); */
        pattrib.drvinfo_sz = (byte)(GET_RX_STATUS_DESC_DRVINFO_SIZE_8812(pdesc) * 8); /* ((le32_to_cpu(pdesc.rxdw0) >> 16) & 0xf) * 8; */ /* uint 2^3 = 8 bytes */
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
        /* pattrib.tsfl=(byte)GET_RX_STATUS_DESC_TSFL_8812(pdesc); */

        return pattrib;
    }

    private static UInt16 GET_RX_STATUS_DESC_PKT_LEN_8812(ReadOnlySpan<byte> __pRxStatusDesc)
        => (UInt16)LE_BITS_TO_4BYTE(__pRxStatusDesc, 0, 14);

    private static bool GET_RX_STATUS_DESC_CRC32_8812(ReadOnlySpan<byte> __pRxStatusDesc)
        => LE_BITS_TO_4BYTE(__pRxStatusDesc, 14, 1) != 0;

    private static bool GET_RX_STATUS_DESC_ICV_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 15, 1) != 0;

    private static byte GET_RX_STATUS_DESC_DRVINFO_SIZE_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxStatusDesc, 16, 4);

    private static byte GET_RX_STATUS_DESC_SECURITY_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxStatusDesc, 20, 3);

    private static bool GET_RX_STATUS_DESC_QOS_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 23, 1) != 0;

    private static byte GET_RX_STATUS_DESC_SHIFT_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxStatusDesc, 24, 2);

    private static bool GET_RX_STATUS_DESC_PHY_STATUS_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 26, 1) != 0;

    private static bool GET_RX_STATUS_DESC_SWDEC_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc, 27, 1) != 0;

    private static byte GET_RX_STATUS_DESC_TID_8812(ReadOnlySpan<byte> __pRxDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 8, 4);

    private static bool GET_RX_STATUS_DESC_MORE_DATA_8812(ReadOnlySpan<byte> __pRxDesc) =>
        LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 26, 1) != 0;

    private static bool GET_RX_STATUS_DESC_MORE_FRAG_8812(ReadOnlySpan<byte> __pRxDesc) =>
        LE_BITS_TO_4BYTE(__pRxDesc.Slice(4), 27, 1) != 0;

    private static UInt16 GET_RX_STATUS_DESC_SEQ_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (UInt16)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 0, 12);

    private static byte GET_RX_STATUS_DESC_FRAG_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 12, 4);

    private static bool GET_RX_STATUS_DESC_RPT_SEL_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(8), 28, 1) != 0;

    private static byte GET_RX_STATUS_DESC_RX_RATE_8812(ReadOnlySpan<byte> __pRxStatusDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxStatusDesc.Slice(12), 0, 7);

    private static byte GET_RX_STATUS_DESC_SPLCP_8812(ReadOnlySpan<byte> __pRxDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 0, 1);

    private static byte GET_RX_STATUS_DESC_LDPC_8812(ReadOnlySpan<byte> __pRxDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 1, 1);

    private static byte GET_RX_STATUS_DESC_STBC_8812(ReadOnlySpan<byte> __pRxDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 2, 1);

    private static byte GET_RX_STATUS_DESC_BW_8812(ReadOnlySpan<byte> __pRxDesc) =>
        (byte)LE_BITS_TO_4BYTE(__pRxDesc.Slice(16), 4, 2);
}