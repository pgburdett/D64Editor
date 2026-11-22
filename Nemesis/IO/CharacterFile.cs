using Nemesis.Data;
using Nemesis.Models;
using System.Text;

namespace Nemesis.IO;

/// <summary>
/// Reads/writes the original Nemesis 640-byte character (.CHR) files.
///
/// Confirmed from the ARAGORN.CHR sample and the recovered C source:
///   0x12   name (ASCIIZ, lowercase in the sample)
///   0x27   race/type name (ASCIIZ, e.g. "Hobbit")
///   0x31   class name (ASCIIZ, e.g. "Peasant")
///   0x3b   gold as a 10-char zero-filled decimal string
///   0x46   experience as a 10-char zero-filled decimal string
///   0x50   six big-endian 16-bit stats in STR,INT,WIS,CHA,CON,DEX order
///   0x5c   level (BE16)
///   0x5e   age (BE16)
///   0x64   current hit points (BE16)
///   0x66   maximum hit points (BE16)
///   0x61   current magic units (LE16, runtime 0x80a8)
///   0x63   current cleric units (LE16, runtime 0x80aa)
///   0x79   maximum magic units (LE16, runtime 0x80c0)
///   0x7b   maximum cleric units (LE16, runtime 0x80c2)
///   0x89   per-inventory-slot identified bits (LE16 word, runtime 0x80d0)
///   0x8b   per-slot equipped/enchantment-active bits (LE16, runtime 0x80d2)
///   0x93   displayed armor class (signed LE16, runtime 0x80da)
///   0xa3   inventory records, 0x25 bytes each; AC modifier, fight bonus and
///          base price are signed LE16 at record +0, +2 and +4; item type is
///          at +0x20, and 0xff marks an empty slot
///
/// The save buffer starts at runtime 0x8047: NEMESIS.COM loads five 128-byte
/// records into it and CASTLE.COM/DUNGEON.COM write the same buffer back on
/// exit.  All other regions are kept verbatim in RawData so that the file always
/// round-trips byte-exactly, even where the original layout is still opaque.
/// </summary>
public static class CharacterFile
{
    private const int NameOffset = 0x12;
    private const int TypeOffset = 0x27;
    private const int ClassOffset = 0x31;
    private const int GoldOffset = 0x3b;
    private const int ExpOffset = 0x46;
    private const int StatOffset = 0x50;
    private const int MagicUnitsOffset = 0x61;
    private const int ClericUnitsOffset = 0x63;
    private const int MaxMagicUnitsOffset = 0x79;
    private const int MaxClericUnitsOffset = 0x7b;
    private const int IdentifiedOffset = 0x89;
    private const int EquippedOffset = 0x8b;
    private const int ArmorClassOffset = 0x93;
    private const int ItemSlotOffset = 0xa3;
    private const int ItemSlotSize = 0x25;
    private const int ItemSlotAcOffset = 0;
    private const int ItemSlotFOffset = 2;
    private const int ItemSlotPriceOffset = 4;
    private const int ItemSlotTypeOffset = 0x20;
    private const int NumericWidth = 10;

    public static Character Read(string path) => Read(File.ReadAllBytes(path));

