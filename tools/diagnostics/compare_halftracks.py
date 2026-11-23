#!/usr/bin/env python3
"""Compare whole/half-track GCR data captured directly from a real drive with nibread -h."""
import collections
import re
import sys

NIB = sys.argv[1] if len(sys.argv) > 1 else "/tmp/opencode/real-disk.nib"
LOG = sys.argv[2] if len(sys.argv) > 2 else "/tmp/opencode/nibread-real2.log"
HEADER = 256
BLOCK = 8192


def track_slots(path):
    b = open(path, "rb").read()
    assert b[:13] == b"MNIB-1541-RAW", "not a NIB image"
    slots = {}
    order = []
    off = HEADER
    for i in range(120):
        x2 = b[0x10 + i * 2]
        if x2 == 0:
            break
        slots[x2] = off
        order.append(x2)
        off += BLOCK
    return b, slots, order


def log_lengths(path):
    lengths = {}
    current = None
    for line in open(path, errors="replace"):
        if "(reverted)" in line:
            continue
        m = re.match(r"\s*(\d+)\.(\d):\s+\([^)]*\)\s+(\d+)", line)
        if m:
            whole, half, length = int(m.group(1)), int(m.group(2)), int(m.group(3))
            current = whole * 2 + half
            lengths[current] = length
            continue
        m = re.match(r"\s+\([^)]*\)\s+(\d+)", line)
        if m and current is not None:
            lengths[current] = int(m.group(1))
    return lengths


def best_cyclic_match(a, b, probe=24):
    """Return (match_ratio, shift) for best cyclic alignment of b against a."""
    n = min(len(a), len(b))
    if n == 0:
        return 0.0, 0
    pat = a[:probe]
    hay = b + b[:probe]
    positions = []
    start = 0
    while True:
        p = hay.find(pat, start)
        if p < 0 or p >= len(b):
            break
        positions.append(p)
        start = p + 1
    if not positions:
        # Also try the opposite direction.
        pat = b[:probe]
        hay = a + a[:probe]
        start = 0
        while True:
            p = hay.find(pat, start)
            if p < 0 or p >= len(a):
                break
            positions.append(-p)
            start = p + 1
    if not positions:
        return 0.0, 0

    best = (0.0, 0)
    for shift in positions:
        if shift >= 0:
            rb = b[shift:] + b[:shift]
            equal = sum(1 for x, y in zip(a[:n], rb[:n]) if x == y)
        else:
            s = -shift
            ra = a[s:] + a[:s]
            equal = sum(1 for x, y in zip(ra[:n], b[:n]) if x == y)
        ratio = equal / n
        if ratio > best[0]:
            best = (ratio, shift)
    return best


def main():
    b, slots, order = track_slots(NIB)
    lengths = log_lengths(LOG)
    print(f"NIB slots: {len(order)}, log track lengths: {len(lengths)}")
    print("whole/half   lenW  lenH  exact_prefix  best_cyclic  shift  hist_overlap")
    for x2 in sorted(order):
        if x2 % 2 != 1:
            continue
        wx2 = x2 - 1
        if wx2 not in slots or x2 not in slots:
            continue
        lw = lengths.get(wx2, BLOCK)
        lh = lengths.get(x2, BLOCK)
        a = b[slots[wx2]:slots[wx2] + lw]
        c = b[slots[x2]:slots[x2] + lh]
        n = min(len(a), len(c))
        if n == 0:
            continue
        prefix = 0
        while prefix < n and a[prefix] == c[prefix]:
            prefix += 1
        ratio, shift = best_cyclic_match(a, c)
        ca, cc = collections.Counter(a), collections.Counter(c)
        overlap = sum((ca & cc).values())
        hist_ratio = overlap / max(len(a), len(c))
        wname = f"{wx2 // 2}"
        hname = f"{x2 // 2}.5"
        mark = " <== MATCH" if ratio > 0.95 else ""
        print(f"T{wname:>2}/{hname:<4} {len(a):5} {len(c):5} "
              f"{prefix/n:12.3f} {ratio:11.3f} {shift:6} {hist_ratio:12.3f}{mark}")


if __name__ == "__main__":
    main()
