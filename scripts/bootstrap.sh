#!/bin/sh

set -e

# Check if version file exists and is non-empty
if [ ! -s tsqllint.version ]; then
  echo "Error: tsqllint.version file is missing or empty."
  exit 1
fi

VERSION=$(< tsqllint.version)
FILE="v$VERSION.tar.gz"
RESOURCES_DIR=".resources/"

# Remove possible old source
rm -rf tsqllint/
mkdir -p "$RESOURCES_DIR"

# Download the source file if it does not already exist
if [ ! -f "$RESOURCES_DIR/$FILE" ]; then
  echo "Downloading tsqllint version $VERSION..."
  wget -q -O "$RESOURCES_DIR/$FILE" "https://github.com/tsqllint/tsqllint/archive/$FILE"
  if [ $? -ne 0 ]; then
    echo "Error: Failed to download tsqllint version $VERSION."
    exit 1
  fi
fi

# Extract the tarball
echo "Extracting tsqllint version $VERSION..."
tar -zxvf "$RESOURCES_DIR/$FILE"
if [ $? -ne 0 ]; then
  echo "Error: Failed to extract tsqllint version $VERSION."
  exit 1
fi

# Rename the extracted folder
mv "$RESOURCES_DIR/$FILE" tsqllint
if [ $? -ne 0 ]; then
  echo "Error: Failed to rename the extracted folder."
  exit 1
fi

echo "tsqllint version $VERSION downloaded and extracted successfully."
