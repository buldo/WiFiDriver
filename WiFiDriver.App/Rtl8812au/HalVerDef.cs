namespace WiFiDriver.App.Rtl8812au;

public static class HalVerDef
{
    public static bool IS_A_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.A_CUT_VERSION) ? true : false);

    public static bool IS_B_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.B_CUT_VERSION) ? true : false);

    public static bool IS_C_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.C_CUT_VERSION) ? true : false);

    public static bool IS_D_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.D_CUT_VERSION) ? true : false);

    public static bool IS_E_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.E_CUT_VERSION) ? true : false);

    public static bool IS_F_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.F_CUT_VERSION) ? true : false);

    public static bool IS_I_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.I_CUT_VERSION) ? true : false);

    public static bool IS_J_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.J_CUT_VERSION) ? true : false);

    public static bool IS_K_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == HAL_CUT_VERSION_E.K_CUT_VERSION) ? true : false);

    public static bool IS_1T1R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_1T1R) ? true : false);

    public static bool IS_1T2R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_1T2R) ? true : false);

    public static bool IS_2T2R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_2T2R) ? true : false);

    public static bool IS_3T3R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_3T3R) ? true : false);

    public static bool IS_3T4R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_3T4R) ? true : false);

    public static bool IS_4T4R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HAL_RF_TYPE_E.RF_TYPE_4T4R) ? true : false);

    private static HAL_RF_TYPE_E GET_CVID_RF_TYPE(HAL_VERSION version) => version.RFType;

    private static HAL_CUT_VERSION_E GET_CVID_CUT_VERSION(HAL_VERSION version) => version.CUTVersion;
}