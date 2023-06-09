namespace WiFiDriver.App.Rtl8812au;

public class TxPowerInfo5G
{
    public u8[,] IndexBW40_Base = new byte[MAX_RF_PATH,MAX_CHNL_GROUP_5G];
    /* If only one tx, only BW20, OFDM, BW80 and BW160 are used. */
    public s8[,] OFDM_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW20_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW40_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW80_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW160_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
}