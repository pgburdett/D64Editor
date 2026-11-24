# 6502 drive-code reference

This is the detailed reference for the 1541/1571 drive-side code used in D64Editor's
copy-protection and raw-media investigation. It covers VIA registers, byte-ready handshaking,
FDC job queues, the Kracker Jax TGREAD port, dynamic fastloader traces, protected-track byte
patterns, the destructive serial raw writer, and the in-progress raw GCR reader.

Product code lives in `D64Editor.Core`; experimental assembly and captured samples live in this
folder. `AGENTS.md` is the short agent-facing summary; this document is the long-form reference.

## Background: EA PRODOS / PirateSlayer protection

Arctic Fox (Electronic Arts / Dynamix, 1986) uses **PRODOS**, EA's early proprietary protection
system, which later evolved into PirateSlayer. It relies on several layers:

- **Non-standard tracks and gaps.** Protected tracks replace normal `$55` intersector gaps and
  sector framing with sync-less streams, altered gap lengths, and custom GCR markers. Sector-based
  copiers either miss them or reproduce them incorrectly.
- **Custom drive code.** The loader uploads 6502 code into drive RAM `$0300-$07FF`, bypassing the
  drive ROM, reading raw GCR from VIA2 `$1C01`, and checking patterns in gaps and between blocks.
- **Rotation/timing verification.** Like PirateSlayer, it checks physical rotation timing and
  tail-gap spacing. At ~300 RPM, drive-speed variation means a byte-exact copy can still fail the
  timing window.
- **Killer/unformatted regions.** Deliberate read errors, unformatted space, and long sync runs
  hang naïve nibblers that try to find track boundaries.
- **Imaging requirements.** These tracks cannot be represented faithfully in `.d64`. Preservation
  needs raw GCR (`.g64`/`.nib`). `nibread` reports non-standard track lengths and custom-GCR
  warnings on T35/T36; `nibconv` preserves exact byte counts for VICE.

### Confirmed vs. not directly measured

Confirmed by our traces and gap analysis:

- Uploaded drive code at `$0300-$07FF` executes in the emulated 1571 (sections 9–10).
- Raw GCR reads from `$1C01` use a `BVC`/`CLV` byte-ready loop (fastloader disassembly and
  `GcrReader`).
- Void track 2 (no `$FF`, zero decoded sectors) and non-standard extended tracks 36–40, with
  long sync runs on T36 on the preservation image.
- Tail-gap reading is strongly supported by the Kracker Jax `TGREAD` utility and our gap-byte
  capture (`60 24 46 23 99`).
- D64 → G64 conversion loses protection and fails the EA loader (section 11).

Not directly measured:

- Track-length/rotation timing. The fastloader's `$0520-$0525` delay/handshake loop is consistent
  with such a check, but no isolation test was performed.

Some background is user-provided research context; the confirmed items above are backed by traces
and hardware/CLI observations.

### Assembling and disassembling

```bash
xa -o out.bin source.s
da65 --cpu 6502 --start-addr 0x0300 out.bin
```

---

## 1. Drive hardware

The 1541/1571 has two MOS 6522 VIAs. **VIA2** controls the disk mechanism; **VIA1** controls the
IEC bus and is abused by fast loaders.

### VIA2 registers (disk side)

| Address | Register | Meaning used here |
|---|---|---|
| `$1C00` | Port B (DSKCNT) | bit 7 = SYNC (0 means SYNC found); bits 6–5 = speed zone; bit 4 = write-protect; bit 3 = LED; **bit 2 = motor (1 = on)**; bits 1–0 = stepper phase (never written by the safe sampler). |
| `$1C01` | Port A | **Raw GCR data to/from the read/write head.** |
| `$1C03` | DDRA | 0 = port A input (reading). |
| `$1C0A` | Shift register | DOS burst modes; not used here. |
| `$1C0C` | PCR | bits 7–5 = CB2 control, bits 3–1 = CA2 control, bit 4 = CB1 edge, bit 0 = CA1 edge. |
| `$1C0D` | IFR | bit 1 = CA1 = **BYTE-READY**; writing `02` clears it. |
| `$1C0E` | IER | Interrupt enable; not needed because IFR is polled directly. |
| `$1C0F` | Port A without handshake | unused. |

