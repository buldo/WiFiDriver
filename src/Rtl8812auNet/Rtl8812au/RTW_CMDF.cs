namespace Rtl8812auNet.Rtl8812au;

[Flags]
public enum RTW_CMDF : uint
{
    RTW_CMDF_DIRECTLY = BIT0,
    RTW_CMDF_WAIT_ACK = BIT1,
};