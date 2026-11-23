#!/usr/bin/env python3
"""Sample C64 screen/color RAM and VIC registers while a protected disk loads."""
import hashlib
import socket
import struct
import sys
import time

DURATION = int(sys.argv[1]) if len(sys.argv) > 1 else 180
INTERVAL = 5
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


def memget(start, end, rid):
    body = bytes([0]) + struct.pack("<HH", start, end) + bytes([0]) + struct.pack("<H", 0)
    send(0x01, body, rid=rid)
    out = drain(2.0)
    want = end - start + 1
    while len(out) >= 12:
        if out[0] != 0x02:
            out = out[1:]
            continue
        length = struct.unpack_from("<I", out, 2)[0]
        rtype = out[6]
        if len(out) < 12 + length:
            break
        body = out[12:12 + length]
        if rtype == 0x01 and length >= 2 + want:
            return body[2:2 + want]
        out = out[12 + length:]
    return None


def feed(text, wait):
    text = text.encode() + b"\r"
    send(0x72, bytes([len(text)]) + text, rid=1)
    send(0xAA, b"", rid=2)
    drain(1.0)
    time.sleep(wait)


feed('LOAD"*",8,1', 3)
feed("RUN", 2)

start = time.time()
while time.time() - start < DURATION:
    screen = memget(0x0400, 0x07E7, 10)
    color = memget(0xD800, 0xDBFF, 11)
    vic = memget(0xD020, 0xD02F, 12)
    if screen is None or color is None or vic is None:
        print(f"{int(time.time()-start):4}s  MEM_GET failed")
    else:
        sh = hashlib.sha256(screen).hexdigest()[:12]
        ch = hashlib.sha256(color).hexdigest()[:12]
        non = sum(1 for x in screen if x != 0x20)
        print(f"{int(time.time()-start):4}s  screen={sh} nonspace={non:4} "
              f"color={ch} vic={vic.hex()}")
    time.sleep(INTERVAL)
