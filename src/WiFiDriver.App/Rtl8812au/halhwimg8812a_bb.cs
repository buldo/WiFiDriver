using System.Dynamic;

namespace WiFiDriver.App.Rtl8812au;

public static class halhwimg8812a_bb
{
    public static bool check_positive(dm_struct dm, u32 condition1, u32 condition2, u32 condition3, u32 condition4)
    {

        u8 _board_type = (byte)(((dm.board_type & BIT4) >> 4) << 0 | /* _GLNA*/
                                ((dm.board_type & BIT3) >> 3) << 1 | /* _GPA*/
                                ((dm.board_type & BIT7) >> 7) << 2 | /* _ALNA*/
                                ((dm.board_type & BIT6) >> 6) << 3 | /* _APA */
                                ((dm.board_type & BIT2) >> 2) << 4 | /* _BT*/
                                ((dm.board_type & BIT1) >> 1) << 5 | /* _NGFF*/
                                ((dm.board_type & BIT5) >> 5) << 6); /* _TRSWT*/

        u32 cond1 = condition1, cond2 = condition2, cond3 = condition3, cond4 = condition4;

        u8 cut_version_for_para = (dm.cut_version == (byte)odm_cut_version.ODM_CUT_A) ? (u8)15 : dm.cut_version;
        u8 pkg_type_for_para = (dm.package_type == 0) ? (u8)15 : dm.package_type;

        u32 driver1 = (uint)(cut_version_for_para << 24 |
                             (dm.support_interface & 0xF0) << 16 |
                             dm.support_platform << 16 |
                             pkg_type_for_para << 12 |
                             (dm.support_interface & 0x0F) << 8 |
                             _board_type);

        u32 driver2 = (uint)((dm.type_glna & 0xFF) << 0 |
                             (dm.type_gpa & 0xFF) << 8 |
                             (dm.type_alna & 0xFF) << 16 |
                             (dm.type_apa & 0xFF) << 24);

        u32 driver3 = 0;

        u32 driver4 = (uint)((dm.type_glna & 0xFF00) >> 8 |
                             (dm.type_gpa & 0xFF00) |
                             (dm.type_alna & 0xFF00) << 8 |
                             (dm.type_apa & 0xFF00) << 16);

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //"===> %s (cond1, cond2, cond3, cond4) = (0x%X 0x%X 0x%X 0x%X)\n",
        //      __func__, cond1, cond2, cond3, cond4);
        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //"===> %s (driver1, driver2, driver3, driver4) = (0x%X 0x%X 0x%X 0x%X)\n",
        //      __func__, driver1, driver2, driver3, driver4);

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //"	(Platform, Interface) = (0x%X, 0x%X)\n",
        //      dm.support_platform, dm.support_interface);
        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //"	(Board, Package) = (0x%X, 0x%X)\n", dm.board_type,
        //      dm.package_type);


        /*============== value Defined Check ===============*/
        /*QFN type [15:12] and cut version [27:24] need to do value check*/

        if (((cond1 & 0x0000F000) != 0) && ((cond1 & 0x0000F000) != (driver1 & 0x0000F000)))
            return false;
        if (((cond1 & 0x0F000000) != 0) && ((cond1 & 0x0F000000) != (driver1 & 0x0F000000)))
            return false;

        /*=============== Bit Defined Check ================*/
        /* We don't care [31:28] */

        cond1 &= 0x00FF0FFF;
        driver1 &= 0x00FF0FFF;

        if ((cond1 & driver1) == cond1)
        {
            u32 bit_mask = 0;

            if ((cond1 & 0x0F) == 0) /* board_type is DONTCARE*/
                return true;

            if ((cond1 & BIT0) != 0) /*GLNA*/
                bit_mask |= 0x000000FF;
            if ((cond1 & BIT1) != 0) /*GPA*/
                bit_mask |= 0x0000FF00;
            if ((cond1 & BIT2) != 0) /*ALNA*/
                bit_mask |= 0x00FF0000;
            if ((cond1 & BIT3) != 0) /*APA*/
                bit_mask |= 0xFF000000;

            if (((cond2 & bit_mask) == (driver2 & bit_mask)) &&
                ((cond4 & bit_mask) == (driver4 & bit_mask))) /* board_type of each RF path is matched*/
                return true;
            else
                return false;
        }
        else
            return false;
    }

    static bool check_negative(dm_struct dm, u32 condition1, u32 condition2)
    {
        return true;
    }

