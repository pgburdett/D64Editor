# 6502 drive-code reference (detailed)

This folder documents the 6502 code that has been uploaded into the 1541/1571 drive RAM
while developing raw GCR access. All files are reference material; only
`../../D64Editor.Core/Assets/GcrSampler.s` is built into the application (embedded as
`gcrsampler.bin` via `D64Editor.Core.csproj`).

Assemble with `xa`:

```bash
xa -o out.bin source.s
```

Disassemble with `da65 --cpu 6502 --start-addr 0x0300 out.bin`.

---

## 1. Drive hardware used by these routines

The 1541/1571 has two MOS 6522 VIAs. VIA2 controls the disk mechanism; VIA1 controls the
IEC bus (and is what fast loaders abuse).

### VIA2 registers (disk side)

| Address | Register | Meaning used here |
|---|---|---|
| `$1C00` | Port B (DSKCNT) | bit 7 = SYNC (0 means SYNC found); bits 6-5 = speed zone; bit 4 = write-protect; bit 3 = LED; **bit 2 = motor (1 = on)**; bits 1-0 = stepper phase (never written by the safe sampler) |
| `$1C01` | Port A | **Raw GCR data to/from the read/write head** |
| `$1C03` | DDRA | 0 = port A is input (reading) |
| `$1C0A` | Shift register | used by DOS burst modes, not by these routines |
| `$1C0C` | PCR | bits 7-5 = CB2 control, bits 3-1 = CA2 control, bit 4 = CB1 edge, bit 0 = CA1 edge |
| `$1C0D` | IFR | bit 1 = CA1 = **BYTE-READY**; writing `02` clears it |
| `$1C0E` | IER | interrupt enable (not needed — the IFR bit is polled directly) |
| `$1C0F` | Port A without handshake | unused |

### PCR value `$EF` used by the working sampler

```
$EF = 1110 1111
      ^^^  ^^^^
      CB2  CA2/CA1
```

- CB2 (bits 7-5) = `111` → **high output**. On VIA2 this is the head-mode line:
  high = **read**, low = write.
- CA2 (bits 3-1) = `111` → **high output**. On VIA2 this is SOE; high activates BYTE-READY.
- CA1 edge (bit 0) = `1` → positive edge latches BYTE-READY.

Without CA2/SOE set high, BYTE-READY never appears in the IFR — this was the cause of the
`EF` timeout in `sampler3-strobe-wrong-pcr.s`.

### VIA1 registers (IEC bus, used by the game fast loader)

`$1800` Port B: bit 7 = ATN in, bit 4 = ATN ack out, bit 3 = CLOCK out, bit 2 = CLOCK in,
bit 1 = DATA out, bit 0 = DATA in. The fast loader bit-bangs these lines.

---

## 2. Why the routines are entered with M-E

`cbmctrl command 8 'M-E' <lo> <hi>` makes the drive's Interface Processor (IP) jump to a
6502 routine in RAM. That works for code that only touches VIA registers (our sampler).

It does **not** work for the DOS disk-reading ROM routines (`$F4D1` read sector, `$F556`
wait for SYNC, `$F97E` motor on, `$FE00` switch to reading). *Inside Commodore DOS* states
that disk I/O routines must be started by placing a JUMP job code (`$D0`) in the FDC job
queue at `$0000` — the FDC (not the IP) must be the one executing them. Calling them via
M-E from the IP wedged the drive (`sync-test-rom-jump.s`) and required a power cycle.

Safe alternatives implemented in D64Editor:
- Raw VIA sampling: `GcrSampler.s` (no ROM calls).
- Decoded sector reads: `Core/SectorRead.cs` uses the **FDC job queue** (see section 6).

---

## 3. The current safe sampler — `GcrSampler.safe.s` / embedded `GcrSampler.s`

126 bytes assembled at `$0300` (drive buffer 0). It never writes stepper phase bits.

### Memory map used

