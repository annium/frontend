#!/bin/bash

set -ex

make clean
make build-debug > local.log