#!/bin/bash
# Boot-test a G64 image in x64sc with true drive emulation and dump C64 screen RAM.
# Usage: boot-test-g64.sh <image.g64> <tag>
set -u

img="$1"
tag="$2"
script_dir=$(cd "$(dirname "$0")" && pwd)
out_dir=${TMPDIR:-/tmp/opencode}

pkill -x x64sc 2>/dev/null || true
sleep 2

for attempt in $(seq 1 8); do
  env -u GTK_MODULES -u GTK_USE_PORTAL -u WAYLAND_DISPLAY GDK_BACKEND=x11 \
    LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe \
    timeout 110s x64sc -model c64c -ntsc -power60 -VICIIaspectmode 0 \
      -windowwidth 900 -windowheight 650 -8 "$img" -drive8truedrive \
      -sounddev dummy -binarymonitor -binarymonitoraddress 127.0.0.1:6502 \
      -logfile "$out_dir/boot-$tag.log" > "$out_dir/boot-$tag-console.log" 2>&1 &
  pid=$!
  sleep 5
  if python3 -c "import socket,sys; s=socket.socket(); sys.exit(0 if s.connect_ex(('127.0.0.1',6502))==0 else 1)"; then
    timeout 90s python3 "$script_dir/verify_vice_boot_long.py" "$out_dir/screen-$tag.bin" \
      > "$out_dir/screen-$tag.txt" 2>&1
    rc=$?
    kill "$pid" 2>/dev/null || true
    wait "$pid" 2>/dev/null || true
    echo "$tag attempt=$attempt rc=$rc"
    python3 - "$tag" "$out_dir" <<'PY'
import sys
tag, out_dir = sys.argv[1], sys.argv[2]
try:
    b = open(f"{out_dir}/screen-{tag}.bin", "rb").read()
except FileNotFoundError:
    print("  no screen dump")
    raise SystemExit
non = sum(1 for x in b if x != 0x20)
print(f"  screen non-space: {non}/1000")
for line in open(f"{out_dir}/screen-{tag}.txt"):
    if line.strip():
        print("  " + line.rstrip())
PY
    exit $rc
  fi
  kill -9 "$pid" 2>/dev/null || true
  wait "$pid" 2>/dev/null || true
done

echo "$tag: could not start VICE" >&2
exit 1
