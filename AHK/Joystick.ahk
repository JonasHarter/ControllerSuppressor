
#include BattlePadRaphnetReader.ahk
#include xBoxCaller.ahk

class JoyStick
{
	; The Joystick this objects reads from
	reader := 0
	; The xBox controller we map the input to
	caller := 0
	
    __New(source)
    {
		; Currently we expect every controller to be a hori battlepad
		this.reader := new BattlePadRaphnetReader(source)
    }
	
	holdCounter := 0
	static limit := 40
	profile := 0
	; Handles registering and controls
	Control()
	{
		; Check wether this is already bound to a xBox controller
		isBound := this.caller != 0
		; Check for start key hold
		if(this.reader.Button_Start())
		{
			this.holdCounter += 1
		}
		else {
			this.holdCounter := 0
		}
		if(this.holdCounter < this.limit)
		{
			return
		}
		pov := this.reader.POV()
		; (Un)register
		if((isBound and pov == 0) or !isBound)
		{
			if(!isBound)
			{
				; Try to register. Destory  object on fail
				this.caller := new xBoxCaller()
				if(!this.caller.Register())
				{
					this.caller := 0
				}
			}
			else
			{
				this.caller.Unregister()
				this.caller := 0
			}
		}
		; Check for Keymap change
		else if(isBound and pov != 0)
		{
			if(pov == 3)
			{
				this.profile := 1
			}
			else if(pov == 5)
			{
				this.profile := 2
			}
			else if(pov == 7)
			{
				this.profile := 3
			}
			else
			{
				this.profile := 0
			}
			Speak("Player " . this.caller.target . " switched to Profile " . this.profile)
		}
		this.holdCounter := 0
	}

	
	Map()
	{
		; Abort if unregistered
		if(this.caller == 0)
		{
			return
		}
		s := this.profile
		if(s == 0)
		{
			this.Mapper_P1()
		}
		else if(s == 1)
		{
			this.Mapper_P2()
		}
		else if(s == 2)
		{
			this.Mapper_P3()
		}
		else if(s == 3)
		{
			this.Mapper_P4()
		}
	}
	
	
	;MAPPERS
	;Functions that map the reader to the caller
	
	;Normal
	Mapper_P1()
	{
		c := this.caller
		r := this.reader
		;BUTTONS
		c.Button_A(r.Button_A())
		c.Button_B(r.Button_B())
		c.Button_X(r.Button_X())
		c.Button_Y(r.Button_Y())
		c.Button_L(r.Button_L())
		c.Button_R(r.Button_R())
		c.Button_Start(r.Button_Start())
		;c.Button_Back(r.asdf)
		c.Button_LStick(r.Button_Minus())
		c.Button_RStick(r.Button_Plus())
		;POV
		c.POV(r.POV())
		;AXIS
		c.Axis_LX(r.Axis_LX())
		ly := r.Axis_LY() * -1
		c.Axis_LY(ly)
		c.Axis_RX(r.Axis_RX())
		ry := r.Axis_RY() * -1
		c.Axis_RY(ry)
		;TRIGGER
		if(r.Button_LT())
			c.Trigger_L(100)
		else
			c.Trigger_L(0)
		if(r.Button_RT())
			c.Trigger_R(100)
		else
			c.Trigger_R(0)
	}
	
	; X&B switched
	Mapper_P2()
	{
		c := this.caller
		r := this.reader
		;BUTTONS
		c.Button_A(r.Button_A())
		c.Button_B(r.Button_X())
		c.Button_X(r.Button_B())
		c.Button_Y(r.Button_Y())
		c.Button_L(r.Button_L())
		c.Button_R(r.Button_R())
		c.Button_Start(r.Button_Start())
		;tc.Button_Back(r.asdf)
		c.Button_LStick(r.Button_Minus())
		c.Button_RStick(r.Button_Plus())
		;POV
		c.POV(r.POV())
		;AXIS
		c.Axis_LX(r.Axis_LX())
		ly := r.Axis_LY() * -1
		c.Axis_LY(ly)
		c.Axis_RX(r.Axis_RX())
		ry := r.Axis_RY() * -1
		c.Axis_RY(ry)
		;TRIGGER
		if(r.Button_LT())
			c.Trigger_L(100)
		else
			c.Trigger_L(0)
		if(r.Button_RT())
			c.Trigger_R(100)
		else
			c.Trigger_R(0)
	}
	
