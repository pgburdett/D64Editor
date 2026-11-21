# Code map — disk/format engine and hardware tooling

This document lists the code added for the copy-protection investigation and how it fits into
the existing Core/App split. Product behaviour stays in `D64Editor.Core`; CLI dispatch stays in
`D64Editor.App/Cli.cs`.

## Core: image formats and GCR
| File | Role |
|---|---|
| `G64Image.cs` | G64 container parse/write. `RawTrackOverrides` lets callers inject raw GCR tracks before `ToG64Bytes(d64)` rebuilds the container. Half-track and out-of-range slots are preserved verbatim. |
| `NibImage.cs` | MNIB-1541-RAW NIB parse/encode; `ToD64Bytes()` decodes raw GCR tracks to sectors. |
| `Gcr.cs` | 4-to-5 GCR encode/decode, track encoding, sector decoding (`DecodeTrackSectors`). |
| `Nbz.cs` | LZ77 decompressor for `.nbz` (marker byte, literal/marker-escape, varint length+offset, overlapping copy). Output is a standard NIB image. |
| `GapScan.cs` | Raw GCR analysis: counts `$FF` sync runs, measures non-sync gaps, decodes sectors, flags anomalies against expected sector count. |
| `RawG64.cs` | Wraps captured raw track byte arrays into a G64 container using a blank D64 base. |

## Core: hardware / drive-side engines
| File | Role |
|---|---|
| `OpenCbm.cs` | Process wrapper around `cbmctrl` and `d64copy` using `ProcessStartInfo.ArgumentList` (no shell). Adds `UploadMemory`, `DownloadMemory`, `Execute` (M-E) for drive RAM work. Filters harmless libudev `keyctl` warning. |
| `SectorRead.cs` | FDC job-queue operations: `Read(device, track, sector)` (job `$80`, track `$0006`, sector `$0007`, data `$0300`) and `Seek(device, track)` (job `$B0`). The drive firmware does head movement and GCR decode. Exposes FDC status codes. |
| `SurfaceScan.cs` | Loops every track/sector via FDC READ jobs, classifies `$02/$03/$04/$05/$09/$0B` errors and optional data heuristics (opt-in `--data-checks`). |
| `TailGap.cs` | Port of Kracker Jax `TGREAD`: installs JMP at `$0300`, queues JUMP job `$D0`, reads 5 bytes from `$1C01` into `$0300-$0304`, status at `$0305`. |
| `GcrScan.cs` | Non-destructive raw GCR sampler (current track only). Uploads a 6502 routine that waits for SYNC and BYTE-READY, stores 256 bytes at `$0400`, status/phase at `$0300/$0301`. Never writes stepper bits. |
| `ProtectedTracks.cs` | Generates EA PRODOS pattern tracks: `GenerateTrack2()` (6,666 bytes, no `$FF`), `GenerateExtendedTrack(36..40)` (sync/filler and GCR noise). `Apply()` injects them into a G64. |
| `RawWriter.cs` | DESTRUCTIVE serial raw track writer. Uploads pattern/count/mask to drive RAM, seeks the track via FDC, launches `Assets/RawWriter.s` with M-E, polls status. Tracks >35 use the short `Stepper.s` routine. |
| `GcrRead.cs` | Raw GCR chunk reader (read counterpart of RawWriter). `ReadChunk()` captures 256 bytes via BVC/SO byte-ready; `Stitch()` aligns overlapping windows with a 128-byte reverse-searched prefix (synthetically validated); `ReadTrack()` uses it. CLI `--test-stitch` validates against known G64 tracks. |
| `D64Image.cs` | Existing D64 engine; `CreateBlank(name,id).GetRawDataCopy()` supplies the base D64 used when building raw G64 containers. |

## Embedded 6502 assets (`D64Editor.Core/Assets/`)
All are embedded resources with logical names `D64Editor.Core.Assets.<file>`, declared in `D64Editor.Core.csproj`. Source `.s` files are kept beside them and copied into `docs/drive-assembly/` for reference.

| Binary | Source | Purpose |
|---|---|---|
| `CPM.SYS`, `BOOT.D71.bin` | (binary fixtures) | Earlier CP/M boot experiment; not used by current CP/M path. |
| `gcrsampler.bin` (126 B) | `GcrSampler.s` | Safe VIA2 raw sampler; no stepper writes. |
| `tailgap.bin` (87 B) | `TailGap.s` | TGREAD port; FDC JUMP-job gap reader. |
| `rawwriter.bin` (93 B) | `RawWriter.s` | VIA2 write-mode raw GCR writer (BVC/CLV/STA `$1C01`). |
| `stepper.bin` (82 B) | `Stepper.s` | Two VIA phase changes per track, inward from 35; no homing. |
| `gcrreader.bin` (79 B) | `GcrReader.s` | Raw GCR chunk reader with skip/count and SYNC wait; motor via `$F97E`. |

## App: CLI surface
`D64Editor.App/Cli.cs` dispatches top-level verbs and `--cbm` subverbs. New hardware verbs added:
`sector`, `seek`, `surface`, `tailgap`, `gcrscan`, `gaps`; new top-level verbs `--protect`,
`--writeraw`, `--gcrread`, `--raw2g64`. `--convert` handles `.nbz`. Help text lists every verb.

## Docs and reference data
| Path | Contents |
|---|---|
| `HARDWARE_WORKFLOW.md` | End-to-end hardware/workflow guide: OpenCBM setup, warp writes, VICE launch/stop, CP/M verification, SRQ nibbling, safety rules. |
| `docs/TOOLS.md` | This repo's tools/workflows index (CLI verbs, external utilities, reproducible commands). |
| `docs/CODE.md` | This code map. |
| `docs/drive-assembly/README.md` | Detailed 6502/VIA reference, FDC job queue, original TGREAD disassembly, fastloader traces, protected-track histograms, serial writer/reader, PRODOS context, hardware results. |
| `docs/drive-assembly/*.s/.asm/.bin` | Experimental assembly, disassemblies and captured raw samples. |
| `docs/disk-images/README.md`, `docs/disk-images/generated/README.md` | Reference/generated NIB/G64/D64/NBZ images and how they were made. |
| `docs/inside-commodore-dos.txt` | pdftotext extraction of Immers & Neufeld (copyright reference only). |
| `tools/diagnostics/README.md` | VICE launchers, binary-monitor trace clients, archive search scripts, SRQ capture script. |
| `patches/README.md`, `tools/build-patched-vice.sh` | VICE 3.10 OpenCBM filename fix and reproducible build. |

## Safety and invariants
- Core never depends on App.
- Only one process may own the XUM1541/IEC bus. Stop VICE before `cbmctrl`/`d64copy`/`nibread`.
- `--writeraw` is DESTRUCTIVE: blank disk only.
- No uploaded stepper homing: it caused head clatter and was removed. FDC SEEK moves the head safely.
- Motor-on for custom drive code: `$F97E` + spin-up delay; raw `ORA #$04` is insufficient.
- Faithful protected images require raw GCR (`nibread`/`nibconv` or preservation archive); D64→G64 loses protection.
