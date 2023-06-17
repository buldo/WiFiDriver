namespace Rtl8812auNet.Rtl8812au;

public class dm_rf_calibration_struct
{
    public bool is_apk_thermal_meter_ignore;
    public bool is_iqk_in_progress;
    public int default_ofdm_index;
    public sbyte bb_swing_diff_2g { get; set; }
    public sbyte bb_swing_diff_5g { get; set; }
}