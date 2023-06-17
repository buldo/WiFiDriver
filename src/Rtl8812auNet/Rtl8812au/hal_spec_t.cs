namespace Rtl8812auNet.Rtl8812au;

public class hal_spec_t
{
    public u8 rfpath_num_2g = 4; /* used for tx power index path */
    public u8 rfpath_num_5g = 4; /* used for tx power index path */
    public u8 txgi_max; /* maximum tx power gain index */

    public u8 max_tx_cnt;
    public u8 band_cap;    /* value of BAND_CAP_XXX */
    public u8 proto_cap;   /* value of PROTO_CAP_XXX */

    public u8 pg_txpwr_saddr; /* starting address of PG tx power info */
    public u8 pg_txgi_diff_factor; /* PG tx power gain index diff to tx power gain index */
};