using System.Diagnostics.CodeAnalysis;

namespace WiFiDriver.App.Rtl8812au;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum Registers : UInt16
{
    /* -----------------------------------------------------
    *
    *   0x0000h ~ 0x00FFh	System Configuration
    *
    * ----------------------------------------------------- */
    REG_SYS_ISO_CTRL = 0x0000,
    REG_SYS_FUNC_EN = 0x0002,
    REG_APS_FSMCO = 0x0004,
    REG_SYS_CLKR = 0x0008,
    REG_SYS_CLK_CTRL = REG_SYS_CLKR,
    REG_9346CR = 0x000A,
    REG_SYS_EEPROM_CTRL = 0x000A,
    REG_EE_VPD = 0x000C,
    REG_AFE_MISC = 0x0010,
    REG_SPS0_CTRL = 0x0011,
    REG_SPS0_CTRL_6 = 0x0016,
    REG_POWER_OFF_IN_PROCESS = 0x0017,
    REG_SPS_OCP_CFG = 0x0018,
    REG_RSV_CTRL = 0x001C,
    REG_RF_CTRL = 0x001F,
    REG_LDOA15_CTRL = 0x0020,
    REG_LDOV12D_CTRL = 0x0021,
    REG_LDOHCI12_CTRL = 0x0022,
    REG_LPLDO_CTRL = 0x0023,
    REG_AFE_XTAL_CTRL = 0x0024,
    REG_AFE_LDO_CTRL = 0x0027, /* 1.5v for 8188EE test chip, 1.4v for MP chip */
    REG_AFE_PLL_CTRL = 0x0028,
    REG_MAC_PHY_CTRL = 0x002c, /* for 92d, DMDP, SMSP, DMSP contrl */
    REG_APE_PLL_CTRL_EXT = 0x002c,
    REG_EFUSE_CTRL = 0x0030,
    REG_EFUSE_TEST = 0x0034,
    REG_PWR_DATA = 0x0038,
    REG_CAL_TIMER = 0x003C,
    REG_ACLK_MON = 0x003E,
    REG_GPIO_MUXCFG = 0x0040,
    REG_GPIO_IO_SEL = 0x0042,
    REG_MAC_PINMUX_CFG = 0x0043,
    REG_GPIO_PIN_CTRL = 0x0044,
    REG_GPIO_INTM = 0x0048,
    REG_LEDCFG0 = 0x004C,
    REG_LEDCFG1 = 0x004D,
    REG_LEDCFG2 = 0x004E,
    REG_LEDCFG3 = 0x004F,
    REG_FSIMR = 0x0050,
    REG_FSISR = 0x0054,
    REG_HSIMR = 0x0058,
    REG_HSISR = 0x005c,
    REG_GPIO_PIN_CTRL_2 = 0x0060, /* RTL8723 WIFI/BT/GPS Multi-Function GPIO Pin Control. */
    REG_GPIO_IO_SEL_2 = 0x0062, /* RTL8723 WIFI/BT/GPS Multi-Function GPIO Select. */
    REG_PAD_CTRL_1 = 0x0064,
    REG_MULTI_FUNC_CTRL = 0x0068, /* RTL8723 WIFI/BT/GPS Multi-Function control source. */
    REG_GSSR = 0x006c,
    REG_AFE_XTAL_CTRL_EXT = 0x0078, /* RTL8188E */
    REG_XCK_OUT_CTRL = 0x007c, /* RTL8188E */
    REG_MCUFWDL = 0x0080,
    REG_WOL_EVENT = 0x0081, /* RTL8188E */
    REG_MCUTSTCFG = 0x0084,
    REG_FDHM0 = 0x0088,
    REG_HOST_SUSP_CNT = 0x00BC, /* RTL8192C Host suspend counter on FPGA platform */
    REG_SYSTEM_ON_CTRL = 0x00CC, /* For 8723AE Reset after S3 */
    REG_EFUSE_ACCESS = 0x00CF, /* Efuse access protection for RTL8723 */
    REG_BIST_SCAN = 0x00D0,
    REG_BIST_RPT = 0x00D4,
    REG_BIST_ROM_RPT = 0x00D8,
    REG_USB_SIE_INTF = 0x00E0,
    REG_PCIE_MIO_INTF = 0x00E4,
    REG_PCIE_MIO_INTD = 0x00E8,
    REG_HPON_FSM = 0x00EC,
    REG_SYS_CFG = 0x00F0,
    REG_GPIO_OUTSTS = 0x00F4, /* For RTL8723 only. */
    REG_TYPE_ID = 0x00FC,

