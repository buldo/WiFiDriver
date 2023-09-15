using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au;

public static class halhwimg8812a_bb
{
    public static bool check_positive(hal_com_data hal, dm_struct d, u32 condition1, u32 condition2, u32 condition4)
    {
        uint _board_type =
            ((d.board_type & BIT4) >>> 4) << 0 | /* _GLNA*/
            ((d.board_type & BIT3) >>> 3) << 1 | /* _GPA*/
            ((d.board_type & BIT7) >>> 7) << 2 | /* _ALNA*/
            ((d.board_type & BIT6) >>> 6) << 3 | /* _APA */
            ((d.board_type & BIT2) >>> 2) << 4 | /* _BT*/
            ((d.board_type & BIT1) >>> 1) << 5 | /* _NGFF*/
            ((d.board_type & BIT5) >>> 5) << 6;  /* _TRSWT*/


        u32 cond1 = condition1;
        u32 cond2 = condition2;
        u32 cond4 = condition4;

        uint cut_version_for_para = (d.cut_version == odm_cut_version.ODM_CUT_A) ? (uint)15 : (uint)d.cut_version;
        uint pkg_type_for_para = (u8)15 ;

        u32 driver1 = cut_version_for_para << 24 |
                      ((uint)RTL871X_HCI_TYPE.RTW_USB & 0xF0) << 16 |
                      pkg_type_for_para << 12 |
                      ((uint)RTL871X_HCI_TYPE.RTW_USB & 0x0F) << 8 |
                      _board_type;

        u32 driver2 =
            ((uint)hal.TypeGLNA & 0xFF) << 0 |
            ((uint)hal.TypeGPA & 0xFF) << 8 |
            ((uint)hal.TypeALNA & 0xFF) << 16 |
            ((uint)hal.TypeAPA & 0xFF) << 24;

        u32 driver4 =
            ((uint)hal.TypeGLNA & 0xFF00) >> 8 |
            ((uint)hal.TypeGPA & 0xFF00) |
            ((uint)hal.TypeALNA & 0xFF00) << 8 |
            ((uint)hal.TypeAPA & 0xFF00) << 16;

        /*============== value Defined Check ===============*/
        /*QFN type [15:12] and cut version [27:24] need to do value check*/

        if (((cond1 & 0x0000F000) != 0) && ((cond1 & 0x0000F000) != (driver1 & 0x0000F000)))
        {
            return false;
        }

        if (((cond1 & 0x0F000000) != 0) && ((cond1 & 0x0F000000) != (driver1 & 0x0F000000)))
        {
            return false;
        }

        /*=============== Bit Defined Check ================*/
        /* We don't care [31:28] */

        cond1 &= 0x00FF0FFF;
        driver1 &= 0x00FF0FFF;

        if ((cond1 & driver1) == cond1)
        {
            u32 bit_mask = 0;

            if ((cond1 & 0x0F) == 0) /* board_type is DONTCARE*/
            {
                return true;
            }

            if ((cond1 & BIT0) != 0) /*GLNA*/
            {
                bit_mask |= 0x000000FF;
            }
            if ((cond1 & BIT1) != 0) /*GPA*/
            {
                bit_mask |= 0x0000FF00;
            }
            if ((cond1 & BIT2) != 0) /*ALNA*/
            {
                bit_mask |= 0x00FF0000;
            }
            if ((cond1 & BIT3) != 0) /*APA*/
            {
                bit_mask |= 0xFF000000;
            }

            if (((cond2 & bit_mask) == (driver2 & bit_mask)) &&
                ((cond4 & bit_mask) == (driver4 & bit_mask))) /* board_type of each RF path is matched*/
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }



}