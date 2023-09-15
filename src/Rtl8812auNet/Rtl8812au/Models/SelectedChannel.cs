using Rtl8812auNet.Rtl8812au.Enumerations;

namespace Rtl8812auNet.Rtl8812au.Models;

public class SelectedChannel
{
    public required byte Channel { get; init; }
    public required byte ChannelOffset { get; init; }
    public required ChannelWidth ChannelWidth { get; init; }
}