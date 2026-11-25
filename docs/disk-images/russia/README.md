# RUSSIA (ID `GM`) — Preservation Artifacts and Protection Analysis

This directory contains preservation artifacts and a technical analysis of the RUSSIA disk's
copy protection. The NIB is the preservation master; G64 and D64 derivatives are not faithful
for protected behavior.

## Artifacts

| File | Description |
|---|---|
| `disk8-protected.nib` | Raw MNIB-1541-RAW capture (nibread SRQ, 1571/XUM1541, 1541 mode). 672000 bytes, 82 tracks. Preservation master. |
| `disk8-protected.g64` | nibconv-derived G64 from the NIB. **Not faithful** for RUSSIA protection playback in VICE (loads briefly, returns to `READY`). |
| `disk8-protected-report.txt` | `--cbm preserve` report: surface scan, directory, NIB track summary, protection findings. |
| `disk8-protected.log` | Capture/preserve log. |
| `write-verify-nibread.log` | Post-write verification nibread (after writing NIB back to a fresh disk in 1571/1541 mode with `nibwrite`). Confirms protection markers are present. |

## Disk identity

- Directory name: `RUSSIA`
- Disk ID: `GM`
- DOS: 2A
- Tracks reported by directory: 40
- Files: 20 PRGs, 202 blocks free
- Files observed: `RUS`, `CA`, `S0`–`S3`, `MAPN`, `G0`–`G4`, `CD`, `SV`, `C0`, `C1`, `U1`, `U2`, `CF`, `ST`

## Verification (physical write-back)

The NIB master was written to a fresh disk in a real 1571 (device 8) set to 1541 mode:

```bash
cbmctrl command 8 'U0>M0'
nibwrite -@xum1541 -D8 /home/pat/disk8-protected.nib
```

nibwrite reported tracks 1.0–35.0 written, track 36.0 written with weak bits
(`[weak:5230][sync:-19]`), and tracks 37.0–41.0 unformatted. A post-write nibread
(`nibread -@xum1541 -D8 -e5 -h -v`, see `write-verify-nibread.log`) confirmed:

- Normal CBM tracks through 35.0, with half-track/weak-GCR activity on many half tracks.
- Track 36.0/36.5: long reads / NDOS / heavy weak GCR.
- Tracks 37.0–41.5: unformatted / NOSYNC.

## Protection summary (deep logic)

RUSSIA uses a **deliberately unreadable sector** plus a **drive-head-bump disable**. It does
not upload drive code (`M-E`) and does not use a custom fastloader for the check. The check
is performed through normal 1541 DOS commands.

### 1. Loader entry: `RUS` (BASIC)

`RUS` is a BASIC program that prepares the drive and runs the machine-language checker.

De-tokenized (`petcat -2`):

```basic
10 ifx=1then100
15 open 15,8,15
16 print#15,"m-w"chr$(106)chr$(0)chr$(1)chr$(133)
17 close15
20 print"{clr}{lblu}"
30 x=1
40 poke53272,(peek(53272)and240)or6
50 poke53281,0:poke53280,0
60 print"...Please wait..."
70 load "st",8,1
100 sys (12032)
```

Key action: **Memory-Write to drive RAM `$006A`** (decimal 106, 0) with value `$85` (133).

- `$006A` is the 1541 DOS REVCNT/read-retry counter area (normally 5).
- Value `$85` keeps the retry count at 5 but sets bit 7.
- Bit 7 disables the 1541's head bump/recalibration behavior on repeated read errors.
- Purpose: the damaged sector must remain damaged. If DOS bumped the head during retries,
  behavior could change or recovery might occur; disabling bump makes the check stable and
  keeps the protection meaningful.

After setting REVCNT, it loads `ST` at its PRG load address (`$2F00`) and executes it with
`SYS 12032` (`$2F00`).

### 2. Protection checker: `ST` (machine language, load `$2F00`)

`ST` is a small 6502 program. Entry:

```asm
$2F00: JMP $2F0B
```

#### 2.1 Environment hardening / setup ($2F0B)

```asm
LDA #$36        ; STA $01 - normal C64 RAM/IO/KERNAL mapping
LDA #$80        ; STA $0291 - STOP/screen control flag
LDA #$02        ; STA $0289 - keyboard buffer size
LDA #$C1        ; STA $0318 - NMI vector low byte (NMI hijack/hardening)
LDX #$07        ; begin signature check
```

This is light anti-tamper/environment setup: normal memory map, keyboard/STOP behavior,
and NMI vector low-byte modification.

#### 2.2 Signature/crack-flag bypass ($2F20)

`ST` compares 8 bytes at `$CF00,X` against bytes at `$2F03,X` (`X=7..0`):

