namespace Rtl8812auNet.Rtl8812au.Models;

public class BbRegisterDefinition
{
    /// <summary>
    /// LSSI data
    /// </summary>
    public UInt32 Rf3WireOffset { get; set; }

    /// <summary>
    /// wire parameter control2
    /// </summary>
    public UInt32 RfHSSIPara2 { get; set; }

    /// <summary>
    /// LSSI RF readback data SI mode
    /// </summary>
    public UInt16 RfLSSIReadBack { get; set; }

    /// <summary>
    /// LSSI RF readback data PI mode 0x8b8-8bc for Path A and B
    /// </summary>
    public UInt16 RfLSSIReadBackPi { get; set; }
}