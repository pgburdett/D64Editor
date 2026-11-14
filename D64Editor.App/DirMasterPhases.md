# DirMaster Parity — Phased Implementation Plan

## Phase 1 — Multi-disk Workspace & Save As
- [x] Implement Save As... (format selection, path dialog)
- [x] Implement Close disk + unsaved-changes prompt (dirty tracking)
- [x] Dirty-state tracking (checksum-based; queried on Close/Exit)
- [x] Disk filename shown in title
- [x] Multi-disk: open windows tracked + Window menu lists/activates them; "New Disk Window" / "Open in New Window"
- [x] True single-window tabbed MDI host: DiskDocumentView (per-disk editor) hosted in a TabControl; shell menus/toolbar act on the active document; Window menu lists tabs
- [x] CLI: --convert (G64/NIB→D64 or copy), --list (print directory), --help, and opening an image path launches the GUI on it

## Phase 2 — Multi-Format Engine
- [x] `IDiskImage` abstraction; `D64Image` conforms (explicit Entries/CrossLinkWarnings)
- [x] Format detection (`DiskFormats.FromExtension` / D64 size checks)
- [x] Open dialog lists DirMaster extensions; non-D64 formats fail gracefully (no crash)
- [ ] D71 (1571 dual-sided) read/write
- [ ] D81 (1581 3.5") read/write
- [ ] D80/D82 (8050/8250) read/write
- [ ] T64 (tape archive) read/write
- [ ] G64/NIB (raw track) read/write
- [ ] DNP/DHD/D2M/D4M (CMD/native) read/write
- [x] T64 tape archive — read-only reader + viewer (verified with synthetic .t64)
- [x] D71 double-sided image — read-only reader + viewer (verified with synthetic .d71)
- [x] D81 image — read-only reader + viewer (BAM both sides, 40/3 root dir; verified with synthetic .d81)
- [x] G64 image — header + track-offset + speed-zone tables; structural parse (verified with synthetic .g64)
- [x] G64 → D64 converter (full GCR bitstream decode; verified byte-exact on synthetic .g64); opening a .g64 auto-converts & opens in the editor
- [x] NIB image — header + track table/density structural parse (verified with synthetic .nib)
- [x] NIB → D64 converter (reuses the verified GCR decoder; byte-exact on synthetic .nib)
- [x] DNP (CMD native partition) — reader + viewer (verified with synthetic .dnp)
- [x] D2M/D4M/DHD CMD containers — partition-table listing + per-partition open (verified with synthetic .d2m)
- [x] CP/M 2.2 images + .LBR catalogs — best-effort directory reader + viewer (verified with synthetic CP/M dir)
- [x] Registered formats: .cpm/.img/.dsk/.lbr/.d40/.d67 (+ existing), Open dialog lists them
- [ ] Full GCR→D64 sector decode for G64; G64→D64 conversion
- [x] Export-to-PRG for read-only images (D71/D81/D80/D82/T64) via their viewers
- [x] Scratch with block-freeing for D71/D81 (chain-walk → BAM bits + free-count; verified byte-exact on D71/D81)
- [x] Unscratch (re-allocate) for D71/D81/D80/D82/DNP (generalized via ICbmWritable; verified scratch→unscratch round-trip on D81)
- [x] File import for D81 (allocate free blocks → write chain → update BAM → add entry; verified byte-exact, incl. fixing a double-allocation bug the test caught)
- [ ] File import for D71/D80/D82/DNP
- [x] D80 (8050, 77 tracks) + D82 (8250, 154 tracks) images — reader + viewer (verified with synthetic .d80/.d82)
- [ ] NIB/DNP/DHD/D2M/D4M readers

## Phase 3 — Directory Editing Enhancements
- [x] Rename file (dialog)
- [x] Toggle case (upper/lower PETSCII)
- [x] Font size dialog (cycled sizes)
- [x] Refresh directory view
- [x] Show deleted files toggle
- [ ] Drag-and-drop between disk windows and OS
- [ ] Undo/Redo stack

## Phase 4 — Viewers
- [x] Hex viewer (read-only dump) — fixed-column `ADDR <16 bytes> <PETSCII>` rows, load-address base (lo,hi) when present, C64/ROM glyph char column, PETSCII/ASCII toggle
- [x] BASIC viewer (C64 BASIC V2 de-tokenizer) — ROM-bitmap rendered (quote mode)
- [x] Disassembly viewer (6502/6510)
- [x] SEQ file viewer (ported from the original) — ROM-bitmap PETSCII mode (control/quote handling) + ASCII text mode
- [x] REL file viewer (side-sector chain walk → fixed-length records; verified record segmentation)
- [x] Picture viewer: Koala Painter + Art Studio hi-res (C64 palette pixel decode; decoder verified on a synthetic Koala)
- [x] Charset viewer (256-glyph grid) + Sprite viewer (hires + multicolor; both verified on synthetic data)
- [x] ROM-bitmap PETSCII renderer: VICE `chargen.bin` + `PetsciiScreen`/`PetsciiView` (Pepto palette); directory rows and SEQ/BASIC viewers rendered via it
- [x] Centralized View → PETSCII Lower/Upper Case (upper/graphics ↔ lower/upper), persisted and wired across PETSCII-displaying views
- [ ] Graphics viewer (charset/sprite/bitmap) — enhance
- [ ] GEOS file viewer

## Phase 5 — BAM / Sector / Block Editors
- [ ] BAM editor (visual block allocation map)
- [ ] Sector/block editor (already partial — enhance with hex+ASCII)
- [ ] File chain viewer (already exists — enhance)
- [ ] Cross-links viewer (already exists — enhance)
- [ ] Error blocks viewer (already exists — enhance)

## Phase 6 — Search / Compare / Batch
- [x] Find in Disks (filename search across a folder of images, Datei>…)
- [x] Multi-format catalog (`ImageCatalog`): d64/d71/d81/d80/d82/dnp + g64/nib (decoded) → Find spans all supported formats; garbage/unsupported skipped
- [x] Compare Disks (diff header/directory/BAM of two images)
- [x] Batch processing (export all files from every image in a folder)
- [x] Batch Validate (report-only): per-image file count + blocks free
- [x] Find in Disks: name search + file-contents search (across supported formats)
- [x] Compare disks: header/directory/BAM diff + differing-block-number list
- [ ] Archive extraction (zip/tar of disk sets)

## Phase 7 — GEOS & Advanced
- [ ] GEOS disk support (VLIR, application data)
- [ ] GEOS file metadata editing
- [ ] Print disk directory
- [ ] Command-line options / scripting

## Phase 8 — Platform Integration
- [x] Emulator launch: Run > Run in Emulator (command template {disk}/{image}); Run > Configure Emulators; mounts drive 8, autostarts disk (*) or selected file (disk:NAME), $0801 BASIC → -autostartprgmode 1 (LOAD ,8,1); safe shell quoting
- [x] OpenCBM transfer: CBM > OpenCBM Transfer (detects cbmcopy/cbmctrl on PATH; {device}/{disk}/{out} template)
- [x] File associations: packaging/ (Linux .desktop + MIME install script; Windows .reg)
- [x] "Check for Updates" reads a version-feed URL (configurable)
- [x] Printable directory export (Disk > Print Directory…)
- [ ] Native OS print dialog / built-in updater installer (platform-dependent; intentionally skipped)

## Phase 5 — BAM / Sector / Block Editors
- [x] BAM editor: colored free/allocated cells + live FREE/ALLOCATED summary + legend

## Current Focus: Phase 1
Starting with Save As... and multi-disk workspace groundwork.
