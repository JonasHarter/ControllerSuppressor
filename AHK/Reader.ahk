;This is supposed to be an interface :)
class Reader {
	; The Joystick that provides the input
	source := 0
	
	__New(source)
    {
		this.source := source
    }
	
	;BUTTONS
	;Returns true if button is pressed
	Button_A(){
		MsgBox, "Not Implemented"
	}
	Button_B(){
		MsgBox, "Not Implemented"
	}
	Button_X(){
		MsgBox, "Not Implemented"
	}
	Button_Y(){
		MsgBox, "Not Implemented"
	}
	Button_L(){
		MsgBox, "Not Implemented"
	}
	Button_R(){
		MsgBox, "Not Implemented"
	}
	Button_LT(){
		MsgBox, "Not Implemented"
	}
	Button_RT(){
		MsgBox, "Not Implemented"
	}
	Button_Minus(){
		MsgBox, "Not Implemented"
	}
	Button_Plus(){
		MsgBox, "Not Implemented"
	}
	Button_Start(){
		MsgBox, "Not Implemented"
	}
	
	;POV
	POV() {
		MsgBox, "Not Implemented"
	}
	
	;AXES
	Axis_LX(){
		MsgBox, "Not Implemented"
	}
	Axis_LY(){
		MsgBox, "Not Implemented"
	}
	Axis_RX(){
		MsgBox, "Not Implemented"
	}
	Axis_RY(){
		MsgBox, "Not Implemented"
	}
}