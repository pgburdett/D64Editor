#!/usr/bin/env python3
"""Minimal VICE binary-monitor client: log A on every store to $DD00."""
import socket, struct, sys, time

HOST, PORT = '127.0.0.1', 6502
STX, API = 0x02, 0x02

def send(sock, cmd, body=b'', rid=1):
    frame = bytes([STX, API]) + struct.pack('<I', len(body)) + struct.pack('<I', rid) + bytes([cmd]) + body
    sock.sendall(frame)

def recv_frames(sock, timeout=1.0):
    sock.settimeout(timeout)
    out = b''
    try:
        while True:
            chunk = sock.recv(65536)
            if not chunk: break
            out += chunk
    except socket.timeout:
        pass
    frames = []
    while len(out) >= 12:
        if out[0] != STX:
            out = out[1:]; continue
        ln = struct.unpack_from('<I', out, 2)[0]
        total = 12 + ln
        if len(out) < total: break
        frames.append(out[:total]); out = out[total:]
    return frames

def parse_a(frame):
    # Response type at byte 6; register response = 0x31
    if len(frame) < 14 or frame[6] != 0x31: return None
    body = frame[12:]
    if len(body) < 2: return None
    cnt = struct.unpack_from('<H', body, 0)[0]
    pos = 2
    for _ in range(cnt):
        if pos + 4 > len(body): break
        size, rid = body[pos], body[pos+1]
        val = struct.unpack_from('<H', body, pos+2)[0]
        if rid == 0:  # A register
            return val & 0xFF
        pos += 2 + size
    return None

def main():
    sock = socket.create_connection((HOST, PORT), timeout=3)
    # checkpoint set: SA=$DD00, EA=$DD00, stop=1, enabled=1, op=store(2), temporary=0
    body = struct.pack('<HH', 0xDD00, 0xDD00) + bytes([1, 1, 2, 0])
    send(sock, 0x12, body, rid=1)
    time.sleep(0.2)
    # feed LOAD command via the monitor keyboard buffer
    text = b'LOAD"*",8,1\r'
    send(sock, 0x72, bytes([len(text)]) + text, rid=20)
    time.sleep(0.3)
    send(sock, 0xAA, b'', rid=2)  # continue
    log = open('/tmp/opencode/binmon-iec.log', 'w')
    hits = 0
    deadline = time.time() + 90
    while time.time() < deadline:
        for f in recv_frames(sock, 0.5):
            if len(f) > 6 and f[6] == 0x11 and len(f) >= 17:
                hit = f[12+4]
                if hit:
                    hits += 1
            a = parse_a(f)
            if a is not None:
                log.write(f'{a:02X}\n'); log.flush()
        send(sock, 0xAA, b'', rid=3)  # keep continuing
    log.write(f'# hits={hits}\n')
    log.close()

if __name__ == '__main__': main()
