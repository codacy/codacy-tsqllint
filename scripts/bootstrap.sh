#!/usr/bin/env bash

set -e

VERSION="$(tr -d '\n' < tsqllint.version)"
FILE="$VERSION.tar.gz"
RESOURCES_DIR=".resources/"

# Remove possible old source
rm -rf tsqllint/
mkdir -p "$RESOURCES_DIR"
wget -nc -q -O "$RESOURCES_DIR/$FILE" "https://github.com/tsqllint/tsqllint/archive/$FILE"
tar -zxvf "$RESOURCES_DIR/$FILE"
mv "tsqllint-$VERSION" tsqllint
