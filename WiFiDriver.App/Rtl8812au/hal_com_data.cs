namespace WiFiDriver.App.Rtl8812au;

public class hal_com_data
{
    public HAL_VERSION version_id;
//    RT_MULTI_FUNC MultiFunc; /* For multi-function consideration. */
//    RT_POLARITY_CTL PolarityCtl; /* For Wifi PDn Polarity control. */
//    RT_REGULATOR_MODE RegulatorMode; /* switching regulator or LDO */
public bool hw_init_completed;
//    /****** FW related ******/
//    u32 firmware_size;
public u16 firmware_version;
    u16 FirmwareVersionRev;
    public u16 firmware_sub_version;

    public u16 FirmwareSignature;
//    u8 RegFWOffload;
    public bool bFWReady;

//    u8 bBTFWReady;
    public bool fw_ractrl;
public    u8 LastHMEBoxNum;   /* H2C - for host message to fw */

//    /****** current WIFI_PHY values ******/
//    WIRELESS_MODE CurrentWirelessMode;
//    enum channel_width current_channel_bw;
//	BAND_TYPE current_band_type;    /* 0:2.4G, 1:5G */
    public BAND_TYPE BandSet;

    public u8 current_channel;

//    u8 cch_20;
//    u8 cch_40;
//    u8 cch_80;
//    u8 CurrentCenterFrequencyIndex1;
//    u8 nCur40MhzPrimeSC;    /* Control channel sub-carrier */
//    u8 nCur80MhzPrimeSC;   /* used for primary 40MHz of 80MHz mode */
//    BOOLEAN bSwChnlAndSetBWInProgress;
    public bool bDisableSWChannelPlan; /* flag of disable software change channel plan	 */
//    u16 BasicRateSet;
//    u32 ReceiveConfig;
//    u32 rcr_backup; /* used for switching back from monitor mode */
//    u8 rx_tsf_addr_filter_config; /* for 8822B/8821C USE */
//    BOOLEAN bSwChnl;
//    BOOLEAN bSetChnlBW;
//    BOOLEAN bSWToBW40M;
//    BOOLEAN bSWToBW80M;
//    BOOLEAN bChnlBWInitialized;
//    u32 BackUp_BB_REG_4_2nd_CCA[3];

//# ifdef CONFIG_RTW_ACS
//    struct auto_chan_sel acs;
//#endif
//#ifdef CONFIG_BCN_RECOVERY
//	u8 issue_bcn_fail;
//#endif /*CONFIG_BCN_RECOVERY*/

//    /****** rf_ctrl *****/
    public RF_CHIP_E rf_chip;

    public rf_type rf_type; /*enum rf_type*/
//    u8 PackageType;
public     u8 NumTotalRFPath;
//    u8 antenna_test;

//    /****** Debug ******/
//    u16 ForcedDataRate; /* Force Data Rate. 0: Auto, 0x02: 1M ~ 0x6C: 54M. */
//    u8 bDumpRxPkt;
//    u8 bDumpTxPkt;
//    u8 dis_turboedca; /* 1: disable turboedca,
//						  2: disable turboedca and setting EDCA parameter based on the input parameter*/
//    u32 edca_param_mode;

//    /****** EEPROM setting.******/
    public bool bautoload_fail_flag { get; set; }
    public RT_MULTI_FUNC MultiFunc { get; set; }
    public RT_POLARITY_CTL PolarityCtl { get; set; }
    public phydm_bb_op_mode phydm_op_mode { get; set; }
    public uint ReceiveConfig { get; set; }
    public channel_width current_channel_bw { get; set; }
    public byte cch_80 { get; set; }
    public byte cch_40 { get; set; }
    public byte cch_20 { get; set; }
    public bool bSwChnl { get; set; }
    public bool bChnlBWInitialized { get; set; }
    public byte nCur40MhzPrimeSC { get; set; }
    public byte nCur80MhzPrimeSC { get; set; }
    public bool bSetChnlBW { get; set; }
    public byte CurrentCenterFrequencyIndex1 { get; set; }
    public bool bNeedIQK { get; set; }

