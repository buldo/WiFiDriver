﻿using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public static class usb_intf
{
    static readonly bool rtw_wifi_spec = false;
    private static int[] ui_pid = new[] { 0, 0, 0 };

    public static _adapter rtw_drv_init(IRtlUsbDevice pusb_intf)
    {

        var dvobj = usb_dvobj_init(pusb_intf);

        var padapter = rtw_usb_primary_adapter_init(dvobj, pusb_intf);
        if (padapter == null)
        {
            RTW_INFO("rtw_usb_primary_adapter_init Failed!");
            throw new Exception("rtw_usb_primary_adapter_init Failed!");
        }

        if (ui_pid[1] != 0)
        {
            RTW_INFO("ui_pid[1]:%d", ui_pid[1]);
            throw new Exception("ui_pid[1]:%d");
            //rtw_signal_process(ui_pid[1], SIGUSR2);
        }

        return padapter;
    }

    private static _adapter rtw_usb_primary_adapter_init(dvobj_priv dvobj, IRtlUsbDevice pusb_intf)
    {
        _adapter padapter = new _adapter()
        {
            Device = pusb_intf
        };

        loadparam(padapter);

        padapter.dvobj = dvobj;

        dvobj.padapters[dvobj.iface_nums++] = padapter;

        padapter.hw_port = hw_port.HW_PORT0;

        /* step 2. hook HalFunc, allocate HalData */
        init_hal_spec_8812a(padapter);

        /* step read_chip_version */
        read_chip_version_8812a(padapter);
        rtw_odm_init_ic_type(padapter);

        /* step usb endpoint mapping */
        rtl8812au_interface_configure(padapter);

        /* step read efuse/eeprom data and get mac_addr */
        ReadAdapterInfo8812AU(padapter);

        /* step 5. */
        Init_ODM_ComInfo_8812(padapter);

        return padapter;
    }

    static void init_hal_spec_8812a(_adapter adapter)
    {
        hal_spec_t hal_spec = GET_HAL_SPEC(adapter);

        hal_spec.rfpath_num_2g = 2;
        hal_spec.rfpath_num_5g = 2;
        hal_spec.txgi_max = 63;
        hal_spec.max_tx_cnt = 2;
        hal_spec.band_cap = BAND_CAP_2G | BAND_CAP_5G;
        hal_spec.proto_cap = (byte)(PROTO_CAP_11B | PROTO_CAP_11G | PROTO_CAP_11N | PROTO_CAP_11AC);
        hal_spec.pg_txpwr_saddr = 0x10;
        hal_spec.pg_txgi_diff_factor = 1;
    }

    public static int rtl8812_EFUSE_GetEfuseDefinition(PADAPTER pAdapter, u8 efuseType, EFUSE_DEF_TYPE type)
    {
        switch (type)
        {
            case EFUSE_DEF_TYPE.TYPE_EFUSE_MAP_LEN:
            {

                if (efuseType == EFUSE_WIFI)
                    return EFUSE_MAP_LEN_JAGUAR;
                else
                    throw new NotImplementedException();
            }
                break;
        }

        return 0;
    }

    static void loadparam(_adapter padapter)
    {
        var registry_par = padapter.registrypriv;

        registry_par.channel = 36;
        registry_par.rf_config = rf_type.RF_TYPE_MAX;
        registry_par.wifi_spec = rtw_wifi_spec;
        registry_par.special_rf_path = (u8)0;
        registry_par.TxBBSwing_2G = -1;
        registry_par.TxBBSwing_5G = -1;
        registry_par.RFE_Type = 64;
        registry_par.AmplifierType_2G = 0;
        registry_par.AmplifierType_5G = 0;
    }

    private static dvobj_priv usb_dvobj_init(IRtlUsbDevice usb_intf)
    {
        dvobj_priv pdvobjpriv = new()
        {
            RtNumInPipes = 0,
            RtNumOutPipes = 0,

            nr_endpoint = usb_intf.GetEndpointsCount()
        };

        /* RTW_INFO("\ndump usb_endpoint_descriptor:\n"); */
        int i = 0;
        foreach (var endpoint in usb_intf.GetEndpoints())
        {
            var type = endpoint.Type;
            var direction = endpoint.Direction;

            RTW_INFO("usb_endpoint_descriptor(%d):", i);

            if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.In)
            {
                RTW_INFO("RT_usb_endpoint_is_bulk_in = %x", endpoint.GetUsbEndpointNum());
                pdvobjpriv.RtNumInPipes++;
            }
            else if (direction == RtlEndpointDirection.In)
            {
                pdvobjpriv.RtNumInPipes++;
            }
            else if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.Out)
            {
                RTW_INFO("RT_usb_endpoint_is_bulk_out = %x\n", endpoint.GetUsbEndpointNum());
                pdvobjpriv.RtNumOutPipes++;
            }

            pdvobjpriv.ep_num[i] = endpoint.GetUsbEndpointNum();
        }

        RTW_INFO("nr_endpoint=%d, in_num=%d, out_num=%d\n\n", pdvobjpriv.nr_endpoint, pdvobjpriv.RtNumInPipes,
            pdvobjpriv.RtNumOutPipes);

        switch (usb_intf.Speed)
        {
            case USB_SPEED_LOW:
                RTW_INFO("USB_SPEED_LOW");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_1_1;
                break;
            case USB_SPEED_FULL:
                RTW_INFO("USB_SPEED_FULL");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_1_1;
                break;
            case USB_SPEED_HIGH:
                RTW_INFO("USB_SPEED_HIGH");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_2;
                break;

            case USB_SPEED_SUPER:
                RTW_INFO("USB_SPEED_SUPER");
                pdvobjpriv.usb_speed = RTW_USB_SPEED_3;
                break;

            default:
                RTW_INFO("USB_SPEED_UNKNOWN(%x)", usb_intf.Speed);
                pdvobjpriv.usb_speed = RTW_USB_SPEED_UNKNOWN;
                break;
        }

        if (pdvobjpriv.usb_speed == RTW_USB_SPEED_UNKNOWN)
        {
            RTW_INFO("UNKNOWN USB SPEED MODE, ERROR !!!");
            throw new Exception();
        }

        return pdvobjpriv;
    }

    public static bool rtw_hal_init(_adapter padapter, InitChannel initChannel)
    {
        var status = rtl8812au_hal_init(padapter);

        if (status)
        {
            init_hw_mlme_ext(padapter, initChannel);
            setopmode_hdl(padapter);
        }
        else
        {
            RTW_ERR("rtw_hal_init: fail");
        }

        return status;
    }
}
