# Code map

This document maps the C# code in `D64Editor.Core` and `D64Editor.App`, with extra detail for
the disk-format, hardware, and copy-protection engines. `AGENTS.md` is the short agent-facing
authority for repository rules and CLI summaries; this file is the long-form explanation of
where code lives and why.

## Structure and invariants

- `D64Editor.sln` contains exactly two projects:
  - `D64Editor.Core` — library, `net10.0`, `LangVersion 12`, nullable enabled. All disk/image
    behavior, allocation logic, file-format parsing, CP/M logic, and hardware protocol wrappers
    live here.
  - `D64Editor.App` — Avalonia desktop UI, `net10.0`, `LangVersion 12`, nullable enabled,
    `Avalonia 11.3.4`. Entry point: `D64Editor.App/Program.cs`.
- `D64Editor.App` references `D64Editor.Core`; Core never references App.
- CLI dispatch lives in `D64Editor.App/Cli.cs`; script automation lives in
  `D64Editor.Core/ScriptEngine.cs` and is invoked by `--run-script`.
- Format/disk behavior belongs in Core. UI rendering and user interaction belong in App.
- The original VB6 source in `src/` (`Module1.bas`, `frmMain.frm`, etc.) is the reference for
  1541-format fidelity questions.

---

## Core: primary disk engine

| File | Role |
|---|---|
| `D64Image.cs` | Full D64 engine: geometry (35/40 track), BAM, directory entry parsing, file-chain walking, import/export, rename, raw snapshot/restore, and validation. `_data` holds the entire image; `GetRawDataCopy()`/`RestoreRawData(byte[])` support undo/redo by rebuilding directory/BAM/owners from the restored bytes. |
| `D64BlockAllocator.cs` | Free-block search using the standard spiral pattern starting near track 17. |
| `D64FileEntry.cs` | Directory-entry model, including file type, start track/sector, size, flags, and best-effort GEOS metadata fields. |
| `D64FileType.cs` / `D64ErrorCodes.cs` | CBM file-type and FDC/DOS status code definitions. |
| `IDiskImage.cs` | Minimal format-agnostic contract. `D64Image` is the only full implementation; other formats use dedicated readers/writers. |
| `RelFile.cs` | REL file model and raw record access. |
| `RelReader.cs` | REL side-sector chain reader. |
| `RelWrite.cs` | REL file creation and resize (grow/shrink record count, side/data-sector allocation, LSU updates). |

---

## Core: multi-format layer

| File | Role |
|---|---|
| `DiskFormats.cs` | Extension-based format detection and the `DiskFormat` enum (D64, D71, D81, D80, D82, T64, G64, NIB, DNP, D2M/D4M/DHD, CP/M, LBR, D40). |
| `CbmImage.cs` | Shared CBM-layout helpers used by D71/D81/D80/D82/DNP readers. |
| `D71Image.cs` | D71 double-sided 1571 image reader; implements `ICbmWritable` for scratch/unscratch/import. |
| `D81Image.cs` | D81 1581 image reader; implements `ICbmWritable`. |
| `D80D82Image.cs` | D80 (8050) / D82 (8250) readers; implement `ICbmWritable`. |
| `DnpImage.cs` | CMD DNP native-partition reader/writer with container slice support; implements `ICbmWritable`. |
| `DiskContainers.cs` | D2M/D4M/DHD partition-table parsing and container slice access. |
| `ContainerFormats.cs` | Shared container helpers and extension handling. |
| `T64Image.cs` | T64 tape archive reader/writer; mutable rebuild/save. |
| `CpmDisk.cs` | CP/M 2.2 directory reader. |
| `C128Cpm.cs` | C128 CP/M extraction, creation, template-based copy/format, donor-based bootable image creation, and host-directory import with 8.3 name mapping. All default `--cpm-*` and `cpm*` script commands use this in-process engine. |
| `CbmCatalog.cs` / `ImageCatalog.cs` | Directory/file enumeration across image types, used by viewers, search, and scripting. |
| `CbmScratch.cs` / `CbmImport.cs` | Shared scratch/unscratch and import behavior for CBM-layout images. |
| `G64Image.cs` | G64 container parse/write. `RawTrackOverrides` injects raw GCR tracks before `ToG64Bytes(d64)` rebuilds the container; half-track and out-of-range slots are preserved verbatim. |
| `NibImage.cs` | MNIB-1541-RAW NIB parse/encode; `ToD64Bytes()` decodes raw GCR tracks to sectors. |
| `Gcr.cs` | 4-to-5 GCR encode/decode, track encoding, and sector decoding (`DecodeTrackSectors`). |
| `Nbz.cs` | LZ77 decompressor for preservation-project `.nbz` images; `--convert` expands NBZ to MNIB NIB, then decodes to D64. |
| `GapScan.cs` | Offline raw-GCR analysis: counts `$FF` sync runs, measures non-sync gaps, decodes sectors, and flags anomalies against expected sector counts. |
| `RawG64.cs` | Wraps captured raw track byte arrays into a G64 container using a blank D64 base. |
| `Disassembler.cs` | 6502/6510 disassembler: linear and control-flow tracing modes, PRG load-address aware, branch/JMP/JSR target labels, KERNAL symbol annotations, BIT-skip overlap handling, optional extra trace entry points. |

