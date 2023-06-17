namespace Rtl8812auNet.Rtl8812au;

public class hal_spec_t
{
    public string ic_name;
    public u8 macid_num;

    public u8 sec_cam_ent_num;
    public u8 sec_cap;

    public u8 rfpath_num_2g = 4; /* used for tx power index path */
    public u8 rfpath_num_5g = 4; /* used for tx power index path */
    public u8 txgi_max; /* maximum tx power gain index */
    public u8 txgi_pdbm; /* tx power gain index per dBm */

    public u8 max_tx_cnt;
    public u8 tx_nss_num = 4;
    public u8 rx_nss_num = 4;
    public u8 band_cap;    /* value of BAND_CAP_XXX */
    public u8 bw_cap;      /* value of BW_CAP_XXX */
    public u8 port_num;
    public u8 proto_cap;   /* value of PROTO_CAP_XXX */
    public u8 wl_func;     /* value of WL_FUNC_XXX */

    u8 rx_tsf_filter = 1;

    public u8 pg_txpwr_saddr; /* starting address of PG tx power info */
    public u8 pg_txgi_diff_factor; /* PG tx power gain index diff to tx power gain index */

    u8 hci_type;	/* value of HCI Type */
};