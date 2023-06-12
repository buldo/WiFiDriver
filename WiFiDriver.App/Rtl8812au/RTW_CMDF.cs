namespace WiFiDriver.App.Rtl8812au;

[Flags]
public enum RTW_CMDF
{
    RTW_CMDF_DIRECTLY = BIT0,
    RTW_CMDF_WAIT_ACK = BIT1,
};