; Class that handles calling a vxBox Controller

Global DllPath := ".\vGenInterface.dll\"
;Global pathPlugin := DllPath . "PlugIn"
Global pathPlugInNext := DllPath . "PlugInNext"
Global pathPlugout := DllPath . "UnPlug"
Global pathButton := DllPath . "SetButton"
Global pathDPad := DllPath . "SetDpad"
Global pathAxis := DllPath . "SetAxis"
Global pathTrigger := DllPath . "SetTrigger"
Global hModule := DllCall("LoadLibrary", "Str", DllPath, "Ptr")

;TODO statics var paths
class XBoxCaller
{
	; The XBox controller's id that recieves the input (1-4)
	target := 0
	
	Register()
	{
		; 'result' is zero on success, 't' carries the controller number
		t := 0
		result := DllCall(pathPlugInNext, "uint*", t)
		if(!result)
		{
			this.target := t
			Speak("Player " . t . " registered")
			return true
		}
		Speak("Error")
		return false
	}
	
	Unregister()
	{
		x := this.target
		result := DllCall(pathPlugout, "uint", x)
		if(!result)
		{
			Speak("Player " . this.target . " unregistered")
			this.target := 0
			return
		}
		Speak("Error")
	}
	
	; BUTTONS
	Button_A(d){
		this.Button_Generic(0x1000, d)
	}
	Button_B(d){
		this.Button_Generic(0x2000, d)
	}
	Button_X(d){
		this.Button_Generic(0x4000, d)
	}
	Button_Y(d){
		this.Button_Generic(0x8000, d)
	}
	Button_L(d){
		this.Button_Generic(0x0100, d)
	}
	Button_R(d){
		this.Button_Generic(0x0200, d)
	}
	Button_Start(d){
		this.Button_Generic(0x0010, d)
	}
	Button_Back(d){
		this.Button_Generic(0x0020, d)
	}
	Button_LStick(d){
		this.Button_Generic(0x0040, d)
	}
	Button_RStick(d){
		this.Button_Generic(0x0080, d)
	}
	
	; d = true -> press button
	Button_Generic(button_code, d)
	{
		x := 0
		if(d)
		{
			x := 1
		}
		DllCall(pathButton, "uint", this.target, "ushort", button_code, "int", x)
	}
	
	;POV
	; POV goes from 1(Up) clockwise in half steps until 8(Up-Left), 0 is neutral
	POV(pov)
	{
		povNew := 0
		; Up
		if pov = 1
			povNew := 1
		; Up-Right
		else if pov = 2
			povNew := 9
		; Right
		else if pov = 3
			povNew := 8
		; Down-Right
		else if pov = 4
			povNew := 10
		; Down
		else if pov = 5
			povNew := 2
		; Down-Left
		else if pov = 6
			povNew := 6
		; Left
		else if pov = 7
			povNew := 4
		; Up-Left
		else if pov = 8
			povNew := 5
		DllCall(pathDPad, "uint", this.target, "uchar", povNew)
	}
	
	;AXES
	Axis_LX(value){
		this.Axis_Generic("Lx", value)
	}
	Axis_LY(value){
		this.Axis_Generic("Ly", value)
	}
	Axis_RX(value){
		this.Axis_Generic("Rx", value)
	}
	Axis_RY(value){
		this.Axis_Generic("Ry", value)
	}
	
	; Uses -100 to 100
	Axis_Generic(axis_name, value){
		path := pathAxis . axis_name
		; xBox goes from -32768 to 32767
		valNew := (value / 100) * 32767.0 * 0.95
		DllCall(path, "uint", this.target, "short", valNew)
	}
	
	;TRIGGER
	Trigger_L(value){
		this.Trigger_Generic("L", value)
	}
	Trigger_R(value){
		this.Trigger_Generic("R", value)
	}
	
	; Uses 0 to 100
	Trigger_Generic(trig_name, value){
		path := pathTrigger . trig_name
		; xBox goes from 0 to 255
		valNew := (value / 100) * 255
		DllCall(path, "uint", this.target, "uchar", valNew)
	}
}