### PCR value `$EF`

`1110 1111`:

- CB2 bits 7–5 = `111` → high output. On VIA2 this selects read mode for the head (low = write).
- CA2 bits 3–1 = `111` → high output. On VIA2 this is SOE; high activates BYTE-READY.
- CA1 edge bit 0 = `1` → positive edge latches BYTE-READY.

Without CA2/SOE high, BYTE-READY never appears in IFR. That was the cause of the `EF` timeout in
`sampler3-strobe-wrong-pcr.s`.

### VIA1 registers (IEC side)

`$1800` Port B: bit 7 = ATN in, bit 4 = ATN ack out, bit 3 = CLOCK out, bit 2 = CLOCK in,
bit 1 = DATA out, bit 0 = DATA in. Fast loaders bit-bang these lines directly.

---

## 2. Why routines are entered with M-E

`cbmctrl command 8 'M-E' <lo> <hi>` makes the drive's Interface Processor (IP) jump to a 6502
routine in RAM. It works for code that touches only VIA registers.

It does **not** work for DOS disk-reading ROM routines such as `$F4D1` (read sector), `$F556`
(wait for SYNC), `$F97E` (motor on), or `$FE00` (switch to reading). Per *Inside Commodore DOS*,
disk I/O must be started by the FDC via a JUMP job (`$D0`) in the job queue at `$0000`; the FDC,
not the IP, must execute those routines. Calling them via M-E wedged the drive
(`sync-test-rom-jump.s`) and required a power cycle.

Safe approaches:

- Raw VIA sampling with `GcrSampler.s` — no ROM calls.
- Decoded sector reads and head movement via the FDC job queue (`Core/SectorRead.cs`).

---

## 3. Current safe sampler

`docs/drive-assembly/GcrSampler.safe.s` is the annotated reference copy; the build uses
`D64Editor.Core/Assets/GcrSampler.s` and embeds it as `gcrsampler.bin`. The routine is 126 bytes
assembled at `$0300` (drive buffer 0). It never writes stepper phase bits.

| Address | Size | Purpose |
|---|---|---|
| `$0300` | 1 | Status: `00` while running, `$A5` when done. |
| `$0301` | 1 | Phase: `04` sync found, `05` 256 bytes read, `EE` sync timeout, `EF` byte-ready timeout. |
| `$0400-$04FF` | 256 | Raw GCR bytes sampled from `$1C01`. |

Sequence:

1. `SEI`; clear status/phase.
2. PCR `$EF`; clear IFR with `02`; DDRA `00` (input).
3. Motor on with `ORA #$04` on `$1C00` (preserving other bits), plus a nested spin-up delay.
4. Wait for SYNC by polling `BIT $1C00` bit 7 with a 16-bit timeout. Timeout → phase `EE`.
5. Enter read loop: poll IFR bit 1, `CLV`, `LDA $1C01`, store to `$0400,y`, clear IFR, `INY`,
   repeat for 256 bytes. Per-byte timeout → phase `EF`.
6. Motor off with `AND #$FB`, status `$A5`, `CLI`, `RTS`.

Host protocol in `Core/GcrScan.cs`:

1. `OpenCbm.UploadMemory(8, 0x0300, samplerBytes)`.
2. `OpenCbm.Execute(8, 0x0300)` → `cbmctrl command 8 'M-E' 0 3`.
3. Poll `$0300/$0301` every 150 ms until `$0300 == $A5` (30 s timeout).
4. Require `$0301 == $05`; reject `EE`/`EF`.
5. `OpenCbm.DownloadMemory(8, 0x0400, 256, out raw)`.

