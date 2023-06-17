﻿using System.Threading.Channels;

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace WiFiDriver.App.Rtl8812au;

public class Rtl8812aDevice
{
    private readonly UsbDevice _usbDevice;
    private readonly _adapter _adapter;
    private readonly UsbEndpointReader _reader;
    private Task _readTask;

    public Rtl8812aDevice(UsbDevice usbDevice)
    {
        _usbDevice = usbDevice;
        _usbDevice.Open();
        _usbDevice.SetConfiguration(1);
        _usbDevice.ClaimInterface(0);
        _adapter = usb_intf.rtw_drv_init(_usbDevice);

        _reader = _usbDevice.OpenEndpointReader(GetInEp());
    }

    public void Init()
    {

        ioctl_cfg80211.cfg80211_rtw_change_iface(_adapter, new InitChannel()
        {
            cur_bwmode = channel_width.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        ioctl_cfg80211.cfg80211_rtw_set_monitor_channel(_adapter, new InitChannel()
        {
            cur_bwmode = channel_width.CHANNEL_WIDTH_20,
            cur_ch_offset = 0,
            cur_channel = 8
        });

        _readTask = Task.Run(() => InfinityRead());
    }

    private ReadEndpointID GetInEp()
    {
        foreach (var endpoint in _usbDevice.Configs[0].Interfaces[0].Endpoints)
        {
            var type = (EndpointType)(endpoint.Attributes & 0x3);
            var direction = (EndpointDirection)(endpoint.EndpointAddress & (0b1000_0000));
            if (type == EndpointType.Bulk && direction == EndpointDirection.In)
            {

                return (ReadEndpointID)endpoint.EndpointAddress;
            }
        }

        throw new Exception("Read EP not found");
    }

    private void InfinityRead()
    {
        var readBuffer = new byte[8192 + 1024];
        while (true)
        {
            try
            {
                var result = _reader.Read(readBuffer, 5000, out var len);
                if (result == Error.NotFound)
                {

                }
                else if (result != Error.Success)
                {
                    Console.WriteLine($"BULK read ERR {result}");
                }

                if (len != 0)
                {
                    Console.WriteLine($"BULK read OK {len}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}