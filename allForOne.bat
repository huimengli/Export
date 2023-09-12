::@echo off
setlocal enabledelayedexpansion

set source_folder=%~dp0bin\Debug\
set destination_folder=%~dp0dlls\
set ILMerge_folder=%~dp0packages\ILMerge.3.0.41\tools\net452\

xcopy "%source_folder%*.dll" "%destination_folder%" /Y /I
del "%destination_folder%UnityEditor.dll"
del "%destination_folder%UnityEngine.dll"
cd "%destination_folder%"

set "all_files="

for /f %%f in ('dir /b %destination_folder%') do (
	set "all_files=!all_files! %%f"
)

echo !all_files!
echo !lib_files!
"%ILMerge_folder%ILMerge.exe" /lib:%~dp0lib /out:ExportAll.dll!all_files!
pause