A real track-18 sample began:

```text
24 44 12 49 91 9C 4C E3 C9 24 44 82 28 91 24 82
24 49 F2 49 C3 29 E6 3E 61 F1 17 F9 9C 9E 79 9F
F3 C4 CF F3 99 C8 91 F3 CF 33 F7 89 F9 34 F7 89
...
FF 32 79 9C FF FC
```

`$FF` is the GCR sync byte (ten consecutive 1 bits). Other bytes obey the 4-to-5 rule (no more
than two adjacent 0 bits, no adjacent 1 bits). `GcrScan` reports distinct values and `$FF` runs;
full GCR decoding of hardware samples is not yet done.

---

## 4. Experimental files and lessons

| File | Result | Lesson |
|---|---|---|
| `sync-test-rom-jump.s` | Wedged the drive | ROM disk routines must run in FDC context, not via M-E. Required power cycle. |
| `sync2-direct-via-timeout.s` | Works | Direct VIA: PCR `$EF`, motor bit 2, SYNC via `BIT $1C00`, hard timeout, one byte from `$1C01`, status `A5 02 <byte>`. |
| `sampler2-no-strobe.s` | One byte then zeros | Must wait for BYTE-READY; a tight loop re-reads the same latched byte. |
| `sampler3-strobe-wrong-pcr.s` | Timeout `EF` | IFR bit 1 polled, but CA2/SOE left at ROM value → no BYTE-READY. |
| `sampler4-working-direct-via.s` | Works | Adds PCR `$EF` and IFR clear; basis of the safe sampler. |
| `halfstep-one-phase.s/.bin` | Used once, supervised | 46-byte outward half-step (one VIA phase decrement). No zero-page writes, no homing. Used T35 → 35.5. |
| `halfstep-in-one-phase.s/.bin` | Used once, supervised | 48-byte inverse inward step. Used 35.5 → T35, verified by successful T35/S0 read. `inc` is reserved in `xa`; label is `incr`. |
| `game-fastloader-disassembly.asm` | Reference | 1,280-byte game fastloader uploaded to `$0300-$07FF` via 40 `M-W` commands and executed with `M-E`. Disables interrupts, installs `JMP $04CC` stub at `$07E0`, rotates its first 22 bytes to hide the stub, then bit-bangs VIA1 `$1800` and decodes nibbles through a table at `$0413`. |

### Abandoned stepper experiments

Several versions tried to move the head from uploaded code; all were removed after head clatter:

- One phase change of `$1C00` bits 1–0 = **half a track**; on-track phase is always even (0 or 2).
- Inward: `00 → 01 → 10 → 11 → 00`. Outward: `00 → 11 → 10 → 01 → 00`.
- Speed zones: T1–17 `$60`, T18–24 `$40`, T25–30 `$20`, T31–35 `$00`.
- One version moved two tracks per step (four phase changes per "track"); another corrupted DOS
  workspace via zero-page `$0003`; another overlapped parameter bytes with the code image.

Do not resurrect uploaded stepper/homing code. Use FDC SEEK for whole tracks.

---

## 5. Safe sector reads via FDC job queue

`Core/SectorRead.cs` exposes `--cbm sector` and `--cbm seek`. The buffer-0 job interface:

| Purpose | Address | Value |
|---|---|---|
| Job queue slot 0 | `$0000` | `$80` READ, `$B0` SEEK, `$D0` JUMP |
| Track (header) | `$0006` | track number |
| Sector (header) | `$0007` | sector number |
| Data buffer | `$0300-$03FF` | 256-byte sector after READ |

The FDC scans the queue about every 10 ms, performs motor/seek/read/GCR decode/checksum itself,
and replaces the job code with status:

| Code | Meaning |
|---|---|
| `$01` | OK |
| `$02` | header block not found |
| `$03` | no sync character |
| `$04` | data block not present |
| `$05` | data checksum error |
| `$07` | write verify error |
| `$08` | write protect |
| `$09` | header checksum error |
| `$0B` | disk ID mismatch |

