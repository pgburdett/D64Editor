# Reference documents

This directory contains long-form developer references for D64Editor.

| Document | Contents |
|---|---|
| `CODE.md` | Full code map: Core/App separation, image formats, GCR/protection engines, hardware wrappers, embedded 6502 assets, UI windows, safety invariants. |
| `TOOLS.md` | CLI verb index, external utilities, reproducible capture/trace/archive workflows, safety rules. |
| `REVERSE-ENGINEERING.md` | Z80/CP/M analysis toolchain, BDOS conventions, verified CP/M findings, drive-code and protection discoveries. |
| `disk-images/README.md` | Reference disk images used by the copy-protection investigation and how to regenerate decoded D64s. |
| `disk-images/generated/README.md` | Generated/captured images, including the faithful SRQ nib capture and the EA disk in drive 8. |
| `drive-assembly/README.md` | Detailed 6502/VIA reference, FDC job queue, safe GCR sampling, TGREAD port, fastloader traces, protected-track patterns, raw writer/reader, hardware verification, RUSSIA `GM` G64 variant matrix and in-VICE U1 probe recipe. |
| `drive-assembly/samples/README.md` | Captured raw samples: protected tracks, VIA2 GCR chunks, tail-gap bytes, drive-code fragments. |
| `drive-assembly/c-probes/README.md` | Small OpenCBM C probes used to isolate the VICE real-device OPEN bug and test raw-track APIs. |
| `inside-commodore-dos.txt` | Plain-text extraction of *Inside Commodore DOS* by Immers & Neufeld (1984), kept only as a local development reference. Copyright remains with the authors/publisher. |

These documents are reference material, not source of truth for executable behavior. When prose
and code/config disagree, prefer the code/config.
