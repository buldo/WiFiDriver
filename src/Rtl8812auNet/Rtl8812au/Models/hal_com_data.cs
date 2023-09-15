using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class hal_com_data
{
    public hal_com_data(
        HalVersion version,
        RfType rfType,
        byte numTotalRfPath)
    {
        Version = version;
        RfType = rfType;
        NumTotalRFPath = numTotalRfPath;
    }

    public HalVersion Version { get; }

    /// <summary>
    /// It strange but this value has 2 source of truth
    /// </summary>
    public RfType RfType { get; set; } /*enum RfType*/

    /// <summary>
    /// It strange but this value has 2 source of truth
    /// </summary>
    public u8 NumTotalRFPath { get; set; }
    public bool AutoloadFailFlag { get; set; }
    public bool EepromOrEfuse { get; init; }
    public u8[] efuse_eeprom_data { get; } = new u8[1024]; /*92C:256bytes, 88E:512bytes, we use union set (512bytes)*/

    public u8 EEPROMVersion { get; set; }
    public u8 EEPROMRegulatory { get; set; }
    public u8 eeprom_thermal_meter { get; set; }
    public bool EEPROMBluetoothCoexist { get; set; }

    public u8 crystal_cap { get; set; }

    public u8 PAType_2G { get; set; }
    public u8 PAType_5G { get; set; }
    public u8 LNAType_2G { get; set; }
    public u8 LNAType_5G { get; set; }
    public bool ExternalPA_2G { get; set; }
    public bool ExternalLNA_2G { get; set; }
    public bool external_pa_5g { get; set; }
    public bool external_lna_5g { get; set; }
    public u16 TypeGLNA { get; set; }
    public u16 TypeGPA { get; set; }
    public u16 TypeALNA { get; set; }

    public u16 TypeAPA { get; set; }
    public u16 rfe_type { get; set; }

    public TxSele OutEpQueueSel { get; init; }
    public u8 OutEpNumber { get; init; }

    public u8 rxagg_usb_size { get; init; }

    public u8 rxagg_usb_timeout { get; init; }

    public BandType current_band_type { get; set; }

    public u8 GetBoardType()
    {
        /* 1 ======= BoardType: ODM_CMNINFO_BOARD_TYPE ======= */
        uint odm_board_type = ODM_BOARD_DEFAULT;

        if (ExternalLNA_2G)
        {
            odm_board_type |= ODM_BOARD_EXT_LNA;
        }

        if (external_lna_5g)
        {
            odm_board_type |= ODM_BOARD_EXT_LNA_5G;
        }

        if (ExternalPA_2G)
        {
            odm_board_type |= ODM_BOARD_EXT_PA;
        }

        if (external_pa_5g)
        {
            odm_board_type |= ODM_BOARD_EXT_PA_5G;
        }

        if (EEPROMBluetoothCoexist)
        {
            odm_board_type |= ODM_BOARD_BT;
        }

        return (u8)odm_board_type;
    }

}