#!/usr/bin/env python3
"""Dump uploaded drive RAM ($0300-$07FF) from VICE drive 8 via the binary monitor.

Use with a VICE x64sc instance started with:
    -drive8truedrive -binarymonitor -binarymonitoraddress 127.0.0.1:6502

The script feeds LOAD"*",8,1, waits for the protected loader to upload its
6502 code with M-W, then issues a MEM_GET for drive-8 RAM and writes the
1,280 bytes to /tmp/opencode/drive-0300.bin by default.
"""
import socket
import struct
import sys
import time

HOST, PORT = "127.0.0.1", 6502
STX, API = 0x02, 0x02
OUT = sys.argv[1] if len(sys.argv) > 1 else "/tmp/opencode/drive-0300.bin"


def send(sock, cmd, body=b"", rid=1):
    sock.sendall(
        bytes([STX, API])
        + struct.pack("<I", len(body))
        + struct.pack("<I", rid)
        + bytes([cmd])
        + body
    )


def drain(sock, timeout=2.0):
    sock.settimeout(timeout)
    out = b""
    try:
        while True:
            chunk = sock.recv(1 << 20)
            if not chunk:
                break
            out += chunk
    except socket.timeout:
        pass
    return out


def main():
    sock = socket.create_connection((HOST, PORT), timeout=3)

    # Feed the C64 LOAD command and let the drive-side loader upload its code.
    text = b'LOAD"*",8,1\r'
    send(sock, 0x72, bytes([len(text)]) + text, rid=1)
    send(sock, 0xAA, b"", rid=2)
    drain(sock, 1.0)
    time.sleep(4)

    # VICE 3.10 MEM_GET body: sidefx, start, end, memspace (1 = drive 8), bank.
    body = (
        bytes([0])
        + struct.pack("<HH", 0x0300, 0x07FF)
        + bytes([1])
        + struct.pack("<H", 0)
    )
    send(sock, 0x01, body, rid=5)
    out = drain(sock, 2.0)

    while len(out) >= 12:
        if out[0] != STX:
            out = out[1:]
            continue
        length = struct.unpack_from("<I", out, 2)[0]
        response_type = out[6]
        if len(out) < 12 + length:
            break
        frame_body = out[12:12 + length]
        # MEM_GET response: 2-byte little-endian length, then the RAM bytes.
        if response_type == 0x01 and length >= 1282:
            with open(OUT, "wb") as fh:
                fh.write(frame_body[2:])
            print(f"wrote {len(frame_body) - 2} bytes to {OUT}")
            return
        out = out[12 + length:]

    raise SystemExit("no drive-RAM MEM_GET response received")


if __name__ == "__main__":
    main()
