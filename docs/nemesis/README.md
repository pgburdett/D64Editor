# Nemesis (CP/M) reverse engineering

Nemesis is a 1985/86 dungeon crawler by Michael Q. Hiller and Michael A. Pagels,
published by SuperSoft.  This folder contains disassembly work on the CP/M
version, plus the scripts used to build and run a working 1581-based game disk
under VICE x128 with the FastBDOS system disk.

## Game files

The package in `128cpm/nemesis.zip` contains four main Z80 `.COM` executables:

| File | Size | Purpose |
|------|------|---------|
| `NEMESIS.COM` | 9,216 | Main launcher / menu |
| `CASTLE.COM` | 18,816 | Castle/town menu |
| `DUNGEON.COM` | 32,512 | Dungeon exploration/combat |
| `PERSON.COM` | 14,336 | Character generation |

All four share a common ~4KB runtime/loader stub at the start (identical byte
prefix), then contain an overlay-specific code/data section.  The game uses
CP/M BDOS calls for file I/O (loading/saving character files and level data).

## Disassembly

- `disasm-*.txt` — linear disassembly of each `.COM` from base `0x0100`, produced
  by `disasm_z80.py` using the Python `z80dis` package.  Linear sweeps include
  data bytes interpreted as opcodes; use the shared stub prefix to find code
  boundaries.
- `ExportNemesis.java` / `ExportNemesis.py` — Ghidra export scripts (Z80
  language: `z80:LE:16:default`).  Ghidra headless can analyze the files with:

  ```sh
  /opt/ghidra/support/analyzeHeadless /tmp/proj nemesis \
    -import NEMESIS.COM -processor 'z80:LE:16:default'
  ```

- `nemesis-manual-ocr.txt` — OCR of the scanned manual from `128cpm/nemesis.zip`
  (the manual is in `manual/Image*.jpg`).  Page 16 (`Image16.jpg`) has the movement
  commands; the OCR there is cleanest with a high-contrast upscaled crop.

## Manual: controls

Movement (one room per press): `R` / `Ctrl-L` / right arrow, `L` / `Ctrl-H` /
left arrow, `U` / `Ctrl-K` / up arrow, `D` / `Ctrl-J` / down arrow.
Other keys: `ESC` display character, `M` magic spell, `C` cleric spell,
`P` replot display (also `Ctrl-H` hardcopy in some contexts).

## Running the original game

`launch-x128-1581.sh` starts VICE x128 with:

- **A:** `CPMFAST3.D71` — FastBDOS 3 (`CP/M 3 - BDOS 3.8 / FastBIOS 3.0`), the
  latest fast BDOS found in `cpmfastfinal.zip`.
- **B:** a 1581 D81 game disk built from the `cpm kit.d81` donor with the full
  Nemesis file set (`ARAGORN.CHR`, `TERM.DEF`, all `.COM` files and level data).

Game launch command: `B:NEMESIS ARAGORN` (the second argument is the character
name, which maps to `ARAGORN.CHR` on B:).

The `Error On Line 00002 : Disk Write Error` seen at boot is from CP/M 3's
`PROFILE.SUB` running `setdef *,a,b [order=(com,sub)]`; it is cosmetic and the
game runs fine after it.

To send commands reliably to CP/M through VICE's binary monitor, feed keystrokes
one character at a time via command `0x72` (`kbdbuf_feed`) to port `6502`.
`xdotool key --window` tends to drop characters due to the emulator's speed.
