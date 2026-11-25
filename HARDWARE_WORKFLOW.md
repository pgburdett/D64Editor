# Verified CP/M → real 1571 → VICE workflow

This document records the complete, verified workflow for taking a C128 CP/M image from Linux,
writing it to a real Commodore 1571 on device 8 via ZoomFloppy/XUM1541, and booting it from the
physical drive in VICE 3.10. All CP/M image construction is in-process via `C128Cpm`; ctools and
cpmtools are optional cross-checks. External tools are used only for physical I/O, emulator
control, screenshots, and key injection.

## Tools and exact parameters

| Tool | Role |
|---|---|
| D64Editor `--cpm-*` | Build/extract/copy/format/import CP/M images in-process. |
| `cbmctrl` | Detect/status/command/reset the IEC bus and drive. |
| `d64copy` | Whole-image read/write. Warp, no `-t`, for CP/M media. |
| VICE `x128` | C128 emulator; `-devicebackend8 2 -busdevice8` boots from the real drive. |
| `xdotool` | Send keys to the VICE window. |
| `spectacle` | Capture the Xwayland session (`-b -o ...`; X11 root capture is black). |

## 0. Install and check hardware

```bash
paru -S opencbm
# If /etc/opencbm.conf is missing:
sudo cp /etc/opencbm.conf.sample /etc/opencbm.conf
sudo opencbm_plugin_helper_tools rebuild
sudo udevadm control --reload-rules && sudo udevadm trigger

cbmctrl detect          # e.g. " 8: 1571"
cbmctrl status 8        # e.g. "73,cbm dos v3.0 1571,00,00"
cbmctrl reset
cbmctrl status 8
```

The harmless `undefined symbol: keyctl` warning from libudev can be ignored. The USB cable may
need a replug after installing udev rules.

## 1. Build a bootable CP/M image in-process

A full donor disk is usually too full to add files. Free blocks first, keeping `CCP.COM`:

```bash
dotnet run --project D64Editor.App -- --cpm-extract 128cpm/MBASIC.d71 \
    /tmp/opencode/cpm-stage --strict

dotnet run --project D64Editor.App -- --cpm-copy \
    128cpm/c128-cpm.system.6228129311-9283de.d64 \
    /tmp/opencode/boot-base.d64 \
    --files CPM+.SYS,CCP.COM,DIR.COM,PIP.COM

dotnet run --project D64Editor.App -- --cpm-bootable /tmp/opencode/boot-base.d64 \
    /tmp/opencode/bootable.d64 \
    MBASIC.COM=/tmp/opencode/cpm-stage/MBASIC.COM \
    WUMP.BAS=/tmp/opencode/cpm-stage/WUMP.BAS

dotnet run --project D64Editor.App -- --cpm-extract /tmp/opencode/bootable.d64 \
    /tmp/opencode/cpm-verify --strict
```

Never drop `CCP.COM`; CP/M Plus will not boot without it. The verified D71 container is 349,696
bytes: a single-sided payload on side 1 and a blank side 2.

## 2. Boot the image locally first

```bash
x128 -80col -8 /tmp/opencode/bootable.d64 -sounddev dummy \
    -logfile /tmp/opencode/x128-boot.log
```

Confirm the CP/M 3.0 banner and `A>` prompt before touching hardware.

## 3. Write both formats to the real 1571

Use warp mode (the default) and never `-t original` for these transfers.

```bash
# D64: 1541-compatible mode
cbmctrl command 8 'U0>M0'
d64copy --no-progress -w /tmp/opencode/bootable.d64 8
d64copy --no-progress -w 8 /tmp/opencode/d64-readback.img
cmp /tmp/opencode/bootable.d64 /tmp/opencode/d64-readback.img

# D71: true two-sided 1571 mode
cbmctrl command 8 'U0>M1'
d64copy --no-progress -w -2 /tmp/opencode/bootable.d71 8
d64copy --no-progress -2 8 /tmp/opencode/d71-readback.img
cmp /tmp/opencode/bootable.d71 /tmp/opencode/d71-readback.img
```

`--cbm verify <image> <dev> [--d71]` performs the same write/read/compare through D64Editor.

## 4. Boot from the physical drive

```bash
x128 -80col -devicebackend8 2 -busdevice8 -sounddev dummy
```

The VICE log should show `Real Device: libusb/xum1541:1/19 opened`. CP/M 3.0 loads DATA TABLES,
COMMON CODE, and BANKED CODE, reports 59K TPA, and reaches `A>`.

### Build/start/stop with D64Editor

```bash
dotnet run --project D64Editor.App -- --cbm vice command
dotnet run --project D64Editor.App -- --cbm vice command \
    --dev 8 --data /tmp/opencode/bootable.d71 --softgl --log /tmp/opencode/vice.log
dotnet run --project D64Editor.App -- --cbm vice start --softgl --log /tmp/opencode/vice.log
dotnet run --project D64Editor.App -- --cbm vice stop
```

Options: `--dev N`, `--data IMG` (drive 9 / `B:`), `--emu PATH`, `--40col`, `--sound DEV`,
`--log FILE`, `--softgl`, `--dry-run`. A manually launched VICE can be stopped with
`pkill -f '^x128 '`.

