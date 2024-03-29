﻿using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.PredefinedData;

public static class PowerIndexDescription
{
    public static Dictionary<RfPath, Dictionary<MGN_RATE, (ushort RegAddress, uint BitMask)>> SetTable { get; } =
        new()
        {
            {
                RfPath.RF_PATH_A,
                new()
                {
                    { MGN_RATE.MGN_1M, (rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_2M, (rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_5_5M, (rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_11M, (rTxAGC_A_CCK11_CCK1_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_6M, (rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_9M, (rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_12M, (rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_18M, (rTxAGC_A_Ofdm18_Ofdm6_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_24M, (rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_36M, (rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_48M, (rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_54M, (rTxAGC_A_Ofdm54_Ofdm24_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS0, (rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS1, (rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS2, (rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS3, (rTxAGC_A_MCS3_MCS0_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS4, (rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS5, (rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS6, (rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS7, (rTxAGC_A_MCS7_MCS4_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS8, (rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS9, (rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS10, (rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS11, (rTxAGC_A_MCS11_MCS8_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS12, (rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS13, (rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS14, (rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS15, (rTxAGC_A_MCS15_MCS12_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS0, (rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS1, (rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT1SS_MCS2, (rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT1SS_MCS3, (rTxAGC_A_Nss1Index3_Nss1Index0_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS4, (rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS5, (rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT1SS_MCS6, (rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT1SS_MCS7, (rTxAGC_A_Nss1Index7_Nss1Index4_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS8, (rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS9, (rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS0, (rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS1, (rTxAGC_A_Nss2Index1_Nss1Index8_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT2SS_MCS2, (rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT2SS_MCS3, (rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS4, (rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS5, (rTxAGC_A_Nss2Index5_Nss2Index2_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT2SS_MCS6, (rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT2SS_MCS7, (rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS8, (rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS9, (rTxAGC_A_Nss2Index9_Nss2Index6_JAguar, bMaskByte3) },
                }
            },
            {
                RfPath.RF_PATH_B,
                new()
                {
                    { MGN_RATE.MGN_1M, (rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_2M, (rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_5_5M, (rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_11M, (rTxAGC_B_CCK11_CCK1_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_6M, (rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_9M, (rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_12M, (rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_18M, (rTxAGC_B_Ofdm18_Ofdm6_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_24M, (rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_36M, (rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_48M, (rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_54M, (rTxAGC_B_Ofdm54_Ofdm24_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS0, (rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS1, (rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS2, (rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS3, (rTxAGC_B_MCS3_MCS0_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS4, (rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS5, (rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS6, (rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS7, (rTxAGC_B_MCS7_MCS4_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS8, (rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS9, (rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS10, (rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS11, (rTxAGC_B_MCS11_MCS8_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_MCS12, (rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_MCS13, (rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_MCS14, (rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_MCS15, (rTxAGC_B_MCS15_MCS12_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS0, (rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS1, (rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT1SS_MCS2, (rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT1SS_MCS3, (rTxAGC_B_Nss1Index3_Nss1Index0_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS4, (rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS5, (rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT1SS_MCS6, (rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT1SS_MCS7, (rTxAGC_B_Nss1Index7_Nss1Index4_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT1SS_MCS8, (rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT1SS_MCS9, (rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS0, (rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS1, (rTxAGC_B_Nss2Index1_Nss1Index8_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT2SS_MCS2, (rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT2SS_MCS3, (rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS4, (rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS5, (rTxAGC_B_Nss2Index5_Nss2Index2_JAguar, bMaskByte3) },
                    { MGN_RATE.MGN_VHT2SS_MCS6, (rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte0) },
                    { MGN_RATE.MGN_VHT2SS_MCS7, (rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte1) },
                    { MGN_RATE.MGN_VHT2SS_MCS8, (rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte2) },
                    { MGN_RATE.MGN_VHT2SS_MCS9, (rTxAGC_B_Nss2Index9_Nss2Index6_JAguar, bMaskByte3) },
                }
            },
        };
}