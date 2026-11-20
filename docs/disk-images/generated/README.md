# Generated diagnostic images

| File | How it was made |
|---|---|
| `protected-repro.g64` | `--protect docs/disk-images/af-g64.d64 protected-repro.g64` — a G64 with synthesized protected tracks: void track 2 and generated tracks 36-40. `--cbm gaps` on it reports the same anomaly signature as the original Arctic Fox preservation image. |

| `physical-read.d64` | `d64copy --no-progress 8 physical-read.d64` from the blank test disk with the reproduced no-sync track 2. d64copy reports `read error: 02/00: 3` and gives up on track 2. |
| `physical-read.g64` | `--to-gcr physical-read.d64 physical-read.g64` — plain D64→G64; regenerates standard GCR, so track 2 is **not** preserved (21/21 sectors). |
| `physical-repro.g64` | `--protect physical-read.d64 physical-repro.g64` — injects the reproduced void track 2 (and extended tracks 36-40) back into the G64; `--cbm gaps` shows T2 with 0 sync/0 sectors, matching the physical disk. |

Important: the physical disk imaged here is the **blank test disk** that was formatted `KK` and
then had the reproduced track 2 written to it — it is not the original EA game media. A faithful
raw capture of the original copy-protected tracks is not possible with XUM1541/OpenCBM (no raw
track read API).

The source images and reproduction commands are documented in the parent
[`README.md`](../README.md) and in `../../drive-assembly/README.md` §11.
