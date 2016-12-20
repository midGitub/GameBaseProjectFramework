set ExcelFileName=ATableMode

call xlsc_lua.bat %ExcelFileName% TableModeOne
call xlsc_lua.bat %ExcelFileName% TableModeTwo

call xlsc_cs.bat %ExcelFileName% TableModeOne
call xlsc_cs.bat %ExcelFileName% TableModeTwo

pause