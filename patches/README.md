# Patches for upstream tools

## `vice-3.10-opencbm-filename.patch`

VICE 3.10's real-device backend calls `cbm_open(..., NULL, 0)` and sends the C64 filename
as later raw IEC writes. With OpenCBM 0.4.99 / XUM1541 this opens no file, so LOAD from a real
drive loops forever in `cbm_raw_read`/`cbm_get_eoi` (verified with gdb traces; see
`docs/drive-assembly/c-probes/README.md`).

The patch changes `src/serial/realdevice.c` to buffer the filename bytes written after OPEN
and pass them to `cbm_open()` at UNLISTEN.

Build the patched emulator with:

```bash
tools/build-patched-vice.sh [source-dir] [install-prefix]
```

Defaults: source `/tmp/opencode/vice-3.10`, install prefix `/tmp/opencode/vice-patched`.
The script downloads the VICE 3.10 tarball if needed, applies the patch idempotently,
configures GTK3 + OpenCBM with sound disabled, and runs `make && make install`.

Use the resulting binary for real-drive experiments:

```bash
LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe \
  /tmp/opencode/vice-patched/bin/x64sc \
  -model c64c -ntsc -power60 -VICIIaspectmode 0 \
  -windowwidth 1400 -windowheight 900 \
  -devicebackend8 2 -busdevice8 -sounddev dummy
```

The separate fast-loader incompatibility (custom drive code uploaded with M-W/M-E and
bit-banging VIA1) is NOT fixed by this patch — it is a console/drive-protocol limitation.
