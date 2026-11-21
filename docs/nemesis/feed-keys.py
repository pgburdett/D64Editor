#!/usr/bin/env python3
"""Reliable keystroke feed for VICE CP/M via the binary monitor.

Usage: python3 feed-keys.py 'B:NEMESIS ARAGORN'
Requires VICE started with -binarymonitor -binarymonitoraddress 127.0.0.1:6502.

xdotool key --window drops characters because VICE emulates a slow 1MHz CPU.
Feeding bytes through the binary monitor's kbdbuf_feed (command 0x72) one at a
time with a delay is reliable.
"""
import socket, struct, sys, time

HOST = ('127.0.0.1', 6502)

def feed(text, rid=0, delay=0.6):
    s = socket.create_connection(HOST, timeout=5)
    for i, ch in enumerate(text):
        body = bytes([1]) + ch.encode('latin-1')
        pkt = bytes([2, 2]) + struct.pack('<I', len(body)) + \
              struct.pack('<I', rid + i) + bytes([0x72]) + body
        s.sendall(pkt)
        time.sleep(0.3)
        try:
            s.recv(64)
        except socket.timeout:
            pass
        time.sleep(delay)
    s.close()

if __name__ == '__main__':
    text = sys.argv[1] if len(sys.argv) > 1 else 'B:NEMESIS ARAGORN\r'
    feed(text)
