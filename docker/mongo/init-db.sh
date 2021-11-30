#! /bin/bash
if [ $(mongo localhost:27017 --eval 'db.getMongo().getDBNames().indexOf("mydb")' --quiet) -lt 0 ]; then
    echo "mydb does not exist"
else
    echo "mydb exists"
fi
