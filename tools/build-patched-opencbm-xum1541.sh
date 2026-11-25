#!/bin/bash
# Build the patched OpenCBM xum1541 plugin that preloads libkeyutils to suppress
# the harmless but noisy "/usr/lib/libudev.so.1: undefined symbol: keyctl" warning
# seen with systemd-libs/libudev on some Linux systems.
#
# Usage:
#   tools/build-patched-opencbm-xum1541.sh [source-dir] [out-dir]
#
# Defaults:
#   source-dir: /tmp/opencode/OpenCBM
#   out-dir:    tools/opencbm
set -euo pipefail

REPO=$(cd "$(dirname "$0")/.." && pwd)
SRC=${1:-/tmp/opencode/OpenCBM}
OUT=${2:-$REPO/tools/opencbm}
PATCH="$REPO/patches/opencbm-xum1541-keyutils-preload.patch"

if [ ! -d "$SRC/.git" ] && [ ! -f "$SRC/opencbm/lib/plugin/xum1541/xum1541.c" ]; then
    echo "Cloning OpenCBM source..."
    rm -rf "$SRC"
    git clone --depth 1 https://github.com/OpenCBM/OpenCBM.git "$SRC"
fi

cd "$SRC"
echo "Applying patch (idempotent)..."
patch -p1 --forward < "$PATCH" || true

echo "Building libmisc..."
make -C opencbm/libmisc -f LINUX/Makefile -j"$(nproc)"

echo "Building xum1541 plugin..."
make -C opencbm/lib/plugin/xum1541 -f LINUX/Makefile -j"$(nproc)"

mkdir -p "$OUT"
cp opencbm/lib/plugin/xum1541/libopencbm-xum1541.so.0.4.99.104 "$OUT/"

echo "Built: $OUT/libopencbm-xum1541.so.0.4.99.104"
echo
echo "To install system-wide (backs up existing plugin):"
echo "  sudo cp /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104 /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104.bak"
echo "  sudo cp $OUT/libopencbm-xum1541.so.0.4.99.104 /usr/lib/opencbm/plugin/libopencbm-xum1541.so.0.4.99.104"