    //    u8 efuse_file_status;
//    u8 macaddr_file_status;
    public bool EepromOrEfuse;
    public u8[] efuse_eeprom_data = new u8[1024]; /*92C:256bytes, 88E:512bytes, we use union set (512bytes)*/

    public u8 InterfaceSel; /* board type kept in eFuse */
//    u16 CustomerID;

//    u16 EEPROMVID;
//    u16 EEPROMSVID;
//# ifdef CONFIG_USB_HCI
    public bool EEPROMUsbSwitch;
//    u16 EEPROMPID;
//    u16 EEPROMSDID;
//#endif
//# ifdef CONFIG_PCI_HCI
//    u16 EEPROMDID;
//    u16 EEPROMSMID;
//#endif

//    u8 EEPROMCustomerID;
//    u8 EEPROMSubCustomerID;
    public u8 EEPROMVersion;
    public u8 EEPROMRegulatory;
    public u8 eeprom_thermal_meter;
    public bool EEPROMBluetoothCoexist;
    public u8 EEPROMBluetoothType;

    public u8 EEPROMBluetoothAntNum;
    //    u8 EEPROMBluetoothAntIsolation;
    //    u8 EEPROMBluetoothRadioShared;
    //    u8 EEPROMMACAddr[ETH_ALEN];
    //    u8 tx_bbswing_24G;
    //    u8 tx_bbswing_5G;
    //    u8 efuse0x3d7;  /* efuse[0x3D7] */
    //    u8 efuse0x3d8;  /* efuse[0x3D8] */

    //# ifdef CONFIG_RF_POWER_TRIM
    //    u8 EEPROMRFGainOffset;
    //    u8 EEPROMRFGainVal;
    //    struct kfree_data_t kfree_data;
    //#endif /*CONFIG_RF_POWER_TRIM*/

    //#if defined(CONFIG_RTL8723B) || defined(CONFIG_RTL8703B) || \
    //	defined(CONFIG_RTL8723D) || \
    //	defined(CONFIG_RTL8192F)

    //	u8	adjuseVoltageVal;
    //	u8	need_restore;
    //#endif
    //	u8 EfuseUsedPercentage;
    //    u16 EfuseUsedBytes;
    //    /*u8		EfuseMap[2][HWSET_MAX_SIZE_JAGUAR];*/
    //    EFUSE_HAL EfuseHal;

    //    /*---------------------------------------------------------------------------------*/
    //    /* 2.4G TX power info for target TX power*/
    public u8[,] Index24G_CCK_Base = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] Index24G_BW40_Base = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] CCK_24G_Diff = new byte[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] OFDM_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    //	/* 5G TX power info for target TX power*/
    //#ifdef CONFIG_IEEE80211_BAND_5GHZ
    public u8[,] Index5G_BW40_Base = new byte[MAX_RF_PATH, CENTER_CH_5G_ALL_NUM];
    public u8[,] Index5G_BW80_Base = new byte[MAX_RF_PATH, CENTER_CH_5G_80M_NUM];
    public s8[,] OFDM_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    public s8[,] BW80_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
//#endif

//	u8 txpwr_by_rate_undefined_band_path[TX_PWR_BY_RATE_NUM_BAND]

//        [TX_PWR_BY_RATE_NUM_RF];

//	s8 TxPwrByRateOffset[TX_PWR_BY_RATE_NUM_BAND]

//        [TX_PWR_BY_RATE_NUM_RF]
//    [TX_PWR_BY_RATE_NUM_RATE];

//	/* Store the original power by rate value of the base rate for each rate section and rf path */
//	u8 TxPwrByRateBase2_4G[TX_PWR_BY_RATE_NUM_RF]

//        [MAX_BASE_NUM_IN_PHY_REG_PG_2_4G];
//	u8 TxPwrByRateBase5G[TX_PWR_BY_RATE_NUM_RF]

//        [MAX_BASE_NUM_IN_PHY_REG_PG_5G];

