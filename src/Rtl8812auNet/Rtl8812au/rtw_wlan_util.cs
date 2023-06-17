namespace Rtl8812auNet.Rtl8812au;

public static class rtw_wlan_util
{
    public static void Set_MSR(_adapter padapter, u8 type)
    {
        rtw_hal_set_msr(padapter, type);
    }

    public static void set_channel_bwmode(_adapter padapter, byte channel, byte channel_offset, channel_width bwmode)
    {
        u8 center_ch, chnl_offset80 = HAL_PRIME_CHNL_OFFSET_DONT_CARE;

        //if (padapter.bNotifyChannelChange)
        //{
        //    RTW_INFO("[%s] ch = %d, offset = %d, bwmode = %d\n", __func__, channel, channel_offset, bwmode);
        //}

        center_ch = rtw_get_center_ch(channel, bwmode, channel_offset);
        if (bwmode == channel_width.CHANNEL_WIDTH_80)
        {
            if (center_ch > channel)
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_LOWER;
            }
            else if (center_ch < channel)
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_UPPER;
            }
            else
            {
                chnl_offset80 = HAL_PRIME_CHNL_OFFSET_DONT_CARE;
            }
        }

        // set Channel
        // saved channel/bw info
        var dvobj = adapter_to_dvobj(padapter);
        dvobj.oper_channel = channel;

        rtw_hal_set_chnl_bw(padapter, center_ch, bwmode, channel_offset, chnl_offset80); /* set center channel */
    }

    private static u8 rtw_get_center_ch(u8 channel, channel_width chnl_bw, u8 chnl_offset)
    {
        u8 center_ch = channel;

        if (chnl_bw == channel_width.CHANNEL_WIDTH_80)
        {
            if (channel == 36 || channel == 40 || channel == 44 || channel == 48)
                center_ch = 42;
            else if (channel == 52 || channel == 56 || channel == 60 || channel == 64)
                center_ch = 58;
            else if (channel == 100 || channel == 104 || channel == 108 || channel == 112)
                center_ch = 106;
            else if (channel == 116 || channel == 120 || channel == 124 || channel == 128)
                center_ch = 122;
            else if (channel == 132 || channel == 136 || channel == 140 || channel == 144)
                center_ch = 138;
            else if (channel == 149 || channel == 153 || channel == 157 || channel == 161)
                center_ch = 155;
            else if (channel == 165 || channel == 169 || channel == 173 || channel == 177)
                center_ch = 171;
            else if (channel <= 14)
                center_ch = 7;
        }
        else if (chnl_bw == channel_width.CHANNEL_WIDTH_40)
        {
            if (chnl_offset == HAL_PRIME_CHNL_OFFSET_LOWER)
            {
                center_ch = (byte)(channel + 2);
            }
            else
            {
                center_ch = (byte)(channel - 2);
            }
        }
        else if (chnl_bw == channel_width.CHANNEL_WIDTH_20)
            center_ch = channel;
        else
        {
            throw new Exception();
        }

        return center_ch;
    }

    public static void rtw_hal_set_chnl_bw(_adapter padapter, u8 channel, channel_width Bandwidth, u8 Offset40,
        u8 Offset80)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        u8 cch_160 = Bandwidth == channel_width.CHANNEL_WIDTH_160 ? channel : (u8)0;
        u8 cch_80 = Bandwidth == channel_width.CHANNEL_WIDTH_80 ? channel : (u8)0;
        u8 cch_40 = Bandwidth == channel_width.CHANNEL_WIDTH_40 ? channel : (u8)0;
        u8 cch_20 = Bandwidth == channel_width.CHANNEL_WIDTH_20 ? channel : (u8)0;

        if (cch_80 != 0)
        {
            cch_40 = rtw_get_scch_by_cch_offset(cch_80, channel_width.CHANNEL_WIDTH_80, Offset80);
        }

        if (cch_40 != 0)
        {
            cch_20 = rtw_get_scch_by_cch_offset(cch_40, channel_width.CHANNEL_WIDTH_40, Offset40);
        }

        pHalData.cch_80 = cch_80;
        pHalData.cch_40 = cch_40;
        pHalData.cch_20 = cch_20;

        PHY_SetSwChnlBWMode8812(padapter, channel, Bandwidth, Offset40, Offset80);
    }
}