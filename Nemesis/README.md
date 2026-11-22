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

`docs/nemesis/EXACT-FORMULAS.md` records a hand disassembly of the combat,
spell, and castle pricing helpers with CONFIRMED/HIGH/UNCERTAIN confidence
ratings. The recovered **spell unit costs** from the runtime tables are applied
in `Data/GameTables.cs`.  The confirmed melee damage roll, kill payout and
monster-template field layout are applied in `Engine/CombatRules.cs`,
`Engine/DungeonEngine.cs` and `Data/MonsterCatalog.cs`; the remaining findings
stay documentation until their supporting helpers are recovered.

### Fidelity notes

- **Confirmed:** control flow, stat order (`STR INT WIS CHA CON DEX`), type/class
  tables, gold/HP/age formulas, spell-user class sets, 3-byte map cell layout
  (26×22), wall-bit movement, both RNGs (15-bit LFSR and 48-bit LCG), the
  compiled default LFSR seed (`0xffff59dc`, with `init_rand()` overriding it
  when a seed is entered), the `roll()` stat dice (**4d5**, from the machine
  code at `PERSON.COM 0x1a71`), melee damage (`f + power/2`, LCG remainder),
  kill payout (`v + rand() % (10*v)`, written to both sheet buffers), the DM.COM
  monster-template offsets, monster type/group/HP/attack creation and the
  per-monster stat refresh on group advancement, hospital `days = missing HP`
  and `rate = (Level+1)*10`, the composite buy/pawn item value, pawn sell formulas
  (`item_value + LFSR%20` identified, `LCG48%70` unidentified), the per-inventory-slot
  identified bits at `0x80d0`, the equipped bits at `0x80d2`, equipped-item AC
  and fight bonuses, their reversal on sale, the item base price at record `+4`,
  the Archives exact-half donation threshold and gold debit, and the 640-byte
  `.CHR` fields that are decoded.
- **Best-effort:** the monster attack hit/miss test and special-attack branch,
  spell level gating/durations, armory buy modifiers, and the hospital/inn
  stay-debit paths.
  Hospital `days`/`rate`, both pawn formulas, and the Archives exact-half
  threshold and gold debit are confirmed; their final HP postings remain
  best-effort.
  The remaining helper routines were not fully recovered; the current model is
  documented in `Engine/DungeonEngine.cs` and `Engine/CastleEngine.cs`.
- The original `valid()` ranger bug (testing the loop counter instead of the
  class index) is intentionally preserved in `GameTables.IsValidClass`.
- Undecoded `.CHR` bytes are retained in `Character.RawData`, so save/load is
  byte-exact for untouched files.  The save buffer starts at runtime `0x8047`,
  so the per-slot identified bits (`0x80d0`) are persisted at `.CHR` offset
  `0x89`, the equipped bits (`0x80d2`) at `0x8b`, and base AC (`0x80da`) at
  `0x93`.  Inventory records start at `0xa3`; each has signed little-endian AC
  modifier, fight bonus and base price at `+0`, `+2` and `+4`, plus the item
  type at `+0x20`.

## Building and running

```sh
dotnet build Nemesis/Nemesis.csproj
dotnet run --project Nemesis/Nemesis.csproj -- docs/nemesis/game-files/ARAGORN.CHR
dotnet run --project Nemesis/Nemesis.csproj -- --verify
```

`--verify` runs 31 self-checks covering the tables, both RNGs, exact
melee/monster-creation/payout/hospital-rate/pawn arithmetic, `.CHR` round-trip
and real `LEVEL.0` map loading.

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
