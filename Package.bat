rmdir /S /Q bin
MSBuild Happiness.sln /t:Clean,Build /property:Configuration=Release

call Package_Server.bat
call Package_Client.bat
