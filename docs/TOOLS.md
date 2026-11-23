# Tools and workflows index

This document indexes every command-line tool, external utility, and reproducible workflow used
to diagnose copy-protected Commodore disks and drive behaviour. Product CLI commands are
implemented in `D64Editor.App/Cli.cs`; helper scripts live in `tools/diagnostics/`.

## 1. D64Editor CLI verbs

### General / images
| Command | Purpose |
|---|---|
| `--help` / `-h` / `help` | Help text. |
| `--convert <in> <out>` / `-c` | Convert G64/NIB/**NBZ** → D64, or copy native images. `.nbz` is LZ77-decompressed to MNIB NIB first (`Core/Nbz.cs`). |
| `--to-gcr <d64> <out.g64\|out.nib>` | Encode D64 into G64/NIB with native GCR tracks. |
| `--list` / `-l`, `--info` / `-i`, `--validate` / `-v` | Directory, image info, D64 validation. |
| `--export` / `-e`, `--extract-archive`, `--print-dir`, `--geos-info` | File/archive/dump utilities. |
| `--run-script` | Headless ScriptEngine automation. |

### CP/M (in-process by default)
| Command | Purpose |
|---|---|
| `--cpm-extract <img> <dir> [--strict]` | Extract CP/M files via `C128Cpm` (no ctools). |
| `--cpm-copy`, `--cpm-format`, `--cpm-new`, `--cpm-bootable`, `--cpm-import-dir` | CP/M image creation/copy/import. |
| `--ctools ...`, `--cpm-cpmtools ...` | Explicit opt-in external tool paths (kept for cross-checking). |

### OpenCBM hardware (`--cbm ...`)
| Command | Purpose |
|---|---|
| `detect`, `status`, `dir`, `command`, `format` | `cbmctrl` wrappers. |
| `read <dev> <image> [--d71]`, `write <image> <dev> [--d71]`, `verify ...` | Whole-image I/O via `d64copy`. Warp/no `-t` by default. |
| `vice <command\|start\|stop> [options]` | Build/print/launch/stop VICE real-device command (`-devicebackendN 2 -busdeviceN`). |
| `boot <image> <dev>` | Legacy alias for `vice start --data`. |
| `sector <dev> <track> <sector>` | FDC job-queue READ (drive firmware seeks/reads/decodes). |
| `seek <dev> <track>` | FDC job-queue SEEK (integer track; safe head movement). |
| `surface <dev> [--from N] [--to N] [--out FILE] [--data-checks]` | Full track/sector scan; classifies FDC errors (no sync, header/checksum, ID mismatch). |
| `tailgap <dev> [outfile]` | Runs ported Kracker Jax TGREAD: 5 gap bytes via FDC JUMP job. |
| `gcrscan <dev> [outfile]` | Uploads 6502 sampler; reads 256 raw GCR bytes from VIA2 on current track. Never moves the head. |
| `gaps <image.g64> [--from N] [--to N] [outfile]` | Offline GCR sync-run/gap-length/sector-decode analysis. |

### Raw-media tools (DESTRUCTIVE where noted)
| Command | Purpose |
|---|---|
| `--protect <base.d64> <out.g64>` | Synthesize EA PRODOS protected tracks (void T2, T36-40) into a G64. |
| `--writeraw <dev> <track> <hexpattern> <count>` | **DESTRUCTIVE.** Serial raw GCR track writer via drive-side VIA2 write mode. Blank disk only. Track >35 uses the safe 82-byte stepper from track 35. |
| `--gcrread <dev> <track> <outfile> [--nosync] [--whole]` | Raw GCR chunk reader; `--whole` window stitching is not a valid single-revolution capture. |
| `--raw2g64 <out.g64> <track:file> ...` | Wrap captured raw track files into a G64 container. |

## 2. External utilities

| Tool | Install / source | Use here |
|---|---|---|
| OpenCBM (`cbmctrl`, `d64copy`, `cbmformat`, `cbmforng`) | AUR `opencbm` on Arch; copy `/etc/opencbm.conf.sample`, `opencbm_plugin_helper_tools rebuild`, udev rules + `udevadm trigger`. | Sector/image I/O to real 1541/1571 via XUM1541/ZoomFloppy. Harmless `keyctl` warning from libudev. |
| nibtools (`nibread`, `nibconv`, `nibsrqtest`) | AUR `nibtools-git` (`paru -S nibtools-git`). | **Faithful raw GCR capture**: `nibread` defaults to SRQ serial mode (`-P` forces parallel), `-e5` retries, `-h` half-tracks, `-v` verbose track log. `nibconv` → G64. |
| VICE 3.10 (`x64sc`, `x128`) | System package; patched build from `tools/build-patched-vice.sh` for real-drive tests. | Emulator with `-drive8truedrive`; `-devicebackend8 2 -busdevice8` for real-device passthrough. |
| `xa` / `da65` | Arch packages `xa`, `cc65`. | Assemble/disassemble 6502 drive code (`D64Editor.Core/Assets/*.s`). |
| `gdb`, `strace`, `ltrace` | system | Attach to VICE for OpenCBM call tracing (needs root/ptrace). |
| `spectacle`, `xdotool`, `pdftotext`, `tesseract` | system | Screenshots, keyboard input, PDF text/OCR of reference books. |

## 3. Reproducible workflows

### Faithful copy-protected disk → G64 → VICE (verified)
```bash
tools/diagnostics/capture-srq.sh 8 arcticfox
# equivalent to:
nibread -@xum1541 -D8 -e5 -h -v arcticfox.nib
nibconv arcticfox.nib arcticfox.g64
x64sc -model c64c -ntsc -power60 -drive8truedrive -8 arcticfox.g64
# LOAD"*",8,1  (PETSCII uppercase bytes via binary monitor keybuf)
```
Result on the real EA disk: `nibread` reports T36 `weakgcr:5188/6107`, T37+ `NOSYNC! [Unformatted Track]`; the converted G64 boots Arctic Fox to its "Enemy Review" tank screen.

### Surface scan of a real disk
```bash
dotnet run --project D64Editor.App -- --cbm surface 8 --from 1 --to 35 --out report.txt
```

### GCR gap/sync analysis of an image
```bash
dotnet run --project D64Editor.App -- --cbm gaps image.g64 --from 1 --to 42
```

### Patched VICE + dynamic trace
```bash
tools/build-patched-vice.sh
tools/diagnostics/launch-x64sc-g64.sh &
python3 tools/diagnostics/binmon_history.py
```

### Kracker Jax / NBZ archive decoding
```bash
for f in *.nbz; do dotnet run --project D64Editor.App -- --convert "$f" "${f%.nbz}.d64"; done
tools/diagnostics/scan_kj.sh
```

See the linked sections in `AGENTS.md`, `HARDWARE_WORKFLOW.md`,
`docs/drive-assembly/README.md`, and `docs/disk-images/README.md` for the analysis behind each step.

## 4. Safety
- Only one process may own the IEC bus: stop VICE before `cbmctrl`/`d64copy`/`nibread` and vice versa. `cbmctrl reset` (no device argument) resets all drives afterward.
- `--writeraw` is DESTRUCTIVE: use a blank, expendable disk, never an original.
- Uploaded stepper code caused head clatter and was removed. Use FDC SEEK for whole tracks; the short stepper in `Assets/Stepper.s` is only used by `--writeraw` for tracks 36-40.
- Motor control for custom drive code must use DOS `$F97E` plus a spin-up delay.
