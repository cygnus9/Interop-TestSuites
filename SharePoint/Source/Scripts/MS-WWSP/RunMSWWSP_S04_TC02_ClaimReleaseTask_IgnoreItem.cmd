@echo off
pushd %~dp0
"%VS120COMNTOOLS%..\IDE\mstest" /test:Microsoft.Protocols.TestSuites.MS_WWSP.S04_ClaimReleaseTask.MSWWSP_S04_TC02_ClaimReleaseTask_IgnoreItem /testcontainer:..\..\MS-WWSP\TestSuite\bin\Debug\MS-WWSP_TestSuite.dll /runconfig:..\..\MS-WWSP\MS-WWSP.testsettings /unique
pause