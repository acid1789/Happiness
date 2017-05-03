del Happiness_Server_Dist.zip
rmdir /S /Q Dist_S
PING 1.1.1.1 -n 1 -w 1000 >NUL
mkdir Dist_S
cd Dist_S
mkdir GlobalServer
mkdir HappinessServer

cd GlobalServer
copy ..\..\GlobalServer\bin\Release\*
cd ..

cd HappinessServer
copy ..\..\HappinessServer\bin\Release\*
cd ..

cd..
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('Dist_S', 'Happiness_Server_Dist.zip'); }"