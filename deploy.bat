
@ech off
set H=R:\KSP_1.3.0_dev
echo %H%

set d=%H%
if exist %d% goto one
mkdir %d%
:one
set d=%H%\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%H%\Gamedata\KRASH
if exist %d% goto three
mkdir %d%
:three
set d=%H%\Gamedata\KRASH\Plugins
if exist %d% goto four
mkdir %d%
:four
set d=%H%\Gamedata\KRASH\Textures
if exist %d% goto five
mkdir %d%
:five
set d=%H%\Gamedata\KRASH\PluginData
if exist %d% goto six
mkdir %d%
:six



xcopy Source\Textures\KRASH*.png   %H%\GameData\KRASH\Textures /Y
copy ..\MiniAVC.dll %H%\Gamedata\KRASH
copy Source\bin\Debug\KRASH.dll %H%\Gamedata\KRASH\Plugins
copy KRASH.version %H%\Gamedata\KRASH\KRASH.version
copy Source\KRASH.cfg %H%\Gamedata\KRASH\PluginData
copy Source\KRASHCustom.cfg %H%\Gamedata\KRASH\PluginData

