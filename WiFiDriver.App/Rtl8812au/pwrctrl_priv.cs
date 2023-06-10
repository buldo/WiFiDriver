public class pwrctrl_priv
{
 //   _pwrlock	lock;
	//_pwrlock check_32k_lock;
    volatile u8 rpwm; /* requested power state for fw */
    volatile u8 cpwm; /* fw current power state. updated when 1. read from HCPWM 2. driver lowers power level */
    volatile u8 tog; /* toggling */
    volatile u8 cpwm_tog; /* toggling */
    u8 rpwm_retry;

    u8 pwr_mode;
    u8 smart_ps;
    u8 bcn_ant_mode;
    u8 dtim;
    u8 lps_chk_by_tp;
    u16 lps_tx_tp_th;/*Mbps*/
    u16 lps_rx_tp_th;/*Mbps*/
    u16 lps_bi_tp_th;/*Mbps*//*TRX TP*/
    int lps_chk_cnt_th;
    int lps_chk_cnt;
    u32 lps_tx_pkts;
    u32 lps_rx_pkts;

    u8 wmm_smart_ps;


    u32 alives;
//    _workitem cpwm_event;
    //_workitem dma_event; /*for handle un-synchronized tx dma*/

    u8 bpower_saving; /* for LPS/IPS */

    u8 b_hw_radio_off;
    u8 reg_rfoff;
    u8 reg_pdnmode; /* powerdown mode */
    u32 rfoff_reason;

    /* RF OFF Level */
    u32 cur_ps_level;
    u32 reg_rfps_level;

    uint ips_enter_cnts;
    uint ips_leave_cnts;
    uint lps_enter_cnts;
    uint lps_leave_cnts;

    u8 ips_mode;
    u8 ips_org_mode;
    u8 ips_mode_req; /* used to accept the mode setting request, will update to ipsmode later */
    uint bips_processing;
    //systime ips_deny_time; /* will deny IPS when system time is smaller than this */
    u8 pre_ips_type;/* 0: default flow, 1: carddisbale flow */

    /* ps_deny: if 0, power save is free to go; otherwise deny all kinds of power save. */
    /* Use PS_DENY_REASON to decide reason. */
    /* Don't access this variable directly without control function, */
    /* and this variable should be protected by lock. */
    u32 ps_deny;

    u8 ps_processing; /* temporarily used to mark whether in rtw_ps_processor */

    u8 fw_psmode_iface_id;
    u8 bLeisurePs;
    u8 LpsIdleCount;
    u8 power_mgnt;
    u8 org_power_mgnt;
    u8 bFwCurrentInPSMode;
//    systime DelayLPSLastTimeStamp;
    s32 pnp_current_pwr_state;
    u8 pnp_bstop_trx;

    int ps_flag; /* used by autosuspend */
    u8 bInternalAutoSuspend;

    u8 bInSuspend;
    public bool bSupportRemoteWakeup;
    u8 wowlan_wake_reason;
    u8 wowlan_last_wake_reason;
    u8 wowlan_ap_mode;
    u8 wowlan_mode;
    u8 wowlan_p2p_mode;
    u8 wowlan_pno_enable;
    u8 wowlan_in_resume;


    u8 is_high_active;

    u8 hst2dev_high_active;

    bool default_patterns_en;

    u8 wowlan_ns_offload_en;

    u8 wowlan_txpause_status;
    u8 wowlan_pattern_idx;
    //u64 wowlan_fw_iv;
    //struct rtl_priv_pattern patterns[MAX_WKFM_CAM_NUM];

//    _mutex wowlan_pattern_cam_mutex;

    u8 wowlan_aoac_rpt_loc;
    //struct aoac_report wowlan_aoac_rpt;
	u8 wowlan_dis_lps;/*for debug purpose*/

    //_timer pwr_state_check_timer;
    int pwr_state_check_interval;
    u8 pwr_state_check_cnts;


    rt_rf_power_state rf_pwrstate;/* cur power state, only for IPS */
    /* rt_rf_power_state	current_rfpwrstate; */
    rt_rf_power_state change_rfpwrstate;

    public bool bHWPowerdown; /* power down mode selection. 0:radio off, 1:power down */
    u8 bHWPwrPindetect; /* come from registrypriv.hwpwrp_detect. enable power down function. 0:disable, 1:enable */
    u8 bkeepfwalive;
    u8 brfoffbyhw;
//    unsigned long PS_BBRegBackup[PSBBREG_TOTALCNT];


    u8 lps_level_bk;
    u8 lps_level; /*LPS_NORMAL,LPA_CG,LPS_PG*/
    u8 current_lps_hw_port_id;
}