| Address | Size | Purpose |
|---|---|---|
| `$0300` | 1 | Status: `00` while running, `$A5` when done |
| `$0301` | 1 | Phase code: `04` sync found, `05` 256 bytes read, `EE` sync timeout, `EF` byte-ready timeout |
| `$0400-$04FF` | 256 | Raw GCR bytes sampled from `$1C01` |

### Annotated listing

```asm
* = $0300                  ; buffer 0 in drive RAM

status = $0300
phase  = $0301
buffer = $0400

start:
        sei                 ; block IRQ so the IP cannot preempt our VIA polling
        lda #$00
        sta status          ; tell host "running"
        sta phase           ; clear phase code
        lda #$EF            ; CB2 high (read), CA2 high (SOE), CA1 positive edge
        sta $1C0C           ; write PCR
        lda #$02
        sta $1C0D           ; clear any stale BYTE-READY flag in IFR
        lda #$00
        sta $1C03           ; DDRA = 0, port A is an input
        lda $1C00
        ora #$04            ; motor on (bit 2), preserve zone/phase/other bits
        sta $1C00

        ldy #$08            ; spin-up delay: 8 * 256 * ~5 cycles ~= 10 ms
spinupl:
        ldx #$00
spinup2:
        dex
        bne spinup2
        dey
        bne spinupl

        ldy #$00            ; SYNC wait with 16-bit timeout
wait1:
        ldx #$00
wait2:
        bit $1C00           ; bit 7 -> N flag; 0 means SYNC found
        bpl found           ; branch when N=0 (SYNC present)
        dex
        bne wait2
        dey
        bne wait1
        lda #$EE            ; timeout
        sta phase
        jmp finish

found:
        lda #$04
        sta phase           ; "SYNC found" phase code
        ldy #$00
readloop:
        ldx #$00
bytop1:
        lda $1C0D           ; IFR
        and #$02            ; BYTE-READY (CA1)
        bne bytok
        dex
        bne bytop1          ; per-byte timeout (~128 us) prevents a bus wedge
        lda #$EF
        sta phase
        jmp finish
bytok:
        lda $1C01           ; read the GCR byte from VIA2 port A
        sta buffer,y
        lda #$02
        sta $1C0D           ; clear BYTE-READY for the next byte
        iny
        bne readloop        ; 256 bytes ($0400-$04FF)
        lda #$05
        sta phase

finish:
        lda $1C00
        and #$FB            ; clear motor bit (bit 2)
        sta $1C00
        lda #$A5
        sta status          ; completion marker
        cli
        rts                 ; return to the IP
```

### Host protocol (`Core/GcrScan.cs`)

1. `OpenCbm.UploadMemory(8, 0x0300, samplerBytes)` — write the 126-byte routine.
2. `OpenCbm.Execute(8, 0x0300)` → `cbmctrl command 8 'M-E' 0 3`.
3. Poll `$0300/$0301` every 150 ms until `$0300 == $A5` (30 s timeout).
4. Require `$0301 == $05`; reject `EE`/`EF` (timeout) codes.
5. `OpenCbm.DownloadMemory(8, 0x0400, 256, out raw)`.

### What valid output looks like

A real sample from track 18 began:

```
24 44 12 49 91 9C 4C E3 C9 24 44 82 28 91 24 82
24 49 F2 49 C3 29 E6 3E 61 F1 17 F9 9C 9E 79 9F
F3 C4 CF F3 99 C8 91 F3 CF 33 F7 89 F9 34 F7 89
...
FF 32 79 9C FF FC
```

`$FF` is the GCR sync byte (ten consecutive 1 bits on disk). Other bytes should obey the
GCR 4-to-5 rule (no more than two adjacent 0 bits, no adjacent 1 bits); bytes with adjacent
1 bits indicate the sampler is not byte-aligned to the data separator yet. `GcrScan` reports
the count of distinct byte values and `$FF` runs as a quick sanity check; full GCR decoding
is not yet done for hardware samples.

---

## 4. Experimental files and why they failed or succeeded

