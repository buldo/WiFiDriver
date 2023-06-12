namespace WiFiDriver.App.Rtl8812au;

public static class RcrBits
{
    public static uint RCR_AAP = BIT0;				/* accept all physical address */
    public static uint RCR_APM = BIT1;				/* accept physical match */
    public static uint RCR_AM = BIT2;				/* accept multicast */
    public static uint RCR_AB = BIT3;			/* accept broadcast */
    public static uint RCR_ACRC32 = BIT5;			/* accept error packet */
    public static uint RCR_9356SEL = BIT6;
    public static uint RCR_AICV = BIT9;		/* Accept ICV error packet */
    public static uint RCR_RXFTH0 = (BIT13 | BIT14 | BIT15);	/* Rx FIFO threshold */
    public static uint RCR_ADF = BIT18;		/* Accept Data(frame type) frame */
    public static uint RCR_ACF = BIT19;			/* Accept control frame */
    public static uint RCR_AMF = BIT20;			/* Accept management frame */
    public static uint RCR_ADD3 = BIT21;
    public static uint RCR_APWRMGT = BIT22;		/* Accept power management packet */
    public static uint RCR_CBSSID = BIT23;			/* Accept BSSID match packet */
    public static uint RCR_ENMARP = BIT28;			/* enable mac auto reset phy */
    public static uint RCR_EnCS1 = BIT29;			/* enable carrier sense method 1 */
    public static uint RCR_EnCS2 = BIT30;		/* enable carrier sense method 2 */
    public static uint RCR_OnlyErlPkt = BIT31; /* Rx Early mode is performed for packet size greater than 1536 */

    public static uint RCR_APP_PHYST_RXFF = BIT28;	/* PHY Status is appended before RX packet in RXFF */
    public static uint RCR_APPFCS = BIT31; /* WMAC append FCS after pauload */
    public static uint RCR_CBSSID_DATA = BIT6; /* Accept BSSID match packet (Data) */
    public static uint RCR_CBSSID_BCN = BIT7; /* Accept BSSID match packet (Rx beacon, probe rsp) */
    public static uint RCR_APP_ICV = BIT29; /* MACRX will retain the ICV at the bottom of the packet. */
    public static uint RCR_HTC_LOC_CTRL = BIT14; /* MFC<--HTC = 1 MFC-.HTC = 0 */
    public static uint RCR_APP_MIC = BIT30; /* MACRX will retain the MIC at the bottom of the packet. */
    public static uint FORCEACK = BIT26;

}