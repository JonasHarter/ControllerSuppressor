
; Setup TTS Object
Global TTS := ComObjCreate("SAPI.SpVoice")
TTS.Voice := TTS.GetVoices().Item(1)

Speak(s)
{
	TTS.Speak(s)
}