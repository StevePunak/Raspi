#!/bin/bash

# Get directory we are running out of
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
RUN_FILE=$SCRIPT_DIR/running.hosts
echo $RUN_FILE > /tmp/mystuff
HOSTS="raspi1 raspi2 raspi3 feyd"

# fping -c1 $HOSTS 2> /dev/null | cut -d' ' -f 1 > $RUN_FILE
echo "fping -c1 $HOSTS | cut -d' ' -f 1" >> /tmp/mystuff
fping -c1 $HOSTS 2> /dev/null | cut -d' ' -f 1 > $RUN_FILE
