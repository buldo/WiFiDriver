namespace Rtl8812auNet.Rtl8812au;

public class registry_priv
{
    public u8 chip_version;
    public u8 rfintfs;
    public u8 lbkmode;
    u8 hci;
    //NDIS_802_11_SSID ssid;
    u8 network_mode;    /* infra, ad-hoc, auto */
    public u8 channel;/* ad-hoc support requirement */
    public NETWORK_TYPE wireless_mode;/* A, B, G, auto */
    u8 scan_mode;/* active, passive */
    u8 radio_enable;
    u8 preamble;/* long, short, auto */
    u8 vrtl_carrier_sense;/* Enable, Disable, Auto */
    u8 vcs_type;/* RTS/CTS, CTS-to-self */
    u16 rts_thresh;
    u16 frag_thresh;
    u8 adhoc_tx_pwr;
    u8 soft_ap;
    u8 power_mgnt;
    u8 ips_mode;
    u8 lps_level;
    u8 lps_chk_by_tp;
    u8 smart_ps;
    u8 usb_rxagg_mode;
    u8 dynamic_agg_enable;
    u8 long_retry_lmt;
    u8 short_retry_lmt;
    u16 busy_thresh;
    u16 max_bss_cnt;
    u8 ack_policy;
    public u8 mp_mode;
    u8 mp_dm;
    u8 software_encrypt;
    u8 software_decrypt;
//# ifdef CONFIG_TX_EARLY_MODE
//    u8 early_mode;
//#endif
//# ifdef CONFIG_RTW_SW_LED
//    u8 led_ctrl;
//#endif
//    u8 acm_method;
//    /* WMM */
//    u8 wmm_enable;

//    WLAN_BSSID_EX dev_network;

//    u8 tx_bw_mode;

//    u8 bmc_tx_rate;

//# ifdef CONFIG_80211N_HT
//    u8 ht_enable;
//    /* 0: 20 MHz, 1: 40 MHz, 2: 80 MHz, 3: 160MHz */
//    /* 2.4G use bit 0 ~ 3, 5G use bit 4 ~ 7 */
//    /* 0x21 means enable 2.4G 40MHz & 5G 80MHz */
//    u8 bw_mode;
//    u8 ampdu_enable;/* for tx */
//    u8 rx_stbc;
//    u8 rx_ampdu_amsdu;/* Rx A-MPDU Supports A-MSDU is permitted */
//    u8 tx_ampdu_amsdu;/* Tx A-MPDU Supports A-MSDU is permitted */
//    u8 rx_ampdu_sz_limit_by_nss_bw[4][4]; /* 1~4SS, BW20~BW160 */
//	/* Short GI support Bit Map */
//	/* BIT0 - 20MHz, 1: support, 0: non-support */
//	/* BIT1 - 40MHz, 1: support, 0: non-support */
//	/* BIT2 - 80MHz, 1: support, 0: non-support */
//	/* BIT3 - 160MHz, 1: support, 0: non-support */
//	u8 short_gi;
//    /* BIT0: Enable VHT LDPC Rx, BIT1: Enable VHT LDPC Tx, BIT4: Enable HT LDPC Rx, BIT5: Enable HT LDPC Tx */
//    u8 ldpc_cap;
//    /* BIT0: Enable VHT STBC Rx, BIT1: Enable VHT STBC Tx, BIT4: Enable HT STBC Rx, BIT5: Enable HT STBC Tx */
//    u8 stbc_cap;
//    /*
//	 * BIT0: Enable VHT SU Beamformer
//	 * BIT1: Enable VHT SU Beamformee
//	 * BIT2: Enable VHT MU Beamformer, depend on VHT SU Beamformer
//	 * BIT3: Enable VHT MU Beamformee, depend on VHT SU Beamformee
//	 * BIT4: Enable HT Beamformer
//	 * BIT5: Enable HT Beamformee
//	 */
//    u8 beamform_cap;
//    u8 beamformer_rf_num;
//    u8 beamformee_rf_num;
//#endif /* CONFIG_80211N_HT */

//# ifdef CONFIG_80211AC_VHT
//    u8 vht_enable; /* 0:disable, 1:enable, 2:auto */
//    u8 ampdu_factor;
//    u8 vht_rx_mcs_map[2];
//#endif /* CONFIG_80211AC_VHT */

//    u8 lowrate_two_xmit;

    public rf_type rf_config;
//    u8 low_power;

