* = $0300

status = $0300
phase = $0301
delta = $03F0
zone = $03F1
buffer = $0400

start:
        sei
        lda #$00
        sta status
        sta phase
        lda $1C00
        ora #$04
        sta $1C00

        lda delta
        beq nostep
        bpl stepinpath
        lda delta
        eor #$FF
        clc
        adc #$01
        tax
stepoutloop:
        jsr phaseout
        jsr phaseout
        dex
        bne stepoutloop
        jmp nostep
stepinpath:
        tax
stepinloop:
        jsr phasein
        jsr phasein
        dex
        bne stepinloop
nostep:
        lda $1C00
        and #$9F
        ora zone
        ora #$04
        sta $1C00

        lda #$EF
        sta $1C0C
        lda #$02
        sta $1C0D
        lda #$00
        sta $1C03
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

phasein:
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
        jsr setphase
        rts

phaseout:
        lda $1C00
        and #$03
        beq po2
        sec
        sbc #$01
        jmp po3
po2:
        lda #$03
po3:
        jsr setphase
        rts

setphase:
        pha
        lda $1C00
        and #$FC
        sta $1C00
        pla
        ora $1C00
        sta $1C00
        ldy #$10
sd1:
        ldx #$00
sd2:
        dex
        bne sd2
        dey
        bne sd1
        rts
