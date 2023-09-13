using Microsoft.Extensions.Logging;

namespace Rtl8812auNet.Rtl8812au.Modules;

public class RfPowerManagementModule
{
    private readonly RtlUsbAdapter _device;
    private readonly ILogger _logger;

    public RfPowerManagementModule(RtlUsbAdapter device, ILogger logger)
    {
        _device = device;
        _logger = logger;
    }

    static u8 phy_get_tx_power_index()
    {
        return 16;
    }

    public void PHY_SetTxPowerLevel8812(hal_com_data pHalData, u8 Channel)
    {
        for (var path = (u8)RfPath.RF_PATH_A; (byte)path < pHalData.NumTotalRFPath; ++path)
        {
            phy_set_tx_power_level_by_path(pHalData, Channel, (RfPath)path);
            PHY_TxPowerTrainingByPath_8812(pHalData, (RfPath)path);
        }
    }

    private void phy_set_tx_power_level_by_path(hal_com_data pHalData, u8 channel, RfPath path)
    {
        BOOLEAN bIsIn24G = (pHalData.current_band_type == BandType.BAND_ON_2_4G);

        if (bIsIn24G)
        {
            phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.CCK);
        }

        phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.OFDM);

        phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.HT_MCS0_MCS7);
        phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.VHT_1SSMCS0_1SSMCS9);

        if (pHalData.NumTotalRFPath >= 2)
        {
            phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.HT_MCS8_MCS15);
            phy_set_tx_power_index_by_rate_section(pHalData, path, channel, RATE_SECTION.VHT_2SSMCS0_2SSMCS9);
        }
    }

    private void phy_set_tx_power_index_by_rate_section(
        hal_com_data pHalData,
        RfPath rfPath,
        u8 channel,
        RATE_SECTION rateSection)
    {
        _logger.LogDebug("SET_TX_POWER {RfPath}; {Channel}; {RateSection}", rfPath, channel, rateSection);

        if (rateSection >= RATE_SECTION.RATE_SECTION_NUM)
        {
            throw new Exception("RateSection >= RATE_SECTION.RATE_SECTION_NUM");
        }

        // TODO: WTF is going on?
        if (rateSection == RATE_SECTION.CCK && pHalData.current_band_type != BandType.BAND_ON_2_4G)
        {
            return;
        }

        PHY_SetTxPowerIndexByRateArray(rfPath, rates_by_sections[(int)rateSection].rates);
    }

    private void PHY_TxPowerTrainingByPath_8812(hal_com_data pHalData, RfPath rfPath)
    {
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

        _device.phy_set_bb_reg(writeOffset, 0xffffff, writeData);
    }

    private void PHY_SetTxPowerIndexByRateArray(
        RfPath rfPath,
        MGN_RATE[] rates)
    {
        for (int i = 0; i < rates.Length; ++i)
        {
            var powerIndex = phy_get_tx_power_index();
            MGN_RATE rate = rates[i];
            PHY_SetTxPowerIndex_8812A(powerIndex, rfPath, rate);
        }
    }

    private void PHY_SetTxPowerIndex_8812A(u32 powerIndex, RfPath rfPath, MGN_RATE rate)
    {
        if (PowerIndexDescription.SetTable.TryGetValue(rfPath, out var rfTable))
        {
            if (rfTable.TryGetValue(rate, out var values))
            {
                _device.phy_set_bb_reg(values.RegAddress, values.BitMask, powerIndex);
            }
            else
            {
                _logger.LogError("Invalid rate! RfPath: {RfPath} Rate:{Rate}", rfPath, rate);
            }
        }
        else
        {
            _logger.LogError("Invalid RfPath! RfPath: {RfPath} Rate:{Rate}", rfPath, rate);
        }
    }
}