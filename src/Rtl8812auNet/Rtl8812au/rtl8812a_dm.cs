namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_dm
{
    public static void Init_ODM_ComInfo_8812(hal_com_data pHalData)
    {
        dm_struct pDM_Odm = (pHalData.odmpriv);
        odm_cut_version cut_ver;
        Init_ODM_ComInfo(pHalData);

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

        pDM_Odm.cut_version = cut_ver;
    }

    static void Init_ODM_ComInfo(hal_com_data pHalData)
    {
        dm_struct pDM_Odm = (pHalData.odmpriv);

        {
            /* 1 ======= BoardType: ODM_CMNINFO_BOARD_TYPE ======= */
            uint odm_board_type = ODM_BOARD_DEFAULT;

            if (pHalData.ExternalLNA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA;
            }

            if (pHalData.external_lna_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_LNA_5G;
            }

            if (pHalData.ExternalPA_2G)
            {
                odm_board_type |= ODM_BOARD_EXT_PA;
            }

            if (pHalData.external_pa_5g)
            {
                odm_board_type |= ODM_BOARD_EXT_PA_5G;
            }

            if (pHalData.EEPROMBluetoothCoexist)
                odm_board_type |= ODM_BOARD_BT;

            pDM_Odm.board_type = (u8)odm_board_type;
            /* 1 ============== End of BoardType ============== */
        }
    }
}