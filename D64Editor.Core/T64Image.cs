using System.Text;

namespace D64Editor.Core;

/// <summary>One file inside a T64 (.t64) tape archive.</summary>
public sealed class T64Entry
{
    public required string FileName { get; init; }
    public ushort StartAddress { get; init; }
    public ushort EndAddress { get; init; }
    public long DataOffset { get; init; }
    public int Length { get; init; }
}

/// <summary>
///     Read-only reader for the T64 (Commodore tape archive) format, à la DirMaster's
///     support for .t64 containers. Parses the 64-byte header and 32-byte directory
///     records, exposing the contained files (typically PRG data with a 2-byte load
///     address). Best-effort: the format has minor vendor variants, so unknown entries
///     are skipped rather than guessed.
/// </summary>
public sealed class T64Image
{
    public string Description { get; private set; } = "";
    public int MajorVersion { get; private set; }
    public int MinorVersion { get; private set; }
    public int MaxEntries { get; private set; }
    public int UsedEntries { get; private set; }
    public List<T64Entry> Entries { get; } = new();

    private readonly byte[] _data;

    private T64Image(byte[] data) => _data = data;

    public static T64Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }

        if (bytes.Length < 64) return null;
        // Loose magic check: "C64" tape signature.
        if (!(bytes[0] == (byte)'C' && bytes[1] == (byte)'6' && bytes[2] == (byte)'4'))
            return null;

        var img = new T64Image(bytes);
        img.Description = Encoding.ASCII.GetString(bytes, 0, 32).TrimEnd('\0', ' ');
        img.MinorVersion = bytes[32] | (bytes[33] << 8);
        img.MajorVersion = bytes[34] | (bytes[35] << 8);
        img.MaxEntries = bytes[36] | (bytes[37] << 8);
        img.UsedEntries = bytes[38] | (bytes[39] << 8);

        var records = Math.Max(img.MaxEntries, img.UsedEntries);
        for (var i = 0; i < records; i++)
        {
            var rec = 64 + i * 32;
            if (rec + 32 > bytes.Length) break;

            var status = bytes[rec];
            if (status == 0) continue; // free slot

            var start = (ushort)(bytes[rec + 2] | (bytes[rec + 3] << 8));
            var end = (ushort)(bytes[rec + 4] | (bytes[rec + 5] << 8));
            var offset = BitConverter.ToInt32(bytes, rec + 24);
            var name = Encoding.Latin1.GetString(bytes, rec + 8, 16).TrimEnd(' ', '\0', (char)0xA0);

            var length = Math.Max(0, end - start + 1);
            img.Entries.Add(new T64Entry
            {
                FileName = name,
                StartAddress = start,
                EndAddress = end,
                DataOffset = offset,
                Length = length
            });
        }

        return img;
    }

    /// <summary>Extracts the PRG bytes for an entry (2-byte load address + data).</summary>
    public byte[] Extract(T64Entry entry)
    {
        var outBytes = new byte[2 + entry.Length];
        outBytes[0] = (byte)entry.StartAddress;
        outBytes[1] = (byte)(entry.StartAddress >> 8);
        var src = entry.DataOffset;
        var copy = Math.Min(entry.Length, _data.Length - src);
        if (copy > 0)
            Array.Copy(_data, src, outBytes, 2, copy);
        return outBytes;
    }
}
