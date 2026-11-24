# AGENTS.md

## Project overview
- C#/.NET port of "D64 Editor" (1541 `.d64` disk editor) with Avalonia UI.
- Solution: `D64Editor.sln` with two projects:
  - `D64Editor.Core/` — disk/image engine and format logic (library, `net10.0`, `LangVersion 12`, nullable enabled).
  - `D64Editor.App/` — Avalonia desktop UI (Exe, `net10.0`, `LangVersion 12`, nullable enabled, `Avalonia 11.3.4`). Entry point: `D64Editor.App/Program.cs`.
- `D64Editor.App` references `D64Editor.Core`. Keep format/disk behavior changes in `Core` and UI changes in `App` unless there is a clear reason.

## Commands
- Restore: `dotnet restore`
- Build: `dotnet build` (or `dotnet build --no-restore` after restore)
- Run UI: `dotnet run --project D64Editor.App`
- Test: `dotnet test` / `dotnet test --no-build --verbosity normal` (the D64Editor solution currently has no test projects)

## CI / environment notes
- `.github/workflows/dotnet.yml` runs on `ubuntu-latest` (push/PR to `master`) with .NET SDK `10.0.x`: `dotnet restore`, `dotnet build --no-restore`, `dotnet test --no-build --verbosity normal`.
- Projects target `net10.0` (`LangVersion 12`, nullable enabled), no `global.json`.
- README documents local build with `dotnet-sdk` installed (`sudo pacman -S dotnet-sdk` example) and running `D64Editor.App`.

