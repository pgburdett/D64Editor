# D64 Editor — VB6 → C#/.NET 8 / Avalonia port

Original: D64 Editor by Forrest Mook, GPL v2, 1999-2016. This is a from-scratch
C# port of the core disk engine plus one fully working window, as a template
for porting the rest.

**Everything below was hand-translated line-by-line against the
original .bas/.frm source. 

## Build (on your Garuda Dragonize (ARCH) or Windows 11 box)

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
  payloads; the P00 and T64 container-format detection isn't ported)
- `PetsciiConverter` ← the character-transform loop in `ViewSeqFile`
- `ErrorInfo` (read/write the 683-byte error-info section on 175531-byte
  images) + `D64ErrorCodes` ← `ErrorCodeBlocks` and frmErrorNumbers' error table
- `PreferredStartTrack`/`PreferredStartSector` on `D64Image`, consulted by
  `ImportFile` ← `UserSpecifiedFirstTrack`/`UserSpecifiedFirstSector` from Config

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

Together these show the pattern (event handler → calls into `D64Image` →
updates controls) so the remaining form can follow the same shape.

## Using a real PETSCII/CBM64 font

The Block Editor's ASCII column and the SEQ Viewer's PETASCII mode fall back
to plain ASCII/Latin-1 without a real C64 font. To wire one in:

1. Drop the font file into `D64Editor.App/Assets/Fonts/` (any name, `.ttf` or `.otf`).
2. Open `D64Editor.App/Fonts.cs` and update `FileName` and `FamilyName` to match
   (FamilyName has to be the font's actual internal family name, not just the
   filename — a font viewer or `fc-scan yourfont.ttf | grep family` will show it).
3. Flip `PetsciiFontAvailable` to `true`.

Once that's done, both windows switch to using the font directly instead of
their ASCII/Latin-1 approximations.

## Not yet ported

- T64 and P00 container-format import (multi-file T64s were unsupported in
  the original too — it explicitly rejects those)
- Filename Builder (`frmFilenameBuilder`) — the PETSCII character-art editor
  used to build directory entries like the "USE 38 TRACKS COPY" example disk.
  This is the one substantial form left; it's a big one (6,196 lines in the
  original, mostly a hand-placed PETSCII character grid) and worth scoping
  as its own pass.
- 40-track disk images (the original only partially supported these; this
  port is standard 35-track only)
- GEOS-specific interpretation of bytes 0x15-0x1D (currently preserved
  byte-for-byte on round-trip, not decoded)
- Drag-and-drop import (`DragDrop.bas`)
- `frmMemory`'s original population path (whatever elsewhere in `frmMain`
  fed files into the memory cache before this port's "Add File..." button)
- Config's separator-line setting and Filename Builder-related options
  (belong with that form)

## A note on fidelity vs. cleanup

Where the VB6 code did something because VB6 forced it to (module-level
globals instead of instance state, double-precision math to avoid writing
integer division, five different `If Then` blocks repeating the same
track-range check), I cleaned it up. Where the VB6 code encoded actual 1541
disk-format knowledge (byte offsets, bit ordering, the directory-chain
algorithm, the free-block search's spiral pattern), I kept it faithful and
commented where each piece came from, so you can diff against the original
if something's ever in question.
