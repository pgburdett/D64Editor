using Nemesis.Data;
using Nemesis.IO;
using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// Port of DUNGEON.COM.  The original keeps authoritative state in three
/// globals (X=0x80b8, Y=0x80ba, level=0x80bc) over a 26x22 grid of 3-byte
/// cells.  Control flow (movement, stairs, hazards, encounters, combat) follows
/// the disassembly.  Spell unit costs and the kill-payout shape are decoded in
/// `docs/nemesis/EXACT-FORMULAS.md`; the melee damage roll below is still a
/// best-effort model because the original 48-bit LCG at 0x6c75 is not ported.
/// </summary>
public sealed class DungeonEngine
{
    private readonly LfsrRandom _rng;
    private DungeonMap? _map;
    private readonly Dictionary<int, DungeonMap> _maps = [];

    public Character? Character { get; private set; }
    public DungeonMap? Map => _map;
    public int Level { get; private set; } = -1;
    public int X => Character?.X ?? -1;
    public int Y => Character?.Y ?? -1;

    public bool InCombat { get; private set; }
    public Monster? CurrentMonster { get; private set; }
    public List<string> Log { get; } = [];

    /// <summary>Directory containing LEVEL.0 .. LEVEL.10 (defaults to the repo's game-files).</summary>
    public string DataDirectory { get; set; } = "docs/nemesis/game-files";

    public DungeonEngine(LfsrRandom rng) => _rng = rng;

    public void Bind(Character c)
    {
        Character = c;
        Level = c.DungeonLevel;
        _map = null;
        Log.Clear();
        Log.Add($"You stand at the entrance to the dungeon, {c.Name}.");
    }

    public IEnumerable<string> TakeLog()
    {
        foreach (var s in Log) yield return s;
        Log.Clear();
    }

    private void Say(string message) => Log.Add(message);

    // ---------------------------------------------------------------- map ---

    private bool TryLoadMap(int level)
    {
        if (_maps.TryGetValue(level, out var cached))
        {
            _map = cached;
            return true;
        }

        string path = Path.Combine(DataDirectory, $"LEVEL.{level}");
        if (!File.Exists(path)) path = Path.Combine(DataDirectory, $"L.{level}");

        if (File.Exists(path))
        {
            try
            {
                var map = path.EndsWith($"LEVEL.{level}", StringComparison.Ordinal)
                    ? LevelFile.ReadLevel(path)
                    : LevelFile.ReadStandalone(path);
                _maps[level] = map;
                _map = map;
                return true;
            }
            catch (InvalidDataException)
            {
                // Fall through to a generated map.
            }
        }

        _map = GenerateMap(level);
        _maps[level] = _map;
        return false;
    }

    /// <summary>Deterministic fallback used when the original data files are unavailable.</summary>
    private DungeonMap GenerateMap(int level)
    {
        var map = new DungeonMap();
        var local = new LfsrRandom((uint)(0x4e4d4553 + level * 7919));
        for (int i = 0; i < DungeonMap.CellCount; i++)
        {
            int walls = 0;
            int x = i % DungeonMap.Width;
            int y = i / DungeonMap.Width;
            if (y == 0) walls |= 1 << 0;
            if (x == DungeonMap.Width - 1) walls |= 1 << 1;
            if (y == DungeonMap.Height - 1) walls |= 1 << 2;
            if (x == 0) walls |= 1 << 3;
            if (local.Next(6) == 0) walls |= 1 << local.Next(4);

            byte features = 0;
            if (local.Next(25) == 0) features |= 0x01; // dark
            if (local.Next(30) == 0) features |= 0x02; // pit
            if (local.Next(40) == 0) features |= 0x04; // teleport
            if (local.Next(50) == 0) features |= 0x08; // shaft

            byte param = 0;
            if (x == DungeonMap.Width / 2 && y == DungeonMap.Height / 2) param = 1; // stairs down
            map.Cells[i] = new MapCell((byte)(walls << 4), features, param);
        }
        return map;
    }

    // ------------------------------------------------------------ movement ---

