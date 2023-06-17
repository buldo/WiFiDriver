namespace Rtl8812auNet.Rtl8812au;

public class TxPowerInfo24G
{
    public u8[,] IndexCCK_Base = new byte[MAX_RF_PATH, MAX_CHNL_GROUP_24G];

    public u8[,] IndexBW40_Base = new byte[MAX_RF_PATH, MAX_CHNL_GROUP_24G];
    /* If only one tx, only BW20 and OFDM are used. */
    public s8[,] CCK_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] OFDM_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW20_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
    public s8[,] BW40_Diff = new s8[MAX_RF_PATH,MAX_TX_COUNT];
}