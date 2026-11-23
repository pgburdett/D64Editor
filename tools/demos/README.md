# CP/M demo programs

Z80 assembly demos for C128 CP/M, assembled with `z80asm` (AUR) and staged onto
D71 images with the app's `--ctools` path.

## `vdcchar.asm` — VDC full-screen I/O experiment

Build and stage:

```bash
cd tools/demos
z80asm -o vdcchar.com vdcchar.asm
cd ../..
dotnet run --project D64Editor.App -- --ctools create d71 /tmp/vdc.d71 tools/demos/vdcchar.com
# boot: x128 -80col -8 <c128-cpm-system.d64> -9 /tmp/vdc.d71
# at A>: B:  then  VDCCHAR
```

**What this demonstrates.** CP/M on the C128 *can* drive the 80-column VDC
(8563) for full-screen graphics. The C128 Programmer's Reference Guide
(Appendix K, "Commodore 128 CP/M BIOS Routines") documents **User Function 4**
(8502 BIOS) with subfunctions for:

- `4` cursor position (D = row, E = column)
- `0` write character, `8/12/16/20` cursor movement
- `28`/`32` clear-to-end-of-line/screen
- `56` set 80-column attributes, `64` write character attribute
- `126` character install (define an 8-byte custom glyph in VDC RAM, and
  subfunction `116` places the 80-column cursor at a VDC RAM address)

This is the *native* C128 CP/M way to do full-screen graphics — **not** ANSI/VT
escape sequences. That distinction explains why line-oriented CP/M programs
(Infocom Zork, MBASIC) work under x128 while full-screen CP/M 2.2 programs that
emit VT/ANSI escapes can render garbled: the stock C128 console does not interpret
those escape sequences.

**Status.** The demo builds, stages, boots and its banner prints correctly via
BDOS CONOUT. The cursor-position subfunction call (the `USERFN` routine) is a
documented-but-unverified starting point: under x128 CP/M 3.0 the pattern
printed sequentially rather than at the requested rows/columns, so the exact
USERF entry/parameter passing for this BDOS build still needs checking. Useful
next steps: verify the WBOOT vector at `(1)` and the USERF offset (`30*3=90`),
or call the BIOS through BDOS function 50 with a parameter block as also
described in Appendix K.
