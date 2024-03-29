﻿namespace Rtl8812auNet.Rtl8812au.Enumerations;

public static class CommonConsts
{
    public const ushort REG_9346CR = 0x000A;
    private const ushort REG_EFUSE_CTRL = 0x0030;
    public const ushort EFUSE_CTRL = REG_EFUSE_CTRL;
    public const ushort REG_EFUSE_BURN_GNT_8812 = 0x00CF;
    public const ushort REG_SYS_ISO_CTRL = 0x0000;
    public const ushort REG_SYS_FUNC_EN = 0x0002;
    public const ushort REG_EFUSE_TEST = 0x0034;
    public const ushort REG_SYS_CLKR = 0x0008;
    public const byte EEPROM_DEFAULT_BOARD_OPTION = 0x00;
    public const byte EEPROM_THERMAL_METER_8812 = 0xBA;
    public const byte EEPROM_USB_MODE_8812 = 0x08;
    
    public const ushort REG_USB_HRPWM = 0xFE58;
    public const ushort REG_BAR_MODE_CTRL = 0x04CC;
    public const ushort REG_FAST_EDCA_CTRL = 0x0460;
    public const ushort REG_QUEUE_CTRL = 0x04C6;
    public const ushort REG_FWHW_TXQ_CTRL = 0x0420;
    public const ushort REG_CR = 0x0100;
    public const ushort REG_EARLY_MODE_CONTROL_8812 = 0x02BC;
    public const ushort REG_TX_RPT_TIME = 0x04F0; /* 2 byte */
    public const ushort REG_SDIO_CTRL_8812 = 0x0070;
    public const ushort REG_ACLK_MON = 0x003E;
    public const byte DRVINFO_SZ = 4; /* unit is 8bytes */
    public const uint MACTXEN = BIT6;
    public const uint MACRXEN = BIT7;
    public const ushort REG_HWSEQ_CTRL = 0x0423;
    public const ushort REG_MCUFWDL = 0x0080;
    public const ushort rFPGA0_XCD_RFPara = 0x8b4;
    public const ushort REG_RF_CTRL = 0x001F;
    public const ushort REG_APS_FSMCO = 0x0004;
    public const ushort REG_RSV_CTRL = 0x001C;
    private const byte WOWLAN_PAGE_NUM_8812 = 0x00;
    private const byte BCNQ_PAGE_NUM_8812 = 0x07;
    private const byte FW_NDPA_PAGE_NUM = 0x02; // MAYBE 0x00 because CONFIG_BEAMFORMER_FW_NDPA
    public const byte TX_TOTAL_PAGE_NUMBER_8812 = 0xFF - BCNQ_PAGE_NUM_8812 - WOWLAN_PAGE_NUM_8812 - FW_NDPA_PAGE_NUM;
    public const byte TX_PAGE_BOUNDARY_8812 = TX_TOTAL_PAGE_NUMBER_8812 + 1;
    public const uint LAST_ENTRY_OF_TX_PKT_BUFFER_8812 = 255;
    public const byte POLLING_LLT_THRESHOLD = 20;
    public const ushort REG_LLT_INIT = 0x01E0;
    public const UInt32 _LLT_WRITE_ACCESS = 0x1;
    public const UInt32 _LLT_NO_ACTIVE = 0x0;
    public const ushort REG_TXDMA_OFFSET_CHK = 0x020C;
    public const ushort REG_HMETFR = 0x01CC;
    public const ushort FW_START_ADDRESS = 0x1000;
    public const ushort REG_SYS_CFG = 0x00F0;
    public const uint FWDL_ChkSum_rpt = BIT2;
    public const uint MCUFWDL_RDY = BIT1;
    public const uint WINTINI_RDY = BIT6;
    public const byte COND_ELSE = 2;
    public const byte COND_ENDIF = 3;
    public const uint CHIP_VER_RTL_MASK = 0xF000; /* Bit 12 ~ 15 */
    public const int CHIP_VER_RTL_SHIFT = 12;
    public const uint WL_FUNC_EN = BIT2;
    public const uint BT_FUNC_EN = BIT18;
    public const ushort REG_MULTI_FUNC_CTRL = 0x0068; /* RTL8723 WIFI/BT/GPS Multi-Function control source. */
    public const uint NORMAL_PAGE_NUM_HPQ_8812 = 0x10;
    public const uint NORMAL_PAGE_NUM_LPQ_8812 = 0x10;
    public const uint NORMAL_PAGE_NUM_NPQ_8812 = 0x00;
    public const uint WMM_NORMAL_PAGE_NUM_HPQ_8812 = 0x30;
    public const uint WMM_NORMAL_PAGE_NUM_LPQ_8812 = 0x20;
    public const uint WMM_NORMAL_PAGE_NUM_NPQ_8812 = 0x20;
    public const ushort REG_RQPN_NPQ = 0x0214;
    public const ushort REG_RQPN = 0x0200;
    public const byte _HW_STATE_NOLINK_ = 0x00;
    public const ushort MSR = REG_CR + 2; /* Media Status register */
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
    private const ushort MAX_RX_DMA_BUFFER_SIZE_8812 = 0x3E80;/* RX 16K */
    public const ushort RX_DMA_BOUNDARY_8812 = MAX_RX_DMA_BUFFER_SIZE_8812 - RX_DMA_RESERVED_SIZE_8812 - 1;
    private const ushort RX_DMA_RESERVED_SIZE_8812 = 0x0; /* 0B */
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
    public const ushort REG_RXDMA_STATUS = 0x0288;
    public const ushort REG_AMPDU_MAX_TIME_8812 = 0x0456;
    public const ushort REG_AMPDU_MAX_LENGTH_8812 = 0x0458;
    public const ushort REG_RXDMA_PRO_8812 = 0x0290;
    public const ushort REG_HT_SINGLE_AMPDU_8812 = 0x04C7;
    public const ushort REG_RX_PKT_LIMIT = 0x060C;
    public const ushort REG_PIFS = 0x0512;
    public const ushort REG_MAX_AGGR_NUM = 0x04CA;
    public const ushort rA_TxScale_Jaguar = 0xc1c;/* Pah_A TX scaling factor */
    public const ushort rB_TxScale_Jaguar = 0xe1c;/* Path_B TX scaling factor */
    public const ushort rA_RFE_Pinmux_Jaguar = 0xcb0; /* Path_A RFE cotrol pinmux */
    public const ushort rB_RFE_Pinmux_Jaguar = 0xeb0;/* Path_B RFE control pinmux */
    public const ushort rA_RFE_Inv_Jaguar = 0xcb4;/* Path_A RFE cotrol   */
    public const ushort rB_RFE_Inv_Jaguar = 0xeb4;/* Path_B RFE control */
    public const ushort r_ANTSEL_SW_Jaguar = 0x900;/* ANTSEL SW Control */
    public const ushort REG_TRXDMA_CTRL = 0x010C;
    public const ushort REG_HIQ_NO_LMT_EN = 0x05A7;

