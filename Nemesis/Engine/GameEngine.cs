using Nemesis.Data;
using Nemesis.IO;
using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// High-level game state machine mirroring the original overlay structure:
///   NEMESIS -> launcher/character selection
///   PERSON  -> character generation
///   CASTLE  -> town menu
///   DUNGEON -> exploration/combat
/// </summary>
public sealed class GameEngine
{
    public enum Screen { Launcher, CharacterGen, Castle, Dungeon, GameOver }

    private readonly LfsrRandom _rng = new();
    private readonly CharacterGenerator _generator;
    private readonly CastleEngine _castle;
    private readonly DungeonEngine _dungeon;

    public Screen CurrentScreen { get; private set; } = Screen.Launcher;
    public Character? Character { get; private set; }
    public string? Message { get; set; }
    public LfsrRandom Random => _rng;
    public CastleEngine Castle => _castle;
    public DungeonEngine Dungeon => _dungeon;

    public GameEngine()
    {
        _generator = new CharacterGenerator(_rng);
        _castle = new CastleEngine(_rng);
        _dungeon = new DungeonEngine(_rng);
    }

    // ------------------------------------------------------------ lifecycle ---

    public void LoadCharacter(Character c)
    {
        Character = c;
        _castle.Bind(c);
        _dungeon.Bind(c);
        CurrentScreen = Screen.Castle;
        Message = $"Welcome to the castle, {c.Name}.";
    }

    public void LoadCharacter(string path) => LoadCharacter(CharacterFile.Read(path));

    public void SaveCharacter(string path)
    {
        if (Character == null) { Message = "No character."; return; }
        CharacterFile.Save(Character, path);
        Message = "Saved.";
    }

    public void NewCharacter(string name, int typeIndex, Alignment alignment,
        int? classIndex = null, int? seed = null)
    {
        var c = _generator.Create(name, typeIndex, alignment, classIndex, seed);
        Character = c;
        _castle.Bind(c);
        _dungeon.Bind(c);
        CurrentScreen = Screen.Castle;
        Message = $"Created {c.Name} the {c.TypeName} {c.ClassName}.";
    }

    public IEnumerable<string> ValidClasses() => Character == null
        ? [] : _generator.ValidClassNames(Character);

    public void EnterDungeon()
    {
        if (Character == null) { Message = "No character loaded."; return; }
        CurrentScreen = Screen.Dungeon;
        _dungeon.Enter();
        Message = null;
    }

    public void ReturnToCastle()
    {
        if (Character == null) { Message = "No character loaded."; return; }
        CurrentScreen = Screen.Castle;
        Message = $"Back in the castle, {Character.Name}.";
    }

    public void Exit()
    {
        CurrentScreen = Screen.GameOver;
        Message = "Farewell.";
    }

    // -------------------------------------------------------------- commands ---

    public void Move(char direction)
    {
        if (CurrentScreen != Screen.Dungeon) { Message = "You are not in the dungeon."; return; }
        _dungeon.Move(direction);
        Message = null;
        if (_dungeon.IsDead) CurrentScreen = Screen.GameOver;
    }

    public void Fight()
    {
        RequireDungeon();
        if (!InDungeon) return;
        _dungeon.Fight();
        Message = null;
        if (_dungeon.IsDead) CurrentScreen = Screen.GameOver;
    }

    public string Cast(string school, string spell)
    {
        RequireDungeon();
        if (!InDungeon) return "You are not in the dungeon.";
        var s = school.ToLowerInvariant() switch
        {
            "m" or "magic" => SpellSchool.Magic,
            "c" or "cleric" => SpellSchool.Cleric,
            _ => SpellSchool.Magic
        };
        string result = _dungeon.Cast(s, spell);
        Message = null;
        return result;
    }

    public string UseItem(string name)
    {
        RequireDungeon();
        if (!InDungeon) return "You are not in the dungeon.";
        string result = _dungeon.UseItem(name);
        Message = null;
        return result;
    }

    public void Stairs(int delta)
    {
        RequireDungeon();
        if (!InDungeon) return;
        _dungeon.Stairs(delta);
        Message = null;
    }

    public void Replot()
    {
        RequireDungeon();
        if (!InDungeon) return;
        _dungeon.Replot();
        Message = null;
    }

    public void CastleCommand(char key)
    {
        if (CurrentScreen != Screen.Castle) { Message = "You are not in the castle."; return; }
        _castle.Handle(key);
        Message = null;
    }

    public string Buy(string name) => CastleAction(() => _castle.Buy(name));
    public string Sell(string name) => CastleAction(() => _castle.Sell(name));
    public string Heal(int days) => CastleAction(() => _castle.Heal(days));
    public string Rest() => CastleAction(() => _castle.Rest());
    public string Donate(int amount) => CastleAction(() => _castle.Donate(amount));
    public string CharacterSheet() => Character == null ? "No character." : _castle.CharacterSheet();
    public string Inventory() => Dungeon.ListInventory();

    private string CastleAction(Func<string> action)
    {
        if (CurrentScreen != Screen.Castle) return "You are not in the castle.";
        string result = action();
        Message = null;
        return result;
    }

    private bool InDungeon => CurrentScreen == Screen.Dungeon;

    private void RequireDungeon()
    {
        if (!InDungeon) Message = "You are not in the dungeon.";
    }

    /// <summary>Prints and clears all pending log messages from both subsystems.</summary>
    public IEnumerable<string> DrainLogs()
    {
        foreach (var s in _castle.TakeLog()) yield return s;
        foreach (var s in _dungeon.TakeLog()) yield return s;
    }
}
