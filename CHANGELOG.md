# Changelog

All notable changes to D64Editor are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/).

## [Unreleased] — Legacy archive support

### Added
- `D64Editor.Core/LegacyArchives.cs` for LHA/LZH, C64 LHA SFX, and ARJ operations.
- CLI verbs `--lha-list`, `--lha-extract`/`--lha-decompress`, `--lha-create`/`--lha-compress`,
  `--lha-sfx`, `--arj-list`, `--arj-extract`/`--arj-decompress`, `--arj-create`/`--arj-compress`,
  plus `--lhz-*` aliases.
- Script commands `lhaextract`, `lhacreate`, `lhasfx`, `arjextract`, `arjcreate`.
- `--extract-archive` and the `archive` script command accept `.lha`, `.lzh`, `.arj`, and C64 LHA
  SFX PRGs; SFX PRGs are detected by embedded LHA header checksum and carved automatically.
- Embedded `Assets/C64LhaSfxStub.prg`, recovered from a user-owned original disk and included with
  the user's stated license to use it; it is the default stub for `--lha-sfx`/`lhasfx`.

### Changed
- LHA creation writes standard LH0/stored archives directly (portable, not compressed). LHA
  listing/extraction uses external `lha` or `7z`; ARJ uses external `arj`.

## [Unreleased] — 1 KB streaming GCR capture

### Added
- `GcrReader.s` moved to `$0300` so the full 1 KB drive RAM buffer `$0400-$07FF` is captured in
  one revolution using 512-byte overlap windows.
- `GcrRead.Stitch()` uses a half-window probe (512 bytes for 1 KB windows, 128 for older 256-byte
  windows) with reverse search.

### Verified
- Exact rotation reconstruction on Arctic Fox T1 (7,692), T2 (6,666), and T36 (7,015) through
  `--test-stitch`. This is synthetic validation; real-hardware drift remains untested.

## [Unreleased] — Correlation-based GCR window stitcher

### Changed
- Earlier iteration aligned 256-byte windows with a 128-byte reverse-searched prefix.
- `--test-stitch <image.g64> <track>` added to validate stitcher output against known G64 tracks.

## [Unreleased] — Faithful SRQ nib capture and full documentation

### Added
- `tools/diagnostics/capture-srq.sh` and G64 launcher variants.
- `arcticfox.nib` and `arcticfox-srq.g64` captured from the real EA disk.
- Expanded `AGENTS.md` hardware/protection section, safety rules, and investigation fixtures.
- Complete `docs/TOOLS.md`, `docs/CODE.md`, and detailed drive-assembly/disk-image references.

### Verified
- `nibread -@xum1541 -D8 -e5 -h -v` captured the original EA disk; `nibconv` produced a G64 that
  boots in x64sc with `-drive8truedrive` to the "Enemy Review" tank screen. D64 → G64 conversion
  does not preserve protection.

## [Unreleased] — Raw GCR reader

### Added
- `GcrRead.cs`, `Assets/GcrReader.s/.bin`, `--gcrread`, and `--raw2g64` (`RawG64.cs`).
- Drive code reads VIA2 port A via BVC/SO byte-ready; motor-on uses DOS `$F97E` plus a spin-up
  delay (raw `ORA #$04` is insufficient).

### Known limitations
- 256-byte chunks capture correctly, but a 30-window `--whole` attempt produced 19 sync runs and
  0 decodable sectors on T1 — not a valid single-revolution stream. SRQ nibtools remains
  authoritative for full-track images.

## [Unreleased] — Serial raw-track writer

### Added
- `RawWriter.cs`, `Assets/RawWriter.s/.bin`, and `--writeraw <dev> <track> <hexpattern> <count>`.
- Drive-side VIA2 write mode; host uploads pattern/count/mask to drive RAM, FDC seeks, M-E runs the
  routine. Tracks >35 use the short non-homing `Assets/Stepper.s`.

### Safety
- DESTRUCTIVE. Blank test disk only. Hardware-verified on track 2; T36–40 path code-complete but
  untested.

## [Unreleased] — Protected-track reproduction

### Added
- `ProtectedTracks.cs` and `--protect <base.d64> <out.g64>`.
- T2: 6,666 bytes of alternating no-sync GCR pairs; T36: 7,015 bytes with 468 sync runs
  (original 469); T37–40: ~6,250 bytes with 33 sync runs each.

### Verified
- `--cbm gaps` shows T2 0 sync/0 sectors, T36 468/0, T37–40 33/0, matching the preservation
  image's anomaly signature.

## [Unreleased] — Offline GCR gap/sync analyzer

### Added
- `GapScan.cs` and `--cbm gaps <image.g64> [--from N] [--to N] [outfile]`.
- Counts `$FF` sync runs, measures non-sync gaps, decodes sectors, and flags anomalies.

### Verified
- Arctic Fox preservation G64: T2 0 sync/0 sectors; T36 469 sync runs/0 sectors; T37–40 sync/no
  sectors.

## [Unreleased] — TGREAD tail-gap port and NBZ decoding

### Added
- `TailGap.cs`, `Assets/TailGap.s/.bin`, and `--cbm tailgap`.
- FDC JUMP job `$D0`; five gap bytes read from `$1C01` into `$0300-$0304`, status at `$0305`.
- `Nbz.cs` LZ77 decompressor; `.nbz` supported by `--convert` (10 Kracker Jax disks decoded).
- `docs/drive-assembly/` with disassembly, probes, and captured samples.

### Verified
- Hardware tail-gap bytes `60 24 46 23 99`; drive status `00, ok,00,00`.

## [Unreleased] — Drive-side raw GCR sampler

