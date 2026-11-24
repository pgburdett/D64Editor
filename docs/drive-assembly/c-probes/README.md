# OpenCBM C probes

Small standalone C programs used to isolate host-side OpenCBM/libopencbm behavior while diagnosing
the VICE real-device LOAD hang and the XUM1541 raw-track API limitation. They are diagnostic
only and are not built by the .NET solution.

## Build

On Arch with `opencbm` installed:

```bash
gcc -O0 -g <file>.c -lopencbm -o <file>
./<file> [args]
```

## Probes

| File | Purpose | Result |
|---|---|---|
| `eoi_test.c` | `cbm_open(8,0,"$",1)` → `cbm_talk` → `cbm_raw_read` + `cbm_get_eoi`. Proves EOI and byte reads work through XUM1541 when the filename is passed correctly. | Directory returned 128 bytes, EOI at byte 127. PRG (`*`) returned 337 bytes with EOI on last byte. |
| `eoi_test2.c` | Reproduces the VICE 3.10 sequence: `cbm_open(...,"",0)`, filename as later `cbm_raw_write`, `UNLISTEN`, `TALK`, read loop. | `n=0, eoi=1` immediately — no data. This pinned the VICE OPEN filename bug. |
| `gcr_probe.c` | Calls `cbm_parallel_burst_read_track()` and `cbm_srq_burst_read_track()`. | Both returned `-1` with `LIBUSB_ERROR_NO_DEVICE`. |
| `gcr_probe2.c` | Same burst calls with explicit bus setup (`TALK`, `LISTEN`, `B-R:0`). | All variants failed. Raw track/GCR reads need XP1541/XP1571 parallel hardware, not just ZoomFloppy. |

These results motivate the patched VICE build (`patches/README.md`) and the reliance on SRQ
nibtools (`nibread`/`nibconv`) for faithful raw disk images.
