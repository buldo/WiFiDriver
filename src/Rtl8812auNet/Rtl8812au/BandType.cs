using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum BandType
{
    BAND_ON_2_4G = 0,
    BAND_ON_5G = 1,
    BAND_ON_BOTH = 2,
    BAND_MAX = 3,
}