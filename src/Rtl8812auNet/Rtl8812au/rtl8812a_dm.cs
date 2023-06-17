namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_dm
{
    public static void rtl8812_init_dm_priv(PADAPTER Adapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        dm_struct podmpriv = pHalData.odmpriv;

        Init_ODM_ComInfo_8812(Adapter);
        odm_init_all_timers(podmpriv);
    }

    private static void Init_ODM_ComInfo_8812(PADAPTER Adapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(Adapter);

        dm_struct pDM_Odm = (pHalData.odmpriv);
        odm_cut_version cut_ver;
        odm_fab fab_ver;

        Init_ODM_ComInfo(Adapter);

        // TODO: WTF no mapping
        fab_ver = odm_fab.ODM_TSMC;
        if (IS_A_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_A;
        }
        else if (IS_B_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_B;
        }
        else if (IS_C_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_C;
        }
        else if (IS_D_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_D;
        }
        else if (IS_E_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_E;
        }
        else
        {
            cut_ver = odm_cut_version.ODM_CUT_A;
        }

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_FAB_VER, (ulong)fab_ver);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_CUT_VER, (ulong)cut_ver);
    }

    static void odm_init_all_timers(dm_struct dm)
    {
//#if (defined(CONFIG_PHYDM_ANTENNA_DIVERSITY))
//	odm_ant_div_timers(dm, INIT_ANTDIV_TIMMER);
//#endif
//#if (defined(PHYDM_TDMA_DIG_SUPPORT))
//#ifdef IS_USE_NEW_TDMA
//	phydm_tdma_dig_timers(dm, INIT_TDMA_DIG_TIMMER);
//#endif
//#endif
//#ifdef CONFIG_ADAPTIVE_SOML
//        phydm_adaptive_soml_timers(dm, INIT_SOML_TIMMER);
//#endif
//# ifdef PHYDM_LNA_SAT_CHK_SUPPORT
//# ifdef PHYDM_LNA_SAT_CHK_TYPE1
//        phydm_lna_sat_chk_timers(dm, INIT_LNA_SAT_CHK_TIMMER);
//#endif
//#endif

//#if (DM_ODM_SUPPORT_TYPE == ODM_WIN)
//        odm_initialize_timer(dm, &dm.sbdcnt_timer,
//            (void*) phydm_sbd_callback, NULL, "SbdTimer");
//# ifdef PHYDM_BEAMFORMING_SUPPORT
//        odm_initialize_timer(dm, &dm.beamforming_info.txbf_info.txbf_fw_ndpa_timer,
//            (void*) hal_com_txbf_fw_ndpa_timer_callback, NULL,
//            "txbf_fw_ndpa_timer");
//#endif
//#endif

//#if (DM_ODM_SUPPORT_TYPE & (ODM_WIN | ODM_CE))
//#ifdef PHYDM_BEAMFORMING_SUPPORT
//	odm_initialize_timer(dm, &dm.beamforming_info.beamforming_timer,
//			     (void *)beamforming_sw_timer_callback, NULL,
//			     "beamforming_timer");
//#endif
//#endif
    }

    static void Init_ODM_ComInfo(_adapter adapter)
    {

        dvobj_priv dvobj = adapter_to_dvobj(adapter);
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(adapter);
        dm_struct pDM_Odm = (pHalData.odmpriv);
        pwrctrl_priv pwrctl = adapter_to_pwrctl(adapter);
        int i;

        /*phydm_op_mode could be change for different scenarios: ex: SoftAP - PHYDM_BALANCE_MODE*/
        pHalData.phydm_op_mode = phydm_bb_op_mode.PHYDM_PERFORMANCE_MODE; /*Service one device*/
        rtw_odm_init_ic_type(adapter);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_INTERFACE, (uint)adapter.dvobj.interface_type);

        //odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_MP_TEST_CHIP, pHalData.version_id.ChipType == HAL_CHIP_TYPE_E.NORMAL_CHIP );

        //odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_PATCH_ID, pHalData.CustomerID);

        //odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BWIFI_TEST, adapter.registrypriv.wifi_spec);


        //odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ADVANCE_OTA, adapter.registrypriv.adv_ota);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RF_TYPE, (ulong)pHalData.rf_type);

        {
            /* 1 ======= BoardType: ODM_CMNINFO_BOARD_TYPE ======= */
            uint odm_board_type = ODM_BOARD_DEFAULT;

            if (pHalData.ExternalLNA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_LNA, 1);
            }

            if (pHalData.external_lna_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA_5G;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_5G_EXT_LNA, 1);
            }

            if (pHalData.ExternalPA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_PA;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_PA, 1);
            }

            if (pHalData.external_pa_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_PA_5G;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_5G_EXT_PA, 1);
            }

            if (pHalData.EEPROMBluetoothCoexist)
                odm_board_type |= ODM_BOARD_BT;

            odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BOARD_TYPE, odm_board_type);
            /* 1 ============== End of BoardType ============== */
        }

        //phydm_adaptivity_info_update(podmpriv, PHYDM_ADAPINFO_DOMAIN_CODE_2G, 0);
        //phydm_adaptivity_info_update(podmpriv, PHYDM_ADAPINFO_DOMAIN_CODE_5G, 0);

        //# ifdef CONFIG_DFS_MASTER
        //        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_DFS_REGION_DOMAIN, adapter.registrypriv.dfs_region_domain);
        //        odm_cmn_info_hook(pDM_Odm, ODM_CMNINFO_DFS_MASTER_ENABLE, &(adapter_to_rfctl(adapter).radar_detect_enabled));
        //#endif

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_GPA, pHalData.TypeGPA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_APA, pHalData.TypeAPA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_GLNA, pHalData.TypeGLNA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ALNA, pHalData.TypeALNA);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RFE_TYPE, pHalData.rfe_type);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_X_CAP_SETTING, pHalData.crystal_cap);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_TRSW, 0);

