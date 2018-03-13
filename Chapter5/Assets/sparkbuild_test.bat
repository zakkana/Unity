@ECHO OFF
ECHO Bumping Version
ECHO Building Win
"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -executeMethod FHG.BuildScripts.Build_Spark --win --release --bumpversion
IF %ERRORLEVEL% NEQ 0 (
	ECHO Build Failed, Check Unity Editor Logs
	EXIT /B 1
)

ECHO Script Complete!