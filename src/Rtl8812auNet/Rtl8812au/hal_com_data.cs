namespace Rtl8812auNet.Rtl8812au;

public class hal_com_data
{
    public HAL_VERSION version_id = new HAL_VERSION();
    public u16 firmware_version;
    public u16 firmware_sub_version;
    public u16 FirmwareSignature;
    public u8 current_channel;
    public RF_CHIP_E rf_chip = RF_CHIP_E.RF_6052;
    public rf_type rf_type; /*enum rf_type*/
    public u8 NumTotalRFPath;
    public bool bautoload_fail_flag { get; set; }
    public RT_MULTI_FUNC MultiFunc { get; set; }
    public channel_width current_channel_bw { get; set; }
    public bool bSwChnl { get; set; }
    public bool bChnlBWInitialized { get; set; }
    public byte nCur40MhzPrimeSC { get; set; }
    public byte nCur80MhzPrimeSC { get; set; }
    public bool bSetChnlBW { get; set; }
    public byte CurrentCenterFrequencyIndex1 { get; set; }
    public bool bNeedIQK { get; set; }
    public bool AMPDUBurstMode { get; set; }

    public bool EepromOrEfuse;
    public u8[] efuse_eeprom_data = new u8[1024]; /*92C:256bytes, 88E:512bytes, we use union set (512bytes)*/

    public u8 InterfaceSel; /* board type kept in eFuse */

    public bool EEPROMUsbSwitch;

    public u8 EEPROMVersion;
    public u8 EEPROMRegulatory;
    public u8 eeprom_thermal_meter;
    public bool EEPROMBluetoothCoexist;

    //    /*---------------------------------------------------------------------------------*/
    //    /* 2.4G TX power info for target TX power*/
    public u8[,] Index24G_CCK_Base = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] Index24G_BW40_Base = new byte[MAX_RF_PATH, CENTER_CH_2G_NUM];
    public u8[,] CCK_24G_Diff = new byte[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] OFDM_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_24G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    //	/* 5G TX power info for target TX power*/
    //#ifdef CONFIG_IEEE80211_BAND_5GHZ
    public u8[,] Index5G_BW40_Base = new byte[MAX_RF_PATH, CENTER_CH_5G_ALL_NUM];
    public u8[,] Index5G_BW80_Base = new byte[MAX_RF_PATH, CENTER_CH_5G_80M_NUM];
    public s8[,] OFDM_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW20_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];
    public s8[,] BW40_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    public s8[,] BW80_5G_Diff = new s8[MAX_RF_PATH, MAX_TX_COUNT];

    public u8 crystal_cap { get; set; }

    public u8 PAType_2G;
    public u8 PAType_5G;
    public u8 LNAType_2G;
    public u8 LNAType_5G;
    public bool ExternalPA_2G;
    public bool ExternalLNA_2G;
    public bool external_pa_5g;
    public bool external_lna_5g;
    public u16 TypeGLNA;
    public u16 TypeGPA;
    public u16 TypeALNA;

    public u16 TypeAPA;
    public u16 rfe_type;

    public Dictionary<rf_path, BB_REGISTER_DEFINITION_T> PHYRegDef = new()
    {
        { rf_path.RF_PATH_A, new BB_REGISTER_DEFINITION_T() },
        { rf_path.RF_PATH_B, new BB_REGISTER_DEFINITION_T() }
    }; /* Radio A/B/C/D */

    public dm_struct odmpriv = new dm_struct();

    public bool bMacPwrCtrlOn;

    public TxSele OutEpQueueSel;
    public u8 OutEpNumber;

    public RX_AGG_MODE rxagg_mode;

    public u8 rxagg_dma_size;

    public u8 rxagg_dma_timeout;

    public u32 UsbBulkOutSize;

    public u32[] IntrMask = new u32[3];

    public bool UsbTxAggMode;
    public u8 UsbTxAggDescNum;

    public u8 rxagg_usb_size;

    public u8 rxagg_usb_timeout;

    public hal_spec_t hal_spec = new();

    public BAND_TYPE current_band_type;
}