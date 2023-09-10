namespace Rtl8812auNet.Rtl8812au;

public static class rtl8812a_phycfg
{
    public static void PHY_SetSwChnlBWMode8812(
        AdapterState adapterState,
        u8 channel,
        ChannelWidth Bandwidth,
        u8 Offset40,
        u8 Offset80)
    {
        PHY_HandleSwChnlAndSetBW8812(adapterState, true, true, channel, Bandwidth, Offset40, Offset80, channel);
    }

    public static u32 PHY_CalculateBitShift(u32 BitMask)
    {
        int i;

        for (i = 0; i <= 31; i++)
        {
            if (((BitMask >> i) & 0x1) == 1)
                break;
        }

        return (u32)i;
    }

    static void PHY_HandleSwChnlAndSetBW8812(
        AdapterState adapterState,
        BOOLEAN bSwitchChannel,
        BOOLEAN bSetBandWidth,
        u8 ChannelNum,
        ChannelWidth ChnlWidth,
        u8 ChnlOffsetOf40MHz,
        u8 ChnlOffsetOf80MHz,
        u8 CenterFrequencyIndex1
    )
    {
        var pHalData = adapterState.HalData;

        /* RTW_INFO("=> PHY_HandleSwChnlAndSetBW8812: bSwitchChannel %d, bSetBandWidth %d\n",bSwitchChannel,bSetBandWidth); */

        /* check is swchnl or setbw */
        if (!bSwitchChannel && !bSetBandWidth)
        {
            RTW_INFO("PHY_HandleSwChnlAndSetBW8812:  not switch channel and not set bandwidth\n");
            return;
        }

        /* skip change for channel or bandwidth is the same */
        if (bSwitchChannel)
        {
            if (pHalData.current_channel != ChannelNum)
            {
                pHalData.bSwChnl = true;
            }
        }

        if (bSetBandWidth)
        {
            if (pHalData.bChnlBWInitialized == false)
            {
                pHalData.bChnlBWInitialized = true;
                pHalData.bSetChnlBW = true;
            }
            else if ((pHalData.current_channel_bw != ChnlWidth) ||
                     (pHalData.nCur40MhzPrimeSC != ChnlOffsetOf40MHz) ||
                     (pHalData.nCur80MhzPrimeSC != ChnlOffsetOf80MHz) ||
                     (pHalData.CurrentCenterFrequencyIndex1 != CenterFrequencyIndex1))
            {
                pHalData.bSetChnlBW = true;
            }
        }

        if (!pHalData.bSetChnlBW && !pHalData.bSwChnl && pHalData.bNeedIQK != true)
        {
            /* RTW_INFO("<= PHY_HandleSwChnlAndSetBW8812: bSwChnl %d, bSetChnlBW %d\n",pHalData.bSwChnl,pHalData.bSetChnlBW); */
            return;
        }


        if (pHalData.bSwChnl)
        {
            pHalData.current_channel = ChannelNum;
            pHalData.CurrentCenterFrequencyIndex1 = ChannelNum;
        }


        if (pHalData.bSetChnlBW)
        {
            pHalData.current_channel_bw = ChnlWidth;

            pHalData.nCur40MhzPrimeSC = ChnlOffsetOf40MHz;
            pHalData.nCur80MhzPrimeSC = ChnlOffsetOf80MHz;


            pHalData.CurrentCenterFrequencyIndex1 = CenterFrequencyIndex1;
        }

        /* Switch workitem or set timer to do switch channel or setbandwidth operation */
        phy_SwChnlAndSetBwMode8812(adapterState);
    }

    static void phy_SwChnlAndSetBwMode8812(AdapterState adapterState)
    {
        var pHalData = adapterState.HalData;

        if (pHalData.bSwChnl)
        {
            phy_SwChnl8812(adapterState);
            pHalData.bSwChnl = false;
        }

        if (pHalData.bSetChnlBW)
        {
            phy_PostSetBwMode8812(adapterState);
            pHalData.bSetChnlBW = false;
        }

        PHY_SetTxPowerLevel8812(adapterState, pHalData.current_channel);

        pHalData.bNeedIQK = false;
    }

