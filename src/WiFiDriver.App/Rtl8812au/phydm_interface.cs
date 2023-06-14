﻿namespace WiFiDriver.App.Rtl8812au;

public static class phydm_interface
{
    public static void odm_write_1byte(_adapter adapter, u16 reg_addr, u8 data)
    {
        rtw_write8(adapter, reg_addr, data);
    }

    public static void odm_set_bb_reg(_adapter dm, u32 reg_addr, u32 bit_mask, u32 data)
    {
        phy_set_bb_reg(dm, (u16)reg_addr, bit_mask, data);
    }
}