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
| `binmon_drive.py` | Sets a checkpoint on **execution in drive-8 RAM `$0300-$07FF`**, then feeds the same LOAD command. On each hit it logs `PC/A/X/Y` from the register response to `/tmp/opencode/g64-drive-trace.log`. This is the drive-CPU trace of the uploaded fast-loader/protection code when using `launch-x64sc-g64.sh`. |

Both send command `0x12` (checkpoint set) and `0x72` (keyboard feed), then repeatedly send
`0xAA` (exit/continue) while collecting responses.

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

Remember: only one process may own the real XUM1541. Stop VICE before any `cbmctrl` or
`d64copy` command, and `cbmctrl reset` afterward.
