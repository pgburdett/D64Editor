# Captured raw samples

Small binary captures from the hardware/VICE investigation.

| File | Capture |
|---|---|
| `g64-T2.bin` | Raw track 2 (6,666 bytes) extracted from the Arctic Fox preservation G64. No `$FF` sync bytes. |
| `g64-T35.bin` | Raw track 35 (6,240 bytes) from the same G64, for GCR comparison. |
| `tgread-drive.bin` | 6502 drive code embedded in the Kracker Jax `TGREAD` BASIC loader (see `../tgread-original.bin`). |
| `af-drive-blob.bin` | Drive-code fragment near the `M-W`/`M-E` command table in Arctic Fox `LOADER`. |
| `repro-t2-gcr.bin`, `repro-t2-gcr2.bin` | 256-byte VIA2 samples of the reproduced no-sync track 2 on the real 1571. They differ between reads because there is no sync to lock byte alignment. |
| `gcr-track35-final.bin` | VIA2 sample of track 35 on the real drive. |
| `gcr-35half.bin` | VIA2 sample after one half-track phase step (35.5). |
| `gcr-back35.bin` | VIA2 sample after stepping back to track 35. |
| `tailgap-out.bin` | Five tail-gap bytes read by the ported `TGREAD` routine (`60 24 46 23 99`). |

See `../README.md` sections 8-13 for the analysis these samples support.
