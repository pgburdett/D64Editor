# Diagnostic tools

Scripts used during the VICE/OpenCBM real-drive investigation. They are not built by the
.NET solution; they are kept for reproducibility. Paths default to `/tmp/opencode`, so edit the
constants at the top of each script for your system.

## VICE launchers (shell)

| Script | Purpose |
|---|---|
| `launch-x64sc-trace.sh` | Launch patched VICE with the real 1571 attached via OpenCBM (`-devicebackend8 2 -busdevice8`), software GL, XUM1541 debug, retrying the known GTK3 startup race. |
| `launch-x64sc-mon.sh` | Same, with the VICE text-monitor command playback file (`vice-mon-cmds.txt`) and monitor logging. |
| `launch-x64sc-binmon.sh` | Same, with the **binary monitor** enabled on `127.0.0.1:6502` for scripted checkpoint tracing. |
| `launch-x64sc-g64.sh` | Launch VICE with the Arctic Fox preservation **G64** image on emulated drive 8 (no real device), binary monitor enabled, small lower-right window. Uses `-drive8truedrive` so the 6502 in drive 8 is actually executed. Used for drive-CPU tracing of the loader. |
| `launch-x64sc-capture.sh` / `launch-x64sc-protected.sh` / `launch-x64sc-srq.sh` | Variants that attach the sector-captured G64, the protection-injected G64, or the successful SRQ-nib G64 respectively. All use `-drive8truedrive`. |
| `capture-srq.sh` | Combined SRQ nibbling workflow: `nibread -@xum1541 -D8 -e5 -h -v` then `nibconv` to G64, with a confirmation prompt before reading the real disk. |

All use:

```bash
export LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe
```

because VICE 3.10's GTK3 backend segfaults intermittently on this Xwayland session. Each
launcher retries up to 12 times and writes the winning PID to `/tmp/opencode/x64sc-*.pid`.

## Binary-monitor clients (Python)

These implement a minimal subset of VICE's binary monitor protocol (STX `0x02`, API version 2,
little-endian length/request ID, command byte, body).

| Script | What it traces |
|---|---|
| `binmon_trace.py` | Sets a checkpoint on **stores to `$DD00`** (C64 CIA2 IEC data port), then feeds `LOAD"*",8,1` via the monitor keyboard buffer. Logs the accumulator value on each hit to `/tmp/opencode/binmon-iec.log`. This traces the C64-side bit-banging during a real-drive LOAD. |
| `binmon_drive.py` | Sets a **stopping** checkpoint on execution in drive-8 RAM `$0300-$07FF`, then feeds the same LOAD command. On each hit it logs `PC/A/X/Y` to `/tmp/opencode/g64-drive-trace.log`. Accurate but ~14,000× slower — the loader only reaches its `$0520-$0525` delay loop. |
| `binmon_history.py` | **Preferred.** Feeds the LOAD command and then polls VICE's CPU-history API (command `0x86`) for drive 8 **without stopping execution**. Logs instruction bytes to `/tmp/opencode/g64-cpu-history.log`. This is fast enough that the protected loader completes and the game reaches its playable screen. Includes a PC filter (register ID 3) for `$0300-$07FF`. |
| `dump_drive_mem.py` | Feeds `LOAD"*",8,1`, waits for the `M-W` upload, then issues a binary-monitor MEM_GET for drive-8 RAM `$0300-$07FF` and writes the 1,280 uploaded bytes. |
| `launch-x64sc-g64-dump.sh` | Launches x64sc with a G64 image and the binary monitor, retries the flaky GTK3 startup, runs `dump_drive_mem.py`, and stops VICE. Unsets `GTK_MODULES`/`GTK_USE_PORTAL`/`WAYLAND_DISPLAY` and forces `GDK_BACKEND=x11`. |
| `verify_vice_boot.py` | Feeds `LOAD"*",8,1` and `RUN`, then MEM_GETs C64 screen RAM `$0400-$07E7` and decodes it, so a protected-disk boot can be confirmed without relying on a desktop screenshot. |
| `verify_vice_boot_long.py` | Longer-wait variant (35 s after LOAD, 25 s after RUN) for chained multi-stage loaders. |
| `verify_vice_boot_very_long.py` | Very-long-wait variant (120 s after LOAD, 120 s after RUN) for loaders that color-cycle a logo for several minutes. |
| `monitor_load_progress.py` | Samples screen RAM, color RAM `$D800-$DBFF`, and VIC registers `$D020-$D02F` during a long load, for boot loaders that display a color-cycling logo while loading. |
| `sample_progress_only.py` | Same sampling loop without keyboard feed, for use when keys are sent with `xdotool`. |