    /*
    * 2010/12/29 MH Add for 92D
    *   */
    REG_MAC_PHY_CTRL_NORMAL = 0x00f8,


    /* -----------------------------------------------------
    *
    *	=0x0100h ~ =0x01FFh	MACTOP General Configuration
    *
    * ----------------------------------------------------- */
    REG_CR = 0x0100,
    REG_PBP = 0x0104,
    REG_PKT_BUFF_ACCESS_CTRL = 0x0106,
    REG_TRXDMA_CTRL = 0x010C,
    REG_TRXFF_BNDY = 0x0114,
    REG_TRXFF_STATUS = 0x0118,
    REG_RXFF_PTR = 0x011C,
    REG_HIMR = 0x0120,
    REG_FE1IMR = 0x0120,
    REG_HISR = 0x0124,
    REG_HIMRE = 0x0128,
    REG_HISRE = 0x012C,
    REG_CPWM = 0x012F,
    REG_FWIMR = 0x0130,
    REG_FWISR = 0x0134,
    REG_FTIMR = 0x0138,
    REG_FTISR = 0x013C, /* RTL8192C */
    REG_PKTBUF_DBG_CTRL = 0x0140,
    REG_RXPKTBUF_CTRL =(REG_PKTBUF_DBG_CTRL+2),
    REG_PKTBUF_DBG_DATA_L = 0x0144,
    REG_PKTBUF_DBG_DATA_H = 0x0148,

    REG_TC0_CTRL = 0x0150,
    REG_TC1_CTRL = 0x0154,
    REG_TC2_CTRL = 0x0158,
    REG_TC3_CTRL = 0x015C,
    REG_TC4_CTRL = 0x0160,
    REG_TCUNIT_BASE = 0x0164,
    REG_MBIST_START = 0x0174,
    REG_MBIST_DONE = 0x0178,
    REG_MBIST_FAIL = 0x017C,
    REG_32K_CTRL = 0x0194, /* RTL8188E */
    REG_C2HEVT_MSG_NORMAL = 0x01A0,
    REG_C2HEVT_CLEAR = 0x01AF,
    REG_MCUTST_1 = 0x01c0,
    REG_MCUTST_WOWLAN = 0x01C7, /* Defined after 8188E series. */
    REG_FMETHR = 0x01C8,
    REG_HMETFR = 0x01CC,
    REG_HMEBOX_0 = 0x01D0,
    REG_HMEBOX_1 = 0x01D4,
    REG_HMEBOX_2 = 0x01D8,
    REG_HMEBOX_3 = 0x01DC,
    REG_LLT_INIT = 0x01E0,
    REG_HMEBOX_EXT_0 = 0x01F0,
    REG_HMEBOX_EXT_1 = 0x01F4,
    REG_HMEBOX_EXT_2 = 0x01F8,
    REG_HMEBOX_EXT_3 = 0x01FC,


    /* -----------------------------------------------------
    *
    *	=0x0200h ~ =0x027Fh	TXDMA Configuration
    *
    * ----------------------------------------------------- */
    REG_RQPN = 0x0200,
    REG_FIFOPAGE = 0x0204,
    REG_TDECTRL = 0x0208,
    REG_TXDMA_OFFSET_CHK = 0x020C,
    REG_TXDMA_STATUS = 0x0210,
    REG_RQPN_NPQ = 0x0214,
    REG_AUTO_LLT = 0x0224,


    /* -----------------------------------------------------
    *
    *	=0x0280h ~ =0x02FFh	RXDMA Configuration
    *
    * ----------------------------------------------------- */
    REG_RXDMA_AGG_PG_TH = 0x0280,
    REG_RXPKT_NUM = 0x0284,
    REG_RXDMA_STATUS = 0x0288,

