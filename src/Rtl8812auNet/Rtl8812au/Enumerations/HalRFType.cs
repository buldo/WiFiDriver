using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum HalRFType
{
    RF_TYPE_1T1R = 0,
    RF_TYPE_1T2R = 1,
    RF_TYPE_2T2R = 2,
    RF_TYPE_2T3R = 3,
    RF_TYPE_2T4R = 4,
    RF_TYPE_3T3R = 5,
    RF_TYPE_3T4R = 6,
    RF_TYPE_4T4R = 7,
}