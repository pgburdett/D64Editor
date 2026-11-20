* = $0500

count = $03F0
waitsync = $03F1
skiplo = $03F2
skphi = $03F3
status = $03F4
worklo = $03F5
workhi = $03F6
buffer = $0400

start:
        sei
        lda #$EF
        sta $1C0C
        lda #$02
        sta $1C0D
        lda #$00
        sta $1C03
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
        beq discard
syncwait:
        bit $1C00
        bpl discard
        jmp syncwait

discard:
        lda skiplo
        sta worklo
        lda skphi
        sta workhi
        lda #$00
        sta $03F7
disc1:
        lda worklo
        ora workhi
        beq readstart
discwait:
        bvc discwait
        clv
        lda $1C01
        dec worklo
        bne disc1
        dec workhi
        bne disc1
readstart:
        ldy #$00
loop:
        bvc loop
        clv
        lda $1C01
        sta buffer,y
        iny
        cpy count
        bne loop
        lda #$A5
        sta status
finish:
        lda $1C00
        and #$FB
        sta $1C00
        cli
        rts
