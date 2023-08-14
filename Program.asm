; ===============================================================
; Emerson Demo
; Ben Ryves 2005-2006
; Note that this is a demo program. Assembly does not usually
; lend itself to clean-looking code, so I wouldn't really use
; this as a guide to how to use the library. Look at the
; documentation or the "Simplest" section for clearer code.
; ===============================================================

.module Program
.export
Main
	; Program entry point
	
	; Load the library:
	.include "Emerson/Emerson.asm"
	
	im 1
		
	bcall(_ForceFullScreen)
	res appAutoScroll,(iy+appFlags)
	res plotLoc,(iy+plotFlags)
	set bufferOnly,(iy+plotFlag3)
	set fullScrnDraw,(iy+apiFlg4)
	set appTextSave,(iy+appFlags)
	bcall(_ClrTxtShd)

	.var ubyte, LastMenuOption
	xor a
	ld (LastMenuOption),a
	
MenuLoop
	bcall(_ClrLCDFull)
	ld a,(LastMenuOption)
	ld hl,MainMenu
	call Menu.Display
	ld (LastMenuOption),a
	ret nz

	; Console	
	or a \ jp z,Console.Run
	; Device
	dec a \ jp z,Device.Run
	; Keyboard
	dec a \ jp z,Keyboard.Run
	; Mouse
	dec a \ jp z,Mouse.Run
	
	bcall(_ClrTxtShd)
	ret
	
.module Console
	.tvar ubyte, EnteredByte
	.tvar ubyte, BytesReceived
	Run
		xor a
		ld (BytesReceived),a
		bcall(_ClrLCDFull)
		bcall(_homeup)
		res 0,(iy+asm_Flag2)
	Loop
		bcall(_GetCSC)
		di
		cp skClear
		jp z,Parent.MenuLoop
		cp skDel
		jr nz,{+}
		bcall(_ClrLCDFull)
		bcall(_homeup)
		di
	+	ld hl,KeyTable
		ld bc,16
		cpir
		jr nz,{+}
		; Recognised key:
		ld de,-1-KeyTable
		add hl,de
		ld a,l
		ld b,a
		ld a,(EnteredByte)
		sla a
		sla a
		sla a
		sla a			
		or b
		ld (EnteredByte),a
		bit 0,(iy+asm_Flag2)
		jr z,FirstNibble
		; Finished a byte!
		res 0,(iy+asm_Flag2)
		ld a,(EnteredByte)
		set textInverse,(iy+textFlags)
		
		call Parent.Emerson.AT.SendByte
		push af
			call GetInput
			ld (BytesReceived),a		
		pop af
		
		jr z,SuccessSend
		ld a,'?'
		call SafePutC
		call SafePutC
		jr {+}

	SuccessSend
		ld a,(EnteredByte)
		call Parent.DisplayHex
	+	res textInverse,(iy+textFlags)
		
		ld a,(BytesReceived)
		call DisplayInput
		xor a
		ld (BytesReceived),a
		jr GetBytes
		
	FirstNibble
		set 0,(iy+asm_Flag2)	
	
	GetBytes
		call GetInput
		call DisplayInput

		jr Loop
		
	KeyTable
	.db sk0, sk1, sk2, sk3, sk4, sk5, sk6, sk7, sk8, sk9, skMath, $27, skPrgm, skRecip, skSin, skCos
	

		
	.var ubyte[256], InputBuffer
	GetInput
		ld hl,InputBuffer
		ld b,0
		ld c,0
	-	push bc
		push hl
		call Parent.Emerson.AT.GetByte
		pop hl
		pop bc
		jr nz,Done
		ld (hl),a
		inc hl
		inc c
		djnz {-}
		or a
	Done
		ld a,c
		ld hl,InputBuffer
		ret
		
	DisplayInput
		or a
		ret z
		ld b,a
		ld hl,InputBuffer

	-
		ld a,(hl)
		inc hl
		push hl
		push bc
		call Parent.DisplayHex
		pop bc
		pop hl
		djnz {-}
		ret
.endmodule


.module Device
Run

MainMenu
	bcall(_ClrLCDFull)
	bcall(_homeup)
	ld hl,Detecting
	set textInverse,(iy+textFlags)
	bcall(_PutS)
	call SafeNewLine
	bcall(_PutS)
	res textInverse,(iy+textFlags)
	bcall(_PutS)

	bcall(_RunIndicOn)
