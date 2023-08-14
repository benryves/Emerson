; =========================================================
; Module: AT
; =========================================================
; Description:
;     Routines for sending and receiving bytes using the AT
;     protocol.
; Functions:
;     SendByte: Send a byte.
;     GetByte:  Receive a byte.
; =========================================================

.module AT

; .........................................................
; Internal equates
; .........................................................

LinkNeutral  = %11010000  ; ORd value written to bport.

; .........................................................
; Definitions for the "clock" and "data" line bitmasks
; .........................................................

.enum Line, Clock = %01, Data = %10

; .........................................................
; Equates for the various AT command bytes
; .........................................................

.module Command
    Reset    = $FF        ; Reset connected device.
    Resend   = $FE        ; Resend the last byte.
    Identify = $F2        ; Get the device ID.
    Enable   = $F4        ; Enable the device.
    Disable  = $F5        ; Disable the device.
    Echo     = $EE        ; Check for echo.
    Ack      = $FA        ; Acknowledgement.
    Post     = $AA        ; Power-on self test passed.
.endmodule

; ---------------------------------------------------------
; SendByte -> Send a byte.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------

SendByte
	di
	cpl
	ld c,a

	; Issue RTS:

	; Set clock low
	ld a,LinkNeutral|Line.Clock
	out (bport),a

	nop

	; Set data low
	ld a,LinkNeutral|Line.Clock|Line.Data
	out (bport),a

	; Release clock again
	ld a,LinkNeutral|Line.Data
	out (bport),a

	ld e,8
	ld d,c ; Original value

-	call WaitBitLow
	ld a,c
	and 1
	add a,a
	or LinkNeutral
	out (bport),a
	call WaitBitHigh
	srl c
	dec e
	jr nz,{-}

	; Send the parity bit

	call WaitBitLow
	ld a,d
	or a
	ld a,LinkNeutral|Line.Data
	jp po,{+}
	ld a,LinkNeutral
+	out (bport),a
	call WaitBitHigh

	; Send the stop bit

	call WaitBitLow
	ld a,LinkNeutral
	out (bport),a
	call WaitBitHigh

	; Send the ACK bit

	call WaitBitLow
	ld a,LinkNeutral|Line.Data
	out (bport),a
	call WaitBitHigh

	xor a
	ld a,LinkNeutral|Line.Clock
	out (bport),a
	ret

; ---------------------------------------------------------
; SendAckByte -> Send a byte and check acknowledgement.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------

SendAckByte
	call SendByte
	ret nz
	call GetByte
	ret nz
	cp Command.Ack
	ret

; ---------------------------------------------------------
; SendSafeByte -> Send a byte securely, checking resend.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------
	
SendSafeByte
	ld c,a
	ld b,Parent.Config.AT.Retries

-	push bc
	ld a,c
	call SendAckByte
	pop bc
	ret z
	cp Command.Resend
	ret nz
	djnz {-}
	

; ---------------------------------------------------------
; GetByte -> Receives a byte.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  a = received byte.
;           z on success, nz on failure.
; Destroys: af, bc, e
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------

GetByte
	di
	; Clear Link port
	ld a,LinkNeutral
	out (bport),a

	; Get the start bit:

	call WaitBitLow
	call WaitBitHigh

	; Now we need to get the 8 bits for the byte

	; Reset the output byte
	ld c,0
	ld e,8

-
	call WaitBitLow

	; Now we get the bit itself
	
	in a,(bport)

	rrca
	rrca
	rr c

	call WaitBitHigh

	dec e
	jr nz,{-}

	; Get the parity/stop bits

	call WaitBitLow
	call WaitBitHigh
	call WaitBitLow
	call WaitBitHigh

	; Clear flags, load code into accumulator and exit
	ld a,LinkNeutral|Line.Clock
	out (bport),a
	xor a
	ld a,c
	ret

; For internal use only.
; Waits until the clock line falls low.
WaitBitLow
	in a,(bport)
	and Line.Clock
	ret z
	ld b,Parent.Config.AT.Timeout
-	in a,(bport)
	and Line.Clock
	ret z
	call WaitDelay
	djnz {-}
	pop bc
	jr Fail

; For internal use only.
; Waits until the clock line goes high.
WaitBitHigh
	in a,(bport)
	and Line.Clock
	ret nz
	ld b,Parent.Config.AT.Timeout
-	in a,(bport)
	and Line.Clock
	ret nz
	call WaitDelay
	djnz {-}
	pop bc
	jr Fail

; For internal use only.
; A short delay when waiting for the next clock.
WaitDelay
	push af
	inc hl
	nop
	dec hl
	pop af
	ret

; For internal use only.
; Stops device sending data, returns nz (failure).
Fail
	; Set nz to indicate failure, return.
	ld a,LinkNeutral|Line.Clock
	out (bport),a
	or 1
	ret

; For internal use only.
; A short delay which can be issued between commands.
SafeDelay
	push af
	ei
	halt
	halt
	di
	pop af
	ret

.endmodule