    public static Character Read(byte[] data)
    {
        if (data.Length != Character.FileSize)
            throw new InvalidDataException($"Expected {Character.FileSize} bytes, got {data.Length}");

        var c = new Character { RawData = (byte[])data.Clone() };

        c.Name = ReadAsciiZ(data, NameOffset, 12).Trim();
        c.Type = ParseType(ReadAsciiZ(data, TypeOffset, 10));
        c.Class = ParseClass(ReadAsciiZ(data, ClassOffset, 8));
        c.DungeonName = c.Name;

        c.Gold = ParsePaddedInt(data, GoldOffset);
        c.Experience = ParsePaddedInt(data, ExpOffset);

        for (int i = 0; i < 6; i++) c.BaseAttributes[i] = ReadBE16(data, StatOffset + i * 2);
        c.CurrentAttributes = c.BaseAttributes.Clone();

        c.Level = ReadBE16(data, 0x5c);
        c.Age = ReadBE16(data, 0x5e);
        c.HitPoints = ReadBE16(data, 0x64);
        c.MaxHitPoints = ReadBE16(data, 0x66);

        // CASTLE.COM 0x1fce/0x1fe8 display 0x80a8/0x80aa, and the Inn copies
        // 0x80c0/0x80c2 into them.  Subtracting the 0x8047 save-buffer base
        // gives these four little-endian .CHR offsets.  0xffff means unlimited.
        c.MagicUnits = ReadLE16(data, MagicUnitsOffset);
        c.ClericUnits = ReadLE16(data, ClericUnitsOffset);
        c.MaxMagicUnits = ReadLE16(data, MaxMagicUnitsOffset);
        c.MaxClericUnits = ReadLE16(data, MaxClericUnitsOffset);
        c.ArmorClass = ReadLE16Signed(data, ArmorClassOffset);

        // 0x80d0 and 0x80d2 are inside the 640-byte save buffer at 0x8047.
        // Both Z80 words are little endian and bit N is inventory slot N.
        ushort identified = (ushort)(data[IdentifiedOffset] |
                                     (data[IdentifiedOffset + 1] << 8));
        ushort equipped = (ushort)(data[EquippedOffset] |
                                   (data[EquippedOffset + 1] << 8));
        for (int i = 0; i < Character.MaxItems; i++)
        {
            c.ItemIdentified[i] = (identified & (1 << i)) != 0;
            c.ItemEquipped[i] = (equipped & (1 << i)) != 0;
        }

        // The expanded inventory records start at 0x80ea.  Records +0, +2 and
        // +4 are signed little-endian AC modifier, fight bonus and base price.
        // The catalog-copy routine at DUNGEON.COM 0x3253 writes these words
        // low-byte first, and the item-effect routines read them the same way.
        // Record +0x20 is the item type and 0xff marks an empty slot.
        for (int slot = 0; slot < Character.MaxItems; slot++)
        {
            int record = ItemSlotOffset + slot * ItemSlotSize;
            c.ItemAcModifier[slot] = ReadLE16Signed(data, record + ItemSlotAcOffset);
            c.ItemFModifier[slot] = ReadLE16Signed(data, record + ItemSlotFOffset);
            c.ItemPrice[slot] = ReadLE16Signed(data, record + ItemSlotPriceOffset);
            c.ItemTypes[slot] = data[record + ItemSlotTypeOffset];
        }

        return c;
    }

    public static byte[] Write(Character c)
    {
        var data = c.RawData != null && c.RawData.Length == Character.FileSize
            ? (byte[])c.RawData.Clone()
            : new byte[Character.FileSize];

        WriteAsciiZ(data, NameOffset, c.Name, 12);
        WriteAsciiZ(data, TypeOffset, GameTables.Races[(int)c.Type].Name, 10);
        WriteAsciiZ(data, ClassOffset, GameTables.Classes[(int)c.Class].Name, 8);
        WritePaddedInt(data, GoldOffset, c.Gold);
        WritePaddedInt(data, ExpOffset, c.Experience);

        for (int i = 0; i < 6; i++) WriteBE16(data, StatOffset + i * 2, c.BaseAttributes[i]);
        WriteBE16(data, 0x5c, c.Level);
        WriteBE16(data, 0x5e, c.Age);
        WriteBE16(data, 0x64, c.HitPoints);
        WriteBE16(data, 0x66, c.MaxHitPoints);
        WriteLE16(data, MagicUnitsOffset, c.MagicUnits);
        WriteLE16(data, ClericUnitsOffset, c.ClericUnits);
        WriteLE16(data, MaxMagicUnitsOffset, c.MaxMagicUnits);
        WriteLE16(data, MaxClericUnitsOffset, c.MaxClericUnits);
        WriteLE16(data, ArmorClassOffset, c.ArmorClass);

        ushort identified = 0;
        ushort equipped = 0;
        for (int i = 0; i < Character.MaxItems; i++)
        {
            if (i < c.ItemIdentified.Length && c.ItemIdentified[i]) identified |= (ushort)(1 << i);
            if (i < c.ItemEquipped.Length && c.ItemEquipped[i]) equipped |= (ushort)(1 << i);
        }
        data[IdentifiedOffset] = (byte)identified;
        data[IdentifiedOffset + 1] = (byte)(identified >> 8);
        data[EquippedOffset] = (byte)equipped;
        data[EquippedOffset + 1] = (byte)(equipped >> 8);

        for (int slot = 0; slot < Character.MaxItems && slot < c.ItemTypes.Length; slot++)
        {
            int record = ItemSlotOffset + slot * ItemSlotSize;
            if (slot < c.ItemAcModifier.Length)
                WriteLE16(data, record + ItemSlotAcOffset, c.ItemAcModifier[slot]);
            if (slot < c.ItemFModifier.Length)
                WriteLE16(data, record + ItemSlotFOffset, c.ItemFModifier[slot]);
            if (slot < c.ItemPrice.Length)
                WriteLE16(data, record + ItemSlotPriceOffset, c.ItemPrice[slot]);
            data[record + ItemSlotTypeOffset] = c.ItemTypes[slot];
        }

        return data;
    }

