using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum RATE_SECTION
{
    CCK = 0,
    OFDM = 1,
    HT_MCS0_MCS7 = 2,
    HT_MCS8_MCS15 = 3,
    HT_MCS16_MCS23 = 4,
    HT_MCS24_MCS31 = 5,
    HT_1SS = HT_MCS0_MCS7,
    HT_2SS = HT_MCS8_MCS15,
    HT_3SS = HT_MCS16_MCS23,
    HT_4SS = HT_MCS24_MCS31,
    VHT_1SSMCS0_1SSMCS9 = 6,
    VHT_2SSMCS0_2SSMCS9 = 7,
    VHT_3SSMCS0_3SSMCS9 = 8,
    VHT_4SSMCS0_4SSMCS9 = 9,
    VHT_1SS = VHT_1SSMCS0_1SSMCS9,
    VHT_2SS = VHT_2SSMCS0_2SSMCS9,
    VHT_3SS = VHT_3SSMCS0_3SSMCS9,
    VHT_4SS = VHT_4SSMCS0_4SSMCS9,
    RATE_SECTION_NUM,
}