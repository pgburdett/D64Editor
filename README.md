# D64 Editor — VB6 → C#/.NET 8 / Avalonia port

Original: D64 Editor by Forrest Mook, GPL v2, 1999-2016. This is a from-scratch
C# port of the core disk engine plus one fully working window, as a template
for porting the rest.

**I could not compile or run this in the sandbox** (no .NET SDK, no NuGet
access there). Everything below was hand-translated line-by-line against the
original .bas/.frm source and I'm confident in the logic, but you'll hit the
first real compiler errors when you build it — treat that as normal, not a
sign something's badly wrong.

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

## What's in the demo window (D64Editor.App)

`MainWindow` ports the "browse a disk" slice of `frmMain`: Open button, disk
name/ID display, directory listing, blocks-free/file-count summary. It's
read-only — no save, no file import/export yet. This is meant to show the
pattern (event handler → calls into `D64Image` → updates controls) so you can
see how the rest of `frmMain`'s ~50 event handlers would follow the same shape.

## Not yet ported

- File import/export (`FileImport`, `FileExport` in frmMain — reads/writes
  actual PRG/SEQ/REL data through the block chain)
- The other 13 forms (BAM editor, block viewer, sector dump, memory viewer,
  filename builder, cross-links, etc.)
- Cross-link detection (`getBlockOwners`'s crosslink-flagging half — the
  block-ownership walk itself is ported, the "flag it and pop up frmCrossLinks"
  part isn't, since that's UI)
- GEOS-specific interpretation of bytes 0x15-0x1D (currently preserved
  byte-for-byte on round-trip, not decoded)

## A note on fidelity vs. cleanup

Where the VB6 code did something because VB6 forced it to (module-level
globals instead of instance state, double-precision math to avoid writing
integer division, five different `If Then` blocks repeating the same
track-range check), I cleaned it up. Where the VB6 code encoded actual 1541
disk-format knowledge (byte offsets, bit ordering, the directory-chain
algorithm, the free-block search's spiral pattern), I kept it faithful and
commented where each piece came from, so you can diff against the original
if something's ever in question.