    public static void Save(Character c, string path) => File.WriteAllBytes(path, Write(c));

    private static int ReadBE16(byte[] d, int o) => (d[o] << 8) | d[o + 1];

    private static int ReadLE16(byte[] d, int o) => d[o] | (d[o + 1] << 8);

    private static int ReadLE16Signed(byte[] d, int o)
    {
        int v = ReadLE16(d, o);
        return v >= 0x8000 ? v - 0x10000 : v;
    }

    private static void WriteBE16(byte[] d, int o, int v)
    {
        d[o] = (byte)(v >> 8);
        d[o + 1] = (byte)v;
    }

    private static void WriteLE16(byte[] d, int o, int v)
    {
        d[o] = (byte)v;
        d[o + 1] = (byte)(v >> 8);
    }

    private static int ParsePaddedInt(byte[] d, int off)
    {
        int start = off, end = off + NumericWidth;
        while (start < end && d[start] == '0') start++;
        if (start == end) return 0;
        var sb = new StringBuilder();
        for (int i = start; i < end && d[i] >= '0' && d[i] <= '9'; i++) sb.Append((char)d[i]);
        return sb.Length == 0 ? 0 : int.Parse(sb.ToString());
    }

    private static void WritePaddedInt(byte[] d, int off, int value)
    {
        string s = Math.Clamp(value, 0, 99_999_999).ToString();
        s = s.PadLeft(NumericWidth, '0');
        for (int i = 0; i < NumericWidth; i++) d[off + i] = (byte)s[i];
    }

    private static string ReadAsciiZ(byte[] d, int off, int max)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < max && off + i < d.Length && d[off + i] != 0; i++)
            sb.Append((char)d[off + i]);
        return sb.ToString();
    }

    private static void WriteAsciiZ(byte[] d, int off, string s, int max)
    {
        // Only write the string and its terminator.  Zeroing the whole field
        // would destroy the stale bytes that follow the NUL in original files.
        int len = Math.Min(s.Length, max - 1);
        for (int i = 0; i < len; i++) d[off + i] = (byte)s[i];
        d[off + len] = 0;
    }

    private static CharacterClass ParseClass(string s)
    {
        for (int i = 0; i < GameTables.Classes.Length; i++)
            if (string.Equals(GameTables.Classes[i].Name, s, StringComparison.OrdinalIgnoreCase))
                return (CharacterClass)i;
        return CharacterClass.Peasant;
    }

    private static CharacterType ParseType(string s)
    {
        for (int i = 0; i < GameTables.Races.Length; i++)
            if (string.Equals(GameTables.Races[i].Name, s, StringComparison.OrdinalIgnoreCase))
                return (CharacterType)i;
        return CharacterType.Human;
    }
}
