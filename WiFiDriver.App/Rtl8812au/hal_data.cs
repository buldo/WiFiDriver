namespace WiFiDriver.App.Rtl8812au;

public static class hal_data
{
    public static hal_spec_t GET_HAL_SPEC(_adapter __pAdapter) => GET_HAL_DATA(__pAdapter).hal_spec;
}