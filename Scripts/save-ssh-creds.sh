#!/bin/bash

RED='\033[0;031m'
NC='\033[0;0m'

if [ "$1" == "" ] || [ "$2" == "" ]
then
	echo -e ${RED}"Usage: $0 <login> <password>"${NC}
	exit 1
fi

login="$1"
password="$2"
domain="$3"
port="$4"

if [ "$3" == "" ]
then
	domain="bandit.labs.overthewire.org"
fi

if [ "$4" == "" ]
then
	port="2220"
fi

echo $password > password-${login}.txt

if [ "$?" != "0" ]
then
	echo -e ${RED}"Error!"${NC}
	exit 1
fi

cat << EOF > ./connect-${login}.sh
#!/bin/bash
sshpass -p $(cat password-${login}.txt) ssh ${login}@${domain} -p ${port}
EOF

chmod +x connect-${login}.sh

cat connect-${login}.sh
