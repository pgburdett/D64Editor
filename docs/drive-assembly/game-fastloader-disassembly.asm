; da65 V2.18 - Git ece63f0
; Created:    2026-11-18 22:31:46
; Input file: /tmp/opencode/fastloader.bin
; Page:       1


        .setcpu "6502"

L01BB           := $01BB
L6040           := $6040
LEB22           := $EB22
LF497           := $F497
LF5E9           := $F5E9
LF5F2           := $F5F2
LF78F           := $F78F
LF8E0           := $F8E0
LF934           := $F934
LFFFC           := $FFFC
L0300:  sei
        lda     #$4C
        sta     L07E0
        sta     L07E3
        lda     #$CC
        sta     L07E1
        lda     #$04
        sta     L07E2
        ldx     #$00
L0315:  ror     L0300,x
        inx
        cpx     #$16
        bne     L0315
        lda     #$01
        sta     $1800
        jsr     L03C6
        ldx     #$12
        jsr     L0575
        lda     #$00
        sta     L07E6
        jmp     L0368

L0332:  lda     #$00
        sta     L07E6
L0337:  jsr     L046C
        sta     $11
        pha
        jsr     L046C
        sta     $06
        pla
        eor     $06
        pha
        jsr     L046C
        sta     $07
        pla
        eor     $07
        pha
        jsr     L046C
        sta     L07E9
        pla
        cmp     L07E9
        beq     L0363
        lda     #$A1
        jsr     L0485
        jmp     L0337

L0363:  lda     #$89
        jsr     L0485
L0368:  jsr     L03C6
        jsr     L07E3
        lda     $11
        cmp     #$80
        bne     L0377
        jmp     L0726

L0377:  cmp     #$90
        bne     L037E
        jmp     L0742

L037E:  cmp     #$F0
        bne     L0385
        jmp     L06EF

L0385:  jsr     L053B
        bcs     L0385
        lda     $0200
        sta     $06
        beq     L03D4
        cmp     #$28
        bcs     L03D4
        lda     $0201
        sta     $07
        lda     #$02
        sta     $0200
        lda     #$00
        sta     $0201
        lda     L07E6
        bne     L03BD
        inc     L07E6
        lda     #$04
        sta     $0200
        lda     $0202
        jsr     L0485
        lda     $0203
        jsr     L0485
L03BD:  jsr     L048E
        jsr     L07E3
        jmp     L0385

L03C6:  lda     $1C00
        ora     #$0C
        sta     $1C00
        lda     #$EE
        sta     $1C0C
        rts

L03D4:  inc     $0201
        inc     $0201
        lda     #$02
        sta     $0200
        jsr     L048E
        lda     $22
        sta     $06
        lda     #$00
        sta     $07
        jsr     L07E3
        jsr     L053B
        lda     #$00
        sta     $0200
        jsr     L048E
L03F8:  jsr     L07C2
        jmp     L0332

L03FE:  lda     #$01
        sta     $1800
L0403:  bit     $1800
        bne     L0403
        ldx     #$08
        stx     $1800
L040D:  bit     $1800
        beq     L040D
        rts

L0413:  brk
        php
        .byte   $02
        asl     a
        .byte   $04
        .byte   $0C
        asl     $0E
        ora     ($09,x)
        .byte   $03
        .byte   $0B
        ora     $0D
        .byte   $07
        .byte   $0F
L0423:  ldy     #$00
L0425:  jsr     L03FE
L0428:  lda     #$01
L042A:  bit     $1800
        bne     L042A
        sta     $1800
        pha
        pla
        nop
        bit     $80
        lda     $1800
        asl     a
        nop
        nop
        nop
        eor     $1800
        and     #$0F
        pha
        lda     $1800
        asl     a
        nop
        nop
        ldx     #$08
        eor     $1800
        stx     $1800
        and     #$0F
        tax
        lda     L0413,x
        asl     a
        asl     a
        asl     a
        asl     a
        sta     $14
        pla
        tax
        lda     L0413,x
        ora     $14
        sta     $0200,y
        iny
        bne     L0428
        rts

L046C:  jsr     L047A
        jsr     L0425
L0472:  tax
        lda     $15
        sta     $02FF
        txa
        rts

L047A:  ldy     #$FF
        ldx     $0200,y
        stx     $15
        sta     $0200,y
        rts

L0485:  jsr     L047A
        jsr     L0490
        jmp     L0472

L048E:  ldy     #$00
L0490:  jsr     L03FE
L0493:  lda     $0200,y
        lsr     a
        lsr     a
        lsr     a
        lsr     a
        pha
        lda     $0200,y
        and     #$0F
        tax
        lda     L0413,x
        tax
        lda     #$01
        sta     $1800
