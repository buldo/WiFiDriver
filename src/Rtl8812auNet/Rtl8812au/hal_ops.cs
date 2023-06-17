using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WiFiDriver.App.Rtl8812au;

public class hal_ops
{
    //    /*** initialize section ***/
    public Action<_adapter> read_chip_version;
    //	void (* init_default_value) (_adapter* padapter);
    public Action<_adapter> intf_chip_configure;
    public Func<_adapter, u8> read_adapter_info;
    public Func<_adapter, bool> hal_power_on;
//	void (* hal_power_off) (_adapter* padapter);
    public Func<_adapter, bool> hal_init;
//	u32(*hal_deinit)(_adapter* padapter);
    public Action<_adapter> dm_init;
//	void (* dm_deinit) (_adapter* padapter);

//	/*** xmit section ***/
//	s32(*init_xmit_priv)(_adapter* padapter);
//	void (* free_xmit_priv) (_adapter* padapter);
//	s32(*hal_xmit)(_adapter* padapter, struct xmit_frame *pxmitframe);
//	/*
//	 * mgnt_xmit should be implemented to run in interrupt context
//	 */
//	s32(*mgnt_xmit)(_adapter* padapter, struct xmit_frame *pmgntframe);
//	s32(*hal_xmitframe_enqueue)(_adapter* padapter, struct xmit_frame *pxmitframe);
//#ifdef CONFIG_XMIT_THREAD_MODE
//	s32(*xmit_thread_handler)(_adapter* padapter);
//#endif
//	void (* run_thread) (_adapter* padapter);
//	void (* cancel_thread) (_adapter* padapter);

//	/*** recv section ***/
//	s32(*init_recv_priv)(_adapter* padapter);
//	void (* free_recv_priv) (_adapter* padapter);
//#ifdef CONFIG_RECV_THREAD_MODE
//	s32(*recv_hdl)(_adapter* adapter);
//#endif
//#if defined(CONFIG_USB_HCI) || defined(CONFIG_PCI_HCI)
//	u32(*inirp_init)(_adapter *padapter);
//	u32(*inirp_deinit)(_adapter *padapter);
//#endif
//	/*** interrupt hdl section ***/
//	void (* enable_interrupt) (_adapter* padapter);
//	void (* disable_interrupt) (_adapter* padapter);
//	u8(*check_ips_status)(_adapter* padapter);
//#if defined(CONFIG_PCI_HCI)
//	s32(*interrupt_handler)(_adapter *padapter);
//	void (*unmap_beacon_icf)(_adapter *padapter);
//#endif

//#if defined(CONFIG_USB_HCI) && defined(CONFIG_SUPPORT_USB_INT)
//	void	(*interrupt_handler)(_adapter *padapter, u16 pkt_len, u8 *pbuf);
//#endif

//#if defined(CONFIG_PCI_HCI)
//	void	(*irp_reset)(_adapter *padapter);
//#endif

//	/*** DM section ***/
//#ifdef CONFIG_RTW_SW_LED
//	void (* InitSwLeds) (_adapter* padapter);
//	void (* DeInitSwLeds) (_adapter* padapter);
//#endif
//	void (* set_chnl_bw_handler) (_adapter* padapter, u8 channel, enum channel_width Bandwidth, u8 Offset40, u8 Offset80);

    //	void (* set_tx_power_level_handler) (_adapter* padapter, u8 channel);
    //	void (* get_tx_power_level_handler) (_adapter* padapter, s32* powerlevel);

    //	void (* set_tx_power_index_handler) (_adapter* padapter, u32 powerindex, enum rf_path rfpath, u8 rate);
    //    u8(*get_tx_power_index_handler)(_adapter* padapter, enum rf_path rfpath, u8 rate, u8 bandwidth, u8 channel, struct txpwr_idx_comp *tic);

    //	void (* hal_dm_watchdog) (_adapter* padapter);

    //	u8(*set_hw_reg_handler)(_adapter* padapter, u8 variable, u8* val);

    //	void (* GetHwRegHandler) (_adapter* padapter, u8 variable, u8* val);



    //	u8(*get_hal_def_var_handler)(_adapter* padapter, HAL_DEF_VARIABLE eVariable, PVOID pValue);

    //	u8(*SetHalDefVarHandler)(_adapter* padapter, HAL_DEF_VARIABLE eVariable, PVOID pValue);

    //	void (* GetHalODMVarHandler) (_adapter* padapter, HAL_ODM_VARIABLE eVariable, PVOID pValue1, PVOID pValue2);
    //	void (* SetHalODMVarHandler) (_adapter* padapter, HAL_ODM_VARIABLE eVariable, PVOID pValue1, BOOLEAN bSet);

    //	void (* SetBeaconRelatedRegistersHandler) (_adapter* padapter);

    //	u8(*interface_ps_func)(_adapter* padapter, HAL_INTF_PS_FUNC efunc_id, u8* val);

