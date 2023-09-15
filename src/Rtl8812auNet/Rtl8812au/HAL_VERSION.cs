using Rtl8812auNet.Rtl8812au.Enumerations;
namespace Rtl8812auNet.Rtl8812au;

public class HAL_VERSION
{
    public CutVersion CUTVersion { get; set; }
    public HalRFType RFType { get; set; }

    public (RfType rf_type, u8 NumTotalRFPath) GetRfType()
    {
        if (IS_1T1R())
        {
            return (RfType.RF_1T1R, 1);
        }

        if (IS_2T2R())
        {
            return (RfType.RF_2T2R, 2);
        }

        if (IS_1T2R())
        {
            return (RfType.RF_1T2R, 2);
        }

        if (IS_3T3R())
        {
            return (RfType.RF_3T3R, 3);
        }

        if (IS_4T4R())
        {
            return (RfType.RF_4T4R, 4);
        }


        return (RfType.RF_1T1R, 1);
    }

    public bool IS_A_CUT() => CUTVersion == CutVersion.A_CUT_VERSION;

    public bool IS_C_CUT() => CUTVersion == CutVersion.C_CUT_VERSION;

    private bool IS_1T1R() => RFType == HalRFType.RF_TYPE_1T1R;

    private bool IS_1T2R() => RFType == HalRFType.RF_TYPE_1T2R;

    private bool IS_2T2R() => RFType == HalRFType.RF_TYPE_2T2R;

    private bool IS_3T3R() => RFType == HalRFType.RF_TYPE_3T3R;

    private bool IS_4T4R() => RFType == HalRFType.RF_TYPE_4T4R;
}