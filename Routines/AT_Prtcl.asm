.module AT

_timeout      = 255
_link_neutral = %11010000
_clock        = %01
_data         = %10

_get_byte
	di	

	; Clear Link port
	ld a,_link_neutral
	out (bport),a

	; Get the start bit:

	call _wait_bit_low
	call _wait_bit_high

	; Now we need to get the 8 bits for the byte

	; Reset the output byte
	ld c,0
	ld e,8

-
	call _wait_bit_low

	; Now we get the bit itself
	
	in a,(bport)

	rrca
	rrca
	rr c

	call _wait_bit_high

	dec e
	jr nz,{-}

	; Get the parity/stop bits

	call _wait_bit_low
	call _wait_bit_high
	call _wait_bit_low
	call _wait_bit_high

	; Clear flags, load code into accumulator and exit
	ld a,_link_neutral
	out (bport),a
	ld a,c
	ret

_fail
	; Set nz to indicate failure, return.
	ld a,_link_neutral
	out (bport),a
	or 1
	ret


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

_send_byte
	di
	cpl
	ld c,a

	; Issue RTS:

	; Set clock low
	ld a,_link_neutral|_clock
	out (bport),a

	nop

	; Set data low
	ld a,_link_neutral|_clock|_data
	out (bport),a

	; Release clock again
	ld a,_link_neutral|_data
	out (bport),a

	ld e,8
	ld d,c ; Original value

_send_byte_loop
	call _wait_bit_low

	ld a,c
	and 1
	add a,a
	or _link_neutral
	
	out (bport),a
	call _wait_bit_high
	srl c

	dec e
	jr nz,_send_byte_loop

	; Send the parity bit

	call _wait_bit_low
	ld a,d
	or a
	ld a,_link_neutral|%10
	jp po,{+}
	ld a,_link_neutral|%00
+
	out (bport),a
	call _wait_bit_high

	; Send the stop bit

	call _wait_bit_low
	ld a,_link_neutral
	out (bport),a
	call _wait_bit_high

	; Send the ACK bit

	call _wait_bit_low
	ld a,_link_neutral|_data
	out (bport),a
	call _wait_bit_high

	ld a,_link_neutral|_clock
	out (bport),a
	ret
	
.endmodule