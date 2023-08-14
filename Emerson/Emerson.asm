; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;   ______  ______  ______  ______  ______  ______  ______
;  /  ___/\/     /\/  ___/\/ ____/\/  ___/\/ __  /\/ __  /\
; /  ___/\/ / / / /  ___/\/ /\___\/___  /\/ /\/ / / / / / /
;/_____/\/_/_/_/ /_____/\/_/ /   /_____/ /_____/ /_/ /_/ /
;\_____\/\_\_\_\/\_____\/\_\/    \_____\/\_____\/\_\/\_\/
; 
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;
; - A MaxCoderz Production - see http://www.maxcoderz.org -
;
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;
; AT and PS/2 device library for the TI-83 and TI-83 Plus
; graphical calculators by Ben Ryves 2005-2006.
; See readme.htm for more detailed usage instructions as
; well as examples and good-practice information.
;
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;
; Written for the Brass Z80 assembler - download it from
; the site http://www.benryves.com/bin/brass/
;
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;
; These routines require that PS/2 pin 1 (data) goes to the
; ring of the stereo minijack and pin 5 (clock) goes to the
; tip of the stereo minijack.
;
; +----------------------+-------+-------+-------+-------+
; | PS/2 pin number      | 5     | 1     | 3     | 4     |
; | PS/2 pin description | Clock | Data  | Ground| Vcc   |
; +----------------------+-------+-------+-------+-------+
; | TI jack terminal     | Tip   | Ring  | Base  | N/A   |
; | TI jack wire colour  | Red   | White | Braid | N/A   |
; +----------------------+-------+-------+-------+-------+
; | Power                | N/A   | N/A   | 0V    | +5V   |
; +----------------------+-------+-------+-------+-------+
;                                .-.
;         6.-..-.5               \_/ <- Red (Tip)
;         /o [] o\               |_| <- White (Ring)
;       4 o      o 3             | | <- Base (Ground)
;         \ o  o /               |_|
;         2'-..-'1              /   \
;       PS/2 (socket)           \___/  TI minijack
;
; XT keyboards used a different protocol, and so will not
; work with these routines. I do not have an XT keyboard
; to test on, I'm afraid; the oldest keyboards I have are
; AT, most are PS/2. The only difference between PS/2
; and AT devices are the connector sizes; PS/2 uses the
; smaller 6-pin mini DIN, AT uses the 5-pin 180 degree full
; size DIN. Pins 2 and 6 are reserved.
;
; Please use 5V; most keyboard controllers seem happy on
; 9V, but mouse controllers don't appear to work at this
; level.
;
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
;
; Inspired by the work of Cullen Ashford Logan.
; Named after Keith.
; A MaxCoderz production.
;
; References:
; - Mark Schultz's "AT keyboard interface".
; - Adam Chapweske's "The PS/2 Mouse Interface".
;
; Thanks:
; - Kerm M for prompting interest in this project.
; - CoBB for the correct bit pattern and explanation for
;   the TI-83 ROM paging magic that is on the bport.
;
; ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

; =========================================================
; Module: AT
; =========================================================
; Description:
;     Routines for sending and receiving bytes using the AT
;     protocol.
; Functions:
;     _send_byte: Send a byte.
;     _get_byte:  Receive a byte.
; =========================================================

.module AT

; .........................................................
; Internal equates
; .........................................................

_timeout      = 255        ; Timeout delay for routines.
_link_neutral = %11010000  ; ORd value written to bport.
_clock        = %01        ; Clock line bitmask.
_data         = %10        ; Data line bitmask.
_retries      = 4          ; Number of retries on failure.

; .........................................................
; Useful general-purpose equates
; .........................................................

_cmd_reset    = $FF        ; Reset connected device.
_cmd_resend   = $FE        ; Resend the last byte.
_cmd_identify = $F2        ; Get the device ID.
_cmd_enable   = $F4        ; Enable the device.
_cmd_disable  = $F5        ; Disable the device.

_ack          = $FA        ; Acknowledgement
_post_passed  = $AA        ; Power-on self test passed.

; ---------------------------------------------------------
; _send_byte -> Send a byte.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------

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

-	call _wait_bit_low
	ld a,c
	and 1
	add a,a
	or _link_neutral
	out (bport),a
	call _wait_bit_high
	srl c
	dec e
	jr nz,{-}

	; Send the parity bit

	call _wait_bit_low
	ld a,d
	or a
	ld a,_link_neutral|_data
	jp po,{+}
	ld a,_link_neutral
+	out (bport),a
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

	xor a
	ld a,_link_neutral|_clock
	out (bport),a
	ret

; ---------------------------------------------------------
; _send_ack_byte -> Send a byte and check acknowledgement.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------

_send_ack_byte
	call _send_byte
	ret nz
	call _get_byte
	ret nz
	cp _ack
	ret

