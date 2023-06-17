namespace Rtl8812auNet.Rtl8812au;

public static class rtw_ieee80211
{
    private static MGN_RATE[] mgn_rates_cck =
    {
        MGN_RATE.MGN_1M,
        MGN_RATE.MGN_2M,
        MGN_RATE.MGN_5_5M,
        MGN_RATE.MGN_11M
    };

    private static MGN_RATE[] mgn_rates_ofdm =
    {
        MGN_RATE.MGN_6M,
        MGN_RATE.MGN_9M,
        MGN_RATE.MGN_12M,
        MGN_RATE.MGN_18M,
        MGN_RATE.MGN_24M,
        MGN_RATE.MGN_36M,
        MGN_RATE.MGN_48M,
        MGN_RATE.MGN_54M
    };

    static MGN_RATE[] mgn_rates_mcs0_7 =
    {
        MGN_RATE.MGN_MCS0,
        MGN_RATE.MGN_MCS1,
        MGN_RATE.MGN_MCS2,
        MGN_RATE.MGN_MCS3,
        MGN_RATE.MGN_MCS4,
        MGN_RATE.MGN_MCS5,
        MGN_RATE.MGN_MCS6,
        MGN_RATE.MGN_MCS7
    };

    static MGN_RATE[] mgn_rates_mcs8_15 =
    {
        MGN_RATE.MGN_MCS8,
        MGN_RATE.MGN_MCS9,
        MGN_RATE.MGN_MCS10,
        MGN_RATE.MGN_MCS11,
        MGN_RATE.MGN_MCS12,
        MGN_RATE.MGN_MCS13,
        MGN_RATE.MGN_MCS14,
        MGN_RATE.MGN_MCS15
    };

    static MGN_RATE[] mgn_rates_mcs16_23 =
    {
        MGN_RATE.MGN_MCS16,
        MGN_RATE.MGN_MCS17,
        MGN_RATE.MGN_MCS18,
        MGN_RATE.MGN_MCS19,
        MGN_RATE.MGN_MCS20,
        MGN_RATE.MGN_MCS21,
        MGN_RATE.MGN_MCS22,
        MGN_RATE.MGN_MCS23
    };

    private static MGN_RATE[] mgn_rates_mcs24_31 =
    {
        MGN_RATE.MGN_MCS24,
        MGN_RATE.MGN_MCS25,
        MGN_RATE.MGN_MCS26,
        MGN_RATE.MGN_MCS27,
        MGN_RATE.MGN_MCS28,
        MGN_RATE.MGN_MCS29,
        MGN_RATE.MGN_MCS30,
        MGN_RATE.MGN_MCS31
    };

    private static MGN_RATE[] mgn_rates_vht1ss =
    {
        MGN_RATE.MGN_VHT1SS_MCS0,
        MGN_RATE.MGN_VHT1SS_MCS1,
        MGN_RATE.MGN_VHT1SS_MCS2,
        MGN_RATE.MGN_VHT1SS_MCS3,
        MGN_RATE.MGN_VHT1SS_MCS4,
        MGN_RATE.MGN_VHT1SS_MCS5,
        MGN_RATE.MGN_VHT1SS_MCS6,
        MGN_RATE.MGN_VHT1SS_MCS7,
        MGN_RATE.MGN_VHT1SS_MCS8,
        MGN_RATE.MGN_VHT1SS_MCS9
    };

    static MGN_RATE[] mgn_rates_vht2ss =
    {
        MGN_RATE.MGN_VHT2SS_MCS0,
        MGN_RATE.MGN_VHT2SS_MCS1,
        MGN_RATE.MGN_VHT2SS_MCS2,
        MGN_RATE.MGN_VHT2SS_MCS3,
        MGN_RATE.MGN_VHT2SS_MCS4,
        MGN_RATE.MGN_VHT2SS_MCS5,
        MGN_RATE.MGN_VHT2SS_MCS6,
        MGN_RATE.MGN_VHT2SS_MCS7,
        MGN_RATE.MGN_VHT2SS_MCS8,
        MGN_RATE.MGN_VHT2SS_MCS9
    };

    static MGN_RATE[] mgn_rates_vht3ss =
    {
        MGN_RATE.MGN_VHT3SS_MCS0,
        MGN_RATE.MGN_VHT3SS_MCS1,
        MGN_RATE.MGN_VHT3SS_MCS2,
        MGN_RATE.MGN_VHT3SS_MCS3,
        MGN_RATE.MGN_VHT3SS_MCS4,
        MGN_RATE.MGN_VHT3SS_MCS5,
        MGN_RATE.MGN_VHT3SS_MCS6,
        MGN_RATE.MGN_VHT3SS_MCS7,
        MGN_RATE.MGN_VHT3SS_MCS8,
        MGN_RATE.MGN_VHT3SS_MCS9
    };

    static MGN_RATE[] mgn_rates_vht4ss =
    {
        MGN_RATE.MGN_VHT4SS_MCS0,
        MGN_RATE.MGN_VHT4SS_MCS1,
        MGN_RATE.MGN_VHT4SS_MCS2,
        MGN_RATE.MGN_VHT4SS_MCS3,
        MGN_RATE.MGN_VHT4SS_MCS4,
        MGN_RATE.MGN_VHT4SS_MCS5,
        MGN_RATE.MGN_VHT4SS_MCS6,
        MGN_RATE.MGN_VHT4SS_MCS7,
        MGN_RATE.MGN_VHT4SS_MCS8,
        MGN_RATE.MGN_VHT4SS_MCS9
    };

    public static rate_section_ent[] rates_by_sections =
    {
        new(RfTxNum.RF_1TX, 4, mgn_rates_cck),
        new(RfTxNum.RF_1TX, 8, mgn_rates_ofdm),
        new(RfTxNum.RF_1TX, 8, mgn_rates_mcs0_7),
        new(RfTxNum.RF_2TX, 8, mgn_rates_mcs8_15),
        new(RfTxNum.RF_3TX, 8, mgn_rates_mcs16_23),
        new(RfTxNum.RF_4TX, 8, mgn_rates_mcs24_31),
        new(RfTxNum.RF_1TX, 10, mgn_rates_vht1ss),
        new(RfTxNum.RF_2TX, 10, mgn_rates_vht2ss),
        new(RfTxNum.RF_3TX, 10, mgn_rates_vht3ss),
        new(RfTxNum.RF_4TX, 10, mgn_rates_vht4ss),
    };
}