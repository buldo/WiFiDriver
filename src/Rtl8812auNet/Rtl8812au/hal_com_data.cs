namespace Rtl8812auNet.Rtl8812au;

public class hal_com_data
{
    public HAL_VERSION version_id { get; } = new HAL_VERSION();
    public u16 firmware_version { get; set; }
    public u16 firmware_sub_version { get; set; }
    public u16 FirmwareSignature { get; set; }
    public u8 current_channel { get; set; }
    public RfType rf_type { get; set; } /*enum RfType*/
    public u8 NumTotalRFPath { get; set; }
    public bool AutoloadFailFlag { get; set; }
    public RT_MULTI_FUNC MultiFunc { get; set; }
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

    //    /*---------------------------------------------------------------------------------*/
    //    /* 2.4G TX power info for target TX power*/
    public u8[,] Index24G_CCK_Base { get; } = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] Index24G_BW40_Base { get; } = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] CCK_24G_Diff { get; } = new byte[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] OFDM_24G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_24G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_24G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    //	/* 5G TX power info for target TX power*/
    //#ifdef CONFIG_IEEE80211_BAND_5GHZ
    public u8[,] Index5G_BW40_Base { get; } = new byte[MAX_RF_PATH, CENTER_CH_5G_ALL_NUM];
    public u8[,] Index5G_BW80_Base { get; } = new byte[MAX_RF_PATH, CENTER_CH_5G_80M_NUM];
    public s8[,] OFDM_5G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_5G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_5G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    public s8[,] BW80_5G_Diff { get; } = new s8[MAX_RF_PATH, MAX_TX_COUNT];

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

    public Dictionary<RfPath, BbRegisterDefinition> PHYRegDef { get; } = new()
    {
        { RfPath.RF_PATH_A, new BbRegisterDefinition() },
        { RfPath.RF_PATH_B, new BbRegisterDefinition() }
    }; /* Radio A/B/C/D */

    public dm_struct odmpriv { get; } = new dm_struct();

    public bool bMacPwrCtrlOn { get; set; }

    public TxSele OutEpQueueSel { get; set; }
    public u8 OutEpNumber { get; set; }

    public RX_AGG_MODE rxagg_mode { get; set; }

    public u8 rxagg_dma_size { get; set; }

    public u8 rxagg_dma_timeout { get; set; }

    public u32[] IntrMask { get; } = new u32[3];

    public bool UsbTxAggMode { get; set; }
    public u8 UsbTxAggDescNum { get; set; }

    public u8 rxagg_usb_size { get; set; }

    public u8 rxagg_usb_timeout { get; set; }

    public hal_spec_t hal_spec { get; } = new();

    public BandType current_band_type { get; set; }
}