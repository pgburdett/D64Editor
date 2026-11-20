* = $0300
        sei
        lda #$00
        sta $0300
        sta $0301
         lda #$EF
        sta $1C0C
        lda #$02
        sta $1C0D
        lda #$00
        sta $1C03
        lda $1C00
        ora #$04
        sta $1C00
        ldy #$08
spinupl:
        ldx #$00
spinup2:
        dex
        bne spinup2
        dey
        bne spinupl
        ldy #$00
wait1:
        ldx #$00
wait2:
        bit $1C00
        bpl found
        dex
        bne wait2
        dey
        bne wait1
        lda #$EE
        sta $0301
        jmp finish
found:
        lda #$01
        sta $0301
        ldy #$00
readloop:
        ldx #$00
bytop1:
        lda $1C0D
        and #$02
        bne bytok
        dex
        bne bytop1
        lda #$EF
        sta $0301
        jmp finish
bytok:
        lda $1C01
        sta $0400,y
        lda #$02
        sta $1C0D
        iny
        bne readloop
        lda #$02
        sta $0301
finish:
        lda $1C00
        and #$FB
        sta $1C00
        lda #$A5
        sta $0300
        cli
        rts
