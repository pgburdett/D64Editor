# Generated diagnostic images

| File | How it was made |
|---|---|
| `protected-repro.g64` | `--protect docs/disk-images/af-g64.d64 protected-repro.g64` — a G64 with synthesized protected tracks: void track 2 and generated tracks 36-40. `--cbm gaps` on it reports the same anomaly signature as the original Arctic Fox preservation image. |

| `physical-read.d64` | `d64copy --no-progress 8 physical-read.d64` from the blank test disk with the reproduced no-sync track 2. d64copy reports `read error: 02/00: 3` and gives up on track 2. |
| `physical-read.g64` | `--to-gcr physical-read.d64 physical-read.g64` — plain D64→G64; regenerates standard GCR, so track 2 is **not** preserved (21/21 sectors). |
| `physical-repro.g64` | `--protect physical-read.d64 physical-repro.g64` — injects the reproduced void track 2 (and extended tracks 36-40) back into the G64; `--cbm gaps` shows T2 with 0 sync/0 sectors, matching the physical disk. |

## G64 captures from the real Arctic Fox disk

| File | How it was made / result |
|---|---|
| `arcticfox-capture.d64` | `d64copy 8 ...` from the **original EA disk**. Complete except 5 bad sectors on track 2 (`read error 02/xx: 3/4/5`, then "giving up"). |
| `arcticfox-capture.g64` | `--to-gcr` of that D64. Plain regeneration: T2 becomes 21/21 readable — protection lost. Booting it in VICE hangs on the track-2 check. |
| `arcticfox-capture-protected.g64` | `--protect` applied to the captured D64: void track 2 + extended tracks 36-40 restored (`--cbm gaps`: T2 0 sync/0 sectors). Booting in VICE gets past the track-2 check but hangs in the fastloader's red-stripe loading screen — the D64 intermediate destroyed authentic raw GCR gaps/timing on the other tracks. |
| `arcticfox-from-g64.d64` | D64 regenerated from `arcticfox-capture.g64` (round-trip, ready to write to a blank disk). |

**Conclusion:** a faithful G64 must come from a raw GCR capture (nibbler with parallel cable or
the preservation archive image). A sector-level D64→G64 conversion loses the physical timing
information the EA fastloader needs, so injecting only the void track 2 is not sufficient.

The `physical-*` images above came from the blank test disk formatted `KK` with a reproduced
void track 2 — not from the EA game media.

The source images and reproduction commands are documented in the parent
[`README.md`](../README.md) and in `../../drive-assembly/README.md` §11.
