namespace Rtl8812auNet.Rtl8812au;

public class hal_com_data
{
    public hal_com_data(
        HAL_VERSION versionId,
        RfType rfType,
        byte numTotalRfPath)
    {
        version_id = versionId;
        rf_type = rfType;
        NumTotalRFPath = numTotalRfPath;
    }

    public HAL_VERSION version_id { get; }
    public u8 current_channel { get; set; }

    /// <summary>
    /// It strange but this value has 2 source of truth
    /// </summary>
    public RfType rf_type { get; set; } /*enum RfType*/

    /// <summary>
    /// It strange but this value has 2 source of truth
    /// </summary>
    public u8 NumTotalRFPath { get; set; }
    public bool AutoloadFailFlag { get; set; }
    public ChannelWidth current_channel_bw { get; set; }
    public bool bSwChnl { get; set; }
    public bool bChnlBWInitialized { get; set; }
    public byte nCur40MhzPrimeSC { get; set; }
    public byte nCur80MhzPrimeSC { get; set; }
    public bool bSetChnlBW { get; set; }
    public byte CurrentCenterFrequencyIndex1 { get; set; }
    public bool bNeedIQK { get; set; }
    public bool EepromOrEfuse { get; set; }
    public u8[] efuse_eeprom_data { get; } = new u8[1024]; /*92C:256bytes, 88E:512bytes, we use union set (512bytes)*/

    public u8 InterfaceSel { get; set; } /* board type kept in eFuse */

    public bool EEPROMUsbSwitch { get; set; }

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

    public dm_struct odmpriv { get; } = new dm_struct();

    public TxSele OutEpQueueSel { get; init; }
    public u8 OutEpNumber { get; init; }

    public u32[] IntrMask { get; } = new u32[3];

    public u8 rxagg_usb_size { get; init; }

    public u8 rxagg_usb_timeout { get; init; }

    public BandType current_band_type { get; set; }
}