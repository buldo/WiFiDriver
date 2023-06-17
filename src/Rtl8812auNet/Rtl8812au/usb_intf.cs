using Rtl8812auNet.Abstractions;

namespace Rtl8812auNet.Rtl8812au;

public static class usb_intf
{
    public static AdapterState InitAdapter(DvObj dvobj, IRtlUsbDevice pusb_intf)
    {
        var adapterState = new AdapterState(dvobj, HwPort.HW_PORT0, pusb_intf);

        /* step read_chip_version */
        read_chip_version_8812a(adapterState);
        rtw_odm_init_ic_type(adapterState);

        /* step usb endpoint mapping */
        rtl8812au_interface_configure(adapterState);

        /* step read efuse/eeprom data and get mac_addr */
        ReadAdapterInfo8812AU(adapterState);

        /* step 5. */
        Init_ODM_ComInfo_8812(adapterState);

        return adapterState;
    }

    public static DvObj InitDvObj(IRtlUsbDevice usbInterface)
    {
        u8 numOutPipes = 0;

        foreach (var endpoint in usbInterface.GetEndpoints())
        {
            var type = endpoint.Type;
            var direction = endpoint.Direction;

            if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.Out)
            {
                numOutPipes++;
            }
        }

        var usbSpeed = usbInterface.Speed switch
        {
            USB_SPEED_LOW => RTW_USB_SPEED_1_1,
            USB_SPEED_FULL => RTW_USB_SPEED_1_1,
            USB_SPEED_HIGH => RTW_USB_SPEED_2,
            USB_SPEED_SUPER => RTW_USB_SPEED_3,
            _ => RTW_USB_SPEED_UNKNOWN
        };

        if (usbSpeed == RTW_USB_SPEED_UNKNOWN)
        {
            RTW_INFO("UNKNOWN USB SPEED MODE, ERROR !!!");
            throw new Exception();
        }

        return new DvObj(numOutPipes, usbSpeed);
    }
}
