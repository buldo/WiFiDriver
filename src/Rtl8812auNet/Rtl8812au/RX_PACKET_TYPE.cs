using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum RX_PACKET_TYPE
{
    NORMAL_RX,/* Normal rx packet */
    TX_REPORT1,/* CCX */
    TX_REPORT2,/* TX RPT */
    HIS_REPORT,/* USB HISR RPT */
    C2H_PACKET
}