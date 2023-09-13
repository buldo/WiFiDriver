namespace Rtl8812auNet.Rtl8812au;

public class WlanPowerConfig
{
    public WlanPowerConfig(
        ushort offset,
        FabMsk fabMask,
        InterfaceMask interfaceMask,
        PwrCmd command,
        uint mask,
        uint value)
    {
        Offset = offset;
        FabMask = fabMask;
        InterfaceMask = interfaceMask;
        Command = command;
        Mask = mask;
        Value = value;
    }

    public UInt16 Offset { get; }
    public FabMsk FabMask { get; }
    public InterfaceMask InterfaceMask { get; }
    public PwrCmd Command { get; }
    public uint Mask { get; }
    public uint Value { get; }
}