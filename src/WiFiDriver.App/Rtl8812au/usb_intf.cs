using System.Xml.Linq;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

using WiFiDriver.App.Rtl8812au;

namespace WiFiDriver.App.Rtl8812au;

public static class usb_intf
{
    private const byte USB_ENDPOINT_NUMBER_MASK = 0x0f;
    static readonly int rtw_rfintfs = (int)RFINTFS.HWPI;
    static readonly int rtw_chip_version = 0x00;
    static readonly int rtw_lbkmode = 0; /* RTL8712_AIR_TRX; */
    static readonly bool rtw_wifi_spec = false;


    private static int[] ui_pid = new[] { 0, 0, 0 };

    public static _adapter rtw_drv_init(UsbDevice pusb_intf)
    {
        _adapter padapter = null;
        int status = _FAIL;

        /* step 0. */
        /* Initialize dvobj_priv */
        var dvobj = usb_dvobj_init(pusb_intf);

        padapter = rtw_usb_primary_adapter_init(dvobj, pusb_intf);
        if (padapter == null)
        {
            RTW_INFO("rtw_usb_primary_adapter_init Failed!");
            throw new Exception("rtw_usb_primary_adapter_init Failed!");
        }

        //if (usb_reprobe_switch_usb_mode(padapter) == true)
        //{
        //    //goto free_if_prim;
        //}

        if (ui_pid[1] != 0)
        {
            RTW_INFO("ui_pid[1]:%d", ui_pid[1]);
            throw new Exception("ui_pid[1]:%d");
            //rtw_signal_process(ui_pid[1], SIGUSR2);
        }

        /* dev_alloc_name && register_netdev */
        // if (rtw_os_ndevs_init(dvobj) != _SUCCESS)
        // {
        //     throw new Exception();
        // }

        //hostapd_mode_init(padapter);

        return padapter;
    }

    private static _adapter rtw_usb_primary_adapter_init(dvobj_priv dvobj, UsbDevice pusb_intf)
    {
        _adapter padapter = new _adapter()
        {
            Device = pusb_intf
        };

        if (loadparam(padapter) != _SUCCESS)
        {
            throw new Exception("loadparam");
        }

        padapter.dvobj = dvobj;

        rtw_set_drv_stopped(padapter); /*init*/

        dvobj.padapters[dvobj.iface_nums++] = padapter;
        padapter.iface_id = IFACE_ID.IFACE_ID0;

        /* set adapter_type/iface type for primary padapter */
        padapter.isprimary = true;
        padapter.adapter_type = ADAPTER_TYPE.PRIMARY_ADAPTER;
        padapter.hw_port = hw_port.HW_PORT0;

        /* step init_io_priv */
        // TODO: this is adapter USB read write delegates
        //if (rtw_init_io_priv(padapter, usb_set_intf_ops) == _FAIL)
        //{
        //    throw new Exception("rtw_init_io_priv");
        //}

        /* step 2. hook HalFunc, allocate HalData */
        if (rtw_set_hal_ops(padapter) == _FAIL)
        {
            throw new Exception("rtw_set_hal_ops");
        }

        //padapter.intf_start = &usb_intf_start;
        //padapter.intf_stop = &usb_intf_stop;

        /* step read_chip_version */
        rtw_hal_read_chip_version(padapter);

        /* step usb endpoint mapping */
        rtw_hal_chip_configure(padapter);


        rtw_btcoex_wifionly_initialize(padapter);

        /* step read efuse/eeprom data and get mac_addr */
        if (rtw_hal_read_chip_info(padapter) == _FAIL)
        {
            throw new Exception("rtw_hal_read_chip_info");
        }

        /* step 5. */
        if (rtw_init_drv_sw(padapter) == _FAIL)
        {
            throw new Exception("rtw_init_drv_sw");
        }

        return padapter;
    }

    /// <summary>
    /// By design here Read queue is initialised
    /// </summary>
    static u8 rtw_init_drv_sw(_adapter padapter)
    {
        u8 ret8 = _SUCCESS;

        //# ifdef CONFIG_RTW_CFGVENDOR_RANDOM_MAC_OUI
        //    struct rtw_wdev_priv *pwdev_priv = adapter_wdev_data(padapter);
        //#endif

        //#if defined(CONFIG_AP_MODE) && defined(CONFIG_SUPPORT_MULTI_BCN)
        //	_rtw_init_listhead(&padapter.list);
        //# ifdef CONFIG_FW_HANDLE_TXBCN
        //	padapter.vap_id = CONFIG_LIMITED_AP_NUM;
        //	if (is_primary_adapter(padapter))
        //		adapter_to_dvobj(padapter).vap_tbtt_rpt_map = adapter_to_regsty(padapter).fw_tbtt_rpt;
        //#endif
        //#endif

        //# ifdef CONFIG_CLIENT_PORT_CFG
        //    padapter.client_id = MAX_CLIENT_PORT_NUM;
        //	padapter.client_port = CLT_PORT_INVALID;
        //	#endif

        //ret8 = rtw_init_default_value(padapter);

        //if ((rtw_init_cmd_priv(padapter.cmdpriv)) == _FAIL)
        //{
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        //padapter.cmdpriv.padapter = padapter;

        //if ((rtw_init_evt_priv(padapter.evtpriv)) == _FAIL)
        //{
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        //if (is_primary_adapter(padapter))
        //    rtw_rfctl_init(padapter);

        //if (rtw_init_mlme_priv(padapter) == _FAIL)
        //{
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        //# ifdef CONFIG_P2P
        //rtw_init_wifidirect_timers(padapter);
        //init_wifidirect_info(padapter, P2P_ROLE_DISABLE);
        //reset_global_wifidirect_info(padapter);
        //# ifdef CONFIG_IOCTL_CFG80211
        //rtw_init_cfg80211_wifidirect_info(padapter);
        //#endif
        //# ifdef CONFIG_WFD
        //if (rtw_init_wifi_display_info(padapter) == _FAIL)
        //    RTW_ERR("Can't init init_wifi_display_info\n");
        //#endif
        //#endif /* CONFIG_P2P */

        //if (init_mlme_ext_priv(padapter) == _FAIL)
        //{
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        //# ifdef CONFIG_TDLS
        //if (rtw_init_tdls_info(padapter) == _FAIL)
        //{
        //    RTW_INFO("Can't rtw_init_tdls_info\n");
        //    ret8 = _FAIL;
        //    goto exit;
        //}
        //#endif /* CONFIG_TDLS */

        //# ifdef CONFIG_RTW_MESH
        //rtw_mesh_cfg_init(padapter);
        //#endif

        //if (_rtw_init_xmit_priv(padapter.xmitpriv, padapter) == _FAIL)
        //{
        //    RTW_INFO("Can't _rtw_init_xmit_priv\n");
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        //if (_rtw_init_recv_priv(padapter.recvpriv, padapter) == _FAIL)
        //{
        //    RTW_INFO("Can't _rtw_init_recv_priv\n");
        //    ret8 = _FAIL;
        //    goto exit;
        //}

        /* add for CONFIG_IEEE80211W, none 11w also can use */
        //_rtw_spinlock_init(&padapter.security_key_mutex);

        /* We don't need to memset padapter.XXX to zero, because adapter is allocated by rtw_zvmalloc(). */
        /* _rtw_memset((unsigned char *)&padapter.securitypriv, 0, sizeof (struct security_priv)); */

        //        if (_rtw_init_sta_priv(&padapter.stapriv) == _FAIL)
        //        {
        //            RTW_INFO("Can't _rtw_init_sta_priv\n");
        //            ret8 = _FAIL;
        //            goto exit;
        //        }

        //        padapter.setband = WIFI_FREQUENCY_BAND_AUTO;
        //        padapter.fix_rate = 0xFF;
        //        padapter.power_offset = 0;
        //        padapter.rsvd_page_offset = 0;
        //        padapter.rsvd_page_num = 0;

        //        padapter.data_fb = 0;
        //        padapter.fix_rx_ampdu_accept = RX_AMPDU_ACCEPT_INVALID;
        //        padapter.fix_rx_ampdu_size = RX_AMPDU_SIZE_INVALID;
        ////# ifdef DBG_RX_COUNTER_DUMP
        //padapter.dump_rx_cnt_mode = 0;
        //padapter.drv_rx_cnt_ok = 0;
        //padapter.drv_rx_cnt_crcerror = 0;
        //padapter.drv_rx_cnt_drop = 0;
        //#endif
        //rtw_init_bcmc_stainfo(padapter);

        //rtw_init_pwrctrl_priv(padapter);

        /* _rtw_memset((u8 *)&padapter.qospriv, 0, sizeof (struct qos_priv)); */ /* move to mlme_priv */

        //# ifdef CONFIG_MP_INCLUDED
        //if (init_mp_priv(padapter) == _FAIL)
        //    RTW_INFO("%s: initialize MP private data Fail!\n", __func__);
        //#endif

        rtw_hal_dm_init(padapter);
//# ifdef CONFIG_RTW_SW_LED
//rtw_hal_sw_led_init(padapter);
//#endif
//# ifdef DBG_CONFIG_ERROR_DETECT
//rtw_hal_sreset_init(padapter);
//#endif


//# ifdef CONFIG_WAPI_SUPPORT
//padapter.WapiSupport = true; /* set true temp, will revise according to Efuse or Registry value later. */
//rtw_wapi_init(padapter);
//#endif

//# ifdef CONFIG_BR_EXT
//_rtw_spinlock_init(&padapter.br_ext_lock);
//#endif /* CONFIG_BR_EXT */

//# ifdef CONFIG_BEAMFORMING
//# ifdef RTW_BEAMFORMING_VERSION_2
//rtw_bf_init(padapter);
//#endif /* RTW_BEAMFORMING_VERSION_2 */
//#endif /* CONFIG_BEAMFORMING */

//# ifdef CONFIG_RTW_REPEATER_SON
//init_rtw_rson_data(adapter_to_dvobj(padapter));
//#endif

//# ifdef CONFIG_RTW_80211K
//rtw_init_rm(padapter);
//#endif

//# ifdef CONFIG_RTW_CFGVENDOR_RANDOM_MAC_OUI
//memset(pwdev_priv.pno_mac_addr, 0xFF, ETH_ALEN);
//#endif

        exit:

        return ret8;
    }

