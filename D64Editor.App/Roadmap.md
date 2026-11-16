# Feature Roadmap — Phased Implementation Plan

Roadmap for adding multi-format support and features comparable to other C64/CBM
disk editors, implemented incrementally.

## Phase 1 — Multi-disk Workspace & Save As
- [x] Save As… (format selection, path dialog)
- [x] Close disk + unsaved-changes prompt (dirty tracking)
- [x] Dirty-state tracking (checksum-based; queried on Close/Exit)
- [x] Disk filename shown in title
- [x] Multi-disk workspace and Window menu
- [x] True single-window tabbed MDI host (`DiskDocumentView` in `TabControl`; shell menus/toolbar act on active document)
- [x] CLI: `--convert` (G64/NIB→D64 or copy), `--list`, `--help`, and opening an image path launches the GUI

## Phase 2 — Multi-Format Engine
- [x] `IDiskImage` abstraction; `D64Image` conforms
- [x] Format detection (`DiskFormats`) and broad Open filter; unsupported formats fail gracefully
- [x] T64 tape archive — reader + viewer + mutable rebuild/save support (basic native read/write)
- [x] D71 (1571) — reader + viewer; directory/BAM-aware (synthetic-verified)
- [x] D81 (1581) — reader + viewer (synthetic-verified)
- [x] D80 (8050, 77 tracks) / D82 (8250, 154 tracks) — reader + viewer (synthetic-verified)
- [x] DNP (CMD native partition) — reader + viewer (synthetic-verified)
- [x] D2M/D4M/DHD CMD containers — partition-table listing + per-partition open (synthetic-verified)
- [x] G64 — structural parse + GCR bitstream decode → D64 conversion (byte-exact on synthetic .g64); opening auto-converts for editing, with structural-viewer fallback on decode failure
- [x] NIB — structural parse + decode → D64 conversion (byte-exact on synthetic .nib); opening auto-converts for editing, with structural-viewer fallback on decode failure
- [x] CP/M 2.2 images + .LBR catalogs — best-effort directory reader + viewer
- [x] C128 CP/M (.d64/.d71/.d81) — DPB-aware extraction and template-based create/copy (all/subset) via CLI (best-effort; `.d71/.d81` mappings need validation against real images)
- [x] Export-to-PRG for read-only CBM-layout views (D71/D81/D80/D82/T64)
- [x] Scratch with block-freeing (CBM layouts): D71/D81/D80/D82/DNP (chain-walk → BAM update, single flush)
- [x] Unscratch/re-allocate (CBM layouts): D71/D81/D80/D82/DNP via `ICbmWritable`/`CbmScratch`
- [x] File import (CBM layouts): D71/D81/D80/D82/DNP — allocate/write chain, directory slot, BAM update, single flush (`CbmImport`)
- [x] REL (D64): create, in-place record edit, and resize/grow/shrink record count (side/data-sector allocation and relinking)
- [x] G64/NIB structural viewers/fallback (G64 info window; NIB info window on decode fallback)
- [x] G64/NIB: native GCR-level write-back — `Gcr.EncodeTrack` + `G64Image.ToG64Bytes`/`FromD64` and `NibImage.ToNibBytes`/`FromD64`; CLI `--to-gcr` and script `togcr`. Byte-exact D64->G64/NIB->D64 round-trip verified (683/683 sectors). Half-track/copy-protected data preserved verbatim
- [x] T64/DNP/CMD: deeper native read/write — T64 mutable save/rebuild; DNP + emulated D64/D71/D81 partitions all open directly from D2M/D4M/DHD containers with Save() writing back into the container (`TryLoadFromContainer`, `CbmCatalog.Open*FromContainer`, PartitionsWindow opens directly). Verified container slice edit/save-back
- [x] C128 CP/M (.d64/.d71/.d81): from-scratch formatting/writing — `C128Cpm.FormatBlank`/`WriteFiles`/`FormatAndWrite` (multi-extent 8.3 files, 1-/2-byte AL) + CLI `--cpm-new` and script `cpmnew`. Extraction round-trip verified for d64/d71/d81. NOTE: bootability/DPB/skew not verified against real bootable CP/M media; template-based copy/format still available

## Phase 3 — Directory Editing Enhancements
- [x] Rename file (dialog)
- [x] Toggle case (upper/lower PETSCII)
- [x] Font size cycling
- [x] Refresh directory view
- [x] Show deleted files toggle
- [x] Drag-and-drop to/from OS files (import by dropping files onto a disk, drag selected files out to the OS); inter-tab/window disk-to-disk drag remains limited
- [x] Undo/Redo (basic snapshot-based undo/redo for common D64 editing operations via Edit menu/shortcuts)