    /* -----------------------------------------------------
    *
    *	=0x0300h ~ =0x03FFh	PCIe
    *
    * ----------------------------------------------------- */
//# ifndef CONFIG_TRX_BD_ARCH	/* prevent CONFIG_TRX_BD_ARCH to use old registers */

//    REG_PCIE_CTRL_REG = 0x0300
//    REG_INT_MIG = 0x0304 /* Interrupt Migration */
//    REG_BCNQ_DESA = 0x0308 /* TX Beacon Descriptor Address */
//    REG_HQ_DESA = 0x0310 /* TX High Queue Descriptor Address */
//    REG_MGQ_DESA = 0x0318 /* TX Manage Queue Descriptor Address */
//    REG_VOQ_DESA = 0x0320 /* TX VO Queue Descriptor Address */
//    REG_VIQ_DESA = 0x0328 /* TX VI Queue Descriptor Address */
//    REG_BEQ_DESA = 0x0330 /* TX BE Queue Descriptor Address */
//    REG_BKQ_DESA = 0x0338 /* TX BK Queue Descriptor Address */
//    REG_RX_DESA = 0x0340 /* RX Queue Descriptor Address */
//    /* sherry added for DBI Read/Write  20091126 */
//    REG_DBI_WDATA = 0x0348 /*  Backdoor REG for Access Configuration */
//    REG_DBI_RDATA = 0x034C /* Backdoor REG for Access Configuration */
//    REG_DBI_CTRL = 0x0350 /* Backdoor REG for Access Configuration */
//    REG_DBI_FLAG = 0x0352 /* Backdoor REG for Access Configuration */
//    REG_MDIO = 0x0354 /* MDIO for Access PCIE PHY */
//    REG_DBG_SEL = 0x0360 /* Debug Selection Register */
//    REG_WATCH_DOG = 0x0368
//    REG_RX_RXBD_NUM = 0x0382

//    /* RTL8723 series ------------------------------- */
//    REG_PCIE_HISR_EN = 0x0394 /* PCIE Local Interrupt Enable Register */
//    REG_PCIE_HISR = 0x03A0
//    REG_PCIE_HISRE = 0x03A4
//    REG_PCIE_HIMR = 0x03A8
//    REG_PCIE_HIMRE = 0x03AC

//#endif /* !CONFIG_TRX_BD_ARCH */

    REG_USB_HIMR = 0xFE38,
    REG_USB_HIMRE = 0xFE3C,
    REG_USB_HISR = 0xFE78,
    REG_USB_HISRE = 0xFE7C,


    /* -----------------------------------------------------
    *
    *	=0x0400h ~ =0x047Fh	Protocol Configuration
    *
    * ----------------------------------------------------- */

    /* 92C, 92D */
    REG_VOQ_INFO = 0x0400,
    REG_VIQ_INFO = 0x0404,
    REG_BEQ_INFO = 0x0408,
    REG_BKQ_INFO = 0x040C,

    /* 88E, 8723A, 8812A, 8821A, 92E, 8723B */
    REG_Q0_INFO = 0x400,
    REG_Q1_INFO = 0x404,
    REG_Q2_INFO = 0x408,
    REG_Q3_INFO = 0x40C,

    REG_MGQ_INFO = 0x0410,
    REG_HGQ_INFO = 0x0414,
    REG_BCNQ_INFO = 0x0418,
    REG_TXPKT_EMPTY = 0x041A,
    REG_CPU_MGQ_INFORMATION = 0x041C,
    REG_FWHW_TXQ_CTRL = 0x0420,
    REG_HWSEQ_CTRL = 0x0423,
    REG_BCNQ_BDNY = 0x0424,
    REG_MGQ_BDNY = 0x0425,
    REG_LIFETIME_CTRL = 0x0426,
    REG_MULTI_BCNQ_OFFSET = 0x0427,
    REG_SPEC_SIFS = 0x0428,
    REG_RL = 0x042A,
    REG_DARFRC = 0x0430,
    REG_RARFRC = 0x0438,
    REG_RRSR = 0x0440,
    REG_ARFR0 = 0x0444,
    REG_ARFR1 = 0x0448,
    REG_ARFR2 = 0x044C,
    REG_ARFR3 = 0x0450,
    REG_CCK_CHECK = 0x0454,
    REG_BCNQ1_BDNY = 0x0457,

    REG_AGGLEN_LMT = 0x0458,
    REG_AMPDU_MIN_SPACE = 0x045C,
    REG_WMAC_LBK_BF_HD = 0x045D,
    REG_FAST_EDCA_CTRL = 0x0460,
    REG_RD_RESP_PKT_TH = 0x0463,

    /* 8723A, 8812A, 8821A, 92E, 8723B */
    REG_Q4_INFO = 0x468,
    REG_Q5_INFO = 0x46C,
    REG_Q6_INFO = 0x470,
    REG_Q7_INFO = 0x474,

    REG_INIRTS_RATE_SEL = 0x0480,
    REG_INIDATA_RATE_SEL = 0x0484,

