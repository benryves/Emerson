; AT_Prtcl.asm
; Benjamin Ryves 2005 [benryves@benryves.com]
; AT Protocol subroutines for the TI-83 Plus.


.module AT_Protocol

at_timeout = 255

; Get byte:

at_get_byte:
	di	

	; Clear Link port
	xor a
	out (bport),a

	; Get the start bit:

	call _wait_bit_low
	call _wait_bit_high

	; Now we need to get the 8 bits for the byte

	; Reset the output byte
	ld c,0
	ld e,8

_get_byte_loop:
	call _wait_bit_low

	; Now we get the bit itself
	in a,(bport)

	rrca
	rrca
	rr c

	call _wait_bit_high

	dec e
	jr nz, _get_byte_loop

	; Get the parity/stop bits

	call _wait_bit_low
	call _wait_bit_high
	call _wait_bit_low
	call _wait_bit_high

	; Clear flags, load code into accumulator and exit
	xor a
	out (bport),a
	ld a,c
	ret

_get_byte_fail:
	; Set nz to indicate failure, return.
	xor a
	out (bport),a
	or 1
	ret

_timeout = at_timeout
_clock = 1
_data = 2
_fail = _get_byte_fail

_wait_bit_low
	in a,(bport)
	and _clock
	ret z
	ld b,_timeout
-	in a,(bport)
	and _clock
	ret z
	djnz {-}
	pop bc
	jr _fail


_wait_bit_high
	in a,(bport)
	and _clock
	ret nz
	ld b,_timeout
-	in a,(bport)
	and _clock
	ret nz
	djnz {-}
	pop bc
	jr _fail

; Send byte:

at_send_byte:
	di

	xor $FF
	ld c,a

	; Issue RTS :|
	

	; Set clock low
	ld a,1
	out (bport),a

	nop

	; Set data low
	ld a,3
	out (bport),a

	; Release clock again
	ld a,2
	out (bport),a

	ld e,8
	ld d,0	; Parity counter

_send_byte_loop:
	call _wait_bit_low

	ld a,c
	and 1

	add a,d
	ld d,a

	ld a,c
	and 1

	add a,a
	out (bport),a
	call _wait_bit_high
	srl c

	dec e
	jr nz,_send_byte_loop

	; Send the parity bit

	call _wait_bit_low
	ld a,d
	and 1
	add a,a
	out (bport),a
	call _wait_bit_high

	; Send the stop bit

	call _wait_bit_low
	xor a
	out (bport),a
	call _wait_bit_high

	; Send the ACK bit

	call _wait_bit_low
	ld a,2
	out (bport),a
	call _wait_bit_high

	xor a
	out (bport),a
	ret
	
.endmodule