## Repo-specific guidance
- `src/` contains the original VB6 source (`.frm`/`.bas`/`.vbp`, e.g. `Module1.bas`, `frmMain.frm`); use it as the reference for 1541-format behavior and port fidelity questions. `README.md` summarizes port scope and intentional differences.
- Prefer the code/config as executable truth over README/CI prose when they disagree (e.g., target framework vs CI SDK version).
- Workspace may contain local artifacts (`bin/`, `obj/`, `.idea/`, `D64Editor.sln.DotSettings.user`, `d64editor-build-archives/`); avoid treating them as source of truth or modifying them unless requested.
- PETSCII font wiring is documented in `README.md` and centered around `D64Editor.App/Fonts.cs` and `D64Editor.App/Assets/Fonts/` (font file + internal family name + `PetsciiFontAvailable` flag).
- Multi-format layer lives in Core (`D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `C128Cpm`, `DiskContainers`) with shared helpers (`CbmImage`, `Gcr`, `DiskFormats`, `CbmCatalog`, `ImageCatalog`, `CbmScratch`, `CbmImport`). Readers were manually verified during development with generated/synthetic images; there is no automated test suite in the solution. Feature/phase roadmap is tracked in `D64Editor.App/Roadmap.md`.

## Architecture notes
- **Core/App separation:** All disk-format behavior, allocation logic, and file I/O belong in `D64Editor.Core`. UI rendering, window management, and user interaction belong in `D64Editor.App`. The App references Core; Core has no dependency on App.
- **IDiskImage interface:** Implemented by `D64Image` for format-agnostic operations. CBM-layout images (D71/D81/D80/D82/DNP) implement `ICbmWritable` for scratch/unscratch/import.
- **D64Image raw data:** Internal `_data` byte array holds the full image. `GetRawDataCopy()`/`RestoreRawData(byte[])` provide snapshot/restore for undo/redo. Directory/BAM/owners are rebuilt from `_data` on restore.
- **Nullable enabled:** All projects use nullable reference types. Use `?` annotations and null-conditional access where appropriate.
- **LangVersion 12:** C# 12 features (primary constructors, collection expressions, etc.) are available.

## Key files and their roles
- `D64Editor.Core/D64Image.cs` — D64 geometry, BAM, directory, chain logic, import/export, snapshot/restore.
- `D64Editor.Core/D64BlockAllocator.cs` — Free-block search (spiral pattern from track 17).
- `D64Editor.Core/RelFile.cs` — REL file model, reading, in-place record writing.
- `D64Editor.Core/RelReader.cs` — REL side-sector chain reader.
- `D64Editor.Core/RelWrite.cs` — REL file create and resize (grow/shrink record count).
- `D64Editor.Core/T64Image.cs` — T64 tape archive reader/writer (mutable, rebuild/save).
- `D64Editor.Core/C128Cpm.cs` — C128 CP/M extraction, creation, template copy/format. Implements correct C128 speed-zone + interleave-skew sector translation (ctools `bios.cc`) and EX/RC extent record encoding (ctools `fcb.cc`). **In-process by default for all CP/M operations**; no external tool is used by the `--cpm-*` CLI verbs or `cpm*` script commands.
- `D64Editor.Core/CtoolsCopier.cs` — optional wrapper for `cformat`/`ctools` used only by the explicit `--ctools` verb (kept for cross-checking). `FormatImage`, `PutFiles`, `GetFiles`, `GetFilesAll`, `ListFiles`, `CreateAndPopulate`.
- `D64Editor.Core/CpmToolsCopier.cs` — wrapper for `mkfs.cpm`/`cpmcp`/`cpmls` (needs a valid C128 diskdef + libdsk for variable geometry).
- `D64Editor.Core/DnpImage.cs` — DNP reader/writer with container slice support.
- `D64Editor.Core/DiskContainers.cs` — D2M/D4M/DHD partition table parsing.
- `D64Editor.Core/G64Image.cs` / `NibImage.cs` — GCR image parsing and D64 conversion.
- `D64Editor.Core/CpmDisk.cs` — CP/M 2.2 directory reader.
- `D64Editor.Core/OpenCbm.cs` — OpenCBM process wrapper (`cbmctrl`/`d64copy`): detect/status/dir/command/format, image read/write/verify, plus `UploadMemory`/`DownloadMemory`/`Execute` for drive RAM and M-E.
- `D64Editor.Core/GcrScan.cs` — raw GCR sampler via uploaded 6502 code (current track only; never moves the head).
- `D64Editor.Core/SectorRead.cs` — decoded sector read and integer-track SEEK via FDC job queue (no uploaded code; safe head movement).
- `D64Editor.Core/SurfaceScan.cs` — full track/sector FDC surface scan with error classification (sync/header/checksum).
- `D64Editor.Core/TailGap.cs` — port of Kracker Jax TGREAD: 5 tail-gap bytes via FDC JUMP job.
- `D64Editor.Core/Nbz.cs` — LZ77 decompressor for preservation-project `.nbz` images; `--convert` turns them into NIB→D64.
- `D64Editor.Core/GapScan.cs` — offline raw-GCR sync/gap/sector analysis of G64 images.
- `D64Editor.Core/ProtectedTracks.cs` — synthesizes EA PRODOS-style protected tracks (void T2, T36-40) into G64 via `G64Image.RawTrackOverrides`.
- `D64Editor.Core/RawWriter.cs` + `Assets/RawWriter.s/.bin` — serial-cable raw GCR track writer (drive-side VIA2 write mode; DESTRUCTIVE, blank disk only).
- `D64Editor.Core/Assets/Stepper.s/.bin` — safe short inward step for tracks 36-40 (no homing), used by RawWriter.
- `D64Editor.Core/GcrRead.cs` + `Assets/GcrReader.s/.bin` — raw GCR chunk reader (BVC/SO byte-ready; motor via `$F97E`). `--whole` stitching is not a valid single-revolution capture.
- `D64Editor.Core/RawG64.cs` — wraps captured raw track files into a G64 container (`--raw2g64`).
- `D64Editor.App/DiskDocumentView.axaml.cs` — Per-disk editor view (directory, actions, undo/redo).
- `D64Editor.App/MainWindow.axaml.cs` — Shell: menu/toolbar/tabbed MDI, active-document dispatch.
- `D64Editor.App/Cli.cs` — CLI handler (all commands).
- `D64Editor.App/AppSettings.cs` — Persisted settings (emulator command, charset, etc.).
- `D64Editor.App/Fonts.cs` — PETSCII font wiring.
- `D64Editor.App/Roadmap.md` — Feature roadmap and phase status.

## CLI commands (summary)
All CLI commands are implemented in `D64Editor.App/Cli.cs`. See README.md for full usage.
- `--help` / `-h` / `help` — Show help text.
- `--convert <in> <out>` / `-c` — Convert G64/NIB to D64 (or copy native image).
- `--list <image>` / `-l` — Print an image's directory.
- `--export <image> <dir>` / `-e` — Export files from D64/T64/D71/D81/D80/D82/DNP to a directory.
- `--info <image>` / `-i` — Show image/format information.
- `--validate <image>` / `-v` — Validate/report cross-links and basic issues (D64).
- `--cpm-extract <img> <dir> [--strict]` — Extract CP/M files in-process (`--strict` narrows scanning to real directory slots).
- `--cpm-copy <tmpl> <out> [--all|--files A,B]` — Create CP/M image from template (keep all or subset).
- `--cpm-format <tmpl> <out>` — Create empty CP/M image from template (clears directory; template-based).
- `--cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]` — Create CP/M image with files, in-process.
- `--cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]` — Bootable CP/M image from a system-disk donor.
- `--cpm-import-dir <donor|new:*> <srcdir> <out>` — Import a whole host directory with 8.3 name mapping.
- `--ctools <format|create|import|list|extract> ...` — Optional external ctools-backed CP/M operations (explicit opt-in; kept for cross-checking).
- `--cpm-cpmtools <format|create|import-dir|list> ...` — cpmtools-backed CP/M operations.
- `--cbm <detect|status|dir|command|format|read|write|verify|vice|boot> ...` — OpenCBM real-hardware operations. `vice <command|start|stop>` builds/prints, launches or stops the VICE passthrough command (`--dev`, `--data`, `--emu`, `--40col`, `--sound`, `--log`, `--softgl`, `--dry-run`); PID saved in `/tmp/d64editor-vice.pid`.
- `--cbm sector <dev> <track> <sector>` / `seek <dev> <track>` — FDC job-queue sector read / integer-track head move (drive firmware does the work).
- `--cbm surface <dev> [--from N] [--to N] [--out FILE] [--data-checks]` — full track/sector surface scan via FDC READ jobs; reports sync/header/checksum errors.
- `--cbm gaps <image.g64> [--from N] [--to N] [outfile]` — offline raw GCR sync/gap/sector analysis.
- `--cbm tailgap <dev> [outfile]` — read 5 tail-gap bytes via FDC JUMP job (Kracker Jax TGREAD port).
- `--cbm gcrscan <dev> [outfile]` — sample raw GCR on the current track via uploaded 6502 code (never moves the head).
- `--gcrread <dev> <track> <outfile> [--nosync] [--whole]` — raw GCR chunk reader; `--whole` uses overlap/prefix-match stitching (512-byte half-window probe, reverse search; synthetically validated, hardware drift untested). `--test-stitch <image.g64> <track>` validates the stitcher against a known track.
- `--raw2g64 <out.g64> <track:file> ...` — wrap captured raw track files into a G64 container.
- `--protect <base.d64> <out.g64>` — inject reproducible EA PRODOS protected tracks into a G64.
- `--writeraw <dev> <track> <hexpattern> <count>` — DESTRUCTIVE serial raw GCR track writer (blank test disk only; track >35 uses safe 82-byte stepper).
- `--convert` also accepts `.nbz`: LZ77-decompress to MNIB NIB, then NIB→D64 decode.
- `--extract-archive <zip|tar|tar.gz|tgz|lha|lzh|arj|sfx.prg> <dir> [--images-only]` — Extract archive contents; C64 LHA SFX PRGs are detected by content.
- `--lha-list/--lha-extract/--lha-create/--lha-sfx` and `--arj-list/--arj-extract/--arj-create` — Legacy LHA/LZH/SFX and ARJ archive operations. LHA creation is built-in LH0/stored; listing/extraction uses external `lha` or `7z`; ARJ uses external `arj`; SFX creation needs a user-supplied PRG stub.
- `--print-dir <image>` — Print directory listing (text, D64).
- `--geos-info <d64> <file>` — Show GEOS/REL metadata and best-effort VLIR info for a file (D64).
- `<image>` (no verb) — Open the image in the GUI.
- (no arguments) — Start the GUI.

## Multi-format layer
- `D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `C128Cpm`, `DiskContainers` — format readers.
- Shared helpers: `CbmImage`, `Gcr`, `DiskFormats`, `CbmCatalog`, `ImageCatalog`, `CbmScratch`, `CbmImport`, `RelFile`, `RelReader`, `RelWrite`.
- Each reader verified against synthetic images of its format.
- `ICbmWritable` interface: implemented by D71/D81/D80/D82/DNP for scratch/unscratch/import.

