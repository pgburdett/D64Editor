# Arctic Fox / EA PRODOS — Preservation Artifacts and Protection Analysis

This directory contains Arctic Fox (Electronic Arts / Dynamix, 1986) preservation artifacts,
EA/PRODOS reproduction experiments, and a technical analysis of the protection. The faithful
preservation master for Arctic Fox is the raw SRQ NIB capture; D64 and regenerated G64 copies
lose protection.

## Directory layout

| Path | Description |
|---|---|
| `arcticfox[ea_1986](!).g64` | Archived Arctic Fox preservation G64 (reference). |
| `arcticfox[ea_1986](!).nbz` | Archived NBZ preservation image. |
| `arcticfox.d64` | Sector-level D64 representation (not protection-faithful). |
| `af-g64.d64` | D64 derived for experiments/protection reproduction. |
| `generated/` | Captures and reproduction experiments made during investigation. See `generated/README.md`. |

## Key generated artifacts

| File | Description |
|---|---|
| `generated/arcticfox.nib` | **Faithful SRQ nibread capture** from the original EA Arctic Fox disk (1571/XUM1541). 657 KB, 82 tracks. |
| `generated/arcticfox-srq.g64` | nibconv G64 from the faithful NIB. Boots in VICE `x64sc -drive8truedrive` to the "Enemy Review" tank-selection screen. |
| `generated/arcticfox-capture.d64` | `d64copy` from original disk. Complete except five bad sectors on track 2; protection lost. |
| `generated/arcticfox-capture.g64` | Regenerated from captured D64; T2 becomes readable (21/21 sectors), protection lost. |
| `generated/arcticfox-capture-protected.g64` | `--protect` applied to captured D64; restores void T2 and extended T36–40, but still not fully faithful because D64 intermediate lost raw gaps/timing. |
| `generated/protected-repro.g64` | Synthesized EA PRODOS tracks from `af-g64.d64` via `--protect`; reproduces gap/anomaly signature. |
| `generated/physical-*` | Blank-disk reproduction experiments (not EA game media). |
| `generated/ea-disk-in-drive.g64` | Different EA disk/revision (PirateBusters boot v2.0 / TSDS), not the same mastering as Arctic Fox preservation image. Preserved for analysis. |

## Faithful capture workflow (verified)

The only faithful path is raw GCR capture:

```bash
# Read-only capture from real 1541/1571 in 1541 mode
cbmctrl reset
nibread -@xum1541 -D8 -e5 -h -v arcticfox.nib

# Convert for emulator use
nibconv arcticfox.nib arcticfox-srq.g64

# Boot test (VICE)
x64sc -drive8truedrive -8 arcticfox-srq.g64
# Then: LOAD"*",8,1
```

**Result:** Arctic Fox boots past the EA splash into the "Enemy Review" tank-selection screen.

Why this works: nibread SRQ captures raw GCR track streams, including non-standard sync/gap/weak
patterns. nibconv preserves the G64 representation needed by VICE true drive emulation.

Why D64 fails: D64 stores decoded sector data only. It cannot represent void tracks, non-standard
gaps, weak bits, sync length, half tracks, or rotation/timing-sensitive layout.

## Protection summary (deep logic)

Arctic Fox uses **EA PRODOS**, Electronic Arts' early proprietary C64 disk protection. It is an
ancestor of PirateSlayer-style protection. The protection is not a single bad sector check; it is
a combination of non-standard physical track layout, custom drive code, raw GCR checks, and
timing-sensitive behavior.

### Confirmed by traces/hardware observations

- Uploaded drive code executes at `$0300-$07FF` in the 1541/1571 drive RAM.
- Raw GCR reads come from VIA2 `$1C01` using byte-ready handshaking (`BVC`/`CLV` polling IFR CA1).
- Track 2 is a void/sync-less protection track on the faithful image (0 decoded sectors).
- Track 36 contains non-standard/weak GCR protection data (long reads, heavy weak GCR in nibread).
- Tracks 37–41.5 are unformatted/NOSYNC killer regions on the faithful capture.
- D64 → G64 regeneration loses protection and fails the EA loader.
- Injecting only void T2 (via `--protect`) is insufficient; other raw/timing/gap checks remain.

### Not directly measured / inferred

- Exact rotation/track-length timing threshold in the fastloader delay loop (`$0520-$0525`) is
  consistent with timing checks, but no isolation test has conclusively measured the threshold.
- Some behavior is inferred from EA PRODOS/PirateSlayer family characteristics and from observed
  loader behavior.

## Protection layers

### 1. Non-standard physical tracks and gaps

Protected tracks replace normal CBM DOS sector framing with custom raw GCR patterns. Instead of
normal `$55` filler gaps and standard sector headers/data blocks, protection tracks may contain:

- Sync-less streams (`$FF` sync marks absent where DOS expects them).
- Altered gap lengths and custom framing.
- Long sync runs / weak-bit regions.
- Unformatted/killer regions that confuse naive nibblers.

The faithful Arctic Fox NIB shows:

```text
T35.0: CBM OK (normal)
T35.5: NDOS with weak GCR (half-track protection data)
T36.0: NDOS, heavy weak GCR, long reads (PRODOS protection track)
T37-41.5: NOSYNC / Unformatted Track (killer region)
```