`$80` or higher means job pending. This path uploads no executable code and never touches the
stepper directly. A READ job matches a header at the **current head position**; it does not move
the head. `SurfaceScan.Scan` therefore queues an explicit `$B0` SEEK for every track. Without it,
a scan of tracks 19–22 read 0/76 clean sectors; after an explicit SEEK, 76/76 were clean.

---

## 6. NBZ decoding

`Core/Nbz.cs` implements the Marcus Geelnard LZ77 decompressor for preservation `.nbz` images:

- byte 0 = marker;
- any other byte = literal;
- marker + `0x00` = literal marker byte;
- otherwise marker followed by two 7-bit variable-length integers: **length** then **offset**,
  copying `length` bytes from `output.Length - offset` (overlapping copies allowed).

The result is a standard MNIB-1541-RAW NIB: 256-byte header with magic `"MNIB-1541-RAW"`, version
at `$0D`, 120 `(half_track, density)` pairs from `$10`, then 8192-byte raw GCR blocks per present
track. `NibImage.TryLoad()` / `ToD64Bytes()` decode to D64, so `--convert in.nbz out.d64` works.

Batch decoding of the Kracker Jax archive found:

- side 2 `TGREAD` — reads five tail-gap bytes (section 8);
- volume 3 — routine referring to header gap set to `$0A`;
- no literal "fat track" text.

---

## 7. TGREAD tail-gap reader

The Kracker Jax "Revealed Trilogy" side 2 contains a C64 BASIC loader whose DATA statements
assemble a 6502 program at drive RAM `$0500`. The original host PRG is not committed; the
extracted drive-code fragment is preserved as `samples/tgread-drive.bin`.

Original behavior:

1. Write `JMP $0524` into `$0300` (buffer 0 start — FDC JUMP vector).
2. Copy track/sector params from `$0203/$0204` into zero page `$06/$07`.
3. Wait on `$00`, return.
4. FDC jumps to `$0524`, calls ROM `$F510` (read header), waits for SYNC via `$1C00`, reads
   `$1C01` with VIA handshaking, stores five bytes at `$0300-$0304`, calls `$F98F` (motor off),
   sets `$00=1`, returns.

Original setup bytes:

```asm
78        sei
A9 4C     lda #$4C
8D 00 03  sta $0300
A9 24     lda #$24
8D 01 03  sta $0301
A9 05     lda #$05
8D 02 03  sta $0302        ; JMP $0524
AD 03 02  lda $0203
85 06     sta $06
AD 04 02  lda $0204
85 07     sta $07
A9 E0     lda #$E0
85 00     sta $00
58        cli
A5 00     lda $00
30 FC     bmi *-4
60        rts
```

FDC routine excerpt:

```asm
78        sei
20 10 F5  jsr $F510        ; ROM: read header
2C 00 1C  bit $1C00
10 FB     bpl *-5          ; wait SYNC low
2C 00 1C  bit $1C00
30 FB     bmi *-5          ; wait SYNC high
AD 01 1C  lda $1C01        ; read GCR byte
B8        clv
A2 01     ldx #$01
A0 45     ldy #$45
50 FE     bvc *-2
B8        clv
AD 01 1C  lda $1C01
88        dey
D0 F7     bne *-9
CA        dex
10 F4     bpl *-14
```

Lesson: ROM calls are legal only in FDC context via a JUMP job; the same calls via M-E from the IP
wedged the drive.

### Our port

`TailGap.port.s/.bin`, embedded as `Assets/tailgap.bin`, is implemented in `Core/TailGap.cs`:

1. Upload the 87-byte routine to `$0500`.
2. Write `4C 00 05` (`JMP $0500`) into `$0300`.
3. Write job code `$D0` (JUMP) into `$0000`.
4. Wait for the FDC.
5. Download six bytes from `$0300`: five gap bytes plus status at `$0305`.

