#!/bin/bash
echo "Syncing files to pi 3"
rsync -ruvzh * pi@raspi3:~/opt/testing