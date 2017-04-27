del Happiness_Dist.zip
rmdir /S /Q bin
MSBuild Happiness.sln /t:Clean,Build /property:Configuration=Release
rmdir /S /Q Dist
mkdir Dist
cd Dist
mkdir GlobalServer
mkdir HappinessServer
mkdir Client

cd GlobalServer
copy ..\..\GlobalServer\bin\Release\*
cd ..

cd HappinessServer
copy ..\..\HappinessServer\bin\Release\*
cd ..

cd ..
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('Dist', 'Happiness_Dist.zip'); }"