namespace D64Editor.Core;

/// <summary>
///     Decompressor for Commodore preservation-project .nbz files. An .nbz is a Marcus
///     Geelnard LZ77-compressed MNIB raw NIB image (magic "MNIB-1541-RAW"); decompressing
///     yields a standard .nib that <see cref="NibImage"/> already knows how to decode.
/// </summary>
public static class Nbz
{
    private const int MaxOutput = 16 * 1024 * 1024;

    private static int ReadVarSize(byte[] input, ref int pos)
    {
        long val = 0;
        while (true)
        {
            if (pos >= input.Length) throw new InvalidDataException("Truncated LZ77 varint.");
            var b = input[pos++];
            val = (val << 7) | (uint)(b & 0x7F);
            if (val > MaxOutput) throw new InvalidDataException("LZ77 varint overflow.");
            if ((b & 0x80) == 0) return (int)val;
        }
    }

    public static byte[] Decompress(byte[] input)
    {
        if (input.Length == 0) throw new InvalidDataException("Empty NBZ stream.");
        var marker = input[0];
        var pos = 1;
        var output = new List<byte>(input.Length * 2);

        while (pos < input.Length)
        {
            var symbol = input[pos++];
            if (symbol != marker)
            {
                if (output.Count >= MaxOutput) throw new InvalidDataException("NBZ output limit exceeded.");
                output.Add(symbol);
                continue;
            }
            if (pos >= input.Length) throw new InvalidDataException("Truncated LZ77 marker sequence.");
            if (input[pos] == 0)
            {
                pos++;
                if (output.Count >= MaxOutput) throw new InvalidDataException("NBZ output limit exceeded.");
                output.Add(marker);
                continue;
            }
            var length = ReadVarSize(input, ref pos);
            var offset = ReadVarSize(input, ref pos);
            if (offset == 0 || offset > output.Count)
                throw new InvalidDataException($"Invalid LZ77 reference offset {offset} at output {output.Count}.");
            if (output.Count + length > MaxOutput) throw new InvalidDataException("NBZ output limit exceeded.");
            for (var i = 0; i < length; i++)
                output.Add(output[output.Count - offset]);
        }
        return output.ToArray();
    }
}
