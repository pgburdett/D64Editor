# Nemesis C# port

A work-in-progress high-level-language port of the 1981 CP/M dungeon crawler
**Nemesis** by Michael Q. Hiller and Michael A. Pagels.

## Status

This is a **scaffold**, not a playable game yet.  The original Z80 code is
~76 KB across four overlaid `.COM` files; a faithful port requires reconstructing
the game logic from the decompiled output in `docs/nemesis/`.

### What works
- Builds cleanly (`dotnet build Nemesis/Nemesis.csproj`)
- Console front-end with launcher/castle/dungeon screen state machine
- Character file I/O: reads the original 640-byte `.CHR` format
  (name, class, race, alignment, and six attributes decode correctly)
- New character creation with 4d6-drop-lowest stat rolls
- Save/load round-trips raw bytes for un-decoded fields

### What's not done
- Dungeon map generation and exploration (R/L/U/D movement is placeholder only)
- Combat, spells, monsters, items, traps
- Castle menu (armory, hospital, inn, archives)
- Terminal/ANSI rendering to match the ADM-31 screen output
- Exact field decoding for all 640 character-file bytes

## Building

```sh
dotnet build Nemesis/Nemesis.csproj
dotnet run --project Nemesis/Nemesis.csproj -- docs/nemesis/game-files/ARAGORN.CHR
```

## Architecture

| Folder | Purpose |
|--------|---------|
| `Models/` | `Character`, `Attributes`, enums — mirrors the original data model |
| `IO/` | `CharacterFile` — reads/writes original 640-byte `.CHR` files |
| `Engine/` | `GameEngine` — high-level state machine (launcher/castle/dungeon) |
| `Program.cs` | Console UI |

## Porting approach

The original game is four CP/M overlays sharing a common runtime:

| Overlay | Size | Role |
|---------|------|------|
| `NEMESIS.COM` | 9 KB | Launcher, character selection, overlay loading |
| `PERSON.COM` | 14 KB | Character generation |
| `CASTLE.COM` | 18 KB | Town menu, shops, hospital |
| `DUNGEON.COM` | 32 KB | Exploration, combat, dungeon levels |

Ghidra Z80 decompilation of all four is in `docs/nemesis/decomp-*.c` (the
`.java` script that produces them is also there).  The linear `z80dis` sweeps
are in `docs/nemesis/disasm-*.txt`.

The port proceeds by:
1. Identifying BDOS calls (file I/O, console I/O) and replacing them with
   .NET `File`/`Console` equivalents.
2. Mapping the shared runtime routines (string output, number formatting,
   memory allocation) to C# helpers.
3. Porting overlay-specific game logic function by function, using the
   decompiled C as a guide and the running original game in VICE for behavior
   verification.

See `docs/nemesis/README.md` and `docs/nemesis/FIXES.md` for the full RE context.
