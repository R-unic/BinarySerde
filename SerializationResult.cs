namespace BinarySerde;

public class SerializationResult
{
    public required Roblox.Buffer Buf { get; init; }
    public required List<Instance> Blobs { get; init; }
}