    static void PHY_SetTxPowerLevel8812(AdapterState adapterState, u8 Channel)
    {
        var pHalData = adapterState.HalData;

        for (var path = (u8)RfPath.RF_PATH_A; (byte)path < pHalData.NumTotalRFPath; ++path)
        {
            phy_set_tx_power_level_by_path(adapterState, Channel, (RfPath)path);
            PHY_TxPowerTrainingByPath_8812(adapterState, (RfPath)path);
        }
    }

    static void PHY_TxPowerTrainingByPath_8812(AdapterState adapterState, RfPath rfPath)
    {
        var pHalData = adapterState.HalData;
        if ((u8)rfPath >= pHalData.NumTotalRFPath)
        {
            return;
        }

        u16 writeOffset;
        u32 powerLevel;
        if (rfPath == RfPath.RF_PATH_A)
        {
            powerLevel = phy_get_tx_power_index();
            writeOffset = rA_TxPwrTraing_Jaguar;
        }
        else
        {
            powerLevel = phy_get_tx_power_index();
            writeOffset = rB_TxPwrTraing_Jaguar;
        }

        u32 writeData = 0;
        for (u8 i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                powerLevel = powerLevel - 10;
            }
            else if (i == 1)
            {
                powerLevel = powerLevel - 8;
            }
            else
            {
                powerLevel = powerLevel - 6;
            }
            writeData |= (((powerLevel > 2) ? (powerLevel) : 2) << (i * 8));
        }