	; Enslaved
	; http://strategywiki.org/wiki/Enslaved:_Odyssey_to_the_West/Controls
	Mapper_P3()
	{
		; Map r to c
		r := this.reader
		c := this.caller
		
		; A//Jump/Evade at B
		c.Button_A(r.Button_B())
		; B//Interact? at A
		c.Button_B(r.Button_A())
		; X//Atk at RT
		c.Button_X(r.Button_RT())
		; Y//StrongAtk at LT
		c.Button_Y(r.Button_LT())
		; LStick
		;c.Button_LStick()
		; RStick//Cloud at
		c.Button_RStick(r.Button_Minus())
		; L//Look/Command at X?
		c.Button_L(r.Button_X())
		; R//Yell at Y
		c.Button_R(r.Button_Y())
		; LT//Aim at L
		if(r.Button_L())
		{
			c.Trigger_L(100)
		} else {
			c.Trigger_L(0)
		}
		; RT//Shield at R
		if(r.Button_R())
		{
			c.Trigger_R(100)
		} else {
			c.Trigger_R(0)
		}
		
		; Start//Menu at Start
		c.Button_Start(r.Button_Start())
		; Back//Cloud at Plus
		c.Button_Back(r.Button_Plus())
		;POV//Left:PlasmaAmmo/Right:StunAmmo at POV
		c.POV(r.POV())
		; LAxis//Move at LAxis
		c.Axis_LX(r.Axis_LX())
		ly := r.Axis_LY() * -1
		c.Axis_LY(ly)
		; RAxis//Camera at RAxis
		rx := r.Axis_RX()
		c.Axis_RX(rx)
		ry := r.Axis_RY() * -1
		c.Axis_RY(ry)
	}
	
	; Var to tone down input frequency
	lock := false
	; Keyboard
	Mapper_P4()
	{
		r := this.reader
		; LStick as Mouse
		; B to slow
		lx := r.Axis_LX()
		if(lx < 0)
			lx *= -1
		if(lx >= 10)
		{
			lx *= 0.1
			if(r.Button_B())
				lx := 1
		}
		else
			lx := 0
		if(r.Axis_LX() < 0)
			lx *= -1
			
			
		ly := r.Axis_LY()
		if(ly < 0)
			ly *= -1
		if(ly >= 10)
		{
			ly *= 0.1
			if(r.Button_B())
				ly := 1
		}
		else
			ly := 0
		if(r.Axis_LY() < 0)
			ly *= -1

		MouseMove, lx, ly , 0, R
		; RStick as Wheel
		;ry := r.Axis_RY()
		;if(ry <= -40)
		;	Click WheelUp
		;else if(ry >= 40)
		;	Click WheelDown
		; Button
		if(r.Button_A())
		{
			if(!this.lock)
			{
				Click L
				this.lock := true
			}
		}
		else if(r.Button_X())
		{
			if(!this.lock)
			{
				Click R
				this.lock := true
			}
		}
		else if(r.Button_Y())
		{
			if(!this.lock)
			{
				Click M
				this.lock := true
			}
		}
		else if(r.Button_Minus())
		{
			if(!this.lock)
			{
				Send !{F4}
				this.lock := true
			}
		}
		else if(r.Button_Plus())
		{
			if(!this.lock)
			{
				Send ^w
				this.lock := true
			}
		}
		else if(r.Button_L())
		{
			if(!this.lock)
			{
				Send {PgUp}
				this.lock := true
			}
		}
		else if(r.Button_R())
		{
			if(!this.lock)
			{
				Send {PgDn}
				this.lock := true
			}
		}
		else if(r.Button_LT())
		{
			if(!this.lock)
			{
				Send ^+{Tab}
				this.lock := true
			}
		}
		else if(r.Button_RT())
		{
			if(!this.lock)
			{
				Send ^{Tab}
				this.lock := true
			}
		}
		else if(r.POV() != 0)
		{
			if(!this.lock)
			{
				pov := r.POV()
				if(pov == 1)
					Send {Up}
				else if(pov == 3)
					Send {Right}
				else if(pov == 5)
					Send {Down}
				else if(pov == 7)
					Send {Left}
				this.lock := true
			}
		}
		else
			this.lock := false
	}
}