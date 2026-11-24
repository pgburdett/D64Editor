# Patches for upstream tools

## `vice-3.10-opencbm-filename.patch`

### Problem

VICE 3.10's real-device backend opens CBM files with:

```c
cbm_open(fd, device, secondary, NULL, 0)
```

and then sends the filename as later raw IEC writes before `UNLISTEN`. With OpenCBM 0.4.99 and
the XUM1541/ZoomFloppy this opens no file at all. The subsequent `TALK` / `cbm_raw_read` /
`cbm_get_eoi` loop never receives EOI or data, so `LOAD` from a real drive hangs.

This was isolated with `gdb` and the small probes in `docs/drive-assembly/c-probes/`:

- Correct call `cbm_open(f,8,0,"$",1)` → `cbm_talk` → `cbm_raw_read` + `cbm_get_eoi` returns the
  128-byte directory with EOI at byte 127.
- Correct call with `"*"` reads a 337-byte PRG with EOI on the last byte.
- Replicating VICE's empty-name-then-raw-write sequence returns `n=0, eoi=1` immediately.

### Fix

`patches/vice-3.10-opencbm-filename.patch` changes `src/serial/realdevice.c` to buffer filename
bytes written after `OPEN` and pass them to `cbm_open()` at `UNLISTEN` instead of calling it with
`NULL, 0`.

### Build

```bash
tools/build-patched-vice.sh [source-dir] [install-prefix]
```

Defaults:

- source: `/tmp/opencode/vice-3.10`
- install prefix: `/tmp/opencode/vice-patched`

The script downloads the VICE 3.10 tarball if needed, applies the patch idempotently, configures
GTK3 + OpenCBM with sound disabled, and runs `make && make install`. It requires the `xa`
assembler for VICE's 6502 assets.

### Usage

```bash
LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe \
  /tmp/opencode/vice-patched/bin/x64sc \
    -model c64c -ntsc -power60 -VICIIaspectmode 0 \
    -windowwidth 1400 -windowheight 900 \
    -devicebackend8 2 -busdevice8 -sounddev dummy
```

The patched build is still experimental. Custom fast loaders are **not** fixed by this patch: they
upload 1,280 bytes to drive RAM `$0300-$07FF` and bit-bang VIA1 at `$1800`, bypassing the IEC
protocol layer that `cbm_open` operates on. Use `-drive8truedrive` with a faithful raw G64 for
protected games.

On this Xwayland session, VICE GTK3 startup is flaky. Use software GL, retry, capture with
`spectacle -b -o ...` (X11 root capture is black), and send keys with `xdotool`. See
`HARDWARE_WORKFLOW.md` and `tools/diagnostics/README.md`.
