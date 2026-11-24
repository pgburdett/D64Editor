# Tools and workflows index

This is the practical index for D64Editor's CLI verbs, external utilities, and reproducible
workflows. Product behavior lives in `D64Editor.Core`; CLI dispatch lives in
`D64Editor.App/Cli.cs`. `AGENTS.md` is the short agent-facing authority; this document explains
how the tools are used together.

## 1. CLI verbs

### 1.1 General and image utilities

| Command | Purpose |
|---|---|
| `--help` / `-h` / `help` | Print the built-in CLI help. Runtime `--help` lists main verbs; `--cbm` with no arguments lists hardware subverbs. |
| `--convert <in> <out>` / `-c` | Convert G64/NIB/NBZ → D64, or copy a native image. `.nbz` is LZ77-decompressed to MNIB NIB first (`Core/Nbz.cs`). |
| `--to-gcr <d64> <out.g64\|out.nib>` | Encode a D64 into G64/NIB with native GCR tracks; byte-exact D64 round-trip. |
| `--list <image>` / `-l` | Print an image's directory. |
| `--info <image>` / `-i` | Show image/format information. |
| `--validate <image>` / `-v` | Validate cross-links and basic D64 issues. |
| `--export <image> <dir>` / `-e` | Export files from D64/T64/D71/D81/D80/D82/DNP to a directory. |
| `--print-dir <image>` | Print a text directory listing. |
| `--geos-info <d64> <file>` | Show GEOS/REL metadata and best-effort VLIR information. |
| `--run-script <script.txt> [workdir]` | Run a headless `ScriptEngine` automation script; exit code is propagated. |
| `<image>` | Open the image in the GUI. |
| (no arguments) | Start the GUI. |

### 1.2 Archive tools

| Command | Purpose |
|---|---|
| `--extract-archive <archive> <dir> [--images-only]` | Extract zip/tar/tar.gz/tgz, LHA/LZH, ARJ, or a C64 LHA SFX PRG (detected by content). |
| `--lha-list <archive.lha\|.lzh\|sfx.prg>` / `--lhz-list` | List LHA/LZH archive or C64 SFX PRG entries. |
| `--lha-extract <archive\|sfx.prg> <dir>` / `--lha-decompress` | Extract LHA/LZH or carved SFG payload. `--lhz-*` aliases also work. |
| `--lha-create <srcdir> <out.lha> [files]` / `--lha-compress` | Create a standard LH0/stored LHA archive. |
| `--lha-sfx <archive.lha> [stub.prg] <out.prg>` | Wrap LHA with a C64 PRG stub. Omitting the stub uses the embedded licensed `Assets/C64LhaSfxStub.prg`. |
| `--arj-list <archive.arj>` | List ARJ entries. |
| `--arj-extract <archive.arj> <dir>` / `--arj-decompress` | Extract ARJ. |
| `--arj-create <srcdir> <out.arj> [files]` / `--arj-compress` | Create ARJ (requires external `arj`). |

LHA creation is built in and always writes LH0/stored (portable, not compressed). LHA
listing/extraction requires external `lha` or `7z`; ARJ requires external `arj`.

### 1.3 CP/M tools

All default CP/M operations use the in-process `C128Cpm` engine. External `ctools`/`cpmtools`
are explicit opt-in cross-checks.

| Command | Purpose |
|---|---|
| `--cpm-extract <img> <dir> [--strict]` | Extract CP/M files; `--strict` scans only real directory slots. |
| `--cpm-copy <tmpl> <out> [--all\|--files A,B]` | Create CP/M image from a bootable template, keeping all or a subset of files. |
| `--cpm-format <tmpl> <out>` | Create an empty CP/M image from a template. |
| `--cpm-new <d64\|d71\|d81> <out> [NAME.EXT=hostfile ...]` | Create a data-only CP/M image from scratch. |
| `--cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]` | Create a bootable CP/M image from a donor system disk. |
| `--cpm-import-dir <donor\|new:d64\|new:d71\|new:d81> <srcdir> <out>` | Import a host directory with 8.3 name mapping. |
| `--ctools <format\|create\|import\|list\|extract> ...` | Explicit external `ctools` path. |
| `--cpm-cpmtools <format\|create\|import-dir\|list> ...` | Explicit external `cpmtools` path (needs C128 diskdef + libdsk). |

