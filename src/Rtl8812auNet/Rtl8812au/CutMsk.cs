using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au;

[Flags]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum CutMsk : byte
{
    PWR_CUT_TESTCHIP_MSK = 1 << (0),
    PWR_CUT_A_MSK = 1 << (1),
    PWR_CUT_B_MSK = 1 << (2),
    PWR_CUT_C_MSK = 1 << (3),
    PWR_CUT_D_MSK = 1 << (4),
    PWR_CUT_E_MSK = 1 << (5),
    PWR_CUT_F_MSK = 1 << (6),
    PWR_CUT_G_MSK = 1 << (7),
    PWR_CUT_ALL_MSK = 0xFF
}