    public void Enter()
    {
        if (Character == null) { Say("No character."); return; }
        Level = Character.DungeonLevel < 0 ? 0 : Character.DungeonLevel;
        Character.DungeonLevel = Level;
        if (Character.X < 0 || Character.Y < 0)
        {
            Character.X = DungeonMap.Width / 2;
            Character.Y = DungeonMap.Height / 2;
        }
        TryLoadMap(Level);
        Say($"You enter dungeon level {Level}.");
        DescribeLocation();
    }

    /// <summary>Direction indices match the wall bit positions: 0=N, 1=E, 2=S, 3=W.</summary>
    public void Move(int direction)
    {
        if (Character == null || _map == null) { Say("You are not in the dungeon."); return; }
        if (InCombat) { Say("You cannot flee while fighting. Use F, M, C or U."); return; }

        int nx = Character.X, ny = Character.Y;
        switch (direction)
        {
            case 0: ny--; break;
            case 1: nx++; break;
            case 2: ny++; break;
            case 3: nx--; break;
        }

        if (nx < 0 || nx >= DungeonMap.Width || ny < 0 || ny >= DungeonMap.Height)
        {
            Say("You cannot go that way.");
            return;
        }

        var here = _map.At(Character.X, Character.Y);
        if (here.HasWall(direction))
        {
            Say("A wall blocks your way.");
            return;
        }

        Character.X = nx;
        Character.Y = ny;
        DescribeLocation();
        ResolveCell();
        MaybeEncounter();
    }

    public void Move(char key) => Move(key switch
    {
        'u' or 'U' or '\x0b' => 0,
        'r' or 'R' or '\x0c' => 1,
        'd' or 'D' or '\x0a' => 2,
        'l' or 'L' or '\x08' => 3,
        _ => -1
    });

    public void Replot() { DescribeLocation(); }

    private void DescribeLocation()
    {
        if (_map == null || Character == null) return;
        var cell = _map.At(Character.X, Character.Y);
        string features = string.Join(", ", FeatureNames(cell));
        Say($"Level {Level}  Pos ({Character.X},{Character.Y})" +
            (features.Length > 0 ? $"  [{features}]" : "") +
            $"  HP {Character.HitPoints}/{Character.MaxHitPoints}" +
            $"  AC {Character.ArmorClass}");
    }

    private static IEnumerable<string> FeatureNames(MapCell c)
    {
        if (c.IsDark) yield return "dark";
        if (c.IsPit) yield return "pit";
        if (c.IsTeleport) yield return "tport";
        if (c.IsShaft) yield return "shaft";
        if (c.IsAntiMagic) yield return "anti-magic";
        if (c.IsAntiCleric) yield return "anti-cleric";
        if (c.IsIllusion) yield return "illusion";
        if (c.IsFire) yield return "fire";
        if (c.Parameter == 1) yield return "stairs down";
        if (c.Parameter == 2) yield return "stairs up";
    }

    // -------------------------------------------------------------- hazards ---

    private void ResolveCell()
    {
        if (Character == null || _map == null) return;
        var cell = _map.At(Character.X, Character.Y);

        if (cell.IsPit)
        {
            if (Character.BaseAttributes.Dexterity >= 12 && _rng.Next(2) == 0)
            {
                Say("You have passed over a pit");
            }
            else
            {
                int dmg = 1 + _rng.Next(6);
                DamagePlayer(dmg, "You have fallen into a pit");
            }
        }

        if (cell.IsFire)
        {
            int dmg = 1 + _rng.Next(8);
            DamagePlayer(dmg, "You have been engulfed by flame !!");
        }

        if (cell.IsShaft)
        {
            Say("A shaft drops you to the level below!");
            ChangeLevel(1);
        }

        if (cell.IsTeleport)
        {
            Character.X = _rng.Next(DungeonMap.Width);
            Character.Y = _rng.Next(DungeonMap.Height);
            Say("A teleporter whisks you away!");
            DescribeLocation();
        }

        if (cell.IsIllusion) Say("For a moment, nothing is quite what it seems.");
        if (cell.IsAntiMagic) Say("The air hums: magic will not work here.");
        if (cell.IsAntiCleric) Say("A cold silence: cleric magic will not work here.");

        if (cell.Parameter >= 1 && cell.Parameter <= 3)
            Say(cell.Parameter == 1 ? "You see stairs leading down." :
                cell.Parameter == 2 ? "You see stairs leading up." : "You see a winding staircase.");
    }

