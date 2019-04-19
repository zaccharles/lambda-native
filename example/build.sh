#!/bin/bash

rm -f $(pwd)/publish/bootstrap
rm -f $(pwd)/publish/package.zip

docker run --rm -v $(pwd)/publish:/app/out lambdanative
cd publish
cp LambdaNative.Example bootstrap
zip package.zip bootstrap

# aws s3 cp package.zip s3://<bucket>/package.zip