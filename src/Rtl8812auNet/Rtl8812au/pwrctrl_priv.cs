namespace Rtl8812auNet.Rtl8812au;

public class pwrctrl_priv
{
    public bool bSupportRemoteWakeup{ get; set; }
    public rt_rf_power_state rf_pwrstate { get; set; }/* cur power state, only for IPS */
}