namespace WiFiDriver.App.Rtl8812au;

public static class odm_board_type
{
    public static byte ODM_BOARD_DEFAULT = 0;    /* The DEFAULT case. */
    public static byte ODM_BOARD_MINICARD = BIT(0); /* @0 = non-mini card, 1= mini card. */
    public static byte ODM_BOARD_SLIM = BIT(1); /* @0 = non-slim card, 1 = slim card */
    public static byte ODM_BOARD_BT = BIT(2); /* @0 = without BT card, 1 = with BT */
    public static byte ODM_BOARD_EXT_PA = BIT(3); /* @0 = no 2G ext-PA, 1 = existing 2G ext-PA */
    public static byte ODM_BOARD_EXT_LNA = BIT(4); /* @0 = no 2G ext-LNA, 1 = existing 2G ext-LNA */
    public static byte ODM_BOARD_EXT_TRSW = BIT(5); /* @0 = no ext-TRSW, 1 = existing ext-TRSW */
    public static byte ODM_BOARD_EXT_PA_5G = BIT(6); /* @0 = no 5G ext-PA, 1 = existing 5G ext-PA */
    public static byte ODM_BOARD_EXT_LNA_5G = BIT(7); /* @0 = no 5G ext-LNA, 1 = existing 5G ext-LNA */
};