; ---------------------------------------------------------
; _send_safe_byte -> Send a byte securely, checking resend.
; ---------------------------------------------------------
; Inputs:   a = byte to send.
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Disables interrupts. Holds clock line low at
;           the end, stopping the connected device from
;           sending any more data until the function is
;           called again.
; ---------------------------------------------------------
	
_send_safe_byte
	ld c,a
	ld b,_retries

-	push bc
	ld a,c
	call _send_ack_byte
	pop bc
	ret z
	cp _cmd_resend
	ret nz
	djnz {-}
	

; ---------------------------------------------------------
; _get_byte -> Receives a byte.
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
	ld a,_link_neutral|_clock
	out (bport),a
	xor a
	ld a,c
	ret

; For internal use only.
; Waits until the clock line falls low.
_wait_bit_low
	in a,(bport)
	and _clock
	ret z
	ld b,_timeout
-	in a,(bport)
	and _clock
	ret z
	call _wait_delay
	djnz {-}
	pop bc
	jr _fail

; For internal use only.
; Waits until the clock line goes high.
_wait_bit_high
	in a,(bport)
	and _clock
	ret nz
	ld b,_timeout
-	in a,(bport)
	and _clock
	ret nz
	call _wait_delay
	djnz {-}
	pop bc
	jr _fail

; For internal use only.
; A short delay when waiting for the next clock.
_wait_delay
	push af
	inc hl
	nop
	dec hl
	pop af
	ret

; For internal use only.
; Stops device sending data, returns nz (failure).
_fail
	; Set nz to indicate failure, return.
	ld a,_link_neutral|_clock
	out (bport),a
	or 1
	ret

; For internal use only.
; A short delay which can be issued between commands.
_safe_delay
	push af
	ei
	halt
	halt
	di
	pop af
	ret

.endmodule

; =========================================================
; Module: Mouse
; =========================================================
; Description:
;     PS/2 mouse routines.
; Functions:
;     _initialise: Reset and initialise the mouse.
;     _update:     Update the mouse status variables.
; Variables:
;     _x:       16-bit x coordinate.
;     _y:       16-bit y coordinate.
;     _dx:      Lower 8-bits of change in x.
;     _dy:      Lower 8-bits of change in y.
;     _buttons: 8-bit button status byte.
; Remarks:
;     Anything with (MSI) after it in comments is a
;     Microsoft Intellimouse-specific feature, and will not
;     work on some old devices. (If it has a scrollwheel,
;     it should be Microsoft Intellimouse compatible).
; Options:
;     Emerson.Mouse.Intellimouse - enable Intellimouse
;                                  extension support.
;     Emerson.Mouse.InvertY      - invert the Y axis so it
;                                  points from North to
;                                  South.
;     Emerson.Mouse.Clip         - Clip the mouse.
;     Emerson.Mouse.Clip.X       - Maximum X coordinate.
;     Emerson.Mouse.Clip.Y       - Maximum Y coordinate.
; =========================================================

.ifdef Emerson.Mouse
.module Mouse

; .........................................................
; Variables
; .........................................................

_x       .dw 0 ; 16-bit X axis.
_y       .dw 0 ; 16-bit Y axis.
_buttons .db 0 ; Buttons bit array.

_dx      .db 0 ; x Delta (lower 8 bits only).
_dy      .db 0 ; y Delta (lower 8 bits only).

.ifdef Emerson.Mouse.Intellimouse
_z       .db 0 ; 8-bit Z axis (scrollwheel). (MSI)
_mode    .db 0 ; Mouse mode
_dz      .db 0 ; z Delta
.endif

.ifdef Emerson.Mouse.Clip
	.ifndef Emerson.Mouse.Clip.X
		.define Emerson.Mouse.Clip.X 65535
	.endif
	.ifndef Emerson.Mouse.Clip.Y
		.define Emerson.Mouse.Clip.Y 65535
	.endif
.endif

; .........................................................
; Equates
; .........................................................

_btn_l             = 0  ; Left mouse button bit index
_btn_r             = 1  ; Right mouse button bit index
_btn_m             = 2  ; Middle mouse button bit index

_x_sign            = 4  ; Sign bit index for x movement
_y_sign            = 5  ; Sign bit index for y movement

_mode_standard     = 0 ; Standard PS/2 mouse mode.
_mode_intellimouse = 3 ; Intellimouse mode (scrollwheel).

_reset_time        = 12  ; How long a timeout on reset?

_cmd_defaults      = $F6 ; Set defaults.
_cmd_sample_rate   = $F3 ; Set sample rate.
_cmd_mode_remote   = $F0 ; Enter remote mode.
_cmd_mode_wrap     = $EE ; Enter wrap mode.
_cmd_reset_wrap    = $EC ; Come back from wrap mode.
_cmd_read          = $EB ; Read a position/button packet.
_cmd_mode_stream   = $EA ; Enter stream mode.
_cmd_status_req    = $E9 ; Request a status packet.
_cmd_resolution    = $E8 ; Set resolution.
_cmd_scaling_2t1   = $E7 ; Set scaling 2:1
_cmd_scaling_1t1   = $E6 ; Set scaling 1:1

