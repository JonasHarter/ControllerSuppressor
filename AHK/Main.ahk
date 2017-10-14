#Persistent
#SingleInstance force
Menu, Tray, icon, logo.ico

;Speedup
#NoEnv
#MaxHotkeysPerInterval 99000000
#HotkeyInterval 99000000
#KeyHistory 0
SendMode Input
ListLines Off
Process, Priority, , A
SetBatchLines, -1
SetKeyDelay, -1, -1
SetMouseDelay, -1
SetDefaultMouseSpeed, 0
SetWinDelay, -1

#include TTS.ahk
#include JoyManager.ahk

;TODO
;auto-det profile based on running game?
;caller interface
;reader interface