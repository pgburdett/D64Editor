#!/usr/bin/env python3
"""Feed LOAD/RUN to a binary-monitor VICE instance and dump C64 screen RAM."""
import socket
import struct
import sys
import time

OUT = sys.argv[1] if len(sys.argv) > 1 else "/tmp/opencode/vice-screen.bin"
s = socket.create_connection(("127.0.0.1", 6502), timeout=3)


def send(cmd, body=b"", rid=1):
    s.sendall(bytes([0x02, 0x02]) + struct.pack("<I", len(body)) +
              struct.pack("<I", rid) + bytes([cmd]) + body)


def drain(timeout=2.0):
    s.settimeout(timeout)
    out = b""
    try:
        while True:
            c = s.recv(1 << 20)
            if not c:
                break
            out += c
    except socket.timeout:
        pass
    return out


def feed(text, wait):
    text = text.encode() + b"\r"
    send(0x72, bytes([len(text)]) + text, rid=1)
    send(0xAA, b"", rid=2)
    drain(1.0)
    time.sleep(wait)


feed('LOAD"*",8,1', 35)
feed("RUN", 25)

# MEM_GET: sidefx, start, end, memspace 0 (C64), bank 0.
body = bytes([0]) + struct.pack("<HH", 0x0400, 0x07E7) + bytes([0]) + struct.pack("<H", 0)
send(0x01, body, rid=5)
out = drain(3.0)

raw = None
while len(out) >= 12:
    if out[0] != 0x02:
        out = out[1:]
        continue
    length = struct.unpack_from("<I", out, 2)[0]
    rtype = out[6]
    if len(out) < 12 + length:
        break
    body = out[12:12 + length]
    if rtype == 0x01 and length >= 1002:
        raw = body[2:2 + 1000]
        break
    out = out[12 + length:]

if raw is None:
    raise SystemExit("no screen-RAM MEM_GET response")

open(OUT, "wb").write(raw)


def decode(b):
    rows = []
    for row in range(25):
        line = ""
        for col in range(40):
            c = b[row * 40 + col] & 0x7F
            if c == 0x20:
                line += " "
            elif 1 <= c <= 26:
                line += chr(ord("A") + c - 1)
            elif 0x30 <= c <= 0x39:
                line += chr(ord("0") + c - 0x30)
            elif 0x2A <= c <= 0x2F:
                line += ".,:;/?!"[c - 0x2A]
            elif c == 0x00:
                line += "@"
            else:
                line += "."
        rows.append(line.rstrip())
    return rows

print("--- screen RAM decode ---")
for row in decode(raw):
    print(row)
