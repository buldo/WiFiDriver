namespace WiFiDriver.App.Rtl8812au;

public static class phydm_hwconfig
{
    public static bool odm_config_rf_with_header_file(dm_struct dm, odm_rf_config_type config_type, rf_path e_rf_path)
    {
        bool result = true;

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "support_platform: 0x%X, support_interface: 0x%X, board_type: 0x%X\n",
        //    dm.support_platform, dm.support_interface, dm.board_type);

/* @1 AP doesn't use PHYDM power tracking table in these ICs */

        if (config_type == odm_rf_config_type.CONFIG_RF_RADIO)
        {
            if (e_rf_path == rf_path.RF_PATH_A)
            {
                //READ_AND_CONFIG_MP(8812a, _radioa);
                odm_read_and_config_mp_8812a_radioa(dm);
            }
            else if (e_rf_path == rf_path.RF_PATH_B)
            {
                //READ_AND_CONFIG_MP(8812a, _radiob);
                odm_read_and_config_mp_8812a_radiob(dm);
            }
        }
        else if (config_type == odm_rf_config_type.CONFIG_RF_TXPWR_LMT)
        {

            //READ_AND_CONFIG_MP(8812a, _txpwr_lmt);
            odm_read_and_config_mp_8812a_txpwr_lmt(dm);
        }

        if (config_type == odm_rf_config_type.CONFIG_RF_RADIO)
        {
            // TODO:
            //if (dm.fw_offload_ability & PHYDM_PHY_PARAM_OFFLOAD)
            //{
            //    result = phydm_set_reg_by_fw(dm, PHYDM_HALMAC_CMD_END, 0, 0, 0, (enum rf_path)0,0);
            //    PHYDM_DBG(dm, ODM_COMP_INIT, "rf param offload end!result = %d", result);
            //}
        }

        return result;
    }

    public static bool odm_config_bb_with_header_file(_adapter dm, odm_bb_config_type config_type)
    {
        bool result = true;

        /* @1 AP doesn't use PHYDM initialization in these ICs */

        if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG)
        {
            //READ_AND_CONFIG_MP(8812a, _phy_reg);
            odm_read_and_config_mp_8812a_phy_reg(dm);
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB)
        {
            //READ_AND_CONFIG_MP(8812a, _agc_tab);
            odm_read_and_config_mp_8812a_agc_tab(dm);
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG_PG)
        {
            throw new NotImplementedException("odm_bb_config_type.CONFIG_BB_PHY_REG_PG");
            // READ_AND_CONFIG_MP(8812a, _phy_reg_pg);
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG_MP)
        {
            //READ_AND_CONFIG_MP(8812a, _phy_reg_mp);
            odm_read_and_config_mp_8812a_phy_reg_mp(dm);
        }
        else if (config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB_DIFF)
        {
            throw new NotImplementedException("odm_bb_config_type.CONFIG_BB_AGC_TAB_DIFF");
            //dm.fw_offload_ability &= ~PHYDM_PHY_PARAM_OFFLOAD;
            ///*@AGC_TAB DIFF dont support FW offload*/
            //if ((dm.channel >= 36) && (dm.channel <= 64))
            //{
            //    AGC_DIFF_CONFIG_MP(8812a, lb);
            //}
            //else if (*dm.channel >= 100)
            //{
            //    AGC_DIFF_CONFIG_MP(8812a, hb);
            //}
        }

        // TODO:
        //if (config_type == odm_bb_config_type.CONFIG_BB_PHY_REG || config_type == odm_bb_config_type.CONFIG_BB_AGC_TAB)
        //{
        //    if (dm.fw_offload_ability & PHYDM_PHY_PARAM_OFFLOAD)
        //    {
        //        result = phydm_set_reg_by_fw(dm, PHYDM_HALMAC_CMD_END, 0, 0, 0, (rf_path)0,0);
        //        PHYDM_DBG(dm, ODM_COMP_INIT, "phy param offload end!result = %d", result);
        //    }
        //}

        return result;
    }

    public static bool odm_config_rf_with_tx_pwr_track_header_file(dm_struct dm)
    {
        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "support_platform: 0x%X, support_interface: 0x%X, board_type: 0x%X\n",
        //    dm.support_platform, dm.support_interface, dm.board_type);

        /* @1 AP doesn't use PHYDM power tracking table in these ICs */
        //READ_AND_CONFIG_MP(8812a, _txpowertrack_usb);
        odm_read_and_config_mp_8812a_txpowertrack_usb(dm);
        return true;
    }
}