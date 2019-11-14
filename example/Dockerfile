# docker build -t lambdanative .
# docker run --rm -v $(pwd):/app lambdanative

FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
RUN apt-get update && apt-get install cmake clang-3.9 libicu-dev uuid-dev libcurl4-openssl-dev zlib1g-dev libkrb5-dev -y
ENTRYPOINT ["dotnet", "publish", "-c", "Release", "-r", "linux-x64"]