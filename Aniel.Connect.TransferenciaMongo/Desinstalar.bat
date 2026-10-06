@ECHO off

@ECHO ...Parando Servico...
net stop "Aniel.Connect.TransferenciaMongo"

@ECHO Desinstalando Servico...
@SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\
@InstallUtil /u %0\..\Aniel.Connect.TransferenciaMongo.exe
@pause