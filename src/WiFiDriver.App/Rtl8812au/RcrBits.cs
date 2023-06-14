namespace WiFiDriver.App.Rtl8812au;

public static class RcrBits
{
    public static uint RCR_APPFCS = BIT31;	/* WMAC append FCS after pauload */
    public static uint RCR_APP_MIC = BIT30;/* MACRX will retain the MIC at the bottom of the packet. */
    public static uint RCR_APP_ICV = BIT29;/* MACRX will retain the ICV at the bottom of the packet. */
    public static uint RCR_APP_PHYST_RXFF = BIT28;/* PHY Status is appended before RX packet in RXFF */
    public static uint RCR_APP_BA_SSN = BIT27;/* SSN of previous TXBA is appended as after original RXDESC as the 4-th DW of RXDESC. */
    public static uint RCR_VHT_DACK = BIT26;/* This bit to control response type for vht single mpdu data packet. 1. ACK as response 0. BA as response */
    public static uint RCR_TCPOFLD_EN = BIT25;/* Enable TCP checksum offload */
    public static uint RCR_ENMBID = BIT24;/* Enable Multiple BssId. Only response ACK to the packets whose DID(A1) matching to the addresses in the MBSSID CAM Entries. */
    public static uint RCR_LSIGEN = BIT23;/* Enable LSIG TXOP Protection function. Search KEYCAM for each rx packet to check if LSIGEN bit is set. */
    public static uint RCR_MFBEN = BIT22;/* Enable immediate MCS Feedback function. When Rx packet with MRQ = 1'b1, then search KEYCAM to find sender's MCS Feedback function and send response. */
    public static uint RCR_DISCHKPPDLLEN = BIT21;/* Do not check PPDU while the PPDU length is smaller than 14 byte. */
    public static uint RCR_PKTCTL_DLEN = BIT20;/* While rx path dead lock occurs, reset rx path */
    public static uint RCR_DISGCLK = BIT19;/* Disable macrx clock gating control (no used) */
    public static uint RCR_TIM_PARSER_EN = BIT18;/* RX Beacon TIM Parser. */
    public static uint RCR_BC_MD_EN = BIT17;/* Broadcast data packet more data bit check interrupt enable.*/
    public static uint RCR_UC_MD_EN = BIT16;/* Unicast data packet more data bit check interrupt enable. */
    public static uint RCR_RXSK_PERPKT = BIT15;/* Executing key search per MPDU */
    public static uint RCR_HTC_LOC_CTRL = BIT14;/* MFC<--HTC = 1 MFC-.HTC = 0 */
    public static uint RCR_AMF = BIT13;/* Accept management type frame */
    public static uint RCR_ACF = BIT12;/* Accept control type frame. Control frames BA, BAR, and PS-Poll (when in AP mode) are not controlled by this bit. They are controlled by ADF. */
    public static uint RCR_ADF = BIT11;/* Accept data type frame. This bit also regulates BA, BAR, and PS-Poll (AP mode only). */
    public static uint RCR_DISDECMYPKT = BIT10;/* This bit determines whether hw need to do decryption.1: If A1 match, do decryption.0: Do decryption. */
    public static uint RCR_AICV = BIT9;/* Accept ICV error packet */
    public static uint RCR_ACRC32 = BIT8;/* Accept CRC32 error packet */
    public static uint RCR_CBSSID_BCN = BIT7;/* Accept BSSID match packet (Rx beacon, probe rsp) */
    public static uint RCR_CBSSID_DATA = BIT6;/* Accept BSSID match packet (Data) */
    public static uint RCR_APWRMGT = BIT5;/* Accept power management packet */
    public static uint RCR_ADD3 = BIT4;/* Accept address 3 match packet */
    public static uint RCR_AB = BIT3;/* Accept broadcast packet */
    public static uint RCR_AM = BIT2;/* Accept multicast packet */
    public static uint RCR_APM = BIT1;/* Accept physical match packet */
    public static uint RCR_AAP = BIT0;/* Accept all unicast packet */

    public static uint FORCEACK = BIT26;

}