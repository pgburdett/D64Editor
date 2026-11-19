# Verified CP/M → real 1571 → VICE workflow

Standalone reference for the end-to-end flow verified with a **ZoomFloppy (XUM1541)**, a real
**Commodore 1571 on device 8**, **VICE 3.10 `x128`**, and D64Editor's in-process CP/M engine.
All CP/M image work is done inside D64Editor (`C128Cpm`); external tools are used only for
physical disk I/O and emulator/screenshot control.

For the broader feature context see [README.md](README.md#complete-real-hardware-workflow-verified-end-to-end).

## Tools and exact parameters

| Application | Parameters | Purpose |
|-------------|------------|---------|
| `dotnet run --project D64Editor.App --` | `--cpm-extract <img> <dir> --strict` | Extract CP/M files in-process; `--strict` limits scanning to real directory slots |
| D64Editor | `--cpm-copy <donor> <base> --files CPM+.SYS,CCP.COM,DIR.COM,PIP.COM` | Keep system files, free data blocks by dropping large files |
| D64Editor | `--cpm-bootable <base> <out> NAME.EXT=hostfile ...` | Clone boot code/CCP/BDOS and add files |
| `cbmctrl` | `detect`, `status 8`, `command 8 'U0>M0'`, `command 8 'U0>M1'`, `reset` | Bus state, drive status, 1541/native-1571 mode, reset all drives on the IEC bus |
| `d64copy` | `--no-progress -w <image.d64> 8` | Write D64 in **warp** mode; **no `-t original`** |
| `d64copy` | `--no-progress -w -2 <image.d71> 8` | Write D71 in true 1571 mode (`-2`); `-w` is implicit for `.d71` |
| `d64copy` | `--no-progress -w 8 <readback>` / `--no-progress -2 8 <readback>` | Non-destructive readback |
| `cmp` | `<image> <readback>` | Byte-for-byte write verification |
| `x128` | `-80col -8 <image.d64> -sounddev dummy -logfile <file>` | Local image boot check |
| `x128` | `-80col -devicebackend8 2 -busdevice8 -sounddev dummy` | Boot from the **real** drive via OpenCBM passthrough; add `-9 <data.d64>` for drive 9 (`B:`) |
| `xdotool` | `search --onlyvisible --class x128`, `windowactivate --sync`, `type --delay 100 'DIR'`, `key Return` | Send CP/M commands to the emulator |
| `spectacle` | `-b -o <file.png>` | Desktop capture (X11 root capture is black on this Xwayland session) |

## 0. Install and check the hardware

```bash
paru -S opencbm                    # Arch/AUR
# if /etc/opencbm.conf is missing: copy /etc/opencbm.conf.sample to it, then:
sudo opencbm_plugin_helper_tools rebuild
# if the cable was plugged in before install:
sudo udevadm control --reload-rules && sudo udevadm trigger

cbmctrl detect                     # e.g. " 8: 1571"
cbmctrl status 8                   # e.g. "73,cbm dos v3.0 1571,00,00"
cbmctrl reset                      # resets all drives on the IEC bus (no device argument)
cbmctrl status 8
```

The harmless `undefined symbol: keyctl` warning from libudev can be ignored.

## 1. Build a bootable CP/M image (in-process)

```bash
# Stage files from a reference image
dotnet run --project D64Editor.App -- --cpm-extract 128cpm/MBASIC.d71 \
    /tmp/opencode/cpm-stage --strict

# Free space: the full system donor has no room for new files, and CCP.COM must stay
dotnet run --project D64Editor.App -- --cpm-copy \
    128cpm/c128-cpm.system.6228129311-9283de.d64 /tmp/opencode/boot-base.d64 \
    --files CPM+.SYS,CCP.COM,DIR.COM,PIP.COM

# Add the files; boot code/CCP/BDOS come from the donor
dotnet run --project D64Editor.App -- --cpm-bootable /tmp/opencode/boot-base.d64 \
    /tmp/opencode/bootable.d64 \
    MBASIC.COM=/tmp/opencode/cpm-stage/MBASIC.COM \
    WUMP.BAS=/tmp/opencode/cpm-stage/WUMP.BAS

# Verify contents before touching hardware
dotnet run --project D64Editor.App -- --cpm-extract /tmp/opencode/bootable.d64 \
    /tmp/opencode/cpm-verify --strict
# -> CPM+.SYS, CCP.COM, PIP.COM, DIR.COM, MBASIC.COM, WUMP.BAS (byte-exact)
```

Do **not** drop `CCP.COM` — CP/M Plus will not boot without it. A D71 container can be made
from the D64 image by copying it to a 349,696-byte file (single-sided payload on side 1, blank
side 2). A true double-sided 2K-block CP/M system disk would need a real D71 boot donor.

## 2. Boot the image locally first

```bash
x128 -80col -8 /tmp/opencode/bootable.d64 -sounddev dummy \
    -logfile /tmp/opencode/x128-boot.log
# At the A> prompt: DIR -> CPM+.SYS, CCP.COM, PIP.COM, DIR.COM, MBASIC.COM, WUMP.BAS
```

## 3. Write both formats to the real 1571 (warp, no `-t`)

```bash
# D64 — 1541 (single-sided) mode
cbmctrl command 8 'U0>M0'
d64copy --no-progress -w /tmp/opencode/bootable.d64 8
d64copy --no-progress -w 8 /tmp/opencode/d64-readback.img
cmp /tmp/opencode/bootable.d64 /tmp/opencode/d64-readback.img

# D71 — native 1571 mode, two-sided (-2)
cbmctrl command 8 'U0>M1'
d64copy --no-progress -w -2 /tmp/opencode/bootable.d71 8
d64copy --no-progress -2 8 /tmp/opencode/d71-readback.img
cmp /tmp/opencode/bootable.d71 /tmp/opencode/d71-readback.img
```

Both `cmp` calls must report identical files. `--cbm verify <image> <dev> [--d71]` in D64Editor
performs the same write/readback/compare, also using warp with no `-t`.

## 4. Boot from the physical drive

```bash
x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy
# VICE log: "Real Device: libusb/xum1541:1/19 opened"
# CP/M 3.0: DATA TABLES / COMMON CODE / BANKED CODE -> 59K TPA -> A>
```

### Build/start/stop the command with D64Editor

```bash
# Print the exact command line
dotnet run --project D64Editor.App -- --cbm vice command
# x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy

# With optional data disk, software GL and log file
dotnet run --project D64Editor.App -- --cbm vice command \
    --dev 8 --data /tmp/opencode/bootable.d71 --softgl --log /tmp/opencode/vice.log
# LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe x128 -80col \
#   -devicebackend8 2 -busdevice8 -9 /tmp/opencode/bootable.d71 \
#   -sounddev dummy -logfile /tmp/opencode/vice.log

# Launch and stop (PID recorded in /tmp/d64editor-vice.pid)
dotnet run --project D64Editor.App -- --cbm vice start --softgl --log /tmp/opencode/vice.log
dotnet run --project D64Editor.App -- --cbm vice stop
```

Options: `--dev N` (real IEC device, default 8), `--data IMG` (drive 9/`B:`), `--emu PATH`
(default `x128`), `--40col`, `--sound DEV` (default `dummy`), `--log FILE`, `--softgl`,
`--dry-run`. `--cbm boot <image> <dev>` remains a legacy alias for `vice start --data`.

To stop a manually launched VICE: `pkill -f '^x128 '`.

### C64 (`x64sc`) with the real drive — verified display settings

For a C64C/NTSC session with the real 1571 attached as device 8, this is the verified launch
(command line from the successful session; drive must be in 1541 mode, `U0>M0`):

```bash
LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe x64sc \
    -model c64c -ntsc -power60 \
    -VICIIaspectmode 0 -windowwidth 1400 -windowheight 900 \
    -devicebackend8 2 -busdevice8 -sounddev dummy \
    -logfile /tmp/opencode/x64sc-real.log
```

- `-model c64c -ntsc -power60` → C64C, NTSC (VIC-II 8562), 60 Hz power grid.
- `-VICIIaspectmode 0` → square pixels; removes the grey pillarboxes VICE otherwise adds for
  aspect-ratio correction.
- `-windowwidth 1400 -windowheight 900` → the preferred 2× screen size; the C64 border fills
  the window width with no grey side bars. If the window is resized live, VICE scales the
  rendered screen to fill it:
  ```bash
  wid=$(xdotool search --onlyvisible --class x64sc | tail -1)
  xdotool windowsize "$wid" 1400 900
  ```
- Software GL (`LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe`) works around the intermittent
  GTK3 startup segfault on this Xwayland session; retry the launch until one instance stays up.
- Stop with `pkill -f '^x64sc '`, then `cbmctrl reset` to release the IEC bus.

## 5. Observing and cleaning up

```bash
# After the A> prompt appears (allow ~20-40 s for a real-drive boot):
wid=$(xdotool search --onlyvisible --class x128 | tail -1)
xdotool windowactivate --sync "$wid"
xdotool type --window "$wid" --delay 100 'DIR'
xdotool key --window "$wid" Return
sleep 5
spectacle -b -o /tmp/opencode/vice-real-d71-dir.png

# Stop VICE, then release/reset the bus:
pkill -f '^x128 '
cbmctrl reset
cbmctrl status 8
```

## Notes and troubleshooting

- **`x64sc` LOAD from the real drive hangs (VICE 3.10 + OpenCBM 0.4.99):** diagnosed with a
  gdb trace of the OpenCBM plugin calls. VICE issues `cbm_open(dev=8, sec=0, name="", len=0)`,
  then sends the filename (`*`) as a separate `cbm_raw_write`, `UNLISTEN`, `cbm_talk(8,0)`, and
  then loops forever on `cbm_raw_read(1)` + `cbm_get_eoi()` — EOI never arrives and no data is
  returned. The drive-side error is the empty filename at OPEN time.
  - Verified directly against libopencbm: `cbm_open(f,8,0,"$",1)` → `cbm_talk` →
    `cbm_raw_read`+`cbm_get_eoi` returns the 128-byte directory with EOI at byte 127;
    `cbm_open(f,8,0,"*",1)` reads a 337-byte PRG with EOI at the last byte. Replicating VICE's
    empty-name-then-write sequence returns `n=0, eoi=1` immediately (no data).
  - So the XUM1541 plugin and EOI handling are fine; VICE 3.10's real-device backend is not
    passing the filename through `cbm_open` correctly for this OpenCBM version. Workarounds:
    use a VICE build where this is fixed, or use the emulated filesystem backend (`.d64`/`.d71`
    image on drive 8/9) for program loading while keeping the real drive for CP/M boot writes.
  - To reproduce the trace: launch x64sc with `-logfile`, `-verbose -debug -loglimit 255`, set
    `XUM1541_DEBUG=1`, and attach `gdb` (needs root/`ptrace_scope`) with breakpoints on
    `opencbm_plugin_{open,talk,raw_read,raw_write,get_eoi,unlisten}`.
- **One owner of the bus:** VICE uses OpenCBM, so stop VICE before any `cbmctrl`/`d64copy`
  command, and finish those before launching VICE.
- **Never use `-t original`** for these transfers — it is slow and can time out; warp is the
  default and `-w` makes it explicit. `.d71` transfers are always warp.
- **VICE startup race:** on this Xwayland session the GTK3 build segfaults on most launches.
  Use `--softgl` (`LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe`) and retry until it stays up.
- **Screenshots:** `import`/X11 root `x11grab` produce a black image here; `spectacle -b -o ...`
  captures the composited desktop correctly.
- **Input timing:** wait for the `A>` prompt before `xdotool type`; typing during the boot
  banner is ignored.
- The CP/M image occupies 35 tracks; formatting 40-track extended (`cbmforng -x`) is harmless.
