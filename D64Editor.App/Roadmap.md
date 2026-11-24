# Feature roadmap and phase status

This file tracks the major feature phases completed since the VB6 port began. Checkboxes are
historical; see `AGENTS.md` and `README.md` for current command names and behavior.

## Phase 1 — Multi-disk workspace and Save As

- [x] Save As with path/format selection.
- [x] Close with dirty prompt and unsaved-change tracking.
- [x] Dirty detection via image checksum comparison.
- [x] Disk name shown in the window/tab title.
- [x] Multi-disk workspace: Window menu lists documents and tabs.
- [x] Tabbed MDI shell: `DiskDocumentView` inside a `TabControl`.
- [x] CLI `--convert`, `--list`, `--help`; image argument opens in GUI.

## Phase 2 — Multi-format engine

- [x] `IDiskImage` abstraction and `DiskFormats` detection.
- [x] T64 reader/viewer with mutable rebuild/save.
- [x] D71, D81, D80, D82, DNP readers; D2M/D4M/DHD CMD containers.
- [x] G64/NIB structural parsing, bit-level GCR decode to D64, and fallback viewers on decode
      failure.
- [x] CP/M 2.2 directories and `.LBR` catalogs.
- [x] C128 CP/M extraction and template-based create/copy/format, fully in-process by default.
- [x] Export-PRG for read-only CBM-layout views.
- [x] Scratch with block-freeing, unscratch, and import for CBM layouts.
- [x] REL create, in-place record editing, and resize.
- [x] G64/NIB structural viewers.
- [x] GCR write-back: D64 → G64/NIB byte-exact (683/683 sectors); half-track and protected data
      preserved verbatim when present in a container. Fidelity tuning remains incremental.
- [x] T64 mutable save and native DNP container write-back; CMD partition write-back works for all
      supported partition types.
- [x] CP/M from-scratch data images (`--cpm-new`), byte-exact extraction, and donor-based
      bootable images (`--cpm-bootable`). From-scratch images are **not** bootable without a donor.
- [x] CP/M host-directory import with 8.3 name mapping and collision suffixes.
- [x] CP/M directory layout aligned to real interleaved track-1 sectors; external ctools/cpmtools
      are explicit opt-in cross-checks only.

## Phase 3 — Directory editing enhancements

- [x] Rename and renamedisk.
- [x] Name-case toggle, font-size cycling, refresh, Show DEL.
- [x] OS drag-and-drop: `.d64` opens; other supported files import into the active disk.
- [x] Basic undo/redo snapshot stacks.

## Phase 4 — Viewers

- [x] Hex, BASIC, disassembly, SEQ, and REL viewers.
- [x] Picture viewer (Koala/Art Studio hi-res) and charset/sprite viewers with zoom and BMP export.
- [x] GEOS file info with metadata and best-effort VLIR inspection.
- [x] ROM-bitmap PETSCII renderer and central charset toggle.
- [x] G64/NIB info and Extended Image/Partitions viewers.

## Phase 5 — BAM, sector, and block editors

- [x] BAM editor with tracks as columns and sectors as rows; set/error-code modes.
- [x] Sector/block viewer with hex + PETSCII, navigation, and dump-to-file.
- [x] File-chain viewer with ordered track/sector/block list.
- [x] Cross-links viewer with recompute and navigation.
- [x] Error-blocks list.

## Phase 6 — Search, compare, batch, and archives

- [x] Find in Disks (name and file-contents search).
- [x] Compare Disks (header/directory/BAM diff and differing blocks).
- [x] Batch export and batch validate (report-only).
- [x] Archive extraction: zip/tar/tar.gz/tgz plus LHA/LZH, ARJ, and C64 LHA SFX PRGs.
- [x] LHA/LZH/SFX and ARJ list/create/extract verbs in CLI and scripts.

## Phase 7 — GEOS and advanced

- [x] GEOS directory metadata editing and VLIR index viewing.
- [x] Headless CLI: `--list`, `--info`, `--validate`, `--convert`, `--export`, CP/M operations,
      archive extraction, `--print-dir`, `--help`.
- [x] Expanded `ScriptEngine` with image lifecycle, directory/file operations, REL, CP/M,
      archives, GEOS info, and GCR export; sample scripts in `scripts/`.
- [ ] Deeper GEOS disk/application-data and full VLIR record-chain workflows remain best-effort.

## Phase 8 — Platform integration

- [x] Emulator launch with templates (`{disk}`, `{image}`, `{appdir}`, `{app}`, `{cwd}`), safe
      quoting, drive-8 mount, and autostart rules.
- [x] Structured OpenCBM transfer dialog (`OpenCbmWindow`) using `d64copy` with `ArgumentList`.
- [x] `--cbm vice <command|start|stop>` builder and verified real-1571 CP/M boot.
- [x] Linux/Windows file associations.
- [x] Check-for-updates opens the configured release feed.
- [x] Printable directory export as text.
- [ ] Native OS print dialog and built-in updater/installer are platform-dependent and not
      implemented.

## Current focus

- Incremental GCR fidelity: per-zone gaps/interleave, copy-protected track re-encoding, and
  half-track editing. Faithful images still require SRQ nibtools (`nibread`/`nibconv`).
- Richer CMD in-container editing workflows; write-back already works for all partition types.
- CP/M from-scratch validation against more DPB variants and real 1581 media.
- Deeper GEOS/VLIR support.
- Wider undo/redo coverage and inter-tab disk-to-disk drag-and-drop.
- Keep documentation in sync with code and smoke-test MDI, REL, GEOS, hardware, and scripting paths.
