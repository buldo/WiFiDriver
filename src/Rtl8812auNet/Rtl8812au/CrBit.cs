using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[Flags]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum CrBit : short
{
    HCI_TXDMA_EN = 1 << (0),
    HCI_RXDMA_EN = 1 << (1),
    TXDMA_EN = 1 << (2),
    RXDMA_EN = 1 << (3),
    PROTOCOL_EN = 1 << (4),
    SCHEDULE_EN = 1 << (5),
    MACTXEN = 1 << (6),
    MACRXEN = 1 << (7),
    ENSWBCN = 1 << (8),
    ENSEC = 1 << (9),
    CALTMR_EN = 1 << (10) /* 32k CAL TMR enable */
}