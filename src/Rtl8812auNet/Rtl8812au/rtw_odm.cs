namespace Rtl8812auNet.Rtl8812au;

public static class rtw_odm
{
    static u32[] _chip_type_to_odm_ic_type = {
        0,
        0,
        0,
        (u32)phydm_ic.ODM_RTL8812,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
    };

    /* set ODM_CMNINFO_IC_TYPE based on chip_type */
    public static void rtw_odm_init_ic_type(AdapterState adapterState)
    {
        dm_struct odm = adapter_to_phydm(adapterState);
        UInt32 ic_type = _chip_type_to_odm_ic_type[3];
        odm_cmn_info_init(odm, odm_cmninfo.ODM_CMNINFO_IC_TYPE, ic_type);
    }

    public static dm_struct adapter_to_phydm(AdapterState adapterState) => (GET_HAL_DATA(adapterState).odmpriv);
}