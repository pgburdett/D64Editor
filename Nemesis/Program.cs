using Nemesis.Data;
using Nemesis.Engine;
using Nemesis.Models;

namespace Nemesis;

/// <summary>
/// Console front-end for the Nemesis C# port.  The original game was a
/// text-mode CP/M program, so a console UI preserves its presentation style.
/// </summary>
internal static class Program
{
    private static readonly GameEngine Engine = new();

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Nemesis (C# port)";

        if (args.Length > 0 && args[0] is "--verify" or "-v")
        {
            Environment.ExitCode = Verify.Run(FindRepoRoot()) ? 0 : 1;
            return;
        }

        Console.WriteLine("Nemesis Version 1.2 (C# port)");
        Console.WriteLine("Nemesis and Dungeon Master");
        Console.WriteLine("Copyright (c) 1981 Michael Q. Hiller and Michael A. Pagels");
        Console.WriteLine();

        if (args.Length > 0 && File.Exists(args[0]))
        {
            Engine.LoadCharacter(args[0]);
            Console.WriteLine($"Loaded {Engine.Character?.Name}.");
        }

        Console.WriteLine("Type 'help' for commands.");

        while (true)
        {
            Console.WriteLine();
            foreach (var logLine in Engine.DrainLogs()) Console.WriteLine(logLine);
            if (Engine.Message != null) Console.WriteLine($"> {Engine.Message}");
            Console.Write($"{Engine.CurrentScreen}> ");

            var line = Console.ReadLine();
            if (line == null) break;
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            var cmd = parts[0].ToLowerInvariant();

            if (cmd is "quit" or "exit")
            {
                Engine.Exit();
                return;
            }

            switch (cmd)
            {
                case "help": PrintHelp(); break;
                case "new": CmdNew(parts); break;
                case "load":
                    if (parts.Length < 2) Engine.Message = "Usage: load <file>";
                    else if (File.Exists(parts[1])) Engine.LoadCharacter(parts[1]);
                    else Engine.Message = "File not found.";
                    break;
                case "save":
                    if (parts.Length < 2) Engine.Message = "Usage: save <file>";
                    else Engine.SaveCharacter(parts[1]);
                    break;
                case "castle": Engine.ReturnToCastle(); break;
                case "dungeon": case "enter": Engine.EnterDungeon(); break;
                case "back": Engine.ReturnToCastle(); break;
                case "r": case "l": case "u": case "d": Engine.Move(cmd[0]); break;
                case "f": case "fight": Engine.Fight(); break;
                case "m": case "magic":
                    if (parts.Length < 2) Engine.Message = "Usage: m <spell> (or 'spells m')";
                    else Console.WriteLine(Engine.Cast("magic", parts[1]));
                    break;
                case "c": case "cleric":
                    if (parts.Length < 2) Engine.Message = "Usage: c <spell> (or 'spells c')";
                    else Console.WriteLine(Engine.Cast("cleric", parts[1]));
                    break;
                case "use":
                    if (parts.Length < 2) Engine.Message = "Usage: use <item>";
                    else Console.WriteLine(Engine.UseItem(parts[1]));
                    break;
                case "inv": case "inventory": Console.WriteLine(Engine.Inventory()); break;
                case "+": Engine.Stairs(-1); break;
                case "-": Engine.Stairs(1); break;
                case "p": Engine.Replot(); break;
                case "spells": PrintSpells(parts); break;
                case "menu": Console.WriteLine(Engine.Castle.Menu); break;
                case "choice":
                    if (parts.Length < 2) Engine.Message = "Usage: choice <A-G>";
                    else Engine.CastleCommand(parts[1][0]);
                    break;
                case "buy":
                    if (parts.Length < 2) Engine.Message = "Usage: buy <item>";
                    else Console.WriteLine(Engine.Buy(string.Join(' ', parts.Skip(1))));
                    break;
                case "sell":
                    if (parts.Length < 2) Engine.Message = "Usage: sell <item>";
                    else Console.WriteLine(Engine.Sell(string.Join(' ', parts.Skip(1))));
                    break;
                case "heal":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int days))
                        Engine.Message = "Usage: heal <days>";
                    else Console.WriteLine(Engine.Heal(days));
                    break;
                case "rest": Console.WriteLine(Engine.Rest()); break;
                case "donate":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int amount))
                        Engine.Message = "Usage: donate <gold> [slot]";
                    else
                    {
                        int slot = parts.Length >= 3 && int.TryParse(parts[2], out int parsedSlot)
                            ? parsedSlot
                            : -1;
                        Console.WriteLine(Engine.Donate(amount, slot));
                    }
                    break;
                case "char": Console.WriteLine(Engine.CharacterSheet()); break;
                default: Engine.Message = $"Unknown command: {cmd}"; break;
            }
        }
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "docs/nemesis/game-files/ARAGORN.CHR")))
                return dir.FullName;
            dir = dir.Parent;
        }
        return Directory.GetCurrentDirectory();
    }

    private static void CmdNew(string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: new <name> [type] [alignment] [class] [seed]");
            Console.WriteLine("Types:  0 Human, 1 Elf, 2 Dwarf, 3 Halfdwarf, 4 Halfelf, 5 Hobbit, 6 Orc,");
            Console.WriteLine("        7 Uruk-hai, 8 Orge, 9 Pixie, 10 Goblin, 11 Hobgoblin, 12 Kobold,");
            Console.WriteLine("        13 Ur-vile, 14 Grey elf");
            Console.WriteLine("Align:  L, N, C");
            return;
        }

        string name = parts[1];
        int type = parts.Length > 2 && int.TryParse(parts[2], out int t) ? t : 0;
        var align = parts.Length > 3 ? parts[3].ToUpperInvariant() switch
        {
            "L" => Alignment.Lawful, "C" => Alignment.Chaotic, _ => Alignment.Neutral
        } : Alignment.Neutral;
        int? klass = parts.Length > 4 && int.TryParse(parts[4], out int k) ? k : null;
        int? seed = parts.Length > 5 && int.TryParse(parts[5], out int s) ? s : null;

        Engine.NewCharacter(name, type, align, klass, seed);
        Console.WriteLine(Engine.Message);
        Console.WriteLine("Valid classes for this character:");
        foreach (var line in Engine.ValidClasses()) Console.WriteLine("  " + line);
    }

    private static void PrintSpells(string[] parts)
    {
        var school = parts.Length > 1 && parts[1].ToLowerInvariant().StartsWith('c')
            ? SpellSchool.Cleric : SpellSchool.Magic;
        Console.WriteLine($"{school} spells:");
        foreach (var s in school == SpellSchool.Cleric ? SpellCatalog.Cleric : SpellCatalog.Magic)
            Console.WriteLine($"  {s.Index,2} {s.Name,-18} lvl {s.MinLevel,2}  {s.UnitCost} units  {s.Effect}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Launcher: new <name> [type] [align] [class] [seed] | load <file> | save <file>");
        Console.WriteLine("          char | castle | dungeon | quit");
        Console.WriteLine();
        Console.WriteLine("Castle:   menu | choice A-G | buy <item> | sell <item> | heal <days>");
        Console.WriteLine("          rest | donate <gold> [slot] | char | dungeon");
        Console.WriteLine();
        Console.WriteLine("Dungeon:  r/l/u/d or Ctrl-L/H/K/J | + / - stairs | p replot | f fight");
        Console.WriteLine("          m <spell> | c <spell> | u <item> | inv | spells [m|c] | back");
        Console.WriteLine();
        Console.WriteLine("Movement: R/Ctrl-L right, L/Ctrl-H left, U/Ctrl-K up, D/Ctrl-J down.");
    }
}
