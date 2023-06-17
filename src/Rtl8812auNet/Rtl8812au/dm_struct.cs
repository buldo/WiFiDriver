namespace Rtl8812auNet.Rtl8812au;

public class dm_struct
{
    public dm_rf_calibration_struct rf_calibrate_info = new dm_rf_calibration_struct();
    public u8 board_type { get; set; }
    public byte support_platform { get; set; }
    public byte support_interface { get; set; }
    public odm_cut_version cut_version { get; set; }
    public u8 package_type { get; set; }
    public bool is_init_hw_info_by_rfe { get; set; }

    public u16 type_glna { get; set; }
    public u16 type_gpa { get; set; }
    public u16 type_alna { get; set; }
    public u16 type_apa { get; set; }
}