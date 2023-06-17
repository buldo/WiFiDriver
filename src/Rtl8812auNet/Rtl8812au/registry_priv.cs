namespace Rtl8812auNet.Rtl8812au;

public class registry_priv
{
    public u8 channel;/* ad-hoc support requirement */
    public NETWORK_TYPE wireless_mode;/* A, B, G, auto */
    public rf_type rf_config;
    public bool wifi_spec;/* !turbo_mode */
    public     u8 special_rf_path; /* 0: 2T2R ,1: only turn on path A 1T1R */
    public     s8 TxBBSwing_2G;
    public     s8 TxBBSwing_5G;
    public u8 AmplifierType_2G;
    public u8 AmplifierType_5G;
    public u8 RFE_Type;
};