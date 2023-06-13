namespace WiFiDriver.App.Rtl8812au;

public static class Rtl8812aDevices
{
    private const UInt16 USB_VENDER_ID_REALTEK = 0x0BDA;

    public static List<UsbDeviceVidPid> Devices { get; } = new()
    {
        /*=== Realtek demoboard ===*/
        new(USB_VENDER_ID_REALTEK, 0x8812), /* Default ID */
        new(USB_VENDER_ID_REALTEK, 0x881A), /* Default ID */
        new(USB_VENDER_ID_REALTEK, 0x881B), /* Default ID */
        new(USB_VENDER_ID_REALTEK, 0x881C), /* Default ID */
        /*=== Customer ID ===*/
        new(0x050D, 0x1106), /* Belkin - sercomm */
        new(0x7392, 0xA822), /* Edimax - Edimax */
        new(0x0DF6, 0x0074), /* Sitecom - Edimax */
        new(0x04BB, 0x0952), /* I-O DATA - Edimax */
        new(0x0789, 0x016E), /* Logitec - Edimax */
        new(0x0409, 0x0408), /* NEC - */
        new(0x0B05, 0x17D2), /* ASUS - Edimax */
        new(0x0E66, 0x0022), /* HAWKING - Edimax */
        new(0x0586, 0x3426), /* ZyXEL - */
        new(0x2001, 0x3313), /* D-Link - ALPHA */
        new(0x1058, 0x0632), /* WD - Cybertan */
        new(0x1740, 0x0100), /* EnGenius - EnGenius */
        new(0x2019, 0xAB30), /* Planex - Abocom */
        new(0x07B8, 0x8812), /* Abocom - Abocom */
        new(0x0846, 0x9051), /* Netgear A6200 v2 */
        new(0x2001, 0x330E), /* D-Link - ALPHA */
        new(0x2001, 0x3313), /* D-Link - ALPHA */
        new(0x2001, 0x3315), /* D-Link - Cameo */
        new(0x2001, 0x3316), /* D-Link - Cameo */
        new(0x13B1, 0x003F), /* Linksys - WUSB6300 */
        new(0x2357, 0x0101), /* TP-Link - Archer T4U AC1200 */
        new(0x2357, 0x0103), /* TP-Link - T4UH */
        new(0x2357, 0x010D), /* TP-Link - Archer T4U AC1300 */
        new(0x2357, 0x0115), /* TP-Link - Archer T4U AC1300 */
        new(0x2357, 0x010E), /* TP-Link - Archer T4UH AC1300 */
        new(0x2357, 0x010F), /* TP-Link - T4UHP */
        new(0x2357, 0x0122), /* TP-Link - T4UHP (other) */
        new(0x20F4, 0x805B), /* TRENDnet - */
        new(0x0411, 0x025D), /* Buffalo - WI-U3-866D */
        new(0x050D, 0x1109), /* Belkin F9L1109 - SerComm */
        new(0x148F, 0x9097), /* Amped Wireless ACA1 */
        new(0x0BDA, 0x8812), /* Alfa - AWUS036AC, AWUS036ACH & AWUS036EAC */
        new(0x2604, 0x0012), /* Tenda U12 */
        new(0x0BDA, 0x881A), /* Unex DAUK-W8812 */
    };
}