### C64 `x64sc` with the real drive

Verified display settings:

```bash
LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe x64sc \
    -model c64c -ntsc -power60 \
    -VICIIaspectmode 0 -windowwidth 1400 -windowheight 900 \
    -devicebackend8 2 -busdevice8 -sounddev dummy \
    -logfile /tmp/opencode/x64sc-real.log
```

`-VICIIaspectmode 0` gives square pixels; the window size fills the C64 border. Live resize with
`xdotool windowsize <wid> 1400 900`. Stop VICE and run `cbmctrl reset` afterward.

## 5. Observe and clean up

```bash
wid=$(xdotool search --onlyvisible --class x128 | tail -1)
xdotool windowactivate --sync "$wid"
xdotool type --window "$wid" --delay 100 'DIR'
xdotool key --window "$wid" Return
sleep 5
spectacle -b -o /tmp/opencode/vice-real-d71-dir.png

pkill -f '^x128 '
cbmctrl reset
cbmctrl status 8
```

Wait for the `A>` prompt before typing. Only one process may own the IEC bus.

## Notes and troubleshooting

### VICE 3.10 real-device LOAD hang

`x64sc` issues `cbm_open(dev=8, sec=0, name="", len=0)` then sends the filename as later raw
writes; the read loop then loops forever in `cbm_raw_read`/`cbm_get_eoi`. Direct libopencbm tests
prove EOI handling is fine when the filename is passed to `cbm_open`. The fix is
`patches/vice-3.10-opencbm-filename.patch`, built by `tools/build-patched-vice.sh`; details in
`patches/README.md` and `docs/drive-assembly/c-probes/README.md`.

Custom fast loaders are still unsupported by the high-level backend: they upload ~1,280 bytes to
drive RAM `$0300-$07FF`, install a `JMP $04CC` stub, rotate their first 22 bytes to hide it, and
bit-bang VIA1 at `$1800`. Use `-drive8truedrive` with a faithful raw G64 for protected games.

### VICE environment

GTK3 startup is flaky on this Xwayland session. Use `--softgl` (software GL/llvmpipe), retry,
capture with `spectacle -b -o ...`, and inject keys with `xdotool`. 40-track format commands are
harmless for CP/M images (35 tracks).

#### Keyboard injection, autostart, and tokenized probes

VICE 3.10's PETSCII keymap expects **lowercase** ASCII from `xdotool`/`-keybuf`. Uppercase text such
as `LOAD"*",8,1` is mangled into `LOAD""` and produces `?SYNTAX ERROR` or `?DEVICE NOT FOUND`.
For reliable LOADs use lowercase:

```bash
x64sc -8 disk.g64 -keybuf 'load"*",8,1\n' -keybuf-delay 100
```

For raw DOS-status probes, avoid keyboard typing entirely: autostart a tokenized BASIC PRG with the
disk under test on **device 8** (do not load a helper from device 9 when the command targets 8).
`-autostartprgmode 1` aborts VICE on this session; plain `-autostart` injects the PRG into RAM and
runs it. Probe source used for the RUSSIA T18/S18 investigation:

```basic
10 OPEN 15,8,15
20 OPEN 5,8,5,CHR$(35)
30 PRINT#15,"U1:5,0,18,18"
40 INPUT#15,A,B$,C,D
50 PRINT A,B$,C,D
60 CLOSE 5
70 CLOSE 15
```

Standard BASIC v2 token bytes are `0x9F` OPEN, `0x98` PRINT#, `0x84` INPUT#, `0x99` PRINT and
`0xC7` CHR$; minimal PRGs confirmed these tokens execute (a bare `PRINT#`/`INPUT#` reports
`?FILE NOT OPEN`, not a syntax error). `petcat -w2` output on this system did not contain those
bytes, so hex-dump any generated PRG before relying on it. The first full probe run still ended with
`?SYNTAX ERROR` before printing a DOS status, so the conclusive in-VICE U1 reading is still open;
see `docs/drive-assembly/README.md` §13.

### NBZ and protection references

- `--convert in.nbz out.d64` decodes preservation NBZ images.
- Kracker Jax `TGREAD` uploads code to `$0500`, reads five bytes from `$1C01` into `$0300-$0304`;
  `--cbm tailgap` is the port.
- Arctic Fox's loader runs at `$0300-$07FF` and settles into a `$0520-$0525` delay loop.
- Faithful capture: `nibread -@xum1541 -D8 -e5 -h -v` → `nibconv` → G64 → x64sc
  `-drive8truedrive`. D64 copies lose gaps/timing.
- `--cbm gaps` anomaly signature: T2 0 sync/0 sectors, T36 469/0, T37–40 sync/no sectors;
  `--protect` reproduces 0/0, 468/0, and 33/0.

### Raw GCR sampler

`--cbm gcrscan` uploads a 6502 routine that samples 256 bytes from VIA2 `$1C01` on the **current**
track into `$0400-$04FF`; it never writes stepper bits. See `docs/drive-assembly/README.md` for the
full VIA/FDC reference and safety notes.
