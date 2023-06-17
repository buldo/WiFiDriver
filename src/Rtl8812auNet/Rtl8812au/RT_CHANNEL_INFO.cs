namespace WiFiDriver.App.Rtl8812au;

public class RT_CHANNEL_INFO
{
    public u8 ChannelNum;      /* The channel number. */

    public RT_SCAN_TYPE ScanType;      /* Scan type such as passive or active scan. */
    /* u16				ScanPeriod;		 */ /* Listen time in millisecond in this channel. */
    /* s32				MaxTxPwrDbm;	 */ /* Max allowed tx power. */
    /* u32				ExInfo;			 */ /* Extended Information for this channel. */
    u32 rx_count;
    u8 hidden_bss_cnt; /* per scan count */
}