///*Add by YuChen for kfree init*/
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_REGRFKFREEENABLE, adapter.registrypriv.RegPwrTrimEnable);
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RFKFREEENABLE, pHalData.RfKFreeEnable);

//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RF_ANTENNA_TYPE, pHalData.TRxAntDivType);
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BE_FIX_TX_ANT, pHalData.b_fix_tx_ant);
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_WITH_EXT_ANTENNA_SWITCH, pHalData.with_extenal_ant_switch);

///* (8822B) efuse 0x3D7 & 0x3D8 for TX PA bias */
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EFUSE0X3D7, pHalData.efuse0x3d7);
//        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EFUSE0X3D8, pHalData.efuse0x3d8);

/*Add by YuChen for adaptivity init*/
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ADAPTIVITY, &(adapter.registrypriv.adaptivity_en));
//        phydm_adaptivity_info_init(pDM_Odm, PHYDM_ADAPINFO_CARRIER_SENSE_ENABLE,
//            (adapter.registrypriv.adaptivity_mode != 0) ? TRUE : FALSE);
//        phydm_adaptivity_info_init(pDM_Odm, PHYDM_ADAPINFO_TH_L2H_INI, adapter.registrypriv.adaptivity_th_l2h_ini);
//        phydm_adaptivity_info_init(pDM_Odm, PHYDM_ADAPINFO_TH_EDCCA_HL_DIFF,
//            adapter.registrypriv.adaptivity_th_edcca_hl_diff);

///*halrf info init*/
//        halrf_cmn_info_init(pDM_Odm, HALRF_CMNINFO_EEPROM_THERMAL_VALUE, pHalData.eeprom_thermal_meter);
//        halrf_cmn_info_init(pDM_Odm, HALRF_CMNINFO_PWT_TYPE, 0);

