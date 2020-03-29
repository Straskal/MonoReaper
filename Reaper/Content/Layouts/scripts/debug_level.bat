set lvl=%1
echo Debugging level:" %lvl%
dotnet build ..\..\..\Core.sln --no-restore & dotnet ..\..\..\Reaper\bin\Debug\netcoreapp2.0\Reaper.dll %lvl%