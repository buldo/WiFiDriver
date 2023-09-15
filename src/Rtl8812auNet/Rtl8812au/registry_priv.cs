using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au;

/// <summary>
/// Looks like this is static
/// </summary>
public static class registry_priv
{
    public static u8 channel => 36; /* ad-hoc support requirement */
    public static RfType rf_config => RfType.RF_TYPE_MAX;
    public static bool wifi_spec => false; /* !turbo_mode */
    public static u8 special_rf_path => (u8)0; /* 0: 2T2R ,1: only turn on path A 1T1R */
    public static s8 TxBBSwing_2G => -1;
    public static s8 TxBBSwing_5G => -1;
    public static u8 AmplifierType_2G => 0;
    public static u8 AmplifierType_5G => 0;
    public static u8 RFE_Type => 64;
}