#!/bin/bash

docker run -d -it -p 9000:9000 --name minio-serv \
  -e "MINIO_ACCESS_KEY=minioadmin" \
  -e "MINIO_SECRET_KEY=minioadmin" \
  -v /home/minio/data:/data \
  minio/minio server /data
