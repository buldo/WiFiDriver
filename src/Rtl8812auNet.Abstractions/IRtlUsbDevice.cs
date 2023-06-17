namespace Rtl8812auNet.Abstractions;

public interface IRtlUsbDevice
{
    void InfinityRead();
    int Speed { get; }

    void WriteBytes(ushort register, Span<byte> data);

    ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount);

    public List<IRtlEndpoint> GetEndpoints();
}