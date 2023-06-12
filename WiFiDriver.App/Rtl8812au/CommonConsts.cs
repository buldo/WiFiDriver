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
    public const byte EEPROM_USB_MODE_8812 = 0x08;
    public const byte EEPROM_RFE_OPTION_8812 = 0xCA;
    public const byte EEPROM_COUNTRY_CODE_8812 = 0xCB;
    public const byte EEPROM_ChannelPlan_8812 = 0xB8;
    public const byte PG_TXPWR_1PATH_BYTE_NUM_2G = 18;
    public const byte PG_TXPWR_1PATH_BYTE_NUM_5G = 24;
    public const bool DBG_PG_TXPWR_READ = true;
    public const byte BAND_CAP_2G = 0;
    public const byte BAND_CAP_5G = 1;
    public const byte BW_CAP_20M = BIT2;
    public const byte BW_CAP_40M = BIT3;
    public const byte BW_CAP_80M = BIT4;
    public const byte PROTO_CAP_11B = BIT0;
    public const byte PROTO_CAP_11G = BIT1;
    public const byte PROTO_CAP_11N = BIT2;
    public const byte PROTO_CAP_11AC = BIT3;
    public const byte WL_FUNC_P2P = BIT0;
    public const byte WL_FUNC_MIRACAST = BIT1;
    public const byte WL_FUNC_TDLS = BIT2;
    public const ushort REG_MACID_SLEEP = 0x04D4;
    public const ushort REG_MACID_SLEEP_1 = 0x0488;
    public const ushort REG_MACID_SLEEP_2 = 0x04D0;
    public const ushort REG_MACID_SLEEP_3 = 0x0484;
    public const byte EFUSE_WIFI = 0;
    public const int EFUSE_MAP_LEN_JAGUAR = 512;
    public const uint RTW_PWR_STATE_CHK_INTERVAL = 2000;
    public const ushort REG_USB_HRPWM = 0xFE58;
    public const ushort REG_BAR_MODE_CTRL = 0x04CC;
    public const ushort REG_FAST_EDCA_CTRL = 0x0460;
    public const ushort REG_QUEUE_CTRL = 0x04C6;
    public const ushort REG_FWHW_TXQ_CTRL = 0x0420;
    public const ushort REG_CR_EXT = 0x1100;
    public const ushort REG_CR = 0x0100;
    public const ushort REG_EARLY_MODE_CONTROL_8812 = 0x02BC;
    public const ushort REG_TX_RPT_TIME = 0x04F0; /* 2 byte */
    public const ushort REG_SDIO_CTRL_8812 = 0x0070;
    public const ushort REG_ACLK_MON = 0x003E;
    public const byte DRVINFO_SZ = 4; /* unit is 8bytes */
    public const byte MACTXEN = BIT6;
    public const byte MACRXEN = BIT7;
    public const ushort REG_HWSEQ_CTRL = 0x0423;
    public const ushort REG_MCUFWDL = 0x0080;
    public const ushort rFPGA0_XCD_RFPara = 0x8b4;
    public const ushort REG_RF_CTRL = 0x001F;
    public const ushort REG_APS_FSMCO = 0x0004;
    public const ushort REG_RSV_CTRL = 0x001C;
    public const byte WOWLAN_PAGE_NUM_8812 = 0x00;
    public const byte BCNQ_PAGE_NUM_8812 = 0x07;
    public const byte FW_NDPA_PAGE_NUM = 0x02; // MAYBE 0x00 because CONFIG_BEAMFORMER_FW_NDPA
    public const byte TX_TOTAL_PAGE_NUMBER_8812 = (0xFF - BCNQ_PAGE_NUM_8812 - WOWLAN_PAGE_NUM_8812 - FW_NDPA_PAGE_NUM);
    public const byte TX_PAGE_BOUNDARY_8812 = (TX_TOTAL_PAGE_NUMBER_8812 + 1);
    public const uint LAST_ENTRY_OF_TX_PKT_BUFFER_8812 = 255;
    public const byte POLLING_LLT_THRESHOLD = 20;
    public const ushort REG_LLT_INIT = 0x01E0;
    public const UInt32 _LLT_WRITE_ACCESS = 0x1;
    public const UInt32 _LLT_NO_ACTIVE = 0x0;
    public const ushort REG_TXDMA_OFFSET_CHK = 0x020C;
    public const ushort REG_HMETFR = 0x01CC;
    public const ushort FW_START_ADDRESS = 0x1000;
    public const ushort REG_SYS_CFG = 0x00F0;
    public const byte FWDL_ChkSum_rpt = BIT2;
    public const byte MCUFWDL_RDY = BIT1;
    public const byte WINTINI_RDY = BIT6;
    public const byte COND_ELSE = 2;
    public const byte COND_ENDIF = 3;
    public const uint RTL_ID = BIT23;
    public const uint VENDOR_ID = BIT19;
    public const uint CHIP_VER_RTL_MASK = 0xF000; /* Bit 12 ~ 15 */
    public const int CHIP_VER_RTL_SHIFT = 12;
    public const uint WL_FUNC_EN = BIT2;
    public const uint BT_FUNC_EN = BIT18;
    public const uint WL_HWPDN_SL = BIT1;	/* WiFi HW PDn polarity control */
    public const ushort REG_MULTI_FUNC_CTRL = 0x0068; /* RTL8723 WIFI/BT/GPS Multi-Function control source. */
    public const uint NORMAL_PAGE_NUM_HPQ_8812 = 0x10;
    public const uint NORMAL_PAGE_NUM_LPQ_8812 = 0x10;
    public const uint NORMAL_PAGE_NUM_NPQ_8812 = 0x00;
    public const uint WMM_NORMAL_PAGE_NUM_HPQ_8812 = 0x30;
    public const uint WMM_NORMAL_PAGE_NUM_LPQ_8812 = 0x20;
    public const uint WMM_NORMAL_PAGE_NUM_NPQ_8812 = 0x20;
    public const ushort REG_RQPN_NPQ = 0x0214;
    public const ushort REG_RQPN = 0x0200;
    public const byte _HW_STATE_MONITOR_ = 0x04;
    public const byte _HW_STATE_NOLINK_ = 0x00;
    public const ushort MSR = (REG_CR + 2); /* Media Status register */
    public const ushort REG_RXFLTMAP2 = 0x06A4;
    public const ushort REG_RCR = 0x0608;
    public const ushort REG_RXFLTMAP1 = 0x06A2;
    public const ushort REG_DATA_SC_8812 = 0x0483;
    public const byte HAL_PRIME_CHNL_OFFSET_DONT_CARE = 0;
    public const byte HAL_PRIME_CHNL_OFFSET_LOWER = 1;
    public const byte HAL_PRIME_CHNL_OFFSET_UPPER = 2;
    public const byte RF_CHNLBW_Jaguar = 0x18; /* RF channel and BW switch */
}