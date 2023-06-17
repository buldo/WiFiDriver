namespace Rtl8812auNet.Rtl8812au;

public static class hal_data
{
    public static hal_spec_t GET_HAL_SPEC(_adapter __pAdapter) => GET_HAL_DATA(__pAdapter).hal_spec;
    public static bool HAL_SPEC_CHK_RF_PATH_2G(hal_spec_t _spec, byte _path) => ((_spec).rfpath_num_2g > (_path));
    public static bool HAL_SPEC_CHK_RF_PATH_5G(hal_spec_t _spec, byte _path) => ((_spec).rfpath_num_5g > (_path));
}