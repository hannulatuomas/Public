#!/bin/bash

RED='\033[0;031m'
YELLOW='\033[0;033m'
NC='\033[0;0m'

if [ "$1" == "" ]
then
	echo -e ${RED}"Usage: $0 <file>"${NC}
	exit 1
fi

file=$1
#cat $file | grep 'eval' > /dev/null

#if [ $? -ne 0 ]
#then
#	echo "We don't have a new file."
#	exit 1
#fi

# We have a file !
#echo -e ${GREEN}"We have a file: $(du -sh $file)"${NC}

if [ "$(file $file | grep "shell archive")" != "" ]
then
	echo -e ${YELLOW}"shell archive: $(du -sh $file)"${NC}
	sh $file
	rm $file
elif [ "$(file $file | grep "bzip2")" != "" ]
then
	echo -e ${YELLOW}"bzip2: $(du -sh $file)"${NC}
	mv $file $file.bz2
	bunzip2 $file.bz2
elif [ "$(file $file | grep "tar")" != "" ]
then
	echo -e ${YELLOW}"tar: $(du -sh $file)"${NC}
	mv $file $file.tar
	tar xvf $file.tar
elif [ "$(file $file | grep "current ar")" != "" ]
then
	echo -e ${YELLOW}"current ar archive: $(du -sh $file)"${NC}
	mv $file $file.ar
	ar xv $file.ar
	rm $file.ar
elif [ "$(file $file | grep "Zip")" != "" ]
then
	echo -e ${YELLOW}"Zip: $(du -sh $file)"${NC}
	mv $file $file.zip
	unzip $file.zip
	rm $file.zip
elif [ "$(file $file | grep "cpio")" != "" ]
then
	echo -e ${YELLOW}"cpio: $(du -sh $file)"${NC}
	mv $file $file.cpio
	cpio --file $file.cpio --extract
	rm $file.cpio
elif [ "$(file $file | grep "gzip")" != "" ]
then
	echo -e ${YELLOW}"gzip: $(du -sh $file)"${NC}
	mv $file $file.gz
	gunzip $file.gz
elif [ "$(file $file | grep "lzip")" != "" ]
then
	echo -e ${YELLOW}"lzip: $(du -sh $file)"${NC}
	mv $file $file.lz
	lzip -d $file.lz
elif [ "$(file $file | grep "LZ4")" != "" ]
then
	echo -e ${YELLOW}"LZ4: $(du -sh $file)"${NC}
	mv $file $file.lz4
	lz4 -d $file.lz4 $file
	rm $file.lz4
elif [ "$(file $file | grep "LZMA")" != "" ]
then
	echo -e ${YELLOW}"LZMA: $(du -sh $file)"${NC}
	mv $file $file.lzma
	unlzma $file.lzma
elif [ "$(file $file | grep "lzop")" != "" ]
then
	echo -e ${YELLOW}"lzop: $(du -sh $file)"${NC}
	mv $file $file.lzop
	lzop -x $file.lzop
	rm $file.lzop
elif [ "$(file $file | grep "XZ")" != "" ]
then
	echo -e ${YELLOW}"XZ: $(du -sh $file)"${NC}
	mv $file $file.xz
	unxz $file.xz
elif [ "$(file $file | grep "ASCII")" != "" ]
then
	echo -e ${YELLOW}"ASCII: $(du -sh $file)"${NC}
	mv $file $file.txt
	echo -e ${YELLOW}"TXT:"${NC}
	tail -c 100 $file.txt
	echo -e ${YELLOW}"xxd:"${NC}
	cat $file.txt | xxd -r -p
	echo -e ${YELLOW}"base64:"${NC}
	base64 -d $file.txt
elif [ "$(file $file | grep "empty")" != "" ]
then
	echo -e ${RED}"Empty File $file!"${NC}
	exit 1
else
	echo -e ${RED}"Wrong File type $(file $file)!"${NC}
	exit 1
fi

#echo "#\!/bin/bash\n" 
