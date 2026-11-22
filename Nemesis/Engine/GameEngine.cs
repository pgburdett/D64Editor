using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// High-level game state machine.  Mirrors the original overlay structure:
///   NEMESIS  -> launcher / character selection
///   PERSON   -> character generation
///   CASTLE   -> town menu (shopping, hospital, archives, dungeon entrance)
///   DUNGEON  -> exploration / combat
/// </summary>
public sealed class GameEngine
{
    public enum Screen { Launcher, CharacterGen, Castle, Dungeon, GameOver }

    public Screen CurrentScreen { get; private set; } = Screen.Launcher;
    public Character? Character { get; private set; }
    public string? Message { get; set; }

    private readonly Random _rng = new();

    public void LoadCharacter(Character c)
    {
        Character = c;
        CurrentScreen = Screen.Castle;
        Message = $"Welcome to the castle, {c.Name}.";
    }

    public void NewCharacter(string name, Alignment alignment, CharacterClass klass, CharacterType type)
    {
        var c = new Character
        {
            Name = name.ToUpperInvariant(),
            Alignment = alignment,
            Class = klass,
            Type = type
        };

        // PERSON.COM rolls 4d6 drop-lowest for each of the six attributes.
        // (Exact roll formula pending disassembly of PERSON's dice routine.)
        for (int i = 0; i < 6; i++)
            c.BaseAttributes[i] = Roll4D6DropLowest();

        var con = c.BaseAttributes.Constitution;
        c.MaxHitPoints = Math.Max(1, 8 + (con - 10) / 2);
        c.HitPoints = c.MaxHitPoints;
        c.Gold = 100 + _rng.Next(0, 101);
        c.Experience = 0;
        c.Level = 1;

        for (int i = 0; i < 6; i++) c.CurrentAttributes[i] = c.BaseAttributes[i];

        Character = c;
        CurrentScreen = Screen.Castle;
        Message = $"Created {c.Name} the {type} {klass}.";
    }

    public void EnterDungeon()
    {
        if (Character == null) { Message = "No character loaded."; return; }
        CurrentScreen = Screen.Dungeon;
        Message = "You enter the dungeon. Use R/L/U/D to move.";
    }

    public void Move(char direction)
    {
        if (CurrentScreen != Screen.Dungeon || Character == null) return;
        // Dungeon movement and encounter logic ported from DUNGEON.COM
        // (pending detailed analysis of the collision/monster tables).
        Message = direction switch
        {
            'r' or 'R' or '\x0C' => "You move right.",   // Ctrl-L
            'l' or 'L' or '\x08' => "You move left.",    // Ctrl-H
            'u' or 'U' or '\x0B' => "You move up.",      // Ctrl-K
            'd' or 'D' or '\x0A' => "You move down.",    // Ctrl-J
            _ => "Unknown direction."
        };
    }

    public void ReturnToCastle()
    {
        if (Character == null) return;
        CurrentScreen = Screen.Castle;
        Message = $"Back in the castle, {Character.Name}.";
    }

    private int Roll4D6DropLowest()
    {
        var rolls = new int[4];
        for (int i = 0; i < 4; i++) rolls[i] = _rng.Next(1, 7);
        Array.Sort(rolls);
        return rolls[1] + rolls[2] + rolls[3];
    }
}
