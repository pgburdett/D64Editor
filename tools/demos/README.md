# CP/M demo programs

Small Z80 programs used to probe C128 CP/M capabilities. They are assembled with `z80asm`
(AUR) and staged onto D71 images through D64Editor's explicit `--ctools` path.

## `vdcchar.asm` / `vdcchar.com`

Demonstrates that CP/M on the C128 can drive the 80-column VDC (8563) for full-screen graphics.

### Build and stage

```bash
cd tools/demos
z80asm -o vdcchar.com vdcchar.asm
cd ../..
dotnet run --project D64Editor.App -- --ctools create d71 /tmp/vdc.d71 tools/demos/vdcchar.com
# boot with a C128 CP/M system disk in drive 8:
x128 -80col -8 <c128-cpm-system.d64> -9 /tmp/vdc.d71
# at A>: B:  then  VDCCHAR
```

### Background

Reference: C128 Programmer's Reference Guide, Appendix K, "Commodore 128 CP/M BIOS Routines",
User Function 4 (8502 BIOS) with subfunctions:

| Subfunction | Meaning |
|---:|---|
| 4 | Cursor position (D = row, E = column). |
| 0 | Write character. |
| 8 / 12 / 16 / 20 | Cursor movement. |
| 28 / 32 | Clear to end of line / clear to end of screen. |
| 56 | Set 80-column attributes. |
| 64 | Write character attribute. |
| 126 | Character install: define an 8-byte custom glyph in VDC RAM. |
| 116 | Place the 80-column cursor at a VDC RAM address. |

This is the native C128 CP/M way to do full-screen graphics — **not** ANSI/VT escape sequences.
It explains why line-oriented programs (MBASIC, Infocom Zork II) work under x128 while
full-screen CP/M 2.2 programs that emit VT/ANSI cursor sequences render garbled: the stock C128
console does not interpret those escapes. This is a terminal limitation, not file corruption.

### Status

The demo builds, stages, boots, and prints its banner via BDOS CONOUT. The cursor-position call
through the `USERFN` routine is a documented but unverified starting point: under x128 CP/M 3.0
the test printed sequentially instead of at the requested rows/columns. Next steps are to verify
the WBOOT vector at `(1)` and the USERF offset (`30*3 = 90`), or call the BIOS through BDOS
function 50 with a parameter block as described in Appendix K.
