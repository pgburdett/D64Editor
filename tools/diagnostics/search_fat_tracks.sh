#!/bin/bash
ROOT="/run/media/pat/store/bk/_emu/_COMMODORE"
find "$ROOT" -type f -iname '*.pdf' -print0 2>/dev/null |
  xargs -0 -P 8 -I{} sh -c '
    f="$1"
    txt=$(pdftotext -layout "$f" - 2>/dev/null | grep -iE "fat[[:space:]-]?tracks?|fat.?track" | head -5)
    if [ -n "$txt" ]; then
      echo "===== $f"
      echo "$txt"
    fi
  ' _ {}
