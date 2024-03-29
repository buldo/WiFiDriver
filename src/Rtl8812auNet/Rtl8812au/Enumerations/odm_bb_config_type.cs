﻿using System.Diagnostics.CodeAnalysis;

namespace Rtl8812auNet.Rtl8812au.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum odm_bb_config_type
{
    CONFIG_BB_PHY_REG,
    CONFIG_BB_AGC_TAB,
    CONFIG_BB_AGC_TAB_2G,
    CONFIG_BB_AGC_TAB_5G,
    CONFIG_BB_PHY_REG_PG,
    CONFIG_BB_PHY_REG_MP,
    CONFIG_BB_AGC_TAB_DIFF,
    CONFIG_BB_RF_CAL_INIT,
}