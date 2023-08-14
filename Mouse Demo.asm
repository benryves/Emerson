.include "Includes/headers.inc"

main
	; Reset cursor position
	ld a,(96/2)-4
	ld (Mouse._x),a

	ld a,(64/2)-4
	ld (Mouse._y),a
	
	ld a,(contrast)
	ld (Mouse._z),a

	bcall(_homeup)
	ld hl,_waiting
	bcall(_PutS)

	; Wait for mouse to become available

-	bcall(_GetCSC)
	cp skClear
	ret z	
	call Mouse._initialise
	jr nz,{-}

	; The mouse has reset and initialised...
	
	; Display a "success" message
	bcall(_newline)
	ld hl,_success
	bcall(_PutS)
	bcall(_newline)

	ld hl,_ps2
	ld a,(Mouse._mode)
	or a
	jr z,{+}
	ld hl,_intellimouse
+
	bcall(_PutS)
	
	; Wait for a keypress
-	bcall(_GetCSC)
	or a
	jr z,{-}

	; Main loop

_loop
	call Mouse._update ; Update mouse variables

	bcall(_GrBufClr)

	ld a,(Mouse._y)
	ld l,a
	ld a,(Mouse._x)

	push af
	push hl

	ld b,8
	ld ix,sprite
	call ionPutSprite
	
	
	
	set textWrite,(iy+sGrFlags)
	
	xor a
	ld (penCol),a
	ld (penRow),a
	
	
	ld hl,(Mouse._x)
	bcall(_SetXXXXOP2)
	bcall(_OP2ToOP1)
	ld a,255
	bcall(_DispOP1A)
	
	ld a,48
	ld (penCol),a
	
	ld hl,(Mouse._y)
	bcall(_SetXXXXOP2)
	bcall(_OP2ToOP1)
	ld a,255
	bcall(_DispOP1A)	


	
	; Draw the buttons
	
	ld a,(Mouse._buttons)
	and 7
	ld l,a
	ld h,0
	ld de,_button_masks
	add hl,de
	ld a,(hl)
	ld (saveSScreen+0),a
	ld (saveSScreen+1),a

	pop hl
	pop af
	inc l
	ld b,2
	ld ix,saveSScreen
	call ionPutSprite
	
	
	call ionFastCopy
	
	; Update scroll (z) contrast
	ld a,(Mouse._z)
	cp $27
	jr c,{+}
	ld a,$27
	jp p,{++}
	xor a
++	ld (Mouse._z),a
+	
	ld (contrast),a
	
	add a,$18
	or $C0
	out ($10),a

	
	bcall(_GetCSC)
	cp skClear
	ret z
	
	cp skYEqu
	jr z,_set_res_1
	cp skWindow
	jr z,_set_res_2
	cp skZoom
	jr z,_set_res_4
	cp skTrace
	jr z,_set_res_8
	
	jp _loop	

_set_res_1 ld a,0 \ jr {+}
_set_res_2 ld a,1 \ jr {+}
_set_res_4 ld a,2 \ jr {+}
_set_res_8 ld a,3
+	push af
	ld a,Mouse._cmd_resolution
	call AT._send_safe_byte
	pop bc
	jp nz,_loop
	ld a,b
	call AT._send_safe_byte
	jp _loop
	
_button_masks
.db %00000000
.db %01000000
.db %00000100
.db %01000100
.db %00010000
.db %01010000
.db %00010100
.db %01010100

_waiting
.db "Waiting...",0

_success
.db "Detected:",0
_ps2
.db "Standard PS/2.",0
_intellimouse
.db "Intellimouse.",0


sprite
.incbmp "Resources/cursor.bmp"

.define Emerson.Mouse.Clip
.define Emerson.Mouse.Clip.X 96-8
.define Emerson.Mouse.Clip.Y 64-8
.define Emerson.Mouse
.define Emerson.Mouse.InvertY
.define Emerson.Mouse.Intellimouse
.include "Emerson/Emerson.asm"