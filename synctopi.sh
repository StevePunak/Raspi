#!/bin/bash
#set -x

# Get directory we are running out of
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
RUN_FILE=$SCRIPT_DIR/running.hosts

if [ -z "$1" ]; then
	echo "Must specify source folder"
	exit 1
fi

if [ -z "$2" ]; then
	echo "Must specify output folder"
	exit 1
fi

is_host_up()
{
	cat $RUN_FILE | grep $1
	return $?
}

HOSTS="raspi1 raspi2 raspi3"
OUTPUT_DIRECTORY=/home/pi/opt/$2
for host in $HOSTS
do
	if is_host_up $host; then
		echo "Syncing files to pis from $1 into $host"
		rsync -ruvzh $1/*.exe pi@$host:$OUTPUT_DIRECTORY
		rsync -ruvzh $1/*.dll pi@$host:$OUTPUT_DIRECTORY
		if ls $1/*.so 1> /dev/null 2>&1;  then
			rsync -ruvzh $1/*.so pi@$host:$OUTPUT_DIRECTORY
		fi
	fi
done
