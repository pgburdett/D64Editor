# Patched OpenCBM xum1541 plugin (keyutils preload fix)

This directory contains a patched `libopencbm-xum1541.so` that removes the noisy
`/usr/lib/libudev.so.1: undefined symbol: keyctl` warning seen on some Linux systems when
OpenCBM/nibtools load the xum1541 USB backend.

## Problem

On affected systems (observed with `systemd-libs 262.2-1`, OpenCBM 0.4.99.104, xum1541), running
`cbmctrl`, `nibread`, or `nibwrite` prints:

```text
/usr/lib/libudev.so.1: undefined symbol: keyctl
```

The command still works; the warning is harmless but noisy.

`LD_DEBUG=symbols` shows the xum1541/libusb/libudev load path triggers runtime lookups for
`add_key` and `keyctl` (libkeyutils symbols). `libudev.so.1` is not linked against
`libkeyutils.so.1`, so the symbols are unresolved unless libkeyutils is already loaded.

## Fix

`patches/opencbm-xum1541-keyutils-preload.patch` adds a Linux-only constructor in
`opencbm/lib/plugin/xum1541/xum1541.c`:

```c
__attribute__((constructor))
static void xum1541_preload_keyutils(void)
{
    (void) dlopen("libkeyutils.so.1", RTLD_NOW | RTLD_GLOBAL);
}
```

It also adds `-ldl` to the plugin Linux Makefile. The dlopen is non-fatal; if libkeyutils is not
present, behavior is unchanged.

## Files

| File | Purpose |
|---|---|
| `libopencbm-xum1541.so.0.4.99.104` | Compiled patched plugin (x86_64, built from OpenCBM 0.4.99.104 source). |
| `../../patches/opencbm-xum1541-keyutils-preload.patch` | Source patch. |
| `../../tools/build-patched-opencbm-xum1541.sh` | Rebuild script. |

## Build

```bash
tools/build-patched-opencbm-xum1541.sh [source-dir] [out-dir]
```

Defaults: source `/tmp/opencode/OpenCBM`, output `tools/opencbm`.

## Install (permanent proper fix on this machine)

Back up and replace the installed plugin:

```bash
sudo cp /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104 \
        /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104.bak
sudo cp tools/opencbm/libopencbm-xum1541.so.0.4.99.104 \
        /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104
```

Verify:

```bash
cbmctrl detect
```

Expected: no `/usr/lib/libudev.so.1: undefined symbol: keyctl` line.

## Rollback

```bash
sudo cp /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104.bak \
        /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104
```

## Notes

- This is a workaround for a systemd-libs/libudev + libkeyutils runtime linking/lookup issue
  exposed through OpenCBM. A distro package fix in `systemd-libs`/`libkeyutils` is also appropriate.
- The compiled `.so` targets the same OpenCBM version as the installed package (0.4.99.104).
  Rebuild if your OpenCBM version differs.
- No hardware behavior changes; only the startup warning is suppressed.