Container-backed D64/D71/D81/DNP images keep `ContainerPath`, `Offset`, and `Size`; `Save()`
writes back into the original container. `CbmCatalog.Open{D64,D71,D81,Dnp}FromContainer` and
`PartitionsWindow` open partitions with write-back.

---

## Core: GCR write-back and protection tooling

| File | Role |
|---|---|
| `Gcr.EncodeTrack(d64, track, speedZone)` | Builds raw GCR track bytes from decoded D64 sectors. D64 → G64/NIB → D64 round-trips are byte-exact (683/683 sectors) for standard data; protected/half-track data is preserved verbatim when already present in a container. |
| `ProtectedTracks.cs` | Synthesizes EA PRODOS-style protected tracks: `GenerateTrack2()` (6,666 bytes, no `$FF` sync), `GenerateExtendedTrack(36..40)` (sync/filler and GCR noise). `Apply()` injects them through `G64Image.RawTrackOverrides`. Exposed by `--protect <base.d64> <out.g64>`. |
| `GcrScan.cs` | Non-destructive raw GCR sampler for the current track only. Uploads a 6502 routine that waits for SYNC and BYTE-READY, stores 256 bytes at drive RAM `$0400`, and reports status/phase at `$0300/$0301`. Never writes stepper bits. Exposed by `--cbm gcrscan`. |
| `SectorRead.cs` | FDC job-queue operations. READ uses job `$80`, track at `$0006`, sector at `$0007`, data buffer `$0300-$03FF`; SEEK uses job `$B0`. The drive firmware performs head movement, GCR decode, and checksum validation. Exposed by `--cbm sector` and `--cbm seek`. |
| `SurfaceScan.cs` | Loops every track/sector via FDC READ, classifies `$02/$03/$04/$05/$09/$0B` errors, and optionally applies data heuristics (`--data-checks`). Exposed by `--cbm surface`. |
| `TailGap.cs` | Port of Kracker Jax `TGREAD`: installs a JMP at `$0300`, queues FDC JUMP job `$D0`, reads five bytes from `$1C01` into `$0300-$0304`, and checks status at `$0305`. Exposed by `--cbm tailgap`. |
| `RawWriter.cs` + `Assets/RawWriter.s/.bin` | DESTRUCTIVE serial-cable raw GCR track writer. Host uploads pattern/count/mask to drive RAM, seeks via FDC, then launches the 93-byte VIA2 write-mode routine with M-E. Tracks >35 first run the short `Assets/Stepper.s` inward-step routine from track 35. Blank test disk only. Exposed by `--writeraw`. |
| `GcrRead.cs` + `Assets/GcrReader.s/.bin` | Raw GCR streaming reader. Drive code at `$0300` fills the full 1 KB `$0400-$07FF` buffer via BVC/SO byte-ready; motor-on uses DOS `$F97E` plus spin-up delay. `Stitch()` aligns 512-byte-overlap windows with a half-window reverse-searched probe. `--whole` stitching is synthetically validated but is **not** a valid single-revolution capture. `--test-stitch` validates against known G64 tracks. |
| `Assets/Stepper.s/.bin` | 82-byte safe short inward step for tracks 36–40. Two VIA phase increments per track, no homing, no zero-page writes. Used only by `RawWriter`. |

