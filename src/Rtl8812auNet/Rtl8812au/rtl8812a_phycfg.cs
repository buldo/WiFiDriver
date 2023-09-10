namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_phycfg
{
    public static u32 PHY_CalculateBitShift(u32 BitMask)
    {
        int i;

        for (i = 0; i <= 31; i++)
        {
            if (((BitMask >> i) & 0x1) == 1)
                break;
        }

        return (u32)i;
    }
}