using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace WiFiDriver.App.Rtl8812au;

public static class usb_intf
{
    private const byte USB_ENDPOINT_NUMBER_MASK = 0x0f;

    public static dvobj_priv usb_dvobj_init(UsbDevice usb_intf)
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
                pdvobjpriv.RtInPipe[pdvobjpriv.RtNumInPipes] =  RT_usb_endpoint_num(endpoint);
                pdvobjpriv.RtNumInPipes++;
            }
            else if (direction == EndpointDirection.In)
            {
                RTW_INFO("RT_usb_endpoint_is_int_in = %x, Interval = %x\n", RT_usb_endpoint_num(endpoint), endpoint.Interval);
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

        RTW_INFO("nr_endpoint=%d, in_num=%d, out_num=%d\n\n", pdvobjpriv.nr_endpoint, pdvobjpriv.RtNumInPipes, pdvobjpriv.RtNumOutPipes);

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
        pdvobjpriv.interface_type = RTW_USB;
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
}