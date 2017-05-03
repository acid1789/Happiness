del Happiness_Client_Dist.zip
rmdir /S /Q Dist_C
PING 1.1.1.1 -n 1 -w 1000 >NUL
mkdir Dist_C
cd Dist_C
mkdir Launcher

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
Launcher\PatchGen\bin\Release\PatchGen.exe bin\WindowsGL\Release Dist_C\Client
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" "/ODist_C\Launcher\" Launcher\InnoSetup\Happiness.iss

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('Dist_C', 'Happiness_Client_Dist.zip'); }"