namespace Rtl8812auNet.Rtl8812au.Enumerations;

public static class RcrBits
{
    public static uint RCR_APPFCS = BIT31;	/* WMAC append FCS after pauload */
    public static uint RCR_APP_MIC = BIT30;/* MACRX will retain the MIC at the bottom of the packet. */
    public static uint RCR_APP_ICV = BIT29;/* MACRX will retain the ICV at the bottom of the packet. */
    public static uint RCR_APP_PHYST_RXFF = BIT28;/* PHY Status is appended before RX packet in RXFF */
    public static uint RCR_HTC_LOC_CTRL = BIT14;/* MFC<--HTC = 1 MFC-.HTC = 0 */
    public static uint RCR_AMF = BIT13;/* Accept management type frame */
    public static uint RCR_ACF = BIT12;/* Accept control type frame. Control frames BA, BAR, and PS-Poll (when in AP mode) are not controlled by this bit. They are controlled by ADF. */
    public static uint RCR_ADF = BIT11;/* Accept data type frame. This bit also regulates BA, BAR, and PS-Poll (AP mode only). */
    public static uint RCR_CBSSID_BCN = BIT7;/* Accept BSSID match packet (Rx beacon, probe rsp) */
    public static uint RCR_CBSSID_DATA = BIT6;/* Accept BSSID match packet (Data) */
    public static uint RCR_APWRMGT = BIT5;/* Accept power management packet */
    public static uint RCR_AB = BIT3;/* Accept broadcast packet */
    public static uint RCR_AM = BIT2;/* Accept multicast packet */
    public static uint RCR_APM = BIT1;/* Accept physical match packet */
    public static uint RCR_AAP = BIT0;/* Accept all unicast packet */

    public static uint FORCEACK = BIT26;
}