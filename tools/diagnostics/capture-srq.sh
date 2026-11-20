#!/bin/bash
# Faithful raw GCR capture of a protected disk via SRQ serial nibbling.
# Usage: capture-srq.sh [device] [output-basename]
set -euo pipefail
DEV=${1:-8}
BASE=${2:-arcticfox}
NIB="$BASE.nib"
G64="$BASE.g64"

echo "Device: $DEV (Real drive must be in drive $DEV)"
cbmctrl status "$DEV"
read -r -p "Confirm the original disk is in drive $DEV, then press ENTER: " _

nibread -@xum1541 -D"$DEV" -e5 -h -v "$NIB"
nibconv "$NIB" "$G64"

echo "Capture complete: $NIB ($(stat -c%s "$NIB") bytes), $G64 ($(stat -c%s "$G64") bytes)"
echo "Boot with: x64sc -model c64c -ntsc -power60 -drive8truedrive -8 \"$G64\""
