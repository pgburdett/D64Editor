* = $0500

pattern = $0400
countlo = $03F0
counthi = $03F1
mask = $03F2
status = $03F3
idx = $03F4
lo = $03F5
hi = $03F6

start:
        sei
        lda #$FF
        sta $1C03
        lda $1C0C
        and #$1F
        ora #$C0
        sta $1C0C
        lda #$00
        sta status
        sta idx
        lda countlo
        sta lo
        lda counthi
        sta hi

loop:
        bvc loop
        clv
        ldy idx
        lda pattern,y
        sta $1C01
        lda idx
        clc
        adc #$01
        and mask
        sta idx
        dec lo
        bne loop
        dec hi
        bne loop

finishwait:
        bvc finishwait
        clv
        lda $1C0C
        ora #$E0
        sta $1C0C
        lda #$00
        sta $1C03
        lda #$A5
        sta status
        cli
        rts
