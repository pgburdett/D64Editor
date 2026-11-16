using System.Text;

namespace D64Editor.Core;

/// <summary>One file inside a T64 (.t64) tape archive.</summary>
public sealed class T64Entry
{
    public required string FileName { get; set; }
    public ushort StartAddress { get; set; }
    public ushort EndAddress { get; set; }
    public long DataOffset { get; internal set; }
    public int Length { get; set; }
    /// <summary>Raw file data payload (without 2-byte load address), for mutable/editable images.</summary>
    public byte[]? Data { get; set; }
}

/// <summary>
///     Reader/writer for the T64 (Commodore tape archive) format. Parses the 64-byte header
///     and 32-byte directory records and can rebuild/save the archive (native read/write).
/// </summary>
public sealed class T64Image
{
    public string Description { get; set; } = "";
    public int MajorVersion { get; set; } = 1;
    public int MinorVersion { get; set; }
    public int MaxEntries { get; private set; }
    public int UsedEntries => Entries.Count;
    public List<T64Entry> Entries { get; } = new();

    public string? SourcePath { get; private set; }
    private byte[] _sourceData = Array.Empty<byte>();

    private T64Image() { }

    private T64Image(byte[] data)
    {
        _sourceData = data;
    }

    public static T64Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }

        if (bytes.Length < 64) return null;
        if (!(bytes[0] == (byte)'C' && bytes[1] == (byte)'6' && bytes[2] == (byte)'4'))
            return null;

        var img = new T64Image(bytes) { SourcePath = path };
        img.Description = Encoding.ASCII.GetString(bytes, 0, 32).TrimEnd('\0', ' ');
        img.MinorVersion = bytes[32] | (bytes[33] << 8);
        img.MajorVersion = bytes[34] | (bytes[35] << 8);
        img.MaxEntries = bytes[36] | (bytes[37] << 8);
        // keep raw used count from header for reference; actual list built below

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

        img.MaxEntries = Math.Max(img.MaxEntries, img.Entries.Count);
        return img;
    }

    public static T64Image Create(string description = "")
    {
        return new T64Image { Description = description, MajorVersion = 1, MinorVersion = 0, MaxEntries = 0 };
    }

    /// <summary>Extracts the PRG bytes for an entry (2-byte load address + data).</summary>
    public byte[] Extract(T64Entry entry)
    {
        if (entry.Data is not null)
        {
            var outBytes = new byte[2 + entry.Data.Length];
            outBytes[0] = (byte)entry.StartAddress;
            outBytes[1] = (byte)(entry.StartAddress >> 8);
            Array.Copy(entry.Data, 0, outBytes, 2, entry.Data.Length);
            return outBytes;
        }

        var len = entry.Length;
        var outBuf = new byte[2 + len];
        outBuf[0] = (byte)entry.StartAddress;
        outBuf[1] = (byte)(entry.StartAddress >> 8);
        var src = entry.DataOffset;
        var copy = (int)Math.Min(len, Math.Max(0, _sourceData.Length - src));
        if (copy > 0)
            Array.Copy(_sourceData, src, outBuf, 2, copy);
        return outBuf;
    }

    /// <summary>Returns raw payload bytes (without load address) for an entry.</summary>
    public byte[] ExtractPayload(T64Entry entry)
    {
        if (entry.Data is not null) return (byte[])entry.Data.Clone();
        var len = Math.Max(0, entry.Length);
        var buf = new byte[len];
        var src = entry.DataOffset;
        var copy = (int)Math.Min(len, Math.Max(0, _sourceData.Length - src));
        if (copy > 0) Array.Copy(_sourceData, src, buf, 0, copy);
        return buf;
    }

    /// <summary>Adds or replaces file data (PRG with 2-byte load address, or raw if loadAddress provided).</summary>
    public void AddFile(string name, byte[] fileBytes, ushort? loadAddress = null)
    {
        ushort start;
        byte[] payload;
        if (loadAddress is not null)
        {
            start = loadAddress.Value;
            payload = (byte[])fileBytes.Clone();
        }
        else if (fileBytes.Length >= 2)
        {
            start = (ushort)(fileBytes[0] | (fileBytes[1] << 8));
            payload = fileBytes.Length > 2 ? fileBytes[2..] : Array.Empty<byte>();
        }
        else
        {
            start = 0;
            payload = (byte[])fileBytes.Clone();
        }

        var end = (ushort)(start + Math.Max(0, payload.Length - 1));
        Entries.Add(new T64Entry
        {
            FileName = name.Length > 16 ? name[..16] : name,
            StartAddress = start,
            EndAddress = end,
            Length = payload.Length,
            Data = payload,
            DataOffset = 0
        });
        MaxEntries = Math.Max(MaxEntries, Entries.Count);
    }

    /// <summary>Saves/rebuilds the T64 archive to disk.</summary>
    public void Save(string? path = null)
    {
        path ??= SourcePath ?? throw new InvalidOperationException("No path specified for T64 save.");
        var dirCount = Math.Max(Entries.Count, 1);
        MaxEntries = Math.Max(MaxEntries, dirCount);
        var headerSize = 64 + MaxEntries * 32;

        // Build payloads
        var payloads = new List<byte[]>(Entries.Count);
        foreach (var e in Entries) payloads.Add(ExtractPayload(e));

        var totalSize = headerSize + payloads.Sum(p => p.Length);
        var outBuf = new byte[totalSize];

        // Header (tape signature)
        var sig = Encoding.ASCII.GetBytes("C64 tape image file");
        Array.Copy(sig, 0, outBuf, 0, Math.Min(sig.Length, 32));
        // Description/user area bytes 0..31 - keep description in tail of first 32 if desired; many tools expect signature only.
        // Better preserve signature string as-is; store description separately not standardized. Keep "C64" prefix.
        // (leave as signature)
        Write16(outBuf, 32, (ushort)MinorVersion);
        Write16(outBuf, 34, (ushort)MajorVersion);
        Write16(outBuf, 36, (ushort)MaxEntries);
        Write16(outBuf, 38, (ushort)Entries.Count);

        // Directory records
        var dataOff = headerSize;
        for (var i = 0; i < Entries.Count; i++)
        {
            var rec = 64 + i * 32;
            var e = Entries[i];
            var payload = payloads[i];
            outBuf[rec] = 1; // normal file
            outBuf[rec + 1] = 0x82; // PRG-like
            Write16(outBuf, rec + 2, e.StartAddress);
            var end = (ushort)(e.StartAddress + Math.Max(0, payload.Length - 1));
            Write16(outBuf, rec + 4, end);
            // reserved rec+6..7
            Write32(outBuf, rec + 8 + 16, dataOff); // offset at rec+24
            // name at rec+8 (16 bytes)
            var nameBytes = Encoding.ASCII.GetBytes(e.FileName ?? "");
            for (var n = 0; n < 16; n++) outBuf[rec + 8 + n] = n < nameBytes.Length ? nameBytes[n] : (byte)0x20;

            // copy data
            if (payload.Length > 0)
                Array.Copy(payload, 0, outBuf, dataOff, payload.Length);
            e.DataOffset = dataOff;
            e.Length = payload.Length;
            e.EndAddress = end;
            dataOff += payload.Length;
        }

        File.WriteAllBytes(path, outBuf);
        SourcePath = path;
        _sourceData = outBuf;
    }

    private static void Write16(byte[] buf, int off, ushort v) { buf[off] = (byte)(v & 0xFF); buf[off + 1] = (byte)(v >> 8); }
    private static void Write32(byte[] buf, int off, int v) { BitConverter.GetBytes(v).CopyTo(buf, off); }
}
