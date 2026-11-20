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
`--cbm sector <dev> <track> <sector> [outfile]`.

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