## CP/M support notes
- C128 CP/M uses 7-bit ASCII (not PETSCII) for names and text.
- CP/M-on-`.d64` cases are detected (boot signature `CBM`) and opened in the CP/M viewer.
- **Geometry (critical):** logical CP/M blocks map to physical sectors through 4 speed zones with interleave `sec = (5*nr) % sectrk` (source: ctools `bios.cc`). Disk type is detected from boot sector at T1/S0: `"CBM"` + byte 255 `== 0xFF` = double-sided (D71, 2K blocks, 2 extents/entry); otherwise C128 SS (1K blocks) or C64 (1K blocks). Directory occupies logical blocks 0..1 (8 entries/sector, 32-byte entries). `EX`/`RC` encode total record count: `RC>=0x80 ? 128*(EX+1) : 128*EX + RC`, EX modulo extents-per-entry (ctools `fcb.cc`).
- **Tools:** All `--cpm-*` CLI verbs and `cpm*` script commands use the in-process `C128Cpm` directly — no external tool is required for CP/M disk access. `ctools` (`https://github.com/mist64/ctools`) and `cpmtools` (`CpmToolsCopier`, needs a valid C128 diskdef + libdsk-enabled build) are optional, reachable only through the explicit `--ctools`/`--cpm-cpmtools` verbs for cross-checking. Full install/how-to guide is in README's "CP/M Support" section.
- **Verified end-to-end:** `--cpm-new d71` (MBASIC.COM + WUMP.BAS) written on Linux, mounted as drive 9 (`B:`) in x128 with a system disk on drive 8 (`A:`), `MBASIC WUMP.BAS` ran WUMPUS to completion. Byte-exact extraction against ctools on both D71 (19 files) and D64 (10 files).
- **Terminal compatibility (verified):** line-oriented CP/M programs run fine under x128 CP/M 3.0 — MBASIC/WUMPUS and Infocom **Zork II** (INTERLOGIC `ZORK2.COM`+`ZORK2.DAT`, configured with generic `2DEFAULT`/`CONF2` config program) were staged via `--ctools create d71` and played interactively. Full-screen cursor-positioning CP/M 2.2 programs may render garbled under the stock console because the C128 CP/M 3.0 BIOS lacks their cursor-addressing escape sequences. This is a console/terminal limitation, not file corruption — staged files extract byte-exact.
- Template-based create/copy/format: relies on compatible bootable template. From-scratch images are data-only (no boot code/CCP/BDOS); use `--cpm-bootable` with a donor or the two-drive emulator setup.
- `.d81` CP/M mapping is in-process only; the optional external `ctools`/`cpmtools` paths cover `.d64`/`.d71`.

