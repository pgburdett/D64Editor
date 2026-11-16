

[![.NET](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml)

# D64 Editor — VB6 → C#/.NET 10 Core / Avalonia port

Original: D64 Editor by  **_Forrest Mook_**, GPL v2, 1999-2016. https://www.d64editor.com

This is a C# port of the core disk engine and UI Components.

## What's ported (D64Editor.Core)

- `D64Image` — load/save, disk name/ID, geometry
- BAM read/write (`ReadBam`/`WriteBam`) ← `READBAM` / `BAMWRITE`
- Track/sector ↔ flat block conversion ← `convertTrackSectorToBlock` /
  `blockNum2TrackandSector` (cleaned up from VB's double-precision/string-parsing
  version to plain integer division — same math, verified against the original
  branch-by-branch)
- Directory read/write ← the directory-walk part of `LoadFile` and
  `writeOutDirectory` (+ `unAllocateDirSectors`)
- File type decoding ← `GETFILETYPE`
- `D64BlockAllocator` — free-block search for writing new files ←
  `findFirstBlocktoUse` / `findNextBlocktoUse`
- Block ownership + cross-link detection (`ComputeBlockOwners`) ← `getBlockOwners`
- `ClearBlock` / `ReadBlock` / `WriteBlock` — raw block access for the Block Viewer
- `ImportFile` / `ExportFile` ← `FileImport` / `FileExport` (plain PRG/SEQ/USR/REL/DEL
  payloads)
- `ImportContainerFile` + `ContainerFormats` — T64 and P00/S00/U00/R00 container
  import. **Not ported** from the original (that part of `FileImport` wasn't part
  of the reviewed source) — a from-scratch implementation against the published
  T64/P00 format specs. T64 files can hold several entries and all get imported;
  P00-family holds exactly one.
- `PetsciiConverter` ← the character-transform loop in `ViewSeqFile`
- `ErrorInfo` (read/write the error-info section, 683 or 768 bytes depending on
  track count) + `D64ErrorCodes` ← `ErrorCodeBlocks` and frmErrorNumbers' error table
- `PreferredStartTrack`/`PreferredStartSector` on `D64Image`, consulted by
  `ImportFile` ← `UserSpecifiedFirstTrack`/`UserSpecifiedFirstSector` from Config
- `AddArtEntry` ← `CopySeparatorsToD64`
- 40-track (non-standard extended) disk support — `TrackCount`/`TotalBlocks` are
  now per-instance (35/683 or 40/768) instead of fixed constants, detected from
  file size on load. Tracks 36-40 have no BAM representation on any real or
  emulated 1541 (the format spec only ever reserved BAM entries for 1-35), so
  this port always reports those blocks as allocated — existing files there
  read/write fine, but the free-block allocator will never write new files
  into them. This matches the original's own "only partially supported" 40-track
  handling rather than inventing a fuller non-standard scheme with no spec to
  check it against.
- GEOS directory-entry metadata (`D64FileEntry.IsGeosFile`/`GeosFileTypeName`/
  `IsGeosVlir`/`GeosInfoBlock`/`GeosModified`, `D64Image.IsGeosDisk`). **Not
  ported** — bytes 0x15-0x1D were previously just preserved byte-for-byte
  undecoded, same as the original. This is a from-scratch decode against
  published GEOS format documentation, not verified against a real GEOS disk —
  worth testing against one if you have it, since GEOS format details have more
  variance across sources than the base 1541 format.

## What's in the demo windows (D64Editor.App)

`MainWindow` ports `frmMain`'s disk-browsing and file-transfer slice: Open,
Import File, Export Selected, disk name/ID display, directory listing,
blocks-free/file-count summary. Saves after every import so changes aren't
lost, same spirit as the original writing straight to the file on disk.

`BamEditorWindow` ports `frmBAM` ("BAM"): a click-to-toggle grid of all 683
blocks, laid out the same way the original does it — tracks as columns,
sectors as rows, blue cells with a white "*" for allocated blocks, and the
directory track (18) highlighted red. BLOCK EDIT MODE / BAM EDIT MODE and
DECIMAL / HEX are real radio-button toggles now, matching the original's
controls instead of being buried in a menu. Hover shows track/sector +
owning file; menu has Allocate All / Unallocate All (protects 18/0 and 18/1)
/ Clean Disk / Save. The original hand-placed 683 Label controls in the VB6
designer (that's why `frmBAM.frm` was 16,000 lines) — here the grid is
generated at runtime from `D64Image`'s own geometry instead.

`BlockViewerWindow` ports `frmBlockViewer` (captioned "Block Editor" in the
original, despite the file name — kept that caption here too): a 16x16 hex
grid plus a toggleable ASCII column, click a byte (or arrow-key/Tab through
them) and type its new hex value, Wipe Block, Save Changes to Block,
Previous/Next block navigation (◄ ►, matching the original's arrow buttons),
C128 auto-boot sector detection. The ASCII column renders with the C64 PETSCII
font/glyph mapping and follows the centralized **View → PETSCII Lower/Upper
Case** charset toggle.

`SeqViewerWindow` ports `frmFileViewer` ("SEQ File Viewer"): shows a SEQ
file's text content, with 40/80 Column and ASCII/PETSCII toggles. PETSCII mode
is ROM-bitmap rendered (C64 character ROM / quote-control handling) and follows
the centralized charset toggle; ASCII mode shows converted text. "Save to Text"
exports the appropriate form. Printing isn't ported.

`SectorDumpWindow` ports `frmSectorDump`: pick a track/sector, dump that
block's 256 raw bytes to a file. The original's Hex/Decimal display toggle
for the track/sector pickers isn't ported — track and sector are always
plain decimal (the underlying math was identical either way).

`MemoryWindow` ports `frmMemory` — a staging area where files are collected
and then batch-copied into the currently open disk, exported back out, or
dropped. "Copy Files to Memory..." in the main window copies the currently
selected directory entries into memory (mirroring the original population
path), and the memory window also has a direct "Add File..." picker. Selection
uses Avalonia's native multi-select `ListBox` rather than the original's
hand-rolled `Selected(i)` boolean array walked by `mnuInvertSelection`/
`mnuSelectAllFiles`. When files are copied into the disk from the memory
window, the main window's directory listing is refreshed.

`ConfigWindow` ports `frmConfig`: import interleave, starting track/sector,
disable cross-link checking, safe disk clean, SEQ viewer's default character
mode, the separator-line text setting, and the "treat track 18/0-1 start
files as empty when copying to memory" option. Settings live in the static
`AppSettings` class (one place instead of the scattered globals the original
wrote into) and get applied to a `D64Image` immediately when set.

`FilePropertiesWindow` ports `frmFileProperties`: edit a directory entry's
filename, starting track/sector, size, type, locked/splat flags directly.
Much less code than the original needed, since `D64FileEntry` already models
these as real fields instead of a hand-assembled binary string. "Filename
Builder..." opens the Filename Builder and feeds the built name back into the
filename field.

`ErrorNumbersWindow` + `CrossLinksWindow` port `frmErrorNumbers` and
`frmCrossLinks`. Error Numbers opens from the BAM editor's new third mode
("SET ERROR CODE" — the original triggered this via a caption-check hack on
the same BAM editor form; here it's just an honest third mode) and only
applies to 175531-byte images with an error-info section. Cross-Links opens
automatically after loading a disk if any cross-linked blocks are found,
same as the original's automatic popup, unless disabled in Config.

`FilenameBuilderWindow` ports `frmFilenameBuilder` — the last of the fourteen
original forms. It's not just a character-art tool: it's a reusable library
of "separator" snippets that can be copied straight into a disk's directory
as zero-block DEL entries, which is how art like "USE 38 TRACKS COPY" (or
that Krestology disk) actually gets built — a handful of saved lines,
recombined per disk. Two entry points, matching the original's dual purpose:
standalone from the main Edit menu (build art, copy separators into the open
disk), or from File Properties' "Filename Builder..." button (builds a name
and feeds it back into that filename field instead). The PETSCII character
grid shows decimal byte codes rather than glyphs until a real font's wired
up via `Fonts.cs` — same limitation as the Block Editor's ASCII column, for
the same reason. Separators persist to a file under the user's app-data
folder now instead of the original's hard-coded Windows install directory.

Together these show the pattern (event handler → calls into `D64Image` →
updates controls) that all fourteen forms follow.

## PETSCII display (font and ROM rendering)

The app ships with **C64 Pro Mono** (style64.org C64 TrueType, bundled in
`D64Editor.App/Assets/Fonts/`, wired in `Fonts.cs`) and a bundled VICE C64
character ROM (`Assets/chargen.bin`) for exact ROM-bitmap rendering.

- **ROM-bitmap rendering** (`PetsciiScreen`/`PetsciiView`, Pepto palette): used
  for the directory (per-row, with selection preserved), SEQ (PETSCII mode), and
  BASIC listings, with correct C64 graphics/control/quote-mode behavior.
- **Font/PUA rendering:** hex/other PETSCII text uses the C64 font with
  screen-code→PUA glyph mapping (`Petscii.cs`) so the charset is accurate.
- **Central charset:** **View → PETSCII Lower/Upper Case** toggles
  Upper/Graphics (set 1) ↔ Lower/Upper (set 2), persisted and applied across
  PETSCII-displaying views.

To use a different PETSCII font, drop it in `Assets/Fonts/`, update `FileName`
and `FamilyName` in `Fonts.cs` (the internal family name, e.g. `fc-scan <font> |
grep family`), and ensure `PetsciiFontAvailable` is set. `chargen.bin` is the
4×1K screen-code-indexed ROM image (set1/set2, normal+reverse) used by the
bitmap renderer.

## Drag-and-drop and container import

Drop a `.d64` onto the main window to open it, or drop any other file
(including `.t64`/`.p00`/`.s00`/`.u00`/`.r00`, and multiple files at once) to
import it into whatever disk's currently open. This isn't a port of
`DragDrop.bas` — that wrapped Windows' OLE drag-drop APIs, which don't map
onto Avalonia's drag-drop model closely enough for a line-by-line port to
mean much — but the resulting behavior matches. The same container detection
also applies to the regular "Import File..." button and to "Add File..." in
the Memory window.

## The last nine menu items

All of frmMain's menu is now live. The nine that were previously grayed out:

- **Trim File** — standalone tool (works without a disk open), truncates an
  oversized/corrupt dump down to the standard 174848 bytes.
- **Revert** — reloads the open disk from its file, discarding unsaved
  changes, with a confirmation prompt (via the new `ConfirmWindow`, since
  Avalonia has no built-in MessageBox).
- **Create Blank D64** — new `CreateBlankWindow`. Builds a fresh 35- or 40-track
  disk by reusing `WriteBam`/`WriteDirectory` against an empty entry list,
  rather than hand-writing every BAM/directory byte the way the original did.
  "Protect track 1, sector 0" and the C128 auto-boot-patch option are
  supported.
- **Export DIR to TXT** — writes `D64Image.ExportDirectoryListing()` to a
  text file; reformatted to this port's own column layout rather than
  matching the original's fixed-width padding byte-for-byte.
- **Scratch File / Unscratch File** — standard C64 DOS soft-delete: Scratch
  frees the blocks and zeroes the type byte but leaves the entry in place;
  Unscratch re-walks the still-intact track/sector chain and re-allocates
  the blocks. One deliberate difference: the original re-allocated blocks
  on unscratch without restoring a file type, leaving that for File
  Properties afterward. This port restores as an unlocked PRG by default
  instead (right far more often than not) and still leaves File Properties
  there to correct it if it's wrong.
- **Wipe File** — hard delete: zeroes the block data, frees it, and removes
  the entry. Not recoverable, so it goes through the confirmation dialog.
- **Remove Directory Entry** — pulls an entry out of the directory without
  touching its blocks or the BAM at all, for cleaning up a corrupted/orphaned
  entry. The original did this via a hand-rolled shuffle across a dozen
  parallel arrays; `List<T>.Remove` is the same operation without the
  ceremony `D64FileEntry` being one object per file makes possible.
- **View File Chain** — new `FileChainWindow`. Shows a file's block chain as
  an ordered track/sector/block list rather than the original's highlighted
  overlay on the BAM grid — same information, without duplicating the BAM
  grid UI just to highlight a handful of cells in it.

The directory list is multi-select now (it was single-select before), since
Scratch/Unscratch/Wipe/Remove all operate over however many files are
selected, matching the original.

## Not yet ported

- (none noted) All fourteen original forms have a C# counterpart, and the
  follow-up requests (T64/P00 import, 40-track, GEOS metadata, drag-and-drop)
  are in too. Config now includes the separator-line text setting and the
  "treat track 18/0-1 start files as empty when copying to memory" option, and
  "Copy Files to Memory" copies the currently selected directory entries into
  the memory staging window (in addition to adding files from disk inside the
  memory window).

## Multi-format engine and disk-editor features (added)

Beyond the VB6 D64 port, this build now carries a format-agnostic layer and a
growing set of features comparable to other C64/CBM disk editors. The current port scope / status is
tracked in `D64Editor.App/Roadmap.md`.

**Disk image formats**

- **D64** — full editor (load/save, BAM, scratch/unscratch/wipe, import,
  properties, viewers, multi-window).
- **D71 / D81 / D80 / D82 / DNP** — directory viewer + Hex/BASIC viewers +
  Export-PRG. Directory-entry editing (rename, file type, start t/s, size) is
  supported for all of these, and **scratch with block-freeing** plus **file
  import** are implemented for D81 (import/scratch for the others is planned).
- **G64 / NIB** — raw-GCR images; on open they are **decoded to a D64** (nibtools/VICE
  GCR pipeline: bit-level sync detection, 5-to-4 decode, `0x08` header + `0x07`
  data blocks) and then open fully editable in the editor. Verified byte-exact.
- **T64** — tape-archive reader + viewer (Verified on a synthetic `.t64`).
- **D2M / D4M / DHD** — CMD container partition tables; the Parts view lists
  partitions and opens the selected one (emulated 1541/1571/1581 land in the
  editor, native DNP opens in its viewer).
- **CP/M 2.2 images + `.LBR`** — best-effort directory catalog (CP/M is
  DPB-parameterized, so it lists/labels entries and never assumes byte counts).
- **C128 CP/M (`.d64/.d71/.d81`)** — best-effort, DPB-aware file extraction and
  template-based image create/copy (all files or subset) via CLI, suitable for
  writing with OpenCBM (`--cpm-extract`, `--cpm-copy`). Relies on a compatible
  bootable template for the target format/layout.

Core readers: `D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`,
`NibImage`, `T64Image`, `CpmDisk`, `DiskContainers`, plus the shared
`CbmImage`/`Gcr`/`DiskFormats`/`CbmCatalog`/`ImageCatalog`/`CbmScratch`/`CbmImport`
helpers.

**Disk-editor features**

- Menu (`Disk, File, Edit, View, Window, Help`) + toolbar
  with C64 color selectors (Blue / Lt Blue) that recolor the directory.
- C64-screen directory: ROM-bitmap rendered (VICE C64 character ROM via
  `PetsciiScreen`/`PetsciiView`, Pepto palette) with per-row selection preserved;
  PETSCII monospace font fallback also wired. Toggle name case, font-size cycling,
  Show DEL files, save-as, multi-disk/tabbed workspace, unsaved-changes prompt.
- Centralized **View → PETSCII Lower/Upper Case** toggle (Upper/Graphics set 1 vs
  Lower/Upper set 2), persisted and applied across PETSCII-displaying views
  (directory, hex, SEQ, BASIC, disassembly, block/sector ASCII, T64/extended/Find).
- Viewers: Hex, BASIC (C64 BASIC V2 de-tokenizer), Disassembly (6502), plus the
  picture decoder (Koala Painter/Art Studio hi-res), charset (256-glyph grid),
  sprite (hires + multicolor), and REL (fixed-length record) viewers.
  - **Hex viewer:** fixed-column dump per row — `AAAA <16 two-digit bytes> <PETSCII>`
    (4-hex memory address, space, 16 bytes, space, PETSCII chars), base from the
    file’s 2-byte load address (low,high) when present, C64/ROM glyph rendering,
    PETSCII/ASCII char toggle, and it follows the global charset.
  - **SEQ/BASIC:** ROM-bitmap PETSCII rendering (SEQ respects quote/control mode;
    BASIC listing rendered in quote mode).
- Tools: Find in Disks (name **and** file-contents search), Compare Disks
  (header/directory/BAM diff + differing-block list), Batch export of a folder's
  files and Batch Validate (report-only).
- Window management: the Window menu lists open documents/tabs (tabbed MDI shell).
- CLI: `--help`, `--list`, `--info`, `--validate` (D64 cross-links/error-block summary),
  `--export <image> <dir>` (D64/T64/D71/D81/D80/D82/DNP), `--convert <g64/nib> <d64>`
  (headless G64/NIB→D64), `--cpm-extract <image> <dir>` / `--cpm-copy <tmpl> <out> [--all|--files A,B]`
  (C128 CP/M, best-effort/template-based), `--extract-archive <zip|tar|tar.gz|tgz> <dir> [--images-only]`,
  `--print-dir <image>` (D64), and passing an image path opens it in the GUI.
- PETSCII: **C64 Pro Mono** (style64.org C64 TrueType, `Fonts.cs`) + PUA/screen-code
  mapping (`Petscii.cs`) and a bundled VICE `chargen.bin` for exact ROM-bitmap
  rendering. Central charset toggle (upper/graphics ↔ lower/upper).

All new format readers were verified by generating synthetic images of each
format (with a matching GCR encoder for G64/NIB) and byte-comparing the parsed
results — see the per-phase notes in `D64Editor.App/Roadmap.md`.

## Not yet done (see Roadmap.md)

- G64/NIB editing beyond D64 conversion; NIB structural viewer fallback.
- True single-window tabbed MDI host: DONE — `DiskDocumentView` is the per-disk editor;
  `MainWindow` is a menu/toolbar + `TabControl` shell; Window menu lists open documents.
  (Fresh code — please smoke-test open/close/scratch/import/tabs.)
- Full REL *editing*: record content is editable in place (DONE); REL create/grow and resize
  are still to do (need side/data-sector allocation).
- Native OS print dialog and a built-in updater installer (platform-dependent).
  "Print Directory…" writes a printable listing; "Check for Updates" only reads a
  version-feed URL.
- Emulator/OpenCBM need the emulator or OpenCBM + hardware installed to act.

## OS integration (packaging/)

- Emulator: `Run > Configure Emulators…` (command template), then `Run in Emulator…`.
  `{disk}` = autostart target, `{image}` = disk path for mounting. Default mounts
  drive 8 and autostarts (`x64sc -8 "{image}" -autostart "{disk}"` behavior; auto-
  injects `-8 <image>` when needed). No selection → autostart disk/`*` (first file);
  one selected file → autostart `disk:NAME` (normalized); **$0801 BASIC** adds
  `-autostartprgmode 1` (LOAD ,8,1 style) when absent. Paths are shell-quoted safely.
- OpenCBM: `CBM > OpenCBM Transfer…` detects `cbmcopy`/`cbmctrl` on `PATH` and
  runs a command with `{device}/{disk}/{out}` substituted.
- File associations: `packaging/install-associations.sh <exe>` (Linux `.desktop` +
  MIME, per-user) and `packaging/d64-file-association.reg` (Windows).


## A note on fidelity vs. cleanup

Where the VB6 code did something because VB6 forced it to (module-level
globals instead of instance state, double-precision math to avoid writing
integer division, five different `If Then` blocks repeating the same
track-range check), I cleaned it up. Where the VB6 code encoded actual 1541
disk-format knowledge (byte offsets, bit ordering, the directory-chain
algorithm, the free-block search's spiral pattern), I kept it faithful and
commented where each piece came from, so you can diff against the original
if something's ever in question.