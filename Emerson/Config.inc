; =========================================================
; Module: Config
; =========================================================
; Description:
;     Configuration file for the library. By default,
;     everything is enabled so you get the complete
;     functionality. You can edit this file to disable the
;     modules you do not wish to use to save space.
; =========================================================
.module Config

; =========================================================
; Module: Config.AT
; =========================================================
; Description:
;     Settings for the very lowest-level functionality of
;     the library.
; =========================================================
.module AT
Timeout      = 128   ; Timeout delay for routines (1..256).
Retries      = 8     ; Number of retries on failure.
.endmodule

; =========================================================
; Module: Config.Device
; =========================================================
; Description:
;     Settings for the device identification module.
; =========================================================
.module Device
Enabled      = 1     ; Allow use of Device functions.
.endmodule

; =========================================================
; Module: Config.Keyboard
; =========================================================
; Description:
;     Settings for the keyboard routines.
; =========================================================
.module Keyboard
Enabled      = 1     ; Allow use of Keyboard functions.
.endmodule

; =========================================================
; Module: Config.Mouse
; =========================================================
; Description:
;     Settings for the mouse routines.
; =========================================================
.module Mouse
Enabled      = 1     ; Allow use of Mouse functions.
ResetTime    = 12    ; How long a timeout on reset?
InvertY      = 1     ; Invert the Y axis (so positive
                     ; values move down the screen).
Intellimouse = 1     ; Enabled Intellimouse extensions (for
                     ; scrollwheel support).
Clip         = 1     ; Clip the cursor to a fixed rectangle.
ClipX        = 1535  ; Maximum X coordinate.
ClipY        = 1023  ; Maximum Y coordinate.
.endmodule

.endmodule