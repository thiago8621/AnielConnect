@ECHO off

@ECHO Desinstalando Servico...
@SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\
@InstallUtil /i %0\..\Aniel.Connect.ExecucaoService.exe
@pause

@ECHO ...Iniciando Servico...
net start "Aniel.Connect.ExecucaoService"

@pause