//	u8 txpwr_by_rate_loaded:1;
//    u8 txpwr_by_rate_from_file:1;
//    u8 txpwr_limit_loaded:1;
//    u8 txpwr_limit_from_file:1;
//    u8 rf_power_tracking_type;
public u8 CurrentTxPwrIdx;

//    /* Read/write are allow for following hardware information variables	 */
    public u8 crystal_cap;

    public u8 PAType_2G;
    public u8 PAType_5G;
    public u8 LNAType_2G;
    public u8 LNAType_5G;
    public bool ExternalPA_2G;
    public bool ExternalLNA_2G;
    public bool external_pa_5g;
    public bool external_lna_5g;
    public u16 TypeGLNA;
    public u16 TypeGPA;
    public u16 TypeALNA;

    public u16 TypeAPA;
    public u16 rfe_type;

//    u8 bLedOpenDrain; /* Support Open-drain arrangement for controlling the LED. Added by Roger, 2009.10.16. */
//    u32 ac_param_be; /* Original parameter for BE, use for EDCA turbo.	*/
//    u8 is_turbo_edca;
//    u8 prv_traffic_idx;
//    BB_REGISTER_DEFINITION_T PHYRegDef[MAX_RF_PATH];    /* Radio A/B/C/D */

//    u32 RfRegChnlVal[MAX_RF_PATH];

//    /* RDG enable */
//    BOOLEAN bRDGEnable;

//    u16 RegRRSR;
//    /****** antenna diversity ******/
//    u8 AntDivCfg;
//    u8 with_extenal_ant_switch;
//    u8 b_fix_tx_ant;
//    u8 AntDetection;
//    u8 TRxAntDivType;
//    u8 ant_path; /* for 8723B s0/s1 selection	 */
//    u32 antenna_tx_path;                    /* Antenna path Tx */
//    u32 AntennaRxPath;                  /* Antenna path Rx */
//    u8 sw_antdiv_bl_state;

//    /******** PHY DM & DM Section **********/
//    _lock IQKSpinLock;
//    u8 INIDATA_RATE[MACID_NUM_SW_LIMIT];

    public dm_struct odmpriv = new dm_struct();
//	u64 bk_rf_ability;
//    u8 bIQKInitialized;
//    u8 bNeedIQK;
//    u8 neediqk_24g;
//    u8 IQK_MP_Switch;
//    u8 bScanInProcess;
//    /******** PHY DM & DM Section **********/



//    /* 2010/08/09 MH Add CU power down mode. */
//    BOOLEAN pwrdown;

//    /* Add for dual MAC  0--Mac0 1--Mac1 */
    public u32 interfaceIndex;

//# ifdef CONFIG_P2P
//# ifdef CONFIG_P2P_PS_NOA_USE_MACID_SLEEP
//    u16 p2p_ps_offload;
//#else
//    u8 p2p_ps_offload;
//#endif
//#endif
//    /* Auto FSM to Turn On, include clock, isolation, power control for MAC only */
public    bool bMacPwrCtrlOn;
//    u8 hci_sus_state;

//    u8 RegIQKFWOffload;
//    struct submit_ctx   iqk_sctx;
//	u8 ch_switch_offload;
//    struct submit_ctx chsw_sctx;

//	RT_AMPDU_BRUST AMPDUBurstMode; /* 92C maybe not use, but for compile successfully */

    public TxSele OutEpQueueSel;
    public u8 OutEpNumber;

//# ifdef RTW_RX_AGGREGATION
//    RX_AGG_MODE rxagg_mode;

//    /* For RX Aggregation DMA Mode */
//    u8 rxagg_dma_size;
//    u8 rxagg_dma_timeout;
//#endif /* RTW_RX_AGGREGATION */

//#if defined(CONFIG_SDIO_HCI) || defined(CONFIG_GSPI_HCI)
//	/*  */
//	/* For SDIO Interface HAL related */
//	/*  */

