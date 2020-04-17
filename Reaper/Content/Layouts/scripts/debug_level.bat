set lvl=%1
echo Debugging level:" %lvl%
start "C:\Program Files (x86)\NoPipeline\NoPipeline.exe" C:\git\MonoReaper\Reaper\Content\Content.npl
dotnet build ..\..\..\Core.sln --no-restore & dotnet ..\..\..\Reaper\bin\Debug\netcoreapp2.0\Reaper.dll %lvl%