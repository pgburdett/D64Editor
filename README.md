# D64 Editor — VB6 → C#/.NET 8 / Avalonia port

Original: D64 Editor by Forrest Mook, GPL v2, 1999-2016. This is a from-scratch
C# port of the core disk engine plus one fully working window, as a template
for porting the rest.

**Everything below was hand-translated line-by-line against the
original .bas/.frm source. 

## Build (on your CachyOS box)

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

## What's in the demo windows (D64Editor.App)

`MainWindow` ports `frmMain`'s disk-browsing and file-transfer slice: Open,
Import File, Export Selected, disk name/ID display, directory listing,
blocks-free/file-count summary. Saves after every import so changes aren't
lost, same spirit as the original writing straight to the file on disk.

`BamEditorWindow` ports `frmBAM`: a click-to-toggle grid of all 683 blocks
(green = free, red = allocated), hover shows track/sector + owning file, menu
has Allocate All / Unallocate All (protects 18/0 and 18/1, same as the
original) / Clean Disk / Save, plus a mode switch (toggle allocation vs.
view/edit bytes) matching the original's `optBlockEditMode`. The original
hand-placed 683 Label controls in the VB6 designer (that's why `frmBAM.frm`
was 16,000 lines) — here the grid is generated at runtime from `D64Image`'s
own geometry instead.

`BlockViewerWindow` ports `frmBlockViewer`: a 16x16 hex grid of the selected
block's 256 bytes, click a byte (or arrow-key/Tab through them) and type its
new hex value, Wipe Block, Save Changes to Block, Next/Previous block
navigation, and C128 auto-boot sector detection. Opens from the BAM editor
when it's in "view/edit bytes" mode.

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

Together these show the pattern (event handler → calls into `D64Image` →
updates controls) so the remaining forms can follow the same shape.

## Not yet ported

- T64 and P00 container-format import (multi-file T64s were unsupported in
  the original too — it explicitly rejects those)
- 8 other forms (filename builder, error numbers, cross-links display,
  config, file properties, etc.)
- The BAM editor's "safe clean" mode (skip the directory track when cleaning)
  and its confirmation dialogs
- GEOS-specific interpretation of bytes 0x15-0x1D (currently preserved
  byte-for-byte on round-trip, not decoded)
- Drag-and-drop import (`DragDrop.bas`)
- `frmMemory`'s original population path (whatever elsewhere in `frmMain`
  fed files into the memory cache before this port's "Add File..." button)

## A note on fidelity vs. cleanup

Where the VB6 code did something because VB6 forced it to (module-level
globals instead of instance state, double-precision math to avoid writing
integer division, five different `If Then` blocks repeating the same
track-range check), I cleaned it up. Where the VB6 code encoded actual 1541
disk-format knowledge (byte offsets, bit ordering, the directory-chain
algorithm, the free-block search's spiral pattern), I kept it faithful and
commented where each piece came from, so you can diff against the original
if something's ever in question.
