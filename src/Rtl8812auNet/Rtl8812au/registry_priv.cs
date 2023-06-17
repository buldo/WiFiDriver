namespace Rtl8812auNet.Rtl8812au;

public class registry_priv
{
    public u8 channel { get; set; }/* ad-hoc support requirement */
    public NETWORK_TYPE wireless_mode { get; set; }/* A, B, G, auto */
    public rf_type rf_config { get; set; }
    public bool wifi_spec { get; set; }/* !turbo_mode */
    public     u8 special_rf_path { get; set; } /* 0: 2T2R ,1: only turn on path A 1T1R */
    public     s8 TxBBSwing_2G { get; set; }
    public     s8 TxBBSwing_5G { get; set; }
    public u8 AmplifierType_2G { get; set; }
    public u8 AmplifierType_5G { get; set; }
    public u8 RFE_Type { get; set; }
};