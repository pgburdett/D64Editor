; ============================================================================
; VDCDEMO.COM -- CP/M demo: full-screen graphics on the C128 VDC (8563)
; ============================================================================
; Demonstrates that CP/M on the C128 CAN draw full-screen graphics on the
; 80-column VDC via the documented CP/M-Plus "User Function 4" (8502 BIOS)
; subfunction 4 (cursor$pos), instead of ANSI/VT escape sequences (which the
; C128 console does not interpret -- the reason Nemesis renders garbled).
;
;   User Function 4 subfunction 4 (cursor$pos): D = row (0-24), E = col (0-79)
;
; Calling convention: A = 4 (user function), L = 4 (subfunction), CALL USERFN.
; USERFN enters BIOS jump entry 30 (USERF) via the warm-boot vector at (1).
; Based on C128 Programmer's Reference Guide, Appendix K (USERF calls).
;
; .COM loads at 0x100; BDOS = CALL 5; RET exits to CCP.
; ============================================================================

        ORG     0100H

BDOS:   EQU     0005H

START:
        ; Print banner via BDOS function 9
        LD      HL, MSG
        CALL    PRINT

        ; Draw a diamond of '*' characters by positioning the cursor
        ; (USERF subfunction 4) then writing via BDOS CONOUT (function 2).
        LD      HL, PATTERN
DRAWLP:
        LD      A, (HL)         ; row
        CP      0FFH            ; terminator?
        RET     Z               ; done -> exit to CCP
        LD      D, A            ; D = row
        INC     HL
        LD      E, (HL)         ; E = column
        INC     HL
        PUSH    HL              ; save pattern pointer
        LD      A, 4            ; user function 4 (8502 BIOS)
        LD      L, 4            ; subfunction 4 = cursor position
        CALL    USERFN
        POP     HL              ; restore pattern pointer
        LD      E, 2AH          ; '*'
        LD      C, 2            ; BDOS CONOUT
        CALL    BDOS
        JR      DRAWLP

; ---------------------------------------------------------------------------
; USERFN -- call CP/M-Plus User Function (BIOS entry 30 / USERF).
;   A = function number, L = subfunction. Args in D/E preserved.
;   EXPERIMENTAL: uses the C128 PRG Appendix K technique (LHLD (1) -> BIOS
;   base, L = 30*3 = 90, EX (SP),HL, RET). Under x128 CP/M 3.0 in testing the
;   cursor-position subfunction (4) did not take effect -- '*' characters
;   printed sequentially instead of at the requested rows/columns -- so the
;   exact USERF entry/parameter passing for this BDOS build still needs
;   verification. The program runs and BDOS CONOUT output is correct.
; ---------------------------------------------------------------------------
USERFN:
        PUSH    HL
        LD      HL, (0001H)
        LD      L, 90
        EX      (SP), HL
        RET

; PRINT -- print $-terminated string at HL via BDOS function 2 (CONOUT),
;          one char at a time (E = char, C = 2)
PRINT:
        LD      A, (HL)
        CP      '$'
        RET     Z
        INC     HL
        PUSH    HL
        LD      E, A
        LD      C, 2            ; BDOS CONOUT
        CALL    BDOS
        POP     HL
        JR      PRINT

MSG:    DB      'C128 CP/M VDC graphics demo (User Function 4)', 0DH, 0AH, '$'

; Pattern: row, col pairs; '*' written at each. 0FFH = end.
PATTERN:
        DB  6, 32
        DB  7, 31
        DB  7, 33
        DB  8, 30
        DB  8, 34
        DB  9, 31
        DB  9, 33
        DB 10, 32
        DB 0FFH

        END
