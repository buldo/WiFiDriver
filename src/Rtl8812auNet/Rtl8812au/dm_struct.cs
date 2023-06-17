namespace Rtl8812auNet.Rtl8812au;

public class dm_struct
{
    public u8 board_type { get; set; }
    public byte support_platform { get; set; }
    public odm_cut_version cut_version { get; set; }
    public u8 package_type { get; set; }
    public u16 type_glna { get; set; }
    public u16 type_gpa { get; set; }
    public u16 type_alna { get; set; }
    public u16 type_apa { get; set; }
}