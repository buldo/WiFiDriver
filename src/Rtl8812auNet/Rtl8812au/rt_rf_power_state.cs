namespace Rtl8812auNet.Rtl8812au;

public enum rt_rf_power_state
{
    rf_on,      /* RF is on after RFSleep or RFOff */
    rf_sleep,   /* 802.11 Power Save mode */
    rf_off,     /* HW/SW Radio OFF or Inactive Power Save */
    /* =====Add the new RF state above this line===== */
    rf_max
}
;