### Added
- `--cbm gcrscan`, `Core/GcrScan.cs`, `Core/Assets/GcrSampler.s`/`gcrsampler.bin`.
- 256 bytes from `$1C01` into `$0400-$04FF`; status/phase at `$0300/$0301`; never moves the head.

### Notes
- Stepper experiments from uploaded code caused head clatter and were removed. Use FDC SEEK for
  whole-track movement.

## [Unreleased] — VICE OpenCBM diagnosis, patch, and fast-loader finding

### Added
- `patches/vice-3.10-opencbm-filename.patch` and `tools/build-patched-vice.sh`.
- Fix: buffer filename bytes written after `OPEN` and pass them to `cbm_open()` at `UNLISTEN`.
- `docs/drive-assembly/c-probes/` C programs used to isolate the OpenCBM behavior.

### Notes
- Custom fast loaders remain unsupported by the high-level backend: they upload 1,280 bytes to
  drive RAM `$0300-$07FF` and bit-bang VIA1 at `$1800`.

## [Unreleased] — VICE real-drive launch builder and hardware verification

### Added
- `--cbm vice <command|start|stop>` with `--dev`, `--data`, `--emu`, `--40col`, `--sound`,
  `--log`, `--softgl`, `--dry-run`; PID saved in `/tmp/d64editor-vice.pid`.
- `--cbm boot <image> <dev>` legacy alias.
- `HARDWARE_WORKFLOW.md` with the complete verified real-media process.

### Verified
- Bootable CP/M image built from a freed donor; D64 and D71 written with warp/no-`-t`, byte-exact
  readback, and booted from the physical 1571 through VICE passthrough.
- Xwayland GTK3 workaround: software GL, retry, `spectacle -b -o ...`, `xdotool`.

## [Unreleased] — CP/M fully in-process

### Changed
- All default `--cpm-*` verbs and `cpm*` script commands use `C128Cpm`; external `ctools` and
  `cpmtools` are explicit opt-ins only.
- `C128Cpm.WriteFiles` collects entries only from the real directory area, fixing false positives
  such as `BNKBDOS3.SPR` inside `CPM.SYS`.

## [Unreleased] — CP/M D71 mapping, strict extraction, host import

### Added
- D71 speed-zone fix (`TsOffset1541`) for T36–70; files past block ~88 read correctly.
- `ExtractFilesStrict` and `--strict`; donor import directory-sector heuristic.
- `HostNameToCpm83`, `ReadHostDirectory`, `ImportDirectoryFromBootDonor`, `CpmImportReport`.

### Verified
- 36-file import byte-exact; 4-file and 2-file host-directory imports byte-exact modulo 128-byte
  padding.

## [Unreleased] — Emulator boot verification and bootable donor path

### Added
- `--cpm-bootable` / `cpmbootable` using a donor system disk.
- Verified system-disk boot; `--cpm-new` data-only images black-screen as expected.

## [Unreleased] — CP/M D64 directory layout aligned to real disk

### Changed
- Real interleaved track-1 sectors `(3,4,9,10,14,15,19,20)`, eight entries/sector, bitmap at
  sector 2. Byte-exact round-trips; bootability of from-scratch images remains unverified.

## [Unreleased] — Phase 2 completion

### Added
- GCR write-back (D64→G64/NIB byte-exact 683/683), CMD container write-back, CP/M from-scratch
  creation (`FormatBlank`, `WriteFiles`, `FormatAndWrite`, `--cpm-new`), `--extract-archive`.

## [Unreleased] — Headless scripting engine

### Added
- `ScriptEngine`, `--run-script`, `ScriptResult`, sample scripts, `D64Image.RenameDisk`.

### Fixed
- `ReadBam` now computes `BlocksFree` from BAM rather than stale state.

## Recent implementation rounds

- **Round 12:** CMD container write-back for all partition types.
- **Round 11:** GEOS VLIR record-chain handling and CLI completeness.
- **Round 10:** Undo/redo, graphics viewers, `CreateRelWindow`, `--cpm-format`.
- **Round 9:** GEOS info, `--extract-archive`, `--print-dir`, update check.
- **Round 8:** `RelWrite`, REL resize, mutable T64, G64/NIB info windows.
- **Round 7:** File-chain and directory display polish.
- **Round 6:** Emulator quoting, CP/M font, `C128Cpm`, `--cpm-extract`/`--cpm-copy`.
- **Earlier rounds:** pre-2026-11-15 initial port and feature work.

## Known limitations (current)

- G64/NIB fidelity tuning, half-track editing, and copy-protected re-encoding remain incremental.
- CMD in-container editing is incremental; write-back works.
- From-scratch CP/M is data-only; `.d81` in-process only and unverified on real 1581.
- Full-screen CP/M 2.2 programs may render poorly under stock CP/M 3.0.
- Inter-tab disk-to-disk drag and comprehensive undo/redo are incomplete.
- Native print dialog, updater, and installer are not implemented.
- Deeper GEOS/VLIR workflows are best-effort.
- `--gcrread --whole` is not a valid single-revolution capture; SRQ nibtools is authoritative.
- `--writeraw` hardware verification covers only void track 2.

## Verification status

- Synthetic/generated-image verification across all supported readers; no automated test suite.
- CP/M extraction verified byte-exact against ctools (D71 19 files, D64 10 files).
- WUMPUS and Zork II played end-to-end under x128 CP/M 3.0.
- 683/683-sector GCR write-back round-trip.
- Real-media D64/D71 CP/M writes byte-exact; physical-drive boot verified.
- Tail-gap bytes and Arctic Fox faithful capture boot verified on hardware.
- Unverified: CP/M `.d81` real media, raw-GCR hardware stitching, T36–40 raw writer stepping.
