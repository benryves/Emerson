#include "../includes/headers.inc"
#include "../includes/keyval.inc"

; ==========================================
; Description (ignored if not Ion/MirageOS)
; ==========================================
#ifndef TI83P
.db "Emerson",0
#endif

; ==========================================
; Program entry point
; ==========================================

init_all:
	; We need to scroll the screen automatically:
	set appAutoScroll,(iy+appFlags)  
	ld a,10
	; See if we can find a translation table:

	call keyi_load_table
	jr z,_loaded_table
	bcall(_clrLCDFull)
	bcall(_homeUp)
	ld hl,_no_table_string
	bcall(_putS)
_no_table_found:
	or a
	jr z,_no_table_found
	ret
_no_table_string:
	.db "Keyboard layout "
	.db "table not found.",0
_loaded_table:

	; Reset the keyboard
	ld a,at_cmd_reset
	call at_send_byte

	bcall(_homeUp)

	; Load custom event handlers:
	ld hl,key_down
	call keyb_set_keydown

main_loop:

	call keyb_update

	call keyi_get_status_icon
	bcall(_putMap)

	ld a,$FF
	out (1),a
	ld a,KeyRow_5
	out (1),a
	in a,(1)
	cp dkClear
	ret z
	cp dKEnter
	jr nz,main_loop
	jr main_loop



; Key handlers:

key_down:
	call keyi_translate
	call rout_putc
	ret

#include "GetString.asm"
#include "key_input.asm"

.end