; ---------------------------------------------------------
; _initialise -> Initialises the mouse.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on success, nz on failure.
;           _mode = mouse type.
; Destroys: af, bc, de
; ---------------------------------------------------------

_initialise
	ld a,AT._cmd_reset
	call AT._send_safe_byte
	ret nz
	
	; We're resetting...
	ld d,_reset_time
	ld e,0
-	push de
	call AT._get_byte
	call AT._safe_delay
	pop de
	jr z,{+}
	dec de
	ld a,d
	or e
	jr nz,{-}
+
	cp AT._post_passed
	ret nz
		
	call AT._get_byte
	ret nz
	call AT._safe_delay

.ifdef Emerson.Mouse.Intellimouse
	; We know there's a mouse connected, now.
	; Is it an Intellimouse?
	
	ld a,200
	call _set_sample_rate
	ret nz

	ld a,100
	call _set_sample_rate
	ret nz

	ld a,80
	call _set_sample_rate
	ret nz

	ld a,AT._cmd_identify
	call AT._send_safe_byte
	ret nz
	
	call AT._get_byte
	ret nz
	call AT._safe_delay
		
	; Load the value into our mode variable.
	ld (_mode),a
.endif

	xor a
	ret
	
; ---------------------------------------------------------
; _update -> Update the various mouse variables.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on success, nz on failure.
;           _x, _y   = mouse position.
;           _dx, _dy = change in mouse position.
;           _buttons = mouse buttons bit array.
;           _z       = scroll wheel position.
;           _dz      = change in scroll wheel position.
; Destroys: af, bc, de, hl
; ---------------------------------------------------------

_update
	ld a,_cmd_read
	call AT._send_safe_byte
	ret nz
	
	call AT._get_byte ; Buttons/signs/overflow
	ret nz
	ld (_buttons),a
	
	call AT._get_byte ; X
	ret nz
	ld (_dx),a
	
	call AT._get_byte ; Y
	ret nz
	ld (_dy),a

.ifdef Emerson.Mouse.Intellimouse
	ld a,(_mode)
	or a
	jr z,{+} ; Standard PS/2 mouse
	
	call AT._get_byte
	ret nz
	ld (_dz),a
	ld b,a
	ld a,(_z)
	add a,b
	ld (_z),a
+
.endif
	
	
	ld a,(_buttons)
	ld b,a

	; Update _y
		
	ld a,(_dy)
	ld d,$00
	bit _y_sign,b
	jr z,{+}
	ld d,$FF
+	ld e,a
	
	ld hl,(_y)
	
.ifndef Emerson.Mouse.InvertY
	add hl,de
.else
	xor a
	sbc hl,de
.endif

	
.ifdef Emerson.Mouse.Clip
	push hl
	ld de,Emerson.Mouse.Clip.Y + 1
	xor a
	sbc hl,de
	pop hl
	jr c,{+}
	ld hl,0	
	bit _y_sign,b
	.ifdef Emerson.Mouse.Clip.Wrap
		.ifndef Emerson.Mouse.InvertY
			jr z,{+}
		.else
			jr nz,{+}
		.endif
	.else
		.ifndef Emerson.Mouse.InvertY
			jr nz,{+}
		.else
			jr z,{+}
		.endif
	.endif
	ld hl,Emerson.Mouse.Clip.Y
	
+
.endif

	ld (_y),hl
	
	; Update _x
	
	ld a,(_dx)
	ld d,$00
	bit _x_sign,b
	jr z,{+}
	ld d,$FF
+	ld e,a

	ld hl,(_x)
	add hl,de
	
.ifdef Emerson.Mouse.Clip
	push hl
	ld de,Emerson.Mouse.Clip.X + 1
	xor a
	sbc hl,de
	pop hl
	jr c,{+}
	ld hl,0	
	bit _x_sign,b
	.ifdef Emerson.Mouse.Clip.Wrap
		jr z,{+}
	.else
		jr nz,{+}
	.endif
	ld hl,Emerson.Mouse.Clip.X
+
.endif
	
	ld (_x),hl
	
	
		
	xor a
	ret
	
; ---------------------------------------------------------
; _set_sample_rate -> Set the mouse sample rate.
; ---------------------------------------------------------
; Inputs:   a = rate (10, 20, 40, 60, 80, 100, 200).
; Outputs:  z on success, nz on failure.
; Destroys: af, bc, de
; Remarks:  Not much use as we are operating in remote mode
;           rather than stream mode. However, it is used to
;           identify whether the connected mouse has a
;           scroll-wheel or not (Intellimouse).
; ---------------------------------------------------------
_set_sample_rate
	push af
	ld a,_cmd_sample_rate
	call AT._send_safe_byte
	call AT._safe_delay
	pop bc
	ret nz
	ld a,b
	call AT._send_safe_byte
	jp AT._safe_delay
.endmodule
.endif