#!/bin/bash

set -ex

make clean
docker build -t debug .
docker run \
  --name blazor-debug \
  -v "$HOME/.nuget/packages:/root/.nuget/packages" \
  debug
rm -rf debug
docker cp blazor-debug:/app debug
touch debug/.xs.ignore
docker rm blazor-debug