The port keeps the JUMP-job structure but reads VIA2 directly (PCR `$EF`, IFR bit 1,
`$1C01` data) instead of calling ROM. It stores the five bytes at `$0300-$0304`, overwriting the
consumed JMP vector, and signals `$A5` at `$0305`.

Hardware result: gap bytes `60 24 46 23 99`, drive status `00, ok,00,00`.

---

## 8. Dynamic drive-CPU trace

`tools/diagnostics/launch-x64sc-g64.sh` attaches the preservation G64 to emulated drive 8 with
`-drive8truedrive`; `binmon_drive.py` sets a checkpoint on execution in drive RAM `$0300-$07FF`
and logs `PC/A/X/Y`.

Trace highlights:

| PC | Instruction | Meaning |
|---|---|---|
| `$0300` | `SEI` | uploaded fastloader entry |
| `$0301-$0313` | writes `$4C/$CC/$04` into `$07E0` and `$07E3` | installs `JMP $04CC` stubs |
| `$0315-$0319` | `ROR $0300,x ; INX ; CPX #$16` | rotates first 22 bytes to hide the stub |
| `$0322` | `JSR $03C6` | ROM-ish setup |
| `$0325-$0327` | `LDX #$12 ; JSR $0575` | counter setup |
| `$0506-$0517` | `CLC ; LDA $1C00 ; AND #$FC ; STA $1C00` | VIA2 phase manipulation |
| `$0520-$0525` | `DEX ; BNE ; DEC $C1 ; BNE ; RTS` | hot delay/retry loop (~14,500 hits) |
| `$0575-$0577` | `LDA #$0A ; STA $0614` | retry counter = 10 |

Per-instruction checkpointing is too slow for the loader to finish. The non-stopping
`binmon_history.py` samples VICE's CPU-history API; with it, the protected game loads completely
to the "Enemy Review" screen, proving the G64 + true-drive-emulation setup is correct.

With the PC filter `$0300-$07FF`, the stream is dominated by:

```asm
$0403  2C 00 18  BIT $1800     ; VIA1 Port B: IEC lines
$0406  D0 FB     BNE $0403     ; wait for C64 to change the line
```

This is the fastloader's IEC handshake loop. High-level OpenCBM cannot emulate this bit-banging,
which is why real-device passthrough fails for protected games.

---

## 9. Raw contents of protected tracks

Byte histograms from the preservation G64:

| Track | Bytes | Dominant bytes | Interpretation |
|---|---|---|---|
| 2 | 6,666 | `AF`×2736, `5B`×1028, `99`×1024, `AD`×813, `CC`×810, `00`×206 | No `$FF` sync at all. Repeating GCR pattern → FDC reports "no sync character" for every sector. |
| 36 | 7,015 | `$FF`×2252, `6F`×617, `55`×460, `AA`×270 | 469 `$FF` sync runs but zero decodable sector headers — sync marks plus filler. |
| 37–40 | ~6,249 | `6F`/`DE`/`BD`/`ED` repeating | Structured GCR noise with occasional sync, no valid headers/data blocks. |

These are deliberately constructed patterns, not random damage. `--cbm gaps` reports them as
sync/sector-count anomalies.

---

## 10. Reproducing protected tracks

`Core/ProtectedTracks.cs` plus `--protect <base.d64> <out.g64>` generate the patterns from scratch
and inject them via `G64Image.RawTrackOverrides`:

```bash
dotnet run --project D64Editor.App -- --protect docs/disk-images/af-g64.d64 /tmp/repro.g64
dotnet run --project D64Editor.App -- --cbm gaps /tmp/repro.g64
```

- **T2:** 6,666 bytes of alternating no-sync GCR pairs (`CC AD`, `AF AF`, `99 5B`, `5B 99`,
  `AD CC`) — no `$FF` anywhere.