-	bcall(_GetCSC)
	cp skClear
	jr z,ExitInit
	cp skEnter
	jr z,ExitInit
	cp sk1
	jr z,ExitInit
	call Parent.Emerson.Device.Identify
	jr nz,{-}
	
	ld hl,1
	ld (curRow),hl
	
	push af
	ld a,$05
	bcall(_PutC)
	ld a," "
	bcall(_PutC)
	pop af
	
	
	ld hl,DeviceNames-1
	ld b,a
--
-	ld a,(hl)
	inc hl
	or a
	jr nz,{-}
	djnz {--}	
	
	bcall(_PutS)
	
	ld a,busyPause
	ld (indicBusy),a

DetectedLoop
	bcall(_GetCSC)
	or a
	jp nz,ExitInit
	
	call Parent.Emerson.Device.CheckConnected
	jr z,DetectedLoop
	ld a,busyNormal
	ld (indicBusy),a
	jr MainMenu

ExitInit
	bcall(_RunIndicOff)
	jp Parent.MenuLoop

Detecting
.db "DEVICE",0,"1:",0,"Cancel",0
DeviceNames
.db "Keyboard", 0, "Standard Mouse", 0, "Intellimouse", 0

.endmodule

.module Keyboard
.using Parent.Emerson.Keyboard
Run
	.tvar ubyte,LastMenuOption
	xor a
	ld (LastMenuOption),a
MainMenu
	bcall(_ClrLCDFull)
	ld a,(LastMenuOption)
	ld hl,Menu
	call Parent.Menu.Display
	jp nz,Parent.MenuLoop
	ld (LastMenuOption),a

	or a  \ jp z,Scancodes
	dec a \ jp z,TextInput
	jp Parent.MenuLoop
	
ScanCodes
	bcall(_ClrLCDFull)
	bcall(_homeup)
TryAgain
	bcall(_GetCSC)
	or a
	jp nz,MainMenu
	call GetScancode
	jr nz,TryAgain

	push af
	ld a,(curRow)
	ld b,a
	ld a,(curCol)
	or b
	jr z,{+}
	call SafeNewLine
+
	pop af
	push af
	ld a,$1E
	jr c,{+}
	inc a
+
	call SafePutC
	ld a," "
	call SafePutC
	pop af
	push af
	
	ld a,"*"
	jp m,{+}
	ld a,$0C
+
	call SafePutC
	ld a," "
	call SafePutC
	pop af
	push af
	call Parent.DisplayHex
	ld a,"h"
	call SafePutC
	pop af
	ld l,a
	ld h,0
	bcall(_DispHL)	
	jr TryAgain
	
TextInput
	bcall(_ClrLCDFull)
	bcall(_homeup)
	call LoadLayoutFile
	jr z,{+}
	jr {+}

	bcall(_RunIndicOn)
	
	ld hl,NoLayout
	bcall(_PutS)

	ld a,busyPause
	ld (indicBusy),a
	call Parent.WaitKey
	bcall(_RunIndicOff)
	ld a,busyNormal
	ld (indicBusy),a
	jp MainMenu
NoLayout
.db "Layout file not found.",0

YesLayout
.db "Layout: ",0
+

	bcall(_ClrTxtShd)
	
	ld hl,YesLayout
	bcall(_PutS)
	ld hl,(LayoutFileDescription)
	bcall(_PutS)
	call SafeNewLine
InputLoop
	bcall(_GetCSC)
	or a
	jp nz,MainMenu
	
	.var ubyte, LoopFlash
	
	ld a,(LoopFlash)
	inc a
	ld (LoopFlash),a
	bit 7,a
	jr z,{+}	
	res appTextSave,(iy+appFlags)
	ld a,$E4
	bcall(_PutMap)
	set appTextSave,(iy+appFlags)
	jr {++}
+
	call RedrawCurrentChar
++
	
	call GetKey
	jr nz,InputLoop ; No key
	jr c,InputLoop ; Key released
	
	
	; Before we do anything, redraw the current character.

	push af
	call RedrawCurrentChar
	pop af

	
	jp p,StandardKey ; Standard key
	
	; So, it's a non-printable key.
	; Which?
	
	cp KeyCode.Escape
	jp z,MainMenu

	ld hl,curRow
	cp KeyCode.Up \ jr nz,{+} \ dec (hl) \+
	cp KeyCode.Down \ jr nz,{+} \ inc (hl) \+
	
	inc hl
	cp KeyCode.Left \ jr nz,{+} \ dec (hl) \+
	cp KeyCode.Right \ jr nz,{+} \ inc (hl) \+
	
	ld a,(hl) \ and 15 \ ld (hl),a
	dec hl
	ld a,(hl) \ and 7 \ ld (hl),a
	
	jr InputLoop
	
