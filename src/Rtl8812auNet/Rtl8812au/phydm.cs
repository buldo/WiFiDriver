namespace Rtl8812auNet.Rtl8812au;

public static class phydm
{
    public static void odm_cmn_info_init(dm_struct dm, odm_cmninfo cmn_info, u64 value)
    {
        switch (cmn_info)
        {
            case odm_cmninfo.ODM_CMNINFO_ABILITY:
                dm.support_ability = (u64)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_RF_TYPE:
                dm.rf_type = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_PLATFORM:
                dm.support_platform = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_INTERFACE:
                dm.support_interface = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_IC_TYPE:
                dm.support_ic_type = (u32)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_CUT_VER:
                dm.cut_version = (odm_cut_version)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_FAB_VER:
                dm.fab_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_FW_VER:
                dm.fw_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_FW_SUB_VER:
                dm.fw_sub_version = (u8)value;
                break;
            case odm_cmninfo.ODM_CMNINFO_RFE_TYPE:
                dm.rfe_type = (u8)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_BOARD_TYPE:
                if (!dm.is_init_hw_info_by_rfe)
                {
                    dm.board_type = (u8)value;
                    Console.WriteLine($"BoardType = 0x{dm.board_type:X4}");
                }
                break;

            case odm_cmninfo.ODM_CMNINFO_PACKAGE_TYPE:
                if (!dm.is_init_hw_info_by_rfe)
                {
                    dm.package_type = (u8) value;
                }
            	break;

            case odm_cmninfo.ODM_CMNINFO_GPA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_gpa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_APA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_apa = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_GLNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_glna = (u16)value;
                break;

            case odm_cmninfo.ODM_CMNINFO_ALNA:
                if (!dm.is_init_hw_info_by_rfe)
                    dm.type_alna = (u16)value;
                break;
            default:
                //throw new NotImplementedException();
                break;
        }
    }
}