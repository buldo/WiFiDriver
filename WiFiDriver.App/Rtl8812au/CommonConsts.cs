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
    public const ushort REG_MGQ_BDNY = 0x0425;
    public const ushort REG_BCNQ_BDNY = 0x0424;
    public const ushort REG_WMAC_LBK_BF_HD = 0x045D;
    public const ushort REG_TRXFF_BNDY = 0x0114;
    public const ushort REG_TDECTRL = 0x0208;

    /* for 8812
 * TX 128K, RX 16K, Page size 512B for TX, 128B for RX */
    public const ushort MAX_RX_DMA_BUFFER_SIZE_8812 = 0x3E80;/* RX 16K */
    public const ushort RX_DMA_BOUNDARY_8812 = (MAX_RX_DMA_BUFFER_SIZE_8812 - RX_DMA_RESERVED_SIZE_8812 - 1);
    public const ushort RX_DMA_RESERVED_SIZE_8812 = 0x0; /* 0B */
    public const ushort REG_MAR = 0x0620;
    public const ushort REG_HIMR0_8812 = 0x00B0;
    public const ushort REG_HIMR1_8812 = 0x00B8;
    public const ushort REG_RX_DRVINFO_SZ = 0x060F;
    public const ushort REG_PBP = 0x0104;
    public const uint bMaskDWord = 0xffffffff;
    public const uint bMaskLWord = 0x0000ffff;
    public const ushort rA_TxPwrTraing_Jaguar = 0xc54;
    public const ushort rB_TxPwrTraing_Jaguar = 0xe54;
    public const ushort rFc_area_Jaguar = 0x860; /* fc_area */
    public const ushort rBWIndication_Jaguar = 0x834;
    public const ushort REG_CCK_CHECK_8812 = 0x0454;
    public const ushort rRFMOD_Jaguar = 0x8ac;/* RF mode */
    public const ushort rADC_Buf_Clk_Jaguar = 0x8c4;
    public const ushort rL1PeakTH_Jaguar = 0x848;
    public const ushort rCCAonSec_Jaguar = 0x838;
    public const ushort rCCK_System_Jaguar = 0xa00;  /* for cck sideband */
    public const ushort rRxPath_Jaguar = 0x808; /* Rx antenna */
    public const ushort rTxPath_Jaguar = 0x80c; /* Tx antenna */
    public const ushort rCCK_RX_Jaguar = 0xa04; /* for cck rx path selection */
    public const ushort REG_BCN_MAX_ERR = 0x055D;
    public const ushort REG_ARFR0_8812 = 0x0444;
    public const ushort REG_ARFR1_8812 = 0x044C;
    public const ushort REG_ARFR2_8812 = 0x048C;
    public const ushort REG_ARFR3_8812 = 0x0494;
    public const ushort REG_BCN_CTRL = 0x0550;
    public const ushort REG_TBTT_PROHIBIT = 0x0540;
    public const ushort REG_DRVERLYINT = 0x0558;
    public const ushort REG_BCNDMATIM = 0x0559;
    public const ushort REG_BCNTCFG = 0x0510;
    public const ushort REG_ACKTO = 0x0640;
    public const ushort REG_SPEC_SIFS = 0x0428;
    public const ushort REG_MAC_SPEC_SIFS = 0x063A;
    public const ushort REG_SIFS_CTX = 0x0514;
    public const ushort REG_SIFS_TRX = 0x0516;
    public const ushort REG_EDCA_BE_PARAM = 0x0508;
    public const ushort REG_EDCA_BK_PARAM = 0x050C;
    public const ushort REG_EDCA_VI_PARAM = 0x0504;
    public const ushort REG_EDCA_VO_PARAM = 0x0500;
    public const ushort REG_USTIME_TSF = 0x055C;
    public const ushort REG_USTIME_EDCA = 0x0638;
    public const ushort REG_RRSR = 0x0440;
    public const ushort REG_RETRY_LIMIT = 0x042A;
    public const ushort rOFDMCCKEN_Jaguar = 0x808; /* OFDM/CCK block enable */
    public const ushort rPwed_TH_Jaguar = 0x830;
    public const ushort REG_TXPKT_EMPTY = 0x041A;
    public const ushort rAGC_table_Jaguar = 0x82c;/* AGC tabel select */
    public const ushort REG_MAC_PHY_CTRL = 0x002c;/* for 92d, DMDP, SMSP, DMSP contrl */
    public const ushort REG_OPT_CTRL_8812 = 0x0074;
    public const ushort REG_WMAC_TRXPTCL_CTL = 0x0668;
    public const ushort REG_GPIO_MUXCFG = 0x0040;
    public const ushort REG_RXDMA_STATUS = 0x0288;
    public const ushort REG_AMPDU_MAX_TIME_8812 = 0x0456;
    public const ushort REG_AMPDU_MAX_LENGTH_8812 = 0x0458;
    public const ushort REG_RXDMA_PRO_8812 = 0x0290;
    public const ushort REG_HT_SINGLE_AMPDU_8812 = 0x04C7;
    public const ushort REG_RX_PKT_LIMIT = 0x060C;
    public const ushort REG_PIFS = 0x0512;
    public const ushort REG_MAX_AGGR_NUM = 0x04CA;
    public const ushort REG_AMPDU_BURST_MODE_8812 = 0x04BC;

    public const byte bMaskByte0 = 0xff;
    public const uint bCCK_System_Jaguar = 0x10;
}