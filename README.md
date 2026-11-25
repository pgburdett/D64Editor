# D64Editor

D64Editor is a C#/.NET port and major expansion of the classic Windows disk editor **D64 Editor**
by Forrest Mook (GPL v2, 1999–2016, <https://www.d64editor.com>). It began as a faithful port of
the 1541 `.d64` engine and now includes a cross-platform Avalonia UI, a multi-format disk engine,
CLI and scripting automation, C128 CP/M support, OpenCBM hardware integration, and tooling for
copy-protection and raw-GCR research.

- **License:** GPL v2.
- **Stack:** C# 12 (`LangVersion 12`), `net10.0`, nullable reference types enabled, Avalonia
  11.3.4.
- **Solution:** `D64Editor.sln` with `D64Editor.Core` (library) and `D64Editor.App` (Avalonia
  desktop app + CLI). Core never depends on App.
- **Original source:** `src/` contains the VB6 source (`.frm`/`.bas`/`.vbp`) used as the 1541
  format-fidelity reference.
- **Truth policy:** when documentation and code/config disagree, prefer the code/config.

## Contents

1. [Overview](#overview)
2. [Build and run](#build-and-run)
3. [Project structure](#project-structure)
4. [Port scope and fidelity](#port-scope-and-fidelity)
5. [PETSCII display](#petscii-display)
6. [Multi-format engine](#multi-format-engine)
7. [Disk-editor features](#disk-editor-features)
8. [Viewers and editors](#viewers-and-editors)
9. [Search, compare, batch, and archives](#search-compare-batch-and-archives)
10. [Platform integration](#platform-integration)
11. [CP/M support](#cpm-support)
12. [REL support](#rel-support)
13. [Drag-and-drop and container import](#drag-and-drop-and-container-import)
14. [Undo/redo](#undoredo)
15. [CLI](#cli)
16. [Scripting](#scripting)
17. [Copy-protection and raw-media investigation](#copy-protection-and-raw-media-investigation)
18. [Verification and testing](#verification-and-testing)
19. [Known limitations](#known-limitations)
20. [Fidelity vs. cleanup](#fidelity-vs-cleanup)
21. [Contributing](#contributing)

---

## Overview

D64Editor opens and edits Commodore disk images, exports and imports files, displays directories in
a C64-style screen, and provides low-level BAM/sector/block tools. It also reads a wide range of
CBM and CP/M formats, writes selected formats back, and can talk to real 1541/1571 drives through
OpenCBM.

The main design rule is a strict Core/App split:

- `D64Editor.Core` owns all format, disk, allocation, archive, and hardware-protocol behavior.
- `D64Editor.App` owns rendering, window management, user interaction, and CLI dispatch.

This keeps format behavior testable through CLI/scripting without involving Avalonia.

---

## Build and run

Prerequisites:

- .NET SDK 10.0.x or newer. On Arch: `sudo pacman -S dotnet-sdk`; on Ubuntu:
  `sudo apt install dotnet-sdk-10.0`.
- No `global.json`; the projects target `net10.0` with C# 12 and nullable enabled.

Commands:

```bash
dotnet restore
dotnet build
dotnet build --no-restore
dotnet run --project D64Editor.App
dotnet test
dotnet test --no-build --verbosity normal
```

The solution currently has **no automated test projects**; CI runs `dotnet test` for completeness.
CI (`.github/workflows/dotnet.yml`) runs on `ubuntu-latest`, push/PR to `master`, SDK `10.0.x`:
restore → build --no-restore → test --no-build.

Local artifacts to avoid treating as source: `bin/`, `obj/`, `.idea/`,
`D64Editor.sln.DotSettings.user`, `d64editor-build-archives/`, untracked `*.d64`/`*.crt`.

---

## Project structure

```text
D64Editor.sln
├── D64Editor.Core/
│   ├── D64Image.cs              # D64 engine: geometry, BAM, directory, chains, snapshots
│   ├── D64BlockAllocator.cs     # free-block search
│   ├── RelFile/RelReader/RelWrite.cs  # REL files
│   ├── T64Image.cs              # T64 tape archive
│   ├── C128Cpm.cs               # in-process C128 CP/M engine
│   ├── D71Image/D81Image/D80D82Image/DnpImage/DiskContainers.cs  # multi-format layer
│   ├── G64Image/NibImage/Gcr/GapScan/Nbz/RawG64.cs               # raw GCR and containers
│   ├── OpenCbm/SectorRead/SurfaceScan/GcrScan/TailGap/RawWriter/GcrRead.cs
│   ├── ProtectedTracks.cs       # EA PRODOS-style G64 protection patterns
│   ├── ArchiveExtract/LegacyArchives.cs  # zip/tar/LHA/LZH/ARJ/SFX
│   ├── ScriptEngine.cs          # headless automation
│   └── Assets/                  # embedded 6502 binaries, CPM.SYS, C64LhaSfxStub.prg
├── D64Editor.App/
│   ├── Program.cs               # Avalonia entry point
│   ├── MainWindow.axaml(.cs)    # shell: menu/toolbar/tabbed MDI
│   ├── DiskDocumentView.axaml(.cs) # per-disk editor
│   ├── Cli.cs                   # CLI handler
│   ├── AppSettings.cs           # persisted settings
│   ├── Fonts.cs                 # PETSCII font wiring
│   ├── OpenCbmWindow.axaml(.cs) # structured d64copy dialog
│   ├── *.axaml(.cs)             # viewers/editors/dialogs
│   └── Roadmap.md               # feature roadmap
├── src/                         # original VB6 source (reference)
├── docs/                        # detailed developer references
├── tools/                       # standalone RE/diagnostic tools
├── packaging/                   # OS integration helpers
├── patches/                     # VICE 3.10 OpenCBM fix
├── scripts/                     # sample automation scripts
├── AGENTS.md                    # agent/developer guidance
└── README.md                    # this file
```

See `docs/CODE.md` for a complete code map.

---

## Port scope and fidelity

The original VB6 behavior is preserved where it encodes real 1541 format knowledge:

- `D64Image` load/save, disk name/ID, 35-track (683-block) and 40-track (768-block) geometry.
- BAM read/write ← `READBAM`/`BAMWRITE`.
- Track/sector ↔ flat block conversion ← `convertTrackSectorToBlock` / `blockNum2TrackandSector`.
  The VB double-precision/string-parsing approach was cleaned up to integer division, with the
  same math verified branch by branch.
- Directory read/write ← `LoadFile` / `writeOutDirectory` / `unAllocateDirSectors`.
- File types ← `GETFILETYPE`; file chains ← `ComputeBlockOwners`.
- Block read/write/clear, import/export ← `FileImport` / `FileExport`.
- PETSCII SEQ display ← `ViewSeqFile`.
- Error-info blocks (683 or 768 bytes) and the error-number table ← `ErrorCodeBlocks`.
- Separator "art" entries via zero-block DEL files (`AddArtEntry` ← `CopySeparatorsToD64`).

New, from-scratch behavior is clearly labeled:

- `ImportContainerFile` handles T64 and P00/S00/U00/R00 containers against published specs.
- GEOS directory metadata and best-effort VLIR inspection are new decodes, not direct ports.
- 40-track support is per-instance and detected from file size. Tracks 36–40 have no BAM
  representation on a real 1541, so those blocks are reported allocated; existing files there can
  be read, but the allocator never writes new files into them.

---

## PETSCII display

D64Editor renders PETSCII in two complementary ways:

- A bundled **C64 Pro Mono** TrueType font in `D64Editor.App/Assets/Fonts/`, wired through
  `Fonts.cs`, with PUA/screen-code mapping in `Petscii.cs`.
- A bundled VICE `chargen.bin` ROM bitmap in `D64Editor.App/Assets/`, rendered through
  `PetsciiScreen`/`PetsciiView` with a Pepto-style palette for exact ROM glyphs.

The C64-style directory, SEQ viewer, and BASIC listings use ROM-bitmap rendering. Hex and other
text views use the font/PUA path. A central **View → PETSCII Lower/Upper Case** toggle switches
between set 1 (upper/graphics) and set 2 (lower/upper) everywhere; the setting is persisted.

To change the font, drop it into `Assets/Fonts/`, update `FileName` and `FamilyName` in `Fonts.cs`
(find the internal family with `fc-scan <font> | grep family`), and ensure
`PetsciiFontAvailable` is set. `chargen.bin` is four 1 KB screen-code-indexed tables (two charsets,
normal and reverse).

---

## Multi-format engine

In addition to full D64 editing, Core includes:

| Format | Support |
|---|---|
| D71, D81, D80, D82, DNP | Directory viewers, Hex/BASIC viewers, Export-PRG, entry editing, scratch with block-freeing, unscratch, import through `ICbmWritable`/`CbmScratch`/`CbmImport`. DNP supports in-image edits and native container write-back. |
| D2M/D4M/DHD | CMD container partition tables, partition open, per-partition write-back. |
| G64 / NIB | Raw-GCR containers decoded to D64 on open, with structural viewers and fallback. GCR write-back is byte-exact for standard D64→G64/NIB→D64 round-trips; protection fidelity is incremental. |
| T64 | Reader, viewer, mutable rebuild/save. |
| CP/M 2.2 and `.LBR` | Best-effort directory readers/viewers. |
| C128 CP/M (`.d64/.d71/.d81`) | In-process extraction, creation, template copy, donor-based bootable creation, host-directory import. See [CP/M support](#cpm-support). |

`IDiskImage` is implemented by `D64Image`; other formats use dedicated readers. `ICbmWritable`
is implemented by D71/D81/D80/D82/DNP. Container-backed images preserve `ContainerPath`,
`Offset`, and `Size` and write back on `Save()`.

---

## Disk-editor features

- Import/export files, disk name/ID display and editing, directory listing, blocks-free and
  file-count summary.
- Save-after-import, scratch/unscratch/wipe, remove entry, move entry top/bottom, File Properties.
- Filename Builder: separator-art snippets stored as zero-block DEL entries, standalone or launched
  from File Properties; persists to the user's app-data folder.
- REL file creation, viewing, in-place record editing, and resize.
- BAM editor with set/error-code modes, live FREE/ALLOCATED summary, legend, and "view block"
  navigation.
- Sector/block viewer with hex + PETSCII, link/owner display, dump-to-file, and navigation.
- File Chain viewer: ordered track/sector/block list with next links and last-sector used-byte
  count.
- Cross-links viewer, error blocks list, error-number table.
- Undo/redo for common D64 mutations (`Ctrl+Z` / `Ctrl+Y`, stack cap 50).

Menus: `Disk`, `File`, `Edit`, `View`, `Window`, `Help`. The toolbar includes C64 color selectors.

---

## Viewers and editors

| Viewer | Purpose |
|---|---|
| Hex | `AAAA <16 hex bytes> <PETSCII>`, base from the 2-byte load address; PETSCII/ASCII toggle; C64 ROM glyphs. |
| BASIC | C64 BASIC V2 de-tokenizer. |
| Disassembly | 6502 disassembler. |
| SEQ | 40/80-column ASCII/PETSCII text viewer with "Save to Text". |
| REL | Side-sector chain and fixed-length records, in-place editing, resize. |
| Picture | Koala/Art Studio hi-res, zoom, BMP export. |
| Charset / Sprite | 256-glyph charset and 24×21 sprite (hires/multicolor), zoom, BMP export. |
| GEOS info | Metadata bytes, info-block dump, best-effort VLIR index. |
| File chain | Ordered block chain with next links and used-byte counts. |
| T64, Extended Image, CP/M, G64/NIB info, Partitions | Format-specific structural viewers. |

`GfxDecoder` converts 2 KiB charsets to 128×128 grids and 63-byte sprites to image data;
`BitmapDecoder` handles hi-res picture formats.

---

## Search, compare, batch, and archives

- **Find in Disks:** filename and file-content search across supported formats (`ImageCatalog`,
  `DiskContentSearcher`).
- **Compare Disks:** header/directory/BAM diff plus differing-block list.
- **Batch export / Batch Validate:** folder-wide export and report-only validation.
- **Archive extraction:** `--extract-archive` accepts zip, tar, tar.gz/tgz, LHA/LZH, ARJ, and C64
  LHA SFX PRGs (detected by content). Dedicated verbs create/list/extract LHA/LZH/SFX and ARJ.
  LHA creation writes LH0/stored archives; listing/extraction uses external `lha` or `7z`; ARJ
  uses external `arj`; SFX wrapping uses the embedded licensed `C64LhaSfxStub.prg` or a supplied
  stub. See `docs/TOOLS.md`.

---

## Platform integration

### Emulator launch

`Run → Configure Emulators…` sets a command template. Placeholders:

| Placeholder | Meaning |
|---|---|
| `{disk}` | Autostart target (selected file, or disk/`*`). |
| `{image}` | Disk image path for mounting. |
| `{appdir}` | Application directory. |
| `{app}` | Application executable. |
| `{cwd}` | Current working directory. |

Default behavior mounts drive 8: `x64sc -8 "{image}" -autostart "{disk}"`. No selection starts
the disk/`*`; one selected file starts `disk:NAME`. `$0801` BASIC programs add
`-autostartprgmode 1`. `.crt` cartridges use `-cartcrt`. Paths are shell-quoted safely, and
substitution works both for whole quoted tokens and inside quotes.

### OpenCBM transfer

`CBM → OpenCBM Transfer…` opens a structured `d64copy` dialog with D64/D71 (`-2`) selection, device
number, transfer mode, live command preview, and PATH tool detection. Writes require confirmation;
reads are non-destructive. Arguments are passed via `ProcessStartInfo.ArgumentList` (no shell).

Setup on Arch:

```bash
paru -S opencbm
# if /etc/opencbm.conf is missing, copy /etc/opencbm.conf.sample to it
sudo opencbm_plugin_helper_tools rebuild
sudo udevadm control --reload-rules && sudo udevadm trigger
cbmctrl detect
cbmctrl status 8
```

The harmless `undefined symbol: keyctl` warning from libudev can be ignored.

### File associations

`packaging/install-associations.sh <exe>` installs Linux `.desktop`/MIME associations under
`~/.local`; `--uninstall` reverses it. `packaging/d64-file-association.reg` does the same on
Windows after editing the install path.

---

## CP/M support

All default CP/M operations use the in-process `C128Cpm` engine. C128 CP/M is CP/M 3.0 ("CP/M
Plus") and uses 7-bit ASCII names/text, not PETSCII.

### Geometry

- Zones: T1–17 = 21 sectors, T18–24 = 19, T25–30 = 18, T31–35 = 17; repeated on side 2 for D71.
- Interleave: `sec = (5 * nr) % sectrk` (from ctools `bios.cc`).
- Disk type is detected from T1/S0: `"CBM"` plus byte 255 `== 0xFF` means double-sided (D71, 2K
  blocks, two extents/entry); otherwise C128 single-sided (1K) or C64 (1K).
- Directory occupies logical blocks 0–1, eight 32-byte entries per sector.
- Extents: `records = RC >= 0x80 ? 128*(EX+1) : 128*EX + RC`, with `EX` modulo extents-per-entry
  (from ctools `fcb.cc`).

### Commands

| Command | Purpose |
|---|---|
| `--cpm-extract <img> <dir> [--strict]` | Extract files; `--strict` scans only real directory slots. |
| `--cpm-copy <tmpl> <out> [--all\|--files A,B]` | Copy from a bootable template, keeping all or a subset. |
| `--cpm-format <tmpl> <out>` | Create an empty image from a template. |
| `--cpm-new <d64\|d71\|d81> <out> [NAME.EXT=hostfile ...]` | Create a data-only image from scratch. |
| `--cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]` | Create a bootable image from a donor system disk. |
| `--cpm-import-dir <donor\|new:*> <srcdir> <out>` | Import a host directory with 8.3 name mapping. |
| `--ctools ...`, `--cpm-cpmtools ...` | Explicit opt-in external cross-checks. |

Script equivalents: `cpmextract`, `cpmcopy`, `cpmformat`, `cpmnew`, `cpmbootable`,
`cpmimportdir`.

### Verified workflow example

```bash
dotnet build
dotnet run --project D64Editor.App -- --cpm-new d71 /tmp/mbasic.d71 \
    MBASIC.COM=/path/to/mbasic.com WUMP.BAS=/path/to/wump.bas
dotnet run --project D64Editor.App -- --cpm-extract /tmp/mbasic.d71 /tmp/verify
cmp /tmp/verify/mbasic.com /path/to/mbasic.com
x128 -8 /path/to/c128-cpm-system.d64 -9 /tmp/mbasic.d71
# At A>: B:  then  MBASIC WUMP.BAS
```

`--cpm-new` images are data-only; the system disk goes on drive 8 (`A:`), data on drive 9 (`B:`).
Use `--cpm-bootable` with a donor for a self-booting disk. Never drop `CCP.COM` from a donor.

### Terminal compatibility

- MBASIC and BASIC programs: work.
- Infocom Zork II (`ZORK2.COM` + `ZORK2.DAT`, configured with `2DEFAULT`/`CONF2`): played
  interactively under x128 CP/M 3.0.
- Full-screen CP/M 2.2 programs emitting VT/ANSI cursor sequences: may render garbled under the
  stock console. This is a console/BIOS limitation, not file corruption; files extract byte-exact.

### Real-media workflow

See `HARDWARE_WORKFLOW.md` for the complete verified flow: build a bootable image, boot locally,
write with `d64copy --no-progress -w` (D64, `U0>M0`) or `-w -2` (D71, `U0>M1`), read back and
`cmp`, then boot from the physical drive with
`x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy`.

---

## REL support

- Viewer shows the side-sector chain and fixed-length records; records can be edited in place.
- `File → New REL File…` creates REL files; the viewer's Resize button grows/shrinks record count.
- Format: side sectors index data sectors; each data sector holds 254 bytes of record data, with
  the last sector padded using the LSU byte count.
- Core: `RelFile`, `RelReader`, `RelWrite`.

---

## Drag-and-drop and container import

Dropping a `.d64` on the main window opens it. Dropping any other file — including
`.t64`/`.p00`/`.s00`/`.u00`/`.r00` and multiple files at once — imports into the currently open
disk. This is not a line-by-line port of `DragDrop.bas` (Windows OLE APIs do not map to Avalonia),
but the resulting behavior matches. The same container detection is used by "Import File…" and
Memory's "Add File…". Inter-tab/window disk-to-disk drag remains limited.

---

## Undo/redo

`D64Image.GetRawDataCopy()` / `RestoreRawData(byte[])` provide snapshot/restore.
`DiskDocumentView` keeps `_undo` and `_redo` stacks capped at 50 and pushes snapshots before key
mutations (import, scratch/unscratch, wipe, remove entry, move, REL create). Undo/redo is available
from the Edit menu and `Ctrl+Z` / `Ctrl+Y`. Coverage is not comprehensive across every edit path.

---

## CLI

Passing arguments invokes the CLI; a recognized verb prevents the GUI from starting. A single image
path opens in the GUI; no arguments starts the GUI.

### Usage

```text
D64Editor --help
D64Editor --list <image>
D64Editor --info <image>
D64Editor --validate <image>
D64Editor --export <image> <dir>
D64Editor --convert <in> <out>
D64Editor --to-gcr <d64> <out.g64|out.nib>
D64Editor --print-dir <image>
D64Editor --geos-info <d64> <file>
D64Editor --run-script <script.txt> [workdir]
D64Editor --extract-archive <zip|tar|tgz|lha|lzh|arj|sfx.prg> <dir> [--images-only]
D64Editor --lha-list <archive.lha|.lzh|sfx.prg>
D64Editor --lha-extract <archive|sfx.prg> <dir>
D64Editor --lha-create <srcdir> <out.lha> [file ...]
D64Editor --lha-sfx <archive.lha> [stub.prg] <out.prg>
D64Editor --arj-list <archive.arj>
D64Editor --arj-extract <archive.arj> <dir>
D64Editor --arj-create <srcdir> <out.arj> [file ...]
D64Editor --cpm-extract <image> <dir> [--strict]
D64Editor --cpm-copy <tmpl> <out> [--all|--files A,B]
D64Editor --cpm-format <tmpl> <out>
D64Editor --cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]
D64Editor --cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]
D64Editor --cpm-import-dir <donor|new:d64|new:d71|new:d81> <srcdir> <out>
D64Editor --ctools <format|create|import|list|extract> ...
D64Editor --cpm-cpmtools <format|create|import-dir|list> ...
D64Editor --cbm <detect|status|dir|command|format|read|write|verify|vice|boot|gaps|gcrscan|sector|seek|surface|tailgap> ...
D64Editor --cbm mode <dev> 1541|1571
D64Editor --cbm capture <dev> <out.nib> [--retries N] [--no-halftracks] [--adapter NAME]
D64Editor --cbm preserve <dev> <out.nib> [--mode 1541|1571|keep] [--report FILE] [--g64 FILE] [options]
D64Editor --cbm g64 <in.nib> <out.g64>
D64Editor --cbm-console [dev]       Interactive hardware console (readline history, quit to exit)
D64Editor --cbm u1 <dev> <track> <sector> [channel]  Raw DOS U1 read; prints drive status
D64Editor --cbm seek <dev> <track> [--fast]        --fast temporarily enables ROM accelerated stepping
D64Editor --cbm benchseek <dev> <trackA> <trackB> [count] [--fast]  Time repeated seeks
D64Editor --cbm fastseek <dev> on|off|status       Persistent fast-step threshold toggle (reset restores default)
D64Editor --cbm vice <command|start|stop> [--dev N] [--data IMG] [--emu PATH] [--softgl] [--dry-run]
D64Editor --gcrread <dev> <track> <outfile> [--nosync] [--whole]
D64Editor --test-stitch <image.g64> <track>
D64Editor --raw2g64 <out.g64> <track:file> ...
D64Editor --protect <base.d64> <out.g64>
D64Editor --writeraw <dev> <track> <hexpattern> <count>
D64Editor <image>          (opens in GUI)
D64Editor                  (no args: starts GUI)
```

Aliases: `-h`, `-l`, `-e`, `-i`, `-v`, `-c`, `--lha-decompress`, `--lha-compress`,
`--arj-decompress`, `--arj-compress`.

Runtime `--help` lists the main verbs; `--cbm` with no arguments lists the hardware subverbs.

### Hardware preservation from the console

All three console entry points drive the same read-only Core workflow, so a protected disk can be
investigated and captured without leaving D64Editor:

1. **One command** — `--cbm preserve 8 disk.nib --mode 1541 --report scan.txt --g64 disk.g64`.
   Detects the drive, sets the mode, reads status and directory, runs the FDC surface scan,
   captures a raw NIB image with `nibread` (half-tracks on by default), parses the protection
   findings, and writes a text report. `--fast` lowers MINSTP for the surface scan;
   use `--skip-surface` to only capture, or `--capture`-style subcommands (`--cbm capture`,
   `--cbm mode`, `--cbm g64`) for individual steps.
2. **Interactive** — `--cbm-console [dev]` opens a prompt with readline-style up/down history.
   Commands: `detect`, `status`, `dir`, `mode 1541|1571`, `surface`, `capture`, `preserve`,
   `g64`, `gaps`, `sector`, `u1`, `seek [--fast]`, `fastseek`, `use <dev>`, `history`, `quit`. It works with piped input too.
3. **Scripts** — `cbmdetect`, `cbmstatus`, `cbmdir`, `cbmmode`, `cbmsurface`, `cbmpreserve`,
   `cbmgaps`, `cbmu1`, `cbmfastseek` are available to `--run-script`.

### Faster head seeks

The 1541/1571 ROM has an accelerated stepping mode, but its threshold (`MINSTP`, drive RAM `$0064`)
ships as 200 half-track steps — larger than any seek on a 35-track disk — so it never engages.
`--cbm fastseek <dev> on` lowers it to 12 (about 6 tracks), enabling the factory-calibrated
accel/run/decel ramp; `off` restores 200 (a drive reset does too). `--cbm seek ... --fast` applies
the patch just for one seek and always restores the original value. The console exposes the same
`fastseek on|off|status` and `seek <track> --fast` commands.

Only the threshold is changed — no uploaded stepper code and no direct VIA phase writes — so the
ROM still handles acceleration, deceleration and settling. The threshold byte is written with
`cbmctrl upload` (bare M-W commands were intermittently dropped by the real drive) and verified
by reading it back.

Measured on a real 1571, device 8, 8 seeks between tracks 18 and 30 (12 tracks / 24 half-steps):

| MINSTP | avg / seek | total (8 seeks) |
|---|---|---|
| `$C8` (default) | 366 ms | 2931 ms |
| `$0C` (fast) | 270 ms | 2157 ms |

That's about **35% faster** (~96 ms saved per seek). Motor spin-up and FDC job-queue polling still
dominate short moves, so use the persistent `on` mode for bulk scanning rather than per-seek
`--fast` (which pays extra tool round-trips). `--cbm benchseek <dev> <trackA> <trackB> [count]`
reproduces the measurement.

`--cbm surface ... --fast` and `--cbm preserve ... --fast` apply the same threshold to an entire
scan and always restore the previous value; measured on 76 sectors, scans are ~0.3% faster because
per-sector FDC reads dominate. All OpenCBM tool calls have a hard timeout (30 s for
interactive DOS commands), so a wedged U1 read can no longer block the console indefinitely;
`ReadSectorDirect()` also closes its channel in a `finally`.

`preserve`/`capture` require nibtools (`nibread`/`nibconv`) on PATH in addition to OpenCBM.
The NIB file is the preservation master; the optional G64 is a playable derivative for VICE
(conversion can normalize oversized protected tracks). Nothing in this workflow writes to the disk.

Note: 1541 mode (`U0>M0`) addresses only side 0. Reading the reverse side of a physically flipped
disk works in 1541 mode; a true two-sided 1571 disk needs `U0>M1` and a D71-oriented workflow.

---

## Scripting

`D64Editor.Core/ScriptEngine.cs` is a line-oriented runner with no UI dependencies. Run it with
`--run-script <file> [workdir]`. Relative paths resolve against the script folder or the optional
workdir. Each command is logged; the first error stops execution; `exit [code]` sets the process
exit code.

Commands:

| Category | Commands |
|---|---|
| Images | `open`, `close`, `save`, `new`, `convert`, `togcr` |
| Directory/files | `list`, `info`, `validate`, `rename`, `renamedisk`, `import`, `export`, `exportdir`, `scratch`, `unscratch`, `wipe`, `move` |
| REL | `relnew`, `relresize`, `relwrite` |
| CP/M | `cpmextract`, `cpmcopy`, `cpmformat`, `cpmnew`, `cpmbootable`, `cpmimportdir` |
| Archives | `archive`, `lhaextract`, `lhacreate`, `lhasfx`, `arjextract`, `arjcreate` |
| Hardware | `cbmdetect`, `cbmstatus`, `cbmdir`, `cbmmode`, `cbmsurface`, `cbmpreserve`, `cbmgaps`, `cbmu1`, `cbmfastseek` |
| Other | `geosinfo`, `echo`, `help`, `exit` |

Example:

```text
echo === Creating blank disk ===
new out/automation.d64 "MY DISK" ED
info
import test.prg HELLO prg
import test.seq NOTES seq
list
validate
exportdir out/exported
scratch HELLO
unscratch HELLO prg
rename NOTES README
list
save
exit 0
```

Sample scripts live in `scripts/`.

---

## Copy-protection and raw-media investigation

D64Editor includes extensive tooling for analyzing EA PRODOS, PirateSlayer, and related C64
copy protection. The short version:

- Standard sector images (`.d64`) cannot represent protected tracks, gaps, or timing.
- Faithful preservation uses SRQ nibbling:
  `nibread -@xum1541 -D8 -e5 -h -v disk.nib` → `nibconv disk.nib disk.g64` →
  `x64sc -drive8truedrive -8 disk.g64` → `LOAD"*",8,1`.
- The verified Arctic Fox capture boots to the "Enemy Review" screen.
- The RUSSIA (`GM`) disk uses a simpler check: `M-W` sets `$006A=$85` to disable head bump,
  then `U1:5,0,18,18` expects `23,read error,18,18` on a deliberately checksum-bad sector at
  T18/S18. See `docs/drive-assembly/README.md` §13 for the full trace.
  - The raw NIB master reaches the title screen through real-drive passthrough; the `nibconv` G64
    derivative loads briefly and then returns to BASIC/`READY` in `x64sc`.
  - The bad S18 checksum survives in every tested G64 variant, but the NIB's 22 T18 headers are
    reduced to 19 by `nibconv`; handler variants (`x-vmax`, `c-cinema`, `g-securi`, `p-pirate`,
    `r-rapid`, `v-vorpal`, bad-sector simulations, etc.) did not fix playback. Rotation/timing and
    REVCNT/head-bump emulation remain the likely differences.
  - Wrapping raw NIB tracks with `--raw2g64` is not a substitute: wrong zones give
    `?FILE NOT FOUND ERROR`, correct zones with full-length tracks still cannot read the directory,
    and rotating/trimming raw bytes to `nibconv` lengths introduces invalid GCR symbols.
  - A tokenized BASIC U1 probe can be autostarted with the G64 on device 8 to read the emulated DOS
    status; use lowercase keyboard injection or `-autostart`, avoid `-autostartprgmode 1`, and keep
    the target on device 8. See `HARDWARE_WORKFLOW.md`.
- `--cbm gaps` provides offline sync/gap/sector anomaly analysis; `--cbm surface` scans the real
  drive through FDC jobs; `--cbm tailgap` reads five tail-gap bytes; `--cbm gcrscan` samples raw
  GCR on the current track.
- The manual sequence above is now available as `--cbm preserve` (one-shot), `--cbm-console`
  (interactive), and the `cbm*` script commands. `CbmPreserve` parses nibread's verbose track map
  into a protection summary (unformatted tracks, weak GCR, non-standard sectors, long reads).

Detailed references:

- `HARDWARE_WORKFLOW.md` — OpenCBM/CP/M real-media workflow and VICE environment.
- `docs/drive-assembly/README.md` — VIA registers, FDC jobs, TGREAD, fastloader traces, protected
  track byte patterns, raw writer/reader.
- `docs/disk-images/README.md` and `docs/disk-images/generated/README.md` — reference and captured
  images.
- `tools/diagnostics/README.md` — VICE launchers, binary-monitor clients, and analysis scripts.

**Safety:** `--writeraw` is DESTRUCTIVE — blank disks only, never an original. One process owns the
IEC bus; stop VICE before `cbmctrl`/`d64copy`/`nibread` and call `cbmctrl reset` afterward.

---

## Verification and testing

Verification was performed manually and with the CLI; the solution has no automated test suite.
Claims include:

- Synthetic/generated-image verification for D64, D71, D81, D80, D82, DNP, D2M/D4M/DHD, G64,
  NIB, T64, CP/M 2.2, and C128 CP/M (D64).
- Byte-exact CP/M extraction versus ctools: 19 files from `MBASIC.d71`, 10 from the D64 system
  disk, strict and non-strict modes.
- End-to-end WUMPUS run from `--cpm-new d71`; Zork II played interactively under x128.
- D64 → G64/NIB → D64 round-trip byte-exact for 683/683 standard sectors.
- Real-media D64 and D71 writes with byte-exact readback and physical-drive CP/M boot.
- Tail-gap bytes `60 24 46 23 99` verified on real hardware.
- Faithful `nibread`/`nibconv` capture boots Arctic Fox to "Enemy Review" in VICE.

Unverified: CP/M `.d81` against real 1581 media, raw-GCR hardware stitching, and the T36–40
raw-stepper writer path.

---

## Known limitations

- G64/NIB per-zone gap/interleave tuning, copy-protected track re-encoding, and half-track editing
  remain incremental. GCR write-back is byte-exact for standard data but cannot reconstruct
  protection from a D64 intermediate.
- CMD container write-back works for all partition types; richer in-container editing is
  incremental.
- From-scratch CP/M images are data-only; `.d81` CP/M writing is in-process only and unverified on
  real 1581 media.
- Full-screen CP/M 2.2 programs can render poorly under the stock CP/M 3.0 console.
- Inter-tab/window disk-to-disk drag-and-drop is limited.
- Undo/redo coverage is not comprehensive across every edit path.
- Native OS print dialog and built-in updater/installer are not implemented; printable text export
  and a release-feed check exist instead.
- Deeper GEOS disk/application-data and VLIR workflows are best-effort.
- `--gcrread --whole` stitching is not a valid single-revolution capture; use SRQ nibtools for
  faithful full-track images.
- `nibconv` G64 derivatives do not reproduce the RUSSIA disk's rotation/timing and REVCNT/head-bump
  behaviour: the NIB is the only faithful master for that title. `--raw2g64` raw-track wrapping
  cannot replace `nibconv` normalization for playable images.
- `--writeraw` is hardware-verified only for void track 2; T36–40 stepping is code-complete but
  untested.

---

## Fidelity vs. cleanup

VB6 artifacts such as scattered globals, double-precision block arithmetic, and hand-placed UI
controls were cleaned up while preserving genuine 1541 format behavior. Where behavior was
rewritten from published specs (GEOS metadata, container import, CP/M layout, graphics decoders),
it is documented as a new implementation rather than a line-by-line port.

---

## Contributing

- Keep format/disk behavior in Core and UI in App.
- Use `src/` as the reference for 1541-format fidelity questions.
- Prefer code/config as executable truth over prose when they disagree.
- Add Core automation to `ScriptEngine` and document it here.
- Avoid committing local artifacts (`bin/`, `obj/`, `.idea/`, `*.user`, untracked images).
- PETSCII font wiring centers on `Fonts.cs` and `Assets/Fonts/`.
- Feature status lives in `D64Editor.App/Roadmap.md`.
- Commit with detailed messages; push only when requested.

## Credits and license

D64Editor is GPL v2. Original D64 Editor by Forrest Mook. The port and extensions are maintained
in this repository. Bundled reference fixtures and third-party tools retain their own licenses;
see `docs/` and `patches/` for details.
