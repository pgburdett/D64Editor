using Nemesis.Models;
using System.Text;

namespace Nemesis.IO;

/// <summary>
/// Reads/writes the original Nemesis 640-byte character (.CHR) files.
///
/// The layout was inferred from PERSON.COM / DUNGEON.COM disassembly and by
/// comparing generated character files.  Some regions (notably the level maps
/// and inventory) are still treated as opaque bytes so that files round-trip
/// byte-exactly; structured views are layered on top where confidence is high.
/// </summary>
public static class CharacterFile
{
    public static Character Read(string path) => Read(File.ReadAllBytes(path));

    public static Character Read(byte[] data)
    {
        if (data.Length != Character.FileSize)
            throw new InvalidDataException($"Expected {Character.FileSize} bytes, got {data.Length}");

        var c = new Character { RawData = (byte[])data.Clone() };

        // Offsets inferred from PERSON.COM:
        // 0x00-0x2f: filename strings / scratch buffer
        // 0x37-0x3f: class/race/class-name strings (ASCIIZ)
        // 0x40-0x4f: formatted numeric strings (level, HP, etc.)
        // 0x50-0x5f: 16-bit big-endian attributes + level/gold/exp
        // 0x60-0x9f: binary game state (flags, equipment, dungeon position)
        // 0xa0-0x13f: message buffer / CP/M install script remnants
        // 0x140+: banner screen data (not character-specific)

        // Layout confirmed from ARAGORN.CHR hex dump:
        // 0x12 = character name (ASCIIZ, e.g. "aragorn")
        // 0x27 = race/type (e.g. "Hobbit")
        // 0x31 = class (e.g. "Peasant")
        // 0x2e = alignment/flag byte
        c.Name = ReadAsciiZ(data, 0x12, 8).Trim();
        var typeStr = ReadAsciiZ(data, 0x27, 10).Trim();
        var classStr = ReadAsciiZ(data, 0x31, 8).Trim();
        c.Class = ParseClass(classStr);
        c.Type = ParseType(typeStr);

        c.Alignment = data[0x2e] switch
        {
            (byte)'L' => Alignment.Lawful,
            (byte)'C' => Alignment.Chaotic,
            _ => Alignment.Neutral
        };

        // 16-bit big-endian attribute block at 0x50
        c.BaseAttributes.Strength = ReadBE16(data, 0x50);
        c.BaseAttributes.Intelligence = ReadBE16(data, 0x52);
        c.BaseAttributes.Wisdom = ReadBE16(data, 0x54);
        c.BaseAttributes.Dexterity = ReadBE16(data, 0x56);
        c.BaseAttributes.Constitution = ReadBE16(data, 0x58);
        c.BaseAttributes.Charisma = ReadBE16(data, 0x5a);

        c.Level = ReadBE16(data, 0x5c);
        c.Experience = ReadBE16(data, 0x5e);
        c.Gold = ReadBE16(data, 0x60);
        c.MaxHitPoints = ReadBE16(data, 0x62);
        c.HitPoints = ReadBE16(data, 0x64);
        c.ArmorClass = data[0x66];

        // Copy the opaque state regions
        Array.Copy(data, 0x68, c.Inventory, 0, Math.Min(128, data.Length - 0x68));
        for (int i = 0; i < 11; i++)
            Array.Copy(data, 0x100 + i * 32, c.LevelMaps[i], 0, 32);

        // Current attributes start as copies of base (modified by curses/equipment)
        for (int i = 0; i < 6; i++) c.CurrentAttributes[i] = c.BaseAttributes[i];

        return c;
    }

    public static byte[] Write(Character c)
    {
        var data = c.RawData != null && c.RawData.Length == Character.FileSize
            ? (byte[])c.RawData.Clone()
            : new byte[Character.FileSize];

        WriteAsciiZ(data, 0x12, c.Name.ToUpperInvariant(), 8);
        WriteAsciiZ(data, 0x31, c.Class.ToString().ToUpperInvariant(), 8);
        WriteAsciiZ(data, 0x27, c.Type.ToString().ToUpperInvariant(), 10);
        data[0x2e] = (byte)c.Alignment;

        WriteBE16(data, 0x50, c.BaseAttributes.Strength);
        WriteBE16(data, 0x52, c.BaseAttributes.Intelligence);
        WriteBE16(data, 0x54, c.BaseAttributes.Wisdom);
        WriteBE16(data, 0x56, c.BaseAttributes.Dexterity);
        WriteBE16(data, 0x58, c.BaseAttributes.Constitution);
        WriteBE16(data, 0x5a, c.BaseAttributes.Charisma);
        WriteBE16(data, 0x5c, c.Level);
        WriteBE16(data, 0x5e, c.Experience);
        WriteBE16(data, 0x60, c.Gold);
        WriteBE16(data, 0x62, c.MaxHitPoints);
        WriteBE16(data, 0x64, c.HitPoints);
        data[0x66] = (byte)c.ArmorClass;

        Array.Copy(c.Inventory, 0, data, 0x68, Math.Min(128, c.Inventory.Length));
        for (int i = 0; i < 11; i++)
            Array.Copy(c.LevelMaps[i], 0, data, 0x100 + i * 32, 32);

        return data;
    }

    public static void Save(Character c, string path) => File.WriteAllBytes(path, Write(c));

    private static int ReadBE16(byte[] d, int o) => (d[o] << 8) | d[o + 1];
    private static void WriteBE16(byte[] d, int o, int v)
    {
        d[o] = (byte)(v >> 8); d[o + 1] = (byte)v;
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
        for (int i = 0; i < max; i++) d[off + i] = 0;
        for (int i = 0; i < Math.Min(s.Length, max - 1); i++) d[off + i] = (byte)s[i];
    }

    private static CharacterClass ParseClass(string s) => s.ToUpperInvariant() switch
    {
        "CLERIC" => CharacterClass.Cleric, "FIGHTER" => CharacterClass.Fighter,
        "MAGE" => CharacterClass.Mage, "THIEF" => CharacterClass.Thief,
        "PALADIN" => CharacterClass.Paladin, "RANGER" => CharacterClass.Ranger,
        "DRUID" => CharacterClass.Druid, "MONK" => CharacterClass.Monk,
        "BARD" => CharacterClass.Bard, "BARBARIAN" => CharacterClass.Barbarian,
        "ASSASSIN" => CharacterClass.Assassin, "NECROMANCER" => CharacterClass.Necromancer,
        "WIZARD" => CharacterClass.Wizard, "WARLOCK" => CharacterClass.Warlock,
        _ => CharacterClass.Peasant
    };

    private static CharacterType ParseType(string s) => s.ToUpperInvariant() switch
    {
        "HOBBIT" => CharacterType.Hobbit, "ELF" => CharacterType.Elf,
        "DWARF" => CharacterType.Dwarf, "HALFELF" => CharacterType.HalfElf,
        "HALFORC" => CharacterType.HalfOrc, "GNOLL" => CharacterType.Gnoll,
        "GOBLIN" => CharacterType.Goblin, "HOBGOBLIN" => CharacterType.Hobgoblin,
        "ORC" => CharacterType.Orc, "URUK-HAI" => CharacterType.UrukHai,
        "HALFDWARF" => CharacterType.HalfDwarf, "GREY ELF" => CharacterType.GreyElf,
        "FEATHEROR" => CharacterType.Featheror, "DEMONDIM" => CharacterType.Demondim,
        "MINSTREL" => CharacterType.Minstrel, "VALKYRIE" => CharacterType.Valkyrie,
        "HIREBRAND" => CharacterType.Hirebrand,
        _ => CharacterType.Human
    };
}
