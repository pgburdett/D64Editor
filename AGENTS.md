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
- Multi-format layer lives in Core (`D71Image`, `D81Image`, `D80D82Image`, `DnpImage`, `G64Image`, `NibImage`, `T64Image`, `CpmDisk`, `DiskContainers`) with shared helpers (`CbmImage`, `Gcr`, `DiskFormats`, `CbmCatalog`, `ImageCatalog`, `CbmScratch`, `CbmImport`). Each reader is verified against a synthetic image of its format. Feature/phase roadmap is tracked in `D64Editor.App/Roadmap.md`.
