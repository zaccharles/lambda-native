#!/bin/bash

#
# SHOULD BUILD IMAGE BEFORE
# docker build -t lambdanative .
#

rm -f $(pwd)/bootstrap
rm -f $(pwd)/package.zip
docker run --rm -v $(pwd):/app lambdanative
cp bin/Release/netcoreapp2.1/linux-x64/native/LambdaNative.Example bootstrap
zip package.zip bootstrap
# aws s3 cp package.zip s3://backend-layer [--profile [PROFILE-NAME]]