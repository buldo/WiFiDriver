namespace Rtl8812auNet.Rtl8812au;

public class hal_spec_t
{
    public u8 rfpath_num_2g = 2; /* used for tx power index path */
    public u8 rfpath_num_5g = 2; /* used for tx power index path */
    public u8 txgi_max => 63; /* maximum tx power gain index */

    public u8 max_tx_cnt => 2;
    public u8 band_cap => BAND_CAP_2G | BAND_CAP_5G;    /* value of BAND_CAP_XXX */
    public u8 proto_cap { get; set; } = (byte)(PROTO_CAP_11B | PROTO_CAP_11G | PROTO_CAP_11N | PROTO_CAP_11AC);   /* value of PROTO_CAP_XXX */

    public u8 pg_txpwr_saddr => 0x10; /* starting address of PG tx power info */
    public u8 pg_txgi_diff_factor => 1; /* PG tx power gain index diff to tx power gain index */
};