    static void rtw_hal_dm_init(_adapter padapter)
    {
        PHAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        padapter.hal_func.dm_init(padapter);
        phy_load_tx_power_ext_info(padapter, 1);
    }

    static void phy_load_tx_power_ext_info(_adapter adapter, u8 chk_file)
    {

        registry_priv regsty = adapter_to_regsty(adapter);

        /* check registy target tx power */
        regsty.target_tx_pwr_valid = rtw_regsty_chk_target_tx_power_valid(adapter);

        /* power by rate and limit */
        //if (phy_is_tx_power_by_rate_needed(adapter)
        //    || (phy_is_tx_power_limit_needed(adapter) && regsty.target_tx_pwr_valid != true)
        //   )
        //{
        //    phy_load_tx_power_by_rate(adapter, chk_file);
        //}

//#if CONFIG_TXPWR_LIMIT
//	if (phy_is_tx_power_limit_needed(adapter))
//		phy_load_tx_power_limit(adapter, chk_file);
//#endif
    }

    static bool rtw_regsty_chk_target_tx_power_valid(_adapter adapter)
    {
        // TODO: CHECK!!!!!!
        //hal_spec_t hal_spec = GET_HAL_SPEC(adapter);
        //HAL_DATA_TYPE hal_data = GET_HAL_DATA(adapter);
        //int path, tx_num, rs;
        //BAND_TYPE band;
        //s8 target;

        //for (band = BAND_TYPE.BAND_ON_2_4G; band <= BAND_TYPE.BAND_ON_5G; band++)
        //{
        //    if (!hal_is_band_support(adapter, band))
        //    {
        //        continue;
        //    }

        //    for (path = 0; path < RF_PATH_MAX; path++)
        //    {
        //        if (!HAL_SPEC_CHK_RF_PATH(hal_spec, band, path))
        //        {
        //            break;
        //        }

        //        for (rs = 0; rs < RATE_SECTION_NUM; rs++)
        //        {
        //            tx_num = rate_section_to_tx_num(rs);
        //            if (tx_num >= hal_spec.tx_nss_num)
        //                continue;

        //            if (band == BAND_ON_5G && IS_CCK_RATE_SECTION(rs))
        //                continue;

        //            if (IS_VHT_RATE_SECTION(rs) && !IS_HARDWARE_TYPE_JAGUAR_AND_JAGUAR2(adapter))
        //            {
        //                continue;
        //            }

        //            target = rtw_regsty_get_target_tx_power(adapter, band, path, rs);
        //            if (target == -1)
        //            {
        //                RTW_PRINT("%s return false for band:%d, path:%d, rs:%d, t:%d\n", __func__, band, path, rs,
        //                    target);
        //                return false;
        //            }
        //        }
        //    }
        //}

        return true;
    }

    static bool hal_is_band_support(_adapter adapter, u8 band)
    {
        return (GET_HAL_SPEC(adapter).band_cap & band_to_band_cap(band)) != 0;
    }

    static u8[] _band_to_band_cap =
    {
        BAND_CAP_2G,
        BAND_CAP_5G,
        0,
        0,
    };

    static byte band_to_band_cap(u8 band) =>
        (((band) >= (u8)BAND_TYPE.BAND_MAX) ? _band_to_band_cap[(int)BAND_TYPE.BAND_MAX] : _band_to_band_cap[(band)]);

