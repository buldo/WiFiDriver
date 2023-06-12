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
                                ((dm.board_type & BIT5) >> 5) << 6);  /* _TRSWT*/

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

        if ((cond1 & driver1) == cond1) {
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

            if (((cond2 & bit_mask) == (driver2 & bit_mask)) && ((cond4 & bit_mask) == (driver4 & bit_mask)))  /* board_type of each RF path is matched*/
                return true;
            else
                return false;
        } else
            return false;
    }

    static bool check_negative(dm_struct dm,u32 condition1,u32	condition2)
    {
        return true;
    }
}