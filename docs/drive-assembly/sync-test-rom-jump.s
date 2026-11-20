* = $0300
        lda #$00
        sta $0300
        jsr $F97E
        jsr $FE00
        jsr $F556
        sta $0301
        lda #$A5
        sta $0300
        rts