//        if (rtw_odm_adaptivity_needed(adapter) == _TRUE)
//            rtw_odm_adaptivity_config_msg(RTW_DBGDUMP, adapter);

//# ifdef CONFIG_IQK_PA_OFF
//        odm_cmn_info_init(pDM_Odm, ODM_CMNINFO_IQKPAOFF, 1);
//#endif
//        rtw_hal_update_iqk_fw_offload_cap(adapter);
//# ifdef CONFIG_FW_OFFLOAD_PARAM_INIT
//        rtw_hal_update_param_init_fw_offload_cap(adapter);
//#endif

///* Pointer reference */
///*Antenna diversity relative parameters*/
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ANT_DIV, &(pHalData.AntDivCfg));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_MP_MODE, &(adapter.registrypriv.mp_mode));

//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BB_OPERATION_MODE, &(pHalData.phydm_op_mode));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_TX_UNI, &(dvobj.traffic_stat.tx_bytes));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RX_UNI, &(dvobj.traffic_stat.rx_bytes));

//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BAND, &(pHalData.current_band_type));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_FORCED_RATE, &(pHalData.ForcedDataRate));

//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_SEC_CHNL_OFFSET, &(pHalData.nCur40MhzPrimeSC));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_SEC_MODE, &(adapter.securitypriv.dot11PrivacyAlgrthm));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BW, &(pHalData.current_channel_bw));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_CHNL, &(pHalData.current_channel));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_NET_CLOSED, &(adapter.net_closed));

//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_SCAN, &(pHalData.bScanInProcess));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_POWER_SAVING, &(pwrctl.bpower_saving));
///*Add by Yuchen for phydm beamforming*/
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_TX_TP, &(dvobj.traffic_stat.cur_tx_tp));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RX_TP, &(dvobj.traffic_stat.cur_rx_tp));
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ANT_TEST, &(pHalData.antenna_test));
//# ifdef CONFIG_RTL8723B
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_IS1ANTENNA, &pHalData.EEPROMBluetoothAntNum);
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RFDEFAULTPATH, &pHalData.ant_path);
//#endif /*CONFIG_RTL8723B*/

//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_HUBUSBMODE, &(dvobj.usb_speed));

//# ifdef CONFIG_DYNAMIC_SOML
//        odm_cmn_info_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ADAPTIVE_SOML, &(adapter.registrypriv.dyn_soml_en));
//#endif

///*halrf info hook*/
//# ifdef CONFIG_MP_INCLUDED
//        halrf_cmn_info_hook(pDM_Odm, HALRF_CMNINFO_CON_TX, &(adapter.mppriv.mpt_ctx.is_start_cont_tx));
//        halrf_cmn_info_hook(pDM_Odm, HALRF_CMNINFO_SINGLE_TONE, &(adapter.mppriv.mpt_ctx.is_single_tone));
//        halrf_cmn_info_hook(pDM_Odm, HALRF_CMNINFO_CARRIER_SUPPRESSION,
//            &(adapter.mppriv.mpt_ctx.is_carrier_suppression));
//        halrf_cmn_info_hook(pDM_Odm, HALRF_CMNINFO_MP_RATE_INDEX, &(adapter.mppriv.mpt_ctx.mpt_rate_index));
//#endif/*CONFIG_MP_INCLUDED*/
//        for (i = 0; i < ODM_ASSOCIATE_ENTRY_NUM; i++)
//            odm_cmn_info_ptr_array_hook(pDM_Odm, odm_cmninfo.ODM_CMNINFO_STA_STATUS, i, NULL);

//        phydm_init_debug_setting(pDM_Odm);
//        rtw_phydm_ops_func_init(pDM_Odm);
        /* TODO */
        /* odm_cmn_info_hook(pDM_Odm, ODM_CMNINFO_BT_OPERATION, _FALSE); */
        /* odm_cmn_info_hook(pDM_Odm, ODM_CMNINFO_BT_DISABLE_EDCA, _FALSE); */
    }
}