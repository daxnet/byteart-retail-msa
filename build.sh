#!/bin/bash
if [ "$1" == "" ] || [ "$1" == "all" ]; then
	docker-compose -f docker-compose.build.yaml build
else
	echo "Invalid arguments."
fi

