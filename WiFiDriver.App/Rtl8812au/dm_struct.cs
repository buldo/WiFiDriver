namespace WiFiDriver.App.Rtl8812au;

public class dm_struct
{
    public dm_rf_calibration_struct rf_calibrate_info = new dm_rf_calibration_struct();
    public u8 board_type;
    public byte with_extenal_ant_switch;
    public ulong support_ability { get; set; }
    public byte rf_type { get; set; }
    public byte support_platform { get; set; }
    public byte support_interface { get; set; }
    public uint support_ic_type { get; set; }
    public byte cut_version { get; set; }
    public byte fab_version { get; set; }
    public byte fw_version { get; set; }
    public byte fw_sub_version { get; set; }
    public byte rfe_type { get; set; }
    public byte ant_div_type { get; set; }
    public u8 package_type { get; set; }
    public bool is_init_hw_info_by_rfe { get; set; }
    public u64 rssi_trsw_h { get; set; }
    public u64 rssi_trsw_iso { get; set; }

    public u16 type_glna;
    public u16 type_gpa;
    public u16 type_alna;
    public u16 type_apa;
    public u8 ext_lna;     /*@with 2G external LNA  NO/Yes = 0/1*/
    public u8 ext_lna_5g;      /*@with 5G external LNA  NO/Yes = 0/1*/
    public u8 ext_pa;          /*@with 2G external PNA  NO/Yes = 0/1*/
    public u8 ext_pa_5g;		/*@with 5G external PNA  NO/Yes = 0/1*/
    public u64 rssi_trsw_l;
}