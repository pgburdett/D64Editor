

[![.NET](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/pgburdett/D64Editor/actions/workflows/dotnet.yml)

# D64 Editor — VB6 → C#/.NET 8 / Avalonia port

Original: D64 Editor by  *** Forrest Mook ***, GPL v2, 1999-2016. https://www.d64editor.com 

This is a C# port of the core disk engine.  


## Build (on your linux or windows machine ( I use Garuda Dr4g0niz3d as primary)

```
sudo pacman -S dotnet-sdk    # if you don't already have it
cd D64Editor
dotnet restore
dotnet run --project D64Editor.App
```

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
C128 auto-boot sector detection. No CBM64 font is available in this
environment, so the ASCII column shows printable ASCII (or ".") rather than
true PETSCII glyphs — same limitation as the original's own "ASCII" display
mode, just without the "PETASCII" mode's custom bitmap font as an alternative.

`SeqViewerWindow` ports `frmFileViewer` ("SEQ File Viewer"): shows a SEQ
file's text content, with 40/80 Column (window width only, matching the
original) and ASCII/PETASCII toggles. PETASCII mode here is Latin-1 raw
bytes rather than true C64 glyphs, for the same font-availability reason as
the Block Editor's ASCII column. Printing isn't ported.

`SectorDumpWindow` ports `frmSectorDump`: pick a track/sector, dump that
block's 256 raw bytes to a file. The original's Hex/Decimal display toggle
for the track/sector pickers isn't ported — track and sector are always
plain decimal (the underlying math was identical either way).

`MemoryWindow` ports `frmMemory` — a staging area where files are collected
and then batch-copied into the currently open disk, exported back out, or
dropped. The original populated this list via a "copy to memory" action
elsewhere in `frmMain` that wasn't part of the source reviewed for this port;
here there's a direct "Add File..." picker instead. Selection uses Avalonia's
native multi-select `ListBox` rather than the original's hand-rolled
`Selected(i)` boolean array walked by `mnuInvertSelection`/`mnuSelectAllFiles`.
Note: copying files in from here doesn't yet refresh the main window's
directory listing — reopen the disk (or wire up a refresh callback) to see
them.

`ConfigWindow` ports `frmConfig`: import interleave, starting track/sector,
disable cross-link checking, safe disk clean, and SEQ viewer's default
character mode. Settings live in the static `AppSettings` class (one place
instead of the half-dozen scattered globals the original wrote into) and get
applied to a `D64Image` immediately when set. The separator-line and
Filename Builder settings aren't here since that form isn't ported yet.

`FilePropertiesWindow` ports `frmFileProperties`: edit a directory entry's
filename, starting track/sector, size, type, locked/splat flags directly.
Much less code than the original needed, since `D64FileEntry` already models
these as real fields instead of a hand-assembled binary string. The
"Filename Builder..." button is present but disabled until that form exists.

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

## Using a real PETSCII/CBM64 font

The Block Editor's ASCII column and the SEQ Viewer's PETASCII mode fall back
to plain ASCII/Latin-1 without a real C64 font. The Filename Builder's
character grid falls back to decimal byte codes instead, since a Latin-1
guess at 256 arbitrary values would mostly be wrong rather than
approximately right. To wire a real font in:

1. Drop the font file into `D64Editor.App/Assets/Fonts/` (any name, `.ttf` or `.otf`).
2. Open `D64Editor.App/Fonts.cs` and update `FileName` and `FamilyName` to match
   (FamilyName has to be the font's actual internal family name, not just the
   filename — a font viewer or `fc-scan yourfont.ttf | grep family` will show it).
3. Flip `PetsciiFontAvailable` to `true`.

Once that's done, both windows switch to using the font directly instead of
their ASCII/Latin-1 approximations.

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
- **Create Blank D64** — new `CreateBlankWindow`. Builds a fresh, fully-free
  35- or 40-track disk by reusing `WriteBam`/`WriteDirectory` against an
  empty entry list, rather than hand-writing every BAM/directory byte the
  way the original did. The original's "protect track 1/0" and C128
  auto-boot-patch options aren't ported.
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

- Config's separator-line setting (the original tied a single separator
  string into Config directly; this port's separator library in the
  Filename Builder covers the same need without that extra setting)
- `frmMemory`'s original population path (whatever elsewhere in `frmMain`
  fed files into the memory cache before this port's "Add File..." button)

All fourteen original forms have a C# counterpart, and the follow-up requests
(T64/P00 import, 40-track, GEOS metadata, drag-and-drop) are in too. What's
left above is genuinely minor scope, not missing base functionality.

## A note on fidelity vs. cleanup

Where the VB6 code did something because VB6 forced it to (module-level
globals instead of instance state, double-precision math to avoid writing
integer division, five different `If Then` blocks repeating the same
track-range check), I cleaned it up. Where the VB6 code encoded actual 1541
disk-format knowledge (byte offsets, bit ordering, the directory-chain
algorithm, the free-block search's spiral pattern), I kept it faithful and
commented where each piece came from, so you can diff against the original
if something's ever in question.
