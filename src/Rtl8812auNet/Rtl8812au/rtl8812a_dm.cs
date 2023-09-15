using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_dm
{
    public static u8 Init_ODM_ComInfo_8812(hal_com_data pHalData)
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

        return (u8)odm_board_type;
    }
}