L04AA:  bit     $1800
        bne     L04AA
        stx     $1800
        txa
        asl     a
        and     #$0F
        sta     $1800
        pla
        tax
        lda     L0413,x
        sta     $1800
        asl     a
        and     #$0F
        nop
        sta     $1800
        iny
        bne     L0493
        rts

L04CC:  lda     $06
        cmp     $22
        beq     L04F7
        ldx     #$00
        lda     #$FF
        bcc     L04DB
        inx
        lda     #$01
L04DB:  stx     $C3
        sta     $C0
L04DF:  ldy     #$03
        ldx     $C3
        lda     $22
        dey
        clc
        adc     $C0
        pha
L04EA:  jsr     L0506
        dey
        bne     L04EA
        pla
        sta     $22
        cmp     $06
        bne     L04DF
L04F7:  jsr     L0528
        lda     $1C00
        and     #$9F
        ora     L0537,x
        sta     $1C00
        rts

L0506:  clc
        lda     $1C00
        adc     $C0
        and     #$03
        sta     $C1
        lda     $1C00
        and     #$FC
        ora     $C1
        sta     $1C00
        lda     #$04
        sta     $C1
        ldx     #$00
L0520:  dex
        bne     L0520
        dec     $C1
        bne     L0520
        rts

L0528:  ldx     #$03
L052A:  cmp     L0533,x
        bcc     L0532
        dex
        bpl     L052A
L0532:  rts

L0533:  bit     $21
        .byte   $1D
        .byte   $12
L0537:  brk
        jsr     L6040
L053B:  lda     #$02
        sta     $31
        jsr     L0623
        bcs     L0574
L0544:  bvc     L0544
        clv
        lda     $1C01
        sta     ($30),y
        iny
        bne     L0544
        ldy     #$BA
L0551:  bvc     L0551
        clv
        lda     $1C01
        sta     $0100,y
        iny
        bne     L0551
        jsr     LF8E0
        lda     $38
        cmp     $47
        bne     L0571
        jsr     LF5E9
        cmp     $3A
        bne     L0571
        lda     #$01
        clc
        rts

L0571:  lda     #$02
        sec
L0574:  rts

L0575:  lda     #$0A
        sta     L0614
        ldy     #$02
L057C:  lda     L07E0,y
        sta     L07E3,y
        dey
        bpl     L057C
L0585:  lda     #$02
        sta     $06
        lda     #$00
        sta     $07
        jsr     L04CC
        dec     L0614
        bpl     L0598
        jmp     (LFFFC)

L0598:  lda     $1C00
        and     #$9F
        ora     #$60
        sta     $1C00
        ldx     #$00
L05A4:  dex
        bne     L05A4
        lda     $1C00
        and     #$9F
        ora     #$20
        sta     $1C00
L05B1:  inc     L0615
        bne     L05C5
        inc     L0616
        bne     L0598
        lda     #$03
        sta     $07
        jsr     L04CC
        jmp     L0585

L05C5:  bvc     L05C5
        clv
        lda     $1C01
        cmp     #$D7
        bne     L05B1
L05CF:  bvc     L05CF
        clv
        lda     $1C01
        cmp     #$D7
        beq     L05CF
        cmp     #$EB
        bne     L05B1
        ldx     #$02
        ldy     #$00
L05E1:  bvc     L05E1
        clv
        lda     $1C01
        cmp     #$CC
        bne     L05B1
L05EB:  bvc     L05EB
        clv
        lda     $1C01
        cmp     #$AD
        bne     L05B1
        dey
        bne     L05E1
        dex
        bne     L05E1
L05FB:  bvc     L05FB
        clv
        lda     $1C01
        cmp     L0617,y
        bne     L05B1
        iny
        cpy     #$0C
        bne     L05FB
        lda     #$03
        sta     $06
        lda     #$00
        sta     $07
        rts

L0614:  brk
L0615:  brk
L0616:  brk
L0617:  eor     $AE,x
        .byte   $9B
        eor     $AD,x
        eor     $CB,x
        ldx     $AB6B
        .byte   $AD
        .byte   $AF
L0623:  jsr     L064A
        bcs     L0645
        jmp     L062F

L062B:  jsr     L064A
        rts

L062F:  lda     #$D0
        sta     $1805
L0634:  bit     $1805
        bpl     L0646
        bit     $1C00
        bmi     L0634
        lda     $1C01
        clv
        ldy     #$00
        clc
L0645:  rts

L0646:  sec
        rts

        .byte   $02
        brk
L064A:  lda     L07D3
        beq     L0652
        jsr     L07D0
