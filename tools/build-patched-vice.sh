#!/bin/bash
# Build VICE 3.10 with the OpenCBM filename fix from patches/.
# Usage: tools/build-patched-vice.sh [source-dir] [install-prefix]
set -euo pipefail

REPO=$(cd "$(dirname "$0")/.." && pwd)
SRC=${1:-/tmp/opencode/vice-3.10}
PREFIX=${2:-/tmp/opencode/vice-patched}
TARBALL=${SRC}.tar.gz
PATCH="$REPO/patches/vice-3.10-opencbm-filename.patch"
URL='https://sourceforge.net/projects/vice-emu/files/releases/vice-3.10.tar.gz/download'

if [ ! -f "$SRC/configure" ]; then
    echo "Downloading VICE 3.10 source..."
    curl -L --fail -o "$TARBALL" "$URL"
    mkdir -p "$(dirname "$SRC")"
    tar xzf "$TARBALL" -C "$(dirname "$SRC")"
fi

cd "$SRC"
echo "Applying OpenCBM filename patch (idempotent)..."
patch -p1 --forward < "$PATCH" || true

echo "Configuring with GTK3 UI and OpenCBM real-device support..."
./configure --prefix="$PREFIX" \
    --enable-gtk3ui --disable-sdl1ui --disable-sdl2ui \
    --without-alsa --without-pulse --disable-desktop-files

echo "Building..."
make -j"$(nproc)"
make install

echo "Patched VICE installed at $PREFIX/bin/x64sc"
