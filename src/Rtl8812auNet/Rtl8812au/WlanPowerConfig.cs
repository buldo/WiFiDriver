namespace Rtl8812auNet.Rtl8812au;

public class WlanPowerConfig
{
    public WlanPowerConfig(
        ushort offset,
        InterfaceMask interfaceMask,
        PwrCmd command,
        uint mask,
        uint value)
    {
        Offset = offset;
        InterfaceMask = interfaceMask;
        Command = command;
        Mask = mask;
        Value = value;
    }

    public UInt16 Offset { get; }
    public InterfaceMask InterfaceMask { get; }
    public PwrCmd Command { get; }
    public uint Mask { get; }
    public uint Value { get; }
}