namespace Rtl8812auNet.Rtl8812au;

[Flags]
public enum NETWORK_TYPE : uint
{
    WIRELESS_INVALID = 0,
    /* Sub-Element */
    WIRELESS_11B = BIT0, /* tx: cck only , rx: cck only, hw: cck */
    WIRELESS_11G = BIT1, /* tx: ofdm only, rx: ofdm & cck, hw: cck & ofdm */
    WIRELESS_11A = BIT2, /* tx: ofdm only, rx: ofdm only, hw: ofdm only */
    WIRELESS_11_24N = BIT3, /* tx: MCS only, rx: MCS & cck, hw: MCS & cck */
    WIRELESS_11_5N = BIT4, /* tx: MCS only, rx: MCS & ofdm, hw: ofdm only */
    WIRELESS_AUTO = BIT5,
    WIRELESS_11AC = BIT6,

    /* Combination */
    /* Type for current wireless mode */
    WIRELESS_11BG = (WIRELESS_11B | WIRELESS_11G), /* tx: cck & ofdm, rx: cck & ofdm & MCS, hw: cck & ofdm */
    WIRELESS_11G_24N = (WIRELESS_11G | WIRELESS_11_24N), /* tx: ofdm & MCS, rx: ofdm & cck & MCS, hw: cck & ofdm */
    WIRELESS_11A_5N = (WIRELESS_11A | WIRELESS_11_5N), /* tx: ofdm & MCS, rx: ofdm & MCS, hw: ofdm only */
    WIRELESS_11B_24N = (WIRELESS_11B | WIRELESS_11_24N), /* tx: ofdm & cck & MCS, rx: ofdm & cck & MCS, hw: ofdm & cck */
    WIRELESS_11BG_24N = (WIRELESS_11B | WIRELESS_11G | WIRELESS_11_24N), /* tx: ofdm & cck & MCS, rx: ofdm & cck & MCS, hw: ofdm & cck */
    WIRELESS_11_24AC = (WIRELESS_11B | WIRELESS_11G | WIRELESS_11AC),
    WIRELESS_11_5AC = (WIRELESS_11A | WIRELESS_11AC),


    /* Type for registry default wireless mode */
    WIRELESS_11AGN = (WIRELESS_11A | WIRELESS_11G | WIRELESS_11_24N | WIRELESS_11_5N), /* tx: ofdm & MCS, rx: ofdm & MCS, hw: ofdm only */
    WIRELESS_11ABGN = (WIRELESS_11A | WIRELESS_11B | WIRELESS_11G | WIRELESS_11_24N | WIRELESS_11_5N),
    WIRELESS_MODE_24G = (WIRELESS_11B | WIRELESS_11G | WIRELESS_11_24N),
    WIRELESS_MODE_5G = (WIRELESS_11A | WIRELESS_11_5N | WIRELESS_11AC),
    WIRELESS_MODE_MAX = (WIRELESS_11A | WIRELESS_11B | WIRELESS_11G | WIRELESS_11_24N | WIRELESS_11_5N | WIRELESS_11AC),
};