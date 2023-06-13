namespace WiFiDriver.App.Rtl8812au;

public static class phydm
{
    public static void odm_cmn_info_init(dm_struct dm, odm_cmninfo cmn_info, u64 value)
    {
        /* This section is used for init value */
        switch (cmn_info)
        {
            /* @Fixed ODM value. */
            case odm_cmninfo.ODM_CMNINFO_ABILITY:
                dm.support_ability = (u64)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_RF_TYPE:
                dm.rf_type = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_PLATFORM:
                dm.support_platform = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_INTERFACE:
                dm.support_interface = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_IC_TYPE:
                dm.support_ic_type = (u32)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_CUT_VER:
                dm.cut_version = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_FAB_VER:
                dm.fab_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_FW_VER:
                dm.fw_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_FW_SUB_VER:
                dm.fw_sub_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_RFE_TYPE:
                dm.rfe_type = (u8)value;
                break;

            //case odm_cmninfo.ODM_CMNINFO_RF_ANTENNA_TYPE:
            //	dm.ant_div_type = (u8) value;
            //	break;

            //case odm_cmninfo.ODM_CMNINFO_WITH_EXT_ANTENNA_SWITCH:
            //	dm.with_extenal_ant_switch = (u8) value;
            //	break;


            //case odm_cmninfo.ODM_CMNINFO_BE_FIX_TX_ANT:
            //	dm.dm_fat_table.b_fix_tx_ant = (u8) value;
            //	break;


            case odm_cmninfo.ODM_CMNINFO_BOARD_TYPE:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.board_type = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_PACKAGE_TYPE:
                if (!dm.is_init_hw_info_by_rfe)
                {
                    dm.package_type = (u8) value;
                }
            	break;

            case odm_cmninfo.ODM_CMNINFO_EXT_LNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.ext_lna = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_5G_EXT_LNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.ext_lna_5g = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_EXT_PA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.ext_pa = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_5G_EXT_PA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.ext_pa_5g = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_GPA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_gpa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_APA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_apa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_GLNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_glna = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_ALNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_alna = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_EXT_TRSW:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.ext_trsw = (u8)value;
                break;
            //case odm_cmninfo.ODM_CMNINFO_EXT_LNA_GAIN:
            //	dm.ext_lna_gain = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_PATCH_ID:
            //	dm.iot_table.win_patch_id = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_BINHCT_TEST:
            //	dm.is_in_hct_test = (boolean) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_BWIFI_TEST:
            //	dm.wifi_test = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_SMART_CONCURRENT:
            //	dm.is_dual_mac_smart_concurrent = (boolean) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_IQKPAOFF:
            //	dm.rf_calibrate_info.is_iqk_pa_off = (boolean) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_REGRFKFREEENABLE:
            //	dm.rf_calibrate_info.reg_rf_kfree_enable = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_RFKFREEENABLE:
            //	dm.rf_calibrate_info.rf_kfree_enable = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_NORMAL_RX_PATH_CHANGE:
            //	dm.normal_rx_path = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_EFUSE0X3D8:
            //	dm.efuse0x3d8 = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_EFUSE0X3D7:
            //	dm.efuse0x3d7 = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_ADVANCE_OTA:
            //	dm.p_advance_ota = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_DFS_REGION_DOMAIN:
            //	dm.dfs_region_domain = (u8) value;
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_SOFT_AP_SPECIAL_SETTING:
            //	dm.soft_ap_special_setting = (u32) value;
            //	break;
            case odm_cmninfo.ODM_CMNINFO_X_CAP_SETTING:
                // TODO:
                //dm.dm_cfo_track.crystal_cap_default = (u8)value;
                break;
            //case odm_cmninfo.ODM_CMNINFO_DPK_EN:
            //	/*@dm.dpk_en = (u1Byte)value;*/
            //	halrf_cmn_info_set(dm, HALRF_CMNINFO_DPK_EN, (u64) value);
            //	break;
            //case odm_cmninfo.ODM_CMNINFO_HP_HWID:
            //	dm.hp_hw_id = (boolean) value;
            //	break;
            default:
                throw new NotImplementedException();
                break;
        }
    }
}