using Nemesis.Engine;
using Nemesis.IO;
using Nemesis.Models;

namespace Nemesis;

/// <summary>
/// Console front-end for the Nemesis C# port.  The original game was a
/// text-mode CP/M program, so a console UI preserves the presentation style.
/// </summary>
internal static class Program
{
    private static readonly GameEngine Engine = new();

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Nemesis (C# port)";

        if (args.Length > 0 && File.Exists(args[0]))
        {
            var c = CharacterFile.Read(args[0]);
            Engine.LoadCharacter(c);
        }

        Console.WriteLine("Nemesis Version 1.2 (C# port)");
        Console.WriteLine("Nemesis and Dungeon Master");
        Console.WriteLine("Copyright (c) 1981 Michael Q. Hiller and Michael A. Pagels");
        Console.WriteLine();
        Console.WriteLine("Commands: new, load <file>, save <file>, castle, dungeon,");
        Console.WriteLine("          r/l/u/d, back, char, quit");

        while (true)
        {
            Console.WriteLine();
            if (Engine.Message != null) Console.WriteLine($"> {Engine.Message}");
            Console.Write($"{Engine.CurrentScreen}> ");
            var line = Console.ReadLine();
            if (line == null) break;
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            var cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "quit": case "exit": return;

                case "new":
                    if (parts.Length < 2) { Engine.Message = "Usage: new <name>"; break; }
                    Engine.NewCharacter(parts[1], Alignment.Neutral,
                        CharacterClass.Fighter, CharacterType.Human);
                    break;

                case "load":
                    if (parts.Length < 2) { Engine.Message = "Usage: load <file>"; break; }
                    if (File.Exists(parts[1]))
                        Engine.LoadCharacter(CharacterFile.Read(parts[1]));
                    else
                        Engine.Message = "File not found.";
                    break;

                case "save":
                    if (Engine.Character == null) { Engine.Message = "No character."; break; }
                    if (parts.Length < 2) { Engine.Message = "Usage: save <file>"; break; }
                    CharacterFile.Save(Engine.Character, parts[1]);
                    Engine.Message = "Saved.";
                    break;

                case "castle": Engine.ReturnToCastle(); break;
                case "dungeon": Engine.EnterDungeon(); break;
                case "r": case "l": case "u": case "d": Engine.Move(cmd[0]); break;
                case "back": Engine.ReturnToCastle(); break;

                case "char":
                    if (Engine.Character == null) { Engine.Message = "No character."; break; }
                    PrintCharacter(Engine.Character);
                    break;

                default:
                    Engine.Message = $"Unknown command: {cmd}";
                    break;
            }
        }
    }

    private static void PrintCharacter(Character c)
    {
        Console.WriteLine($"Name:    {c.Name}");
        Console.WriteLine($"Class:   {c.Class}   Type: {c.Type}   Align: {c.Alignment}");
        Console.WriteLine($"Level:   {c.Level}   XP: {c.Experience}   Gold: {c.Gold}");
        Console.WriteLine($"HP:      {c.HitPoints}/{c.MaxHitPoints}   AC: {c.ArmorClass}");
        var a = c.BaseAttributes;
        Console.WriteLine($"STR {a.Strength}  INT {a.Intelligence}  WIS {a.Wisdom}  " +
                          $"DEX {a.Dexterity}  CON {a.Constitution}  CHR {a.Charisma}");
    }
}