- **T36:** 7,015 bytes, 15-byte period `6F×6 / FF×6 / 55×3` → 468 sync runs (original: 469).
- **T37–40:** ~6,250 bytes, `fill×180 / FF×5` → 33 sync runs each (original: 30–33).

`--cbm gaps` confirms T2 0 sync/0 sectors, T36 468/0, T37–40 33/0. This reproduces the protection
pattern in a G64; writing it to real media needs the raw-track writer below.

---

## 11. Serial raw-track writer (DESTRUCTIVE)

`RawWriter.s/.bin` and `Core/RawWriter.cs` implement raw GCR writing over a plain IEC serial
cable, with no parallel nibbler. The host uploads the pattern, so it never streams 6 KB in real
time:

1. Seek target track with FDC SEEK job.
2. Upload pattern to `$0400`, byte count to `$03F0/$03F1`, mask to `$03F2`, status `$00` to
   `$03F3`, routine to `$0500`.
3. Launch with M-E `$0500`.
4. Poll `$03F3` for `$A5`.

Drive-side sequence:

```asm
LDA #$FF / STA $1C03          ; port A output
LDA $1C0C / AND #$1F / ORA #$C0 / STA $1C0C   ; CB2 low = write mode
; per byte:
BVC                           ; wait byte-ready / 6502 SO pin
CLV
LDA pattern,y / STA $1C01
; after count:
LDA $1C0C / ORA #$E0 / STA $1C0C              ; read mode
LDA #$00 / STA $1C03                          ; port A input
```

CLI:

```bash
dotnet run --project D64Editor.App -- --writeraw 8 2 CCAD 3333
# pattern must be a power-of-two number of hex bytes; count = bytes written
```

For T36–40, `RawWriter` first seeks to T35, then runs the 82-byte `Assets/Stepper.s` at `$0600`:
one phase increment at a time (two per track), ~20 ms delay, speed-zone bits cleared with
`AND #$9F`, status `$A5` at `$03F8`. No homing, no zero-page writes.

### Hardware verification

On 2026-11-19, after formatting an expendable disk `N0:KK,KK`:

```bash
dotnet run --project D64Editor.App -- --writeraw 8 2 CCAD 6666
dotnet run --project D64Editor.App -- --cbm surface 8 --from 2 --to 2
```

Result: `RAW WRITE OK`; all 21 sectors on T2 returned FDC `$03` "no sync character" — the same
functional signature as the original EA disk. Drive status remained `00, ok,00,00`.

`--cbm gcrscan` samples of the reproduced track do **not** byte-match the original G64 T2 and
differ between reads: without `$FF` sync, the sampler cannot lock byte alignment. The meaningful
comparison is functional (0 sync runs / 0 decodable sectors).

The T36–40 stepper path is code-complete but **untested on hardware**.

---

## 12. Raw GCR reader (in progress)

`GcrReader.s/.bin` + `Core/GcrRead.cs` + `--gcrread` read VIA2 port A using the 6502 SO byte-ready
signal, the same loop as the fastloader. Two hardware details were required:

- Motor on must go through DOS `$F97E`; raw `ORA #$04` does not arm the motor-off timer and the
  drive does not spin reliably.
- A spin-up delay is needed before sampling.

The first attempt used 256-byte chunks. A 30-window `--whole` capture re-locked to SYNC and
skipped successive byte counts, producing 19 sync runs and 0 decodable sectors on T1 — not a valid
single-revolution stream.

The current reader uses 1 KB chunks with 512-byte overlap and reverse-searched prefix matching
via `GcrRead.Stitch()`. `--test-stitch <image.g64> <track>` validates against known G64 tracks:
T1 (7,692 bytes), T2 (6,666), and T36 (7,015) reconstruct to exact rotations. This is synthetic
validation with perfect windows; real-hardware drift remains untested. SRQ nibtools remains the
authoritative full-track imager.

---