    public bool wifi_spec;/* !turbo_mode */
public     u8 special_rf_path; /* 0: 2T2R ,1: only turn on path A 1T1R */
public char[] alpha2 = new char[2];
//    u8 channel_plan;
//    u8 excl_chs[MAX_CHANNEL_NUM];
//    u8 full_ch_in_p2p_handshake; /* 0: reply only softap channel, 1: reply full channel list*/

//# ifdef CONFIG_BT_COEXIST
//    u8 btcoex;
//    u8 bt_iso;
//    u8 bt_sco;
//    u8 bt_ampdu;
//    u8 ant_num;
//    u8 single_ant_path;
//#endif
//    BOOLEAN bAcceptAddbaReq;

//    u8 antdiv_cfg;
//    u8 antdiv_type;
//    u8 drv_ant_band_switch;

//    u8 switch_usb_mode;

//    u8 usbss_enable;/* 0:disable,1:enable */
//    u8 hwpdn_mode;/* 0:disable,1:enable,2:decide by EFUSE config */
//    u8 hwpwrp_detect;/* 0:disable,1:enable */

//    u8 hw_wps_pbc;/* 0:disable,1:enable */

//# ifdef CONFIG_ADAPTOR_INFO_CACHING_FILE
//    char adaptor_info_caching_file_path[PATH_LENGTH_MAX];
//#endif

//# ifdef CONFIG_LAYER2_ROAMING
//    u8 max_roaming_times; /* the max number driver will try to roaming */
//#endif

//# ifdef CONFIG_IOL
//    u8 fw_iol; /* enable iol without other concern */
//#endif

//# ifdef CONFIG_80211D
//    u8 enable80211d;
//#endif

//    u8 ifname[16];
//    u8 if2name[16];

//    u8 notch_filter;

//    /* for pll reference clock selction */
//    u8 pll_ref_clk_sel;

//    /* define for tx power adjust */
//#if CONFIG_TXPWR_LIMIT
//	u8	RegEnableTxPowerLimit;
//#endif
//    u8 RegEnableTxPowerByRate;

public bool target_tx_pwr_valid;
//    s8 target_tx_pwr_2g[RF_PATH_MAX][RATE_SECTION_NUM];
//#ifdef CONFIG_IEEE80211_BAND_5GHZ
//	s8 target_tx_pwr_5g[RF_PATH_MAX][RATE_SECTION_NUM - 1];
//#endif

//	u8 tsf_update_pause_factor;
//    u8 tsf_update_restore_factor;

public     s8 TxBBSwing_2G;
public     s8 TxBBSwing_5G;
    public u8 AmplifierType_2G;
    public u8 AmplifierType_5G;
//    u8 bEn_RFE;
public u8 RFE_Type;
//    u8 PowerTracking_Type;
//    u8 GLNA_Type;
//    u8 check_fw_ps;
//    u8 RegPwrTrimEnable;

//# ifdef CONFIG_LOAD_PHY_PARA_FROM_FILE
//    u8 load_phy_file;
//    u8 RegDecryptCustomFile;
//#endif
//# ifdef CONFIG_CONCURRENT_MODE
//    u8 virtual_iface_num;
//#endif
//    u8 qos_opt_enable;

//    u8 hiq_filter;
//    u8 adaptivity_en;
//    u8 adaptivity_mode;
//    s8 adaptivity_th_l2h_ini;
//    s8 adaptivity_th_edcca_hl_diff;

//    u8 boffefusemask;
//    BOOLEAN bFileMaskEfuse;
//# ifdef CONFIG_RTW_ACS
//    u8 acs_auto_scan;
//    u8 acs_mode;
//#endif

//# ifdef CONFIG_BACKGROUND_NOISE_MONITOR
//    u8 nm_mode;
//#endif
//    u32 reg_rxgain_offset_2g;
//    u32 reg_rxgain_offset_5gl;
//    u32 reg_rxgain_offset_5gm;
//    u32 reg_rxgain_offset_5gh;

//# ifdef CONFIG_DFS_MASTER
//    u8 dfs_region_domain;
//#endif

//# ifdef CONFIG_MCC_MODE
//    u8 en_mcc;
//    u32 rtw_mcc_single_tx_cri;
//    u32 rtw_mcc_ap_bw20_target_tx_tp;
//    u32 rtw_mcc_ap_bw40_target_tx_tp;
//    u32 rtw_mcc_ap_bw80_target_tx_tp;
//    u32 rtw_mcc_sta_bw20_target_tx_tp;
//    u32 rtw_mcc_sta_bw40_target_tx_tp;
//    u32 rtw_mcc_sta_bw80_target_tx_tp;
//    s8 rtw_mcc_policy_table_idx;
//    u8 rtw_mcc_duration;
//    u8 rtw_mcc_enable_runtime_duration;
//#endif /* CONFIG_MCC_MODE */

//# ifdef CONFIG_RTW_NAPI
//    u8 en_napi;
//# ifdef CONFIG_RTW_NAPI_DYNAMIC
//    u32 napi_threshold; /* unit: Mbps */
//#endif /* CONFIG_RTW_NAPI_DYNAMIC */
//# ifdef CONFIG_RTW_GRO
//    u8 en_gro;
//#endif /* CONFIG_RTW_GRO */
//#endif /* CONFIG_RTW_NAPI */

//# ifdef CONFIG_WOWLAN
//    u8 wakeup_event;
//    u8 suspend_type;
//#endif

//# ifdef CONFIG_SUPPORT_TRX_SHARED
//    u8 trx_share_mode;
//#endif
//    u8 check_hw_status;
//    u8 wowlan_sta_mix_mode;
//    u32 pci_aspm_config;

//    u8 iqk_fw_offload;
//    u8 ch_switch_offload;

//# ifdef CONFIG_TDLS
//    u8 en_tdls;
//#endif

//# ifdef CONFIG_ADVANCE_OTA
//    u8 adv_ota;
//#endif

//# ifdef CONFIG_FW_OFFLOAD_PARAM_INIT
//    u8 fw_param_init;
//#endif
//# ifdef CONFIG_DYNAMIC_SOML
//    u8 dyn_soml_en;
//    u8 dyn_soml_train_num;
//    u8 dyn_soml_interval;
//    u8 dyn_soml_period;
//    u8 dyn_soml_delay;
//#endif
//# ifdef CONFIG_FW_HANDLE_TXBCN
//    u8 fw_tbtt_rpt;
//#endif

//# ifdef DBG_LA_MODE
//    u8 la_mode_en;
//#endif
//# ifdef CONFIG_TDMADIG
//    u8 tdmadig_en;
//    u8 tdmadig_mode;
//    u8 tdmadig_dynamic;
//#endif/*CONFIG_TDMADIG*/

//    u8 monitor_overwrite_seqnum;
//    u8 monitor_retransmit;
//    u8 monitor_disable_1m;
};