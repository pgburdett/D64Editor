using System.Text;

namespace D64Editor.Core;

/// <summary>A CP/M file aggregated from its directory extents (name.ext, user area).</summary>
public sealed class CpmFile
{
    public required string Name { get; init; }
    public required string Extension { get; init; }
    public int User { get; init; }
    public bool Deleted { get; init; }
    public int Records { get; init; }          // total 128-byte records across extents
    public int Bytes => Records * 128;
}

/// <summary>
///     Best-effort reader for CP/M 2.2 raw disk images and .LBR members, which store files in a
///     32-byte directory-entry table (user# 0-15, 8-char name, 3-char ext, per-extent record
///     counts). CP/M file systems are parameterised by a DPB that isn't in the image, so this
///     lists/labels files by scanning every 32-byte slot for valid entries and grouping extents —
///     it never assumes byte counts, and corrupt/foreign data simply won't match the pattern.
/// </summary>
public static class CpmDisk
{
    private static bool IsNameChar(byte b) =>
        b == 0x20 ||
        (b >= (byte)'A' && b <= (byte)'Z') ||
        (b >= (byte)'0' && b <= (byte)'9') ||
        b is (byte)'!' or (byte)'#' or (byte)'$' or (byte)'%' or (byte)'&' or (byte)'\'' or
           (byte)'(' or (byte)')' or (byte)'-' or (byte)'@' or (byte)'^' or (byte)'_' or
           (byte)'`' or (byte)'{' or (byte)'}' or (byte)'~';

    private static bool LooksLikeEntry(byte[] data, int off)
    {
        var user = data[off];
        if (user > 15 && user != 0xE5) return false;

        // Name: 8 bytes, must start with a letter (CP/M requires it) — space-padded.
        if (data[off + 1] < 'A' || data[off + 1] > 'Z') return false;
        for (var i = 1; i < 9; i++)
            if (!IsNameChar(data[off + i])) return false;
        for (var i = 9; i < 12; i++)
            if (!IsNameChar(data[off + i])) return false;

        return true;
    }

    /// <summary>Scans a raw CP/M image (or .LBR catalog area) for directory entries.</summary>
    public static List<CpmFile> Read(string path)
    {
        byte[] data;
        try { data = File.ReadAllBytes(path); }
        catch { return new List<CpmFile>(); }

        return Scan(data);
    }

    public static List<CpmFile> Scan(byte[] data)
    {
        var files = new List<CpmFile>();
        if (data.Length < 32) return files;

        // Track (user,name,ext) -> accumulated record count + deleted flag.
        var byKey = new Dictionary<(int, string, string), (bool deleted, int records)>( );
        var order = new List<(int, string, string)>();

        for (var off = 0; off + 32 <= data.Length; off += 32)
        {
            if (!LooksLikeEntry(data, off)) continue;

            var user = data[off];
            var deleted = user == 0xE5;
            var userNum = deleted ? 0 : user;

            var name = Encoding.ASCII.GetString(data, off + 1, 8).TrimEnd(' ');
            var ext = Encoding.ASCII.GetString(data, off + 9, 3).TrimEnd(' ');

            // Record count: byte 15 (RC) is the number of 128-byte records in this extent (0-128).
            // (CP/M 2.2 directory entry: 0=user, 1-8=name, 9-11=ext, 12=EX, 13=S1, 14=S2, 15=RC)
            var rc = (int)data[off + 15];
            if (rc > 128) rc = 128;

            // Require either a record count or a partially-filled allocation map,
            // so stray zeroed directories don't produce phantom files.
            var allocationSeen = false;
            for (var i = 16; i < 32; i++)
                if (data[off + i] != 0x00 && data[off + i] != 0xE5) { allocationSeen = true; break; }
            if (rc == 0 && !allocationSeen) continue;

            var key = (userNum, name, ext);
            if (byKey.TryGetValue(key, out var cur))
                byKey[key] = (cur.deleted || deleted, cur.records + rc);
            else
            {
                byKey[key] = (deleted, rc);
                order.Add(key);
            }
        }

        foreach (var key in order)
        {
            var v = byKey[key];
            var (name, ext) = (key.Item2, key.Item3);
            files.Add(new CpmFile
            {
                Name = name,
                Extension = ext,
                User = key.Item1,
                Deleted = v.deleted,
                Records = v.records
            });
        }

        return files;
    }
}
