#!/bin/bash
set -u
SRC="/run/media/pat/store/bk/_emu/_COMMODORE/8bitfiles.net/8bitfiles.net/archives/c64-preservation-project/copiers"
APP="/home/pat/source/repos/D64Editor/D64Editor.App/bin/Debug/net10.0/D64Editor.App.dll"
OUT=/tmp/opencode/kj-all
mkdir -p "$OUT"
for f in "$SRC"/kracker*jax*.nbz; do
  base=$(basename "$f" .nbz)
  d64="$OUT/$base.d64"
  dir="$OUT/$base-files"
  mkdir -p "$dir"
  dotnet "$APP" --convert "$f" "$d64" >/dev/null 2>&1 || { echo "CONVERT FAIL $base"; continue; }
  dotnet "$APP" --export "$d64" "$dir" >/dev/null 2>&1 || { echo "EXPORT FAIL $base"; continue; }
  echo "===== $base"
  for p in "$dir"/*; do
    strings -n 4 "$p" 2>/dev/null | grep -iE 'fat.?track|half.?track|odd.?phase|gap|sync|track 3[05]|track 4' | sed "s|^|$(basename "$p"): |"
  done | head -40
done