## 13. RUSSIA (ID `GM`) protection chain — 1541 mode

A second protected disk investigated on device 8 (a 1571 switched to 1541 mode with `U0>M0`)
uses a much simpler trick than EA PRODOS: **one deliberately unreadable sector** that the loader
checks through a direct-access DOS command. This matters because a normal D64 copy silently
reconstructs the sector and the protection disappears.

### Disk and files

- Directory name `"RUSSIA"`, ID `GM`, 40 tracks, 20 PRGs, 202 blocks free.
- `NIB → D64` decoding leaves **85 undecoded sectors**; a D64 is not a valid preservation master.
- Files: `RUS`, `CA`, `S0`–`S3`, `MAPN`, `G0`–`G4`, `CD`, `SV`, `C0`, `C1`, `U1`, `U2`, `CF`, `ST`.
- Surface scan (tracks 1–42): 802 sectors, 412 clean, 120 FDC errors, 270 suspicious.
  `T18/S18` reports FDC `$05` (data checksum error); tracks 36–42 have no sync.
- The physically flipped side is blank/unformatted: 0 clean, 802 FDC errors (787 `$03`, 15 `$09`).

### Loader sequence

`rus.prg` is a BASIC loader. Its essential commands are:

```basic
OPEN 15,8,15
PRINT#15,"M-W"CHR$(106)CHR$(0)CHR$(1)CHR$(133)
LOAD "ST",8,1
SYS (12032)
```

`CHR$(106)CHR$(0)` is drive address **$006A = REVCNT** (read-retry counter, normally 5). Setting
it to `$85` keeps the retry count at 5 but sets bit 7, which disables the drive's head-bump /
recalibration on repeated read errors. The damaged sector must stay damaged; an automatic recovery
would defeat the check.

`st.prg` then issues **`U1:5,0,18,18`** — a raw read of track 18, sector 18 into direct channel 5.
There is no `M-E` and no uploaded drive code. (`U1`/`U2` in `cd.prg` are *file names*, not commands.)
The loader expects DOS status `23,read error,18,18`:

```bash
cbmctrl open 8 5 '#'
cbmctrl command 8 'U1:5,0,18,18'
cbmctrl status 8      # 23,read error,18,18
cbmctrl close 8 5
```

Adjacent sectors read fine, and `--cbm sector 8 18 18` reports FDC code `$05`
(data checksum error). `Core/OpenCbm.ReadSectorDirect()` performs this sequence with the channel
close in a `finally`, so a wedged read cannot leave the IEC channel open. It is exposed as
`--cbm u1 8 18 18` (CLI), `u1 18 18` (console) and `cbmu1 8 18 18` (script); all print the drive
status, which should be `23,read error,18,18` on a healthy copy of this disk.

Manual `M-W` details that bit us here:

- The canonical DOS form is **`M-W:100 0 1 <value>`** (colon required when the command is one
  string); with `cbmctrl -p command -e`, address and bytes must be **separate raw-byte arguments**.
  A single quoted `"M-W100,0,1,8"` sends ASCII digits and changes nothing.
- Even with correct syntax, bare `M-W` is **intermittently dropped** by the real drive. Use
  `cbmctrl upload 8 0x0064 <file>` (as `FastSeek.Set` does) when the write must be reliable.

### Captures and playback

```bash
D64Editor --cbm preserve 8 /home/pat/disk8-protected.nib --mode 1541 \
  --report /home/pat/disk8-protected-report.txt --g64 /home/pat/disk8-protected.g64
```

The integrated report found 34 full-track CBM-OK captures, 41 half-track captures,
10 unformatted reads, 118 non-standard sector markers, 37 weak-GCR events, 7 long reads and
35 NDOS tracks. `nibread` prints all tracks as `N.0`, so only entries ending in `.5` are real
half-tracks. Repeating a capture gives different bytes on weak/protected tracks — expected
nondeterminism, not corruption.

