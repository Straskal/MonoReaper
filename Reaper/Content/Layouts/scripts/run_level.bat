set lvl=%1
echo "Running level:" %lvl%
dotnet build ..\..\..\Core.sln --no-restore & dotnet ..\..\..\Reaper\bin\Debug\netcoreapp2.0\Reaper.dll %lvl%