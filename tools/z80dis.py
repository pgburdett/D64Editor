#!/usr/bin/env python3
"""Z80 disassembler for analyzing CP/M .COM programs (e.g. Nemesis PERSON.COM).

Vendors the MIT-licensed z80dis decoder (https://pypi.org/project/z80dis/) in
z80_decoder.py so CP/M program analysis works offline with only Python 3 stdlib.

CP/M .COM convention: the file is loaded at address 0x100, so file offset N maps
to memory address 0x100 + N. This tool uses that mapping for --start/--end,
strings and pattern matching.

Usage:
    python3 tools/z80dis.py <program.com> --start 0x1348 --end 0x1390
    python3 tools/z80dis.py <program.com> --strings
    python3 tools/z80dis.py <program.com> --find-hex "0e 01 cd 05 00"
"""

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from z80_decoder import decode, decoded2str, DECODE_STATUS  # noqa: E402


def iter_strings(data, minlen=4, base=0x100):
    i = start = -1
    buf = []
    while i <= len(data):
        ch = data[i] if i < len(data) else 0
        if 32 <= ch < 127 or ch == 9:
            if start < 0:
                start = i
            buf.append(chr(ch))
        else:
            if start >= 0 and len(buf) >= minlen:
                yield base + start, "".join(buf)
            start = -1
            buf = []
        i += 1


def find_hex(data, pattern, base=0x100):
    i = 0
    while True:
        i = data.find(pattern, i)
        if i < 0:
            return
        yield base + i
        i += 1


def main(argv=None):
    p = argparse.ArgumentParser(description="Z80 disassembler for CP/M .COM analysis")
    p.add_argument("file", help="CP/M .COM binary")
    p.add_argument("--start", type=lambda x: int(x, 0), default=0x100,
                   help="memory address to start disassembling (default 0x100)")
    p.add_argument("--end", type=lambda x: int(x, 0), default=None,
                   help="memory address to stop (default: start + 256)")
    p.add_argument("--strings", action="store_true", help="list printable strings")
    p.add_argument("--minlen", type=int, default=4, help="min string length (default 4)")
    p.add_argument("--find-hex", help="find byte pattern, e.g. '0e 01 cd 05 00'")
    args = p.parse_args(argv)

    data = Path(args.file).read_bytes()

    if args.strings:
        for addr, s in iter_strings(data, args.minlen):
            print("0x%04x: %s" % (addr, s))
        return 0

    if args.find_hex:
        pat = bytes(int(x, 16) for x in args.find_hex.split())
        for addr in find_hex(data, pat):
            print("0x%04x" % addr)
        return 0

    end = args.end if args.end is not None else min(0x100 + len(data), args.start + 256)
    addr = args.start - 0x100
    while addr < end - 0x100 and addr < len(data):
        dec = decode(data[addr:], addr + 0x100)
        if dec.status != DECODE_STATUS.OK:
            print("0x%04x: db %02x" % (addr + 0x100, data[addr]))
            addr += 1
        else:
            print("0x%04x: %s" % (addr + 0x100, decoded2str(dec)))
            addr += dec.len
    return 0


if __name__ == "__main__":
    sys.exit(main())
