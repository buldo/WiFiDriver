namespace Rtl8812auNet.Rtl8812au;

public class registry_priv
{
    public registry_priv()
    {
        channel = 36;
        rf_config = RfType.RF_TYPE_MAX;
        wifi_spec = false;
        special_rf_path = (u8)0;
        TxBBSwing_2G = -1;
        TxBBSwing_5G = -1;
        RFE_Type = 64;
        AmplifierType_2G = 0;
        AmplifierType_5G = 0;
    }

    public u8 channel { get; set; } /* ad-hoc support requirement */
    public RfType rf_config { get; set; }
    public bool wifi_spec { get; set; } /* !turbo_mode */
    public u8 special_rf_path { get; set; } /* 0: 2T2R ,1: only turn on path A 1T1R */
    public s8 TxBBSwing_2G { get; set; }
    public s8 TxBBSwing_5G { get; set; }
    public u8 AmplifierType_2G { get; set; }
    public u8 AmplifierType_5G { get; set; }
    public u8 RFE_Type { get; set; }
}