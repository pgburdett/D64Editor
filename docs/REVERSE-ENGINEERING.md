# Reverse Engineering Notes

This directory documents reverse-engineering work on C64/C128 software, primarily
CP/M games.  The detailed Nemesis work now lives in the sibling repository at
[`../Nemesis/docs/nemesis/`](../../Nemesis/docs/nemesis/README.md).

## Tools

- **Ghidra** (`/opt/ghidra`, v12.2 DEV) with Z80 processor support — used for
  initial program analysis.
- **z80dis** (Python) — pip package used for linear sweeps over CP/M `.COM` files
  (Ghidra's auto-analysis often misses data/output due to lack of code clues).
- **z80asm** — available for reassembling modified code.
- **VICE x128** (patched build in `/tmp/opencode/vice-patched`) — emulator for
  running the original game and verifying behavior.
