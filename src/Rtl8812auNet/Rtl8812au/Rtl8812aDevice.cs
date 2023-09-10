using System.Net;
using System.Net.Sockets;

using Rtl8812auNet.Abstractions;
using Rtl8812auNet.Rtl8812au.Modules;

namespace Rtl8812auNet.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly RtlUsbAdapter _usbDevice;
    private readonly AdapterState _adapterState;
    private readonly StatefulFrameParser _frameParser = new();
    private readonly UdpClient _client = new UdpClient();
    private readonly IPEndPoint _address = new IPEndPoint(IPAddress.Parse("172.23.97.127"), 4321);
    private readonly RadioManagementModule _radioManagement;

    private Task _readTask;
    private Task _parseTask;
    private readonly HalModule _halModule;

    public Rtl8812aDevice(RtlUsbAdapter usbDevice)
    {
        _usbDevice = usbDevice;
        var powerManagement = new RfPowerManagementModule(_usbDevice);
        _radioManagement = new RadioManagementModule(usbDevice, powerManagement);
        _halModule = new HalModule(_usbDevice, _radioManagement);

        var dvobj = InitDvObj(_usbDevice);
        _adapterState = InitAdapter(dvobj, _usbDevice);
    }

    public void Init()
    {

        StartWithMonitorMode(new()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        SetMonitorChannel(_adapterState.HalData, new()
        {
            cur_bwmode = ChannelWidth.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 140,
            //cur_channel = 36
        });

        _readTask = Task.Run(() => _usbDevice.UsbDevice.InfinityRead());
        _parseTask = Task.Run(() => ParseUsbData());
    }

    private DvObj InitDvObj(RtlUsbAdapter usbInterface)
    {
        u8 numOutPipes = 0;

        foreach (var endpoint in usbInterface.UsbDevice.GetEndpoints())
        {
            var type = endpoint.Type;
            var direction = endpoint.Direction;

            if (type == RtlEndpointType.Bulk && direction == RtlEndpointDirection.Out)
            {
                numOutPipes++;
            }
        }

        var usbSpeed = usbInterface.UsbDevice.Speed switch
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


    private AdapterState InitAdapter(DvObj dvobj, RtlUsbAdapter pusb_intf)
    {
        var adapterState = new AdapterState(dvobj, HwPort.HW_PORT0, pusb_intf);

        /* step read_chip_version */
        read_chip_version_8812a(adapterState);

        /* step usb endpoint mapping */
        rtl8812au_interface_configure(adapterState);

        /* step read efuse/eeprom data and get mac_addr */
        ReadAdapterInfo8812AU(adapterState);

        /* step 5. */
        Init_ODM_ComInfo_8812(adapterState);

        return adapterState;
    }


    private void StartWithMonitorMode(InitChannel initChannel)
    {
        if (NetDevOpen(initChannel) == false)
        {
            throw new Exception("StartWithMonitorMode failed NetDevOpen");
        }

        _radioManagement.setopmode_hdl(_adapterState.HwPort);
    }

    private void SetMonitorChannel(hal_com_data pHalData, InitChannel chandef)
    {
        _radioManagement.set_channel_bwmode(pHalData, chandef.cur_channel, chandef.cur_ch_offset, chandef.cur_bwmode);
    }

    private bool NetDevOpen(InitChannel initChannel)
    {
        var status = _halModule.rtw_hal_init(_adapterState.HalData, _adapterState.HwPort, initChannel);
        if (status == false)
        {
            return false;
        }

        return true;
    }

    private async Task ParseUsbData()
    {
        await foreach (var transfer in _usbDevice.UsbDevice.BulkTransfersReader.ReadAllAsync())
        {
            var packet = _frameParser.ParsedRadioPacket(transfer);
            foreach (var radioPacket in packet)
            {
                //await _client.SendAsync(radioPacket.Data, _address);
                //Console.WriteLine(Convert.ToHexString(radioPacket.Data));
                //Console.WriteLine();
            }
        }
    }
}