    /* 8723B, 92E, 8812A, 8821A*/
    REG_MACID_SLEEP_3 = 0x0484,
    REG_MACID_SLEEP_1 = 0x0488,

    REG_POWER_STAGE1 = 0x04B4,
    REG_POWER_STAGE2 = 0x04B8,
    REG_PKT_VO_VI_LIFE_TIME = 0x04C0,
    REG_PKT_BE_BK_LIFE_TIME = 0x04C2,
    REG_STBC_SETTING = 0x04C4,
    REG_QUEUE_CTRL = 0x04C6,
    REG_SINGLE_AMPDU_CTRL = 0x04c7,
    REG_PROT_MODE_CTRL = 0x04C8,
    REG_MAX_AGGR_NUM = 0x04CA,
    REG_RTS_MAX_AGGR_NUM = 0x04CB,
    REG_BAR_MODE_CTRL = 0x04CC,
    REG_RA_TRY_RATE_AGG_LMT = 0x04CF,

    /* 8723A */
    REG_MACID_DROP = 0x04D0,

    /* 88E */
    REG_EARLY_MODE_CONTROL = 0x04D0,

    /* 8723B, 92E, 8812A, 8821A */
    REG_MACID_SLEEP_2 = 0x04D0,

    /* 8723A, 8723B, 92E, 8812A, 8821A */
    REG_MACID_SLEEP = 0x04D4,

    REG_NQOS_SEQ = 0x04DC,
    REG_QOS_SEQ = 0x04DE,
    REG_NEED_CPU_HANDLE = 0x04E0,
    REG_PKT_LOSE_RPT = 0x04E1,
    REG_PTCL_ERR_STATUS = 0x04E2,
    REG_TX_RPT_CTRL = 0x04EC,
    REG_TX_RPT_TIME = 0x04F0, /* 2 byte */
    REG_DUMMY = 0x04FC,

    /* -----------------------------------------------------
    *
    *	=0x0500h ~ =0x05FFh	EDCA Configuration
    *
    * ----------------------------------------------------- */
    REG_EDCA_VO_PARAM = 0x0500,
    REG_EDCA_VI_PARAM = 0x0504,
    REG_EDCA_BE_PARAM = 0x0508,
    REG_EDCA_BK_PARAM = 0x050C,
    REG_BCNTCFG = 0x0510,
    REG_PIFS = 0x0512,
    REG_RDG_PIFS = 0x0513,
    REG_SIFS_CTX = 0x0514,
    REG_SIFS_TRX = 0x0516,
    REG_TSFTR_SYN_OFFSET = 0x0518,
    REG_AGGR_BREAK_TIME = 0x051A,
    REG_SLOT = 0x051B,
    REG_TX_PTCL_CTRL = 0x0520,
    REG_TXPAUSE = 0x0522,
    REG_DIS_TXREQ_CLR = 0x0523,
    REG_RD_CTRL = 0x0524,
    /*
    * Format for offset 540h-542h:
    *	[3:0]:   TBTT prohibit setup in unit of 32us. The time for HW getting beacon content before TBTT.
    *	[7:4]:   Reserved.
    *	[19:8]:  TBTT prohibit hold in unit of 32us. The time for HW holding to send the beacon packet.
    *	[23:20]: Reserved
    * Description:
    *	              |
    *      |<--Setup--|--Hold------------>|
    *   --------------|----------------------
    *                 |
    *                TBTT
    * Note: We cannot update beacon content to HW or send any AC packets during the time between Setup and Hold.
    * Described by Designer Tim and Bruce, 2011-01-14.
    *   */
    REG_TBTT_PROHIBIT = 0x0540,
    REG_RD_NAV_NXT = 0x0544,
    REG_NAV_PROT_LEN = 0x0546,
    REG_BCN_CTRL = 0x0550,
    REG_BCN_CTRL_1 = 0x0551,
    REG_MBID_NUM = 0x0552,
    REG_DUAL_TSF_RST = 0x0553,
    REG_BCN_INTERVAL = 0x0554, /* The same as REG_MBSSID_BCN_SPACE */
    REG_DRVERLYINT = 0x0558,
    REG_BCNDMATIM = 0x0559,
    REG_ATIMWND = 0x055A,
    REG_USTIME_TSF = 0x055C,
    REG_BCN_MAX_ERR = 0x055D,
    REG_RXTSF_OFFSET_CCK = 0x055E,
    REG_RXTSF_OFFSET_OFDM = 0x055F,
    REG_TSFTR = 0x0560,
    REG_TSFTR1 = 0x0568, /* HW Port 1 TSF Register */
    REG_ATIMWND_1 = 0x0570,
    REG_P2P_CTWIN = 0x0572, /* 1 Byte long (in unit of TU) */
    REG_PSTIMER = 0x0580,
    REG_TIMER0 = 0x0584,
    REG_TIMER1 = 0x0588,
    REG_ACMHWCTRL = 0x05C0,
    REG_NOA_DESC_SEL = 0x05CF,
    REG_NOA_DESC_DURATION = 0x05E0,
    REG_NOA_DESC_INTERVAL = 0x05E4,
    REG_NOA_DESC_START = 0x05E8,
    REG_NOA_DESC_COUNT = 0x05EC,

