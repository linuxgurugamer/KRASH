@echo off
set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)

type krash.version
set /p VERSION= "Enter version: "

set d=%HOMEDIR\install
if exist %d% goto one
mkdir %d%
:one
set d=%HOMEDIR%\install\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%HOMEDIR%\install\Gamedata\KRASH
if exist %d% goto three
mkdir %d%
:three
set d=%HOMEDIR%\install\Gamedata\KRASH\Plugins
if exist %d% goto four
mkdir %d%
:four
set d=%HOMEDIR%\install\Gamedata\KRASH\Textures
if exist %d% goto five
mkdir %d%
:five
set d=%HOMEDIR%\install\Gamedata\KRASH\PluginData
if exist %d% goto six
mkdir %d%
:six



del /y %HOMEDIR%\install\Gamedata\KRASH\*.*
del /y %HOMEDIR%\install\Gamedata\KRASH\Plugins\*.*
del /y %HOMEDIR%\install\Gamedata\KRASH\PluginData\*.*
del /y %HOMEDIR%\install\Gamedata\KRASH\Textures\*.*

xcopy Textures\KRASH*.png   %HOMEDIR%\install\GameData\KRASH\Textures /Y
copy MiniAVC.dll %HOMEDIR%\install\Gamedata\KRASH
copy bin\Release\KRASH.dll %HOMEDIR%\install\Gamedata\KRASH\Plugins
copy  KRASH.version %HOMEDIR%\install\Gamedata\KRASH\KRASH.version
copy ..\README.md %HOMEDIR%\install\Gamedata\KRASH
copy README4Modders.txt  %HOMEDIR%\install\Gamedata\KRASH
copy KRASHWrapper.cs  %HOMEDIR%\install\Gamedata\KRASH
copy ChangeLog.txt %HOMEDIR%\install\Gamedata\KRASH
copy KRASH.cfg %HOMEDIR%\install\Gamedata\KRASH\PluginData

rem copy KRASHCustom.cfg %HOMEDIR%\install\Gamedata\KRASH

%HOMEDRIVE%
cd %HOMEDIR%\install

set FILE="%RELEASEDIR%\KRASH-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\KRASH
