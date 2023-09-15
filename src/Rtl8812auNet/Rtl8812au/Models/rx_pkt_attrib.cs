using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class rx_pkt_attrib
{
    public UInt16 pkt_len { get; set; }
    public bool physt { get; set; }
    public byte drvinfo_sz { get; set; }
    public byte shift_sz { get; set; }
    public bool qos { get; set; }
    public byte priority { get; set; }
    public bool mdata { get; set; }
    public UInt16 seq_num { get; set; }
    public byte frag_num { get; set; }
    public bool mfrag { get; set; }
    public bool bdecrypted { get; set; }
    public byte encrypt { get; set; } /* when 0 indicate no encrypt. when non-zero, indicate the encrypt algorith */
    public bool crc_err { get; set; }
    public bool icv_err { get; set; }
    public byte data_rate { get; set; }
    public byte bw { get; set; }
    public byte stbc { get; set; }
    public byte ldpc { get; set; }
    public byte sgi { get; set; }
    public RX_PACKET_TYPE pkt_rpt_type { get; set; }
}