.include "Includes/headers.inc"
.varloc plotSScreen
.var 1, _entered_byte
main

	set appAutoScroll,(iy+appFlags)
	bcall(_homeup)
	res 0,(iy+asm_Flag1)
_loop
	bcall(_GetCSC)
	cp skClear
	ret z
	cp skDel
	jr nz,{+}
	bcall(_ClrLCDFull)
	bcall(_homeup)
+
	
	ld hl,_key_table
	ld bc,16
	cpir
	jr nz,{+}
	; Recognised key:
	ld de,-1-_key_table
	add hl,de
	ld a,l
	ld b,a
	ld a,(_entered_byte)
	sla a
	sla a
	sla a
	sla a			
	or b
	ld (_entered_byte),a
	bit 0,(iy+asm_Flag1)
	jr z,_first_nibble
	; Finished a byte!
	res 0,(iy+asm_Flag1)
	ld a,(_entered_byte)
	set textInverse,(iy+textFlags)
	call AT._send_byte
	jr z,_success_send
	ld a,'?'
	bcall(_PutC)
	bcall(_PutC)
	jr {+}	
_success_send
	ld a,(_entered_byte)
	call _display_hex
	jr {+}
_first_nibble
	set 0,(iy+asm_Flag1)
+

	res textInverse,(iy+textFlags)
-	call AT._get_byte
	jr nz,{+}
	call _display_hex
	jr {-}
+
	jr _loop
	
_key_table
.db sk0, sk1, sk2, sk3, sk4, sk5, sk6, sk7, sk8, sk9, skMath, skApps, skPrgm, skRecip, skSin, skCos

_display_hex
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
	bcall(_PutC)
	ld a,l
	djnz {-}
	ret	

.include "Emerson/Emerson.asm"