The earlier homing/stepper experiments caused head clatter and were removed. Do not resurrect
uploaded homing code. Whole-track movement uses FDC SEEK; only the dedicated non-homing
`Stepper.s` is used by the destructive raw writer.

---

## Core: hardware wrappers and archives

| File | Role |
|---|---|
| `OpenCbm.cs` | Process wrapper around `cbmctrl` and `d64copy` using `ProcessStartInfo.ArgumentList` (no shell). Supports detect/status/dir/command/format, image read/write/verify, and `UploadMemory`, `DownloadMemory`, `Execute` (M-E) for drive RAM work. Filters the harmless libudev `keyctl` warning. |
| `ArchiveExtract.cs` | Extraction for zip/tar/tar.gz/tgz, LHA/LZH, and ARJ. Detects C64 LHA SFX PRGs by content and routes them to `LegacyArchives`. Used by `--extract-archive` and the `archive` script command. |
| `LegacyArchives.cs` | LHA/LZH/ARJ list/create/extract. Writes standard LH0/stored LHA archives directly, carves C64 SFX PRGs by LHA header checksum, wraps LHA archives with the embedded or supplied C64 PRG stub, and delegates ARJ operations to external `arj`. |
| `ScriptEngine.cs` | Line-oriented script runner over Core with no UI dependency. `Run(text, wd)` / `RunFile(path, wd)` return `ScriptResult` (log, error, exit code). Commands include image lifecycle, directory/file operations, REL, CP/M, archives, GEOS, GCR export, and `echo`/`help`/`exit`. |
| `Nbz.cs` | LZ77 decompressor for `.nbz` → MNIB NIB → D64. |
| `CtoolsCopier.cs` | Optional wrapper for `cformat`/`ctools`, used only by explicit `--ctools` verbs for cross-checking. |
| `CpmToolsCopier.cs` | Optional wrapper for `mkfs.cpm`/`cpmcp`/`cpmls`; requires a valid C128 diskdef and libdsk-enabled build. |

---

## Embedded 6502 and fixture assets

All assets are embedded resources with logical names `D64Editor.Core.Assets.<file>`, declared in
`D64Editor.Core.csproj`. Source `.s` files are kept beside the binaries, with parallel reference
copies in `docs/drive-assembly/`.

| Binary | Source | Purpose |
|---|---|---|
| `CPM.SYS`, `BOOT.D71.bin` | Binary fixtures | Earlier CP/M boot experiment; not used by the current CP/M path. |
| `gcrsampler.bin` (126 B) | `GcrSampler.s` | Safe VIA2 raw sampler; waits for SYNC/BYTE-READY; never writes stepper bits. |
| `tailgap.bin` (87 B) | `TailGap.s` | TGREAD port; FDC JUMP-job gap reader. |
| `rawwriter.bin` (93 B) | `RawWriter.s` | VIA2 write-mode raw GCR writer (BVC/CLV/STA `$1C01`). |
| `stepper.bin` (82 B) | `Stepper.s` | Two VIA phase changes per track, inward from 35; no homing. |
| `gcrreader.bin` (69 B) | `GcrReader.s` | Raw GCR chunk reader with skip/count and SYNC wait; motor via `$F97E`. |
| `C64LhaSfxStub.prg` (3,721 B) | Recovered from a user-owned original disk | Default C64 PRG stub for `--lha-sfx`/`lhasfx`; included with the user's stated license to use it. |

The Epyx FastLoad CRT is deliberately **not** in this embedded-resource table. It is a commercial
cartridge fixture kept at `docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt` for local VICE
comparisons only; see `docs/fast-load/README.md`. Moving it from the repository root did not change
any build input or runtime lookup.

---

## App: UI and CLI

