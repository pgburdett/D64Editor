* = $0300
        lda $1C00
        and #$FC
        sta $03FE
        lda $1C00
        and #$03
        cmp #$03
        bne incr
        lda #$00
        jmp have
incr:
        clc
        adc #$01
have:
        sta $03FF
        lda $03FE
        ora $03FF
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
