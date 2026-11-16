# AGENTS.md

## Project overview
- C#/.NET port of "D64 Editor" (1541 `.d64` disk editor) with Avalonia UI.
- Solution: `D64Editor.sln` with two projects:
  - `D64Editor.Core/` — disk/image engine and format logic (library, `net10.0`, `LangVersion 12`, nullable enabled).
  - `D64Editor.App/` — Avalonia desktop UI (Exe, `net10.0`, `LangVersion 12`, nullable enabled, `Avalonia 11.3.4`). Entry point: `D64Editor.App/Program.cs`.
- `D64Editor.App` references `D64Editor.Core`. Keep format/disk behavior changes in `Core` and UI changes in `App` unless there is a clear reason.

## Commands
- Restore: `dotnet restore`
- Build: `dotnet build` (or `dotnet build --no-restore` after restore)
- Run UI: `dotnet run --project D64Editor.App`
- Test: `dotnet test` / `dotnet test --no-build --verbosity normal`
- There are currently no `*Test*`/`*.Tests*` projects in the repo; CI still runs `dotnet test`, so expect "no tests" behavior rather than a populated suite.

## CI / environment notes
- `.github/workflows/dotnet.yml` runs on `ubuntu-latest` (push/PR to `master`) with .NET SDK `10.0.x`: `dotnet restore`, `dotnet build --no-restore`, `dotnet test --no-build --verbosity normal`.
- Projects target `net10.0` (`LangVersion 12`, nullable enabled), no `global.json`.
- README documents local build with `dotnet-sdk` installed (`sudo pacman -S dotnet-sdk` example) and running `D64Editor.App`.