StandardKey	
	cp $7F
	jr nz,NotDelete
	
	; Delete has been pressed
	bcall(_ClrLCDFull)
	bcall(_ClrTxtShd)
	bcall(_homeup)
	jp InputLoop
	
NotDelete
	cp $20
	jr nc,NotControl
	
	; It's a control key!
		
	sub 8
	jr nz,NotBS
	ld a,(curCol)
	dec a
	ld (curCol),a
	cp -1
	jr nz,{++}
	ld a,(curRow)
	or a
	jr nz,{+}
	xor a
	ld (curCol),a
	jr z,{++}
+	dec a
	ld (curRow),a
	ld a,15
	ld (curCol),a
++	ld a," "
	bcall(_PutMap)
	jp InputLoop
NotBS
	dec a
	jr nz,NotHT
	
	ld a,(curCol)
	add a,4
	and ~3
	ld (curCol),a
	cp 16
	jp nz,InputLoop
	call SafeNewLine
	jp InputLoop
NotHT
	dec a
	jr nz,NotLF
	xor a
	ld (curCol),a
NotLF
	sub 3
	jr nz,NotCR
	call SafeNewLine
	jp InputLoop
NotCR
	jp InputLoop
	
NotControl	
	call SafePutC
	jp InputLoop	
Menu
	.db 3
	.db "KEYBOARD",0
	.db "Scancodes",0
	.db "Text Input",0
	.db "Back",0


RedrawCurrentChar
	ld a,(curRow)
	add a,a
	add a,a
	add a,a
	add a,a
	ld l,a
	ld a,(curCol)
	add a,l
	ld l,a
	ld h,0
	ld de,textShadow
	add hl,de
	ld a,(hl)
	bcall(_PutMap)
	ret

.endmodule

.module Mouse
Run
	.tvar ubyte,LastMenuOption
	xor a
	ld (LastMenuOption),a
MainMenu
	bcall(_ClrLCDFull)
	ld a,(LastMenuOption)
	ld hl,Menu
	call Parent.Menu.Display
	jp nz,Parent.MenuLoop
	ld (LastMenuOption),a

	or a  \ jp z,Initialise
	dec a \ jp z,Coordinates
	dec a \ jp z,Cursor
	jp Parent.MenuLoop
	
Initialise
	bcall(_ClrLCDFull)
	bcall(_homeup)
	ld hl,Initialising
	set textInverse,(iy+textFlags)
	bcall(_PutS)
	call SafeNewLine
	bcall(_PutS)
	res textInverse,(iy+textFlags)
	bcall(_PutS)
	

	bcall(_RunIndicOn)
-	bcall(_GetCSC)
	cp skClear
	jr z,ExitInit
	cp skEnter
	jr z,ExitInit
	cp sk1
	jr z,ExitInit
	call Parent.Emerson.Mouse.Initialise	
	jr nz,{-}
	
	; Hooray!
	ld hl,1
	ld (curRow),hl
	ld hl,Found
	bcall(_PutS)
	
	ld a,(Parent.Emerson.Mouse.Status.Mode)
	or a
	jr z,{+}
	ld hl,Intellimouse
+
	call SafeNewLine
	bcall(_puts)		
	
	ld a,busyPause
	ld (indicBusy),a
	call Parent.WaitKey
	bcall(_RunIndicOff)
	ld a,busyNormal
	ld (indicBusy),a
	jp MainMenu

ExitInit
	bcall(_RunIndicOff)
	jp MainMenu
	

Initialising
.db "INITIALISE",0,"1:",0,"Cancel",0

Found
.db "Initialised:",0
PS2
.db "Standard PS/2",0
Intellimouse
.db "Intellimouse",0

Coordinates
	bcall(_ClrLCDFull)
	call Parent.Emerson.Mouse.Update
	ld hl,0
	ld (Parent.Emerson.Mouse.Position.X),hl
	ld (Parent.Emerson.Mouse.Position.Y),hl
	xor a
	ld (Parent.Emerson.Mouse.Position.Z),a
UpdateCoordsLoop
	bcall(_homeup)
	
	call UpdateOrReinit
	jr z,MouseWorks
	bcall(_ClrLCDFull)
	jr NotWorkingMouse