        adapterState.Device.phy_set_bb_reg(writeOffset, 0xffffff, writeData);
    }

    static u8 phy_get_tx_power_index()
    {
        return 16;
    }


    static void phy_set_tx_power_level_by_path(AdapterState adapterState, u8 channel, RfPath path)
    {
        var pHalData = adapterState.HalData;
        BOOLEAN bIsIn24G = (pHalData.current_band_type == BandType.BAND_ON_2_4G);

        if (bIsIn24G)
        {
            phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.CCK);
        }

        phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.OFDM);

        phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.HT_MCS0_MCS7);
        phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.VHT_1SSMCS0_1SSMCS9);

        if (pHalData.NumTotalRFPath >= 2)
        {
            phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.HT_MCS8_MCS15);
            phy_set_tx_power_index_by_rate_section(adapterState, path, channel, RATE_SECTION.VHT_2SSMCS0_2SSMCS9);
        }
    }

    static void phy_set_tx_power_index_by_rate_section(
        AdapterState pAdapterState,
        RfPath RFPath,
        u8 Channel,
        RATE_SECTION RateSection)
    {
        Console.WriteLine($"SET_TX_POWER {RFPath}; {Channel}; {RateSection}");
        var pHalData = pAdapterState.HalData;

        if (RateSection >= RATE_SECTION.RATE_SECTION_NUM)
        {
            throw new Exception("RateSection >= RATE_SECTION.RATE_SECTION_NUM");
        }

        if (RateSection == RATE_SECTION.CCK && pHalData.current_band_type != BandType.BAND_ON_2_4G)
            return;

        PHY_SetTxPowerIndexByRateArray(
            pAdapterState,
            RFPath,
            rates_by_sections[(int)RateSection].rates);
    }

    static void PHY_SetTxPowerIndexByRateArray(
        AdapterState pAdapterState,
        RfPath RFPath,
        MGN_RATE[] Rates)
    {
        for (int i = 0; i < Rates.Length; ++i)
        {
            var powerIndex = phy_get_tx_power_index();
            MGN_RATE rate = Rates[i];
            PHY_SetTxPowerIndex_8812A(pAdapterState, powerIndex, RFPath, rate);
        }
    }

    static void phy_SwChnl8812(AdapterState pAdapterState)
    {

        var pHalData = pAdapterState.HalData;
        u8 channelToSW = pHalData.current_channel;

        if (phy_SwBand8812(pAdapterState, channelToSW) == false)
        {
            RTW_INFO("error Chnl %d !", channelToSW);
        }

        /* RTW_INFO("[BW:CHNL], phy_SwChnl8812(), switch to channel %d !!\n", channelToSW); */

        /* fc_area		 */
        if (36 <= channelToSW && channelToSW <= 48)
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (15 <= channelToSW && channelToSW <= 35)
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x494);
        }
        else if (50 <= channelToSW && channelToSW <= 80)
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x453);
        }
        else if (82 <= channelToSW && channelToSW <= 116)
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x452);
        }
        else if (118 <= channelToSW)
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x412);
        }
        else
        {
            pAdapterState.Device.phy_set_bb_reg(rFc_area_Jaguar, 0x1ffe0000, 0x96a);
        }

        for (RfPath eRFPath = 0; (byte)eRFPath < pHalData.NumTotalRFPath; eRFPath++)
        {
            /* RF_MOD_AG */
            if (36 <= channelToSW && channelToSW <= 80)
            {
                phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (15 <= channelToSW && channelToSW <= 35)
            {
                phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x101); /* 5'b00101); */
            }
            else if (82 <= channelToSW && channelToSW <= 140)
            {
                phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x301); /* 5'b01101); */
            }
            else if (140 < channelToSW)
            {
                phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x501); /* 5'b10101); */
            }
            else
            {
                phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, BIT18 | BIT17 | BIT16 | BIT9 | BIT8,
                    0x000); /* 5'b00000); */
            }

            /* <20121109, Kordan> A workaround for 8812A only. */
            phy_FixSpur_8812A(pAdapterState, pHalData.current_channel_bw, channelToSW);
            phy_set_rf_reg(pAdapterState, eRFPath, RF_CHNLBW_Jaguar, bMaskByte0, channelToSW);
        }
    }

    static BOOLEAN phy_SwBand8812(AdapterState pAdapterState, u8 channelToSW)
    {
        u8 u1Btmp;
        BOOLEAN ret_value = true;
        BandType Band;
        BandType BandToSW;

        u1Btmp = pAdapterState.Device.rtw_read8(REG_CCK_CHECK_8812);
        if ((u1Btmp & BIT7) != 0)
        {
            Band = BandType.BAND_ON_5G;
        }
        else
        {
            Band = BandType.BAND_ON_2_4G;
        }

        /* Use current channel to judge Band Type and switch Band if need. */
        if (channelToSW > 14)
        {
            BandToSW = BandType.BAND_ON_5G;
        }
        else
            BandToSW = BandType.BAND_ON_2_4G;

        if (BandToSW != Band)
        {
            PHY_SwitchWirelessBand8812(pAdapterState, BandToSW);
        }

        return ret_value;
    }

    public static void phy_set_rf_reg(AdapterState adapterState, RfPath eRFPath, u16 RegAddr, u32 BitMask, u32 Data)
    {
        uint data = Data;
        Console.WriteLine($"RFREG;{(byte)eRFPath};{(uint)RegAddr:X};{BitMask:X};{data:X}");
        if (BitMask == 0)
        {
            return;
        }

        /* RF data is 20 bits only */
        if (BitMask != bLSSIWrite_data_Jaguar)
        {
            u32 Original_Value, BitShift;
            Original_Value = phy_RFSerialRead(adapterState, eRFPath, RegAddr);
            BitShift = PHY_CalculateBitShift(BitMask);
            data = ((Original_Value) & (~BitMask)) | (data << (int)BitShift);
        }

        phy_RFSerialWrite(adapterState, eRFPath, RegAddr, data);
    }

    static u32 phy_RFSerialRead(AdapterState adapterState, RfPath eRFPath, u32 Offset)
    {
        u32 retValue;
        var pHalData = adapterState.HalData;
        BbRegisterDefinition pPhyReg = pHalData.PHYRegDef[eRFPath];
        BOOLEAN bIsPIMode = false;



        /* <20120809, Kordan> CCA OFF(when entering), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(adapterState)))
            adapterState.Device.phy_set_bb_reg(rCCAonSec_Jaguar, 0x8, 1);

        Offset &= 0xff;

        if (eRFPath == RfPath.RF_PATH_A)
        {
            bIsPIMode = phy_query_bb_reg(adapterState, 0xC00, 0x4) != 0;
        }
        else if (eRFPath == RfPath.RF_PATH_B)
        {
            bIsPIMode = phy_query_bb_reg(adapterState, 0xE00, 0x4) != 0;
        }

        adapterState.Device.phy_set_bb_reg((ushort)pPhyReg.RfHSSIPara2, bHSSIRead_addr_Jaguar, Offset);

        if (IS_VENDOR_8812A_C_CUT(adapterState))
        {
            Thread.Sleep(20);
        }

        if (bIsPIMode)
        {
            retValue = phy_query_bb_reg(adapterState, pPhyReg.RfLSSIReadBackPi, rRead_data_Jaguar);
            /* RTW_INFO("[PI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.rfLSSIReadBackPi, retValue); */
        }
        else
        {
            retValue = phy_query_bb_reg(adapterState, pPhyReg.RfLSSIReadBack, rRead_data_Jaguar);
            /* RTW_INFO("[SI mode] RFR-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.RfLSSIReadBack, retValue); */
        }

        /* <20120809, Kordan> CCA ON(when exiting), asked by James to avoid reading the wrong value. */
        /* <20120828, Kordan> Toggling CCA would affect RF 0x0, skip it! */
        if (Offset != 0x0 && !(IS_VENDOR_8812A_C_CUT(adapterState)))
        {
            adapterState.Device.phy_set_bb_reg(rCCAonSec_Jaguar, 0x8, 0);
        }


        return retValue;
    }

    private static u32 phy_query_bb_reg(AdapterState adapterState, u16 RegAddr, u32 BitMask) =>
        PHY_QueryBBReg8812(adapterState, RegAddr, BitMask);

    private static u32 PHY_QueryBBReg8812(AdapterState adapterState,u16         RegAddr,u32         BitMask)
    {
        u32 ReturnValue, OriginalValue, BitShift;

        /* RTW_INFO("--.PHY_QueryBBReg8812(): RegAddr(%#x), BitMask(%#x)\n", RegAddr, BitMask); */

        OriginalValue = adapterState.Device.rtw_read32(RegAddr);
        BitShift = PHY_CalculateBitShift(BitMask);
        ReturnValue = (OriginalValue & BitMask) >> (int)BitShift;

        /* RTW_INFO("BBR MASK=0x%x Addr[0x%x]=0x%x\n", BitMask, RegAddr, OriginalValue); */
        return ReturnValue;
    }


    static void phy_RFSerialWrite(AdapterState adapterState, RfPath eRFPath, u32 Offset, u32 Data)
    {
        var pHalData = adapterState.HalData;
        BbRegisterDefinition pPhyReg = pHalData.PHYRegDef[eRFPath];

        Offset &= 0xff;
        /* Shadow Update */
        /* PHY_RFShadowWrite(adapterState, eRFPath, Offset, Data); */
        /* Put write addr in [27:20]  and write data in [19:00] */
        var dataAndAddr = ((Offset << 20) | (Data & 0x000fffff)) & 0x0fffffff;

        /* Write Operation */
        /* TODO: Dynamically determine whether using PI or SI to write RF registers. */
        adapterState.Device.phy_set_bb_reg((ushort)pPhyReg.Rf3WireOffset, bMaskDWord, dataAndAddr);
        /* RTW_INFO("RFW-%d Addr[0x%x]=0x%x\n", eRFPath, pPhyReg.Rf3WireOffset, DataAndAddr); */

    }

    static void phy_SetRegBW_8812(AdapterState adapterState, ChannelWidth CurrentBW)
    {
        u16 RegRfMod_BW, u2tmp;
        RegRfMod_BW = adapterState.Device.rtw_read16(REG_WMAC_TRXPTCL_CTL);

        switch (CurrentBW)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                adapterState.Device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(RegRfMod_BW & 0xFE7F)); /* BIT 7 = 0, BIT 8 = 0 */
                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                u2tmp = (ushort)(RegRfMod_BW | BIT7);
                adapterState.Device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFEFF)); /* BIT 7 = 1, BIT 8 = 0 */
                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                u2tmp = (ushort)(RegRfMod_BW | BIT8);
                adapterState.Device.rtw_write16(REG_WMAC_TRXPTCL_CTL, (ushort)(u2tmp & 0xFF7F)); /* BIT 7 = 0, BIT 8 = 1 */
                break;

            default:
                RTW_INFO($"phy_PostSetBWMode8812():	unknown Bandwidth: {CurrentBW}");
                break;
        }
    }

    static byte phy_GetSecondaryChnl_8812(AdapterState adapterState)
    {
        VHT_DATA_SC SCSettingOf40 = 0, SCSettingOf20 = 0;
        var pHalData = adapterState.HalData;

        /* RTW_INFO("SCMapping: Case: pHalData.current_channel_bw %d, pHalData.nCur80MhzPrimeSC %d, pHalData.nCur40MhzPrimeSC %d\n",pHalData.current_channel_bw,pHalData.nCur80MhzPrimeSC,pHalData.nCur40MhzPrimeSC); */
        if (pHalData.current_channel_bw == ChannelWidth.CHANNEL_WIDTH_80)
        {
            if (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                SCSettingOf40 = VHT_DATA_SC.VHT_DATA_SC_40_LOWER_OF_80MHZ;
            }
            else if (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER)
            {
                SCSettingOf40 = VHT_DATA_SC.VHT_DATA_SC_40_UPPER_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }

            if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER) &&
                (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWEST_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWER_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ;
            }
            else if ((pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER) &&
                     (pHalData.nCur80MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER))
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPERST_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }
        }
        else if (pHalData.current_channel_bw == ChannelWidth.CHANNEL_WIDTH_40)
        {
            /* RTW_INFO("SCMapping: Case: pHalData.current_channel_bw %d, pHalData.nCur40MhzPrimeSC %d\n",pHalData.current_channel_bw,pHalData.nCur40MhzPrimeSC); */

            if (pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_UPPER)
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ;
            }
            else if (pHalData.nCur40MhzPrimeSC == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                SCSettingOf20 = VHT_DATA_SC.VHT_DATA_SC_20_LOWER_OF_80MHZ;
            }
            else
            {
                RTW_INFO("SCMapping: DONOT CARE Mode Setting");
            }
        }

        /*RTW_INFO("SCMapping: SC Value %x\n", ((SCSettingOf40 << 4) | SCSettingOf20));*/
        return (byte)(((byte)SCSettingOf40 << 4) | (byte)SCSettingOf20);
    }

    static void phy_PostSetBwMode8812(AdapterState adapterState)
    {
        u8 L1pkVal = 0, reg_837 = 0;
        var pHalData = adapterState.HalData;


        /* 3 Set Reg668 BW */
        phy_SetRegBW_8812(adapterState, pHalData.current_channel_bw);

        /* 3 Set Reg483 */
        var SubChnlNum = phy_GetSecondaryChnl_8812(adapterState);
        adapterState.Device.rtw_write8(REG_DATA_SC_8812, SubChnlNum);

        reg_837 = adapterState.Device.rtw_read8(rBWIndication_Jaguar + 3);
        /* 3 Set Reg848 Reg864 Reg8AC Reg8C4 RegA00 */
        switch (pHalData.current_channel_bw)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                adapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300200); /* 0x8ac[21,20,9:6,1,0]=8'b11100000 */
                adapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */

                if (pHalData.rf_type == RfType.RF_2T2R)
                {
                    adapterState.Device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, 7); /* 2R 0x848[25:22] = 0x7 */
                }
                else
                {
                    adapterState.Device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, 8); /* 1R 0x848[25:22] = 0x8 */
                }

                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                adapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300201); /* 0x8ac[21,20,9:6,1,0]=8'b11100000		 */
                adapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8c4[30] = 1'b0 */
                adapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x3C, SubChnlNum);
                adapterState.Device.phy_set_bb_reg(rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 6;
                else
                {
                    if (pHalData.rf_type == RfType.RF_2T2R)
                        L1pkVal = 7;
                    else
                        L1pkVal = 8;
                }

                adapterState.Device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x6 */

                if (SubChnlNum == (byte)VHT_DATA_SC.VHT_DATA_SC_20_UPPER_OF_80MHZ)
                {
                    adapterState.Device.phy_set_bb_reg(rCCK_System_Jaguar, bCCK_System_Jaguar, 1);
                }
                else
                {
                    adapterState.Device.phy_set_bb_reg(rCCK_System_Jaguar, bCCK_System_Jaguar, 0);
                }

                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                adapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x003003C3, 0x00300202); /* 0x8ac[21,20,9:6,1,0]=8'b11100010 */
                adapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8c4[30] = 1 */
                adapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x3C, SubChnlNum);
                adapterState.Device.phy_set_bb_reg(rCCAonSec_Jaguar, 0xf0000000, SubChnlNum);

                if ((reg_837 & BIT2) != 0)
                    L1pkVal = 5;
                else
                {
                    if (pHalData.rf_type == RfType.RF_2T2R)
                    {
                        L1pkVal = 6;
                    }
                    else
                    {
                        L1pkVal = 7;
                    }
                }

                adapterState.Device.phy_set_bb_reg(rL1PeakTH_Jaguar, 0x03C00000, L1pkVal); /* 0x848[25:22] = 0x5 */

                break;

            default:
                RTW_INFO("phy_PostSetBWMode8812():	unknown Bandwidth: %#X\n", pHalData.current_channel_bw);
                break;
        }

        /* <20121109, Kordan> A workaround for 8812A only. */
        phy_FixSpur_8812A(adapterState, pHalData.current_channel_bw, pHalData.current_channel);

        /* RTW_INFO("phy_PostSetBwMode8812(): Reg483: %x\n", rtw_read8(adapterState, 0x483)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg668: %x\n", rtw_read32(adapterState, 0x668)); */
        /* RTW_INFO("phy_PostSetBwMode8812(): Reg8AC: %x\n", phy_query_bb_reg(adapterState, rRFMOD_Jaguar, 0xffffffff)); */

        /* 3 Set RF related register */
        PHY_RF6052SetBandwidth8812(adapterState, pHalData.current_channel_bw);
    }

    static void PHY_RF6052SetBandwidth8812(AdapterState adapterState, ChannelWidth Bandwidth) /* 20M or 40M */
    {
        switch (Bandwidth)
        {
            case ChannelWidth.CHANNEL_WIDTH_20:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 20MHz\n"); */
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 3);
                break;

            case ChannelWidth.CHANNEL_WIDTH_40:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 40MHz\n"); */
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 1);
                break;

            case ChannelWidth.CHANNEL_WIDTH_80:
                /* RTW_INFO("PHY_RF6052SetBandwidth8812(), set 80MHz\n"); */
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_A, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                phy_set_rf_reg(adapterState, RfPath.RF_PATH_B, RF_CHNLBW_Jaguar, BIT11 | BIT10, 0);
                break;

            default:
                RTW_INFO("PHY_RF6052SetBandwidth8812(): unknown Bandwidth: %#X\n", Bandwidth);
                break;
        }
    }

    static void phy_FixSpur_8812A(AdapterState pAdapterState, ChannelWidth Bandwidth, u8 Channel)
    {
        /* C cut Item12 ADC FIFO CLOCK */
        if (IS_VENDOR_8812A_C_CUT(pAdapterState))
        {
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_40 && Channel == 11)
                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0xC00, 0x3); /* 0x8AC[11:10] = 2'b11 */
            else
                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0xC00, 0x2); /* 0x8AC[11:10] = 2'b10 */

            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
            {

                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 2'b11 */
                pAdapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */

            }
            else if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_40 &&
                     Channel == 11)
            {

                pAdapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 1); /* 0x8C4[30] = 1 */

            }
            else if (Bandwidth != ChannelWidth.CHANNEL_WIDTH_80)
            {

                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 2'b10	 */
                pAdapterState.Device.phy_set_bb_reg(rADC_Buf_Clk_Jaguar, BIT30, 0); /* 0x8C4[30] = 0 */

            }
        }
        else
        {
            /* <20120914, Kordan> A workarould to resolve 2480Mhz spur by setting ADC clock as 160M. (Asked by Binson) */
            if (Bandwidth == ChannelWidth.CHANNEL_WIDTH_20 &&
                (Channel == 13 || Channel == 14))
                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x3); /* 0x8AC[9:8] = 11 */
            else if (Channel <= 14) /* 2.4G only */
                pAdapterState.Device.phy_set_bb_reg(rRFMOD_Jaguar, 0x300, 0x2); /* 0x8AC[9:8] = 10 */
        }

    }

    public static void PHY_SetTxPowerIndex_8812A(AdapterState adapterState, u32 PowerIndex, RfPath RFPath, MGN_RATE Rate)
    {
        if (PowerIndexDescription.SetTable.TryGetValue(RFPath, out var rfTable))
        {
            if (rfTable.TryGetValue(Rate, out var values))
            {
                adapterState.Device.phy_set_bb_reg(values.RegAddress, values.BitMask, PowerIndex);
            }
            else
            {
                RTW_INFO("Invalid Rate!!");
            }
        }
        else
        {
            RTW_WARN("Invalid RFPath!!");
        }
    }
}