    static void rtw_update_registrypriv_dev_network(_adapter adapter)
    {
        int sz = 0;

//        registry_priv pregistrypriv = adapter.registrypriv;
//        WLAN_BSSID_EX pdev_network = pregistrypriv.dev_network;
//        security_priv psecuritypriv = adapter.securitypriv;
//        wlan_network cur_network = adapter.mlmepriv.cur_network;
//        /* struct	xmit_priv	*pxmitpriv = &adapter.xmitpriv; */
//        mlme_ext_priv pmlmeext = adapter.mlmeextpriv;


//        pdev_network.Privacy = (psecuritypriv.dot11PrivacyAlgrthm > 0 ? 1 : 0); /* adhoc no 802.1x */

//        pdev_network.Rssi = 0;

//        switch (pregistrypriv.wireless_mode)
//        {
//            case WIRELESS_11B:
//                pdev_network.NetworkTypeInUse = (Ndis802_11DS);
//                break;
//            case WIRELESS_11G:
//            case WIRELESS_11BG:
//            case WIRELESS_11_24N:
//            case WIRELESS_11G_24N:
//            case WIRELESS_11BG_24N:
//                pdev_network.NetworkTypeInUse = (Ndis802_11OFDM24);
//                break;
//            case WIRELESS_11A:
//            case WIRELESS_11A_5N:
//                pdev_network.NetworkTypeInUse = (Ndis802_11OFDM5);
//                break;
//            case WIRELESS_11ABGN:
//                if (pregistrypriv.channel > 14)
//                    pdev_network.NetworkTypeInUse = (Ndis802_11OFDM5);
//                else
//                    pdev_network.NetworkTypeInUse = (Ndis802_11OFDM24);
//                break;
//            default:
//                /* TODO */
//                break;
//        }

//        pdev_network.Configuration.DSConfig = (pregistrypriv.channel);

//        if (cur_network.network.InfrastructureMode == Ndis802_11IBSS)
//        {
//            pdev_network.Configuration.ATIMWindow = (0);

//            if (pmlmeext.cur_channel != 0)
//                pdev_network.Configuration.DSConfig = pmlmeext.cur_channel;
//            else
//                pdev_network.Configuration.DSConfig = 1;
//        }

//        pdev_network.InfrastructureMode = (cur_network.network.InfrastructureMode);

        ///* 1. Supported rates */
        ///* 2. IE */

        ///* rtw_set_supported_rate(pdev_network.SupportedRates, pregistrypriv.wireless_mode) ; */
//        /* will be called in rtw_generate_ie */
//        sz = rtw_generate_ie(pregistrypriv);

//        pdev_network.IELength = sz;

//        pdev_network.Length = get_WLAN_BSSID_EX_sz((WLAN_BSSID_EX*)pdev_network);

//        /* notes: translate IELength & Length after assign the Length to cmdsz in createbss_cmd(); */
//        /* pdev_network.IELength = cpu_to_le32(sz); */

    }

    static int rtw_generate_ie(registry_priv pregistrypriv)
    {
        u8 wireless_mode;
        int sz = 0, rateLen;
//    WLAN_BSSID_EX pdev_network = &pregistrypriv.dev_network;
//    u8* ie = pdev_network.IEs;


//    /* timestamp will be inserted by hardware */
//    sz += 8;
//	ie += sz;

//	/* beacon interval : 2bytes */
//	*(u16*) ie = cpu_to_le16((u16)pdev_network.Configuration.BeaconPeriod); /* BCN_INTERVAL; */
//    sz += 2;
//	ie += 2;

//	/* capability info */
//	*(u16*) ie = 0;

//	*(u16*) ie |= cpu_to_le16(cap_IBSS);

//	if (pregistrypriv.preamble == PREAMBLE_SHORT)
//		*(u16*) ie |= cpu_to_le16(cap_ShortPremble);

//	if (pdev_network.Privacy)
//		*(u16*) ie |= cpu_to_le16(cap_Privacy);

//    sz += 2;
//	ie += 2;

//	/* SSID */
//	ie = rtw_set_ie(ie, _SSID_IE_, pdev_network.Ssid.SsidLength, pdev_network.Ssid.Ssid, &sz);

//	/* supported rates */
//	if (pregistrypriv.wireless_mode == WIRELESS_11ABGN) {
//		if (pdev_network.Configuration.DSConfig > 14)
//			wireless_mode = WIRELESS_11A_5N;
//		else
//			wireless_mode = WIRELESS_11BG_24N;
//	} else if (pregistrypriv.wireless_mode == WIRELESS_MODE_MAX) { /* WIRELESS_11ABGN | WIRELESS_11AC */
//		if (pdev_network.Configuration.DSConfig > 14)
//			wireless_mode = WIRELESS_11_5AC;
//		else
//			wireless_mode = WIRELESS_11BG_24N;
//	} else
//    wireless_mode = pregistrypriv.wireless_mode;

//rtw_set_supported_rate(pdev_network.SupportedRates, wireless_mode);

//rateLen = rtw_get_rateset_len(pdev_network.SupportedRates);

//if (rateLen > 8)
//{
//    ie = rtw_set_ie(ie, _SUPPORTEDRATES_IE_, 8, pdev_network.SupportedRates, &sz);
//    /* ie = rtw_set_ie(ie, _EXT_SUPPORTEDRATES_IE_, (rateLen - 8), (pdev_network.SupportedRates + 8), &sz); */
//}
//else
//    ie = rtw_set_ie(ie, _SUPPORTEDRATES_IE_, rateLen, pdev_network.SupportedRates, &sz);

        ///* DS parameter set */
//ie = rtw_set_ie(ie, _DSSET_IE_, 1, (u8*)&(pdev_network.Configuration.DSConfig), &sz);


        ///* IBSS Parameter Set */

//ie = rtw_set_ie(ie, _IBSS_PARA_IE_, 2, (u8*)&(pdev_network.Configuration.ATIMWindow), &sz);

//if (rateLen > 8)
//    ie = rtw_set_ie(ie, _EXT_SUPPORTEDRATES_IE_, (rateLen - 8), (pdev_network.SupportedRates + 8), &sz);

//# ifdef CONFIG_80211N_HT
        ///* HT Cap. */
//if (is_supported_ht(pregistrypriv.wireless_mode)
//    && (pregistrypriv.ht_enable == true))
//{
//    /* todo: */
//}
//#endif /* CONFIG_80211N_HT */

/* pdev_network.IELength =  sz; */ /* update IELength */


/* return _SUCCESS; */

        return sz;

    }

    private static u8 rtw_hal_read_chip_info(_adapter padapter)
    {
        u8 rtn = _SUCCESS;
        /*  before access eFuse, make sure card enable has been called */
        rtn = padapter.hal_func.read_adapter_info(padapter);
        return rtn;
    }

    static void rtw_btcoex_wifionly_initialize(PADAPTER padapter)
    {
        hal_btcoex_wifionly_initlizevariables(padapter);
    }

    static void hal_btcoex_wifionly_initlizevariables(PADAPTER padapter)
    {
        //struct wifi_only_cfg        *pwifionlycfg = &GLBtCoexistWifiOnly;
        //struct wifi_only_haldata    *pwifionly_haldata = &pwifionlycfg.haldata_info;
        //HAL_DATA_TYPE pHalData = GET_HAL_DATA(padapter);
        //_rtw_memset(&GLBtCoexistWifiOnly, 0, sizeof(GLBtCoexistWifiOnly));
        //pwifionlycfg.Adapter = padapter;
        //pwifionlycfg.chip_interface = WIFIONLY_INTF_USB;
        //pwifionly_haldata.customer_id = CUSTOMER_NORMAL;
    }

    static void rtw_hal_read_chip_version(_adapter padapter)
    {
        padapter.hal_func.read_chip_version(padapter);
        rtw_odm_init_ic_type(padapter);
    }

    static u8 rtw_set_hal_ops(_adapter padapter)
    {
        /* alloc memory for HAL DATA */
        if (rtw_hal_data_init(padapter) == _FAIL)
        {
            return _FAIL;
        }

        rtl8812au_set_hal_ops(padapter);

        if (hal_spec_init(padapter) == _FAIL)
        {
            return _FAIL;
        }

        return _SUCCESS;
    }

    static int hal_spec_init(_adapter adapter)
    {
        int ret = _SUCCESS;
        init_hal_spec_8812a(adapter);
        return ret;
    }

    static void init_hal_spec_8812a(_adapter adapter)
    {

        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);