Both send command `0x12` (checkpoint set) and `0x72` (keyboard feed), then repeatedly send
`0xAA` (exit/continue) while collecting responses.

Other analysis helpers:

| Script | Purpose |
|---|---|
| `compare_halftracks.py` | Compares whole/half-track GCR blocks from a `nibread -h` NIB capture, allowing cyclic rotation, to test whether a protected track is duplicated at the adjacent half-track (wide/fat track). |
| `nib2g64_raw.py` | Converts a NIB capture to G64 while preserving exact raw track lengths and all half-track slots (unlike `nibconv`, it does not normalize long tracks). |
| `boot-test-g64.sh` | Launches x64sc with a G64 image, retries the GTK3 startup race, runs `verify_vice_boot_long.py`, and prints a screen-RAM summary. |

## Monitor command file

`vice-mon-cmds.txt` is the text-monitor playback file used with `-moncommands`:

```text
logname "/tmp/opencode/g64-mon.log"
log on
exit
```

Earlier variants used `trace store @io:$dd00`; text-monitor tracepoints did not reliably log
after `exit`, which is why the binary monitor was used instead.

## Archive search helpers

| Script | Purpose |
|---|---|
| `search_fat_tracks.sh` | Parallel `pdftotext` search over all PDFs in the Commodore collection for `fat track` variants (result: zero matches). |
| `scan_kj.sh` | Decodes every Kracker Jax `.nbz` via D64Editor `--convert`, exports all files, then greps `strings` output for gap/sync/track terms. Found `TGREAD` tail-gap reader and a header-gap `$0A` reference. |

## Typical drive-CPU trace session

```bash
tools/build-patched-vice.sh                 # if /tmp/opencode/vice-patched is missing
tools/diagnostics/launch-x64sc-g64.sh &     # emulated G64, binary monitor on :6502
python3 tools/diagnostics/binmon_drive.py   # trace drive code, 120 s
# inspect /tmp/opencode/g64-drive-trace.log
```

To dump the actually uploaded drive code from a G64 (no real drive needed):

```bash
tools/diagnostics/launch-x64sc-g64-dump.sh /tmp/opencode/real-disk.g64 \
    /tmp/opencode/drive-0300.bin
# or, with an already-running binary-monitor instance:
python3 tools/diagnostics/dump_drive_mem.py /tmp/opencode/drive-0300.bin
```

To verify that a G64 actually boots in VICE (with a binary-monitor instance running):

```bash
python3 tools/diagnostics/verify_vice_boot.py /tmp/opencode/vice-screen.bin
```

For a loader that color-cycles a hires logo while loading, sample progress over time:

```bash
python3 tools/diagnostics/monitor_load_progress.py 180
# or, when LOAD/RUN are sent with xdotool:
python3 tools/diagnostics/sample_progress_only.py 180
```

To test whether the disk uses wide/fat tracks from a direct `nibread -h` capture:

```bash
python3 tools/diagnostics/compare_halftracks.py \
    /tmp/opencode/real-disk.nib /tmp/opencode/nibread-real2.log
```

To build a G64 from a NIB capture without normalizing long protected tracks, then
boot-test it:

```bash
python3 tools/diagnostics/nib2g64_raw.py disk.nib nibread.log raw-preserve.g64
tools/diagnostics/boot-test-g64.sh raw-preserve.g64 raw
```

Remember: only one process may own the real XUM1541. Stop VICE before any `cbmctrl` or
`d64copy` command, and `cbmctrl reset` afterward.
