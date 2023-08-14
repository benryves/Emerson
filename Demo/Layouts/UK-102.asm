.db "EMKB1"
.db "UK",0


; Standard:
; These keys are the standard keys on the keyboard.

.db StdToggles-$
.db $58 ; Caps Lock
.db $77 ; Num Lock
.db $7E ; Scroll Lock
StdToggles

.db StdModifiers-$
.db $12 ; Left shift
.db $59 ; Right shift
.db $14 ; Ctrl
StdModifiers




; Extended:
; These keys get the $E0 prefix.
.db ExdToggles-$
ExdToggles

.db ExdModifiers-$
.db $11 ; AltGr
.db $14 ; Ctrl
ExdModifiers