namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_dm
{
    public static void Init_ODM_ComInfo_8812(AdapterState adapterState)
    {
        var pHalData = adapterState.HalData;

        dm_struct pDM_Odm = (pHalData.odmpriv);
        odm_cut_version cut_ver;
        odm_fab fab_ver;

        Init_ODM_ComInfo(adapterState);

        // TODO: WTF no mapping
        fab_ver = odm_fab.ODM_TSMC;
        if (IS_A_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_A;
        }
        else if (IS_B_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_B;
        }
        else if (IS_C_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_C;
        }
        else if (IS_D_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_D;
        }
        else if (IS_E_CUT(pHalData.version_id))
        {
            cut_ver = odm_cut_version.ODM_CUT_E;
        }
        else
        {
            cut_ver = odm_cut_version.ODM_CUT_A;
        }

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_FAB_VER, (ulong)fab_ver);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_CUT_VER, (ulong)cut_ver);
    }

    static void Init_ODM_ComInfo(AdapterState adapterState)
    {
        var pHalData = adapterState.HalData;
        dm_struct pDM_Odm = (pHalData.odmpriv);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RF_TYPE, (ulong)pHalData.rf_type);
        {
            /* 1 ======= BoardType: ODM_CMNINFO_BOARD_TYPE ======= */
            uint odm_board_type = ODM_BOARD_DEFAULT;

            if (pHalData.ExternalLNA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_LNA, 1);
            }

            if (pHalData.external_lna_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA_5G;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_5G_EXT_LNA, 1);
            }

            if (pHalData.ExternalPA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_PA;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_PA, 1);
            }

            if (pHalData.external_pa_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_PA_5G;
                odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_5G_EXT_PA, 1);
            }

            if (pHalData.EEPROMBluetoothCoexist)
                odm_board_type |= ODM_BOARD_BT;

            odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_BOARD_TYPE, odm_board_type);
            /* 1 ============== End of BoardType ============== */
        }

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_GPA, pHalData.TypeGPA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_APA, pHalData.TypeAPA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_GLNA, pHalData.TypeGLNA);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_ALNA, pHalData.TypeALNA);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_RFE_TYPE, pHalData.rfe_type);
        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_X_CAP_SETTING, pHalData.crystal_cap);

        odm_cmn_info_init(pDM_Odm, odm_cmninfo.ODM_CMNINFO_EXT_TRSW, 0);
    }
}