## OpenCBM integration notes
- `D64Editor.App/OpenCbmWindow.axaml(.cs)` — structured whole-disk transfer via `d64copy` (image-level), with D64/D71 (`-2`) selection, device number, transfer mode, live command preview, PATH tool detection, and confirmation for destructive writes. Arguments passed via `ProcessStartInfo.ArgumentList` (no shell). Reads are non-destructive. Old version was a generic `bash -c` command-template runner with a wrong `cbmcopy` default (that tool is for files, not images).
- Setup: OpenCBM is AUR `opencbm` on Arch; its PKGBUILD renames `/etc/opencbm.conf` to `.sample` (copy back + `opencbm_plugin_helper_tools rebuild`); USB cable udev rules need a replug or `udevadm trigger`; verify with `cbmctrl detect` / `cbmctrl status 8`. Harmless `keyctl` warning from libudev can be ignored.
- CP/M real-media: `d64copy -2 image.d71 8` writes a D71 CP/M image to a real 1571; `d64copy -2 8 image.d71` reads back (non-destructive). Verified warp/no-`-t` writes: `cbmctrl command 8 'U0>M0'` then `d64copy --no-progress -w image.d64 8` (D64), and `cbmctrl command 8 'U0>M1'` then `d64copy --no-progress -w -2 image.d71 8` (D71, true two-sided mode). `-w` is implicit for `.d71`. Readback with the matching `-2`/no-`-2` and `cmp` must be byte-exact. `cbmctrl reset` (no device argument) resets all drives on the IEC bus before/after emulator use.
- Bootable CP/M image build: a full donor system disk is too full for new files — use `--cpm-copy <donor> <base> --files CPM+.SYS,CCP.COM,DIR.COM,PIP.COM` to free blocks, then `--cpm-bootable <base> <out> NAME.EXT=hostfile ...`. Never drop `CCP.COM` (CP/M Plus will not boot). Both D64 and a 349,696-byte D71 container (single-sided payload on side 1, blank side 2) were verified byte-exact on real media.