    public static void odm_read_and_config_mp_8812a_phy_reg(_adapter dm)
    {
        u32 i = 0;
        u8 c_cond;
        bool is_matched = true, is_skipped = false;
        u32 array_len = (uint)array_mp_8812a_phy_reg.Length;
        u32[] array = array_mp_8812a_phy_reg;

        u32 v1 = 0, v2 = 0, pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            v1 = array[i];
            v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30))!=0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT3)!=0)
                {
                    /* positive condition*/
                    c_cond = (u8)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30)!=0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(dm.HalData.odmpriv, pre_v1, pre_v2, v1, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_bb_phy_8812a(dm, v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }

    public static void odm_read_and_config_mp_8812a_phy_reg_mp(_adapter dm)
    {
        u32 i = 0;
        u8 c_cond;
        bool is_matched = true, is_skipped = false;
        u32 array_len = (u32)array_mp_8812a_phy_reg_mp.Length;
        u32[] array = array_mp_8812a_phy_reg_mp;

        u32 v1 = 0, v2 = 0, pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            v1 = array[i];
            v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30))!=0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31)!=0)
                {
                    /* positive condition*/
                    c_cond = (u8)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30)!=0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(dm.HalData.odmpriv, pre_v1, pre_v2, v1, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_bb_phy_8812a(dm, v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }

    public static void odm_read_and_config_mp_8812a_agc_tab(_adapter dm)
    {
        u32 i = 0;
        u8 c_cond;
        bool is_matched = true, is_skipped = false;
        u32 array_len = (u32)array_mp_8812a_agc_tab.Length;
        u32[] array = array_mp_8812a_agc_tab;

        u32 v1 = 0, v2 = 0, pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        while ((i + 1) < array_len)
        {
            v1 = array[i];
            v2 = array[i + 1];

            if ((v1 & (BIT31 | BIT30))!=0)
            {
                /*positive & negative condition*/
                if ((v1 & BIT31)!=0)
                {
                    /* positive condition*/
                    c_cond = (u8)((v1 & (BIT29 | BIT28)) >> 28);
                    if (c_cond == COND_ENDIF)
                    {
                        /*end*/
                        is_matched = true;
                        is_skipped = false;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
                    }
                    else if (c_cond == COND_ELSE)
                    {
                        /*else*/
                        is_matched = is_skipped ? false : true;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
                    }
                    else
                    {
                        /*if , else if*/
                        pre_v1 = v1;
                        pre_v2 = v2;
                        //PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
                    }
                }
                else if ((v1 & BIT30)!=0)
                {
                    /*negative condition*/
                    if (is_skipped == false)
                    {
                        if (check_positive(dm.HalData.odmpriv, pre_v1, pre_v2, v1, v2))
                        {
                            is_matched = true;
                            is_skipped = true;
                        }
                        else
                        {
                            is_matched = false;
                            is_skipped = false;
                        }
                    }
                    else
                        is_matched = false;
                }
            }
            else
            {
                if (is_matched)
                {
                    odm_config_bb_agc_8812a(dm, v1, MASKDWORD, v2);
                }
            }

            i = i + 2;
        }
    }

    public static void odm_read_and_config_mp_8812a_txpowertrack_usb(dm_struct dm)
    {
        dm_rf_calibration_struct cali_info = (dm.rf_calibrate_info);

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> ODM_ReadAndConfig_MP_mp_8812a\n");

        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2ga_p, g_delta_swing_table_idx_mp_2ga_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2ga_n, g_delta_swing_table_idx_mp_2ga_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2gb_p, g_delta_swing_table_idx_mp_2gb_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2gb_n, g_delta_swing_table_idx_mp_2gb_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);

        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2g_cck_a_p, g_delta_swing_table_idx_mp_2g_cck_a_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2g_cck_a_n, g_delta_swing_table_idx_mp_2g_cck_a_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2g_cck_b_p, g_delta_swing_table_idx_mp_2g_cck_b_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_2g_cck_b_n, g_delta_swing_table_idx_mp_2g_cck_b_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE);

        //odm_move_memory(dm, cali_info.delta_swing_table_idx_5ga_p, g_delta_swing_table_idx_mp_5ga_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE * 3);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_5ga_n, g_delta_swing_table_idx_mp_5ga_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE * 3);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_5gb_p, g_delta_swing_table_idx_mp_5gb_p_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE * 3);
        //odm_move_memory(dm, cali_info.delta_swing_table_idx_5gb_n, g_delta_swing_table_idx_mp_5gb_n_txpowertrack_usb_8812a, DELTA_SWINGIDX_SIZE * 3);

    }
}