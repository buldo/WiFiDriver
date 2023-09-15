using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum RTL871X_HCI_TYPE : uint
{
    RTW_PCIE = BIT0,
    RTW_USB = BIT1,
    RTW_SDIO = BIT2,
    RTW_GSPI = BIT3
};