    public const uint bMaskByte0 = 0xff;
    public const uint bMaskByte1 = 0xff00;
    public const uint bMaskByte2 = 0xff0000;
    public const uint bMaskByte3 = 0xff000000;

    public const uint bCCK_System_Jaguar = 0x10;
    public const uint bOFDMEN_Jaguar = 0x20000000;
    public const uint bCCKEN_Jaguar = 0x10000000;
    public const uint bRxPath_Jaguar = 0xff;
    public const uint bCCK_RX_Jaguar = 0x0c000000;
    public const uint bMask_RFEInv_Jaguar = 0x3ff00000;

    public const byte PBP_512 = 0x3;
    public const byte TBTT_PROHIBIT_SETUP_TIME = 0x04; /* 128us, unit is 32us */
    public const byte TBTT_PROHIBIT_HOLD_TIME_STOP_BCN = 0x64; /* 3.2ms unit is 32us*/
    public const byte DRIVER_EARLY_INT_TIME_8812 = 0x05;
    public const byte BCN_DMA_ATIME_INT_TIME_8812 = 0x02;
    public const uint DIS_TSF_UDT = BIT4;
    public const uint EN_AMPDU_RTY_NEW = BIT7;
    public const byte EEPROM_TX_BBSWING_2G_8812 = 0xC6;
    public const byte EEPROM_TX_BBSWING_5G_8812 = 0xC7;
    public const uint FEN_USBA = BIT2;
    public const uint FEN_BB_GLB_RSTn = BIT1;
    public const uint FEN_BBRSTB = BIT0;

    public const UInt16 QUEUE_LOW = 1;
    public const UInt16 QUEUE_NORMAL = 2;
    public const UInt16 QUEUE_HIGH = 3;
    public const UInt16 QUEUE_EXTRA = 0;

