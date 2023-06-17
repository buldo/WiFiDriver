namespace Rtl8812auNet.Rtl8812au;

public static class phydm_regconfig8812a
{
    static void odm_config_rf_reg_8812a(AdapterState dm, u32 addr, u32 data,RfPath RF_PATH, u16 reg_addr)
    {
        if (addr == 0xfe || addr == 0xffe)
        {
            ODM_sleep_ms(50);
        }
        else
        {
            odm_set_rf_reg(dm, RF_PATH, reg_addr, RFREGOFFSETMASK, data);
            /* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }
    }

    public static void odm_config_rf_radio_a_8812a(AdapterState dm, u32 addr, u32 data)
    {
        u32 content = 0x1000; /* RF_Content: radioa_txt */
        u32 maskfor_phy_set = (u32)(content & 0xE000);

        odm_config_rf_reg_8812a(dm, addr, data, RfPath.RF_PATH_A, (u16)(addr | maskfor_phy_set));
    }

    public static void odm_config_rf_radio_b_8812a(AdapterState dm, u32 addr, u32 data)
    {
        u32 content = 0x1001; /* RF_Content: radiob_txt */
        u32 maskfor_phy_set = (u32)(content & 0xE000);

        odm_config_rf_reg_8812a(dm, addr, data, RfPath.RF_PATH_B, (u16)(addr | maskfor_phy_set));
    }

    public static void odm_config_bb_phy_8812a(AdapterState dm, u32 addr, u32 bitmask, u32 data)
    {
        if (addr == 0xfe)
        {
            ODM_sleep_ms(50);
        }
        else if (addr == 0xfd)
        {
            ODM_delay_ms(5);
        }
        else if (addr == 0xfc)
        {
            ODM_delay_ms(1);
        }
        else if (addr == 0xfb)
        {
            ODM_delay_us(50);
        }
        else if (addr == 0xfa)
        {
            ODM_delay_us(5);
        }
        else if (addr == 0xf9)
        {
            ODM_delay_us(1);
        }
        else
        {
            odm_set_bb_reg(dm, addr, bitmask, data);
            /* Add 1us delay between BB/RF register setting. */
            ODM_delay_us(1);
        }
    }

    static void ODM_delay_ms(int ms)
    {
        Thread.Sleep(ms);
    }

    static void ODM_sleep_ms(int ms)
    {
        Thread.Sleep(ms);
    }

    static void ODM_delay_us(long us)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long v = (us * System.Diagnostics.Stopwatch.Frequency) / 1000000;
        while (sw.ElapsedTicks < v)
        {
        }
    }

    public static void odm_config_bb_agc_8812a(AdapterState dm, u32 addr, u32 bitmask,u32 data)
    {
        odm_set_bb_reg(dm, addr, bitmask, data);
        /* Add 1us delay between BB/RF register setting. */
        ODM_delay_us(1);
    }
}