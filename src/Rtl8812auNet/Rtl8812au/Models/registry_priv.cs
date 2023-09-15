using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

/// <summary>
/// Looks like this is static
/// </summary>
public static class registry_priv
{
    public static byte channel => 36; /* ad-hoc support requirement */
    public static RfType rf_config => RfType.RF_TYPE_MAX;
    public static bool wifi_spec => false; /* !turbo_mode */
    public static byte special_rf_path => 0; /* 0: 2T2R ,1: only turn on path A 1T1R */
    public static SByte TxBBSwing_2G => -1;
    public static SByte TxBBSwing_5G => -1;
    public static byte AmplifierType_2G => 0;
    public static byte AmplifierType_5G => 0;
    public static byte RFE_Type => 64;
}