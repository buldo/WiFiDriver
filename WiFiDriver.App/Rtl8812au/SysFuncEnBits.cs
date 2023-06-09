namespace WiFiDriver.App.Rtl8812au;

public static class SysFuncEnBits
{
    public const UInt16 FEN_BBRSTB = 1 << (0);
    public const UInt16 FEN_BB_GLB_RSTn = 1 << (1);
    public const UInt16 FEN_USBA = 1 << (2);
    public const UInt16 FEN_UPLL = 1 << (3);
    public const UInt16 FEN_USBD = 1 << (4);
    public const UInt16 FEN_DIO_PCIE = 1 << (5);
    public const UInt16 FEN_PCIEA = 1 << (6);
    public const UInt16 FEN_PPLL = 1 << (7);
    public const UInt16 FEN_PCIED = 1 << (8);
    public const UInt16 FEN_DIOE = 1 << (9);
    public const UInt16 FEN_CPUEN = 1 << (10);
    public const UInt16 FEN_DCORE = 1 << (11);
    public const UInt16 FEN_ELDR = 1 << (12);
    public const UInt16 FEN_EN_25_1 = 1 << (13);
    public const UInt16 FEN_HWPDN = 1 << (14);
    public const UInt16 FEN_MREGEN = 1 << (15);
}