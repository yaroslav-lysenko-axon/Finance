### MinIO server local environment

1. Go to the provisioning folder: <br>
```cd provisioning/minio-server```

2. Start docker container with minio: <br>
```./build.sh```

3. Run MinIO Server: <br>
```./run-daemon.sh```

Go to the web interface http://172.17.0.2:9000 and create a bucket. Add write permissions to the created bucket.

NOTE: 172.17.0.2 -  your docker ip address container