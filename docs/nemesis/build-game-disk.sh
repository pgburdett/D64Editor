#!/bin/bash
# Build a playable D81 game disk for Nemesis.
#
# Key insight: from-scratch D81 CP/M images (--cpm-new d81) are data-only and
# have an invalid 1581 BAM; the 1581 locks up. Always start from the valid
# "cpm kit.d81" donor and let D64Editor's in-process C128Cpm write files.
#
# Usage: ./build-game-disk.sh [output.d81]
set -euo pipefail

HERE="$(cd "$(dirname "$0")" && pwd)"
ROOT="$(cd "$HERE/../../.." && pwd)"
DONOR="/run/media/pat/store/bk/_emu/_COMMODORE/8bitfiles.net/8bitfiles.net/cbm8bits/c128/tools/cpm kit.d81"
STAGE="$HERE/game-files"
OUT="${1:-/tmp/nemesis-game.d81}"
CLEAN="/tmp/nemesis-kit-clean.d81"

# Build D64Editor if needed
if [ ! -f "$ROOT/D64Editor.App/bin/Debug/net10.0/D64Editor.App.dll" ]; then
  dotnet build "$ROOT/D64Editor.sln" -c Debug
fi
APP="$ROOT/D64Editor.App/bin/Debug/net10.0/D64Editor.App.dll"

# Clear CP/M directory entries from the donor (user byte 0..0x0f = active).
# DO NOT use --cpm-format here: it sets BAM byte 0 to 0xE5 and corrupts the 1581.
python3 - "$DONOR" "$CLEAN" <<'PY'
import sys
src,dst=sys.argv[1],sys.argv[2]
b=bytearray(open(src,'rb').read())
# D81 payload begins at 0; CP/M directory lives in logical blocks 0..1 which
# map to T1/S0..S3 and T1/S4..S7. In the D81 container these are at 0x0000..
# 0x0800. Each entry is 32 bytes; status byte 0xE5 = deleted, >=0x20 = user.
for off in range(0, 0x1000, 32):
    if b[off] < 0x20 and b[off] != 0xE5:
        b[off] = 0xE5
open(dst,'wb').write(b)
PY

# Import the full staged file set in one pass
dotnet "$APP" --cpm-import-dir "$CLEAN" "$STAGE" "$OUT"

# Verify key files
rm -rf /tmp/nemesis-verify
dotnet "$APP" --cpm-extract "$OUT" /tmp/nemesis-verify --strict
for f in NEMESIS.COM TERM.DEF ARAGORN.CHR; do
  cmp -s "/tmp/nemesis-verify/$f" "$STAGE/$f" || { echo "FAIL: $f"; exit 1; }
done
echo "OK: $OUT ($(ls /tmp/nemesis-verify | wc -l) files)"
