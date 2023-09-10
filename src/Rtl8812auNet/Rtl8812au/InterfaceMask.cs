namespace Rtl8812auNet.Rtl8812au;

[Flags]
public enum InterfaceMask : byte
{
    PWR_INTF_USB_MSK = 1 << (1),
    PWR_INTF_PCI_MSK = 1 << (2),
    PWR_INTF_ALL_MSK = (1 << (0) | 1 << (1) | 1 << (2) | 1 << (3))
}