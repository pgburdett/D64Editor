# Reverse engineering notes

This document records the reverse-engineering toolchain and the highest-value findings. Work on
the disk/format engine itself belongs in `AGENTS.md` and `docs/CODE.md`; hardware and protection
analysis is in `docs/drive-assembly/README.md`, `docs/disk-images/README.md`, and
`HARDWARE_WORKFLOW.md`.

## Toolchain

| Tool | Role |
|---|---|
| Ghidra 12.2 DEV (`/opt/ghidra`) with Z80 processor support | Initial static analysis of CP/M `.COM` programs. Decompiled C is secondary/noisy, especially where self-modifying code or inline operands confuse auto-analysis. |
| `z80dis` (Python) | Linear sweeps over CP/M binaries. Often finds strings, prompt tables, and code paths that Ghidra misses because CP/M `.COM` files have no symbol or code-section metadata. Vendored offline in `tools/z80dis.py` with `z80_decoder.py`. |
| `z80asm` (AUR) | Reassembling modified Z80 code for C128 CP/M demos (`tools/demos/vdcchar.asm`). |
| VICE x128 / x64sc (patched build in `/tmp/opencode/vice-patched`) | Running original games and verifying behavior; binary monitor for drive-CPU tracing. |
| Epyx FastLoad CRT (`docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt`) | Commercial fast-loader control condition under `-cartcrt`; useful for checking whether a failure is in the disk/data path or in host/accelerator interaction. Not used by D64Editor code. |
| `xa` / `da65` | 6502 assembly and disassembly for drive-side routines. |
| `lha` / `7z` / `arj` | Inspecting and extracting legacy PC/C64 archives found on reference disks. |

## CP/M analysis conventions

- A CP/M `.COM` file is loaded at `0x100`; file offset `N` corresponds to memory address
  `0x100 + N`. `tools/z80dis.py --start/--end` accepts memory addresses and performs the
  translation internally.
- `CALL 0x0005` is the BDOS entry point; the function number is held in `C`.
- Useful BDOS calls:
  - `LD C,1 ; CALL 5` — CONIN, read one console character into `A`.
  - `LD C,2 ; CALL 5` — CONOUT, write `E`.
  - `LD C,9 ; CALL 5` — print `$`-terminated string at `DE`.
  - `LD C,10 ; CALL 5` — buffered console input with line editing.
  - `JP 0x0000` — warm boot to the CP/M `A>` prompt.
- Ctrl-C (`0x03`) is the universal CP/M abort key. Interactive programs commonly poll for it
  after every CONIN and warm-boot when seen: `CALL 5 ; CP 3 ; JP z,0x0000`.

## Verified CP/M findings

- C128 CP/M uses 7-bit ASCII, not PETSCII.
- Geometry is speed-zone + interleave based, not linear: `sec = (5 * nr) % sectrk`, derived from
  ctools `bios.cc`. Disk type is detected from T1/S0: `"CBM"` plus byte 255 `== 0xFF` means
  double-sided (D71, 2K blocks, two extents/entry); otherwise C128 single-sided or C64 (1K).
- Directory occupies logical blocks 0–1, eight 32-byte entries per sector. `EX`/`RC` encode total
  records as `RC >= 0x80 ? 128*(EX+1) : 128*EX + RC`, with `EX` modulo extents-per-entry.
- Byte-exact extraction was verified against ctools: 19 files from `MBASIC.d71` and 10 from the
  D64 system disk (both strict and non-strict modes).
- `--cpm-new d71` with `MBASIC.COM` + `WUMP.BAS`, mounted on drive 9 (`B:`) with a system disk on
  drive 8 (`A:`), ran WUMPUS to completion under x128 CP/M 3.0.
- Infocom Zork II (`ZORK2.COM` + `ZORK2.DAT`, configured via `2DEFAULT`/`CONF2`) was staged and
  played interactively. Full-screen CP/M 2.2 programs that emit VT/ANSI cursor sequences render
  poorly under the stock C128 CP/M console; this is a terminal limitation, not file corruption.
- From-scratch CP/M images are data-only; use `--cpm-bootable` with a donor for bootable media.

## Drive-code and protection findings

- EA PRODOS/PirateSlayer loaders upload ~1,280 bytes of 6502 code into drive RAM `$0300-$07FF`
  via 40 `M-W` commands and execute it with `M-E`. The code reads raw GCR from VIA2 `$1C01` and
  bit-bangs the IEC lines through VIA1 `$1800`; high-level OpenCBM cannot emulate this.
- The validated capture path is SRQ nibbling: `nibread -@xum1541 -D8 -e5 -h -v` → `nibconv` →
  G64 → x64sc `-drive8truedrive`. D64 → G64 regeneration loses physical gaps/timing and fails
  the EA loader.
- Protected tracks are deliberately malformed: void/no-sync tracks, excessive sync runs without
  sector headers, weak GCR, half-tracks, and unformatted regions. See
  `docs/drive-assembly/README.md` for register maps, FDC job codes, TGREAD, and raw-track byte
  histograms.
- Tail-gap bytes `60 24 46 23 99` were read from real hardware with the ported TGREAD routine.

## Files

- `tools/z80dis.py` / `tools/z80_decoder.py` — offline Z80 disassembly and byte-pattern search.
- `tools/demos/vdcchar.asm` / `vdcchar.com` — C128 CP/M VDC full-screen experiment.
- `tools/diagnostics/` — VICE binary-monitor clients and drive-code trace/dump scripts.
- `docs/fast-load/` — Epyx FastLoad CRT and its VICE/D64Editor cross-check notes; moved from the
  repository root with identical bytes.
- `docs/inside-commodore-dos.txt` — line-wrapped OCR reference for DOS-protection and FDC background.
  Treat OCR'd addresses/bytes as unverified; confirm against disassembly or hardware traces.
- `patches/vice-3.10-opencbm-filename.patch` — VICE 3.10 real-device OPEN filename fix.

Cross-references: `AGENTS.md`, `HARDWARE_WORKFLOW.md`, `docs/TOOLS.md`, `docs/CODE.md`.