        hal_spec.ic_name = "rtl8812a";
        hal_spec.macid_num = 128;
        hal_spec.sec_cam_ent_num = 64;
        hal_spec.sec_cap = 0;
        hal_spec.rfpath_num_2g = 2;
        hal_spec.rfpath_num_5g = 2;
        hal_spec.txgi_max = 63;
        hal_spec.txgi_pdbm = 2;
        hal_spec.max_tx_cnt = 2;
        hal_spec.tx_nss_num = 2;
        hal_spec.rx_nss_num = 2;
        hal_spec.band_cap = BAND_CAP_2G | BAND_CAP_5G;
        hal_spec.bw_cap = (byte)(BW_CAP_20M | BW_CAP_40M | BW_CAP_80M);
        hal_spec.port_num = 2;
        hal_spec.proto_cap = (byte)(PROTO_CAP_11B | PROTO_CAP_11G | PROTO_CAP_11N | PROTO_CAP_11AC);

        hal_spec.wl_func = (byte)(0
                            | WL_FUNC_P2P
                            | WL_FUNC_MIRACAST
                            | WL_FUNC_TDLS)
            ;

        hal_spec.pg_txpwr_saddr = 0x10;
        hal_spec.pg_txgi_diff_factor = 1;

        // TODO
        //rtw_macid_ctl_init_sleep_reg(adapter_to_macidctl(adapter)
        //    , REG_MACID_SLEEP
        //    , REG_MACID_SLEEP_1
        //    , REG_MACID_SLEEP_2
        //    , REG_MACID_SLEEP_3);
    }

//    static void rtw_macid_ctl_init_sleep_reg(macid_ctl_t macid_ctl, u16 m0, u16 m1, u16 m2, u16 m3)
//    {
//        macid_ctl.reg_sleep_m0 = m0;
////        MACID_NUM_SW_LIMIT 32
////#if (MACID_NUM_SW_LIMIT > 32)
////	macid_ctl.reg_sleep_m1 = m1;
////#endif
////#if (MACID_NUM_SW_LIMIT > 64)
////	macid_ctl.reg_sleep_m2 = m2;
////#endif
////#if (MACID_NUM_SW_LIMIT > 96)
////	macid_ctl.reg_sleep_m3 = m3;
////#endif
//    }

    static void rtw_hal_chip_configure(_adapter padapter)
    {
        padapter.hal_func.intf_chip_configure(padapter);
    }

    static void rtl8812au_set_hal_ops(_adapter padapter)
    {

        var pHalFunc = padapter.hal_func;


        pHalFunc.hal_power_on = UsbHalInit._InitPowerOn_8812AU;
        //pHalFunc.hal_power_off = hal_poweroff_8812au;

        pHalFunc.hal_init = UsbHalInit.rtl8812au_hal_init;
        //pHalFunc.hal_deinit = &rtl8812au_hal_deinit;

        //pHalFunc.inirp_init = &rtl8812au_inirp_init;
        //pHalFunc.inirp_deinit = &rtl8812au_inirp_deinit;

        //pHalFunc.init_xmit_priv = &rtl8812au_init_xmit_priv;
        //pHalFunc.free_xmit_priv = &rtl8812au_free_xmit_priv;

        //pHalFunc.init_recv_priv = &rtl8812au_init_recv_priv;
        //pHalFunc.free_recv_priv = &rtl8812au_free_recv_priv;
//#ifdef CONFIG_RTW_SW_LED
//        pHalFunc.InitSwLeds = &rtl8812au_InitSwLeds;
//        pHalFunc.DeInitSwLeds = &rtl8812au_DeInitSwLeds;
//#endif/* CONFIG_RTW_SW_LED */

//        pHalFunc.init_default_value = &rtl8812au_init_default_value;
        pHalFunc.intf_chip_configure = UsbHalInit.rtl8812au_interface_configure;
        pHalFunc.read_adapter_info = UsbHalInit.ReadAdapterInfo8812AU;

//        pHalFunc.set_hw_reg_handler = &SetHwReg8812AU;
//        pHalFunc.GetHwRegHandler = &GetHwReg8812AU;
//        pHalFunc.get_hal_def_var_handler = &GetHalDefVar8812AUsb;
//        pHalFunc.SetHalDefVarHandler = &SetHalDefVar8812AUsb;

//        pHalFunc.hal_xmit = &rtl8812au_hal_xmit;
//        pHalFunc.mgnt_xmit = &rtl8812au_mgnt_xmit;
//        pHalFunc.hal_xmitframe_enqueue = &rtl8812au_hal_xmitframe_enqueue;

//#ifdef CONFIG_HOSTAPD_MLME
//        pHalFunc.hostap_mgnt_xmit_entry = &rtl8812au_hostap_mgnt_xmit_entry;
//#endif
//        pHalFunc.interface_ps_func = &rtl8812au_ps_func;
//#ifdef CONFIG_XMIT_THREAD_MODE
//        pHalFunc.xmit_thread_handler = &rtl8812au_xmit_buf_handler;
//#endif
//#ifdef CONFIG_SUPPORT_USB_INT
//        pHalFunc.interrupt_handler = interrupt_handler_8812au;
//#endif

        rtl8812_set_hal_ops(pHalFunc);

    }

    static void rtl8812_set_hal_ops(hal_ops pHalFunc)
    {
        pHalFunc.dm_init = rtl8812_init_dm_priv;
// 	pHalFunc.dm_deinit = &rtl8812_deinit_dm_priv;
//
// 	pHalFunc.SetBeaconRelatedRegistersHandler = &SetBeaconRelatedRegisters8812A;
//
        pHalFunc.read_chip_version = UsbHalInit.read_chip_version_8812a;

// 	pHalFunc.set_tx_power_level_handler = &PHY_SetTxPowerLevel8812;
// 	pHalFunc.get_tx_power_level_handler = &PHY_GetTxPowerLevel8812;
//
        pHalFunc.set_tx_power_index_handler = PHY_SetTxPowerIndex_8812A;

// 	pHalFunc.hal_dm_watchdog = &rtl8812_HalDmWatchDog;
//
// 	pHalFunc.run_thread = &rtl8812_start_thread;
// 	pHalFunc.cancel_thread = &rtl8812_stop_thread;
//
        pHalFunc.read_bbreg = PHY_QueryBBReg8812;
// 	pHalFunc.read_rfreg = &PHY_QueryRFReg8812;
        pHalFunc.write_rfreg = PHY_SetRFReg8812;
//
// 	pHalFunc.read_wmmedca_reg = &rtl8812a_read_wmmedca_reg;
//
// 	/* Efuse related function */
// 	pHalFunc.EfusePowerSwitch = &rtl8812_EfusePowerSwitch;
// 	pHalFunc.ReadEFuse = &rtl8812_ReadEFuse;
        pHalFunc.EFUSEGetEfuseDefinition = rtl8812_EFUSE_GetEfuseDefinition;
// 	pHalFunc.EfuseGetCurrentSize = &rtl8812_EfuseGetCurrentSize;
// 	pHalFunc.Efuse_PgPacketRead = &rtl8812_Efuse_PgPacketRead;
// 	pHalFunc.Efuse_PgPacketWrite = &rtl8812_Efuse_PgPacketWrite;
// 	pHalFunc.Efuse_WordEnableDataWrite = &rtl8812_Efuse_WordEnableDataWrite;
// 	pHalFunc.Efuse_PgPacketWrite_BT = &Hal_EfusePgPacketWrite_BT;
//
// #ifdef DBG_CONFIG_ERROR_DETECT
// 	pHalFunc.sreset_init_value = &sreset_init_value;
// 	pHalFunc.sreset_reset_value = &sreset_reset_value;
// 	pHalFunc.silentreset = &sreset_reset;
// 	pHalFunc.sreset_xmit_status_check = &rtl8812_sreset_xmit_status_check;
// 	pHalFunc.sreset_linked_status_check  = &rtl8812_sreset_linked_status_check;
// 	pHalFunc.sreset_get_wifi_status  = &sreset_get_wifi_status;
// 	pHalFunc.sreset_inprogress = &sreset_inprogress;
// #endif /* DBG_CONFIG_ERROR_DETECT */
//
// 	pHalFunc.GetHalODMVarHandler = GetHalODMVar;
// 	pHalFunc.SetHalODMVarHandler = SetHalODMVar;
// 	pHalFunc.hal_notch_filter = &hal_notch_filter_8812;
//
// 	pHalFunc.c2h_handler = c2h_handler_8812a;
//
// 	pHalFunc.fill_h2c_cmd = &fill_h2c_cmd_8812;
// 	pHalFunc.fill_fake_txdesc = &rtl8812a_fill_fake_txdesc;
// 	pHalFunc.fw_dl = &FirmwareDownload8812;
// 	pHalFunc.hal_get_tx_buff_rsvd_page_num = &GetTxBufferRsvdPageNum8812;
    }

    static int rtl8812_EFUSE_GetEfuseDefinition(PADAPTER pAdapter, u8 efuseType, EFUSE_DEF_TYPE type,
        BOOLEAN bPseudoTest)
    {

        return Hal_EFUSEGetEfuseDefinition8812A(pAdapter, efuseType, type);
    }

    static u8 rtw_hal_data_init(_adapter padapter)
    {
        rtw_phydm_priv_init(padapter);

        return _SUCCESS;
    }

    static int Hal_EFUSEGetEfuseDefinition8812A(PADAPTER pAdapter, u8 efuseType, EFUSE_DEF_TYPE type)
    {
        switch (type)
        {
            //    case EFUSE_DEF_TYPE.TYPE_EFUSE_MAX_SECTION:
            //    {
            //        u8* pMax_section;
            //        pMax_section = (u8*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pMax_section = EFUSE_MAX_SECTION_JAGUAR;
            //        else
            //            *pMax_section = EFUSE_BT_MAX_SECTION;
            //    }
            //        break;
            //    case EFUSE_DEF_TYPE.TYPE_EFUSE_REAL_CONTENT_LEN:
            //    {
            //        u16* pu2Tmp;
            //        pu2Tmp = (u16*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pu2Tmp = EFUSE_REAL_CONTENT_LEN_JAGUAR;
            //        else
            //            *pu2Tmp = EFUSE_BT_REAL_CONTENT_LEN;
            //    }
            //        break;
            //    case EFUSE_DEF_TYPE.TYPE_EFUSE_CONTENT_LEN_BANK:
            //    {
            //        u16* pu2Tmp;
            //        pu2Tmp = (u16*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pu2Tmp = EFUSE_REAL_CONTENT_LEN_JAGUAR;
            //        else
            //            *pu2Tmp = EFUSE_BT_REAL_BANK_CONTENT_LEN;

            //    }
            //        break;
            //    case EFUSE_DEF_TYPE.TYPE_AVAILABLE_EFUSE_BYTES_BANK:
            //    {
            //        u16* pu2Tmp;
            //        pu2Tmp = (u16*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pu2Tmp = (u16)(EFUSE_REAL_CONTENT_LEN_JAGUAR - EFUSE_OOB_PROTECT_BYTES_JAGUAR);
            //        else
            //            *pu2Tmp = (EFUSE_BT_REAL_BANK_CONTENT_LEN - EFUSE_PROTECT_BYTES_BANK);
            //    }
            //        break;
            //    case EFUSE_DEF_TYPE.TYPE_AVAILABLE_EFUSE_BYTES_TOTAL:
            //    {
            //        u16* pu2Tmp;
            //        pu2Tmp = (u16*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pu2Tmp = (u16)(EFUSE_REAL_CONTENT_LEN_JAGUAR - EFUSE_OOB_PROTECT_BYTES_JAGUAR);
            //        else
            //            *pu2Tmp = (EFUSE_BT_REAL_CONTENT_LEN - (EFUSE_PROTECT_BYTES_BANK * 3));
            //    }
            //        break;
            case EFUSE_DEF_TYPE.TYPE_EFUSE_MAP_LEN:
            {

                if (efuseType == EFUSE_WIFI)
                    return EFUSE_MAP_LEN_JAGUAR;
                else
                    throw new NotImplementedException();
            }
                break;
            //    case EFUSE_DEF_TYPE.TYPE_EFUSE_PROTECT_BYTES_BANK:
            //    {
            //        u8* pu1Tmp;
            //        pu1Tmp = (u8*)pOut;
            //        if (efuseType == EFUSE_WIFI)
            //            *pu1Tmp = (u8)(EFUSE_OOB_PROTECT_BYTES_JAGUAR);
            //        else
            //            *pu1Tmp = EFUSE_PROTECT_BYTES_BANK;
            //    }
            //        break;
            //    default:
            //    {
            //        u8* pu1Tmp;
            //        pu1Tmp = (u8*)pOut;
            //        *pu1Tmp = 0;
            //    }
            //        break;
        }

        return 0;
    }

    static void rtw_phydm_priv_init(_adapter adapter)
    {
        PHAL_DATA_TYPE hal = GET_HAL_DATA(adapter);

        // TODO
        // dm_struct phydm = (hal.odmpriv);
        //
        // phydm.adapter = adapter;
        // odm_cmn_info_init(phydm, ODM_CMNINFO_PLATFORM, ODM_CE);
    }

