namespace Rtl8812auNet.Rtl8812au;

public static class PowerSequences
{
    private static WlanPowerConfig[] RTL8812_TRANS_CARDDIS_TO_CARDEMU { get; } =
    {
        /* format */
        /* { offset, CutMask, FabMask|InterfaceMask, base|cmd, Mask, value }, // comments here*/
        new (0x0012, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT0, BIT0),/*0x12[0] = 1 force PWM mode */
        new (0x0014, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, 0x80, 0),/*0x14[7] = 0 turn off ZCD */
        new (0x0015, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, 0x01, 0),/* 0x15[0] =0 trun off ZCD */
        new (0x0023, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, 0x10, 0),/*0x23[4] = 0 hpon LDO leave sleep mode */
        new (0x0046, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, 0xFF, 0x00),/* gpio0~7 input mode */
        new (0x0043, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, 0xFF, 0x00),/* gpio11 input mode, gpio10~8 input mode */
        new (0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_PCI_MSK, PwrCmd.PWR_CMD_WRITE, BIT2, 0), /*0x04[10] = 0, enable SW LPS PCIE only*/
        new (0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT3, 0), /*0x04[11] = 2b'01enable WL suspend*/
        new (0x0003, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT2, BIT2), /*0x03[2] = 1, enable 8051*/
        new (0x0301, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_PCI_MSK, PwrCmd.PWR_CMD_WRITE, 0xFF, 0),/*PCIe DMA start*/
        new (0x0024, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT1, BIT1), /* 0x24[1] Choose the type of buffer after xosc: schmitt trigger*/
        new (0x0028, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT3, BIT3), /* 0x28[33] Choose the type of buffer after xosc: schmitt trigger*/

    };

    private static WlanPowerConfig[] RTL8812_TRANS_CARDEMU_TO_ACT { get; } =
    {
        /* format */
        /* { offset, CutMask, FabMask|InterfaceMask, base|cmd, Mask, value }, // comments here*/
        new(0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT2, 0), /* disable SW LPS 0x04[10]=0*/
        new(0x0006, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_POLLING, BIT1, BIT1), /* wait till 0x04[17] = 1    power ready*/
        /*{0x0005, PWR_CUT_ALL_MSK, PWR_FAB_ALL_MSK, PWR_INTF_ALL_MSK, PWR_BASEADDR_MAC, PWR_CMD_WRITE, BIT7, 0}, disable HWPDN 0x04[15]=0*/
        new(0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT3, 0), /* disable WL suspend*/
        new(0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT0, BIT0), /* polling until return 0*/
        new(0x0005, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_POLLING, BIT0, 0), /**/
        new(0x0024, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT1, 0), /* 0x24[1] Choose the type of buffer after xosc: nand*/
        new(0x0028, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_WRITE, BIT3, 0)  /* 0x28[33] Choose the type of buffer after xosc: nand*/

    };

    private static WlanPowerConfig[] RTL8812_TRANS_END { get; } =
    {
        new(0xFFFF, FabMsk.PWR_FAB_ALL_MSK, InterfaceMask.PWR_INTF_ALL_MSK, PwrCmd.PWR_CMD_END, 0, 0)
    };

    public static WlanPowerConfig[] Rtl8812_NIC_ENABLE_FLOW { get; } =
        RTL8812_TRANS_CARDDIS_TO_CARDEMU
            .Concat(RTL8812_TRANS_CARDEMU_TO_ACT)
            .Concat(RTL8812_TRANS_END)
            .ToArray();
}