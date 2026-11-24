# Developer tools

This directory contains standalone tools that are not built by the .NET solution. They support
reverse engineering of legacy CP/M programs and the C64/C128 hardware investigation.

## `z80dis.py` — offline Z80 disassembler

`z80dis.py` vendors the MIT-licensed `z80_decoder.py` decoder so CP/M `.COM` files can be inspected
offline with Python 3 standard library only — no pip install and no network required.

CP/M `.COM` files load at `0x100`, so file offset `N` maps to memory address `0x100 + N`. All
`--start`/`--end` values are **memory addresses**; the tool performs the translation internally.

### Usage

```bash
# Disassemble a memory-address range
python3 tools/z80dis.py 128cpm/mbascom.lbr --start 0x1348 --end 0x1390

# List printable strings (find prompts and messages)
python3 tools/z80dis.py 128cpm/z80bios.bin --strings

# Find the BDOS CONIN sequence: LD C,1 ; CALL 5
python3 tools/z80dis.py path/to/PERSON.COM --find-hex "0e 01 cd 05 00"
```

`0e 01` is `LD C,1`; `cd 05 00` is `CALL 0x0005`.

### CP/M BDOS conventions

- `CALL 0x0005` — BDOS entry; function number in `C`.
- `LD C,1 ; CALL 5` — CONIN, read one console char into `A`.
- `LD C,2 ; CALL 5` — CONOUT, write `E`.
- `LD C,9 ; CALL 5` — print `$`-terminated string at `DE`.
- `LD C,10 ; CALL 5` — buffered console input with line editing.
- `JP 0x0000` — warm boot to CP/M `A>`.

Ctrl-C (`0x03`) is the universal CP/M abort key. Many interactive programs poll for it after
CONIN and warm-boot on match: `CALL 5 ; CP 3 ; JP z,0x0000`. This is the reliable way to exit a
misbehaving full-screen CP/M program.

## `build-patched-vice.sh`

Builds VICE 3.10 with `patches/vice-3.10-opencbm-filename.patch` applied, installing to
`/tmp/opencode/vice-patched` by default. See `patches/README.md`.

## Other tools

- `tools/z80_decoder.py` — vendored Z80 decoder used by `z80dis.py`.
- `tools/demos/` — C128 CP/M demo programs and notes.
- `tools/diagnostics/` — VICE launchers, binary-monitor clients, raw-media capture and analysis
  scripts. See `tools/diagnostics/README.md`.
