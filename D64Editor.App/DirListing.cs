using D64Editor.Core;

namespace D64Editor.App;

public record DirEntry(int Blocks, string Type, byte[] Name, bool Closed = true, bool Locked = false);

public static class DirListing
{
    public static byte[] Build(byte[] diskName, string id, string dos, IEnumerable<DirEntry> files, int blocksFree)
    {
        var o = new List<byte>();
        void T(string s) => o.AddRange(Petscii.FromText(s.ToUpperInvariant(), lower: false, tags: false));
        int Name(byte[] n)
        {
            int len = Array.IndexOf(n, (byte)0xA0); if (len < 0) len = Math.Min(n.Length, 16);
            o.AddRange(n.AsSpan(0, len).ToArray()); return len;
        }

        T("0 "); o.Add(0x12); o.Add(0x22);
        int dl = Name(diskName); o.AddRange(Enumerable.Repeat((byte)0x20, 16 - dl));
        o.Add(0x22); T($" {id} {dos}"); o.Add(0x0D);

        foreach (var f in files)
        {
            T(f.Blocks.ToString().PadRight(5)); o.Add(0x22);
            int len = Name(f.Name); o.Add(0x22);
            T(new string(' ', 16 - len) + (f.Closed ? " " : "*") + f.Type + (f.Locked ? "<" : ""));
            o.Add(0x0D);
        }
        T($"{blocksFree} BLOCKS FREE."); o.Add(0x0D);
        return o.ToArray();
    }

    // PRG -> LIST-style PETSCII (Petscii.Tokens is public)
    public static byte[] BasicList(ReadOnlySpan<byte> prg)
    {
        var o = new List<byte>();
        int pos = 2;
        while (pos + 4 <= prg.Length)
        {
            int next = prg[pos] | (prg[pos + 1] << 8); if (next == 0) break;
            int line = prg[pos + 2] | (prg[pos + 3] << 8); pos += 4;
            o.AddRange(Petscii.FromText(line + " ", false, false));
            bool q = false;
            while (pos < prg.Length && prg[pos] != 0)
            {
                byte b = prg[pos++];
                if (b == 0x22) q = !q;
                if (!q && b >= 0x80 && b <= 0xCB) o.AddRange(Petscii.FromText(Petscii.Tokens[b - 0x80], false, false));
                else o.Add(b);
            }
            pos++; o.Add(0x0D);
        }
        return o.ToArray();
    }
}