MouseWorks
	
	ld hl,CoordinateList

	bcall(_PutS)
	push hl
	ld hl,(Parent.Emerson.Mouse.Position.X)		
	bcall(_DispHL)
	call SafeNewLine
	pop hl
	
	bcall(_PutS)
	push hl
	ld hl,(Parent.Emerson.Mouse.Position.Y)
	bcall(_DispHL)
	call SafeNewLine
	pop hl

	ld a,(Parent.Emerson.Mouse.Status.Mode)
	or a
	jr z,NoZ
	bcall(_PutS)
	push hl
	ld a,(Parent.Emerson.Mouse.Position.Z)
	ld l,a
	ld h,0
	bcall(_DispHL)
	call SafeNewLine
	pop hl
NoZ
	call SafeNewLine

	ld hl,Buttons
	bcall(_PutS)
	ld a,(Parent.Emerson.Mouse.Status.Buttons)
	ld c,a

	ld b,8
-	ld a,"0"
	sla c
	jr nc,{+}
	inc a
+	call SafePutC
	djnz {-}
	

NotWorkingMouse
	bcall(_GetCSC)
	or a
	jp nz,MainMenu
	jr UpdateCoordsLoop
	
CoordinateList
.db "Position.X=",0,"Position.Y=",0,"Position.Z=",0
Buttons
.db "Buttons=",0

.var ubyte[2], oldpos

Cursor
	call UpdateOrReinit

	ld hl,48*16
	ld (Parent.Emerson.Mouse.Position.X),hl
	ld hl,32*16
	ld (Parent.Emerson.Mouse.Position.Y),hl
	bcall(_GrBufClr)

CursorUpdateLoop
	call UpdateOrReinit
	jr z,CursorWorking
	call ionFastCopy
	jp CursorNotWorking
	
CursorWorking


	ld hl,(Parent.Emerson.Mouse.Position.X)
	ld b,4
-	add hl,hl \ djnz {-}
	ld a,h
	ld hl,(Parent.Emerson.Mouse.Position.Y)
	ld b,4
-	add hl,hl \ djnz {-}
	ld l,h
	ld h,a
	
	di
	push hl
	
	
		ld e,l
		ld h,$00
		ld d,h
		add hl,de
		add hl,de
		add hl,hl
		add hl,hl
		push af
		ld e,a
		and $07
		ld c,a
		srl e
		srl e
		srl e
		pop af
		
		ld b,$B0+7
		cp 96-8
		jr nc,{+}
		ld b,$B0+3
	+	ld a,b
		ld (LeftColumn),a
		
		

		
		add hl,de
		ld de,plotSScreen
		add hl,de
		
		; We're about to mangle the next few rows.
		push hl
			push hl
				ld de,saveSScreen
				ld a,c
				ld bc,12*8
				ldir
				pop hl
			ld c,a
			ld ix,CursorImage
			ld b,8	
		--
			ld d,(ix)
			ld e,$00
			ld a,c
			or a
			jr z,{+}
		-	srl d
			rr e
			dec a
			jr nz,{-}
		+	ld a,(hl)
			or d
			ld (hl),a
			inc hl
			ld a,(hl)
		LeftColumn
			or e
			ld (hl),a
			ld de,11
			add hl,de
			inc ix
			djnz {--}
		
			call ionFastCopy
			
			; Restore the mangled rows
			pop de
		ld hl,saveSScreen
		ld bc,12*8
		ldir
	
		pop hl
	ld a,63
	sub l
	ld l,a
	
	ld a,(Parent.Emerson.Mouse.Status.Buttons)
	and 1 << Parent.Emerson.Mouse.Buttons.Left
	jr z,{+}
	
	push hl
		ld d,h
		ld e,l
		ld bc,(oldpos)
		ld h,1
		bcall(_ILine)
		pop hl
+	ld (oldpos),hl
	
CursorNotWorking
	ei
	bcall(_GetCSC)
	or a
	jp z,CursorUpdateLoop
	cp skDel
	jp nz,MainMenu
	bcall(_GrBufClr)
	jp CursorUpdateLoop

	
; Set mouse to fastest possible mode
SetFast
	ld a,Parent.Emerson.Mouse.Command.Resolution
	call Parent.Emerson.AT.SendSafeByte
	ret nz
	ld a,$03
	call Parent.Emerson.AT.SendSafeByte
	ret nz
	ld a,Parent.Emerson.Mouse.Command.Scaling2t1
	jp Parent.Emerson.AT.SendSafeByte