    public const uint rA_LSSIWrite_Jaguar = 0xc90; /* RF write addr */
    public const uint rB_LSSIWrite_Jaguar = 0xe90; /* RF write addr */
    public const uint rHSSIRead_Jaguar = 0x8b0;  /* RF read addr */
    public const ushort rA_SIRead_Jaguar = 0xd08;/* RF readback with SI */
    public const ushort rB_SIRead_Jaguar = 0xd48;/* RF readback with SI */
    public const ushort rA_PIRead_Jaguar = 0xd04;/* RF readback with PI */
    public const ushort rB_PIRead_Jaguar = 0xd44; /* RF readback with PI */

    public const uint NotRATE_BITMAP_ALL = 0xFF_F0_00_00;
    public const uint RATE_RRSR_CCK_ONLY_1M = 0xFFFF1;
    public const uint RATE_RRSR_WITHOUT_CCK = 0xFFFF0;
    public const uint MASK_NETTYPE = 0x30000;

    public const byte RL_VAL_STA = 0x30;

    public const int BIT_SHIFT_SRL = 8;
    public const UInt16 BIT_MASK_SRL = 0x3f;
    public const int BIT_SHIFT_LRL = 0;
    public const UInt16 BIT_MASK_LRL = 0x3f;

    public const byte NT_LINK_AP = 0x2;

    public const ushort rTxAGC_A_CCK11_CCK1_JAguar = 0xc20;
    public const ushort rTxAGC_A_Ofdm18_Ofdm6_JAguar = 0xc24;
    public const ushort rTxAGC_A_Ofdm54_Ofdm24_JAguar = 0xc28;
    public const ushort rTxAGC_A_MCS3_MCS0_JAguar = 0xc2c;
    public const ushort rTxAGC_A_MCS7_MCS4_JAguar = 0xc30;
    public const ushort rTxAGC_A_MCS11_MCS8_JAguar = 0xc34;
    public const ushort rTxAGC_A_MCS15_MCS12_JAguar = 0xc38;
    public const ushort rTxAGC_A_Nss1Index3_Nss1Index0_JAguar = 0xc3c;
    public const ushort rTxAGC_A_Nss1Index7_Nss1Index4_JAguar = 0xc40;
    public const ushort rTxAGC_A_Nss2Index1_Nss1Index8_JAguar = 0xc44;
    public const ushort rTxAGC_A_Nss2Index5_Nss2Index2_JAguar = 0xc48;
    public const ushort rTxAGC_A_Nss2Index9_Nss2Index6_JAguar = 0xc4c;
    public const ushort rTxAGC_B_CCK11_CCK1_JAguar = 0xe20;
    public const ushort rTxAGC_B_Ofdm18_Ofdm6_JAguar = 0xe24;
    public const ushort rTxAGC_B_Ofdm54_Ofdm24_JAguar = 0xe28;
    public const ushort rTxAGC_B_MCS3_MCS0_JAguar = 0xe2c;
    public const ushort rTxAGC_B_MCS7_MCS4_JAguar = 0xe30;
    public const ushort rTxAGC_B_MCS11_MCS8_JAguar = 0xe34;
    public const ushort rTxAGC_B_MCS15_MCS12_JAguar = 0xe38;
    public const ushort rTxAGC_B_Nss1Index3_Nss1Index0_JAguar = 0xe3c;
    public const ushort rTxAGC_B_Nss1Index7_Nss1Index4_JAguar = 0xe40;
    public const ushort rTxAGC_B_Nss2Index1_Nss1Index8_JAguar = 0xe44;
    public const ushort rTxAGC_B_Nss2Index5_Nss2Index2_JAguar = 0xe48;
    public const ushort rTxAGC_B_Nss2Index9_Nss2Index6_JAguar = 0xe4c;

    public const uint bLSSIWrite_data_Jaguar = 0x000fffff;
    public const uint bHSSIRead_addr_Jaguar = 0xff;
    public const uint rRead_data_Jaguar = 0xfffff;
    public const uint MASKDWORD = 0xffffffff;

    public const ushort REG_RF_B_CTRL_8812 = 0x0076;
    public const ushort REG_RXDMA_AGG_PG_TH = 0x0280;
    //public const byte RXDMA_AGG_EN = BIT2;
    public const uint RXDMA_AGG_EN = BIT2;
    public const uint BLK_DESC_NUM_MASK = 0xF;
    public const int BLK_DESC_NUM_SHIFT = 4;

    //public const ushort REG_TDECTRL = 0x0208;
    public const ushort REG_DWBCN0_CTRL_8812 = REG_TDECTRL;

    public const ushort REG_PKT_VO_VI_LIFE_TIME = 0x04C0;
    public const ushort REG_PKT_BE_BK_LIFE_TIME = 0x04C2;

    public const uint RFREGOFFSETMASK = 0xfffff;

    public const int RXDESC_SIZE = 24;
}
