#!/bin/bash
echo "Syncing files to pi"
rsync -ruvzh * pi@raspi:~/opt/trackbot