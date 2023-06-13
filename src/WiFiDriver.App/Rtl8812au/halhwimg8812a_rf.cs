namespace WiFiDriver.App.Rtl8812au;

public static class halhwimg8812a_rf
{
    public static void odm_read_and_config_mp_8812a_radioa(dm_struct dm)
    {
        // TODO:
        //u32 i = 0;
        //u8 c_cond;
        //bool is_matched = true, is_skipped = false;
        //u32 array_len = sizeof(array_mp_8812a_radioa) / sizeof(u32);
        //u32[] array = array_mp_8812a_radioa;

        //u32 v1 = 0, v2 = 0, pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        //while ((i + 1) < array_len) {
        //    v1 = array[i];
        //    v2 = array[i + 1];

        //    if (v1 & (BIT(31) | BIT(30))) {/*positive & negative condition*/
        //        if (v1 & BIT(31)) {/* positive condition*/
        //            c_cond  = (u8) ((v1 & (BIT(29) | BIT(28))) >> 28);
        //            if (c_cond == COND_ENDIF) {/*end*/
        //                is_matched = true;
        //                is_skipped = false;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
        //            } else if (c_cond == COND_ELSE) { /*else*/
        //                is_matched = is_skipped? false : true;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
        //            } else
        //            {/*if , else if*/
        //                pre_v1 = v1;
        //                pre_v2 = v2;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
        //            }
        //        } else if (v1 & BIT(30))
        //        { /*negative condition*/
        //            if (is_skipped == false)
        //            {
        //                if (check_positive(dm, pre_v1, pre_v2, v1, v2))
        //                {
        //                    is_matched = true;
        //                    is_skipped = true;
        //                }
        //                else
        //                {
        //                    is_matched = false;
        //                    is_skipped = false;
        //                }
        //            }
        //            else
        //                is_matched = false;
        //        }
        //    } else
        //    {
        //        if (is_matched)
        //        {
        //            odm_config_rf_radio_a_8812a(dm, v1, v2);
        //        }
        //    }
        //    i = i + 2;
        //}
    }

    public static void odm_read_and_config_mp_8812a_radiob(dm_struct dm)
    {
        // TODO:
        //u32 i = 0;
        //u8 c_cond;
        //bool is_matched = true, is_skipped = false;
        //u32 array_len = sizeof(array_mp_8812a_radiob) / sizeof(u32);
        //u32* array = array_mp_8812a_radiob;

        //u32 v1 = 0, v2 = 0, pre_v1 = 0, pre_v2 = 0;

        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        //while ((i + 1) < array_len) {
        //    v1 = array[i];
        //    v2 = array[i + 1];

        //    if (v1 & (BIT(31) | BIT(30))) {/*positive & negative condition*/
        //        if (v1 & BIT(31)) {/* positive condition*/
        //            c_cond  = (u8) ((v1 & (BIT(29) | BIT(28))) >> 28);
        //            if (c_cond == COND_ENDIF) {/*end*/
        //                is_matched = true;
        //                is_skipped = false;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "ENDIF\n");
        //            } else if (c_cond == COND_ELSE) { /*else*/
        //                is_matched = is_skipped? false : true;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "ELSE\n");
        //            } else
        //            {/*if , else if*/
        //                pre_v1 = v1;
        //                pre_v2 = v2;
        //                PHYDM_DBG(dm, ODM_COMP_INIT, "IF or ELSE IF\n");
        //            }
        //        } else if (v1 & BIT(30))
        //        { /*negative condition*/
        //            if (is_skipped == false)
        //            {
        //                if (check_positive(dm, pre_v1, pre_v2, v1, v2))
        //                {
        //                    is_matched = true;
        //                    is_skipped = true;
        //                }
        //                else
        //                {
        //                    is_matched = false;
        //                    is_skipped = false;
        //                }
        //            }
        //            else
        //                is_matched = false;
        //        }
        //    } else
        //    {
        //        if (is_matched)
        //        {
        //            odm_config_rf_radio_b_8812a(dm, v1, v2);
        //        }
        //    }
        //    i = i + 2;
        //}
    }

    public static void odm_read_and_config_mp_8812a_txpwr_lmt(dm_struct dm)
    {
        // TODO:
        //u32 i = 0;
        //u32 array_len = sizeof(array_mp_8812a_txpwr_lmt) / sizeof(u8*);
        //u8** array = (u8**)array_mp_8812a_txpwr_lmt;
        //PHYDM_DBG(dm, ODM_COMP_INIT, "===> %s\n", __func__);

        //for (i = 0; i<array_len; i += 7) {
        //    u8* regulation = array[i];
        //    u8* band = array[i + 1];
        //    u8* bandwidth = array[i + 2];
        //    u8* rate = array[i + 3];
        //    u8* rf_path = array[i + 4];
        //    u8* chnl = array[i + 5];
        //    u8* val = array[i + 6];

        //    odm_config_bb_txpwr_lmt_8812a(dm, regulation, band, bandwidth, rate, rf_path, chnl, val);

        //}
    }
}