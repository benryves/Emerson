; =========================================================
; Module: Mouse
; =========================================================
; Description:
;     Routines for handling a PS/2 mouse, optionally in
;     Intellimouse mode.
; Functions:
;     Initialise: Detect, reset, initialise the mouse.
;     Update:     Read the status of the mouse and update
;                 variables.
; Variables:
;     Position:   Position of the mouse and scrollwheel.
;     Status:     Status of the buttons and mouse mode.
; =========================================================

.module Mouse
.using Parent
; =========================================================
; Module: Mouse.Position
; =========================================================
; Description:
;     Stores the current position of the 2 or 3 mouse axes
;     and the delta for the last frame.
; Remarks:
;     Any item with a [MSI] flag is only available when
;     Config.Mouse.Intellimouse is set.
; =========================================================
.module Position
    .var uword, X       ; 16-bit X axis.
    .var uword, Y       ; 16-bit Y axis.
    .var byte, DX       ; x Delta (lower 8 bits only).
    .var byte, DY       ; y Delta (lower 8 bits only).
.if Parent.Parent.Config.Mouse.Intellimouse
    .var ubyte, Z       ; 8-bit Z axis (scrollwheel). [MSI]
    .var byte, DZ       ; z Delta. [MSI]
.endif
.endmodule

; =========================================================
; Module: Mouse.Status
; =========================================================
; Description:
;     Stores the current status of the mouse - the buttons
;     and current mode are listed.
; Remarks:
;     Any item with a [MSI] flag is only available when
;     Config.Mouse.Intellimouse is set.
; =========================================================
.module Status
    .var ubyte, Buttons ; Buttons bit array.
.if Parent.Parent.Config.Mouse.Intellimouse
    .var ubyte, Mode    ; Current mouse mode. [MSI]
.endif
.endmodule

; .........................................................
; Equates
; .........................................................

; Bit indices for the 3 buttons:
.enum Buttons, Left, Right, Middle

; Bit indices for movement signs:
.enum MovementSign, X = 4, Y = 5

; Mouse mode [MSI]. Standard PS/2 mouse mode or Intellimouse
; mode (scrollwheel).
.enum Mode, Standard = 0, Intellimouse = 3

; .........................................................
; Equates for the various mouse command bytes
; ........................................................
.module Command 
    Defaults      = $F6 ; Set defaults.
    SampleRate    = $F3 ; Set sample rate.
    ModeRemote    = $F0 ; Enter remote mode.
    ModeWrap      = $EE ; Enter wrap mode.
    ResetWrap     = $EC ; Come back from wrap mode.
    Read          = $EB ; Read a position/button packet.
    ModeStream    = $EA ; Enter stream mode.
    StatusReq     = $E9 ; Request a status packet.
    Resolution    = $E8 ; Set resolution.
    Scaling2t1    = $E7 ; Set scaling 2:1
    Scaling1t1    = $E6 ; Set scaling 1:1
.endmodule
    
; ---------------------------------------------------------
; Initialise -> Initialises the mouse.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on success, nz on failure.
;           Status.ode = mouse type.
; Destroys: af, bc, de
; ---------------------------------------------------------

Initialise
	ld a,AT.Command.Reset
	call AT.SendSafeByte
	ret nz
	
	; We're resetting...
	ld d,Config.Mouse.ResetTime
	ld e,0
-	push de
	call AT.GetByte
	call AT.SafeDelay
	pop de
	jr z,{+}
	dec de
	ld a,d
	or e
	jr nz,{-}
+
	cp AT.Command.Post
	ret nz
		
	call AT.GetByte
	ret nz
	call AT.SafeDelay

.if Config.Mouse.Intellimouse
CheckIfIntellimouse
	ld a,200 \ call SetSampleRate \ ret nz
	ld a,100 \ call SetSampleRate \ ret nz
	ld a,80  \ call SetSampleRate \ ret nz

	ld a,AT.Command.Identify
	call AT.SendSafeByte
	ret nz
	
	call AT.GetByte
	ret nz
	call AT.SafeDelay
		
	ld (Status.Mode),a
.endif
	xor a
	ret
	
; ---------------------------------------------------------
; Update -> Update the various mouse variables.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on success, nz on failure.
;           Position.X, Position.Y   = mouse position.
;           Position.DX, Position.DY = change in position.
;           Status.Buttons           = buttons bit array.
;           Position.Z               = scroll wheel position.
;           Position.DX              = change in scroll.
; Destroys: af, bc, de, hl
; ---------------------------------------------------------

Update
	ld a,Command.Read
	call AT.SendSafeByte
	ret nz
	
	call AT.GetByte ; Buttons/signs/overflow
	ret nz
	ld (Status.Buttons),a
	
	call AT.GetByte ; X
	ret nz
	ld (Position.DX),a
	
	call AT.GetByte ; Y
	ret nz
	ld (Position.DY),a

.if Config.Mouse.Intellimouse
	ld a,(Status.Mode)
	or a
	jr z,{+} ; Standard PS/2 mouse
	
	call AT.GetByte
	ret nz
	ld (Position.DZ),a
	ld b,a
	ld a,(Position.Z)
	add a,b
	ld (Position.Z),a
+
.endif
	
	
	ld a,(Status.Buttons)
	ld b,a

	; Update Y
		
	ld a,(Position.DY)
	ld d,$00
	bit MovementSign.Y,b
	jr z,{+}
	ld d,$FF
+	ld e,a
	
	ld hl,(Position.Y)
	
.if !Config.Mouse.InvertY
	add hl,de
.else
	xor a
	sbc hl,de
.endif

	
.if Config.Mouse.Clip
	push hl
	ld de,Config.Mouse.ClipY + 1
	xor a
	sbc hl,de
	pop hl
	jr c,{+}
	ld hl,0	
	bit MovementSign.Y,b
	.if !Config.Mouse.InvertY
		jr nz,{+}
	.else
		jr z,{+}
	.endif
	ld hl,Config.Mouse.ClipY
	
+
.endif

	ld (Position.Y),hl
	
	; Update X
	
	ld a,(Position.DX)
	ld d,$00
	bit MovementSign.X,b
	jr z,{+}
	ld d,$FF
+	ld e,a

	ld hl,(Position.X)
	add hl,de
	
.if Config.Mouse.Clip
	push hl
	ld de,Config.Mouse.ClipX + 1
	xor a
	sbc hl,de
	pop hl
	jr c,{+}
	ld hl,0	
	bit MovementSign.X,b
	jr nz,{+}
	ld hl,Config.Mouse.ClipX
+
.endif
	
	ld (Position.X),hl		
	xor a
	ret
	
; ---------------------------------------------------------
; SetSampleRate -> Set the mouse sample rate.
; ---------------------------------------------------------
; Inputs:   a = rate (10, 20, 40, 60, 80, 100, 200).
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Not much use as we are operating in remote mode
;           rather than stream mode. However, it is used to
;           identify whether the connected mouse has a
;           scroll-wheel or not (Intellimouse).
; ---------------------------------------------------------
SetSampleRate
	push af
	ld a,Command.SampleRate
	call AT.SendSafeByte
	call AT.SafeDelay
	pop bc
	ret nz
	ld a,b
	call AT.SendSafeByte
	jp AT.SafeDelay
.endmodule
