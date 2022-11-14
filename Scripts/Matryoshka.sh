#!/bin/bash

if [ "$1" == "" ]
then
	echo "Usage: $0 <file>"
	exit 1
fi

file=$1

mkdir matroyshkaTmp
cp $file matroyshkaTmp

cd ./matroyshkaTmp

while [ 1 ]
do
	file=$(ls *)
	/opt/myScripts/Decompresser.sh ./$file
	exit=$?
	if [ $exit -ne 0 ]
	then
		echo "Exit code: $exit"
		break
	elif [ "$(file $file | grep "ASCII")" != "" ]
	then
		/opt/myScripts/Decompresser.sh ./$file
		file=$(ls *)
		mv ./$file ../$file
		break
	fi
done

cd ..
rm -r ./matroyshkaTmp
