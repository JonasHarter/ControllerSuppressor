
#include Reader.ahk

class BattlePadRaphnetReader extends Reader {

	;BUTTONS
	Button_A(){
		return this.Button_Generic(5)
	}
	Button_B(){
		return this.Button_Generic(4)
	}
	Button_X(){
		return this.Button_Generic(3)
	}
	Button_Y(){
		return this.Button_Generic(2)
	}
	Button_L(){
		return this.Button_Generic(13)
	}
	Button_R(){
		return this.Button_Generic(8)
	}
	Button_LT(){
		return this.Button_Generic(6)
	}
	Button_RT(){
		return this.Button_Generic(7)
	}
	Button_Minus(){
		return this.Button_Generic(14)
	}
	Button_Plus(){
		return this.Button_Generic(1)
	}
	Button_Start(){
		return this.Button_Generic(15)
	}
	Button_Generic(key) {
		s := this.source
		GetKeyState, buttonDown, %s%Joy%key%
		return buttonDown == "D"
	}
	
	;POV
	POV() {
		s := this.source
		
		GetKeyState, upS, %s%Joy9
		up := upS == "D"
		GetKeyState, downS, %s%Joy10
		down := downS == "D"
		GetKeyState, rightS, %s%Joy11
		right := rightS == "D"
		GetKeyState, leftS, %s%Joy12
		left := leftS == "D"
		
		GetKeyState, pov, %s%JoyPOV
		povNew := 0
		; Up
		if up
		{
			if right
				povNew := 2
			else if Left
				povNew := 8
			else
				povNew := 1
		}
		else if down
		{
			if right
				povNew := 4
			else if Left
				povNew := 6
			else
				povNew := 5
		}
		else if right
			povNew := 3
		else if left
			povNew := 7
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
		return this.Axis_Generic("V")
	}
	Axis_RY(){
		return this.Axis_Generic("U")
	}
	
	Axis_Generic(axis_name){
		s := this.source
		;MsgBox % axis_name
		GetKeyState, x, %s%Joy%axis_name%
		;MsgBox % x
		r := x * 2 - 100
		return r
	}
}