    REG_DMC = 0x05F0, /* Dual MAC Co-Existence Register */
    REG_SCH_TX_CMD = 0x05F8,

    REG_FW_RESET_TSF_CNT_1 = 0x05FC,
    REG_FW_RESET_TSF_CNT_0 = 0x05FD,
    REG_FW_BCN_DIS_CNT = 0x05FE,

    /* -----------------------------------------------------
    *
    *	=0x0600h ~ =0x07FFh	WMAC Configuration
    *
    * ----------------------------------------------------- */
    REG_APSD_CTRL = 0x0600,
    REG_BWOPMODE = 0x0603,
    REG_TCR = 0x0604,
    REG_RCR = 0x0608,
    REG_RX_PKT_LIMIT = 0x060C,
    REG_RX_DLK_TIME = 0x060D,
    REG_RX_DRVINFO_SZ = 0x060F,

    REG_MACID = 0x0610,
    REG_BSSID = 0x0618,
    REG_MAR = 0x0620,
    REG_MBIDCAMCFG_1 = 0x0628,
    REG_MBIDCAMCFG_2 = 0x062C,

    REG_PNO_STATUS = 0x0631,
    REG_USTIME_EDCA = 0x0638,
    REG_MAC_SPEC_SIFS = 0x063A,
    /* 20100719 Joseph: Hardware register definition change. (HW datasheet v54) */
    REG_RESP_SIFS_CCK = 0x063C, /* [15:8]SIFS_R2T_OFDM, [7:0]SIFS_R2T_CCK */
    REG_RESP_SIFS_OFDM = 0x063E, /* [15:8]SIFS_T2T_OFDM, [7:0]SIFS_T2T_CCK */

    REG_ACKTO = 0x0640,
    REG_CTS2TO = 0x0641,
    REG_EIFS = 0x0642,


    ///* RXERR_RPT */
    //RXERR_TYPE_OFDM_PPDU 0
    //RXERR_TYPE_OFDM_FALSE_ALARM 1
    //RXERR_TYPE_OFDM_MPDU_OK 2
    //RXERR_TYPE_OFDM_MPDU_FAIL 3
    //RXERR_TYPE_CCK_PPDU 4
    //RXERR_TYPE_CCK_FALSE_ALARM 5
    //RXERR_TYPE_CCK_MPDU_OK 6
    //RXERR_TYPE_CCK_MPDU_FAIL 7
    //RXERR_TYPE_HT_PPDU 8
    //RXERR_TYPE_HT_FALSE_ALARM 9
    //RXERR_TYPE_HT_MPDU_TOTAL 10
    //RXERR_TYPE_HT_MPDU_OK 11
    //RXERR_TYPE_HT_MPDU_FAIL 12
    //RXERR_TYPE_RX_FULL_DROP 15

    //RXERR_COUNTER_MASK = 0xFFFFF
    //RXERR_RPT_RST BIT(27)
    //_RXERR_RPT_SEL(type) ((type) << 28)

    /*
    * Note:
    *	The NAV upper value is very important to WiFi 11n 5.2.3 NAV test. The default value is
    *	always too small, but the WiFi TestPlan test by 25,000 microseconds of NAV through sending
    *	CTS in the air. We must update this value greater than 25,000 microseconds to pass the item.
    *	The offset of NAV_UPPER in 8192C Spec is incorrect, and the offset should be =0x0652. Commented
    *	by SD1 Scott.
    * By Bruce, 2011-07-18.
    *   */
    REG_NAV_UPPER = 0x0652, /* unit of 128 */

