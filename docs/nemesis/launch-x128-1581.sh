#!/bin/bash
export DISPLAY=${DISPLAY:-:0}
export XAUTHORITY=${XAUTHORITY:-/run/user/1000/xauth_mMZpLg}
export LIBGL_ALWAYS_SOFTWARE=1
export GALLIUM_DRIVER=llvmpipe
VICE=/tmp/opencode/vice-patched/bin/x128
for i in $(seq 1 12); do
  "$VICE" -80col -VDCaspectmode 0 -VICIIborders 3 \
    -drive8type 1571 -8 /tmp/opencode/cpmfast-src/CPMFAST3.D71 \
    -drive9type 1581 -9 /tmp/opencode/nemesis-1581-full.d81 \
    -sounddev dummy -binarymonitor -binarymonitoraddress 127.0.0.1:6502 \
    -logfile /tmp/opencode/x128-1581.log >/tmp/opencode/x128-1581-stdout.log 2>&1 &
  pid=$!
  sleep 7
  if kill -0 $pid 2>/dev/null && [ "$(wc -l < /tmp/opencode/x128-1581.log 2>/dev/null || echo 0)" -gt 50 ]; then
    echo "started attempt=$i pid=$pid"
    wait $pid
    break
  fi
  kill -9 $pid 2>/dev/null; wait $pid 2>/dev/null
done
