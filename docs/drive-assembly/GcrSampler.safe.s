* = $0300

status = $0300
phase = $0301
buffer = $0400

start:
        sei
        lda #$00
        sta status
        sta phase
        lda #$EF
        sta $1C0C
        lda #$02
        sta $1C0D
        lda #$00
        sta $1C03
        lda $1C00
        ora #$04
        sta $1C00
        ldy #$20
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
        sta phase
        jmp finish
found:
        lda #$04
        sta phase
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
        sta phase
        jmp finish
bytok:
        lda $1C01
        sta buffer,y
        lda #$02
        sta $1C0D
        iny
        bne readloop
        lda #$05
        sta phase
finish:
        lda $1C00
        and #$FB
        sta $1C00
        lda #$A5
        sta status
        cli
        rts