## Emulator integration notes
- Template placeholders: `{disk}`, `{image}`, `{appdir}`, `{app}`, `{cwd}`.
- Substitution handles whole quoted tokens and inside-quoted arguments.
- VICE-compatible lowercase filename mapping via `DirectoryFormatter.PetsciiToEmulatorName`.
- BASIC programs ($0801) get `-autostartprgmode 1` appended.
- Cartridge paths use `-cartcrt` (not `-cartepyx`) for `.crt` files.
- VICE 3.10's real-device backend calls `cbm_open(..., NULL, 0)` and sends the filename as later raw writes, so LOAD from a real drive hangs in `raw_read`/`get_eoi`. Source fix: `patches/vice-3.10-opencbm-filename.patch` (buffer name bytes, pass them to `cbm_open` at UNLISTEN); EOI/reads themselves verified working via direct libopencbm tests. Custom fast loaders (drive code uploaded with `M-W` and executed with `M-E`, bit-banging VIA at `$1800`) remain unsupported by the high-level backend. See `HARDWARE_WORKFLOW.md`.
- Real-drive passthrough (VICE 3.10+ with OpenCBM): `x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy` boots CP/M from the physical disk; add `-9 <data.d64>` for a local B: drive. D64Editor builds this via `--cbm vice command|start|stop` (options: `--dev`, `--data`, `--emu`, `--40col`, `--sound`, `--log`, `--softgl`, `--dry-run`); only one process may own the IEC bus, so stop VICE before `cbmctrl`/`d64copy` and vice versa. On this Xwayland session VICE GTK3 startup is flaky: use `--softgl` (`LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe`) and retry; capture with `spectacle -b -o ...` (X11 root capture is black), send keys with `xdotool type --window ...`, wait for the `A>` prompt first.

## Copy-protection / raw-media investigation (Arctic Fox, EA PRODOS)

