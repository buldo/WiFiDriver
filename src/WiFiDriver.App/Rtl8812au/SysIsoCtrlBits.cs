namespace WiFiDriver.App.Rtl8812au;

public static class SysIsoCtrlBits
{
    public const UInt16 ISO_MD2PP = 1 << (0);
    public const UInt16 ISO_UA2USB = 1 << (1);
    public const UInt16 ISO_UD2CORE = 1 << (2);
    public const UInt16 ISO_PA2PCIE = 1 << (3);
    public const UInt16 ISO_PD2CORE = 1 << (4);
    public const UInt16 ISO_IP2MAC = 1 << (5);
    public const UInt16 ISO_DIOP = 1 << (6);
    public const UInt16 ISO_DIOE = 1 << (7);
    public const UInt16 ISO_EB2CORE = 1 << (8);
    public const UInt16 ISO_DIOR = 1 << (9);
    public const UInt16 PWC_EV12V = 1 << (15);
}