    private void DamagePlayer(int amount, string reason)
    {
        if (Character == null) return;
        Character.HitPoints = Math.Max(0, Character.HitPoints - amount);
        Say($"{reason}: {amount} damage.");
        if (!Character.IsAlive) Die();
    }

    private void Die()
    {
        InCombat = false;
        CurrentMonster = null;
        Say("You have died. Your character file has been lost.");
    }

    public bool IsDead => Character != null && !Character.IsAlive;

    // --------------------------------------------------------------- stairs ---

    public void Stairs(int delta)
    {
        if (_map == null || Character == null) return;
        var cell = _map.At(Character.X, Character.Y);
        if (cell.Parameter == 0)
        {
            Say("There are no stairs here.");
            return;
        }
        if ((delta > 0 && cell.Parameter == 2) || (delta < 0 && cell.Parameter != 2))
        {
            ChangeLevel(delta);
        }
        else
        {
            Say("The stairs do not lead that way.");
        }
    }

    public void ChangeLevel(int delta)
    {
        if (Character == null) return;
        int newLevel = Level + delta;
        if (newLevel < 0) { Say("You cannot go above the surface."); return; }
        if (newLevel > 999) { Say("The dungeon has no end."); return; }

        Level = newLevel;
        Character.DungeonLevel = newLevel;
        Character.X = -1;
        Character.Y = -1;
        TryLoadMap(newLevel);
        Say($"You arrive on dungeon level {Level}.");
        if (Character.X < 0)
        {
            Character.X = DungeonMap.Width / 2;
            Character.Y = DungeonMap.Height / 2;
        }
        DescribeLocation();
    }

    // ------------------------------------------------------------ encounters ---

    private void MaybeEncounter()
    {
        if (Character == null || InCombat || IsDead) return;
        if (_rng.Next(20) != 0) return;

        var candidates = MonsterCatalog.ForLevel(Level).ToList();
        if (candidates.Count == 0) candidates = MonsterCatalog.All.ToList();
        var def = candidates[_rng.Next(candidates.Count)];

        int count = Math.Clamp(def.GroupSize, 1, 4);
        int hp = Math.Max(1, def.HitPoints + _rng.Next(5) - 2);
        CurrentMonster = new Monster
        {
            Type = new MonsterType
            {
                Name = def.Name.Replace('_', ' '),
                ArmorClass = def.ArmorClass,
                HitDice = Math.Max(1, def.HitPoints / 4),
                DamageDice = Math.Max(1, def.Damage / 2),
                ExperienceValue = def.Experience,
                GoldValue = def.Gold,
            },
            HitPoints = hp * count,
            MaxHitPoints = hp * count,
            GroupIndex = count,
        };
        InCombat = true;
        Say($"A group of {count} {CurrentMonster.Type.Name} attacks!");
        Say("Enter attack command: F (fight), M (magic), C (cleric), U (use item).");
    }

    // ---------------------------------------------------------------- combat ---

    public void Fight()
    {
        if (!InCombat || CurrentMonster == null || Character == null) { Say("Nothing to fight."); return; }

        int attack = _rng.Roll(3, 6) + Character.BaseAttributes.Strength / 2
                     + Character.Level + Character.ArmorClass / 2;
        if (attack >= CurrentMonster.Type.ArmorClass)
        {
            int dmg = Math.Max(1, _rng.Roll(1, 6) + Character.BaseAttributes.Strength / 3
                                 + WeaponDamage());
            CurrentMonster.HitPoints -= dmg;
            Say($"   You gave it {dmg} hits");
            if (CurrentMonster.HitPoints <= 0) { WinCombat(); return; }
        }
        else
        {
            Say("You miss.");
        }
        MonsterTurn();
    }

    private int WeaponDamage()
    {
        if (Character == null) return 0;
        int best = 0;
        for (int i = 0; i < Character.ItemTypes.Length; i++)
        {
            if (Character.ItemTypes[i] == GameConstants.EmptyItem) continue;
            var item = ItemCatalog.All.FirstOrDefault(x => x.Type == Character.ItemTypes[i]);
            if (item != null) best = Math.Max(best, item.Damage);
        }
        return best;
    }

