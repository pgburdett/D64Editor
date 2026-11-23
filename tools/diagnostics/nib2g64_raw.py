#!/usr/bin/env python3
"""Convert a NIB capture to G64 while preserving raw track bytes and half-tracks exactly.

Unlike nibconv, this does not clamp/normalize long tracks to a nominal RPM length.
"""
import re
import struct
import sys

nib_path = sys.argv[1] if len(sys.argv) > 1 else "/tmp/opencode/disk-in-drive.nib"
log_path = sys.argv[2] if len(sys.argv) > 2 else "/tmp/opencode/nibread-final.log"
out_path = sys.argv[3] if len(sys.argv) > 3 else "/tmp/opencode/raw-preserve.g64"

NIB_HEADER = 256
NIB_BLOCK = 8192
G64_SLOTS = 84

raw = open(nib_path, "rb").read()
assert raw[:13] == b"MNIB-1541-RAW"

# NIB track table: 120 pairs [track*2, density] from offset 0x10.
slots = []
for i in range(120):
    x2 = raw[0x10 + i * 2]
    density = raw[0x10 + i * 2 + 1]
    if x2 == 0:
        break
    slots.append((x2, density & 0x03, NIB_HEADER + i * NIB_BLOCK))

# Retained track lengths from nibread log (ignore reverted probes; last line wins).
lengths = {}
current = None
for line in open(log_path, errors="replace"):
    if "(reverted)" in line:
        continue
    m = re.match(r"\s*(\d+)\.(\d):\s+\([^)]*\)\s+(\d+)", line)
    if m:
        current = int(m.group(1)) * 2 + int(m.group(2))
        lengths[current] = int(m.group(3))
        continue
    m = re.match(r"\s+\([^)]*\)\s+(\d+)", line)
    if m and current is not None:
        lengths[current] = int(m.group(1))

offsets = [0] * G64_SLOTS
zones = [0] * G64_SLOTS
blocks = bytearray()
declared = 0

for x2, zone, noff in slots:
    gslot = x2 - 2  # NIB x2=2 -> G64 whole track 1 slot 0
    if not (0 <= gslot < G64_SLOTS):
        continue
    length = min(lengths.get(x2, NIB_BLOCK), NIB_BLOCK)
    if length <= 0:
        continue
    data = raw[noff:noff + length]
    if len(data) < length:
        continue
    declared = max(declared, length)
    offsets[gslot] = 12 + G64_SLOTS * 4 + G64_SLOTS * 4 + len(blocks)
    zones[gslot] = zone
    blocks += struct.pack("<H", length)
    blocks += data

header = bytearray(b"GCR-1541")
header += bytes([0, G64_SLOTS])
header += struct.pack("<H", declared)
for off in offsets:
    header += struct.pack("<I", off)
for z in zones:
    header += struct.pack("<I", z)

open(out_path, "wb").write(header + blocks)
print(f"wrote {out_path}: {len(header) + len(blocks)} bytes, {sum(1 for o in offsets if o)} tracks, declared={declared}")
