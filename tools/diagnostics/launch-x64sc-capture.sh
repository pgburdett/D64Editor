#!/bin/bash
export DISPLAY=${DISPLAY:-:0}
export XAUTHORITY=${XAUTHORITY:-/run/user/1000/xauth_mMZpLg}
export LIBGL_ALWAYS_SOFTWARE=1
export GALLIUM_DRIVER=llvmpipe

VICE=/tmp/opencode/vice-patched/bin/x64sc
G64='/tmp/opencode/arcticfox-capture.g64'
LOG=/tmp/opencode/x64sc-g64.log
: > "$LOG"
: > /tmp/opencode/g64-mon.log

for i in $(seq 1 12); do
    "$VICE" -model c64c -ntsc -power60 \
            -VICIIaspectmode 0 -windowwidth 900 -windowheight 650 \
            -windowxpos 1640 -windowypos 920 \
            -8 "$G64" -sounddev dummy \
            -binarymonitor -binarymonitoraddress 127.0.0.1:6502 \
            -logfile "$LOG" \
            -moncommands /tmp/opencode/vice-mon-cmds.txt \
            -monlog -monlogname /tmp/opencode/g64-mon.log \
            >/tmp/opencode/x64sc-g64-stdout.log 2>&1 &
    pid=$!
    sleep 7
    if kill -0 "$pid" 2>/dev/null && [ "$(wc -l < "$LOG" 2>/dev/null || echo 0)" -gt 50 ]; then
        echo "started attempt=$i pid=$pid"
        wait "$pid"
        break
    fi
    kill -9 "$pid" 2>/dev/null
    wait "$pid" 2>/dev/null
done
