#!/bin/bash
echo "Syncing files to pis from $ProjectDir and 1 is $1"
HOSTS=""
OUTPUT_DIRECTORY=/home/pi/opt/trackbot
for host in $HOSTS
do
	rsync -ruvzh $1/*.exe pi@$host:$OUTPUT_DIRECTORY
	rsync -ruvzh $1/*.dll pi@$host:$OUTPUT_DIRECTORY
	rsync -ruvzh $1/*.so pi@$host:$OUTPUT_DIRECTORY
done