; Update, or reinitialise?
UpdateOrReinit
	bit 0,(iy+asm_Flag2)
	jr z,{+}
	call Parent.Emerson.Mouse.Update
	ret z
	res 0,(iy+asm_Flag2)		
+	call Parent.Emerson.Mouse.Initialise
	ret nz
	call SetFast
	ret nz
	set 0,(iy+asm_Flag2)
	ret

CursorImage
.incbmp "Resources/Cursor.gif"

Menu
	.db 4
	.db "MOUSE",0
	.db "Initialise…",0
	.db "Coordinates",0
	.db "Cursor",0
	.db "Back",0
.endmodule



.global
SafeNewLine
	bcall(_NewLine)
	jr SafeCheckScroll
SafePutC
	bcall(_PutC)

SafeCheckScroll
	push af
	ld a,(curRow)
	cp 8
	jr z,{+}
	pop af
	ret
+
	ld a,7
	ld (curRow),a
	di

	push bc
	push hl
	
	ld c,$20
	ld b,12
	
--	push bc
	
	ld a,c
	call $B
	out ($10),a
	
	
	ld b,64-8
	ld c,$80

-	ld a,c
	add a,8
	call $B
	out ($10),a
	call $B
	in a,($11)
	call $B
	in a,($11)
	ld l,a
	
	ld a,c
	call $B
	out ($10),a
	call $B
	ld a,l
	out ($11),a
	inc c
	djnz {-}
	
	ld b,8
	xor a
-	call $B
	out ($11),a
	djnz {-}
	
	pop bc
	inc c
	djnz {--}
	
	
	push de
	
	ld hl,textShadow+16
	ld de,textShadow
	ld bc,128-16
	ldir
	
	ld hl,textShadow+7*16
	ld (hl)," "
	ld d,h \ ld e,l
	inc de
	ld bc,15
	ldir	
	
	pop de
	
	pop hl
	pop bc
	
	pop af
	
	ei
	ret
.endglobal



WaitKey
	bcall(_GetCSC)
	or a
	jr z,WaitKey
	ret
	
DisplayHex
	ld l,a
	srl a
	srl a
	srl a
	srl a
	ld b,2
-	and $0F
	ld c,'0'
	cp 10
	jr c,{+}
	ld c,'A'-10
+	add a,c
	call SafePutC
	ld a,l
	djnz {-}
	ret

; Menu drawing module

.module Menu
.tvar ubyte, Index
.tvar uword, Text
.tvar ubyte, Max
Display
	ld (Index),a
	ld (Text),hl
	ld a,(hl) \ inc hl
	ld (Max),a
	push af
	bcall(_homeup)
	set textInverse,(iy+textFlags)
	bcall(_PutS)
	res textInverse,(iy+textFlags)
	
	pop bc
	ld c,0
-	call SafeNewLine

	ld a,(Index)
	cp c
	jr nz,{+}
	set textInverse,(iy+textFlags)
+	ld a,"1"
	add a,c
	call SafePutC
	inc c
	ld a,":"
	call SafePutC
	res textInverse,(iy+textFlags)
	bcall(_PutS)
	djnz {-}

	ei
WaitKey
	halt
	bcall(_GetCSC)
	or a
	jr z,WaitKey


	cp skEnter
	jr nz,{+}
	ld a,(Index)
	ret
+	

	cp skClear
	jr nz,{+}
	or 1
	ret
+


	ld hl,(Text)

	cp skDown
	jr z,KeyDown
	cp skUp
	jr z,KeyUp


	ld hl,ScanCodeKeys
	ld bc,(Max)
	ld b,0
	cpir
	jr nz,WaitKey

	ld de,ScanCodeKeys+1
	or a
	sbc hl,de
	ld a,l
	cp a
	ret

ScanCodeKeys
.for kn, 1, 9 \ .db sk{kn} \ .loop
	

KeyDown
	ld a,(Max)
	ld b,a
	ld a,(Index)
	inc a
	cp b
	jr nz,NotBottom
	xor a
NotBottom	
	jp Display
	
KeyUp
	ld a,(Index)
	or a
	jr nz,NotTop
	ld a,(Max)
NotTop
	dec a
	jp Display
.endmodule

	
; Handful of resources
MainMenu
	.db 5
	.db "EMERSON",0
	.db "Console",0
	.db "Device",0
	.db "Keyboard…",0
	.db "Mouse…",0
	.db "Quit",0
	
.endmodule