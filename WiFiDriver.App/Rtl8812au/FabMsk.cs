namespace WiFiDriver.App.Rtl8812au;

[Flags]
public enum FabMsk : byte
{
    PWR_FAB_TSMC_MSK = 1 << (0),
    PWR_FAB_UMC_MSK = 1 << (1),
    PWR_FAB_ALL_MSK = (1 << (0) | 1 << (1) | 1 << (2) | 1 << (3))
}