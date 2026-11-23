#!/bin/bash
# Launch x64sc with a G64 image and dump the uploaded drive RAM via the binary monitor.
# Usage: launch-x64sc-g64-dump.sh [image.g64] [output.bin]
set -u

G64=${1:-/tmp/opencode/real-disk.g64}
OUT=${2:-/tmp/opencode/drive-0300.bin}
LOG=/tmp/opencode/g64-dump.log
STDOUT_LOG=/tmp/opencode/g64-dump-console.log
VICE=${VICE:-x64sc}
PORT=6502

port_open() {
    python3 -c "import socket,sys; s=socket.socket(); sys.exit(0 if s.connect_ex(('127.0.0.1',$PORT))==0 else 1)"
}

cleanup() {
    if [ -n "${pid:-}" ] && kill -0 "$pid" 2>/dev/null; then
        kill "$pid" 2>/dev/null || true
        wait "$pid" 2>/dev/null || true
    fi
}
trap cleanup EXIT INT TERM

: > "$LOG"
: > "$STDOUT_LOG"

for attempt in $(seq 1 12); do
    env -u GTK_MODULES -u GTK_USE_PORTAL -u WAYLAND_DISPLAY \
        GDK_BACKEND=x11 LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe \
        timeout 150s "$VICE" -model c64c -ntsc -power60 \
        -VICIIaspectmode 0 -windowwidth 900 -windowheight 650 \
        -8 "$G64" -drive8truedrive -sounddev dummy \
        -binarymonitor -binarymonitoraddress "127.0.0.1:$PORT" \
        -logfile "$LOG" >"$STDOUT_LOG" 2>&1 &
    pid=$!
    sleep 5
    if kill -0 "$pid" 2>/dev/null && port_open; then
        echo "monitor ready attempt=$attempt pid=$pid image=$G64"
        python3 tools/diagnostics/dump_drive_mem.py "$OUT"
        cleanup
        exit $?
    fi
    kill -9 "$pid" 2>/dev/null || true
    wait "$pid" 2>/dev/null || true
done

echo "could not start x64sc binary monitor" >&2
exit 1
