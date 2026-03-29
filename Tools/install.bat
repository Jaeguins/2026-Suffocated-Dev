@echo off
setlocal

set SCRIPT_DIR=%~dp0
set PIP=%SCRIPT_DIR%pyenv\Scripts\pip.exe
set REQUIREMENTS=%SCRIPT_DIR%requirements.txt

if not exist "%PIP%" (
    echo [ERROR] pip not found: %PIP%
    exit /b 1
)

if not exist "%REQUIREMENTS%" (
    echo [ERROR] requirements.txt not found: %REQUIREMENTS%
    exit /b 1
)

echo Installing packages from requirements.txt...
"%PIP%" install -r "%REQUIREMENTS%"

if %errorlevel% neq 0 (
    echo [ERROR] Installation failed.
    exit /b 1
)

echo Done.
pause
endlocal
