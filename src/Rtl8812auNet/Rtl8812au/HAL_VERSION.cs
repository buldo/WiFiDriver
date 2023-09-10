namespace Rtl8812auNet.Rtl8812au;

public class HAL_VERSION
{
    public CutVersion CUTVersion { get; set; }
    public HalRFType RFType { get; set; }

    public (RfType rf_type, u8 NumTotalRFPath) GetRfType()
    {
        if (IS_1T1R(this))
        {
            return (RfType.RF_1T1R, 1);
        }

        if (IS_2T2R(this))
        {
            return (RfType.RF_2T2R, 2);
        }

        if (IS_1T2R(this))
        {
            return (RfType.RF_1T2R, 2);
        }

        if (IS_3T3R(this))
        {
            return (RfType.RF_3T3R, 3);
        }

        if (IS_4T4R(this))
        {
            return (RfType.RF_4T4R, 4);
        }


        return (RfType.RF_1T1R, 1);
    }

}