//	/*  */
//	/* SDIO ISR Related */
//	/*
//	*	u32			IntrMask[1];
//	*	u32			IntrMaskToSet[1];
//	*	LOG_INTERRUPT		InterruptLog; */
//	u32			sdio_himr;
//	u32			sdio_hisr;
//# ifndef RTW_HALMAC
//	/*  */
//	/* SDIO Tx FIFO related. */
//	/*  */
//	/* HIQ, MID, LOW, PUB free pages; padapter.xmitpriv.free_txpg */
//# ifdef CONFIG_RTL8192F
//	u16			SdioTxFIFOFreePage[SDIO_TX_FREE_PG_QUEUE];
//#else
//    u8 SdioTxFIFOFreePage[SDIO_TX_FREE_PG_QUEUE];
//#endif/*CONFIG_RTL8192F*/
//    _lock SdioTxFIFOFreePageLock;
//    u8 SdioTxOQTMaxFreeSpace;
//    u8 SdioTxOQTFreeSpace;
//#else /* RTW_HALMAC */
//    u16 SdioTxOQTFreeSpace;
//#endif /* RTW_HALMAC */

//    /*  */
//    /* SDIO Rx FIFO related. */
//    /*  */
//    u8 SdioRxFIFOCnt;
//    u16 SdioRxFIFOSize;

//# ifndef RTW_HALMAC
//    u32 sdio_tx_max_len[SDIO_MAX_TX_QUEUE];/* H, N, L, used for sdio tx aggregation max length per queue */
//#else
//# ifdef CONFIG_RTL8821C
//    u16 tx_high_page;
//    u16 tx_low_page;
//    u16 tx_normal_page;
//    u16 tx_extra_page;
//    u16 tx_pub_page;
//    u8 max_oqt_size;
//# ifdef XMIT_BUF_SIZE
//    u32 max_xmit_size_vovi;
//    u32 max_xmit_size_bebk;
//#endif /*XMIT_BUF_SIZE*/
//    u16 max_xmit_page;
//    u16 max_xmit_page_vo;
//    u16 max_xmit_page_vi;
//    u16 max_xmit_page_be;
//    u16 max_xmit_page_bk;

//#endif /*#ifdef CONFIG_RTL8821C*/
//#endif /* !RTW_HALMAC */
//#endif /* CONFIG_SDIO_HCI */

//# ifdef CONFIG_USB_HCI

//    /* 2010/12/10 MH Add for USB aggreation mode dynamic shceme. */
//    BOOLEAN UsbRxHighSpeedMode;
//    BOOLEAN UsbTxVeryHighSpeedMode;
    public u32 UsbBulkOutSize;
//    BOOLEAN bSupportUSB3;
//    u8 usb_intf_start;

//    /* Interrupt relatd register information. */
//    u32 IntArray[3];/* HISR0,HISR1,HSISR */
//    u32 IntrMask[3];
//# ifdef CONFIG_USB_TX_AGGREGATION
//    u8 UsbTxAggMode;
//    u8 UsbTxAggDescNum;
//#endif /* CONFIG_USB_TX_AGGREGATION */

//# ifdef CONFIG_USB_RX_AGGREGATION
//    u16 HwRxPageSize;               /* Hardware setting */

//    /* For RX Aggregation USB Mode */
//    u8 rxagg_usb_size;
//    u8 rxagg_usb_timeout;
//#endif/* CONFIG_USB_RX_AGGREGATION */
//#endif /* CONFIG_USB_HCI */


//# ifdef CONFIG_PCI_HCI
//    /*  */
//    /* EEPROM setting. */
//    /*  */
//    u32 TransmitConfig;
//    u32 IntrMaskToSet[2];
//    u32 IntArray[4];
//    u32 IntrMask[4];
//    u32 SysIntArray[1];
//    u32 SysIntrMask[1];
//    u32 IntrMaskReg[2];
//    u32 IntrMaskDefault[4];

//    BOOLEAN bL1OffSupport;
//    BOOLEAN bSupportBackDoor;
//    u32 pci_backdoor_ctrl;

