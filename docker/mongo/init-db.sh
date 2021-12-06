#! /bin/bash
mongo br_customers --eval "db.dropDatabase()"
mongo br_product_catalog --eval "db.dropDatabase()"
mongorestore
