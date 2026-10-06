@ECHO off

@ECHO Desinstalando Servico...
@SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\
@InstallUtil /i %0\..\Aniel.Connect.EmailService.exe
@pause

@ECHO ...Iniciando Servico...
net start "Aniel.Connect.EmailService"

@pause