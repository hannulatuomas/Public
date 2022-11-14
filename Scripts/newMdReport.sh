#!/bin/bash

if [ "$1" == "" ]
then
        echo "Usage: $0 <target> <IP>"
        exit 1
fi

target=$1
file=PT-report-$target.md
IP=""

if [ "$2" != "" ]
then
        IP=$2
fi

cat << EOF > ./$file
# Pentest report $target

1. **Initial Data**
	- Target: $target
	- IP: $IP

		\`\`\`
		
		\`\`\`
2. **Findings**
	- Open ports:
EOF
