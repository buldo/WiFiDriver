namespace WiFiDriver.App.Rtl8812au;

public static class phydm_regconfig8812a
{
    public static void odm_config_mac_8812a(_adapter adapter, u16 addr, u8 data)
    {
        odm_write_1byte(adapter, addr, data);
        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "===> odm_config_mac_with_header_file: [MAC_REG] %08X %08X\n",
        //    addr, data);
    }

    public static void odm_config_bb_phy_8812a(_adapter dm, u32 addr, u32 bitmask, u32 data)
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

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //	  "===> odm_config_bb_with_header_file: [PHY_REG] %08X %08X\n",
        //	  addr, data);
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

    public static void odm_config_bb_agc_8812a(_adapter dm, u32 addr, u32 bitmask,u32 data)
    {

        odm_set_bb_reg(dm, addr, bitmask, data);
        /* Add 1us delay between BB/RF register setting. */
        ODM_delay_us(1);

        //PHYDM_DBG(dm, ODM_COMP_INIT,
        //    "===> odm_config_bb_with_header_file: [AGC_TAB] %08X %08X\n",
        //    addr, data);
    }
}