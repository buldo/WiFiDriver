namespace Rtl8812auNet.Rtl8812au;

public class rate_section_ent
{
    public rate_section_ent(RF_TX_NUM txNum, byte rateNum, MGN_RATE[] rates)
    {
        tx_num = txNum;
        rate_num = rateNum;
        this.rates = rates;
    }

    public RF_TX_NUM tx_num; /* value of RF_TX_NUM */
    public u8 rate_num;
    public MGN_RATE[] rates;
}