### 1.4 OpenCBM hardware verbs

| Command | Purpose |
|---|---|
| `--cbm detect` | List devices on the IEC bus. |
| `--cbm status [dev]` | Drive status string. |
| `--cbm dir [dev]` | Drive directory (read-only). |
| `--cbm command <dev> <cmd>` | Raw DOS command (e.g. `U0>M0`). |
| `--cbm format <name> <id> [dev]` | Format native 1571 (`U0>M1` + `N0:`). DESTRUCTIVE. |
| `--cbm read <dev> <image> [--d71]` | Read real disk → image (non-destructive). |
| `--cbm write <image> <dev> [--d71]` | Write image → real disk. DESTRUCTIVE. |
| `--cbm verify <image> <dev> [--d71]` | Write, read back, and byte-compare. |
| `--cbm sector <dev> <track> <sector> [outfile]` | Read a sector via the FDC job queue (firmware seek/read/decode). |
| `--cbm seek <dev> <track>` | Move head via FDC SEEK job; integer track only, no homing. |
| `--cbm surface <dev> [--from N] [--to N] [--out FILE] [--data-checks]` | Full track/sector surface scan with FDC error classification. |
| `--cbm tailgap <dev> [outfile]` | Read five tail-gap bytes via FDC JUMP job (Kracker Jax TGREAD port). |
| `--cbm gcrscan <dev> [outfile]` | Sample 256 raw GCR bytes from VIA2 on the current track; never moves the head. |
| `--cbm gaps <image.g64> [--from N] [--to N] [outfile]` | Offline GCR sync/gap/sector analysis. |
| `--cbm vice <command\|start\|stop> [options]` | Build/print, launch, or stop the VICE real-device passthrough command. PID saved in `/tmp/d64editor-vice.pid`. |
| `--cbm boot <image> <dev>` | Legacy alias for `vice start --data`. |

`--cbm vice` options: `--dev N` (default 8), `--data IMG` (drive 9 / `B:`), `--emu PATH`
(default `x128`), `--40col`, `--sound DEV` (default `dummy`), `--log FILE`, `--softgl`,
`--dry-run`.

### 1.5 Raw-media tools

| Command | Purpose |
|---|---|
| `--protect <base.d64> <out.g64>` | Synthesize EA PRODOS-style protected tracks (void T2, generated T36–40) into a G64. |
| `--writeraw <dev> <track> <hexpattern> <count>` | **DESTRUCTIVE.** Serial raw GCR track writer via VIA2 write mode. Blank disk only. Track >35 uses the short non-homing `Stepper.s` from track 35. |
| `--gcrread <dev> <track> <outfile> [--nosync] [--whole]` | Raw GCR chunk reader. `--whole` overlap/prefix-match stitching is synthetically validated but is **not** a valid single-revolution capture. |
| `--test-stitch <image.g64> <track>` | Validate the raw-GCR stitcher against a known G64 track. |
| `--raw2g64 <out.g64> <track:file> ...` | Wrap captured raw track files in a G64 container. |

---

## 2. External utilities

| Tool | Install / source | Use here |
|---|---|---|
| OpenCBM (`cbmctrl`, `d64copy`, `cbmformat`, `cbmforng`) | AUR `opencbm` on Arch. Copy `/etc/opencbm.conf.sample` back if needed, then `opencbm_plugin_helper_tools rebuild`; apply udev rules with `udevadm trigger`. | Sector/image I/O to a real 1541/1571 through XUM1541/ZoomFloppy. The libudev `keyctl` warning is harmless. |
| nibtools (`nibread`, `nibconv`, `nibsrqtest`) | AUR `nibtools-git` (`paru -S nibtools-git`). | Faithful raw GCR capture. `nibread` defaults to SRQ serial; `-P` forces parallel. `-e5 -h -v` retries, captures half-tracks, and logs anomalies. `nibconv` converts NIB → G64. |
| VICE 3.10 (`x64sc`, `x128`) | System package; patched build via `tools/build-patched-vice.sh` for real-drive tests. | Emulation with `-drive8truedrive`; real-device passthrough with `-devicebackend8 2 -busdevice8`. |
| `xa` / `da65` | Arch packages `xa`, `cc65`. | Assemble/disassemble 6502 drive code (`xa -o out.bin src.s`, `da65 --cpu 6502 --start-addr 0x0300 out.bin`). |
| `gdb`, `strace`, `ltrace` | system | Trace VICE/OpenCBM calls when diagnosing real-device hangs. |
| `spectacle`, `xdotool`, `pdftotext`, `tesseract` | system | Screenshots, keyboard input, PDF text extraction, OCR. |
| `lha` / `7z` / `arj` | system | LHA/LZH listing/extraction and ARJ archive operations. |
| ctools / cpmtools | See README "CP/M Support" | Optional cross-checks for C128 CP/M layout and file extraction. |

