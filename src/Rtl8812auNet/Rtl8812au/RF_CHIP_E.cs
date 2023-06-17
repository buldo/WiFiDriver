namespace Rtl8812auNet.Rtl8812au;

public enum RF_CHIP_E
{
    RF_CHIP_MIN = 0,    /* 0 */
    RF_8225 = 1,            /* 1 11b/g RF for verification only */
    RF_8256 = 2,            /* 2 11b/g/n */
    RF_8258 = 3,            /* 3 11a/b/g/n RF */
    RF_6052 = 4,            /* 4 11b/g/n RF */
    RF_PSEUDO_11N = 5,  /* 5, It is a temporality RF. */
    RF_CHIP_MAX
}