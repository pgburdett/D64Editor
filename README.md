[![.NET](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml)

# D64 Editor — VB6 → C#/.NET 10 Core / Avalonia port

Original: D64 Editor by **_Forrest Mook_**, GPL v2, 1999-2016. https://www.d64editor.com

This is a C# port of the core disk engine and UI Components.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Build & Run](#build--run)
3. [Project Structure](#project-structure)
4. [What's Ported (D64Editor.Core)](#whats-ported-d64editorcore)
5. [PETSCII Display (Font & ROM Rendering)](#petscii-display-font--rom-rendering)
6. [Multi-Format Engine & Disk-Editor Features](#multi-format-engine--disk-editor-features-added)
7. [Disk Image Formats](#disk-image-formats)
8. [Disk-Editor Features](#disk-editor-features)
9. [Editing Features](#editing-features)
10. [Viewers](#viewers)
11. [BAM / Sector / Block Editors](#bam--sector--block-editors)
12. [Search / Compare / Batch](#search--compare--batch)
13. [GEOS & Advanced](#geos--advanced)
14. [Platform Integration](#platform-integration)
15. [CLI (Command-Line Interface)](#cli-command-line-interface)
16. [Emulator Integration](#emulator-integration)
17. [OpenCBM Integration](#opencbm-integration)
18. [File Associations](#file-associations)
19. [CP/M Support (C128 CP/M)](#cpm-support-c128-cpm)
20. [REL (Relative) File Support](#rel-relative-file-support)
21. [Drag-and-Drop & Container Import](#drag-and-drop--container-import)
22. [File Chain & Directory Display](#file-chain--directory-display)
23. [Undo/Redo](#undoredo)
24. [Packaging & OS Integration](#packaging--os-integration)
25. [Verification & Testing](#verification--testing)
26. [Known Limitations & Out-of-Scope Items](#known-limitations--out-of-scope-items)
27. [Fidelity vs. Cleanup](#fidelity-vs-cleanup)
28. [Contributing](#contributing)

---

## Project Overview

- **Language:** C# 12, targeting .NET 10.0 (`net10.0`), with nullable reference types enabled.
- **UI Framework:** Avalonia UI 11.3.4 (cross-platform desktop).
- **Solution:** `D64Editor.sln` containing two projects:
  - `D64Editor.Core/` — disk/image engine and format logic (class library).
  - `D64Editor.App/` — Avalonia desktop UI (executable, references Core).
- **Entry Point:** `D64Editor.App/Program.cs`.
- **Original Source:** VB6 source in `src/` (`Module1.bas`, `frmMain.frm`, etc.) used as behavior reference.
- **License:** GPL v2 (original by Forrest Mook, 1999-2016).

## Build & Run

### Prerequisites
- .NET SDK 10.0.x (or later). Install via your package manager:
  - **Arch Linux:** `sudo pacman -S dotnet-sdk`
  - **Ubuntu/Debian:** `sudo apt install dotnet-sdk-10.0`
  - **Windows:** Download from https://dotnet.microsoft.com/download

### Commands
```bash
# Restore dependencies
dotnet restore

# Build
dotnet build
# or after restore:
dotnet build --no-restore

# Run the UI
dotnet run --project D64Editor.App

# Run tests (currently no test projects; CI runs dotnet test for completeness)
dotnet test
dotnet test --no-build --verbosity normal
```

### CI
- GitHub Actions workflow (`.github/workflows/dotnet.yml`) runs on `ubuntu-latest` (push/PR to `master`):
  - `dotnet restore` → `dotnet build --no-restore` → `dotnet test --no-build --verbosity normal`
- No `global.json`; projects target `net10.0` with `LangVersion 12`.

### Local Artifacts
The workspace may contain local build artifacts (`bin/`, `obj/`, `.idea/`, `D64Editor.sln.DotSettings.user`, `d64editor-build-archives/`). These are not source-of-truth and should not be modified unless requested.

---

## Project Structure

```
D64Editor/
├── D64Editor.sln
├── D64Editor.Core/          # Disk/image engine (library, net10.0)
│   ├── D64Image.cs          # D64 geometry, BAM, directory, chain logic
│   ├── D71Image.cs          # 1571 disk support
│   ├── D80D82Image.cs       # 8050/8250 disk support
│   ├── D81Image.cs          # 1581 disk support
│   ├── DnpImage.cs          # CMD native partition support
│   ├── DiskContainers.cs    # D2M/D4M/DHD container parsing
│   ├── G64Image.cs          # GCR image support
│   ├── NibImage.cs          # NIB image support
│   ├── T64Image.cs          # T64 tape archive support
│   ├── CpmDisk.cs           # CP/M 2.2 directory reader
│   ├── C128Cpm.cs           # C128 CP/M (D64/D71/D81) support
│   ├── CbmImage.cs          # Shared CBM-layout helpers
│   ├── CbmCatalog.cs        # CBM image info abstraction
│   ├── CbmScratch.cs        # Scratch/unscratch for CBM layouts
│   ├── CbmImport.cs         # File import for CBM layouts
│   ├── RelFile.cs           # REL file read/write
│   ├── RelReader.cs         # REL file reader
│   ├── RelWrite.cs          # REL file create/resize
│   ├── Gcr.cs               # GCR encoding/decoding
│   ├── DiskFormats.cs       # Format detection
│   ├── ImageCatalog.cs      # Image catalog/search
│   ├── DirectoryFormatter.cs # PETSCII/ASCII conversion
│   └── ...
├── D64Editor.App/           # Avalonia desktop UI (Exe, net10.0)
│   ├── Program.cs           # Entry point
│   ├── MainWindow.axaml(.cs) # Shell: menu/toolbar/tabbed MDI
│   ├── DiskDocumentView.axaml(.cs) # Per-disk editor view
│   ├── Cli.cs               # CLI handler
│   ├── AppSettings.cs       # Persisted settings
│   ├── Fonts.cs             # PETSCII font wiring
│   ├── *.axaml(.cs)         # Various viewer/editor windows
│   ├── Roadmap.md           # Feature roadmap & status
│   └── Assets/              # Fonts, icons, chargen.bin
├── src/                     # Original VB6 source (reference)
├── packaging/               # OS integration helpers
├── AGENTS.md                # Agent/developer guidance
└── README.md                # This file
```

---

## What's Ported (D64Editor.Core)

### Core D64 Engine
- `D64Image` — load/save, disk name/ID, geometry (35-track standard, 40-track extended).
- BAM read/write (`ReadBam`/`WriteBam`) ← `READBAM` / `BAMWRITE`.
- Track/sector ↔ flat block conversion ← `convertTrackSectorToBlock` / `blockNum2TrackandSector` (cleaned up from VB's double-precision/string-parsing version to plain integer division — same math, verified against the original branch-by-branch).
- Directory read/write ← the directory-walk part of `LoadFile` and `writeOutDirectory` (+ `unAllocateDirSectors`).
- File type decoding ← `GETFILETYPE`.
- `D64BlockAllocator` — free-block search for writing new files ← `findFirstBlocktoUse` / `findNextBlocktoUse`.
- Block ownership + cross-link detection (`ComputeBlockOwners`) ← `getBlockOwners`.
- `ClearBlock` / `ReadBlock` / `WriteBlock` — raw block access for the Block Viewer.
- `ImportFile` / `ExportFile` ← `FileImport` / `FileExport` (plain PRG/SEQ/USR/REL/DEL payloads).
- `ImportContainerFile` + `ContainerFormats` — T64 and P00/S00/U00/R00 container import. **Not ported** from the original (that part of `FileImport` wasn't part of the reviewed source) — a from-scratch implementation against the published T64/P00 format specs. T64 files can hold several entries and all get imported; P00-family holds exactly one.
- `PetsciiConverter` ← the character-transform loop in `ViewSeqFile`.
- `ErrorInfo` (read/write the error-info section, 683 or 768 bytes depending on track count) + `D64ErrorCodes` ← `ErrorCodeBlocks` and frmErrorNumbers' error table.
- `PreferredStartTrack`/`PreferredStartSector` on `D64Image`, consulted by `ImportFile` ← `UserSpecifiedFirstTrack`/`UserSpecifiedFirstSector` from Config.
- `AddArtEntry` ← `CopySeparatorsToD64`.
- **40-track (non-standard extended) disk support** — `TrackCount`/`TotalBlocks` are now per-instance (35/683 or 40/768) instead of fixed constants, detected from file size on load. Tracks 36-40 have no BAM representation on any real or emulated 1541 (the format spec only ever reserved BAM entries for 1-35), so this port always reports those blocks as allocated — existing files there read/write fine, but the free-block allocator will never write new files into them. This matches the original's own "only partially supported" 40-track handling rather than inventing a fuller non-standard scheme with no spec to check it against.
- **GEOS directory-entry metadata** (`D64FileEntry.IsGeosFile`/`GeosFileTypeName`/`IsGeosVlir`/`GeosInfoBlock`/`GeosModified`, `D64Image.IsGeosDisk`). **Not ported** — bytes 0x15-0x1D were previously just preserved byte-for-byte undecoded, same as the original. This is a from-scratch decode against published GEOS format documentation, not verified against a real GEOS disk — worth testing against one if you have it, since GEOS format details have more variance across sources than the base 1541 format.

### Multi-Format Engine
Beyond the VB6 D64 port, this build carries a format-agnostic layer and a growing set of features comparable to other C64/CBM disk editors:

- **D71 / D81 / D80 / D82 / DNP** — directory viewer + Hex/BASIC viewers + Export-PRG. Directory-entry editing (rename, file type, start t/s, size) is supported for all of these. Scratch with block-freeing plus file import are implemented for D71/D81/D80/D82/DNP via `ICbmWritable`/`CbmScratch`/`CbmImport`.
- **G64 / NIB** — raw-GCR images; on open they are **decoded to a D64** (nibtools/VICE GCR pipeline: bit-level sync detection, 5-to-4 decode, `0x08` header + `0x07` data blocks) and then open fully editable in the editor. Verified byte-exact. Structural viewers (`G64InfoWindow`, `NibInfoWindow`) and fallback on decode failure are provided.
- **T64** — tape-archive reader + viewer + mutable rebuild/save support (basic native read/write). Verified on a synthetic `.t64`.
- **D2M / D4M / DHD** — CMD container partition tables; the Parts view lists partitions and opens the selected one (emulated 1541/1571/1581 land in the editor, native DNP opens in its viewer with container write-back).
- **CP/M 2.2 images + `.LBR`** — best-effort directory catalog (CP/M is DPB-parameterized, so it lists/labels entries and never assumes byte counts).
- **C128 CP/M (`.d64/.d71/.d81`)** — best-effort, DPB-aware file extraction and template-based image create/copy (all files or subset) via CLI, suitable for writing with OpenCBM. Relies on a compatible bootable template for the target format/layout.

Core readers: `D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `DiskContainers`, `C128Cpm`, plus shared helpers `CbmImage`/`Gcr`/`DiskFormats`/`CbmCatalog`/`ImageCatalog`/`CbmScratch`/`CbmImport`/`RelFile`/`RelReader`/`RelWrite`.

---

## PETSCII Display (Font & ROM Rendering)

The app ships with **C64 Pro Mono** (style64.org C64 TrueType, bundled in `D64Editor.App/Assets/Fonts/`, wired in `Fonts.cs`) and a bundled VICE C64 character ROM (`Assets/chargen.bin`) for exact ROM-bitmap rendering.

- **ROM-bitmap rendering** (`PetsciiScreen`/`PetsciiView`, Pepto palette): used for the directory (per-row, with selection preserved), SEQ (PETSCII mode), and BASIC listings, with correct C64 graphics/control/quote-mode behavior.
- **Font/PUA rendering:** hex/other PETSCII text uses the C64 font with screen-code→PUA glyph mapping (`Petscii.cs`) so the charset is accurate.
- **Central charset:** **View → PETSCII Lower/Upper Case** toggles Upper/Graphics (set 1) ↔ Lower/Upper (set 2), persisted and applied across PETSCII-displaying views.
- **CP/M directory viewer** uses the CBM PETSCII font (C64 Pro Mono) with monospace fallbacks.

To use a different PETSCII font, drop it in `Assets/Fonts/`, update `FileName` and `FamilyName` in `Fonts.cs` (the internal family name, e.g. `fc-scan <font> | grep family`), and ensure `PetsciiFontAvailable` is set. `chargen.bin` is the 4×1K screen-code-indexed ROM image (set1/set2, normal+reverse) used by the bitmap renderer.

---

## What's in the Demo Windows (D64Editor.App)

`MainWindow` ports `frmMain`'s disk-browsing and file-transfer slice: Open, Import File, Export Selected, disk name/ID display, directory listing, blocks-free/file-count summary. Saves after every import so changes aren't lost, same spirit as the original writing straight to the file on disk. True single-window tabbed MDI host: `DiskDocumentView` is the per-disk editor; `MainWindow` is a menu/toolbar + `TabControl` shell; Window menu lists open documents.

`BamEditorWindow` ports `frmBAM` ("BAM"): a click-to-toggle grid of all 683 blocks, laid out the same way the original does it — tracks as columns, sectors as rows, blue cells with a white "*" for allocated blocks, and the directory track (18) highlighted red. BLOCK EDIT MODE / BAM EDIT MODE and DECIMAL / HEX are real radio-button toggles now, matching the original's controls instead of being buried in a menu. Hover shows track/sector + owning file; menu has Allocate All / Unallocate All (protects 18/0 and 18/1) / Clean Disk / Save. The original hand-placed 683 Label controls in the VB6 designer (that's why `frmBAM.frm` was 16,000 lines) — here the grid is generated at runtime from `D64Image`'s own geometry instead.

`BlockViewerWindow` ports `frmBlockViewer` (captioned "Block Editor" in the original, despite the file name — kept that caption here too): a 16x16 hex grid plus a toggleable ASCII column, click a byte (or arrow-key/Tab through them) and type its new hex value, Wipe Block, Save Changes to Block, Previous/Next block navigation (◄ ►, matching the original's arrow buttons), C128 auto-boot sector detection. The ASCII column renders with the C64 PETSCII font/glyph mapping and follows the centralized **View → PETSCII Lower/Upper Case** charset toggle.

`SeqViewerWindow` ports `frmFileViewer` ("SEQ File Viewer"): shows a SEQ file's text content, with 40/80 Column and ASCII/PETSCII toggles. PETSCII mode is ROM-bitmap rendered (C64 character ROM / quote-control handling) and follows the centralized charset toggle; ASCII mode shows converted text. "Save to Text" exports the appropriate form. Printing isn't ported.

`SectorDumpWindow` ports `frmSectorDump`: pick a track/sector, dump that block's 256 raw bytes to a file. The original's Hex/Decimal display toggle for the track/sector pickers isn't ported — track and sector are always plain decimal (the underlying math was identical either way).

`MemoryWindow` ports `frmMemory` — a staging area where files are collected and then batch-copied into the currently open disk, exported back out, or dropped. "Copy Files to Memory..." in the main window copies the currently selected directory entries into memory (mirroring the original population path), and the memory window also has a direct "Add File..." picker. Selection uses Avalonia's native multi-select `ListBox` rather than the original's hand-rolled `Selected(i)` boolean array walked by `mnuInvertSelection`/`mnuSelectAllFiles`. When files are copied into the disk from the memory window, the main window's directory listing is refreshed.

`ConfigWindow` ports `frmConfig`: import interleave, starting track/sector, disable cross-link checking, safe disk clean, SEQ viewer's default character mode, the separator-line text setting, and the "treat track 18/0-1 start files as empty when copying to memory" option. Settings live in the static `AppSettings` class (one place instead of the scattered globals the original wrote into) and get applied to a `D64Image` immediately when set.

`FilePropertiesWindow` ports `frmFileProperties`: edit a directory entry's filename, starting track/sector, size, type, locked/splat flags directly. Much less code than the original needed, since `D64FileEntry` already models these as real fields instead of a hand-assembled binary string. "Filename Builder..." opens the Filename Builder and feeds the built name back into the filename field.

`ErrorNumbersWindow` + `CrossLinksWindow` port `frmErrorNumbers` and `frmCrossLinks`. Error Numbers opens from the BAM editor's new third mode ("SET ERROR CODE" — the original triggered this via a caption-check hack on the same BAM editor form; here it's just an honest third mode) and only applies to 175531-byte images with an error-info section. Cross-Links opens automatically after loading a disk if any cross-linked blocks are found, same as the original's automatic popup, unless disabled in Config.

`FilenameBuilderWindow` ports `frmFilenameBuilder` — the last of the fourteen original forms. It's not just a character-art tool: it's a reusable library of "separator" snippets that can be copied straight into a disk's directory as zero-block DEL entries, which is how art like "USE 38 TRACKS COPY" (or that Krestology disk) actually gets built — a handful of saved lines, recombined per disk. Two entry points, matching the original's dual purpose: standalone from the main Edit menu (build art, copy separators into the open disk), or from File Properties' "Filename Builder..." button (builds a name and feeds it back into that filename field instead). The PETSCII character grid shows decimal byte codes rather than glyphs until a real font's wired up via `Fonts.cs` — same limitation as the Block Editor's ASCII column, for the same reason. Separators persist to a file under the user's app-data folder now instead of the original's hard-coded Windows install directory.

Together these show the pattern (event handler → calls into `D64Image` → updates controls) that all fourteen forms follow.

---

## Multi-Format Engine & Disk-Editor Features (Added)

Beyond the VB6 D64 port, this build carries a format-agnostic layer and a growing set of features comparable to other C64/CBM disk editors. Current port scope / status is tracked in `D64Editor.App/Roadmap.md`.

### Disk Image Formats

- **D64** — full editor (load/save, BAM, scratch/unscratch/wipe, import, properties, viewers, multi-window).
- **D71 / D81 / D80 / D82 / DNP** — directory viewer + Hex/BASIC viewers + Export-PRG. Directory-entry editing (rename, file type, start t/s, size) is supported for all of these. Scratch with block-freeing plus file import are implemented for D71/D81/D80/D82/DNP via `ICbmWritable`/`CbmScratch`/`CbmImport`.
- **G64 / NIB** — raw-GCR images; on open they are **decoded to a D64** (nibtools/VICE GCR pipeline: bit-level sync detection, 5-to-4 decode, `0x08` header + `0x07` data blocks) and then open fully editable in the editor. Verified byte-exact. Structural viewers (`G64InfoWindow`, `NibInfoWindow`) and fallback on decode failure are provided. Native GCR-level re-encoding back to `.g64/.nib` is not yet implemented.
- **T64** — tape-archive reader + viewer + mutable rebuild/save support (basic native read/write). Verified on a synthetic `.t64`.
- **D2M / D4M / DHD** — CMD container partition tables; the Parts view lists partitions and opens the selected one (emulated 1541/1571/1581 land in the editor, native DNP opens in its viewer with container write-back).
- **CP/M 2.2 images + `.LBR`** — best-effort directory catalog (CP/M is DPB-parameterized, so it lists/labels entries and never assumes byte counts).
- **C128 CP/M (`.d64/.d71/.d81`)** — best-effort, DPB-aware file extraction and template-based image create/copy (all files or subset) via CLI, suitable for writing with OpenCBM. Relies on a compatible bootable template for the target format/layout.

Core readers: `D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `C128Cpm`, `DiskContainers`, plus shared helpers `CbmImage`/`Gcr`/`DiskFormats`/`CbmCatalog`/`ImageCatalog`/`CbmScratch`/`CbmImport`/`RelFile`/`RelReader`/`RelWrite`.

### Disk-Editor Features

- Menu (`Disk, File, Edit, View, Window, Help`) + toolbar with C64 color selectors (Blue / Lt Blue) that recolor the directory.
- C64-screen directory: ROM-bitmap rendered (VICE C64 character ROM via `PetsciiScreen`/`PetsciiView`, Pepto palette) with per-row selection preserved; PETSCII monospace font fallback also wired. Toggle name case, font-size cycling, Show DEL files, save-as, multi-disk/tabbed workspace, unsaved-changes prompt.
- Centralized **View → PETSCII Lower/Upper Case** toggle (Upper/Graphics set 1 vs Lower/Upper set 2), persisted and applied across PETSCII-displaying views (directory, hex, SEQ, BASIC, disassembly, block/sector ASCII, T64/extended/Find).
- Viewers: Hex, BASIC (C64 BASIC V2 de-tokenizer), Disassembly (6502), plus the picture decoder (Koala Painter/Art Studio hi-res), charset (256-glyph grid), sprite (hires + multicolor), and REL (fixed-length record) viewers.
  - **Hex viewer:** fixed-column dump per row — `AAAA <16 two-digit bytes> <PETSCII>` (4-hex memory address, space, 16 bytes, space, PETSCII chars), base from the file's 2-byte load address (low,high) when present, C64/ROM glyph rendering, PETSCII/ASCII char toggle, and it follows the global charset.
  - **SEQ/BASIC:** ROM-bitmap PETSCII rendering (SEQ respects quote/control mode; BASIC listing rendered in quote mode).
- Tools: Find in Disks (name **and** file-contents search), Compare Disks (header/directory/BAM diff + differing-block list), Batch export of a folder's files and Batch Validate (report-only), Archive extraction (zip/tar/tar.gz/tgz).
- Window management: the Window menu lists open documents/tabs (tabbed MDI shell).
- PETSCII: **C64 Pro Mono** (style64.org C64 TrueType, `Fonts.cs`) + PUA/screen-code mapping (`Petscii.cs`) and a bundled VICE `chargen.bin` for exact ROM-bitmap rendering. Central charset toggle (upper/graphics ↔ lower/upper).

---

## Drag-and-Drop & Container Import

Drop a `.d64` onto the main window to open it, or drop any other file (including `.t64`/`.p00`/`.s00`/`.u00`/`.r00`, and multiple files at once) to import it into whatever disk's currently open. This isn't a port of `DragDrop.bas` — that wrapped Windows' OLE drag-drop APIs, which don't map onto Avalonia's drag-drop model closely enough for a line-by-line port to mean much — but the resulting behavior matches. The same container detection also applies to the regular "Import File..." button and to "Add File..." in the Memory window. Inter-tab/window disk-to-disk drag remains limited.

---

## Editing Features

### D64 Editing
- Import File / Export Selected, disk name/ID display, directory listing, blocks-free/file-count summary.
- Save after every import so changes aren't lost, same spirit as the original writing straight to the file on disk.
- Scratch / Unscratch / Wipe / Remove Directory Entry / Move to Top/Bottom.
- File Properties editing (filename, start t/s, size, type, locked/splat flags).
- Filename Builder for separator art.
- REL file create, in-place record editing, and resize/grow/shrink record count (side/data-sector allocation and relinking).
- Undo/Redo (basic snapshot-based for common D64 operations via Edit menu / `Ctrl+Z` / `Ctrl+Y`).

### CBM-Layout Editing (D71/D81/D80/D82/DNP)
- Directory-entry editing (rename, file type, start t/s, size).
- Scratch with block-freeing (chain-walk → BAM update, single flush).
- Unscratch/re-allocate via `ICbmWritable`/`CbmScratch`.
- File import via `CbmImport` (allocate/write chain, directory slot, BAM update, single flush).
- Native DNP partition editing with write-back to CMD containers (D2M/D4M/DHD).

### CP/M Editing
- Template-based create/copy (all files or subset) for C128 CP/M disks.
- DPB-aware extraction and template-based formatting (directory clear).

---

## Viewers

- **Hex viewer:** fixed-column dump per row — `ADDR <16 two-digit bytes> <PETSCII>` (4-hex memory address, space, 16 bytes, space, PETSCII chars), base from the file's 2-byte load address (low,high) when present, C64/ROM glyph rendering, PETSCII/ASCII char toggle, follows global charset.
- **BASIC viewer:** C64 BASIC V2 de-tokenizer, ROM-bitmap rendered.
- **Disassembly viewer:** 6502/6510.
- **SEQ viewer:** ROM-bitmap PETSCII mode + ASCII text mode.
- **REL viewer:** side-sector chain → fixed-length records; record-content edit in place; create/resize support.
- **Picture viewer:** Koala/Art Studio hi-res decoder with zoom controls and BMP save/export.
- **Charset viewer:** 256-glyph grid with zoom and BMP export.
- **Sprite viewer:** hires + multicolor with zoom and BMP export.
- **GEOS file info viewer:** metadata + info-block dump (best-effort) with directory metadata editing and best-effort VLIR index/record-chain inspection.
- **File Chain viewer:** per-block next T/S links and last-sector used-byte count, with zero-padded alignment and adjustable font size.
- **Cross-Links viewer:** recompute, claimed-block summary, parsed block navigation/open in block viewer.
- **Error Blocks list viewer:** list/edit/open blocks with non-default error codes (images with error info).
- **T64 viewer:** tape archive file listing and extraction.
- **Extended Image viewer:** directory viewer for D71/D81/D80/D82/DNP with entry editing and import.
- **CP/M viewer:** directory catalog with CBM PETSCII font.
- **G64/NIB info viewers:** structural track/slot viewing with fallback on decode failure.
- **Partitions viewer:** CMD container partition listing with open/save-back support.

---

## BAM / Sector / Block Editors

- **BAM editor:** visual block allocation map with colored free/allocated cells, live FREE/ALLOCATED summary, legend, toggle/view-block/error modes.
- **Sector/block viewer:** in-window hex+ASCII sector dump with track/sector/block navigation, link/owner info, and dump-to-file.
- **File chain viewer:** per-block next T/S links and last-sector used-byte count.
- **Cross-links viewer:** recompute, claimed-block summary, parsed block navigation/open in block viewer.
- **Error blocks list viewer:** list/edit/open blocks with non-default error codes (images with error info).

---

## Search / Compare / Batch

- **Find in Disks:** filename and file-contents search across supported formats (`ImageCatalog`).
- **Compare Disks:** header/directory/BAM diff and differing-block list.
- **Batch export and Batch Validate:** report-only batch operations.
- **Archive extraction:** zip/tar/tar.gz/tgz via CLI `--extract-archive` (optional `--images-only`).

---

## GEOS & Advanced

- **GEOS file info/metadata editing:** directory bytes 15-1D edit/save and best-effort info-block/VLIR index viewing.
- **GEOS VLIR inspection:** best-effort record-chain following with block/byte counts per record.
- **GEOS disk/application-data support:** deeper VLIR record-chain handling remains a work-in-progress.

---

## Platform Integration

- **Emulator launch:** template `{disk}`/`{image}`, drive-8 mount, selection/`*` or `disk:NAME`, `$0801` → `-autostartprgmode 1`, safe quoting.
- **OpenCBM transfer:** detects tools on PATH; `{device}/{disk}/{out}` template.
- **File associations:** Linux `.desktop` + MIME, Windows `.reg`.
- **Check for Updates:** reads a version-feed URL and opens the releases page in the default browser (best-effort).
- **Printable directory export:** text with best-effort open-in-default-handler for OS printing.
- **Native OS print dialog:** not implemented cross-platform (best-effort print-open exists).
- **Built-in updater/installer:** not implemented (platform-dependent).

---

## CLI (Command-Line Interface)

The CLI is invoked by passing arguments to the application. Returns true when a CLI verb was handled (GUI not started); otherwise opens the image in the GUI.

### Usage

```
D64Editor --help
D64Editor --list <image>
D64Editor --info <image>
D64Editor --validate <image>
D64Editor --export <image> <dir>
D64Editor --convert <in> <out>
D64Editor --cpm-extract <image> <dir>
D64Editor --cpm-copy <tmpl> <out> [--all|--files A,B]
D64Editor --cpm-format <tmpl> <out>
D64Editor --extract-archive <zip|tar|tar.gz|tgz> <dir> [--images-only]
D64Editor --print-dir <image>
D64Editor --geos-info <d64> <file>
D64Editor --run-script <script.txt> [workdir]
D64Editor --to-gcr <d64> <out.g64|out.nib>
D64Editor --cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]
D64Editor <image>          (opens in GUI)
D64Editor                  (no args: starts GUI)
```

### Commands

| Command | Description |
|---------|-------------|
| `--help` / `-h` / `help` | Show help text |
| `--convert <in> <out>` / `-c` | Convert G64/NIB to D64 (or copy native image) |
| `--list <image>` / `-l` | Print an image's directory |
| `--export <image> <dir>` / `-e` | Export files from D64/T64/D71/D81/D80/D82/DNP to a directory |
| `--info <image>` / `-i` | Show image/format information |
| `--validate <image>` / `-v` | Validate/report cross-links and basic issues (D64) |
| `--cpm-extract <img> <dir>` | Extract CP/M files (best-effort DPB-aware, .d64/.d71/.d81) |
| `--cpm-copy <tmpl> <out> [--all\|--files A,B]` | Create CP/M image from template (keep all or subset) |
| `--cpm-format <tmpl> <out>` | Create empty CP/M image from template (clears directory; template-based) |
| `--extract-archive <zip\|tar\|tar.gz\|tgz> <dir> [--images-only]` | Extract archive contents |
| `--print-dir <image>` | Print directory listing (text, D64) |
| `--geos-info <d64> <file>` | Show GEOS/REL metadata and best-effort VLIR info for a file (D64) |
| `--run-script <script.txt> [workdir]` | Run a headless automation script over the Core engine (see Scripting section) |
| `--to-gcr <d64> <out.g64\|out.nib>` | Encode a D64 into a G64/NIB image (native GCR write-back; byte-exact round-trip) |
| `--cpm-new <d64\|d71\|d81> <out> [NAME.EXT=hostfile ...]` | From-scratch C128 CP/M image (best-effort spec-derived layout) |

---

## Emulator Integration

- **Run → Configure Emulators…** sets a command template; **Run in Emulator…** launches it.
- `{disk}` = autostart target, `{image}` = disk path for mounting, `{appdir}` = app folder, `{cwd}` = working directory.
- Default mounts drive 8 and autostarts (`x64sc -8 "{image}" -autostart "{disk}"` behavior; auto-injects `-8 <image>` when needed).
- No selection → autostart disk/`*` (first file); one selected file → autostart `disk:NAME` (normalized); **$0801 BASIC** adds `-autostartprgmode 1` (LOAD ,8,1 style) when absent.
- Paths are shell-quoted safely.
- Emulator filenames are sent in VICE-compatible (lowercase-letter) form to avoid shifted/graphics glyphs.
- Placeholders are substituted correctly whether they appear as whole quoted tokens (`"{appdir}"`) or inside existing double-quoted arguments (`"{appdir}/file.crt"`), preserving outer quotes.
- Example template with cartridge:
  ```
  x64sc -cartcrt "{cwd}/Epyx_FastLoad_1984_v2-b11b3b.crt" -8 "{image}" -autostart "{disk}"
  ```

---

## OpenCBM Integration

- **CBM → OpenCBM Transfer…** detects `cbmcopy`/`cbmctrl` on `PATH` and runs a command with `{device}/{disk}/{out}` substituted.
- Requires OpenCBM installed and a real drive to do anything.

---

## File Associations

- **Linux** — `packaging/install-associations.sh /path/to/D64Editor.App` installs a `.desktop` entry, MIME types (`.d64/.d71/.d81/.d80/.d82/.dnp/.g64/.t64/.lbr` → `application/x-*`), and the default-app associations under `~/.local` (no root). `install-associations.sh <exe> --uninstall` reverses it.
- **Windows** — import `d64-file-association.reg` (`regedit /s …`) after editing the install path to point at your extracted `D64Editor.App.exe`.

---

## CP/M Support (C128 CP/M)

### Overview
- C128 CP/M (CP/M 3.0 / CP/M Plus) on Commodore disk images uses 128-byte logical sectors, allocation blocks (1K for 1541, 2K for 1571/1581), and DPB-defined parameters.
- C128 CP/M uses 7-bit ASCII (not PETSCII) for names and text.
- CP/M-on-`.d64` cases are detected and opened in the CP/M viewer.

### DPB-Aware Extraction
- `C128Cpm.ExtractFiles` provides best-effort, format-appropriate file extraction:
  - `.d64` (1541): linear 35-track sector map, 1K blocks (4 sectors), 1-byte allocation pointers.
  - `.d71` (1571): linear 70-track map, 2K blocks (8 sectors), 1-byte AL.
  - `.d81` (1581): linear 80-track × 40-sector map, 2K blocks (8 sectors), 2-byte AL.
- CLI: `--cpm-extract <image> <outdir>`.

### Template-Based Create/Copy
- Uses a known-good, bootable C128 CP/M image of the same format as a template (preserves boot/system/BAM-header/layout).
- Keeps all files (`--all`) or a subset (`--files NAME.EXT,NAME2.EXT,...`).
- Unselected directory extents are marked unused (0xE5); data blocks become unreferenced.
- CLI: `--cpm-copy <template> <out> [--all|--files A,B]`.
- `--cpm-format <template> <out>`: creates an empty CP/M image from template (clears directory; template-based).

### From-Scratch Formatting (no template)
- `C128Cpm.FormatBlank(fmt)` / `WriteFiles(...)` / `FormatAndWrite(...)` create a fresh
  C128 CP/M `.d64/.d71/.d81` with boot signature, directory (all slots `0xE5`) and
  a simple allocation bitmap, then write 8.3 files (multi-extent aware, 1-byte AL for
  D64/D71, 2-byte LE for D81). CLI: `--cpm-new <fmt> <out> [NAME.EXT=hostfile ...]`;
  script: `cpmnew`. Extraction round-trips verified for all three formats.
  The D64 directory is placed in the **real interleaved track-1 sectors** (3,4,9,10,14,15,19,20)
  observed on the reference C128 CP/M system disk (8 entries per sector); the allocation
  vector sits in sector 2 to avoid clashing with directory sector 4.
- **Caveat:** this is a published-spec-derived, best-effort layout — bootability, DPB
  parameters, sector skew and bitmap location are **not verified** against real bootable
  CP/M media (D71/D81 use a self-consistent block-based directory, no real reference available). Template-based copy/format remains available for known-good images.
  CP/M stores record counts, so non-128-aligned files extract padded to 128-byte records.

### Writing to Real Media
- Use OpenCBM for writing to real floppy drives:
  ```bash
  d64copy -2 c128-cpm-new.d64 8
  ```

### Limitations
- Template-based, not a from-scratch formatter. Relies on a compatible template for boot/system/BAM-header, directory location, allocation/skew, AL width, etc.
- `.d71`/`.d81` mappings and DPB params are best-effort and need validation against real C128 CP/M 1571/1581 images/templates.
- Full from-scratch formatting/writing for all variants remains incomplete.

---

## REL (Relative) File Support

- **REL viewer:** side-sector chain → fixed-length records; record-content edit in place.
- **REL create:** `File → New REL File…` creates a new REL file with specified record length (side/data-sector allocation and linking).
- **REL resize:** grow/shrink record count via the REL viewer's Resize button (side/data-sector allocation and relinking, LSU updates).
- **REL file format:** side-sector chain indexes data sectors; each data sector holds 254 bytes of record data (last sector may pad with LSU byte count).
- Core: `RelFile`, `RelReader`, `RelWrite`.

---

## Drag-and-Drop & Container Import

- Drop a `.d64` onto the main window to open it, or drop any other file (including `.t64`/`.p00`/`.s00`/`.u00`/`.r00`, and multiple files at once) to import it into whatever disk's currently open.
- The same container detection applies to the regular "Import File..." button and to "Add File..." in the Memory window.
- Inter-tab/window disk-to-disk drag remains limited.

---

## File Chain & Directory Display

### File Chain Viewer
- Shows a file's block chain as an ordered track/sector/block list.
- Per-block next T/S links and last-sector used-byte count.
- Zero-padded alignment for block/track/sector/link numbers.
- Adjustable font size.
- Format: `N.  block #DDD  TTT  S SS  -> next TT,SS` or `end (bytes used: N)`.

### Directory Display
- C64-screen directory: ROM-bitmap rendered with per-row selection.
- Start/end memory addresses displayed (5-digit, zero-filled in decimal; `$XXXX` in hex).
- Space-separated start/end addresses (no dash).
- Toggle name case, font-size cycling, Show DEL files.
- Multi-select for batch operations.

---

## Undo/Redo

- Basic snapshot-based undo/redo for common D64 editing operations.
- Available via **Edit → Undo** (`Ctrl+Z`) and **Edit → Redo** (`Ctrl+Y`).
- Snapshots are taken before key mutations: import, scratch/unscratch, wipe, remove directory entry, move to top/bottom, REL create, etc.
- Stack depth capped at 50 snapshots.
- More comprehensive coverage across all edit paths is still in progress.

---

## Scripting (Headless Automation)

The Core exposes a line-oriented scripting engine (`D64Editor.Core/ScriptEngine.cs`) that
automates the major disk-engine components without any UI. Run scripts from the CLI:

```bash
D64Editor --run-script scripts/example-basic.d64script
D64Editor --run-script path/to/script.txt /path/to/workdir
```

Script syntax: one command per line; `#` and `;` start comments; arguments are
whitespace-separated; use double or single quotes for names containing spaces. Relative
paths resolve against the script's folder (or the optional `workdir` argument). Scripts
log every command and its result, stop on the first error, and propagate an exit code
(`exit [code]`; CLI sets `Environment.ExitCode` accordingly).

### Commands

| Command | Description |
|---------|-------------|
| `help` | Print the in-script command list |
| `echo <text...>` | Print text |
| `open <image>` | Open `.d64`/`.d71`/`.d81`/`.d80`/`.d82`/`.dnp`/`.t64` (auto-detected by extension) |
| `close` | Close the current image |
| `save [path]` | Save the current image (optionally to a new path) |
| `new <path> [name] [id]` | Create a blank 35-track D64 and open it |
| `convert <in> <out>` | G64/NIB → D64 decode, or passthrough copy |
| `list` | Print the directory |
| `info` | Print disk info + blocks free |
| `validate` | Report cross-links / block issues (D64) |
| `rename <old> <new>` | Rename a directory entry (D64/CBM layouts) |
| `renamedisk <name> <id>` | Rename the disk header (D64) |
| `import <host> [name] [type]` | Import PRG/SEQ/USR/REL/DEL or T64/P00 container (type: prg/seq/usr/rel/del) |
| `export <name> [outpath]` | Export a file's raw bytes |
| `exportdir <outdir>` | Export all non-deleted files |
| `scratch <name>` | Soft-delete (frees blocks, keeps entry) |
| `unscratch <name> [type]` | Restore a scratched entry |
| `wipe <name>` | Hard-delete (D64 only; unrecoverable) |
| `move <name> top\|bottom` | Reorder a directory entry (D64) |
| `relnew <name> <reclen>` | Create a REL file (D64) |
| `relresize <name> <count>` | Grow/shrink a REL file's record count (D64) |
| `relwrite <name> <rec#> <hex...>` | Rewrite a REL record from hex bytes (D64) |
| `cpmextract <img> <dir>` | Extract CP/M files (best-effort DPB-aware) |
| `cpmcopy <tmpl> <out> [--all\|--files A,B]` | Create CP/M image from a template |
| `cpmformat <tmpl> <out>` | Create an empty CP/M image from a template |
| `archive <zip\|tar\|tgz> <dir> [--images-only]` | Extract archive contents |
| `geosinfo <name>` | Show GEOS/REL metadata for a file |
| `togcr <d64> <out.g64\|nib>` | Encode a D64 into a G64/NIB container (GCR write-back) |
| `cpmnew <d64\|d71\|d81> <out> [NAME.EXT=host ...]` | From-scratch CP/M image (best-effort) |
| `exit [code]` | Stop with an exit code |

Example (`scripts/example-basic.d64script`):

```
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

A CP/M-oriented sample is in `scripts/example-cpm.d64script`. Supporting Core helpers:
`ArchiveExtract` (zip/tar/tgz extraction), `D64Image.RenameDisk` and
`D64Image.GetRawDataCopy()/RestoreRawData()` (snapshot support used elsewhere).

## Verification & Testing

- All new format readers were verified by generating synthetic images of each format (with a matching GCR encoder for G64/NIB) and byte-comparing the parsed results.
- Synthetic verification covers: D64, D71, D81, D80, D82, DNP, D2M/D4M/DHD, G64, NIB, T64, CP/M 2.2, C128 CP/M (D64).
- REL create/resize verified against synthetic D64 images.
- CP/M extraction verified against `c128-cpm.system.6228129311-9283de.d64` (9 files extracted).
- Many behaviors (emulator/OpenCBM/real-drive flows, CP/M `.d71/.d81` mappings) remain unverified without external tools/hardware or real images.
- No automated test suite (no `*Test*`/`*.Tests*` projects); CI runs `dotnet test` for completeness.

---

## Known Limitations & Out-of-Scope Items

### Not Yet Implemented
- **G64/NIB native GCR-level editing/save** back to original `.g64/.nib` format (requires GCR re-encoding with correct sync/timing/track rebuilding).
- **T64/DNP/CMD deeper native read/write:** T64 has basic mutable save/rebuild; DNP supports in-image edits/save and native DNP container write-back. CMD container-level write-back for emulated partitions (D64/D71/D81) still limited.
- **C128 CP/M layout validation** — from-scratch formatting/writing exists for all variants; validating bootability/DPB/skew/bitmap against real bootable CP/M media remains external (needs real images/emulator).
- **Broader inter-tab/window disk-to-disk drag-and-drop** (OS drag in/out works; inter-tab drag remains limited).
- **More comprehensive undo/redo** coverage across all edit paths (basic for common D64 ops now).
- **Deeper GEOS disk/application-data/VLIR workflows** beyond best-effort index/chain inspection and metadata editing.
- **Richer non-interactive scripting** — a headless `ScriptEngine` + `--run-script` now automates the major Core components (see Scripting section); further domain-specific command coverage remains incremental.
- **Native OS print dialog** (platform-dependent; best-effort print-open exists).
- **Built-in updater/installer** (platform-dependent; best-effort update-page-open exists).

### Platform/Hardware-Dependent
- Emulator launch requires an emulator (VICE/x64sc, x128, etc.) installed.
- OpenCBM transfer requires OpenCBM installed and a real drive.
- Real CP/M disk verification requires real C128 CP/M images/templates and hardware.
- Graphics viewer enhancements (zoom/pan, richer format/export controls) beyond current zoom + BMP save.

---

## Fidelity vs. Cleanup

Where the VB6 code did something because VB6 forced it to (module-level globals instead of instance state, double-precision math to avoid writing integer division, five different `If Then` blocks repeating the same track-range check), I cleaned it up. Where the VB6 code encoded actual 1541 disk-format knowledge (byte offsets, bit ordering, the directory-chain algorithm, the free-block search's spiral pattern), I kept it faithful and commented where each piece came from, so you can diff against the original if something's ever in question.

---

## Contributing

- Keep format/disk behavior changes in `D64Editor.Core` and UI changes in `D64Editor.App` unless there is a clear reason.
- Use `src/` (VB6 source) as reference for 1541-format behavior and port fidelity questions.
- Prefer code/config as executable truth over README/CI prose when they disagree.
- Avoid modifying local artifacts (`bin/`, `obj/`, `.idea/`, `*.user`, `d64editor-build-archives/`, untracked `*.d64`/`*.crt`).
- PETSCII font wiring centered around `D64Editor.App/Fonts.cs` and `D64Editor.App/Assets/Fonts/`.
- Multi-format layer in Core with shared helpers; each reader verified against synthetic images.
- Feature/phase roadmap tracked in `D64Editor.App/Roadmap.md`.
- Commit with detailed messages (including docs) and push when requested.
- Keep untracked local/test artifacts out of commits unless intended.