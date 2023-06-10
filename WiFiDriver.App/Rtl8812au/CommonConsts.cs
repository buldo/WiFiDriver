namespace WiFiDriver.App.Rtl8812au;

public static class CommonConsts
{
    public const ushort REG_9346CR = 0x000A;
    public const ushort REG_EFUSE_CTRL = 0x0030;
    public const ushort EFUSE_CTRL = REG_EFUSE_CTRL;
    public const ushort REG_EFUSE_BURN_GNT_8812 = 0x00CF;
    public const ushort REG_SYS_ISO_CTRL = 0x0000;
    public const ushort REG_SYS_FUNC_EN = 0x0002;
    public const ushort REG_EFUSE_TEST = 0x0034;
    public const ushort REG_SYS_CLKR = 0x0008;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    //public const ushort ;
    public const int MAX_RF_PATH = 4;
    public const int RF_PATH_MAX = MAX_RF_PATH;
    public const int MAX_CHNL_GROUP_5G = 14;
    public const int MAX_CHNL_GROUP_24G = 6;
    public const int MAX_TX_COUNT = 4;
    public const byte EEPROM_DEFAULT_BOARD_OPTION = 0x00;
    public const byte EEPROM_RF_BOARD_OPTION_8812 = 0xC1;
    public const int CENTER_CH_5G_20M_NUM = (28 + 16);	/* 20M center channels */
    public const int CENTER_CH_5G_40M_NUM = 14;	/* 40M center channels */
    public const int CENTER_CH_5G_80M_NUM = 7; /* 80M center channels */
    public const int CENTER_CH_5G_ALL_NUM = (CENTER_CH_5G_20M_NUM + CENTER_CH_5G_40M_NUM + CENTER_CH_5G_80M_NUM);
    public const int CENTER_CH_2G_NUM = 14;
    public const byte PG_TXPWR_SRC_PG_DATA = 0;
    public const byte PG_TXPWR_SRC_IC_DEF = 1;
    public const byte PG_TXPWR_SRC_DEF = 2;
    public const byte PG_TXPWR_SRC_NUM = 3;
    public const sbyte PG_TXPWR_INVALID_DIFF = 8;
    public const byte PG_TXPWR_INVALID_BASE = 255;
    public const byte BT_RTL8812A = 11;
    public const byte EEPROM_RF_BT_SETTING_8812 = 0xC3;
    public const byte EEPROM_THERMAL_METER_8812 = 0xBA;
    public const byte EEPROM_Default_ThermalMeter_8812 = 0x18;
    public const byte EEPROM_USB_OPTIONAL_FUNCTION0 = 0xD4;
}