    //	u32(*read_bbreg)(_adapter* padapter, u32 RegAddr, u32 BitMask);
    public Func<_adapter, u16, u32, u32> read_bbreg;
    //	void (* write_bbreg) (_adapter* padapter, u32 RegAddr, u32 BitMask, u32 Data);

//	u32(*read_rfreg)(_adapter* padapter, enum rf_path eRFPath, u32 RegAddr, u32 BitMask);
//	void (* write_rfreg) (_adapter* padapter, enum rf_path eRFPath, u32 RegAddr, u32 BitMask, u32 Data);
//# ifdef CONFIG_SYSON_INDIRECT_ACCESS
//    u32(*read_syson_reg)(_adapter* padapter, u32 RegAddr, u32 BitMask);
//	void (* write_syson_reg) (_adapter* padapter, u32 RegAddr, u32 BitMask, u32 Data);
//#endif
//	void (* read_wmmedca_reg) (_adapter* padapter, u16* vo_params, u16* vi_params, u16* be_params, u16* bk_params);

//#ifdef CONFIG_HOSTAPD_MLME
//	s32(*hostap_mgnt_xmit_entry)(_adapter* padapter, _pkt* pkt);
//#endif

//	void (* EfusePowerSwitch) (_adapter* padapter, u8 bWrite, u8 PwrState);
//	void (* BTEfusePowerSwitch) (_adapter* padapter, u8 bWrite, u8 PwrState);
//	void (* ReadEFuse) (_adapter* padapter, u8 efuseType, u16 _offset, u16 _size_byte, u8* pbuf, BOOLEAN bPseudoTest);
public Func<_adapter, byte, EFUSE_DEF_TYPE, bool, int> EFUSEGetEfuseDefinition;

//	u16(*EfuseGetCurrentSize)(_adapter* padapter, u8 efuseType, BOOLEAN bPseudoTest);
//	int (* Efuse_PgPacketRead) (_adapter* padapter, u8 offset, u8* data, BOOLEAN bPseudoTest);
//	int (* Efuse_PgPacketWrite) (_adapter* padapter, u8 offset, u8 word_en, u8* data, BOOLEAN bPseudoTest);
//	u8(*Efuse_WordEnableDataWrite)(_adapter* padapter, u16 efuse_addr, u8 word_en, u8* data, BOOLEAN bPseudoTest);
//	BOOLEAN(*Efuse_PgPacketWrite_BT)(_adapter* padapter, u8 offset, u8 word_en, u8* data, BOOLEAN bPseudoTest);
//#if defined(CONFIG_RTL8710B)
//	BOOLEAN(*efuse_indirect_read4)(_adapter *padapter, u16 regaddr, u8 *value);
//#endif

//#ifdef DBG_CONFIG_ERROR_DETECT
//	void (* sreset_init_value) (_adapter* padapter);
//	void (* sreset_reset_value) (_adapter* padapter);
//	void (* silentreset) (_adapter* padapter);
//	void (* sreset_xmit_status_check) (_adapter* padapter);
//	void (* sreset_linked_status_check) (_adapter* padapter);
//	u8(*sreset_get_wifi_status)(_adapter* padapter);
//	bool (* sreset_inprogress) (_adapter* padapter);
//#endif

//#ifdef CONFIG_IOL
//	int (* IOL_exec_cmds_sync) (_adapter* padapter, struct xmit_frame *xmit_frame, u32 max_wating_ms, u32 bndy_cnt);
//#endif

//	void (* hal_notch_filter) (_adapter* adapter, bool enable);
//#ifdef RTW_HALMAC
//	void (* hal_mac_c2h_handler) (_adapter* adapter, u8* pbuf, u16 length);
//#else
//	s32(*c2h_handler)(_adapter* adapter, u8 id, u8 seq, u8 plen, u8* payload);
//#endif
//	void (* reqtxrpt) (_adapter* padapter, u8 macid);
//	s32(*fill_h2c_cmd)(PADAPTER, u8 ElementID, u32 CmdLen, u8* pCmdBuffer);
//	void (* fill_fake_txdesc) (PADAPTER, u8* pDesc, u32 BufferLen,
//                 u8 IsPsPoll, u8 IsBTQosNull, u8 bDataFrame);
//	s32(*fw_dl)(_adapter* adapter, u8 wowlan);
//#ifdef RTW_HALMAC
//	s32(*fw_mem_dl)(_adapter* adapter, enum fw_mem mem);
//#endif

//#if defined(CONFIG_WOWLAN) || defined(CONFIG_AP_WOWLAN) || defined(CONFIG_PCI_HCI)
//	void (*clear_interrupt)(_adapter *padapter);
//#endif
//	u8(*hal_get_tx_buff_rsvd_page_num)(_adapter* adapter, bool wowlan);
//#ifdef CONFIG_GPIO_API
//	void (* update_hisr_hsisr_ind) (PADAPTER padapter, u32 flag);
//	int (* hal_gpio_func_check) (_adapter* padapter, u8 gpio_num);
//	void (* hal_gpio_multi_func_reset) (_adapter* padapter, u8 gpio_num);
//#endif
//#ifdef CONFIG_FW_CORRECT_BCN
//	void (* fw_correct_bcn) (PADAPTER padapter);
//#endif

//#ifdef RTW_HALMAC
//	u8(*init_mac_register)(PADAPTER);
//	u8(*init_phy)(PADAPTER);
//#endif /* RTW_HALMAC */

//#ifdef CONFIG_PCI_HCI
//	void (* hal_set_l1ssbackdoor_handler) (_adapter* padapter, u8 enable);
//#endif

//#ifdef CONFIG_RFKILL_POLL
//	bool (* hal_radio_onoff_check) (_adapter* adapter, u8* valid);
//#endif

};