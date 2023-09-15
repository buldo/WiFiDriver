using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class rate_section_ent
{
    public rate_section_ent(MGN_RATE[] rates)
    {
        this.rates = rates;
    }

    public MGN_RATE[] rates;
}