| File | Result | Root cause / lesson |
|---|---|---|
| `sync-test-rom-jump.s` | **Wedged the drive** | Called ROM `$F97E`/`$FE00`/`$F556` with M-E. Disk routines must be run by the FDC via a job-queue JUMP, never by the IP. |
| `sync2-direct-via-timeout.s` | Works | Direct VIA: PCR `$EF`, motor bit 2, SYNC via `BIT $1C00`, hard timeout, one byte from `$1C01`, status `A5 02 <byte>`. Proved the hardware path without ROM calls. |
| `sampler2-no-strobe.s` | One byte then zeros | `$1C01` was read in a tight loop without waiting for BYTE-READY. The VIA presents a new byte only on the CA1 strobe. |
| `sampler3-strobe-wrong-pcr.s` | Timeout `EF` | Polled IFR bit 1 but left CA2/SOE at the ROM's value, so BYTE-READY was never generated. Setting PCR `$EF` fixed it. |
| `sampler4-working-direct-via.s` | **Works** | Adds `lda #$EF / sta $1C0C` and `lda #$02 / sta $1C0D`. This is the basis of the current safe sampler. |
| `halfstep-one-phase.s/.bin` | used once, supervised | 46-byte parameter-free routine: reads `$1C00`, computes one phase **decrement** (outward half-track), writes the final byte in a single store, delays, returns. No zero-page writes, no homing. Used to move from track 35 to 35.5 for a GCR comparison. |
| `halfstep-in-one-phase.s/.bin` | used once, supervised | 48-byte inverse routine: one phase **increment** (inward half-track). Used to return from 35.5 to track 35; verified by a successful READ of track 35 sector 0. Note: `inc` is a reserved opcode name in `xa` — the label is `incr`. |
| `game-fastloader-disassembly.asm` | Reference | 1,280 bytes of 6502 uploaded by the game disk to drive RAM `$0300-$07FF` via 40 `M-W` commands, then executed with `M-E`. It disables interrupts, installs a `JMP $04CC` stub at `$07E0`, rotates its first 22 bytes to hide the stub, then bit-bangs VIA1 `$1800` and decodes nibbles through a table at `$0413`. This is why a high-level OpenCBM backend cannot load such disks. |

---

## 5. Stepper experiments (abandoned)

Several versions tried to move the head from uploaded code. All were removed after head
clatter. Correct facts from *Inside Commodore DOS* (see `../inside-commodore-dos.txt`):

- One phase change of bits 1-0 of `$1C00` moves **half a track**; on-track phase is always
  even (0 or 2).
- Inward: `00 -> 01 -> 10 -> 11 -> 00`. Outward: `00 -> 11 -> 10 -> 01 -> 00`.
- Speed zone bits 6-5: tracks 1-17 `$60`, 18-24 `$40`, 25-30 `$20`, 31-35 `$00`.
- A version doing four phase changes per "track" moved two tracks per step. A later version
  stored its delta in zero-page `$0003`, corrupting DOS workspace. Another overlapped its
  parameter bytes with the tail of the code image. Each produced unpredictable step counts.

**Do not resurrect uploaded stepper code.** Use the FDC job queue instead.

---

## 6. The safe way to read a sector — FDC job queue

Implemented in `../../D64Editor.Core/SectorRead.cs` and exposed as
`--cbm sector <dev> <track> <sector> [outfile]` (READ) and
`--cbm seek <dev> <track>` (SEEK, job code `$B0`, track in `$0006`, job in `$0000`).

Buffer 0 job interface:

| Purpose | Address | Value for READ |
|---|---|---|
| Job queue slot 0 | `$0000` | `$80` (READ) |
| Track (header) | `$0006` | track number |
| Sector (header) | `$0007` | sector number |
| Data buffer | `$0300-$03FF` | 256-byte sector after read |

The FDC scans the queue about every 10 ms, performs motor/seek/read/GCR-decode/checksum
itself, and replaces the job code with an FDC status code:

| FDC code | Meaning |
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

Codes `$80` and above mean the job is still pending. This path never uploads executable code
and never touches the stepper directly — the drive firmware owns the head movement.

---

## 7. NBZ decoding — how the Kracker Jax disks were opened

