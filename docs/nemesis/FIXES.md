# Fixes discovered during Nemesis work

## D81 two-byte allocation pointer overflow (D64Editor.Core/C128Cpm.cs)

`FormatBlank` wrote system allocation blocks using a hard-coded loop bound of
16 entries.  A 32-byte CP/M directory entry holds 16 one-byte allocation
pointers **or only 8 two-byte pointers** (`WordAl` images such as D81).  The
loop overflowed the entry into the filename field, corrupting the directory.

Fix: use `var alCapacity = cimg.WordAl ? 8 : 16;` as the loop bound.

## D81 donor required for writable game disk

From-scratch D81 images (`--cpm-new d81`) are data-only and have an invalid 1581
BAM; the 1581 drive locks up.  Always start from the valid `cpm kit.d81` donor.

`--cpm-format` on D81 corrupts the 1581 BAM (sets byte 0 to 0xE5).  Instead,
clear only real CP/M directory entries (status byte < 0x20) while preserving
the BAM.  The `build-game-disk.sh` script does this.

## ADM-31 TERM.DEF required for full-screen readability

Nemesis emits ADM-31-style cursor-control sequences.  The C128 CP/M console BIOS
does not implement these natively; the game ships `TERMDEF.COM` which generates
a `TERM.DEF` terminal definition.  With a generated ADM-31 (type 6) `TERM.DEF`
on the game disk, all menus/dungeon screens render fully readable.

The included `game-files/TERM.DEF` is that generated definition.

## Boot error from PROFILE.SUB

`CPMFAST3.D71` boots with `PROFILE.SUB` containing
`setdef *,a,b [order=(com,sub)]`.  Under VICE the `setdef` command fails with
`Error On Line 00002 : Disk Write Error`.  This is cosmetic; the game still
runs.  Patching the SUB file to a no-op removes the error but does not change
game behavior.  Do not use the boot disk as a writable game disk — Nemesis
writes the character file to the current drive (A: if launched from A:).

## Reliable input to VICE CP/M

`xdotool key --window` drops characters due to VICE's 1 MHz emulation speed.
Use `feed-keys.py` (binary monitor `kbdbuf_feed`, one byte at a time with ~0.6s
delay).  The correct launch command is `B:NEMESIS ARAGORN` (no trailing `n`).
