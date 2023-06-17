namespace Rtl8812auNet.Rtl8812au;

public static class phydm_interface
{
    public static void odm_write_1byte(AdapterState adapterState, u16 reg_addr, u8 data)
    {
        rtw_write8(adapterState, reg_addr, data);
    }

    public static void odm_set_bb_reg(AdapterState dm, u32 reg_addr, u32 bit_mask, u32 data)
    {
        phy_set_bb_reg(dm, (u16)reg_addr, bit_mask, data);
    }

    public static void odm_set_rf_reg(AdapterState dm, rf_path e_rf_path, u16 reg_addr,u32 bit_mask, u32 data)
    {
        phy_set_rf_reg(dm, e_rf_path, reg_addr, bit_mask, data);
    }
}