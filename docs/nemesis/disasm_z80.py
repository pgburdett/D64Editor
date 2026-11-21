#!/usr/bin/env python3
"""Linear Z80 disassembler for CP/M .COM files using z80dis."""
import sys
from z80dis.z80 import decode, disasm

def disassemble_file(path, base=0x0100):
    data = open(path, 'rb').read()
    out = []
    pc = 0
    while pc < len(data):
        d = decode(data[pc:], base + pc)
        ln = max(d.len, 1)
        raw = data[pc:pc+ln]
        text = disasm(d)
        out.append(f"{base+pc:04x}: {raw.hex():<12} {text}")
        pc += ln
    return out

if __name__ == '__main__':
    for path in sys.argv[1:]:
        print(f"=== {path} ===")
        for line in disassemble_file(path):
            print(line)
