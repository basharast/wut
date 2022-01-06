::What is AllowLocalhostAccess.bat:
::if you want to use the desktop telnet you will face restrictions 
::when the app tries to connect to the localhost (127.0.0.1) 
::to solve that just run AllowAppLocalhost.bat as Adminitrator

@echo off
checknetisolation loopbackexempt -a -n=WinUniveralTool_eyr0bca9nc39y
echo Done
timeout 10