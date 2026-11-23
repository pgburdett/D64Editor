# Developer tools

## `z80dis.py` — Z80 disassembler for CP/M `.COM` analysis

Vendors the MIT-licensed [`z80dis`](https://pypi.org/project/z80dis/) decoder
(`z80_decoder.py`) so you can inspect legacy CP/M programs offline with only
Python 3 stdlib — no pip/network required.

CP/M `.COM` files load at `0x100`: file offset `N` = memory address `0x100 + N`.
All addresses below are **memory addresses**; the tool handles the translation.

```bash
# Disassemble a range (memory addresses)
python3 tools/z80dis.py 128cpm/mbascom.lbr --start 0x1348 --end 0x1390

# List printable strings (find prompts/messages)
python3 tools/z80dis.py 128cpm/z80bios.bin --strings

# Find a BDOS "CONIN" call sequence (LD C,1 ; CALL 5)
python3 tools/z80dis.py path/to/PERSON.COM --find-hex "0e 01 cd 05 00"
```

### CP/M BDOS call conventions (handy when reading the output)

- `CALL 0x0005` = BDOS entry; the function is in register `C`.
- `LD C,1` + `CALL 5` = **CONIN** (read one console char into `A`).
- `LD C,2` + `CALL 5` = **CONOUT** (write `E`).
- `LD C,9` + `CALL 5` = print `$`-terminated string at `DE`.
- `LD C,10` + `CALL 5` = buffered console input (line edit).
- `JP 0x0000` = warm boot (exit to the CP/M `A>` prompt).

### Worked example: finding PERSON.COM's exit

```bash
python3 tools/z80dis.py PERSON.COM --find-hex "0e 01 cd 05 00"
# The CONIN helpers each check for Ctrl-C (CP 3) and JP 0x0000 on match:
#   ... CALL 5 ; CP 3 ; JP z,0x0000   -> Ctrl-C exits to A>
```

Ctrl-C (`0x03`) is the universal CP/M program-abort key — many interactive CP/M
programs check it after every CONIN and jump to `0x0000` (warm boot) when seen.
This is the reliable way to leave a misbehaving full-screen CP/M program.
