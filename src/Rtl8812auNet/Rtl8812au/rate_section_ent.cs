namespace Rtl8812auNet.Rtl8812au;

public class rate_section_ent
{
    public rate_section_ent(RF_TX_NUM txNum, byte rateNum, MGN_RATE[] rates)
    {
        this.rates = rates;
    }

    public MGN_RATE[] rates;
}