`nibconv` preserves T2 as 6,666 bytes with 0 sync runs / 0 sectors and T36 as 6,355 bytes with
no decodable sectors.

### 2. Custom drive code uploaded to 1541/1571 RAM

The loader does not rely on normal DOS sector reads for protection checks. It uploads 6502 code
into drive RAM (`$0300-$07FF`) and executes it via DOS `M-W`/`M-E` commands. The drive code
bypasses ROM disk routines and reads raw GCR directly from the hardware.

Observed safe/hardware-relevant mechanism:

- VIA2 `$1C01` is the raw GCR data port.
- VIA2 `$1C00` bit 7 is SYNC (0 = sync found); bit 2 controls motor.
- VIA2 `$1C0C` PCR must be set for read mode/BYTE-READY (`$EF` in the safe sampler: CA2 high for
  SOE/BYTE-READY, CB2 high for read mode).
- VIA2 `$1C0D` IFR bit 1 is CA1/BYTE-READY; cleared by writing `$02`.
- Byte-ready read loop uses `BVC`/`CLV`/`LDA $1C01` (SO pin on 6502).

This is why M-E to ROM disk routines is unsafe: DOS ROM I/O routines are FDC-job driven. Raw VIA
sampling and FDC job-queue commands are the safe paths.

### 3. Rotation/timing/gap checks

EA PRODOS/PirateSlayer-style protection commonly verifies physical rotation timing. The drive runs
at ~300 RPM, but speed variation exists. A sector copy can have correct bytes but wrong physical
placement/timing. Tail-gap and inter-block gap spacing are especially sensitive.

Evidence in this investigation:

- Tail-gap capture from a real drive gave `60 24 46 23 99` (5 bytes) via TGREAD-style FDC JUMP job.
- The fastloader contains a delay/handshake loop at `$0520-$0525`, consistent with timing checks.
- D64→G64 regeneration recreates normal GCR layout and loses timing; boot hangs on protection check.
- `--protect` restores void T2 and extended tracks, but boot still hangs later in loading screen,
  indicating additional raw/timing/gap checks beyond T2.

### 4. Killer/unformatted regions

Tracks 37–41.5 are NOSYNC/unformatted in the faithful capture. These regions punish naive
track-boundary detection and bit-copiers that assume every track has recoverable sectors/sync.

## Why sector copies fail

| Method | Result |
|---|---|
| `d64copy` from original | Fails on T2 bad sectors; creates incomplete/compromised D64. |
| D64→G64 regeneration | T2 becomes normal readable sectors; protection lost. |
| D64 + `--protect` void T2/T36–40 | Reproduces anomaly signature, but not faithful raw timing/gaps; boot still fails later. |
| nibread SRQ NIB → nibconv G64 | Faithful enough for VICE true-drive boot to Enemy Review screen. |

## EA disk in drive 8 (different revision)

`generated/ea-disk-in-drive.g64` is a different EA disk/mastering, not the same as Arctic Fox:

- Disk name: `electronic arts`
- Files include `EA`, `LOADER`, `BLDR`, `GAME`, `DATA`, `INFO`, `L00T`-`L39T`
- Loader identifies as `PIRATEBUSTERS BOOT VERSION 2.0` by Kris Hatlelid and Kevin Pickell with TSDS.
- Track anomalies differ: T2 normal (21/21 sectors), T35 partial (12/17), T36–37 no sync/sectors,
  T38–41 sync/no sectors. Direct FDC scan: 785 sectors, 489 clean, 122 FDC errors, 174 suspicious.
- Under VICE true drive, LOAD/RUN uploads drive program (M-W through $06E0, M-E $0300) and
  populates screen RAM with a non-BASIC screen. Full completion remains under investigation.

## Reproduction with D64Editor

D64Editor can synthesize EA PRODOS-style anomaly signatures for testing:

```bash
D64Editor --protect docs/disk-images/arcticfox/af-g64.d64 /tmp/protected-repro.g64
D64Editor --cbm gaps /tmp/protected-repro.g64
```

Expected signature: T2 0 sync/0 sectors, T36 468/0 sync/sectors (generated), T37–40 sync/no sectors.
This is useful for detector/tooling tests, **not** as a substitute for a faithful capture.

## Safety and handling

- Never run `--writeraw` on an original disk. Use blank media only.
- Real-drive reads are non-destructive when using nibread/d64copy/cbmctrl read paths, but bus
  ownership rules apply: stop VICE before OpenCBM tools; `cbmctrl reset` after emulator use.
- Do not upload stepper-homing experiments. Only FDC job-queue SEEK / safe step routines are allowed.
- For protection-faithful playback, prefer NIB master and SRQ-derived G64 tested in VICE true drive.

## References

- `docs/drive-assembly/README.md` — VIA/FDC details, GCR sampling, tail-gap, fastloader traces.
- `docs/disk-images/arcticfox/generated/README.md` — per-artifact capture notes.
- `D64Editor.Core/ProtectedTracks.cs` — synthetic EA PRODOS track generation.
- `D64Editor.Core/GcrRead.cs`, `RawWriter.cs`, `TailGap.cs`, `GapScan.cs` — raw GCR tooling.
