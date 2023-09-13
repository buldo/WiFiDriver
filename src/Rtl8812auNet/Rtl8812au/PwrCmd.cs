namespace Rtl8812auNet.Rtl8812au;

public enum PwrCmd : byte
{
    /* offset: the read register offset
     * Mask: the mask of the read value
     * value: N/A, left by 0
     * note: dirver shall implement this function by read & Mask */
    PWR_CMD_READ = 0x00,

    /* offset: the read register offset
     * Mask: the mask of the write bits
     * value: write value
     * note: driver shall implement this cmd by read & Mask after write */
    PWR_CMD_WRITE = 0x01,

    /* offset: the read register offset
     * Mask: the mask of the polled value
     * value: the value to be polled, masked by the msd field.
     * note: driver shall implement this cmd by
     * do {
     * if( (Read(offset) & Mask) == (value & Mask) )
     * break;
     * } while(not timeout); */
    PWR_CMD_POLLING = 0x02,

    /* offset: the value to delay
     * Mask: N/A
     * value: the unit of delay, 0: us, 1: ms */
    PWR_CMD_DELAY = 0x03,

    /* offset: N/A
     * Mask: N/A
     * value: N/A */
    PWR_CMD_END = 0x04
}