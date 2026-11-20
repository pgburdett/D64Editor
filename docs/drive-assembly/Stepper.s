* = $0600

delta = $03F7
status = $03F8
base = $03F9
phase = $03FA

start:
        lda #$00
        sta status
        lda delta
        beq done
        tax
loop:
        jsr phasein
        jsr phasein
        dex
        bne loop
done:
        lda $1C00
        and #$9F
        sta $1C00
        lda #$A5
        sta status
        rts

phasein:
        lda $1C00
        and #$FC
        sta base
        lda $1C00
        and #$03
        cmp #$03
        bne pi2
        lda #$00
        jmp pi3
pi2:
        clc
        adc #$01
pi3:
        sta phase
        lda base
        ora phase
        sta $1C00
        ldy #$10
d1:
        ldx #$00
d2:
        dex
        bne d2
        dey
        bne d1
        rts
