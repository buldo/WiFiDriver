using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class WlanPowerConfig
{
    public WlanPowerConfig(
        ushort offset,
        PwrCmd command,
        uint mask,
        uint value)
    {
        Offset = offset;
        Command = command;
        Mask = mask;
        Value = value;
    }

    public UInt16 Offset { get; }
    public PwrCmd Command { get; }
    public uint Mask { get; }
    public uint Value { get; }
}