    /* WMA, BA, CCX */
    REG_NAV_CTRL = 0x0650,
    REG_BACAMCMD = 0x0654,
    REG_BACAMCONTENT = 0x0658,
    REG_LBDLY = 0x0660,
    REG_FWDLY = 0x0661,
    REG_RXERR_RPT = 0x0664,
    REG_WMAC_TRXPTCL_CTL = 0x0668,

    /* Security */
    REG_CAMCMD = 0x0670,
    REG_CAMWRITE = 0x0674,
    REG_CAMREAD = 0x0678,
    REG_CAMDBG = 0x067C,
    REG_SECCFG = 0x0680,

    /* Power */
    REG_WOW_CTRL = 0x0690,
    REG_PS_RX_INFO = 0x0692,
    REG_WMMPS_UAPSD_TID = 0x0693,
    REG_WKFMCAM_CMD = 0x0698,
    REG_WKFMCAM_NUM = REG_WKFMCAM_CMD,
    REG_WKFMCAM_RWD = 0x069C,
    REG_RXFLTMAP0 = 0x06A0,
    REG_RXFLTMAP1 = 0x06A2,
    REG_RXFLTMAP2 = 0x06A4,
    REG_BCN_PSR_RPT = 0x06A8,
    REG_BT_COEX_TABLE = 0x06C0,

    /* Hardware Port 1 */
    REG_MACID1 = 0x0700,
    REG_BSSID1 = 0x0708,
    /* Hardware Port 2 */
    REG_MACID2 = 0x1620,
    REG_BSSID2 = 0x1628,
    /* Hardware Port 3*/
    REG_MACID3 = 0x1630,
    REG_BSSID3 = 0x1638,
    /* Hardware Port 4 */
    REG_MACID4 = 0x1640,
    REG_BSSID4 = 0x1648,


    REG_CR_EXT = 0x1100,

    /* -----------------------------------------------------
    *
    *	=0xFE00h ~ =0xFE55h	USB Configuration
    *
    * ----------------------------------------------------- */
    REG_USB_INFO = 0xFE17,
    REG_USB_SPECIAL_OPTION = 0xFE55,
    REG_USB_DMA_AGG_TO = 0xFE5B,
    REG_USB_AGG_TO = 0xFE5C,
    REG_USB_AGG_TH = 0xFE5D,

    REG_USB_HRPWM = 0xFE58,
    REG_USB_HCPWM = 0xFE57,

    /* for 92DU high_Queue low_Queue Normal_Queue select */
    REG_USB_High_NORMAL_Queue_Select_MAC0 = 0xFE44,
    /*  REG_USB_LOW_Queue_Select_MAC0		=0xFE45 */
    REG_USB_High_NORMAL_Queue_Select_MAC1 = 0xFE47,
    /*  REG_USB_LOW_Queue_Select_MAC1		=0xFE48 */

    /* For test chip */
    REG_TEST_USB_TXQS = 0xFE48,
    REG_TEST_SIE_VID = 0xFE60, /* =0xFE60~=0xFE61 */
    REG_TEST_SIE_PID = 0xFE62, /* =0xFE62~=0xFE63 */
    REG_TEST_SIE_OPTIONAL = 0xFE64,
    REG_TEST_SIE_CHIRP_K = 0xFE65,
    REG_TEST_SIE_PHY = 0xFE66, /* =0xFE66~=0xFE6B */
    REG_TEST_SIE_MAC_ADDR = 0xFE70, /* =0xFE70~=0xFE75 */
    REG_TEST_SIE_STRING = 0xFE80, /* =0xFE80~=0xFEB9 */


    /* For normal chip */
    REG_NORMAL_SIE_VID = 0xFE60, /* =0xFE60~=0xFE61 */
    REG_NORMAL_SIE_PID = 0xFE62, /* =0xFE62~=0xFE63 */
    REG_NORMAL_SIE_OPTIONAL = 0xFE64,
    REG_NORMAL_SIE_EP = 0xFE65, /* =0xFE65~=0xFE67 */
    REG_NORMAL_SIE_PHY = 0xFE68, /* =0xFE68~=0xFE6B */
    REG_NORMAL_SIE_OPTIONAL2 = 0xFE6C,
    REG_NORMAL_SIE_GPS_EP = 0xFE6D, /* =0xFE6D, for RTL8723 only. */
    REG_NORMAL_SIE_MAC_ADDR = 0xFE70, /* =0xFE70~=0xFE75 */
    REG_NORMAL_SIE_STRING = 0xFE80, /* =0xFE80~=0xFEDF */

}