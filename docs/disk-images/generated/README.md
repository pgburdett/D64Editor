# Generated diagnostic images

| File | How it was made |
|---|---|
| `protected-repro.g64` | `--protect docs/disk-images/af-g64.d64 protected-repro.g64` — a G64 with synthesized protected tracks: void track 2 and generated tracks 36-40. `--cbm gaps` on it reports the same anomaly signature as the original Arctic Fox preservation image. |
| `ea-disk-in-drive.g64` | Fresh SRQ raw-GCR capture from the EA disk in drive 8, converted with `nibconv`. This is a different mastering/revision from the archived Arctic Fox image; see the section below. |

| `physical-read.d64` | `d64copy --no-progress 8 physical-read.d64` from the blank test disk with the reproduced no-sync track 2. d64copy reports `read error: 02/00: 3` and gives up on track 2. |
| `physical-read.g64` | `--to-gcr physical-read.d64 physical-read.g64` — plain D64→G64; regenerates standard GCR, so track 2 is **not** preserved (21/21 sectors). |
| `physical-repro.g64` | `--protect physical-read.d64 physical-repro.g64` — injects the reproduced void track 2 (and extended tracks 36-40) back into the G64; `--cbm gaps` shows T2 with 0 sync/0 sectors, matching the physical disk. |

## EA disk in drive 8 (distinct from the Arctic Fox preservation image)

| File | How it was made / result |
|---|---|
| `ea-disk-in-drive.g64` | `nibread -@xum1541 -D8 -e5 -h -v /tmp/opencode/disk-in-drive.nib` from the physical disk in drive 8, then `nibconv /tmp/opencode/disk-in-drive.nib /tmp/opencode/disk-in-drive.g64`. SHA-256 `b50be3d482be5494434d6f3eae4cb81d95ce6548e2d15de9b9152ccef1890105`. |

The inserted disk identifies itself as `"electronic arts"` and contains `ea`, `loader`, `bldr`,
`game`, `data`, `info`, and `l00t`–`l39t`. It is **not** the same mastering as the archived
Arctic Fox image: track 2 is normal (7,787 bytes, 42 sync runs, 21/21 sectors) instead of void.
The protection is in the extended region:

```text
T35: 12/17 sectors, 2151-byte non-sync run
T36: 0 sync runs, 0 sectors
T37: 0 sync runs, 0 sectors
T38: 125 sync runs, 0 sectors
T39: 24 sync runs, 0 sectors
T40: 118 sync runs, 0 sectors
T41: 32 sync runs, 0 sectors
```

A direct FDC surface scan reported 785 sectors: 489 clean, 122 FDC errors, 174 suspicious.
`d64copy` fails with data-checksum/no-sync errors on scattered sectors (`01/05`, `23/00`,
`23/01`), confirming weak/marginal sectors on otherwise readable tracks.

**Boot test:** attached with `x64sc -drive8truedrive -8 ea-disk-in-drive.g64`, `LOAD"*",8,1`
and `RUN` upload the 1,280-byte drive program (`M-W` through `$06E0`, `M-E $0300`) and populate
screen RAM with a non-BASIC screen. This image works in VICE with true drive emulation.

## G64 captures from the real Arctic Fox disk

| File | How it was made / result |
|---|---|
| `arcticfox-capture.d64` | `d64copy 8 ...` from the **original EA disk**. Complete except 5 bad sectors on track 2 (`read error 02/xx: 3/4/5`, then "giving up"). |
| `arcticfox-capture.g64` | `--to-gcr` of that D64. Plain regeneration: T2 becomes 21/21 readable — protection lost. Booting it in VICE hangs on the track-2 check. |
| `arcticfox-capture-protected.g64` | `--protect` applied to the captured D64: void track 2 + extended tracks 36-40 restored (`--cbm gaps`: T2 0 sync/0 sectors). Booting in VICE gets past the track-2 check but hangs in the fastloader's red-stripe loading screen — the D64 intermediate destroyed authentic raw GCR gaps/timing on the other tracks. |
| `arcticfox-from-g64.d64` | D64 regenerated from `arcticfox-capture.g64` (round-trip, ready to write to a blank disk). |

**Conclusion:** a faithful G64 must come from a raw GCR capture. A sector-level D64→G64
conversion loses the physical timing information the EA fastloader needs, so injecting only the
void track 2 is not sufficient. The successful raw capture is below.

The `physical-*` images above came from the blank test disk formatted `KK` with a reproduced
void track 2 — not from the EA game media.

## Faithful SRQ nib capture from the real EA disk (successful)

| File | How it was made |
|---|---|
| `arcticfox.nib` | `nibread -@xum1541 -D8 -e5 -h -v arcticfox.nib` from the original EA disk in drive 8 (1571 / ZoomFloppy XUM1541, default SRQ serial nibbling mode). 657 KB, 82 tracks. |
| `arcticfox-srq.g64` | `nibconv arcticfox.nib arcticfox-srq.g64`. Preserves T2 as 6,666 bytes with **0 sync runs / 0 sectors**, and T36 as 6,355 bytes with no decodable sectors. |

nibread track log highlights:
- T35: `(0) 6204 [CBM OK] (weakgcr:1)` — normal.
- T35.5: `[NDOS]` with weak GCR — half-track protection data present.
- T36: `weakgcr:5188` / `weakgcr:6107`, repeated "Long Read" — the PRODOS protection track.
- T37–41.5: `NOSYNC! [Unformatted Track]` — intentional killer/unformatted region.

**Boot test:** attached to VICE x64sc with `-drive8truedrive`, `LOAD"*",8,1` loads **Arctic Fox**
past the EA splash into its **"Enemy Review" tank-selection screen**. This is the first faithful
image made from the physical disk, and it proves SRQ nibbling works on this 1571/XUM1541 setup.

The source images and reproduction commands are documented in the parent
[`README.md`](../README.md) and in `../../drive-assembly/README.md` §11.
