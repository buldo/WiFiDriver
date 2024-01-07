namespace Rtl8812auNet.Abstractions;

public delegate void BulkDataHandler(ReadOnlySpan<byte> dataHandler);

public interface IRtlUsbDevice
{
    int Speed { get; }

    void SetBulkDataHandler(BulkDataHandler handler);

    void InfinityRead();

    void WriteBytes(ushort register, Span<byte> data);

    ReadOnlySpan<byte> ReadBytes(ushort register, ushort bytesCount);

    List<IRtlEndpoint> GetEndpoints();
}