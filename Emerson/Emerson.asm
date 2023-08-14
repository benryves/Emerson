; =========================================================
; Module: Emerson
; =========================================================
; Description:
;     Links together the various modules that make up the
;     library.
; Remarks:
;     You must enable nested modules and force local labels
;     using the .nestmodules and .local directives (Brass).
;     If you are using Latenite, the default project sets
;     these up for you.
;     Please see the manual for guidance.
; =========================================================

.module Emerson
	SizeStart = $
	; Load settings
	.include "Config.inc"
	; Jump over the routines
	jp SkipFile
	.include "Modules/AT Protocol.asm"    ; Core functionality (can't work without these).
	.if Config.Device.Enabled   \ .include "Modules/Device.asm"   \ .endif
	.if Config.Keyboard.Enabled \ .include "Modules/Keyboard.asm" \ .endif
	.if Config.Mouse.Enabled    \ .include "Modules/Mouse.asm"    \ .endif
SkipFile
	.echoln "Emerson - Ben Ryves 2005-2006 for MaxCoderz ({0} bytes).", $ - SizeStart
.endmodule