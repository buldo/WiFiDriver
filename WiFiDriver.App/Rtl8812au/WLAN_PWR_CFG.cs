namespace WiFiDriver.App.Rtl8812au;

public class WLAN_PWR_CFG
{
    public WLAN_PWR_CFG(
        ushort offset,
        CutMsk cutMsk,
        FabMsk fabMsk,
        InterfaceMask interfaceMsk,
        PwrBaseaddr @base,
        PwrCmd cmd,
        byte msk,
        byte value)
    {
        this.offset = offset;
        cut_msk = cutMsk;
        fab_msk = fabMsk;
        interface_msk = interfaceMsk;
        base_ = @base;
        this.cmd = cmd;
        this.msk = msk;
        this.value = value;
    }

    public UInt16 offset { get; init; }
    public CutMsk cut_msk { get; init; }
    public FabMsk fab_msk { get; init; }
    public InterfaceMask interface_msk { get; init; }
    public PwrBaseaddr base_ { get; init; }
    public PwrCmd cmd { get; init; }
    public byte msk { get; init; }
    public byte value { get; init; }
}