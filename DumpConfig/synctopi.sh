#!/bin/bash
echo "Syncing files to pi's from $ProjectDir and 1 is $1"
rsync -ruvzh $1/* pi@raspi1:~/opt/trackbot
#rsync -ruvzh $1/* pi@raspi2:~/opt/testing
rsync -ruvzh $1/* pi@raspi3:~/opt/trackbot
