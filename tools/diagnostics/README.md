# Diagnostic tools

These scripts support the VICE/OpenCBM real-drive investigation. They are **not** built by the .NET
solution. Most paths default to `/tmp/opencode`; edit the constants at the top of each script for
your system. Drive-code tracing requires the patched VICE from `patches/` and
`tools/build-patched-vice.sh`.

## Conventions

All launchers export:

```bash
export LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe
```

because VICE 3.10's GTK3 backend segfaults intermittently on this Xwayland session. Launchers
retry up to 12 times and write the winning PID to `/tmp/opencode/x64sc-*.pid`.

Only one process may own the real XUM1541. Stop VICE before `cbmctrl`, `d64copy`, or `nibread`,
and call `cbmctrl reset` (no device argument) afterward.

## VICE launchers (shell)

| Script | Purpose |
|---|---|
| `launch-x64sc-trace.sh` | Launch patched VICE with the real 1571 attached via OpenCBM (`-devicebackend8 2 -busdevice8`), software GL, XUM1541 debug; retries GTK3 startup. |
| `launch-x64sc-mon.sh` | Same, with text-monitor command playback (`vice-mon-cmds.txt`) and monitor logging. |
| `launch-x64sc-binmon.sh` | Same, with the binary monitor on `127.0.0.1:6502` for scripted checkpoint tracing. |
| `launch-x64sc-g64.sh` | VICE with the Arctic Fox preservation G64 on emulated drive 8 (no real device), binary monitor enabled, small lower-right window. Uses `-drive8truedrive` so drive-8 6502 executes. |
| `launch-x64sc-capture.sh` | Variant attaching the sector-captured G64. |
| `launch-x64sc-protected.sh` | Variant attaching the protection-injected G64. |
| `launch-x64sc-srq.sh` | Variant attaching the successful SRQ-nib G64. |
| `launch-x64sc-g64-dump.sh` | Launch a G64 with binary monitor, retry GTK3 startup, run `dump_drive_mem.py`, and stop VICE. Unsets `GTK_MODULES`/`GTK_USE_PORTAL`/`WAYLAND_DISPLAY` and forces `GDK_BACKEND=x11`. |
| `capture-srq.sh` | Combined SRQ capture workflow: `nibread -@xum1541 -D8 -e5 -h -v` then `nibconv`, with confirmation before reading the real disk. |

## Binary-monitor clients (Python)

These implement a minimal subset of VICE's binary monitor protocol: STX `0x02`, API version 2,
little-endian length/request ID, command byte, body. Checkpoint clients use command `0x12`;
keyboard feed uses `0x72`; exit/continue uses `0xAA`.

| Script | What it does |
|---|---|
| `binmon_trace.py` | Sets a checkpoint on stores to `$DD00` (C64 CIA2 IEC data port), feeds `LOAD"*",8,1`, logs accumulator values to `/tmp/opencode/binmon-iec.log`. Traces C64-side bit-banging. |
| `binmon_drive.py` | Stopping checkpoint on execution in drive RAM `$0300-$07FF`; logs `PC/A/X/Y`. Accurate but ~14,000× slower, so the loader only reaches its `$0520-$0525` delay loop. |
| `binmon_history.py` | **Preferred.** Feeds LOAD and polls VICE's CPU-history API (command `0x86`) for drive 8 without stopping execution. Fast enough for the protected loader to complete. Logs instruction bytes with a PC filter for `$0300-$07FF`. |
| `dump_drive_mem.py` | Feeds LOAD, waits for the `M-W` upload, then MEM_GETs drive-8 RAM `$0300-$07FF` and writes the 1,280 uploaded bytes. |
| `verify_vice_boot.py` | Feeds LOAD and RUN, then MEM_GETs C64 screen RAM `$0400-$07E7` and decodes it. Confirms a boot without screenshots. |
| `verify_vice_boot_long.py` | Longer-wait variant: 35 s after LOAD, 25 s after RUN, for chained loaders. |
| `verify_vice_boot_very_long.py` | Very-long-wait variant: 120 s after LOAD, 120 s after RUN, for color-cycling loaders. |
| `monitor_load_progress.py` | Samples screen RAM, color RAM `$D800-$DBFF`, and VIC registers `$D020-$D02F` during a long load. |
| `sample_progress_only.py` | Same sampling loop without keyboard feed, for sessions where keys are sent with `xdotool`. |

## Other analysis helpers

| Script | Purpose |
|---|---|
| `compare_halftracks.py` | Compares whole/half-track GCR blocks from a `nibread -h` NIB capture, allowing cyclic rotation, to test for wide/fat duplicated tracks. |
| `nib2g64_raw.py` | Converts NIB → G64 while preserving exact raw track lengths and all half-track slots; unlike `nibconv`, it does not normalize long tracks. |
| `boot-test-g64.sh` | Launches x64sc with a G64, retries GTK3 startup, runs `verify_vice_boot_long.py`, and prints a screen summary. |
| `boot-test-rpm.sh` | Same as `boot-test-g64.sh` but sets `-drive8rpm` to test motor-speed-sensitive protection (`30000` = 300 rpm). |
| `scan_kj.sh` | Decodes every Kracker Jax `.nbz` via `--convert`, exports files, and greps `strings` for gap/sync/track terms. Found `TGREAD` and a header-gap `$0A` reference. |
| `search_fat_tracks.sh` | Parallel `pdftotext` search over the Commodore PDF collection for "fat track" variants (result: zero matches). |

## Monitor command file

`vice-mon-cmds.txt` is used with `-moncommands`:

```text
logname "/tmp/opencode/g64-mon.log"
log on
exit
```

Earlier variants used `trace store @io:$dd00`; text-monitor tracepoints did not reliably log after
`exit`, hence the binary monitor clients.

## Typical workflows

Drive-CPU trace:

```bash
tools/build-patched-vice.sh
tools/diagnostics/launch-x64sc-g64.sh &
python3 tools/diagnostics/binmon_drive.py
# inspect /tmp/opencode/g64-drive-trace.log
```

Dump the uploaded drive code:

```bash
tools/diagnostics/launch-x64sc-g64-dump.sh /tmp/opencode/real-disk.g64 \
    /tmp/opencode/drive-0300.bin
# or, with an already-running monitor:
python3 tools/diagnostics/dump_drive_mem.py /tmp/opencode/drive-0300.bin
```

Verify a boot and sample color cycling:

```bash
python3 tools/diagnostics/verify_vice_boot.py /tmp/opencode/vice-screen.bin
python3 tools/diagnostics/monitor_load_progress.py 180
# or when keys come from xdotool:
python3 tools/diagnostics/sample_progress_only.py 180
```

Test wide/fat tracks and raw G64 conversion:

```bash
python3 tools/diagnostics/compare_halftracks.py \
    /tmp/opencode/real-disk.nib /tmp/opencode/nibread-real2.log
python3 tools/diagnostics/nib2g64_raw.py disk.nib nibread.log raw-preserve.g64
tools/diagnostics/boot-test-g64.sh raw-preserve.g64 raw
tools/diagnostics/boot-test-rpm.sh raw-preserve.g64 29500 rpm295
```
