namespace WiFiDriver.App.Rtl8812au;

public static class rtw_rf
{
    /// <summary>
    /// Get center channel of smaller bandwidth by @param cch, @param bw, @param offset
    /// </summary>
    /// <param name="cch">the given center channel</param>
    /// <param name="bw">the given bandwidth</param>
    /// <param name="offset">the given primary SC offset of the given bandwidth</param>
    /// <returns>center channel of smaller bandiwdth if valid, or 0</returns>
    public static u8 rtw_get_scch_by_cch_offset(u8 cch, channel_width bw, u8 offset)
    {
        u8 t_cch = 0;

        if (bw == channel_width.CHANNEL_WIDTH_20)
        {
            t_cch = cch;
            goto exit;
        }

        if (offset == HAL_PRIME_CHNL_OFFSET_DONT_CARE)
        {
            throw new Exception();
            goto exit;
        }

        /* 2.4G, 40MHz */
        if (cch >= 3 && cch <= 11 && bw == channel_width.CHANNEL_WIDTH_40)
        {
            t_cch = (byte)((offset == HAL_PRIME_CHNL_OFFSET_UPPER) ? cch + 2 : cch - 2);
            goto exit;
        }

        /* 5G, 160MHz */
        if (cch >= 50 && cch <= 163 && bw == channel_width.CHANNEL_WIDTH_160)
        {
            t_cch = (byte)((offset == HAL_PRIME_CHNL_OFFSET_UPPER) ? cch + 8 : cch - 8);
            goto exit;

            /* 5G, 80MHz */
        }
        else if (cch >= 42 && cch <= 171 && bw == channel_width.CHANNEL_WIDTH_80)
        {
            t_cch = (byte)((offset == HAL_PRIME_CHNL_OFFSET_UPPER) ? cch + 4 : cch - 4);
            goto exit;

            /* 5G, 40MHz */
        }
        else if (cch >= 38 && cch <= 175 && bw == channel_width.CHANNEL_WIDTH_40)
        {
            t_cch = (byte)((offset == HAL_PRIME_CHNL_OFFSET_UPPER) ? cch + 2 : cch - 2);
            goto exit;

        }
        else
        {
            throw new Exception();
            goto exit;
        }

        exit:
        return t_cch;
    }
}