## Phase 4 — Viewers
- [x] Hex viewer — `ADDR <16 bytes> <PETSCII>`, load-address base, C64/ROM glyphs, PETSCII/ASCII toggle
- [x] BASIC viewer (C64 BASIC V2 de-tokenizer), ROM-bitmap rendered
- [x] Disassembly viewer (6502/6510)
- [x] SEQ viewer — ROM-bitmap PETSCII mode + ASCII text mode
- [x] REL viewer (side-sector chain → fixed-length records; record-content edit in place)
- [x] Picture viewer (Koala/Art Studio hi-res)
- [x] Charset + Sprite viewers
- [x] GEOS file info viewer (metadata + info-block/VLIR index dump, best-effort) with directory metadata editing
- [x] ROM-bitmap PETSCII renderer (`chargen.bin`, `PetsciiScreen`/`PetsciiView`)
- [x] Centralized PETSCII Lower/Upper Case toggle, persisted across PETSCII-displaying views
- [x] Graphics viewer enhancements (zoom controls and BMP export/save for picture and charset/sprite viewers)

## Phase 5 — BAM / Sector / Block Editors
- [x] BAM editor (visual block allocation map): colored free/allocated cells, live FREE/ALLOCATED summary, legend, toggle/view-block/error modes
- [x] Sector/block viewer (enhanced): in-window hex+ASCII sector dump with track/sector/block navigation, link/owner info, and dump-to-file
- [x] File chain viewer (enhanced): per-block next T/S links and last-sector used-byte count
- [x] Cross-links viewer (enhanced): recompute, claimed-block summary, parsed block navigation/open in block viewer
- [x] Error blocks list viewer (enhanced): list/edit/open blocks with non-default error codes (images with error info)

## Phase 6 — Search / Compare / Batch
- [x] Find in Disks — filename and file-contents search across supported formats (`ImageCatalog`)
- [x] Compare Disks — header/directory/BAM diff and differing-block list
- [x] Batch export and Batch Validate (report-only)
- [x] Archive extraction (zip/tar/tar.gz/tgz of disk sets) via CLI `--extract-archive` (optional `--images-only`)

## Phase 7 — GEOS & Advanced
- [x] GEOS file info/metadata editing (directory bytes 15-1D) and best-effort VLIR index/info-block viewing
- [ ] GEOS disk/application-data support (deeper VLIR record-chain handling, GEOS-specific workflows)
- [x] CLI: headless `--list`, `--info`, `--validate`, `--convert` (G64/NIB→D64), `--export <image> <dir>`, CP/M extract/copy/format (template-based), archive extraction, `--print-dir`, `--help`
- [x] Expanded CLI/scripting — headless `ScriptEngine` (Core) + CLI `--run-script` automates open/convert/save/import/export/scratch/unscratch/wipe/rename/move, REL create/resize/write, CP/M extract/copy/format, GEOS info and archive extraction; sample scripts in `scripts/`

## Phase 8 — Platform Integration
- [x] Emulator launch (template `{disk}`/`{image}`, drive-8 mount, selection/`*` or `disk:NAME`, `$0801` → `-autostartprgmode 1`, safe quoting)
- [x] OpenCBM transfer (detects tools on PATH; `{device}/{disk}/{out}` template)
- [x] File associations (Linux .desktop + MIME, Windows .reg)
- [x] "Check for Updates" reads a version-feed URL and opens the releases page in the default browser (best-effort)
- [x] Printable directory export (text) with best-effort open-in-default-handler for OS printing
- [ ] Native OS print dialog and built-in updater/installer (platform-dependent; not implemented cross-platform)

## Current Focus
- Deeper GCR fidelity: per-zone tuned interleave/gaps, copy-protected track re-encoding, half-track editing
- CMD/DNP richer editing workflows inside containers; validation against real container images
- C128 CP/M validation of from-scratch layouts against real bootable media / specific DPB variants (boot code, skew, bitmap location)
- Deeper GEOS disk/application-data/VLIR workflows and richer metadata tooling
- Broader inter-tab/window disk-to-disk drag-and-drop and more comprehensive undo/redo coverage
- Platform-dependent native print dialog / updater installer remain out of scope for cross-platform build; keep best-effort flows working
- Roadmap/docs kept in sync with implemented behavior; smoke-test tabbed MDI/REL/GEOS flows