- `docs/drive-assembly/README.md` — detailed 6502 drive-code reference: VIA registers, byte-ready handshaking, FDC job queue, original TGREAD disassembly, fastloader trace results, protected-track byte histograms, serial raw writer/reader, and safety notes. Experimental `.s/.bin` files and captured samples live in that folder.
- `docs/disk-images/README.md` + `docs/disk-images/generated/README.md` — all reference/generated disk images. Faithful raw G64 of Arctic Fox: `docs/disk-images/arcticfox[ea_1986](!).g64` (preservation) and `docs/disk-images/generated/arcticfox-srq.g64` (captured from the real disk with nibread).
- `tools/diagnostics/README.md` — VICE launchers and binary-monitor Python clients for C64 IEC and drive-CPU tracing; archive search helpers. Requires patched VICE from `patches/` + `tools/build-patched-vice.sh`.
- EA PRODOS/PirateSlayer uses custom track layouts, gap manipulation, raw GCR reads from `$1C01`, rotation/tail-gap timing checks and killer/unformatted regions. Confirmed on the EA disk: void track 2 (no `$FF` sync, 0 sectors), T36 with hundreds of sync runs/no sectors (weak GCR in nibread), T37-40 unformatted, half-track 35.5 activity.
- **Faithful capture workflow (verified):** real 1571 on device 8 → `nibread -@xum1541 -D8 -e5 -h -v arcticfox.nib` (nibtools-git from AUR; default mode is SRQ serial, `-P` would force parallel) → `nibconv ... .g64` → VICE x64sc with `-drive8truedrive` → `LOAD"*",8,1` boots Arctic Fox to its "Enemy Review" tank screen. D64→G64 sector conversion does NOT preserve protection.
- OpenCBM/XUM1541 has no raw-track API (`cbm_*_burst_*` fails with `LIBUSB_ERROR_NO_DEVICE`), but SRQ nibtools works. Motor control for custom drive code must use DOS `$F97E` plus spin-up delay; raw `ORA #$04` is not enough.
- **Safety:** never run `--writeraw` on an original; use a blank disk. `--cbm seek` uses FDC jobs (no homing). Early uploaded *homing* experiments caused head clatter and were abandoned (one phase change = half track; two per track; zone bits in `$1C00` bits 5-6); the short `Stepper.s` used by `--writeraw` for tracks 36–40 is a separate, non-homing routine.

## Graphics viewers
- `GfxViewerWindow` (charset/sprite) and `PictureViewerWindow` (Koala/Art Studio hi-res) support zoom and BMP save/export.
- `GfxDecoder` decodes charset (2 KiB → 128×128) and sprite (63 bytes → 24×21 hires or multicolor).

## Undo/Redo implementation
- `D64Image.GetRawDataCopy()` / `RestoreRawData(byte[])` provide snapshot/restore.
- `DiskDocumentView` maintains `_undo` and `_redo` stacks (capped at 50).
- `PushUndo()` called before key mutations.
- Available via Edit menu and `Ctrl+Z`/`Ctrl+Y`.

## Scripting (headless automation)
- `D64Editor.Core/ScriptEngine.cs` — line-oriented script runner over the Core engine; no UI dependencies. `Run(text, wd)` / `RunFile(path, wd)` return `ScriptResult` (log, error, exit code).
- CLI entry point: `D64Editor.App/Cli.cs` `--run-script`. `Environment.ExitCode` propagates the script exit code.
- `D64Editor.Core/ArchiveExtract.cs` — zip/tar/tar.gz/tgz/LHA/LZH/ARJ extraction used by the `archive` script command (and the CLI `--extract-archive`).
- `D64Editor.Core/LegacyArchives.cs` — LHA/LZH/SFX and ARJ list/create/extract; LHA creation writes LH0/stored archives, C64 SFX PRGs are carved by LHA header checksum, ARJ delegates to external `arj`.
- Script commands cover: image lifecycle (open/close/save/new/convert), directory/file ops (list/info/validate/import/export/exportdir/rename/renamedisk/scratch/unscratch/wipe/move), REL (relnew/relresize/relwrite), CP/M (cpmextract/cpmcopy/cpmformat), archive (including LHA/LZH/ARJ/C64 SFX), lhaextract/lhacreate/lhasfx/arjextract/arjcreate, geosinfo, echo/help/exit.
- Sample scripts live in `scripts/`. When adding Core automation, expose it through ScriptEngine and document it in README's Scripting section.
- `D64Image.RenameDisk(name,id)` was added for headless disk-header renaming.

