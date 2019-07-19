#!/bin/bash
echo "Syncing files to pi 3"
#rsync -ruvzh * pi@raspi1:~/opt/testing
#rsync -ruvzh * pi@raspi2:~/opt/testing
rsync -ruvzh * pi@raspi3:~/opt/testing
