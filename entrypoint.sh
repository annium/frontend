#!/bin/bash

set -e

rm -rf debug

echo "Restore tools"
dotnet tool restore

echo "Build..."
make build-debug > debug.log

echo "Debug workflow completed successfully!"