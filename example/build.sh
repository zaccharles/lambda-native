#!/bin/bash

dotnet publish -r linux-x64 -c release
cp bin/release/netcoreapp*/linux-x64/native/* bootstrap
# zip package.zip bootstrap
# aws s3 cp package.zip s3://<bucket>/package.zip