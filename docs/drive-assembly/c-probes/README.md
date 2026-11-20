# OpenCBM C probes

Small standalone C programs used to test the host-side OpenCBM/libopencbm paths while
diagnosing VICE's real-device LOAD hang and the XUM1541's raw GCR capabilities.

Build (Arch, `opencbm` installed):

```bash
gcc -O0 -g <file>.c -lopencbm -o <file>
./<file> [args]
```

| File | Purpose | Result |
|------|---------|--------|
| `eoi_test.c` | `cbm_open(8,0,"$",1)` → `cbm_talk` → `raw_read` + `get_eoi`. Proves EOI and byte reads work through the XUM1541 when the filename is passed correctly. | Directory returned 128 bytes, EOI at byte 127. A PRG (`*`) returned 337 bytes with EOI on the last byte. |
| `eoi_test2.c` | Reproduces VICE 3.10's sequence: `cbm_open(...,"",0)`, filename as later `raw_write`, `UNLISTEN`, `TALK`, read loop. | Returns `n=0, eoi=1` immediately — no data. This pinned the VICE bug on the OPEN filename. |
| `gcr_probe.c` | Calls `cbm_parallel_burst_read_track()` and `cbm_srq_burst_read_track()`. | Both return `-1` with `LIBUSB_ERROR_NO_DEVICE`. |
| `gcr_probe2.c` | Same burst calls with bus setup (`TALK`, `LISTEN`, `B-R:0`). | All variants fail. Raw track/GCR reads need an XP1541/XP1571 parallel cable, not just a ZoomFloppy. |

These are diagnostic programs only — they are not built by the .NET solution.
