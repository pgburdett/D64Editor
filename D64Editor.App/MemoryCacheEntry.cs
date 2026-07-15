using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// One staged file waiting to be copied into a disk image. Ported from
/// frmMemory's parallel arrays (memoryFilename, memoryCache, memoryFileType,
/// memoryFileLength, memorydisplayFilename) — one object instead of five
/// arrays indexed by the same position.
/// </summary>
public class MemoryCacheEntry
{
    public required string DisplayName { get; init; }
    public required byte[] Data { get; init; }
    public D64FileType FileType { get; init; } = D64FileType.Prg;

    public override string ToString() => $"{DisplayName}  ({Data.Length:N0} bytes, {FileType.ToDisplayCode()})";
}
