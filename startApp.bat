timeout /T 30
:begin
taskkill /F /IM explorer.exe
::cd move to app directory
start /WAIT appname.exe -screen-width 1080 -screen-height 1920 -screen-fullscreen 0 -popupwindow
start explorer.exe
timeout /T 10
goto begin