namespace Rtl8812auNet.Rtl8812au;

public class BbRegisterDefinition
{
    public u32 rf3wireOffset;  /* LSSI data: */

    public u32 rfHSSIPara2;    /* wire parameter control2 :  */

    public u16 rfLSSIReadBack; /* LSSI RF readback data SI mode */

    public u16 rfLSSIReadBackPi;	/* LSSI RF readback data PI mode 0x8b8-8bc for Path A and B */
}