namespace Rtl8812auNet.Rtl8812au;

public static class halhwimg8812a_mac
{
    public static void odm_read_and_config_mp_8812a_mac_reg(AdapterState adapterState, dm_struct dm)
    {
        u32 i = 0;
        u8 c_cond;
        bool is_matched = true, is_skipped = false;
        var array_len = array_mp_8812a_mac_reg.Length;
        var array = array_mp_8812a_mac_reg;

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
                        if (check_positive(dm, pre_v1, pre_v2, v1, v2))
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
                    ushort addr = (u16)v1;
                    byte data = (u8)v2;
                    odm_write_1byte(adapterState, addr, data);
                }
            }

            i = i + 2;
        }
    }

    private static u32[] array_mp_8812a_mac_reg =
    {
        0x010, 0x0000000C,
        0x80000200, 0x00000000, 0x40000000, 0x00000000,
        0x011, 0x00000066,
        0xA0000000, 0x00000000,
        0x011, 0x0000005A,
        0xB0000000, 0x00000000,
        0x025, 0x0000000F,
        0x072, 0x00000000,
        0x420, 0x00000080,
        0x428, 0x0000000A,
        0x429, 0x00000010,
        0x430, 0x00000000,
        0x431, 0x00000000,
        0x432, 0x00000000,
        0x433, 0x00000001,
        0x434, 0x00000002,
        0x435, 0x00000003,
        0x436, 0x00000005,
        0x437, 0x00000007,
        0x438, 0x00000000,
        0x439, 0x00000000,
        0x43A, 0x00000000,
        0x43B, 0x00000001,
        0x43C, 0x00000002,
        0x43D, 0x00000003,
        0x43E, 0x00000005,
        0x43F, 0x00000007,
        0x440, 0x0000005D,
        0x441, 0x00000001,
        0x442, 0x00000000,
        0x444, 0x00000010,
        0x445, 0x00000000,
        0x446, 0x00000000,
        0x447, 0x00000000,
        0x448, 0x00000000,
        0x449, 0x000000F0,
        0x44A, 0x0000000F,
        0x44B, 0x0000003E,
        0x44C, 0x00000010,
        0x44D, 0x00000000,
        0x44E, 0x00000000,
        0x44F, 0x00000000,
        0x450, 0x00000000,
        0x451, 0x000000F0,
        0x452, 0x0000000F,
        0x453, 0x00000000,
        0x45B, 0x00000080,
        0x460, 0x00000066,
        0x461, 0x00000066,
        0x4C8, 0x000000FF,
        0x4C9, 0x00000008,
        0x4CC, 0x000000FF,
        0x4CD, 0x000000FF,
        0x4CE, 0x00000001,
        0x500, 0x00000026,
        0x501, 0x000000A2,
        0x502, 0x0000002F,
        0x503, 0x00000000,
        0x504, 0x00000028,
        0x505, 0x000000A3,
        0x506, 0x0000005E,
        0x507, 0x00000000,
        0x508, 0x0000002B,
        0x509, 0x000000A4,
        0x50A, 0x0000005E,
        0x50B, 0x00000000,
        0x50C, 0x0000004F,
        0x50D, 0x000000A4,
        0x50E, 0x00000000,
        0x50F, 0x00000000,
        0x512, 0x0000001C,
        0x514, 0x0000000A,
        0x516, 0x0000000A,
        0x525, 0x0000004F,
        0x550, 0x00000010,
        0x551, 0x00000010,
        0x559, 0x00000002,
        0x55C, 0x00000050,
        0x55D, 0x000000FF,
        0x604, 0x00000009,
        0x605, 0x00000030,
        0x607, 0x00000003,
        0x608, 0x0000000E,
        0x609, 0x0000002A,
        0x620, 0x000000FF,
        0x621, 0x000000FF,
        0x622, 0x000000FF,
        0x623, 0x000000FF,
        0x624, 0x000000FF,
        0x625, 0x000000FF,
        0x626, 0x000000FF,
        0x627, 0x000000FF,
        0x638, 0x00000050,
        0x63C, 0x0000000A,
        0x63D, 0x0000000A,
        0x63E, 0x0000000E,
        0x63F, 0x0000000E,
        0x640, 0x00000080,
        0x642, 0x00000040,
        0x643, 0x00000000,
        0x652, 0x000000C8,
        0x66E, 0x00000005,
        0x700, 0x00000021,
        0x701, 0x00000043,
        0x702, 0x00000065,
        0x703, 0x00000087,
        0x708, 0x00000021,
        0x709, 0x00000043,
        0x70A, 0x00000065,
        0x70B, 0x00000087,
        0x718, 0x00000040,

    };
}