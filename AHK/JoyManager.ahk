#include Joystick.ahk

Global JoyManager := new JoyStickManager()
SetTimer, ConLoop, 1000, 0
SetTimer, MapLoop, 10, 1
return

; Mapping maps registered controllers to xBox controllers
MapLoop:
	JoyManager.MapLoop()

; Loops over all non-xBox controllers, giving them a chance to (un)register, etc.
ConLoop:
	JoyManager.ControlLoop()

; Holds all Joystickobjects and contains the loop functions
; Unregistered controllers don't do anything for map
class JoyStickManager
{
	; 16 joystick objects that represent the direct input ones
	Joysticks := []
	
	__New() {
		Loop, 16 {
			j := new JoyStick(a_index)
			this.Joysticks[a_index] := j
		}
    }
	
	MapLoop() { 
		Loop, 16 {
			this.Joysticks[a_index].Map()
		}
	}
	
	ControlLoop() {
		Loop, 16 {
			this.Joysticks[a_index].Control()
		}
	}
}