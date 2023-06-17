namespace Rtl8812auNet.Rtl8812au;

[Flags]
public enum RT_MULTI_FUNC
{
    RT_MULTI_FUNC_NONE = 0x00,
    RT_MULTI_FUNC_WIFI = 0x01,
    RT_MULTI_FUNC_BT = 0x02,
    RT_MULTI_FUNC_GPS = 0x04,
}