//private static int rtw_init_io_priv(_adapter padapter, void (* set_intf_ops)(_adapter* padapter, struct _io_ops *pops))
//{
//    struct io_priv  *piopriv = &padapter.iopriv;
//    struct intf_hdl *pintf = &piopriv.intf;

//    if (set_intf_ops == null)
//        return _FAIL;

//    piopriv.padapter = padapter;
//    pintf.padapter = padapter;
//    pintf.pintf_dev = adapter_to_dvobj(padapter);

//    set_intf_ops(padapter, &pintf.io_ops);

//    return _SUCCESS;
//}

    static void rtw_set_drv_stopped(_adapter padapter)
    {
        dev_set_drv_stopped(adapter_to_dvobj(padapter));
    }

    static void dev_set_drv_stopped(dvobj_priv dvobj)
    {
        dvobj.bDriverStopped = true;
    }

    static uint loadparam(_adapter padapter)
    {
        uint status = _SUCCESS;

        var registry_par = padapter.registrypriv;

        registry_par.chip_version = (u8)rtw_chip_version;
        registry_par.rfintfs = (u8)rtw_rfintfs;
        registry_par.lbkmode = (u8)rtw_lbkmode;
        /* registry_par.hci = (u8)hci; */
        //registry_par.network_mode  = (u8) rtw_network_mode;

        //_rtw_memcpy(registry_par.ssid.Ssid, "ANY", 3);
        //registry_par.ssid.SsidLength = 3;

        //registry_par.channel = (u8) rtw_channel;
        //registry_par.wireless_mode = (u8) rtw_wireless_mode;

        //if (IsSupported24G(registry_par.wireless_mode) && (!is_supported_5g(registry_par.wireless_mode))
        //    && (registry_par.channel > 14))
        //	registry_par.channel = 1;
        //else if (is_supported_5g(registry_par.wireless_mode) && (!IsSupported24G(registry_par.wireless_mode)) && (registry_par.channel <= 14))
        registry_par.channel = 36;

//	registry_par.vrtl_carrier_sense = (u8) rtw_vrtl_carrier_sense;
//    registry_par.vcs_type = (u8) rtw_vcs_type;
//    registry_par.rts_thresh = (u16) rtw_rts_thresh;
//    registry_par.frag_thresh = (u16) rtw_frag_thresh;
//    registry_par.preamble = (u8) rtw_preamble;
//    registry_par.scan_mode = (u8) rtw_scan_mode;
//    registry_par.smart_ps = (u8) rtw_smart_ps;
//    registry_par.check_fw_ps = (u8) rtw_check_fw_ps;
//	registry_par.power_mgnt = (u8) rtw_power_mgnt;
//    registry_par.ips_mode = (u8) rtw_ips_mode;

//    registry_par.lps_level = (u8) rtw_lps_level;
//    registry_par.lps_chk_by_tp = (u8) rtw_lps_chk_by_tp;
//    registry_par.radio_enable = (u8) rtw_radio_enable;
//    registry_par.long_retry_lmt = (u8) rtw_long_retry_lmt;
//    registry_par.short_retry_lmt = (u8) rtw_short_retry_lmt;
//    registry_par.busy_thresh = (u16) rtw_busy_thresh;
//    registry_par.max_bss_cnt = (u16) rtw_max_bss_cnt;
//    /* registry_par.qos_enable = (u8)rtw_qos_enable; */
//    registry_par.ack_policy = (u8) rtw_ack_policy;
    registry_par.mp_mode = 0;

//    registry_par.software_encrypt = (u8) rtw_software_encrypt;
//    registry_par.software_decrypt = (u8) rtw_software_decrypt;

//    registry_par.acm_method = (u8) rtw_acm_method;
//    registry_par.usb_rxagg_mode = (u8) rtw_usb_rxagg_mode;
//    registry_par.dynamic_agg_enable = (u8) rtw_dynamic_agg_enable;

//    /* WMM */
//    registry_par.wmm_enable = (u8) rtw_wmm_enable;

//    registry_par.RegPwrTrimEnable = (u8) rtw_pwrtrim_enable;

//    registry_par.tx_bw_mode = (u8) rtw_tx_bw_mode;

//    registry_par.ht_enable = (u8) rtw_ht_enable;
//	if (registry_par.ht_enable && is_supported_ht(registry_par.wireless_mode)) {
//		registry_par.bw_mode = (u8) rtw_bw_mode;
//    registry_par.ampdu_enable = (u8) rtw_ampdu_enable;
//    registry_par.rx_stbc = (u8) rtw_rx_stbc;
//    registry_par.rx_ampdu_amsdu = (u8) rtw_rx_ampdu_amsdu;
//    registry_par.tx_ampdu_amsdu = (u8) rtw_tx_ampdu_amsdu;
//    registry_par.short_gi = (u8) rtw_short_gi;
//    registry_par.ldpc_cap = (u8) rtw_ldpc_cap;
//#if defined(CONFIG_CUSTOMER01_SMART_ANTENNA)
//		rtw_stbc_cap = 0x0;
//#elif defined(CONFIG_RTW_TX_2PATH_EN)
//		rtw_stbc_cap &= ~(BIT1|BIT5);
//#endif
//    registry_par.stbc_cap = (u8) rtw_stbc_cap;
//#if defined(CONFIG_RTW_TX_2PATH_EN)
//		rtw_beamform_cap &= ~(BIT0|BIT2|BIT4);
//#endif
//    registry_par.beamform_cap = (u8) rtw_beamform_cap;
//    registry_par.beamformer_rf_num = (u8) rtw_bfer_rf_number;
//    registry_par.beamformee_rf_num = (u8) rtw_bfee_rf_number;
//    rtw_regsty_init_rx_ampdu_sz_limit(registry_par);
//}

//# ifdef DBG_LA_MODE
//registry_par.la_mode_en = (u8) rtw_la_mode_en;
//#endif
//# ifdef CONFIG_80211AC_VHT
//registry_par.vht_enable = (u8) rtw_vht_enable;
//registry_par.ampdu_factor = (u8) rtw_ampdu_factor;
//registry_par.vht_rx_mcs_map[0] = (u8) (rtw_vht_rx_mcs_map & 0xFF);
//	registry_par.vht_rx_mcs_map[1] = (u8) ((rtw_vht_rx_mcs_map & 0xFF00) >> 8);
//#endif

//#ifdef CONFIG_TX_EARLY_MODE
//	registry_par.early_mode = (u8) rtw_early_mode;
//#endif
//# ifdef CONFIG_RTW_SW_LED
//registry_par.led_ctrl = (u8) rtw_led_ctrl;
//#endif
//registry_par.lowrate_two_xmit = (u8) rtw_lowrate_two_xmit;
registry_par.rf_config = rf_type.RF_TYPE_MAX;
//registry_par.low_power = (u8) rtw_low_power;

//registry_par.check_hw_status = (u8) rtw_check_hw_status;

        registry_par.wifi_spec = rtw_wifi_spec;

//	if (strlen(rtw_country_code) != 2
//		|| is_alpha(rtw_country_code[0]) == false
//		|| is_alpha(rtw_country_code[1]) == false
//	) {
//		if (rtw_country_code != rtw_country_unspecified)
//			RTW_ERR("%s discard rtw_country_code not in alpha2\n", __func__);
//_rtw_memset(registry_par.alpha2, 0xFF, 2);
//	} else
//    _rtw_memcpy(registry_par.alpha2, rtw_country_code, 2);

//registry_par.channel_plan = (u8)rtw_channel_plan;
//rtw_regsty_load_excl_chs(registry_par);

registry_par.special_rf_path = (u8)0;

//registry_par.full_ch_in_p2p_handshake = (u8)rtw_full_ch_in_p2p_handshake;
//# ifdef CONFIG_BT_COEXIST
//registry_par.btcoex = (u8)rtw_btcoex_enable;
//registry_par.bt_iso = (u8)rtw_bt_iso;
//registry_par.bt_sco = (u8)rtw_bt_sco;
//registry_par.bt_ampdu = (u8)rtw_bt_ampdu;
//registry_par.ant_num = (u8)rtw_ant_num;
//registry_par.single_ant_path = (u8)rtw_single_ant_path;
//#endif

//registry_par.bAcceptAddbaReq = (u8)rtw_AcceptAddbaReq;

//registry_par.antdiv_cfg = (u8)rtw_antdiv_cfg;
//registry_par.antdiv_type = (u8)rtw_antdiv_type;

//registry_par.drv_ant_band_switch = (u8)rtw_drv_ant_band_switch;

//registry_par.switch_usb_mode = (u8)rtw_switch_usb_mode;

//# ifdef CONFIG_AUTOSUSPEND
//registry_par.usbss_enable = (u8)rtw_enusbss;/* 0:disable,1:enable */
//#endif
//# ifdef SUPPORT_HW_RFOFF_DETECTED
//registry_par.hwpdn_mode = (u8)rtw_hwpdn_mode;/* 0:disable,1:enable,2:by EFUSE config */
//registry_par.hwpwrp_detect = (u8)rtw_hwpwrp_detect;/* 0:disable,1:enable */
//#endif

//registry_par.hw_wps_pbc = (u8)rtw_hw_wps_pbc;

//# ifdef CONFIG_ADAPTOR_INFO_CACHING_FILE
//snprintf(registry_par.adaptor_info_caching_file_path, PATH_LENGTH_MAX, "%s", rtw_adaptor_info_caching_file_path);
//registry_par.adaptor_info_caching_file_path[PATH_LENGTH_MAX - 1] = 0;
//#endif

//# ifdef CONFIG_LAYER2_ROAMING
//registry_par.max_roaming_times = (u8)rtw_max_roaming_times;
//# ifdef CONFIG_INTEL_WIDI
//registry_par.max_roaming_times = (u8)rtw_max_roaming_times + 2;
//#endif /* CONFIG_INTEL_WIDI */
//#endif

//# ifdef CONFIG_IOL
//registry_par.fw_iol = rtw_fw_iol;
//#endif

//# ifdef CONFIG_80211D
//registry_par.enable80211d = (u8)rtw_80211d;
//#endif

//snprintf(registry_par.ifname, 16, "%s", ifname);
//snprintf(registry_par.if2name, 16, "%s", if2name);

//registry_par.notch_filter = (u8)rtw_notch_filter;

//# ifdef CONFIG_CONCURRENT_MODE
//registry_par.virtual_iface_num = (u8)rtw_virtual_iface_num;
//#endif
//registry_par.pll_ref_clk_sel = (u8)rtw_pll_ref_clk_sel;

//#if CONFIG_TXPWR_LIMIT
//	registry_par.RegEnableTxPowerLimit = (u8)rtw_tx_pwr_lmt_enable;
//#endif
//registry_par.RegEnableTxPowerByRate = (u8)rtw_tx_pwr_by_rate;

//rtw_regsty_load_target_tx_power(registry_par);

//registry_par.tsf_update_pause_factor = (u8)rtw_tsf_update_pause_factor;
//registry_par.tsf_update_restore_factor = (u8)rtw_tsf_update_restore_factor;

registry_par.TxBBSwing_2G = -1;
registry_par.TxBBSwing_5G = -1;
//registry_par.bEn_RFE = 1;

        //registry_par.RFE_Type = (u8)rtw_RFE_type;
        registry_par.RFE_Type = 64;

//registry_par.PowerTracking_Type = (u8)rtw_powertracking_type;
        registry_par.AmplifierType_2G = 0;
        registry_par.AmplifierType_5G = 0;
//registry_par.GLNA_Type = (u8)rtw_GLNA_type;
//# ifdef CONFIG_LOAD_PHY_PARA_FROM_FILE
//registry_par.load_phy_file = (u8)rtw_load_phy_file;
//registry_par.RegDecryptCustomFile = (u8)rtw_decrypt_phy_file;
//#endif
//registry_par.qos_opt_enable = (u8)rtw_qos_opt_enable;

//registry_par.hiq_filter = (u8)rtw_hiq_filter;

//registry_par.adaptivity_en = (u8)rtw_adaptivity_en;
//registry_par.adaptivity_mode = (u8)rtw_adaptivity_mode;
//registry_par.adaptivity_th_l2h_ini = (s8)rtw_adaptivity_th_l2h_ini;
//registry_par.adaptivity_th_edcca_hl_diff = (s8)rtw_adaptivity_th_edcca_hl_diff;

//# ifdef CONFIG_DYNAMIC_SOML
//registry_par.dyn_soml_en = (u8)rtw_dynamic_soml_en;
//registry_par.dyn_soml_train_num = (u8)rtw_dynamic_soml_train_num;
//registry_par.dyn_soml_interval = (u8)rtw_dynamic_soml_interval;
//registry_par.dyn_soml_period = (u8)rtw_dynamic_soml_period;
//registry_par.dyn_soml_delay = (u8)rtw_dynamic_soml_delay;
//#endif

//registry_par.boffefusemask = (u8)rtw_OffEfuseMask;
//registry_par.bFileMaskEfuse = (u8)rtw_FileMaskEfuse;

//# ifdef CONFIG_RTW_ACS
//registry_par.acs_mode = (u8)rtw_acs;
//registry_par.acs_auto_scan = (u8)rtw_acs_auto_scan;
//#endif
//# ifdef CONFIG_BACKGROUND_NOISE_MONITOR
//registry_par.nm_mode = (u8)rtw_nm;
//#endif
//registry_par.reg_rxgain_offset_2g = (u32)rtw_rxgain_offset_2g;
//registry_par.reg_rxgain_offset_5gl = (u32)rtw_rxgain_offset_5gl;
//registry_par.reg_rxgain_offset_5gm = (u32)rtw_rxgain_offset_5gm;
//registry_par.reg_rxgain_offset_5gh = (u32)rtw_rxgain_offset_5gh;

//# ifdef CONFIG_DFS_MASTER
//registry_par.dfs_region_domain = (u8)rtw_dfs_region_domain;
//#endif

//# ifdef CONFIG_MCC_MODE
//registry_par.en_mcc = (u8)rtw_en_mcc;
//registry_par.rtw_mcc_ap_bw20_target_tx_tp = (u32)rtw_mcc_ap_bw20_target_tx_tp;
//registry_par.rtw_mcc_ap_bw40_target_tx_tp = (u32)rtw_mcc_ap_bw40_target_tx_tp;
//registry_par.rtw_mcc_ap_bw80_target_tx_tp = (u32)rtw_mcc_ap_bw80_target_tx_tp;
//registry_par.rtw_mcc_sta_bw20_target_tx_tp = (u32)rtw_mcc_sta_bw20_target_tx_tp;
//registry_par.rtw_mcc_sta_bw40_target_tx_tp = (u32)rtw_mcc_sta_bw40_target_tx_tp;
//registry_par.rtw_mcc_sta_bw80_target_tx_tp = (u32)rtw_mcc_sta_bw80_target_tx_tp;
//registry_par.rtw_mcc_single_tx_cri = (u32)rtw_mcc_single_tx_cri;
//registry_par.rtw_mcc_policy_table_idx = rtw_mcc_policy_table_idx;
//registry_par.rtw_mcc_duration = (u8)rtw_mcc_duration;
//registry_par.rtw_mcc_enable_runtime_duration = rtw_mcc_enable_runtime_duration;
//#endif /*CONFIG_MCC_MODE */

//# ifdef CONFIG_WOWLAN
//registry_par.wakeup_event = rtw_wakeup_event;
//registry_par.suspend_type = rtw_suspend_type;
//#endif

//# ifdef CONFIG_SUPPORT_TRX_SHARED
//registry_par.trx_share_mode = rtw_trx_share_mode;
//#endif
//registry_par.wowlan_sta_mix_mode = rtw_wowlan_sta_mix_mode;

//# ifdef CONFIG_PCI_HCI
//registry_par.pci_aspm_config = rtw_pci_aspm_enable;
//#endif

//# ifdef CONFIG_RTW_NAPI
//registry_par.en_napi = (u8)rtw_en_napi;
//# ifdef CONFIG_RTW_NAPI_DYNAMIC
//registry_par.napi_threshold = (u32)rtw_napi_threshold;
//#endif /* CONFIG_RTW_NAPI_DYNAMIC */
//# ifdef CONFIG_RTW_GRO
//registry_par.en_gro = (u8)rtw_en_gro;
//if (!registry_par.en_napi && registry_par.en_gro)
//{
//    registry_par.en_gro = 0;
//    RTW_WARN("Disable GRO because NAPI is not enabled\n");
//}
//#endif /* CONFIG_RTW_GRO */
//#endif /* CONFIG_RTW_NAPI */

//registry_par.iqk_fw_offload = (u8)rtw_iqk_fw_offload;
//registry_par.ch_switch_offload = (u8)rtw_ch_switch_offload;

//# ifdef CONFIG_TDLS
//registry_par.en_tdls = rtw_en_tdls;
//#endif

//# ifdef CONFIG_ADVANCE_OTA
//registry_par.adv_ota = rtw_advnace_ota;
//#endif
//# ifdef CONFIG_FW_OFFLOAD_PARAM_INIT
//registry_par.fw_param_init = rtw_fw_param_init;
//#endif
//# ifdef CONFIG_AP_MODE
//registry_par.bmc_tx_rate = rtw_bmc_tx_rate;
//#endif
//# ifdef CONFIG_FW_HANDLE_TXBCN
//registry_par.fw_tbtt_rpt = rtw_tbtt_rpt;
//#endif

//registry_par.monitor_overwrite_seqnum = (u8)rtw_monitor_overwrite_seqnum;
//registry_par.monitor_retransmit = (u8)rtw_monitor_retransmit;
//registry_par.monitor_disable_1m = (u8)rtw_monitor_disable_1m;

        return status;
    }

    private static dvobj_priv usb_dvobj_init(UsbDevice usb_intf)
    {

        bool status = false;
        dvobj_priv pdvobjpriv = new();


        pdvobjpriv.pusbintf = usb_intf;
        var pusbd = pdvobjpriv.pusbdev = usb_intf;

        pdvobjpriv.RtNumInPipes = 0;
        pdvobjpriv.RtNumOutPipes = 0;

        pdvobjpriv.NumInterfaces = 1;
        pdvobjpriv.InterfaceNumber = 0;
        pdvobjpriv.nr_endpoint = (byte)usb_intf.Configs[0].Interfaces[0].Endpoints.Count;

        /* RTW_INFO("\ndump usb_endpoint_descriptor:\n"); */
        int i = 0;
        foreach (var endpoint in usb_intf.Configs[0].Interfaces[0].Endpoints)
        {
            var type = (EndpointType)(endpoint.Attributes & 0x3);
            var direction = (EndpointDirection)(endpoint.EndpointAddress & (0b1000_0000));

            RTW_INFO("usb_endpoint_descriptor(%d):", i);
            //RTW_INFO("bDescriptorType=%x\n", endpoint.CustomDescriptors);
            RTW_INFO("bEndpointAddress=%x", endpoint.EndpointAddress);
            RTW_INFO("wMaxPacketSize=%d", endpoint.MaxPacketSize);
            RTW_INFO("bInterval=%x", endpoint.Interval);

            if (type == EndpointType.Bulk && direction == EndpointDirection.In)
            {
                RTW_INFO("RT_usb_endpoint_is_bulk_in = %x", RT_usb_endpoint_num(endpoint));
                pdvobjpriv.RtInPipe[pdvobjpriv.RtNumInPipes] = RT_usb_endpoint_num(endpoint);
                pdvobjpriv.RtNumInPipes++;
            }
            else if (direction == EndpointDirection.In)
            {
                RTW_INFO("RT_usb_endpoint_is_int_in = %x, Interval = %x\n", RT_usb_endpoint_num(endpoint),
                    endpoint.Interval);
                pdvobjpriv.RtInPipe[pdvobjpriv.RtNumInPipes] = RT_usb_endpoint_num(endpoint);
                pdvobjpriv.RtNumInPipes++;
            }
            else if (type == EndpointType.Bulk && direction == EndpointDirection.Out)
            {
                RTW_INFO("RT_usb_endpoint_is_bulk_out = %x\n", RT_usb_endpoint_num(endpoint));
                pdvobjpriv.RtOutPipe[pdvobjpriv.RtNumOutPipes] = RT_usb_endpoint_num(endpoint);
                pdvobjpriv.RtNumOutPipes++;
            }

            pdvobjpriv.ep_num[i] = RT_usb_endpoint_num(endpoint);
        }

        RTW_INFO("nr_endpoint=%d, in_num=%d, out_num=%d\n\n", pdvobjpriv.nr_endpoint, pdvobjpriv.RtNumInPipes,
            pdvobjpriv.RtNumOutPipes);

        switch (pusbd.Speed)
        {
            case USB_SPEED_LOW:
                RTW_INFO("USB_SPEED_LOW\n");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_1_1;
                break;
            case USB_SPEED_FULL:
                RTW_INFO("USB_SPEED_FULL\n");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_1_1;
                break;
            case USB_SPEED_HIGH:
                RTW_INFO("USB_SPEED_HIGH\n");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_2;
                break;

            case USB_SPEED_SUPER:
                RTW_INFO("USB_SPEED_SUPER\n");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_3;
                break;

            default:
                RTW_INFO("USB_SPEED_UNKNOWN(%x)\n", pusbd.Speed);
                pdvobjpriv.usb_speed = RTW_USB_SPEED_UNKNOWN;
                break;
        }

        if (pdvobjpriv.usb_speed == RTW_USB_SPEED_UNKNOWN)
        {
            RTW_INFO("UNKNOWN USB SPEED MODE, ERROR !!!");
            throw new Exception();
        }


        /*step 1-1., decide the chip_type via driver_info*/
        pdvobjpriv.interface_type = RTL871X_HCI_TYPE.RTW_USB;
        rtw_decide_chip_type_by_usb_info(pdvobjpriv);

        return pdvobjpriv;
    }

    static void rtw_decide_chip_type_by_usb_info(dvobj_priv pdvobjpriv)
    {
        if (dvobj_priv.chip_type == CHIP_TYPE.RTL8812)
        {
            pdvobjpriv.HardwareType = HARDWARE_TYPE.HARDWARE_TYPE_RTL8812AU;
            RTW_INFO("CHIP TYPE: RTL8812\n");
        }
    }

    static int RT_usb_endpoint_num(UsbEndpointInfo epd)
    {
        return epd.EndpointAddress & USB_ENDPOINT_NUMBER_MASK;
    }

    public static bool rtw_hal_init(_adapter padapter, InitChannel initChannel)
    {
        bool status = true;

        dvobj_priv dvobj = adapter_to_dvobj(padapter);
        int i;

        status = padapter.hal_func.hal_init(padapter);

        if (status == true)
        {
            padapter.HalData.hw_init_completed = true;

            // TODO: Need or not???
            //rtw_mi_set_mac_addr(padapter); /*set mac addr of all ifaces*/

            // notch_filter 1 by default
            //if (padapter.registrypriv.notch_filter == 1)
            //{
            //    rtw_hal_notch_filter(padapter, 1);
            //}

            // TODO: Need or not???
            //for (i = 0; i < dvobj.iface_nums; i++)
            //{
            //    rtw_sec_restore_wep_key(dvobj.padapters[i]);
            //}

            //rtw_led_control(padapter, LED_CTL_POWER_ON);

            init_hw_mlme_ext(padapter, initChannel);

            rtw_hal_init_opmode(padapter);

        }
        else
        {
            padapter.HalData.hw_init_completed =  false;
            RTW_ERR("rtw_hal_init: fail");
        }

        return status;
    }

    private static void rtw_hal_init_opmode(_adapter padapter)
    {
        rtw_setopmode_cmd(padapter, RTW_CMDF.RTW_CMDF_DIRECTLY);
    }
}