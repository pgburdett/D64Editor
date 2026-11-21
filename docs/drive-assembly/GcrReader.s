* = $0300

waitsync = $03F2
status = $03F3
buffer = $0400

start:
        sei
        jsr $F97E
        ldy #$40
spinupl:
        ldx #$00
spinup2:
        dex
        bne spinup2
        dey
        bne spinupl

        lda waitsync
        beq pageset
syncwait:
        bit $1C00
        bpl pageset
        jmp syncwait

pageset:
        lda #$04
        sta storehi + 2
        ldx #$04
page:
        ldy #$00
bytloop:
        bvc bytloop
        clv
        lda $1C01
storehi:
        sta $0400,y
        iny
        bne bytloop
        inc storehi + 2
        dex
        bne page

        lda #$A5
        sta status
        lda $1C00
        and #$FB
        sta $1C00
        cli
        rts