Playing the **real disk** through `x64sc` with `-devicebackend8 2 -busdevice8` and typing lowercase
`load"*",8,1` (VICE's PETSCII keymap) reaches the Rusíia title screen after about two minutes.
Stop VICE and run `cbmctrl reset` before any `cbmctrl`/`nibread` access to the same bus.

Playback check of the **G64 derivative** (2026-11-24): the program runs briefly and then the C64
returns to `READY` instead of staying on the title screen. `G64 → D64` leaves 21 undecoded sectors
where `NIB → D64` leaves 85, so `nibconv` changes some raw tracks. However, a GCR decode of T18
shows the deliberately bad S18 data checksum **is** preserved in every tested G64 variant, so the
failure is not simply that nibconv "fixed" that sector. The NIB's T18 has 22 header blocks
(duplicates of S02–S04); the G64 has 19 — one per sector. Rotation/timing-sensitive checks and
REVCNT/head-bump behaviour are other likely differences. Treat the NIB as the authoritative
master; for this disk the G64 is only a best-effort playable derivative, not a faithful
preservation copy. Real-drive passthrough remains the verified way to play it.

---

## 14. ROM fast stepping (MINSTP) and bulk scans

The 1541/1571 ROM has a factory-calibrated accel/run/decel stepper ramp, but its threshold
`MINSTP` at drive RAM **`$0064`** ships as `$C8` (200 half-steps) — larger than any seek on a
35-track disk — so it never activates. Lowering it to `$0C` (12 half-steps, ~6 tracks) makes
ordinary seeks use the calibrated ramp (AS=4, AF=4, VIA2 Timer1 high byte `$20` → `$10`).

| MINSTP | avg / seek, T18↔T30 | 8 seeks |
|---|---|---|
| `$C8` (default) | 366 ms | 2931 ms |
| `$0C` (fast) | **270 ms** | 2157 ms |

About **35% faster** (~96 ms saved per seek) on a real 1571. Only the threshold byte changes —
no uploaded stepper code and no direct VIA phase writes — so the ROM still handles acceleration,
deceleration and settling. Drive reset restores `$C8`.

`--cbm fastseek 8 on|off|status`, `--cbm seek 8 30 --fast` and `--cbm benchseek 8 18 30 8
[--fast]` expose this from the CLI; the console and `cbmfastseek` script command mirror them.
`SurfaceScan.Scan(fastSeek: true)` and `--cbm surface ... --fast` / `--cbm preserve ... --fast`
wrap the whole scan and restore the prior value in a `finally`.

Measured effect on a **surface scan** (76 clean sectors, tracks 19–22, real 1571): 62.67 s normal
vs 62.48 s fast — only ~0.3% better. Per-sector FDC READ jobs dominate; fast stepping is useful
only for seek-heavy loops (`benchseek`), not for full-surface scans. The option is kept for
completeness and restores MINSTP reliably in both cases.

**Implementation note:** `FastSeek.Set` writes with `cbmctrl upload` and verifies by read-back.
Bare `M-W` was dropped often enough to make `fastseek on` fail verification, even at unrelated
RAM addresses; upload has been reliable.

---

## Safety

- **Never run `--writeraw` on an original disk.** Blank/expendable media only.
- One process owns the XUM1541/IEC bus. Stop VICE before `cbmctrl`/`d64copy`/`nibread`; call
  `cbmctrl reset` afterward.
- Do not call ROM disk routines via M-E; use FDC JUMP jobs.
- Do not resurrect uploaded homing/stepper experiments.
- Custom motor control requires `$F97E` plus spin-up delay.
- Faithful protected images require SRQ raw GCR capture; D64 → G64 loses protection.
- Every OpenCBM tool invocation has a hard timeout; interactive DOS commands default to 30 s.
  A wedged U1 read is killed rather than blocking the bus forever, and direct channels are closed
  in `finally`. Do not remove the timeout from `Core/OpenCbm.Run`.