---

## 3. Reproducible workflows

### 3.1 Faithful copy-protected disk → G64 → VICE

```bash
tools/diagnostics/capture-srq.sh 8 arcticfox
# equivalent manual steps:
nibread -@xum1541 -D8 -e5 -h -v arcticfox.nib
nibconv arcticfox.nib arcticfox.g64
x64sc -model c64c -ntsc -power60 -drive8truedrive -8 arcticfox.g64
# at the BASIC prompt: LOAD"*",8,1
```

On the real EA disk, `nibread` reports weak GCR on T36 and `NOSYNC! [Unformatted Track]` on
T37 onward. The resulting G64 boots Arctic Fox to the "Enemy Review" screen in x64sc with true
drive emulation. A D64 intermediate does **not** preserve the protection.

### 3.2 Surface and gap analysis

```bash
dotnet run --project D64Editor.App -- --cbm surface 8 --from 1 --to 41 --out report.txt --data-checks
dotnet run --project D64Editor.App -- --cbm gaps image.g64 --from 1 --to 42
```

### 3.3 Dynamic drive-CPU trace

```bash
tools/build-patched-vice.sh
tools/diagnostics/launch-x64sc-g64.sh &
python3 tools/diagnostics/binmon_history.py
```

### 3.4 NBZ / Kracker Jax decoding

```bash
for f in *.nbz; do
  dotnet run --project D64Editor.App -- --convert "$f" "${f%.nbz}.d64"
done
tools/diagnostics/scan_kj.sh
```

### 3.5 LHA/SFX and ARJ archives

```bash
dotnet run --project D64Editor.App -- --lha-list disk.sfx.prg
dotnet run --project D64Editor.App -- --lha-extract disk.sfx.prg out/
dotnet run --project D64Editor.App -- --lha-create out/ archive.lha
dotnet run --project D64Editor.App -- --lha-sfx archive.lha boot.prg
dotnet run --project D64Editor.App -- --arj-create out/ archive.arj
dotnet run --project D64Editor.App -- --arj-extract archive.arj out-arj/
```

---

## 4. Safety

- Only one process may own the IEC/XUM1541 bus. Stop VICE before `cbmctrl`, `d64copy`, or
  `nibread`, and call `cbmctrl reset` (no device argument) afterward.
- `--writeraw`, `--cbm write`, `--cbm format`, `cbmforng`, and `cbmformat` are DESTRUCTIVE. Use
  blank/expendable disks and confirm targets.
- Never run `--writeraw` on an original disk. It is hardware-verified only for the void track 2;
  T36–40 stepping is code-complete but untested.
- Uploaded stepper/homing code caused head clatter and was removed. Use FDC SEEK (`--cbm seek`)
  for whole tracks, or the dedicated non-homing `Stepper.s` used by `RawWriter`.
- Custom drive code must turn the motor on through DOS `$F97E` plus a spin-up delay; raw
  `ORA #$04` on `$1C00` is not reliable.
- VICE GTK3 startup is flaky on this Xwayland session. Use software GL (`LIBGL_ALWAYS_SOFTWARE=1
  GALLIUM_DRIVER=llvmpipe`), retry, capture with `spectacle -b -o ...` (X11 root capture is
  black), and wait for prompts before typing with `xdotool`.

Cross-references: `AGENTS.md`, `HARDWARE_WORKFLOW.md`, `docs/CODE.md`,
`docs/drive-assembly/README.md`, `docs/disk-images/README.md`, `tools/diagnostics/README.md`.
