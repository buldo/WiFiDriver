using YetAnotherRemotePcap.Server;

namespace Rtl8812auNet.Grpc;

internal class YarPcapService : YarPcap.YarPcapBase
{
    private readonly WiFiDriver _wiFiDriver;

    public YarPcapService(
        WiFiDriver wiFiDriver
        )
    {
        _wiFiDriver = wiFiDriver;
    }
}