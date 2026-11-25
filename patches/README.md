# Patches for upstream tools

This directory contains patches for external tools used by D64Editor workflows. Patches are kept
small and reproducible; build scripts live in `tools/`.

## VICE 3.10 — OpenCBM real-device filename fix

- **Patch:** `vice-3.10-opencbm-filename.patch`
- **Build script:** `tools/build-patched-vice.sh`
- **Upstream base:** VICE 3.10 source tarball
- **Target component:** `src/serial/realdevice.c`

### Problem

VICE 3.10's real-device backend opens CBM files with:

```c
cbm_open(fd, device, secondary, NULL, 0)
```

and then sends the filename as later raw IEC writes before `UNLISTEN`. With OpenCBM 0.4.99 and
XUM1541/ZoomFloppy this opens no file at all. The subsequent `TALK` / `cbm_raw_read` /
`cbm_get_eoi` loop never receives EOI or data, so `LOAD` from a real drive hangs.

This was isolated with `gdb` and probes in `docs/drive-assembly/c-probes/`:

- Correct call `cbm_open(f,8,0,"$",1)` → `cbm_talk` → `cbm_raw_read` + `cbm_get_eoi` returns the
  128-byte directory with EOI at byte 127.
- Correct call with `"*"` reads a 337-byte PRG with EOI on the last byte.
- Replicating VICE's empty-name-then-raw-write sequence returns `n=0, eoi=1` immediately.

### Modified code / changelog

**File modified:** `src/serial/realdevice.c`

Changes included in the patch:

1. **Buffer filename bytes on OPEN write path.** Instead of discarding/assuming filename was passed
   to `cbm_open()`, the patch buffers bytes written after OPEN for the relevant device/secondary.
2. **Pass filename at UNLISTEN.** On UNLISTEN, the buffered filename is passed to
   `cbm_open(fd, device, secondary, name, len)` (or equivalent realdevice path), so OpenCBM receives
   the actual filename.
3. **Preserve existing real-device behavior for non-filename cases.** The fix is scoped to the
   realdevice OpenCBM backend path; it does not change true-drive emulation or non-OpenCBM backends.

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

### Limitations

- Experimental. The patched build fixes OpenCBM filename-open behavior for normal LOAD paths.
- Custom fast loaders are **not** fixed by this patch: they upload drive code with `M-W` and execute
  with `M-E`, bit-banging VIA at `$1800`, bypassing the IEC protocol layer that `cbm_open` operates on.
- For protected games, prefer `-drive8truedrive` with a faithful raw G64/NIB-derived image.
- On this Xwayland session, VICE GTK3 startup is flaky. Use software GL, retry, capture with
  `spectacle -b -o ...` (X11 root capture is black), and send keys with `xdotool`.

## OpenCBM 0.4.99.104 — xum1541 keyutils preload fix

- **Patch:** `opencbm-xum1541-keyutils-preload.patch`
- **Build script:** `tools/build-patched-opencbm-xum1541.sh`
- **Compiled artifact:** `tools/opencbm/libopencbm-xum1541.so.0.4.99.104`
- **Details:** `tools/opencbm/README.md`
- **Upstream base:** OpenCBM 0.4.99.104 / current git source
- **Target component:** `opencbm/lib/plugin/xum1541/xum1541.c`, `opencbm/lib/plugin/xum1541/LINUX/Makefile`

### Problem

On affected Linux systems, `cbmctrl`/`nibread`/`nibwrite` print:

```text
/usr/lib/libudev.so.1: undefined symbol: keyctl
```

`LD_DEBUG=symbols` shows `libudev.so.1` runtime lookups for `add_key`/`keyctl` fail because
libkeyutils is not loaded/linked in that path. The warning is harmless but noisy.

### Modified code / changelog

**Files modified:**

1. `opencbm/lib/plugin/xum1541/xum1541.c`
   - Added `#include <dlfcn.h>` under Linux.
   - Added a Linux-only constructor `xum1541_preload_keyutils()` that calls
     `dlopen("libkeyutils.so.1", RTLD_NOW | RTLD_GLOBAL)`; result ignored (non-fatal).
2. `opencbm/lib/plugin/xum1541/LINUX/Makefile`
   - Added `LIBS += -ldl`.

### Install

```bash
sudo cp /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104 \
        /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104.bak
sudo cp tools/opencbm/libopencbm-xum1541.so.0.4.99.104 \
        /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104
cbmctrl detect
```

Rollback by restoring `.bak`.
