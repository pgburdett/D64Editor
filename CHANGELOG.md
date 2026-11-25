# Changelog

All notable changes to D64Editor are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/).

## [Unreleased] — RUSSIA G64 fidelity investigation (documentation)

### Documentation
- Expanded `docs/drive-assembly/README.md` §13 with the G64 variant matrix, raw NIB-to-G64 wrapping
  results, and a tokenized in-VICE U1 probe recipe.
- Resolved the U1 probe syntax: `PRINT#15,"U1:..."` returns `30,SYNTAX ERROR,00,00` in VICE; the
  correct form is the U1 command as the OPEN 15 filename (`OPEN 15,8,15,"U1:5 0 18 18"`), matching
  `st.prg` and the 1541 DOS reference.
- Conclusive VICE probe (binary monitor, screen RAM read): protected G64 returns
  `23,READ ERROR,18,18` for T18/S18, so VICE sees the bad sector. The loader reset is not caused by
  a "fixed" checksum; remaining suspects are REVCNT/head-bump semantics, timing/rotation, or missing
  duplicate T18 headers (22 in NIB vs 19 in G64).
- `README.md`, `HARDWARE_WORKFLOW.md`, `AGENTS.md` and the roadmap document the corrected probe
  form and the binary-monitor result.
- Conclusion recorded: the NIB is the only faithful RUSSIA master; `nibconv` G64 derivatives are
  best-effort playable copies and `--raw2g64` cannot replace `nibconv` normalization.

## [Unreleased] — OpenCBM/VICE patch artifacts and keyutils warning fix

### Added
- `patches/opencbm-xum1541-keyutils-preload.patch` — Linux-only OpenCBM xum1541 plugin workaround: constructor dlopens `libkeyutils.so.1` (RTLD_NOW|RTLD_GLOBAL) to suppress `/usr/lib/libudev.so.1: undefined symbol: keyctl` runtime warning caused by libudev/keyutils lookup path; adds `-ldl`.
- `tools/build-patched-opencbm-xum1541.sh` — builds patched OpenCBM xum1541 plugin from source.
- `tools/opencbm/libopencbm-xum1541.so.0.4.99.104` — compiled patched plugin (x86_64, OpenCBM 0.4.99.104).
- `tools/opencbm/README.md` — problem analysis, build/install/rollback instructions.

### Changed
- `patches/README.md` — expanded into patch index with detailed VICE 3.10 realdevice filename-fix changelog (modified `src/serial/realdevice.c`: buffer OPEN filename bytes, pass at UNLISTEN) and OpenCBM xum1541 keyutils preload fix/changelog.

### Notes
- Permanent system install applied on this machine: backed up original plugin to `libopencbm-xum1541.so.0.4.99.104.bak` and installed patched `.so`; `cbmctrl detect` verified clean (no keyctl warning). Rollback by restoring `.bak`.

## [Unreleased] — RUSSIA write-back verification and protection artifact documentation

### Added
- `docs/disk-images/russia/` — RUSSIA (GM) preservation artifacts: `disk8-protected.nib` (NIB master), `disk8-protected.g64` (nibconv derivative), preserve/report/logs, and post-write nibread verification log.
- `docs/disk-images/russia/README.md` — detailed RUSSIA protection analysis: BASIC loader `RUS` M-W to REVCNT `$006A=$85` (head-bump disable), `ST` signature bypass at `$CF00`, U1 direct-channel check on T18/S18 expecting `23,READ ERROR,18,18`, success path loading `0:CD` to `$6B00`, why D64/G64 derivatives fail, and VICE U1 probe pitfalls.
- `docs/disk-images/arcticfox/README.md` — detailed Arctic Fox / EA PRODOS protection analysis: non-standard tracks/gaps, custom drive code uploading to `$0300-$07FF`, raw GCR reads via VIA2 `$1C01`/BYTE-READY, rotation/timing/gap checks, killer/unformatted regions, faithful SRQ capture workflow, and reproduction/safety notes.
- `docs/disk-images/README.md` — restructured as disk-image index pointing to `russia/` and `arcticfox/` subdirectories.

### Changed
- Moved Arctic Fox artifacts into `docs/disk-images/arcticfox/` and generated captures into `docs/disk-images/arcticfox/generated/`; updated path references in README, AGENTS, drive-assembly, and generated artifact docs.

### Verified
- Wrote `disk8-protected.nib` to a fresh disk in real 1571 (device 8, 1541 mode, `U0>M0`) with `nibwrite -@xum1541 -D8 ...`. Tracks 1.0–35.0 wrote normally; T36.0 wrote with weak bits; T37.0–41.0 unformatted. Post-write `nibread -@xum1541 -D8 -e5 -h -v` confirmed protection markers (T36 long reads/weak GCR, T37–41.5 unformatted/NOSYNC).

## [Unreleased] — 6502 disassembler (linear/tracing) and UI sector disassembly

### Added
- `D64Editor.Core/Disassembler.cs`: 6502/6510 disassembler with two modes.
  - **Linear** (default): PRG load-address aware, documented opcode table, `.byte` fallback for unknown opcodes.
  - **Tracing** (`--trace` / `trace: true`): control-flow tracing from PRG load address and optional extra entry points. Follows conditional branches (both paths), `JSR` (target + fallthrough), and `JMP abs`; stops at `RTS`/`RTI`/`BRK`/`JMP (ind)`. Emits unvisited gaps as `.byte` data with ASCII side comments.
  - Branch/JMP/JSR target labels (`LXXXX`), KERNAL symbol annotations for common `$FFxx` vectors and `$FCE2` RESET.
  - BIT skip-trick handling: 2- or 3-byte BIT instructions overlapping 1-byte instruction targets keep the BIT as code and emit the overlapped target as a labeled skip-trick note.
  - Optional extra trace roots via API (`extraEntryPoints`) and CLI/script `--entry $XXXX`.
- CLI: `--disasm <file.prg|image> [entry] [--trace] [--entry $XXXX]` disassembles a PRG file or a named file inside D64/T64 images.
- ScriptEngine: `disasm <name|host.prg> [--trace] [--entry $XXXX]`.
- `DisassemblyWindow` UI: Trace checkbox, Entry field, Mode selector (`File` / `Track/Sector` / `Block`), T/S/Block/Count inputs, Refresh. Raw sector/block modes read D64 blocks and disassemble with a `$0000` base header.
- `DiskDocumentView.ViewDisasm()` passes disk context for file disassembly and opens sector mode (`T18/S0`) when no file is selected but a disk is open.

### Changed
- Disassembly output now includes target labels and KERNAL comments in both linear and tracing modes.

### Known limitations
- Linear mode decodes data embedded in code; tracing mode only follows statically reachable flow.
- `JMP (ind)` targets require `--entry`.
- Self-modifying code is not modeled.
- Only 1-byte overlapped BIT-skip targets are specially handled; other overlaps degrade conservatively.

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
