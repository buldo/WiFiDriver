using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
static class libusb_speed
{
    public const byte USB_SPEED_UNKNOWN = 0;
    public const byte USB_SPEED_LOW = 1;
    public const byte USB_SPEED_FULL = 2;
    public const byte USB_SPEED_HIGH = 3;
    public const byte USB_SPEED_SUPER = 4;
};