namespace WiFiDriver.App.Rtl8812au;

public class BB_REGISTER_DEFINITION_T
{
    public u32 rfintfs;            /* set software control: */
    /*		0x870~0x877[8 bytes] */

    public u32 rfintfo;            /* output data: */
    /*		0x860~0x86f [16 bytes] */

    public u32 rfintfe;            /* output enable: */
    /*		0x860~0x86f [16 bytes] */

    public u32 rf3wireOffset;  /* LSSI data: */
    /*		0x840~0x84f [16 bytes] */

    public u32 rfHSSIPara2;    /* wire parameter control2 :  */
    /*		0x824~0x827,0x82c~0x82f, 0x834~0x837, 0x83c~0x83f [16 bytes] */

    public u16 rfLSSIReadBack; /* LSSI RF readback data SI mode */
    /*		0x8a0~0x8af [16 bytes] */

    public u16 rfLSSIReadBackPi;	/* LSSI RF readback data PI mode 0x8b8-8bc for Path A and B */

}