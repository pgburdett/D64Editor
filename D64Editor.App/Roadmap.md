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
- [x] T64 tape archive — reader + viewer (synthetic-verified)
- [x] D71 (1571) — reader + viewer; directory/BAM-aware (synthetic-verified)
- [x] D81 (1581) — reader + viewer (synthetic-verified)
- [x] D80 (8050, 77 tracks) / D82 (8250, 154 tracks) — reader + viewer (synthetic-verified)
- [x] DNP (CMD native partition) — reader + viewer (synthetic-verified)
- [x] D2M/D4M/DHD CMD containers — partition-table listing + per-partition open (synthetic-verified)
- [x] G64 — structural parse + GCR bitstream decode → D64 conversion (byte-exact on synthetic .g64); opening auto-converts for editing
- [x] NIB — structural parse + decode → D64 conversion (byte-exact on synthetic .nib); opening auto-converts for editing
- [x] CP/M 2.2 images + .LBR catalogs — best-effort directory reader + viewer
- [x] Export-to-PRG for read-only CBM-layout views (D71/D81/D80/D82/T64)
- [x] Scratch with block-freeing (CBM layouts): D71/D81/D80/D82/DNP (chain-walk → BAM update, single flush)
- [x] Unscratch/re-allocate (CBM layouts): D71/D81/D80/D82/DNP via `ICbmWritable`/`CbmScratch`
- [x] File import (CBM layouts): D71/D81/D80/D82/DNP — allocate/write chain, directory slot, BAM update, single flush (`CbmImport`)
- [ ] G64/NIB: editing beyond auto-conversion to D64; NIB structural-viewer fallback
- [ ] T64/G64/NIB/DNP/CMD: full native read/write (beyond current read/view/convert)
- [ ] REL: create/grow/resize (record-content edit in place is done; needs side/data-sector allocation and side-sector rebuild)

## Phase 3 — Directory Editing Enhancements
- [x] Rename file (dialog)
- [x] Toggle case (upper/lower PETSCII)
- [x] Font size cycling
- [x] Refresh directory view
- [x] Show deleted files toggle
- [ ] Drag-and-drop between disk windows and OS
- [ ] Undo/Redo

## Phase 4 — Viewers
- [x] Hex viewer — `ADDR <16 bytes> <PETSCII>`, load-address base, C64/ROM glyphs, PETSCII/ASCII toggle
- [x] BASIC viewer (C64 BASIC V2 de-tokenizer), ROM-bitmap rendered
- [x] Disassembly viewer (6502/6510)
- [x] SEQ viewer — ROM-bitmap PETSCII mode + ASCII text mode
- [x] REL viewer (side-sector chain → fixed-length records; record-content edit in place)
- [x] Picture viewer (Koala/Art Studio hi-res)
- [x] Charset + Sprite viewers
- [x] GEOS file info viewer (metadata + info-block dump, best-effort)
- [x] ROM-bitmap PETSCII renderer (`chargen.bin`, `PetsciiScreen`/`PetsciiView`)
- [x] Centralized PETSCII Lower/Upper Case toggle, persisted across PETSCII-displaying views
- [ ] Graphics viewer enhancements (further)

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
- [ ] Archive extraction (zip/tar of disk sets)

## Phase 7 — GEOS & Advanced
- [ ] GEOS disk support (VLIR, application data)
- [ ] GEOS file metadata editing
- [x] CLI: headless `--list`, `--info`, `--validate`, `--convert` (G64/NIB→D64), `--export <image> <dir>` (D64/T64/D71/D81/D80/D82/DNP), `--help`
- [ ] Expanded CLI/scripting (further automation/options)

## Phase 8 — Platform Integration
- [x] Emulator launch (template `{disk}`/`{image}`, drive-8 mount, selection/`*` or `disk:NAME`, `$0801` → `-autostartprgmode 1`, safe quoting)
- [x] OpenCBM transfer (detects tools on PATH; `{device}/{disk}/{out}` template)
- [x] File associations (Linux .desktop + MIME, Windows .reg)
- [x] "Check for Updates" reads a version-feed URL (configurable)
- [x] Printable directory export (text)
- [ ] Native OS print dialog / built-in updater installer (platform-dependent; intentionally skipped)

## Current Focus
- REL create/grow/resize (side/data-sector allocation, side-sector rebuild)
- G64/NIB native editing and structural viewing
- Drag-and-drop between disk windows/tabs (broader) and Undo/Redo
- Further viewer/editor/CLI polish (graphics enhancements, deeper GEOS/VLIR support, more scripting)
- Roadmap/docs kept in sync with implemented behavior; smoke-test tabbed MDI flows