    private void MonsterTurn()
    {
        if (CurrentMonster == null || Character == null || !InCombat) return;
        if (CurrentMonster.IsAsleep) { CurrentMonster.IsAsleep = false; Say("It sleeps."); return; }
        if (CurrentMonster.IsFrightened) { CurrentMonster.IsFrightened = false; Say("It cowers in fear."); return; }

        int chance = _rng.Next(20) + Character.Level / 2;
        if (chance >= CurrentMonster.Type.ArmorClass / 2)
        {
            int dmg = Math.Max(1, _rng.Roll(1, CurrentMonster.Type.DamageDice * 4 + 2));
            DamagePlayer(dmg, "The monster hits you");
        }
        else Say("The monster misses.");
    }

    private void WinCombat()
    {
        if (CurrentMonster == null || Character == null) return;
        Say("You killed 1");
        Character.Experience += CurrentMonster.Type.ExperienceValue;
        Character.Gold += CurrentMonster.Type.GoldValue;
        CheckLevelUp();
        CurrentMonster = null;
        InCombat = false;
    }

    private void CheckLevelUp()
    {
        if (Character == null) return;
        int threshold = 100 * (Character.Level + 1) * Character.XpMultiplier / 200;
        if (Character.Experience >= threshold)
        {
            Character.Level++;
            int gain = Math.Max(1, _rng.Next(Character.BaseAttributes.Constitution) + 1);
            Character.MaxHitPoints += gain;
            Character.HitPoints = Math.Min(Character.MaxHitPoints, Character.HitPoints + gain);
            Say($"You advance to level {Character.Level}!");
        }
    }

    // ---------------------------------------------------------------- spells ---

    public bool CanCastMagic => Character != null &&
        Character.MagicUnits > 0 && Character.MagicUnits != 0xffff
        ? Character.MagicUnits > 0
        : Character?.MagicUnits == 0xffff;

    public IReadOnlyList<SpellDef> AvailableSpells(SpellSchool school)
    {
        var list = school == SpellSchool.Cleric ? SpellCatalog.Cleric : SpellCatalog.Magic;
        return list.Where(s => Character != null && Character.Level >= s.MinLevel).ToList();
    }

    public string Cast(SpellSchool school, string name)
    {
        if (Character == null) return "No character.";
        var spell = SpellCatalog.Find(name, school);
        if (spell == null) return $"Unknown spell: {name}";
        if (Character.Level < spell.MinLevel) return $"{spell.Name} requires level {spell.MinLevel}.";

        var cell = _map?.At(Character.X, Character.Y) ?? new MapCell(0, 0, 0);
        if (school == SpellSchool.Magic && cell.IsAntiMagic) return "Magic will not work here.";
        if (school == SpellSchool.Cleric && cell.IsAntiCleric) return "Cleric magic will not work here.";

        bool limited = (school == SpellSchool.Magic && Character.MagicUnits != 0xffff) ||
                       (school == SpellSchool.Cleric && Character.ClericUnits != 0xffff);
        int pool = school == SpellSchool.Magic ? Character.MagicUnits : Character.ClericUnits;
        if (limited && pool < spell.UnitCost) return "Not enough spell units.";
        if (limited)
        {
            if (school == SpellSchool.Magic) Character.MagicUnits -= spell.UnitCost;
            else Character.ClericUnits -= spell.UnitCost;
        }

        string result = ResolveSpell(spell);
        Say(result);
        return result;
    }

