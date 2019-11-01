REM USAGE: Install.bat <DEBUG/RELEASE> <UUID>
REM Example: Install.bat RELEASE com.barraider.spotify
setlocal
cd /d %~dp0
cd %1

REM MAKE SURE THE FOLLOWING ARE CORRECT
REM ALSO, UPDATE YOUR_USERNAME ON LINE 18
SET OUTPUT_DIR="C:\TEMP"
SET DISTRIBUTION_TOOL="e:\Projects\DotNet\Stream Deck Distribution\DistributionTool.exe"
SET STREAM_DECK_FILE="D:\Program Files\Elgato\StreamDeck\StreamDeck.exe"

taskkill /f /im streamdeck.exe
taskkill /f /im %2.exe
timeout /t 2
del %OUTPUT_DIR%\%2.streamDeckPlugin
%DISTRIBUTION_TOOL% %2.sdPlugin %OUTPUT_DIR%
rmdir C:\Users\YOUR_USERNAME\AppData\Roaming\Elgato\StreamDeck\Plugins\%2.sdPlugin /s /q
START "" %STREAM_DECK_FILE%
timeout /t 3
%OUTPUT_DIR%\%2.streamDeckPlugin
