* = $0500

status = $0305
gapbuf = $0300

entry:
        sei
        lda #$EF
        sta $1C0C
        lda #$02
        sta $1C0D
        lda #$00
        sta $1C03
        lda $1C00
        ora #$04
        sta $1C00

syncwait:
        bit $1C00
        bpl syncwait
        bit $1C00
        bmi syncwait

        ldy #$00
gaploop:
        ldx #$00
readywait:
        lda $1C0D
        and #$02
        bne gotbyte
        dex
        bne readywait
        lda #$EE
        sta status
        jmp finish
gotbyte:
        lda $1C01
        sta gapbuf,y
        lda #$02
        sta $1C0D
        iny
        cpy #$05
        bne gaploop

        lda #$A5
        sta status
finish:
        lda $1C00
        and #$FB
        sta $1C00
        cli
        rts
