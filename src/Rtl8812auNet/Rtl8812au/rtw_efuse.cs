namespace Rtl8812auNet.Rtl8812au;

public static class rtw_efuse
{
    public static void efuse_ShadowRead1Byte(
        AdapterState pAdapter,
        u16 Offset,
        out u8 Value)
    {
        var pHalData = pAdapter.HalData;
        Value = pHalData.efuse_eeprom_data[Offset];
    }
}