    private string ResolveSpell(SpellDef spell)
    {
        if (Character == null) return "";
        switch (spell.Name)
        {
            case "CURE":
            case "HEAL":
                int healed = Math.Min(Character.MaxHitPoints - Character.HitPoints,
                    _rng.Roll(1, 8) + Character.Level);
                Character.HitPoints += healed;
                return $"You cured {healed} hits.";

            case "DAMAGE":
            case "ATTACK":
            case "FIREBALL":
            case "LIGHTNING":
            case "CLOUDKILL":
            case "POWER WORD":
            case "HOLY WORD":
                if (!InCombat || CurrentMonster == null) return "There is no target.";
                int dmg = _rng.Roll(1, 6) + Character.Level;
                CurrentMonster.HitPoints -= dmg;
                Say($"   You gave it {dmg} hits");
                if (CurrentMonster.HitPoints <= 0) WinCombat(); else MonsterTurn();
                return spell.Name;

            case "SLEEP":
                if (!InCombat || CurrentMonster == null) return "There is no target.";
                CurrentMonster.IsAsleep = true;
                return "slept";

            case "FEAR":
                if (!InCombat || CurrentMonster == null) return "There is no target.";
                CurrentMonster.IsFrightened = true;
                return "The monster is afraid.";

            case "STRENGTH":
                Character.CurrentAttributes.Strength += Math.Max(1, Character.Level / 3);
                return "You feel stronger.";

            case "SPEED":
                Character.CurrentAttributes.Dexterity += Math.Max(1, Character.Level / 3);
                return "You move faster.";

            case "SHIELD":
            case "PROTECT":
                Character.ArmorClass -= 1;
                return "Your armor class improves.";

            case "LEVITATE":
                return "You feel lighter.";

            case "LIGHT":
                return "The room is lit.";

            case "DISCLOSE":
                return "Disclose: state an item to identify it.";

            case "RESTORE":
                Character.CurrentAttributes = Character.BaseAttributes.Clone();
                return "Your attributes are restored.";

            case "TELEPORT":
                Character.X = _rng.Next(DungeonMap.Width);
                Character.Y = _rng.Next(DungeonMap.Height);
                DescribeLocation();
                return "You teleport.";

            default:
                return $"You cast {spell.Name}.";
        }
    }

    // ----------------------------------------------------------------- items ---

    public string UseItem(string name)
    {
        if (Character == null) return "No character.";
        for (int i = 0; i < Character.ItemTypes.Length; i++)
        {
            if (Character.ItemTypes[i] == GameConstants.EmptyItem) continue;
            var def = ItemCatalog.All.FirstOrDefault(x =>
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (def == null || def.Type != Character.ItemTypes[i]) continue;

            string effect = ApplyItem(def);
            Say(effect);
            return effect;
        }
        return $"You do not have {name}.";
    }

    private string ApplyItem(ItemDef item)
    {
        if (Character == null) return "";
        if (item.Name.Contains("HEALING", StringComparison.OrdinalIgnoreCase) ||
            item.Name.Contains("HEAL", StringComparison.OrdinalIgnoreCase))
        {
            int healed = Math.Min(Character.MaxHitPoints - Character.HitPoints, 2 + _rng.Roll(1, 6));
            Character.HitPoints += healed;
            return $"You cured {healed} hits.";
        }
        if (item.Name.Contains("RESTORATION", StringComparison.OrdinalIgnoreCase))
        {
            Character.CurrentAttributes = Character.BaseAttributes.Clone();
            return "Your attributes are restored.";
        }
        if (item.Name.Contains("STRENGTH", StringComparison.OrdinalIgnoreCase) ||
            item.Name.Contains("MIGHT", StringComparison.OrdinalIgnoreCase))
        {
            Character.CurrentAttributes.Strength += 1;
            return "You feel stronger.";
        }
        return $"You use the {item.Name}.";
    }

    public string ListInventory()
    {
        if (Character == null) return "No character.";
        var names = new List<string>();
        for (int i = 0; i < Character.ItemTypes.Length; i++)
        {
            if (Character.ItemTypes[i] == GameConstants.EmptyItem) continue;
            var def = ItemCatalog.All.FirstOrDefault(x => x.Type == Character.ItemTypes[i]);
            names.Add($"{i + 1}. {def?.Name ?? $"type {Character.ItemTypes[i]}"}");
        }
        return names.Count == 0 ? "You are carrying nothing." : string.Join("\n", names);
    }

    public string GiveItem(string name)
    {
        if (Character == null) return "No character.";
        var def = ItemCatalog.All.FirstOrDefault(x =>
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (def == null) return $"Unknown item: {name}";
        int slot = Array.IndexOf(Character.ItemTypes, (byte)GameConstants.EmptyItem);
        if (slot < 0) return "You cannot carry any more items.";
        Character.ItemTypes[slot] = (byte)def.Type;
        Say($"You have found: {def.Name}");
        return $"You have found: {def.Name}";
    }
}
