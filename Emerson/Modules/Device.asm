; =========================================================
; Module: Device
; =========================================================
; Description:
;     Routines for determining the type of device attached
;     to the calculator.
; =========================================================

.module Device
.using Parent

; .........................................................
; Equates for the various device types
; .........................................................

.enum Type, Keyboard = 1, Mouse = 2, Intellimouse = 3

; ---------------------------------------------------------
; Identify -> Identify the connected device.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on success, nz on failure.
;           a    = device type.
; Destroys: af, bc, de.
; Remarks:  If the mouse routines are not being included,
;           and Intellimouse support isn't set, the library
;           can't determine the difference between standard
;           PS/2 devices and an Intellimouse.
; ---------------------------------------------------------

Identify
	ld a,AT.Command.Identify
	call AT.SendSafeByte
	ret nz
	call AT.GetByte
	ret nz
	or a
	jr z,{+}
	cp $AB
	ret nz
	call AT.GetByte
	ret nz
	cp $83
	ret nz
	ld a,Type.Keyboard
	ret	
	
+
.if !(Config.Mouse.Enabled && Config.Mouse.Intellimouse)
	ld a,Type.Mouse
.else
	call Mouse.CheckIfIntellimouse
	ret nz
	ld a,(Mouse.Status.Mode)
	or a
	jr nz,{+}
	ld a,Type.Mouse
	ret
+	cp 3
	ret nz
	xor a
	ld a,Type.Intellimouse
.endif
	ret
	

; ---------------------------------------------------------
; CheckConnected -> Check if a device is connected.
; ---------------------------------------------------------
; Inputs:   None.
; Outputs:  z on connected, nz on not connected.
; Destroys: af, bc, de.
; ---------------------------------------------------------

CheckConnected
	ld a,AT.Command.Echo
	call AT.SendByte
	ret nz
	call AT.GetByte
	ret nz
	cp AT.Command.Echo
	ret z
	cp AT.Command.Ack
	ret

.endmodule