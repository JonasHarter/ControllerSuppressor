
#include Reader.ahk

class BattlePadReader extends Reader {

	;BUTTONS
	Button_A(){
		return this.Button_Generic(2)
	}
	Button_B(){
		return this.Button_Generic(3)
	}
	Button_X(){
		return this.Button_Generic(1)
	}
	Button_Y(){
		return this.Button_Generic(4)
	}
	Button_L(){
		return this.Button_Generic(5)
	}
	Button_R(){
		return this.Button_Generic(6)
	}
	Button_LT(){
		return this.Button_Generic(7)
	}
	Button_RT(){
		return this.Button_Generic(8)
	}
	Button_Minus(){
		return this.Button_Generic(9)
	}
	Button_Plus(){
		return this.Button_Generic(10)
	}
	Button_Start(){
		return this.Button_Generic(11)
	}
	Button_Generic(key) {
		s := this.source
		GetKeyState, buttonDown, %s%Joy%key%
		return buttonDown == "D"
	}
	
	;POV
	POV() {
		s := this.source
		GetKeyState, pov, %s%JoyPOV
		povNew := 0
		; Up
		if pov = 0
			povNew := 1
		; Up-Right
		else if pov = 4500
			povNew := 2
		; Right
		else if pov = 9000
			povNew := 3
		; Down-Right
		else if pov = 13500
			povNew := 4
		; Down
		else if pov = 18000
			povNew := 5
		; Down-Left
		else if pov = 22500
			povNew := 6
		; Left
		else if pov = 27000
			povNew := 7
		; Up-Left
		else if pov = 31500
			povNew := 8
		return povNew
	}
	
	;AXES
	Axis_LX(){
		return this.Axis_Generic("X")
	}
	Axis_LY(){
		return this.Axis_Generic("Y")
	}
	Axis_RX(){
		return this.Axis_Generic("Z")
	}
	Axis_RY(){
		return this.Axis_Generic("R")
	}
	;asdf
	Axis_Generic(axis_name){
		s := this.source
		GetKeyState, x, %s%Joy%axis_name%
		r := x * 2 - 100
		return r
	}
}