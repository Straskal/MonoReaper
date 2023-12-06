set lvl=%1
echo Debugging level:" %lvl%
dotnet build ..\..\..\Core.sln --no-restore & dotnet ..\..\..\Reaper\bin\Debug\netcoreapp3.1\Reaper.dll %lvl%