## GCR write-back / container / CP/M notes
- `Gcr.EncodeTrack(d64, track, speedZone)` builds raw GCR track bytes; `G64Image.ToG64Bytes`/`FromD64` and `NibImage.ToNibBytes`/`FromD64` build containers. Round-trip D64→G64/NIB→D64 is byte-exact (verified 683/683 sectors). Half-track/copy-protected data is preserved verbatim, not re-encoded.
- Container-backed images: `D64Image`/`D71Image`/`D81Image`/`DnpImage` all have `TryLoadFromContainer` + `ContainerPath/Offset/Size`; `Save()` writes back into the container. `CbmCatalog.Open{D64,D71,D81,Dnp}FromContainer` and `PartitionsWindow` open partitions directly with write-back.
- `C128Cpm.ReadHostDirectory`/`HostNameToCpm83`/`ImportDirectoryFromBootDonor` import a whole host dir to CP/M with 8.3 name mapping and collision suffixes; `CpmImportReport` logs mappings/skips.
- `C128Cpm.FormatBlank`/`WriteFiles`/`FormatAndWrite` create CP/M data images from scratch. The block map uses ctools speed-zone+skew translation (built after writing the boot sector so disk-type detection sees `"CBM"`); directory at logical blocks 0..1. Byte-exact round-trips verified against ctools, but images are data-only (no boot tracks). The embedded `Assets/CPM.SYS` + `Assets/BOOT.D71.bin` are an earlier experimental boot attempt that did not yield a bootable image; prefer `--cpm-bootable` with a donor.
- `--ctools`/`--cpm-cpmtools` are explicit opt-in verbs that delegate to external tools; all default `--cpm-*` CLI paths (and `cpm*` script commands) are in-process via `C128Cpm`.

## Known limitations
- G64/NIB GCR fidelity tuning remains incremental (per-zone interleave/gaps, copy-protected track re-encoding); write-back round-trips byte-exact.
- CMD container write-back works for all partition types; richer in-container editing workflows remain incremental.
- From-scratch CP/M (`--cpm-new`) is data-only (no boot code/CCP/BDOS). File layout is correct (byte-exact extraction verified vs ctools as a cross-check); use `--cpm-bootable` with a system-disk donor or the two-drive x128 setup (system disk on 8, data disk on 9) for running files. `.d81` writing is in-process only.
- Legacy CP/M 2.2 full-screen programs may still render poorly under the stock CP/M 3.0 console; alternate BDOS/BIOS builds and terminal definitions may be required.
- Broader inter-tab/window disk-to-disk drag-and-drop not fully implemented.
- Undo/redo coverage not comprehensive across all edit paths.
- Native OS print dialog and built-in updater/installer not implemented (platform-dependent).
- Deeper GEOS disk/application-data/VLIR workflows remain best-effort.
- Raw GCR chunk reads work, but `--gcrread --whole` window stitching is not a valid single-revolution capture; faithful full-track images need SRQ nibtools (`nibread`/`nibconv`).
- `--writeraw` is DESTRUCTIVE and only hardware-verified for the void track 2; the track 36-40 stepper path is code-complete but untested.

## Local artifacts
- **Committed fixtures:** `128cpm/` (CP/M reference images + docs used for verification), `D64Editor.Core/Assets/` (CPM.SYS, BOOT.D71.bin and the 6502 `.bin` assets — required embedded resources), and `Epyx_FastLoad_1984_v2-b11b3b.crt` (test cartridge) are tracked in the repo. Treat them as test/reference data, not source of truth for format behavior.
- **Investigation docs/fixtures:** `docs/drive-assembly/` (6502 sources, disassembly, captured samples), `docs/disk-images/` (reference and generated NIB/G64/D64 images), `tools/diagnostics/` (VICE launchers and trace clients), `patches/` (VICE OpenCBM fix + build script). These are reference material for the copy-protection work, not product code.
- **External tooling:** nibtools (`nibread`, `nibconv`, `nibsrqtest`) installed from AUR `nibtools-git`; OpenCBM + XUM1541 firmware on device 8. `nibread` defaults to SRQ serial mode; `-P` forces parallel.
- **Ignored build artifacts (do not commit):** `bin/`, `obj/`, `.idea/`, `*.user`, `d64editor-build-archives/`.