//    u8 bDefaultAntenna;

//    u8 bInterruptMigration;
//    u8 bDisableTxInt;

//    u16 RxTag;
//# ifdef CONFIG_PCI_DYNAMIC_ASPM
//    BOOLEAN bAspmL1LastIdle;
//#endif
//#endif /* CONFIG_PCI_HCI */


//# ifdef DBG_CONFIG_ERROR_DETECT
//    struct sreset_priv srestpriv;
//#endif /* #ifdef DBG_CONFIG_ERROR_DETECT */

//#ifdef CONFIG_BT_COEXIST
//	/* For bluetooth co-existance */
//	BT_COEXIST bt_coexist;
//#endif /* CONFIG_BT_COEXIST */

//#if defined(CONFIG_RTL8723B) || defined(CONFIG_RTL8703B) \
//	|| defined(CONFIG_RTL8188F) || defined(CONFIG_RTL8188GTV) || defined(CONFIG_RTL8723D)|| defined(CONFIG_RTL8192F)
//# ifndef CONFIG_PCI_HCI	/* mutual exclusive with PCI -- so they're SDIO and GSPI */
//	/* Interrupt relatd register information. */
//	u32			SysIntrStatus;
//	u32			SysIntrMask;
//#endif
//#endif /*endif CONFIG_RTL8723B	*/

//# ifdef CONFIG_LOAD_PHY_PARA_FROM_FILE
//    char para_file_buf[MAX_PARA_FILE_BUF_LEN];
//    char* mac_reg;
//    u32 mac_reg_len;
//    char* bb_phy_reg;
//    u32 bb_phy_reg_len;
//    char* bb_agc_tab;
//    u32 bb_agc_tab_len;
//    char* bb_phy_reg_pg;
//    u32 bb_phy_reg_pg_len;
//    char* bb_phy_reg_mp;
//    u32 bb_phy_reg_mp_len;
//    char* rf_radio_a;
//    u32 rf_radio_a_len;
//    char* rf_radio_b;
//    u32 rf_radio_b_len;
//    char* rf_tx_pwr_track;
//    u32 rf_tx_pwr_track_len;
//    char* rf_tx_pwr_lmt;
//    u32 rf_tx_pwr_lmt_len;
//#endif

//# ifdef CONFIG_BACKGROUND_NOISE_MONITOR
//    struct noise_monitor nm;
//#endif

    public hal_spec_t hal_spec = new();

    public uint rcr_backup;

    public BAND_TYPE current_band_type;
    //#ifdef CONFIG_PHY_CAPABILITY_QUERY
//	struct phy_spec_t phy_spec;
//#endif
//	u8 RfKFreeEnable;
//    u8 RfKFree_ch_group;
//    BOOLEAN bCCKinCH14;
//    BB_INIT_REGISTER RegForRecover[5];

//#if defined(CONFIG_PCI_HCI) && defined(RTL8814AE_SW_BCN)
//	BOOLEAN bCorrectBCN;
//#endif
//    u32 RxGainOffset[4]; /*{2G, 5G_Low, 5G_Middle, G_High}*/
//    u8 BackUp_IG_REG_4_Chnl_Section[4]; /*{A,B,C,D}*/

//    struct hal_iqk_reg_backup iqk_reg_backup[MAX_IQK_INFO_BACKUP_CHNL_NUM];

//#ifdef RTW_HALMAC
//	u16 drv_rsvd_page_number;
//#endif

//# ifdef CONFIG_BEAMFORMING
//    u8 backup_snd_ptcl_ctrl;
//# ifdef RTW_BEAMFORMING_VERSION_2
//    struct beamforming_info beamforming_info;
//#endif /* RTW_BEAMFORMING_VERSION_2 */
//#endif /* CONFIG_BEAMFORMING */

//	u8 not_xmitframe_fw_dl; /*not use xmitframe to download fw*/
//    u8 phydm_op_mode;

//    u8 in_cta_test;

//# ifdef CONFIG_RTW_LED
//    struct led_priv led;
//#endif
}