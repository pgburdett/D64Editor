using Nemesis.IO;
using Nemesis.Models;
using Xunit;

namespace Nemesis.Tests;

public sealed class CharacterFileTests
{
    [Fact]
    public void Aragorn_RoundTrips_ByteExactly()
    {
        byte[] original = File.ReadAllBytes(RepoData.GameFile("ARAGORN.CHR"));
        var character = CharacterFile.Read(original);

        Assert.Equal(original, CharacterFile.Write(character));
    }

    [Fact]
    public void Aragorn_Decodes_Identity_And_Stats()
    {
        var c = CharacterFile.Read(RepoData.GameFile("ARAGORN.CHR"));

        Assert.Equal("aragorn", c.Name);
        Assert.Equal(CharacterType.Hobbit, c.Type);
        Assert.Equal(CharacterClass.Peasant, c.Class);
        Assert.Equal("Hobbit", c.TypeName);
        Assert.Equal("Peasant", c.ClassName);

        Assert.Equal(12, c.BaseAttributes.Strength);
        Assert.Equal(10, c.BaseAttributes.Intelligence);
        Assert.Equal(11, c.BaseAttributes.Wisdom);
        Assert.Equal(12, c.BaseAttributes.Charisma);
        Assert.Equal(8, c.BaseAttributes.Constitution);
        Assert.Equal(15, c.BaseAttributes.Dexterity);
    }

    [Fact]
    public void Aragorn_Decodes_PaddedDecimal_Gold_And_Experience()
    {
        var c = CharacterFile.Read(RepoData.GameFile("ARAGORN.CHR"));

        Assert.Equal(117, c.Gold);
        Assert.Equal(0, c.Experience);
    }

    [Fact]
    public void IdentifiedBits_And_ItemTypes_Use_Save_Buffer_Offsets()
    {
        byte[] original = File.ReadAllBytes(RepoData.GameFile("ARAGORN.CHR"));
        var c = CharacterFile.Read(original);

        Assert.DoesNotContain(true, c.ItemIdentified);
        Assert.All(c.ItemTypes, t => Assert.Equal((byte)0xff, t));

        c.ItemIdentified[0] = true;   // low byte of the little-endian word
        c.ItemIdentified[9] = true;   // high byte, bit 1
        c.ItemTypes[0] = 42;

        byte[] bytes = CharacterFile.Write(c);
        Assert.Equal(0x01, bytes[0x89]);
        Assert.Equal(0x02, bytes[0x8a]);
        Assert.Equal((byte)42, bytes[0xa3 + 0x20]);

        var roundTrip = CharacterFile.Read(bytes);
        Assert.True(roundTrip.ItemIdentified[0]);
        Assert.True(roundTrip.ItemIdentified[9]);
        Assert.False(roundTrip.ItemIdentified[1]);
        Assert.Equal((byte)42, roundTrip.ItemTypes[0]);
    }

    [Fact]
    public void GeneratedCharacter_Also_RoundTrips_ByteExactly()
    {
        var c = new Character
        {
            Name = "TESTCHAR",
            Type = CharacterType.Elf,
            Class = CharacterClass.Mage,
            Gold = 12345,
            Experience = 6789,
            Level = 3,
            Age = 42,
            HitPoints = 17,
            MaxHitPoints = 20,
            MagicUnits = 0xffff,
        };
        c.BaseAttributes.Strength = 11;
        c.BaseAttributes.Intelligence = 18;
        c.BaseAttributes.Wisdom = 9;
        c.BaseAttributes.Charisma = 13;
        c.BaseAttributes.Constitution = 7;
        c.BaseAttributes.Dexterity = 14;

        byte[] bytes = CharacterFile.Write(c);
        Assert.Equal(Character.FileSize, bytes.Length);
        Assert.Equal(bytes, CharacterFile.Write(CharacterFile.Read(bytes)));
        Assert.Equal(c.Gold, CharacterFile.Read(bytes).Gold);
        Assert.Equal(c.Name, CharacterFile.Read(bytes).Name);
        Assert.Equal(c.BaseAttributes.Intelligence, CharacterFile.Read(bytes).BaseAttributes.Intelligence);
    }

    [Fact]
    public void Rejects_Wrong_File_Length()
    {
        Assert.Throws<InvalidDataException>(() => CharacterFile.Read(new byte[639]));
        Assert.Throws<InvalidDataException>(() => CharacterFile.Read(new byte[641]));
    }
}