```asm
L2F20: LDA $CF00,X
       CMP $2F03,X
       BNE L2F2D      ; mismatch -> perform real disk check
       DEX
       BPL L2F20
       BMI L2F35      ; all matched -> bypass protection
L2F2D: JSR L2F63      ; real protection check
       BCC L2F35      ; CLC = pass
       JMP $FCE2      ; fail -> KERNAL RESET/warm start
L2F35: JMP L2F3E      ; success path
```

The signature bytes at `$2F03-$2F0A` are the 8-byte image compared against `$CF00`. If RAM
at `$CF00` already matches, the disk check is skipped. This is a bypass/crack-flag style
check: a patcher can prefill `$CF00-$CF07` with the expected bytes before ST runs.

#### 2.3 The actual protection check ($2F63)

The check uses standard CBM DOS block-read command **U1** over an open direct-access channel.
It does **not** use M-W/M-E drive code upload.

```asm
; Open direct-access channel 5,8,5,"#"
SETNAM "#"
SETLFS 5,8,5
OPEN

; Open command channel 15 with U1 as the OPEN filename
; "U1:5,0,18,18" = U1 <channel> <drive> <track> <sector>
SETNAM "U1:5,0,18,18"
SETLFS 15,8,15
OPEN

CLOSE 5
CHKIN 15
; read status string into $0340+Y until CR or Y wraps
...
CLOSE 15

LDA $0340
CMP #$32        ; '2'
BNE fail
CLC             ; pass
BIT $38         ; skip SEC (BIT zp consumes next byte)
fail: SEC
RTS
```

Pass condition: DOS status begins with ASCII `'2'`, i.e. `23,READ ERROR,18,18`.

Fail condition: any other status (including `00,OK,00,00` on a clean/correctly copied sector)
causes SEC; caller resets through `$FCE2`.

This exactly matches the physical behavior observed on the real disk:

```bash
cbmctrl open 8 5 '#'
cbmctrl command 8 'U1:5,0,18,18'
cbmctrl status 8      # 23,read error,18,18
cbmctrl close 8 5
```

The bad sector is **track 18, sector 18**. FDC surface scans report T18/S18 as data checksum
error (FDC `$05`), while adjacent sectors read fine.

#### 2.4 Success path ($2F3E)

On pass, ST loads and runs the next stage:

```asm
SETNAM "0:CD"
SETLFS 8,8,0
LOAD A=0, X=$00, Y=$6B   ; load to $6B00
JMP $6B00
```

`CD` is the next game stage; later stages may perform additional checks.

## Why a normal D64 copy fails

A normal sector-level D64 copy stores reconstructed sector data, not raw GCR timing/gaps.
The RUSSIA protection sector is a deliberately bad-checksum sector; normal copying makes it
readable, so the U1 check returns `00,OK,...` instead of `23,READ ERROR,...` and ST resets.

## Why the nibconv G64 fails in VICE

The NIB master reaches the title screen through real-drive passthrough. The nibconv G64 loads
briefly and returns to `READY` in VICE. Important findings:

- **The bad T18/S18 checksum is preserved** in the G64 (GCR decode confirms bad data checksum).
- A VICE binary-monitor probe using the correct U1 form (OPEN 15 filename, not `PRINT#15`)
  returned `23,READ ERROR,18,18` for T18/S18 on the protected G64. Therefore VICE sees the
  bad sector; the loader reset is **not** caused by nibconv fixing that sector.
- Remaining suspects: REVCNT/head-bump emulation behavior, rotation/timing/gap sensitivity,
  missing duplicate T18 headers (22 in NIB vs 19 in G64), or later checks in `CD`/game stages.
- Raw NIB-to-G64 wrapping (`--raw2g64`) is not a substitute: wrong zones give `FILE NOT FOUND`,
  correct zones with full-length tracks still fail to read directory, and rotated/trimmed raw
  bytes introduce invalid GCR.

**Conclusion:** NIB is the authoritative preservation master. The G64 is a best-effort playable
derivative, not a faithful preservation copy.

## U1 probe pitfalls (important)

- Do **not** send U1 as `PRINT#15,"U1:..."` for this probe. In VICE that returns
  `30,SYNTAX ERROR,00,00`. Send U1 as the OPEN 15 filename:
  `OPEN 15,8,15,"U1:5 0 18 18"`.
- Autostart probe PRGs in VICE; do not type uppercase via xdotool (PETSCII keymap mangles).
- Avoid `-autostartprgmode 1` on this Xwayland/VICE setup; plain `-autostart` worked.
- For screen capture in GL/VICE, reading C64 screen RAM via VICE binary monitor is reliable.

## References

- `docs/drive-assembly/README.md` §13 — RUSSIA investigation notes.
- `HARDWARE_WORKFLOW.md` — OpenCBM/VICE workflow and probe gotchas.
- `D64Editor.Core/OpenCbm.cs` — `ReadSectorDirect()` implements the safe U1 read with timeout
  and channel close in `finally`.