| File / area | Role |
|---|---|
| `Program.cs` | Avalonia application entry point. |
| `App.axaml(.cs)` | Application styles and service setup. |
| `MainWindow.axaml(.cs)` | Shell: menu/toolbar, tabbed MDI, active-document dispatch, menu commands. |
| `DiskDocumentView.axaml(.cs)` | Per-disk editor view: directory grid, BAM summary, actions, undo/redo. |
| `Cli.cs` | All CLI verbs, including `--cbm` subverbs and `--run-script`. |
| `AppSettings.cs` | Persisted settings: emulator command template, charset, update URL, OpenCBM tool path. |
| `Fonts.cs` / `Petscii*.cs` / `C64Palette.cs` | PETSCII font wiring, screen-code/PETSCII conversion, Pepto palette. |
| `DirListing.cs` / `DirectoryRow.cs` | C64-screen directory rendering. |
| `BamEditorWindow.axaml(.cs)` | BAM editor with set/error-code modes. |
| `FileChainWindow`, `BlockViewerWindow`, `SectorDumpWindow`, `HexViewerWindow` | Block and chain inspection. |
| `BasicViewerWindow`, `SeqViewerWindow` | File-content viewers (BASIC, SEQ/PETSCII). |
| `DisassemblyWindow` | 6502 disassembly viewer for PRG files and raw disk sectors/blocks: linear/trace mode, entry points, track/sector/block selection, C64/PETSCII display. |
| `GfxViewerWindow`, `PictureViewerWindow` | Charset/sprite and Koala/Art Studio hi-res viewers with zoom and BMP export. |
| `CpmViewerWindow`, `T64ViewerWindow`, `G64InfoWindow`, `NibInfoWindow`, `GeosInfoWindow`, `ExtendedImageWindow`, `PartitionsWindow` | Format-specific viewers. |
| `FindInDisksWindow`, `CompareDisksWindow`, `BatchWindow` | Search, compare, batch export/validate. |
| `OpenCbmWindow.axaml(.cs)` | Structured whole-disk transfer via `d64copy`: D64/D71 selection, device, transfer mode, command preview, confirmation for writes. Uses `ArgumentList`, no shell. |
| `RelViewerWindow`, `CreateRelWindow`, `FilenameBuilderWindow`, `FilePropertiesWindow` | REL and directory-entry editing tools. |
| `ConfigWindow`, `ConfigureEmulatorWindow`, `AboutWindow`, `ConfirmWindow`, `RenameWindow`, `CreateBlankWindow`, `ErrorNumbersWindow`, `CrossLinksWindow`, `ErrorBlocksWindow`, `MemoryWindow`, `MemoryCacheEntry.cs`, `SeparatorLibrary.cs` | Configuration, dialogs, memory/cache helpers, and error tools. |

### Undo/redo

`D64Image.GetRawDataCopy()` / `RestoreRawData(byte[])` provide snapshot/restore.
`DiskDocumentView` keeps `_undo` and `_redo` stacks capped at 50, calls `PushUndo()` before key
mutations, and exposes undo/redo through the Edit menu and `Ctrl+Z`/`Ctrl+Y`.

---

## Graphics decoders

- `GfxDecoder` decodes a 2 KiB charset to a 128×128 grid (128 glyphs) and 63-byte sprite data to
  24×21 hires or multicolor images.
- `BitmapDecoder` handles Koala/Art Studio hi-res picture data.
- `C64Colors` and `C64Palette` provide the standard C64 palette and Pepto-style rendering.
- Viewers support zoom and BMP save/export.

---

## Safety and verification

- One process owns the XUM1541/IEC bus at a time. Stop VICE before `cbmctrl`, `d64copy`, or
  `nibread`; call `cbmctrl reset` (no device argument) afterward.
- `--writeraw` is DESTRUCTIVE: blank expendable disk only. It is hardware-verified for the void
  track 2; the T36–40 stepper path is code-complete but untested.
- Faithful protected images require raw GCR capture (`nibread`/`nibconv` or a preservation
  archive). D64 → G64 regeneration loses physical timing and fails the EA loader.
- The solution has no automated test projects. Readers were manually verified with generated or
  synthetic images; hardware paths were verified with the real 1571/XUM1541 setup described in
  `HARDWARE_WORKFLOW.md` and the diagnostic docs.
