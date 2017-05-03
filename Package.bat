del Happiness_Dist.zip
rmdir /S /Q bin
MSBuild Happiness.sln /t:Clean,Build /property:Configuration=Release
rmdir /S /Q Dist
mkdir Dist
cd Dist
mkdir GlobalServer
mkdir HappinessServer
mkdir Launcher

cd GlobalServer
copy ..\..\GlobalServer\bin\Release\*
cd ..

cd HappinessServer
copy ..\..\HappinessServer\bin\Release\*
cd ..

cd Launcher
copy ..\..\Launcher\HappinessLauncher\bin\Release\HappinessLauncher.exe
copy ..\..\Launcher\HappinessLauncher\bin\Release\PatchLib.dll
cd ..
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('Launcher', 'Launcher.zip'); }"
move Launcher.zip Launcher\Launcher.zip
cd Launcher
..\..\Launcher\Hashgen\bin\Release\Hashgen.exe -out=launcher.manifest HappinessLauncher.exe PatchLib.dll
del HappinessLauncher.exe
del PatchLib.dll
del Hashgen.log
cd ..

cd ..
Launcher\PatchGen\bin\Release\PatchGen.exe bin\WindowsGL\Release Dist\Client

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('Dist', 'Happiness_Dist.zip'); }"