L0652:  lda     #$00
        sta     $08
        beq     L06A0
L0658:  lda     #$1E
        sta     L07EB
        sta     L07D1
        lda     #$52
        sta     $24
L0664:  jsr     L062F
        bcs     L0690
        ldy     #$00
L066B:  bvc     L066B
        clv
        lda     $1C01
L0671:  bvc     L0671
        clv
        lda     $1C01
        sta     $25,y
        iny
        cpy     #$07
        bne     L0671
        jsr     LF497
        ldy     #$04
        lda     #$00
L0686:  eor     $16,y
        dey
        bpl     L0686
        cmp     #$00
        beq     L06A0
L0690:  dec     L07EB
        bne     L0664
        jsr     L07D0
        dec     L07D1
        bne     L0664
        jmp     L06EB

L06A0:  lda     $06
        sta     $18
        lda     $07
        sta     $19
        lda     $16
        eor     $17
        eor     $18
        eor     $19
        sta     $1A
        jsr     LF934
        ldx     L07EC
        lda     L07ED
        sta     L07D1
L06BE:  jsr     L062F
        bcs     L06D7
        ldy     #$00
L06C5:  bvc     L06C5
        clv
        lda     $1C01
        cmp     $24,y
        bne     L06D7
        iny
        cpy     #$08
        bne     L06C5
        clc
        rts

L06D7:  dex
        bne     L06BE
        jsr     L07D0
        dec     L07D1
        bne     L06BE
        bit     $08
        bmi     L06EB
        dec     $08
        jmp     L0658

L06EB:  lda     #$03
        sec
        rts

L06EF:  ldy     #$00
L06F1:  lda     L06FF,y
        sta     L01BB,y
        iny
        cpy     #$27
        bne     L06F1
        jmp     L01BB

L06FF:  sei
        ldy     #$00
L0702:  lda     #$00
L0704:  .byte   $99
        brk
L0706:  .byte   $02
        sta     $00,y
        iny
        bne     L0704
        inc     L0706
        lda     L0706
        cmp     #$08
        bne     L0702
        lda     L07E8
        sta     $10
        ldx     #$00
        stx     $1800
        lda     #$00
        jmp     LEB22

L0726:  lda     #$0A
        sta     L07E7
L072B:  jsr     L053B
        bcc     L0739
        cmp     #$03
        beq     L0739
        dec     L07E7
        bne     L072B
L0739:  jsr     L0485
        jsr     L048E
        jmp     L03F8

L0742:  jsr     L0423
        jsr     LF5E9
        sta     $3A
        lda     $1C00
        and     #$10
        bne     L0759
        lda     #$08
        jsr     L0485
        jmp     L03F8

L0759:  jsr     LF78F
        jsr     L062B
        bcc     L0769
        lda     #$02
        jsr     L0485
        jmp     L03F8

L0769:  ldx     #$09
L076B:  bvc     L076B
        clv
        dex
        bne     L076B
        lda     #$FF
        sta     $1C03
        lda     $1C0C
        and     #$1F
        ora     #$C0
        sta     $1C0C
        lda     #$FF
        ldx     #$05
        sta     $1C01
        clv
L0788:  bvc     L0788
        clv
        dex
        bne     L0788
        ldy     #$BB
L0790:  lda     $0100,y
L0793:  bvc     L0793
        clv
        sta     $1C01
        iny
        bne     L0790
L079C:  lda     $0200,y
L079F:  bvc     L079F
        clv
        sta     $1C01
        iny
        bne     L079C
L07A8:  bvc     L07A8
        lda     $1C0C
        ora     #$E0
        sta     $1C0C
        lda     #$00
        sta     $1C03
        jsr     LF5F2
        lda     #$01
        jsr     L0485
        jmp     L03F8

L07C2:  lda     $1C00
        and     #$F3
        sta     $1C00
        lda     #$EC
        sta     $1C0C
        rts

L07D0:  rts

L07D1:  brk
        brk
L07D3:  brk
        lda     $DD00
        and     #$03
        sta     $DD00
        ldx     #$08
        lda     #$0F
L07E0:  .byte   $A0
L07E1:  .byte   $0F
L07E2:  .byte   $20
L07E3:  tsx
        .byte   $FF
        .byte   $A9
L07E6:  brk
L07E7:  .byte   $20
L07E8:  .byte   $BD
L07E9:  .byte   $FF
        .byte   $20
L07EB:  lsr     a
L07EC:  .byte   $F3
L07ED:  lda     #$00
        sta     $04
        lda     #$04
        sta     $05
        lda     #$03
        sta     $0961
        lda     #$00
        sta     $0960
        .byte   $A2
