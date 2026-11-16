# Changelog

All notable changes to D64 Editor (C#/.NET port) are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/).

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