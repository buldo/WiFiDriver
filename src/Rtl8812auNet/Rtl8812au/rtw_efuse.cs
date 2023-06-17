namespace Rtl8812auNet.Rtl8812au;

public static class rtw_efuse
{
    public static void efuse_ShadowRead1Byte(
        AdapterState pAdapter,
        u16 Offset,
        out u8 Value)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(pAdapter);
        Value = pHalData.efuse_eeprom_data[Offset];
    }
}