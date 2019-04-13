%windir%\sysnative\bash.exe -c "`echo '$(ProjectDir)synctopi.sh' | sed -re 's/\\\\/\//g; s/([A-Z]):/\/mnt\/\L\1\E/i;'`"
