# Changelog

All notable changes to D64 Editor (C#/.NET port) are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased] — Drive-Side Raw GCR Sampler (code complete, untested)

### Added
- `--cbm gcrscan <dev> <track> [outfile]` and `Core/GcrScan.cs`: uploads a 6502 sampler to drive RAM, delta-steps to the target track, reads 256 raw GCR bytes from VIA2, and downloads them for anomaly analysis.
- Embedded 6502 source/binary: `Core/Assets/GcrSampler.s` / `gcrsampler.bin`.
- Safe stepping per *Inside Commodore DOS*: two VIA phase changes per track, zone bits set from target track, delta capped at 17 tracks, no end-stop homing, parameters in unused RAM (`$03F0`/`$03F1`) rather than zero page. Status/phase at `$0300`/`$0301` are verified.

### Notes
- Code builds cleanly but has NOT been run on hardware since the stepper fix; a supervised single-track test is required before trusting results.

## [Unreleased] — VICE OpenCBM Diagnosis, Source Patch & Fast-Loader Finding

### Added
- `patches/vice-3.10-opencbm-filename.patch`: VICE 3.10 real-device backend fix — buffer the filename bytes written after OPEN and pass them to `cbm_open()` at UNLISTEN instead of `NULL, 0`.
- `HARDWARE_WORKFLOW.md`: build instructions for the patched VICE, gdb/XUM1541 tracing procedure, and the uploaded fast-loader analysis.

### Notes
- gdb traces proved the stock VICE calls `cbm_open(8,0,"",0)`, then reads endlessly after `TALK`; direct libopencbm tests confirmed EOI works when the filename is passed correctly.
- The test disk used a custom fast loader (1,280 bytes of 6502 code uploaded to drive RAM `$0300–$07FF` with `M-W`, executed with `M-E`, bit-banging VIA `$1800`), which is outside the high-level OpenCBM backend; patch validation needs a plain `LOAD"NAME",8` disk.

## [Unreleased] — VICE Real-Drive Launch Builder & Hardware Verification

### Added
- `--cbm vice <command|start|stop>` builds, prints, launches or stops the verified VICE 3.10+ OpenCBM passthrough command line. Options: `--dev`, `--data` (drive 9 / B:), `--emu`, `--40col`, `--sound`, `--log`, `--softgl`, `--dry-run`. The PID of a launched VICE is recorded in `/tmp/d64editor-vice.pid` so `vice stop` kills only that process. `--cbm boot <image> <dev>` remains a legacy alias.

### Verified
- Built a bootable CP/M 3.0 D64 from the reference donor (`--cpm-copy --files` to free blocks, then `--cpm-bootable`) with `CCP.COM`, `CPM+.SYS`, `PIP.COM`, `DIR.COM`, `MBASIC.COM` and `WUMP.BAS`; booted to `A>` and `DIR` listed all files in x128.
- Wrote both D64 (1541 mode) and D71 (native 1571 mode, `-2`) images to a real 1571 via ZoomFloppy using warp (`-w`) and no `-t original`; readback byte-for-byte identical for both.
- Booted both images directly from the physical drive with `x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy`; CP/M Plus loaded from the real disk with no OpenCBM errors.
- README/AGENTS/Roadmap document the full toolchain, parameters, USB/IEC reset, and VICE start/stop procedure (including the software-GL workaround for flaky GTK3 startup on this Xwayland session).

## [Unreleased] — CP/M Disk Access Fully In-Process

### Changed
- All default CP/M disk operations now run in-process via `C128Cpm`: `--cpm-extract` and `--cpm-new` no longer auto-prefer `ctools`; `--cpm-copy`, `--cpm-format`, `--cpm-bootable`, `--cpm-import-dir` and the `cpm*` script commands were already in-process. No external tool is required for CP/M disk access.
- `ctools`/`cpmtools` are now explicit opt-ins (`--ctools`, `--cpm-cpmtools`) kept only for cross-checking; their code is unchanged.
- `C128Cpm.WriteFiles` now collects existing entries/used blocks only from the real CP/M directory area, avoiding false-positive "files" from embedded system data (e.g. `BNKBDOS3.SPR` inside `CPM.SYS`) that could suppress real imports.

### Docs
- README, AGENTS and Roadmap updated to describe the in-process default and the opt-in role of the external tools.

## [Unreleased] — CP/M D71 Mapping Fix & Strict Extraction (found via Nemesis/Zork import testing)

### Fixed
- `C128Cpm.TsOffset1541`: D71 tracks 36–70 now use the correct speed-zone sector counts when computing byte offsets (previously all treated as 17 sectors, corrupting files allocated past block ~88). Verified: 36-file Nemesis import now extracts byte-exact (was broken for high blocks).
- `C128Cpm.WriteFiles`: donor imports (`ScanAllForFreeSlots`) now identify directory sectors by the "all 8 slots are unused-or-plausible-entry" heuristic for BOTH free-slot finding and used-block collection, preventing file-data sectors containing 0xE5 bytes from being mistaken for directory slots.

### Added
- `C128Cpm.ExtractFilesStrict`: scans only known directory slots (real interleaved D64 sectors / block-based D71/D81 from `FormatBlank`), avoiding false-positive "files" from data sectors. CLI `--cpm-extract ... --strict` and script `cpmextract ... --strict`.

### Verification notes (emulator / x128)
- Authentic CP/M disk images from the local collection were used as boot donors. Nemesis/Zork game files import byte-exact and the resulting D71 image boots to CP/M 3.0 `A>`.
- The original 1981–82 CP/M 2.2 games themselves hang under the available CP/M 3.0 (BDOS 3.0/FastBIOS) donor with mismatched terminal/two-drive requirements — an environment compatibility issue, not file corruption. Documented as a limitation.

## [Unreleased] — Whole-Host-Directory Import to CP/M

### Added
- Core: `C128Cpm.HostNameToCpm83`, `ReadHostDirectory`, `ImportDirectory`, `ImportDirectoryFromBootDonor`, `CpmImportReport` — import all files in a Linux/host directory into a CP/M image with legal 8.3 name mapping (uppercase, 8+3 split, invalid chars → X, first char A-Z, truncation) and collision suffixes (FOOXBAR1.TXT etc.).
- CLI `--cpm-import-dir <donor|new:d64|new:d71|new:d81> <srcdir> <out>` and script `cpmimportdir`; print host→CP/M mapping and skips.
- Verified: 4-file directory import (including long names and collision) into new:d64/d71; 2-file import into a bootable donor; extracted files byte-exact (modulo CP/M 128-byte record padding); resulting donor image boots to CP/M 3.0 in x128.

## [Unreleased] — Emulator Boot Verification & Bootable CP/M Donor Path

### Verified (x128 emulator, VICE 3.10)
- Real reference C128 CP/M system disk boots to the CP/M 3.0 `A>` prompt.
- From-scratch `--cpm-new` images do NOT boot (black screen) — confirmed the documented caveat; they are data-only.
- New `--cpm-bootable <donor.d64> <out> ...` (Core `C128Cpm.FormatAndWriteFromBootDonor`) clones a known-good system disk (boot code/CCP/BDOS) and writes new files into its directory; verified to boot to CP/M 3.0 with the added file present. Script equivalent `cpmbootable`.

## [Unreleased] — CP/M D64 Directory Layout Aligned to Real Disk

### Changed
- `CpmImage.FormatBlank`/`WriteFiles`: D64 from-scratch CP/M directory now uses the real interleaved track-1 sectors (3,4,9,10,14,15,19,20, 8 entries/sector) observed on the reference C128 CP/M system disk, and the allocation bitmap was moved to sector 2 to avoid clashing with directory sector 4. D71/D81 keep their self-consistent block-based directory.
- Verified: from-scratch d64/d71/d81 images still round-trip through `--cpm-extract` with byte-exact data; real reference image still extracts 9 files. Bootability remains unverified against real media.

## [Unreleased] — Phase 2 Completion: GCR Write-back, Container Write-back, CP/M From-Scratch

### Added
- GCR write-back: `Gcr.EncodeTrack`, `G64Image.ToG64Bytes`/`FromD64`, `NibImage.ToNibBytes`/`FromD64`; CLI `--to-gcr` and script `togcr`. Verified byte-exact D64→G64/NIB→D64 round-trip (683/683 sectors).
- CMD container write-back for emulated D64/D71/D81 partitions (`TryLoadFromContainer` + `CbmCatalog.Open*FromContainer`); `PartitionsWindow` now opens all partition types directly with save-back into D2M/D4M/DHD containers. `DiskDocumentView.LoadImage` added for container-backed D64.
- C128 CP/M from-scratch formatting/writing: `C128Cpm.FormatBlank`/`WriteFiles`/`FormatAndWrite` with multi-extent 8.3 files and 1-/2-byte allocation pointers; CLI `--cpm-new` and script `cpmnew`. Extraction round-trip verified for d64/d71/d81.
- CLI `--extract-archive` now delegates to the shared Core `ArchiveExtract` helper.

### Changed
- Roadmark Phase 2 items marked complete; docs updated (README, AGENTS, CHANGELOG).

### Known limitations
- GCR: per-zone tuned interleave/gaps and copy-protected/half-track re-encoding not implemented (preserved verbatim).
- CP/M: from-scratch layout is spec-derived and self-consistent but bootability/DPB/skew/bitmap not verified against real bootable media.

## [Unreleased] — Headless Scripting / Automation Engine

### Added
- Core `ScriptEngine` with line-oriented script syntax and structured `ScriptResult` (log/error/exit code), plus CLI `--run-script`.
- Core `ArchiveExtract` helper (used by the `archive` script command and CLI `--extract-archive`).
- `D64Image.RenameDisk` for headless disk-header renaming.
- Sample scripts `scripts/example-basic.d64script` and `scripts/example-cpm.d64script`.

### Fixed
- `D64Image.ReadBam` now computes `BlocksFree` from the BAM (previously freshly-loaded disks reported 0 free, breaking imports).

### Changed
- Roadmap Phase 7 CLI/scripting item marked done; docs updated (README Scripting section, AGENTS, CHANGELOG).

## [Unreleased] — Detailed Copious Documentation Update

### Added
- **README.md** — comprehensive rewrite covering all features, formats, CLI, emulator/OpenCBM integration, CP/M, REL, drag-and-drop, file chain/directory display, undo/redo, verification, known limitations, and fidelity notes.
- **AGENTS.md** — expanded developer/agent guidance with architecture notes, key file roles, CLI commands, CP/M notes, emulator integration, graphics viewers, undo/redo implementation, known limitations, and local artifact policy.
- **CHANGELOG.md** — this file, tracking all notable changes.

### Changed
- Documentation now accurately reflects all implemented features across Phases 1–8.
- Roadmap status updated to reflect current implementation state.

---

## Recent Implementation Rounds

### Round 12 — CMD Container Write-Back (b41d43b)
#### Added
- `DnpImage.TryLoadFromContainer` — loads DNP partition from container slice with write-back on Save.
- `DnpImage.ContainerPath`/`ContainerOffset`/`ContainerSize` properties for container backing.
- `DnpImage.Save()` — writes back to container when container-backed.
- `CbmCatalog.OpenDnpFromContainer` — opens native DNP partition from container with write-back.
- `PartitionsWindow` — native DNP partitions now open directly as editable images saving back to D2M/D4M/DHD containers.

#### Changed
- Roadmap: native DNP container write-back noted as implemented; emulated-partition write-back still limited.

### Round 11 — GEOS VLIR & CLI Expansion (355725c)
#### Added
- `GeosInfoWindow` — follows VLIR record chains (best-effort) to report block/byte counts per record.
- CLI `--geos-info <d64> <file>` — non-interactive GEOS/REL metadata and VLIR summary.

### Round 10 — Knock Out Remaining Roadmap (45ec90f)
#### Added
- Basic snapshot-based **Undo/Redo** (`Ctrl+Z`/`Ctrl+Y`) for common D64 operations via Edit menu.
- `D64Image.GetRawDataCopy()` / `RestoreRawData(byte[])` for snapshot/restore.
- `DiskDocumentView._undo`/`_redo` stacks (capped at 50).
- Graphics viewers (picture, charset/sprite) — zoom controls and BMP save/export.
- `CreateRelWindow` — UI for creating new REL files.
- `File → New REL File…` menu item.
- CLI `--cpm-format <template> <out>` — template-based CP/M directory clear.
- `C128Cpm.FormatFromTemplate` — creates empty CP/M image from template.
- `DiskDocumentView.CreateRelFile()` — creates REL file with specified record length.
- Roadmap updates reflecting completed enhancements.

### Round 9 — Phase 4/6/7/8 Progress (b33a447 + c2d9b43)
#### Added
- `GeosInfoWindow` — metadata bytes 15-1D editing/saving + best-effort VLIR index/info-block viewing.
- CLI `--extract-archive <zip|tar|tar.gz|tgz> <dir> [--images-only]` — archive extraction.
- CLI `--print-dir <image>` — print directory listing (text, D64).
- `PrintDirectory()` — exports listing and opens in system default handler for OS printing.
- `OnCheckForUpdates` — opens releases page in default browser (best-effort).
- README and Roadmap updates for CLI and GEOS viewer/editor additions.

### Round 8 — Phase 2 Editing Progress (1e145f5)
#### Added
- `RelWrite` — REL file creation and resize/grow/shrink record count (side/data-sector allocation, relinking, LSU updates).
- `RelViewerWindow` — resize control and reload support.
- `T64Image` — mutable/payload-backed entries with `Create`/`AddFile`/`ExtractPayload`/`Save` for basic native read/write.
- `G64InfoWindow` — structural G64 track/slot viewer.
- G64/NIB structural-viewer fallback on decode failure or no decodable tracks.
- Roadmap updates for REL/T64/G64-NIB progress.

### Round 7 — File Chain & Directory Display (3f9f3f2, 4344ddd, 6c55db5, 63c85e3, b9672a8, 9fb7d8e)
#### Changed
- File chain viewer: larger font, zero-padded numbers, spacing between track/sector.
- Directory listing: removed dash between start/end addresses, 5-character width for addresses, zero-filled decimal addresses.

### Round 6 — Emulator & CP/M (80fce89, 061329a, cb1b6ae)
#### Fixed
- Emulator placeholder quoting for paths inside quoted template args (e.g. `"{appdir}/file.crt"`).
- CP/M directory viewer font: now uses CBM PETSCII font (C64 Pro Mono) with monospace fallbacks.

#### Added
- C128 CP/M extract/copy support: `C128Cpm` with DPB-aware extraction and template-based create/copy.
- CLI `--cpm-extract <image> <dir>` and `--cpm-copy <tmpl> <out> [--all|--files A,B]`.
- README/Roadmap documentation for CP/M support.

### Earlier Rounds (pre-2026-11-15)
- Initial VB6 → C#/.NET port of D64 Editor.
- Multi-format engine: D71, D81, D80, D82, DNP, D2M/D4M/DHD, G64, NIB, T64, CP/M 2.2, C128 CP/M.
- CBM-layout editing: scratch/unscratch/import for D71/D81/D80/D82/DNP.
- Viewers: Hex, BASIC, Disassembly, SEQ, REL, Picture, Charset, Sprite, GEOS info, File Chain, Cross-Links, Error Blocks.
- BAM/Sector/Block editors.
- Search/Compare/Batch tools.
- Emulator integration: template-based launch, OpenCBM transfer, file associations.
- PETSCII display: C64 Pro Mono font + VICE chargen.bin ROM-bitmap rendering, central charset toggle.
- Tabbed MDI shell, Save As, dirty tracking, multi-disk workspace.
- Drag-and-drop (OS files), container import (T64/P00).
- 40-track support, GEOS metadata decoding.
- Filename Builder, Memory window, Config window.

---

## Known Limitations (Current)

- **G64/NIB native GCR-level edit/save** back to `.g64/.nib` not implemented (requires GCR re-encoding).
- **CMD container-level write-back for emulated partitions** (D64/D71/D81) still limited.
- **C128 CP/M full from-scratch formatting/writing** for all variants and validated `.d71/.d81` DPB/boot-system layouts (template-based extract/copy/format exists; needs format-specific boot images/templates + real-image verification).
- **Broader inter-tab/window disk-to-disk drag-and-drop** (OS drag in/out works).
- **More comprehensive undo/redo** coverage across all edit paths (basic for common D64 ops).
- **Deeper GEOS disk/application-data/VLIR workflows** beyond best-effort index/chain inspection and metadata editing.
- **Richer non-interactive GEOS/REL/T64 scripting** in CLI.
- **Native OS print dialog** and **built-in updater/installer** (platform-dependent; best-effort print-open and update-page-open exist).

## Verification Status

- All format readers verified against synthetic images (D64, D71, D81, D80, D82, DNP, D2M/D4M/DHD, G64, NIB, T64, CP/M 2.2, C128 CP/M D64).
- REL create/resize verified against synthetic D64 images.
- CP/M extraction verified against `c128-cpm.system.6228129311-9283de.d64` (9 files extracted).
- Emulator/OpenCBM/real-drive flows remain unverified without external tools/hardware.
- CP/M `.d71/.d81` mappings need validation against real images/templates.
- No automated test suite; CI runs `dotnet test` for completeness (no test projects).