#!/bin/bash
echo "Syncing files to pi"
#rsync -ruvzh * pi@raspi1:~/opt/trackbot
#rsync -ruvzh * pi@raspi2:~/opt/trackbot
rsync -ruvzh * pi@raspi3:~/opt/trackbot
