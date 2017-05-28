@echo off
cd

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"


set VERSIONFILE=krash.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%



xcopy Textures\KRASH*.png   ..\GameData\KRASH\Textures /Y
copy MiniAVC.dll ..\Gamedata\KRASH
copy bin\Release\KRASH.dll ..\Gamedata\KRASH\Plugins
copy  KRASH.version ..\Gamedata\KRASH\KRASH.version
copy ..\README.md ..\Gamedata\KRASH
copy README4Modders.txt  ..\Gamedata\KRASH
copy KRASHWrapper.cs  ..\Gamedata\KRASH
copy ChangeLog.txt ..\Gamedata\KRASH
copy KRASH.cfg ..\Gamedata\KRASH\PluginData
copy KRASHCustom.cfg ..\Gamedata\KRASH\PluginData

rem copy KRASHCustom.cfg ..\Gamedata\KRASH

cd ..

set FILE="%RELEASEDIR%\KRASH-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\KRASH

pause