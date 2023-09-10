namespace Rtl8812auNet.Rtl8812au;

public static class HalVerDef
{
    public static bool IS_VENDOR_8812A_C_CUT(AdapterState adapterState) =>
        GET_CVID_CUT_VERSION(adapterState.HalData.version_id) == CutVersion.C_CUT_VERSION;

    public static bool IS_A_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == CutVersion.A_CUT_VERSION) ? true : false);

    public static bool IS_B_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == CutVersion.B_CUT_VERSION) ? true : false);

    public static bool IS_C_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == CutVersion.C_CUT_VERSION) ? true : false);

    public static bool IS_D_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == CutVersion.D_CUT_VERSION) ? true : false);

    public static bool IS_E_CUT(HAL_VERSION version) =>
        ((GET_CVID_CUT_VERSION(version) == CutVersion.E_CUT_VERSION) ? true : false);

    public static bool IS_1T1R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HalRFType.RF_TYPE_1T1R) ? true : false);

    public static bool IS_1T2R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HalRFType.RF_TYPE_1T2R) ? true : false);

    public static bool IS_2T2R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HalRFType.RF_TYPE_2T2R) ? true : false);

    public static bool IS_3T3R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HalRFType.RF_TYPE_3T3R) ? true : false);

    public static bool IS_4T4R(HAL_VERSION version) =>
        ((GET_CVID_RF_TYPE(version) == HalRFType.RF_TYPE_4T4R) ? true : false);

    private static HalRFType GET_CVID_RF_TYPE(HAL_VERSION version) => version.RFType;

    private static CutVersion GET_CVID_CUT_VERSION(HAL_VERSION version) => version.CUTVersion;
}