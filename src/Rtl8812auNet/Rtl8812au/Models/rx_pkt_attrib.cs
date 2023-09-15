using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class rx_pkt_attrib
{
    public u16 pkt_len { get; set; }
    public bool physt { get; set; }
    public u8 drvinfo_sz { get; set; }
    public u8 shift_sz { get; set; }
    public bool qos { get; set; }
    public u8 priority { get; set; }
    public bool mdata { get; set; }
    public u16 seq_num { get; set; }
    public u8 frag_num { get; set; }
    public bool mfrag { get; set; }
    public bool bdecrypted { get; set; }
    public u8 encrypt { get; set; } /* when 0 indicate no encrypt. when non-zero, indicate the encrypt algorith */
    public bool crc_err { get; set; }
    public bool icv_err { get; set; }
    public u8 data_rate { get; set; }
    public u8 bw { get; set; }
    public u8 stbc { get; set; }
    public u8 ldpc { get; set; }
    public u8 sgi { get; set; }
    public RX_PACKET_TYPE pkt_rpt_type { get; set; }
}