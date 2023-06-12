namespace WiFiDriver.App.Rtl8812au;

public static class phydm_interface
{
    public static void odm_write_1byte(_adapter adapter, u16 reg_addr, u8 data)
    {
        rtw_write8(adapter, reg_addr, data);
    }
}