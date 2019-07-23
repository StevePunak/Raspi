#!/bin/bash
echo "Syncing files to pi 3 from $ProjectDir and 1 is $1"
#rsync -ruvzh * pi@raspi1:~/opt/testing
#rsync -ruvzh * pi@raspi2:~/opt/testing
rsync -ruvzh $1/* pi@raspi3:~/opt/testing