`../../D64Editor.Core/Nbz.cs` implements the Marcus Geelnard LZ77 decompressor used by
preservation-project `.nbz` files:

- byte 0 of the file is the **marker**;
- a literal is any byte other than the marker;
- marker followed by `0x00` means a literal marker byte;
- otherwise the marker is followed by two 7-bit variable-length integers: **length** then
  **offset**, and `length` bytes are copied from `output.Length - offset` (overlapping copies
  are allowed, which is how long runs are encoded).

Decompressing an `.nbz` yields a standard **MNIB-1541-RAW NIB** image: a 256-byte header
with magic `"MNIB-1541-RAW"`, version at `$0D`, then 120 `(half_track, density)` pairs from
`$10` onward, followed by 8192-byte raw GCR blocks per present track. `NibImage.TryLoad()`
and `ToD64Bytes()` already decode that to a D64, so `--convert in.nbz out.d64` now works.

This unlocked the Kracker Jax "Revealed Trilogy" disks. A batch over all 10 `.nbz` files found:
- side 2 `TGREAD` — reads 5 tail-gap bytes (port described below);
- vol 3 — a routine referring to a header gap set to `$0A`;
- no literal "fat track" text. See `../HARDWARE_WORKFLOW.md` for the search summary.

---

## 8. TGREAD port — tail-gap reading from FDC context

### Original (Kracker Jax "Revealed Trilogy" side 2)

`tgread-original-host.prg` is the C64 BASIC loader. Its DATA statements contain a 6502
program loaded at drive RAM `$0500` (`tgread-original.bin`, disassembled below). It:

1. writes `JMP $0524` into `$0300` (buffer 0 start — the FDC JUMP vector);
2. copies track/sector parameters from `$0203/$0204` into zero page `$06/$07`;
3. waits on `$00`, then returns;
4. the FDC jumps to `$0524`, which calls ROM `$F510` (read header), waits for SYNC via
   `$1C00`, reads bytes from `$1C01` using VIA handshaking, stores five bytes at
   `$0300-$0304`, calls `$F98F` (motor off), sets `$00=1` and returns.

```asm
; original setup at $0500
78        sei
A9 4C     lda #$4C
8D 00 03  sta $0300        ; JMP opcode
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

; original FDC routine at $0524
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
...       ; five more bytes into $0300-$0304
20 8F F9  jsr $F98F        ; motor off
A9 01     lda #$01
85 00     sta $00
58        cli
60        rts
```

The important lesson: those ROM calls are legal **only** because the routine runs in FDC
context via a JUMP job. The same calls via M-E from the IP wedged the drive
(`sync-test-rom-jump.s`).

### Our port (`TailGap.port.s/.bin`, embedded as `Assets/tailgap.bin`)

`../../D64Editor.Core/TailGap.cs` and `--cbm tailgap <dev> [outfile]`:

1. upload the 87-byte routine to drive RAM `$0500`;
2. write `4C 00 05` (`JMP $0500`) to `$0300` — the buffer-0 jump vector;
3. write job code `$D0` (JUMP) to `$0000`;
4. wait for the FDC to finish;
5. download six bytes from `$0300`: five gap bytes plus status at `$0305`.

The port keeps the JUMP-job structure but reads VIA2 directly (PCR `$EF`, IFR bit 1
BYTE-READY, `$1C01` data) instead of calling ROM routines. It stores the five gap bytes at
`$0300-$0304` (overwriting the now-consumed JMP vector) and signals `$A5` at `$0305`.
Verified on hardware: gap bytes `60 24 46 23 99`, drive status `00, ok,00,00`.

### C host probes

`c-probes/` contains the four standalone OpenCBM C programs used to diagnose the VICE LOAD
bug and the XUM1541 burst limitation; see `c-probes/README.md`.

---

## 9. Dynamic drive-CPU trace (Arctic Fox fastloader)

`../../tools/diagnostics/` contains the launchers and VICE binary-monitor clients used to
trace the loader at runtime. `launch-x64sc-g64.sh` attaches the Arctic Fox preservation G64 to
emulated drive 8 with `-drive8truedrive`, and `binmon_drive.py` sets a checkpoint on execution
in drive RAM `$0300-$07FF`, logging `PC/A/X/Y` on each hit.

