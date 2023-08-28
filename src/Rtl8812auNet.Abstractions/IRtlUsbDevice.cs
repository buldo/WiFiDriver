using System.Threading.Channels;

namespace Rtl8812auNet.Abstractions;

public interface IRtlUsbDevice
{
    int Speed { get; }

    public ChannelReader<byte[]> BulkTransfersReader { get; }

    void InfinityRead();

    void WriteBytes(ushort register, Span<byte> data);

    ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount);

    public List<IRtlEndpoint> GetEndpoints();
}