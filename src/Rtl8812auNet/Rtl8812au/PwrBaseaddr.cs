namespace Rtl8812auNet.Rtl8812au;

public enum PwrBaseaddr : byte
{
    PWR_BASEADDR_MAC = 0x00,
    PWR_BASEADDR_USB = 0x01,
    PWR_BASEADDR_PCIE = 0x02,
    PWR_BASEADDR_SDIO = 0x03
}