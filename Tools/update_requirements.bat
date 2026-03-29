@echo off
setlocal

set SCRIPT_DIR=%~dp0
set PIP=%SCRIPT_DIR%pyenv\Scripts\pip.exe
set OUTPUT=%SCRIPT_DIR%requirements.txt

if not exist "%PIP%" (
    echo [ERROR] pip not found: %PIP%
    exit /b 1
)

echo Collecting installed packages...
"%PIP%" freeze > "%OUTPUT%"

if %errorlevel% neq 0 (
    echo [ERROR] pip freeze failed.
    exit /b 1
)

echo Done. Saved to: %OUTPUT%
pause
endlocal