## Repo-specific guidance
- `src/` contains the original VB6 source (`.frm`/`.bas`/`.vbp`, e.g. `Module1.bas`, `frmMain.frm`); use it as the reference for 1541-format behavior and port fidelity questions. `README.md` summarizes port scope and intentional differences.
- Prefer the code/config as executable truth over README/CI prose when they disagree (e.g., target framework vs CI SDK version).
- Workspace may contain local artifacts (`bin/`, `obj/`, `.idea/`, `D64Editor.sln.DotSettings.user`, `d64editor-build-archives/`); avoid treating them as source of truth or modifying them unless requested.
- PETSCII font wiring is documented in `README.md` and centered around `D64Editor.App/Fonts.cs` and `D64Editor.App/Assets/Fonts/` (font file + internal family name + `PetsciiFontAvailable` flag).
- Multi-format layer lives in Core (`D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `C128Cpm`, `DiskContainers`) with shared helpers (`CbmImage`, `Gcr`, `DiskFormats`, `CbmCatalog`, `ImageCatalog`, `CbmScratch`, `CbmImport`). Each reader is verified against a synthetic image of its format. Feature/phase roadmap is tracked in `D64Editor.App/Roadmap.md`.

## Architecture notes
- **Core/App separation:** All disk-format behavior, allocation logic, and file I/O belong in `D64Editor.Core`. UI rendering, window management, and user interaction belong in `D64Editor.App`. The App references Core; Core has no dependency on App.
- **IDiskImage interface:** Implemented by `D64Image` for format-agnostic operations. CBM-layout images (D71/D81/D80/D82/DNP) implement `ICbmWritable` for scratch/unscratch/import.
- **D64Image raw data:** Internal `_data` byte array holds the full image. `GetRawDataCopy()`/`RestoreRawData(byte[])` provide snapshot/restore for undo/redo. Directory/BAM/owners are rebuilt from `_data` on restore.
- **Nullable enabled:** All projects use nullable reference types. Use `?` annotations and null-conditional access where appropriate.
- **LangVersion 12:** C# 12 features (primary constructors, collection expressions, etc.) are available.

## Key files and their roles
- `D64Editor.Core/D64Image.cs` — D64 geometry, BAM, directory, chain logic, import/export, snapshot/restore.
- `D64Editor.Core/D64BlockAllocator.cs` — Free-block search (spiral pattern from track 17).
- `D64Editor.Core/RelFile.cs` — REL file model, reading, in-place record writing.
- `D64Editor.Core/RelReader.cs` — REL side-sector chain reader.
- `D64Editor.Core/RelWrite.cs` — REL file create and resize (grow/shrink record count).
- `D64Editor.Core/T64Image.cs` — T64 tape archive reader/writer (mutable, rebuild/save).
- `D64Editor.Core/C128Cpm.cs` — C128 CP/M extraction, template-based create/copy/format.
- `D64Editor.Core/DnpImage.cs` — DNP reader/writer with container slice support.
- `D64Editor.Core/DiskContainers.cs` — D2M/D4M/DHD partition table parsing.
- `D64Editor.Core/G64Image.cs` / `NibImage.cs` — GCR image parsing and D64 conversion.
- `D64Editor.Core/CpmDisk.cs` — CP/M 2.2 directory reader.
- `D64Editor.App/DiskDocumentView.axaml.cs` — Per-disk editor view (directory, actions, undo/redo).
- `D64Editor.App/MainWindow.axaml.cs` — Shell: menu/toolbar/tabbed MDI, active-document dispatch.
- `D64Editor.App/Cli.cs` — CLI handler (all commands).
- `D64Editor.App/AppSettings.cs` — Persisted settings (emulator command, charset, etc.).
- `D64Editor.App/Fonts.cs` — PETSCII font wiring.
- `D64Editor.App/Roadmap.md` — Feature roadmap and phase status.

## CLI commands (summary)
All CLI commands are implemented in `D64Editor.App/Cli.cs`. See README.md for full usage.
- `--help` / `-h` / `help` — Show help text.
- `--convert <in> <out>` / `-c` — Convert G64/NIB to D64 (or copy native image).
- `--list <image>` / `-l` — Print an image's directory.
- `--export <image> <dir>` / `-e` — Export files from D64/T64/D71/D81/D80/D82/DNP to a directory.
- `--info <image>` / `-i` — Show image/format information.
- `--validate <image>` / `-v` — Validate/report cross-links and basic issues (D64).
- `--cpm-extract <img> <dir>` — Extract CP/M files (best-effort DPB-aware, .d64/.d71/.d81).
- `--cpm-copy <tmpl> <out> [--all|--files A,B]` — Create CP/M image from template (keep all or subset).
- `--cpm-format <tmpl> <out>` — Create empty CP/M image from template (clears directory; template-based).
- `--extract-archive <zip|tar|tar.gz|tgz> <dir> [--images-only]` — Extract archive contents.
- `--print-dir <image>` — Print directory listing (text, D64).
- `--geos-info <d64> <file>` — Show GEOS/REL metadata and best-effort VLIR info for a file (D64).
- `<image>` (no verb) — Open the image in the GUI.
- (no arguments) — Start the GUI.

## Multi-format layer
- `D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `C128Cpm`, `DiskContainers` — format readers.
- Shared helpers: `CbmImage`, `Gcr`, `DiskFormats`, `CbmCatalog`, `ImageCatalog`, `CbmScratch`, `CbmImport`, `RelFile`, `RelReader`, `RelWrite`.
- Each reader verified against synthetic images of its format.
- `ICbmWritable` interface: implemented by D71/D81/D80/D82/DNP for scratch/unscratch/import.

## CP/M support notes
- C128 CP/M uses 7-bit ASCII (not PETSCII) for names and text.
- CP/M-on-`.d64` cases are detected (boot signature `CBM`) and opened in the CP/M viewer.
- Template-based create/copy/format: relies on compatible bootable template; not a from-scratch formatter.
- `.d71`/`.d81` mappings and DPB params are best-effort; need validation against real images/templates.

## Emulator integration notes
- Template placeholders: `{disk}`, `{image}`, `{appdir}`, `{app}`, `{cwd}`.
- Substitution handles whole quoted tokens and inside-quoted arguments.
- VICE-compatible lowercase filename mapping via `DirectoryFormatter.PetsciiToEmulatorName`.
- BASIC programs ($0801) get `-autostartprgmode 1` appended.
- Cartridge paths use `-cartcrt` (not `-cartepyx`) for `.crt` files.

## Graphics viewers
- `GfxViewerWindow` (charset/sprite) and `PictureViewerWindow` (Koala/Art Studio hi-res) support zoom and BMP save/export.
- `GfxDecoder` decodes charset (2 KiB → 128×128) and sprite (63 bytes → 24×21 hires or multicolor).

## Undo/Redo implementation
- `D64Image.GetRawDataCopy()` / `RestoreRawData(byte[])` provide snapshot/restore.
- `DiskDocumentView` maintains `_undo` and `_redo` stacks (capped at 50).
- `PushUndo()` called before key mutations.
- Available via Edit menu and `Ctrl+Z`/`Ctrl+Y`.

## Scripting (headless automation)
- `D64Editor.Core/ScriptEngine.cs` — line-oriented script runner over the Core engine; no UI dependencies. `Run(text, wd)` / `RunFile(path, wd)` return `ScriptResult` (log, error, exit code).
- CLI entry point: `D64Editor.App/Cli.cs` `--run-script`. `Environment.ExitCode` propagates the script exit code.
- `D64Editor.Core/ArchiveExtract.cs` — zip/tar/tar.gz/tgz extraction used by the `archive` script command (and the CLI `--extract-archive`).
- Script commands cover: image lifecycle (open/close/save/new/convert), directory/file ops (list/info/validate/import/export/exportdir/rename/renamedisk/scratch/unscratch/wipe/move), REL (relnew/relresize/relwrite), CP/M (cpmextract/cpmcopy/cpmformat), archive, geosinfo, echo/help/exit.
- Sample scripts live in `scripts/`. When adding Core automation, expose it through ScriptEngine and document it in README's Scripting section.
- `D64Image.RenameDisk(name,id)` was added for headless disk-header renaming.

## GCR write-back / container / CP/M notes
- `Gcr.EncodeTrack(d64, track, speedZone)` builds raw GCR track bytes; `G64Image.ToG64Bytes`/`FromD64` and `NibImage.ToNibBytes`/`FromD64` build containers. Round-trip D64→G64/NIB→D64 is byte-exact (verified 683/683 sectors). Half-track/copy-protected data is preserved verbatim, not re-encoded.
- Container-backed images: `D64Image`/`D71Image`/`D81Image`/`DnpImage` all have `TryLoadFromContainer` + `ContainerPath/Offset/Size`; `Save()` writes back into the container. `CbmCatalog.Open{D64,D71,D81,Dnp}FromContainer` and `PartitionsWindow` open partitions directly with write-back.
- `C128Cpm.ReadHostDirectory`/`HostNameToCpm83`/`ImportDirectoryFromBootDonor` import a whole host dir to CP/M with 8.3 name mapping and collision suffixes; `CpmImportReport` logs mappings/skips.
- `C128Cpm.FormatBlank`/`WriteFiles`/`FormatAndWrite` create CP/M images from scratch (no template). D64 directory uses the real interleaved track-1 sectors (3,4,9,10,14,15,19,20) from the reference system disk; bitmap in sector 2. D71/D81 use a self-consistent block-based directory. Bootability still not verified against real media.

## Known limitations
- G64/NIB GCR fidelity tuning remains incremental (per-zone interleave/gaps, copy-protected track re-encoding); write-back round-trips byte-exact.
- CMD container write-back works for all partition types; richer in-container editing workflows remain incremental.
- From-scratch CP/M (`--cpm-new`) is data-only (not bootable, emulator-verified); use `--cpm-bootable` with a system-disk donor for bootable images. From-scratch DPB/skew/bitmap remain unverified.
- Broader inter-tab/window disk-to-disk drag-and-drop not fully implemented.
- Undo/redo coverage not comprehensive across all edit paths.
- Native OS print dialog and built-in updater/installer not implemented (platform-dependent).
- Deeper GEOS disk/application-data/VLIR workflows remain best-effort.

## Local artifacts (do not commit)
- `c128-cpm.system.6228129311-9283de.d64`, `c128-cpm-new.d64`, `Epyx_FastLoad_1984_v2-b11b3b.crt` — untracked test artifacts.
- `bin/`, `obj/`, `.idea/`, `*.user`, `d64editor-build-archives/` — build artifacts.