# Nemesis C# port

A high-level-language port of the 1981 CP/M dungeon crawler **Nemesis** by
Michael Q. Hiller and Michael A. Pagels.  The port targets .NET 10 and keeps a
console UI because the original is a text-mode CP/M program.

## Status

Playable.  All four original overlays have a C# counterpart:

| Original | Port | Coverage |
|----------|------|----------|
| `PERSON.COM` | `Engine/CharacterGenerator.cs` | Exact recovered formulas: type modifiers, class requirements/alignment, sex, gold, `init_char()` spell pools/age/HP |
| `CASTLE.COM` | `Engine/CastleEngine.cs` | A–G menu, armory buy/sell, hospital, inn, archives, character sheet |
| `DUNGEON.COM` | `Engine/DungeonEngine.cs` | 26×22 map loading, movement/walls, stairs, pits/shafts/flame/teleport, encounters, combat, spells, items |
| `NEMESIS.COM` | `Engine/GameEngine.cs` + `Program.cs` | Launcher and screen dispatch |

Recovered data (all 15 races, 15 classes, 94 items, 75 monster records, 27
spells) lives in `Data/`.  The exact 32-bit LFSR from `DUNGEON.COM` is in
`Engine/LfsrRandom.cs` — using `System.Random` would diverge from the original
immediately.

### Fidelity notes

- **Confirmed:** control flow, stat order (`STR INT WIS CHA CON DEX`), type/class
  tables, gold/HP/age formulas, spell-user class sets, 3-byte map cell layout
  (26×22), wall-bit movement, RNG algorithm, the compiled default RNG seed
  (`0xffff59dc`, with `init_rand()` overriding it when a seed is entered), and
  the 640-byte `.CHR` fields that are decoded.
- **Best-effort:** the `roll()` dice expression (only its call site survives),
  combat arithmetic, spell costs/durations, shop price formulas,
  and hospital/inn rates.  The original helper routines for these were not
  fully recovered; the current model is documented in `Engine/DungeonEngine.cs`
  and `Engine/CastleEngine.cs`.
- The original `valid()` ranger bug (testing the loop counter instead of the
  class index) is intentionally preserved in `GameTables.IsValidClass`.
- Undecoded `.CHR` bytes are retained in `Character.RawData`, so save/load is
  byte-exact for untouched files.

## Building and running

```sh
dotnet build Nemesis/Nemesis.csproj
dotnet run --project Nemesis/Nemesis.csproj -- docs/nemesis/game-files/ARAGORN.CHR
dotnet run --project Nemesis/Nemesis.csproj -- --verify
```

`--verify` runs 20 self-checks covering the tables, RNG, `.CHR` round-trip and
real `LEVEL.0` map loading.

## Commands

```
new <name> [type] [align] [class] [seed]   create a character (types 0-14, L/N/C)
load <file> / save <file>                  load/save .CHR
char                                       character sheet
castle / dungeon / back                   change screen
menu / choice A-G                          castle menu
buy <item> / sell <item> / heal <days> / rest / donate <gold>
r l u d                                    move (also Ctrl-L/H/K/J)
+ / -                                      ascend / descend stairs
p                                          replot
f                                          fight
m <spell> / c <spell> / u <item>            cast magic/cleric spell, use item
inv / spells [m|c]                          inventory / spell list
quit
```

Movement keys match the original manual: `R`/Ctrl-L right, `L`/Ctrl-H left,
`U`/Ctrl-K up, `D`/Ctrl-J down.

## Architecture

| Folder | Purpose |
|--------|---------|
| `Data/` | Recovered constant tables (races, classes, items, monsters, spells) |
| `IO/` | `CharacterFile` and `LevelFile` readers/writers for original formats |
| `Models/` | `Character`, `Attributes`, `DungeonMap`, item/monster/spell models |
| `Engine/` | LFSR RNG, character generator, castle/dungeon engines, state machine |
| `Program.cs` | Console UI |
| `Verify.cs` | Self-checks (`--verify`) |

## Reverse-engineering sources

The reconstruction work is committed under `docs/nemesis/`:

- `RECONSTRUCTED-SOURCE.md` — recovered C source fragments and full item/monster tables
- `CASTLE-CHARACTER.md` — character generation and castle logic
- `DATA-FORMATS.md` — binary layouts of `LEVEL.*`, `.CHR`, and item files
- `DUNGEON-LOGIC.md` — dungeon control flow, combat, spells and hazards
- `decomp-*.c` / `disasm-*.txt` — Ghidra and linear disassembly of all four overlays
- `game-files/` — the original CP/M data set used as verification fixtures

`docs/nemesis/README.md` and `docs/nemesis/FIXES.md` document the playable
CP/M setup (FastBDOS, ADM-31 `TERM.DEF`, 1581/D81 game disk) used to verify
behavior against the original game.
