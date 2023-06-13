namespace WiFiDriver.App.Rtl8812au;

[Flags]
public enum TxSele : byte
{
    TX_SELE_HQ = 1 << (0), /* High Queue */
    TX_SELE_LQ = 1 << (1), /* Low Queue */
    TX_SELE_NQ = 1 << (2), /* Normal Queue */
    TX_SELE_EQ = 1 << (3), /* Extern Queue */
}