The resulting trace (`/tmp/opencode/g64-mon.log`, ~29k lines) executed exactly the same
fastloader family we disassembled from the other protected game disk:

| PC | Instruction (from monitor) | Meaning |
|---|---|---|
| `$0300` | `SEI` | uploaded fastloader entry |
| `$0301-$0313` | `LDA #$4C; STA $07E0; STA $07E3; LDA #$CC; STA $07E1; LDA #$04; STA $07E2; LDX #$00` | installs `JMP $04CC` stubs |
| `$0315-$0319` | `ROR $0300,x; INX; CPX #$16` | rotates first 22 bytes to hide the stub |
| `$0322` | `JSR $03C6` | ROM-ish setup routine |
| `$0325-$0327` | `LDX #$12; JSR $0575` | sets a counter |
| `$0506-$0517` | `CLC; LDA $1C00; AND #$FC; STA $1C00` | VIA2 stepper/phase manipulation |
| `$0520-$0525` | `DEX; BNE $0520; DEC $C1; BNE $0523; RTS` | **hot loop** where the trace spent ~14,500 hits |
| `$0575-$0577` | `LDA #$0A; STA $0614` | sets a retry/timeout counter to 10 |
| `$0582-$0583` | `DEY; BNE` | inner byte loop |

So the drive CPU really does execute the custom fastloader (true drive emulation), and the
trace settles into its delay/retry loop — consistent with the loader waiting for IEC handshake
behaviour that the host side isn't satisfying in this configuration. No explicit "compare gap
bytes" branch was reached in this run; the earlier static `TGREAD` evidence remains the best
documentation of gap reading.

A **non-stopping** tracer (`tools/diagnostics/binmon_history.py`) was then used: it samples
VICE's CPU-history API instead of stopping at every instruction. With that, the protected game
loads completely to the "Enemy Review" screen, proving the G64 + true-drive-emulation setup is
functionally correct. Per-instruction stopping is far too slow to let the loader finish.

With the PC filter enabled (`$0300-$07FF`), the sampled drive-code stream is dominated by just
two instructions:

```
$0403  2C 00 18  BIT $1800     ; VIA1 Port B: IEC ATN/CLOCK/DATA lines
$0406  D0 FB     BNE $0403     ; wait for the C64 to change the line
```

That is the fastloader's IEC handshake loop from the original disassembly
(`L0403: BIT $1800 / BNE L0403`). It confirms the drive side spends its time waiting for the
host to drive the serial lines — the exact boundary where custom fastloader timing meets VICE's
emulated bus. With both CPUs emulated and `-drive8truedrive`, the handshake completes and the
game loads.

See `tools/diagnostics/README.md` for the full script list and a reproducible trace session.

## 10. Raw contents of the protected tracks (Arctic Fox G64)

Byte histograms of the G64 raw track data explain why the DOS reads fail:

| Track | Bytes | Dominant bytes | Interpretation |
|---|---|---|---|
| 2 | 6,666 | `AF`×2736, `5B`×1028, `99`×1024, `AD`×813, `CC`×810, `00`×206 | No `$FF` sync bytes at all. A repeating GCR pattern (`CC AD ...`), so the FDC reports "no sync character" for every sector. A copier that reformats this as a blank track would insert sync and fail the original-media check. |
| 36 | 7,015 | `$FF`×2252, `6F`×617, `55`×460, `AA`×270 | 469 `$FF` sync runs but zero decodable sector headers — a track largely made of sync marks and filler. |
| 37-40 | ~6,249 | `6F`/`DE`/`BD`/`ED` repeating | Structured GCR noise with occasional sync, but no valid headers/data blocks. |

These are not random damaged tracks: they carry deliberately constructed GCR patterns. Together
with the voided track 2, they form the physical-media signature the loader can verify with
drive-side code but a D64/NIB sector copy cannot reproduce. The `--cbm gaps` analyzer reports
these as sync/sector-count anomalies.
