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
}