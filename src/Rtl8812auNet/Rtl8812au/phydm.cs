namespace Rtl8812auNet.Rtl8812au;

public static class phydm
{
    public static void odm_cmn_info_init(dm_struct dm, odm_cmninfo cmn_info, u64 value)
    {
        switch (cmn_info)
        {
            case odm_cmninfo.ODM_CMNINFO_CUT_VER:
                dm.cut_version = (odm_cut_version)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_BOARD_TYPE:
                dm.board_type = (u8)value;
                Console.WriteLine($"BoardType = 0x{dm.board_type:X4}");
                break;

            case odm_cmninfo.ODM_CMNINFO_PACKAGE_TYPE:
                dm.package_type = (u8) value;
            	break;

            case odm_cmninfo.ODM_CMNINFO_GPA:
                dm.type_gpa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_APA:
                dm.type_apa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_GLNA:
                dm.type_glna = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_ALNA:
                dm.type_alna = (u16)value;
                break;
            default:
                //throw new NotImplementedException();
                break;
        }
    }
}