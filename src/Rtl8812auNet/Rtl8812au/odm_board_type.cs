using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class odm_board_type
{
    public static uint ODM_BOARD_DEFAULT = 0;    /* The DEFAULT case. */
    public static uint ODM_BOARD_MINICARD = BIT0; /* @0 = non-mini card, 1= mini card. */
    public static uint ODM_BOARD_SLIM = BIT1; /* @0 = non-slim card, 1 = slim card */
    public static uint ODM_BOARD_BT = BIT2; /* @0 = without BT card, 1 = with BT */
    public static uint ODM_BOARD_EXT_PA = BIT3; /* @0 = no 2G ext-PA, 1 = existing 2G ext-PA */
    public static uint ODM_BOARD_EXT_LNA = BIT4; /* @0 = no 2G ext-LNA, 1 = existing 2G ext-LNA */
    public static uint ODM_BOARD_EXT_TRSW = BIT5; /* @0 = no ext-TRSW, 1 = existing ext-TRSW */
    public static uint ODM_BOARD_EXT_PA_5G = BIT6; /* @0 = no 5G ext-PA, 1 = existing 5G ext-PA */
    public static uint ODM_BOARD_EXT_LNA_5G = BIT7; /* @0 = no 5G ext-LNA, 1 = existing 5G ext-LNA */
};