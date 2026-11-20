#!/usr/bin/env python3
"""Trace execution of uploaded drive code ($0300-$07FF) in VICE drive 8."""
import socket, struct, time

HOST, PORT = '127.0.0.1', 6502
STX, API = 0x02, 0x02

def send(sock, cmd, body=b'', rid=1):
    sock.sendall(bytes([STX, API]) + struct.pack('<I', len(body)) +
                 struct.pack('<I', rid) + bytes([cmd]) + body)

def recv_frames(sock, timeout=0.5):
    sock.settimeout(timeout); out=b''
    try:
        while True:
            c=sock.recv(65536)
            if not c: break
            out+=c
    except socket.timeout: pass
    frames=[]
    while len(out)>=12:
        if out[0]!=STX: out=out[1:]; continue
        ln=struct.unpack_from('<I',out,2)[0]
        if len(out)<12+ln: break
        frames.append(out[:12+ln]); out=out[12+ln:]
    return frames

def regs(frame):
    if len(frame)<14 or frame[6]!=0x31: return None
    body=frame[12:]
    if len(body)<2: return None
    cnt=struct.unpack_from('<H',body,0)[0]; pos=2; out={}
    for _ in range(cnt):
        if pos+4>len(body): break
        size,rid=body[pos],body[pos+1]
        out[rid]=struct.unpack_from('<H',body,pos+2)[0]
        pos+=2+size
    return out

def main():
    s=socket.create_connection((HOST,PORT),timeout=3)
    # checkpoint: exec $0300-$07FF on drive 8 (memspace 1), stop on hit
    body=struct.pack('<HH',0x0300,0x07FF)+bytes([1,1,4,0,1])
    send(s,0x12,body,rid=1); time.sleep(0.2)
    text=b'LOAD"*",8,1\r'
    send(s,0x72,bytes([len(text)])+text,rid=2); time.sleep(0.3)
    send(s,0xAA,b'',rid=3)
    log=open('/tmp/opencode/g64-drive-trace.log','w')
    deadline=time.time()+120
    while time.time()<deadline:
        for f in recv_frames(s):
            r=regs(f)
            if r is not None and 3 in r:
                log.write(f"PC=${r[3]:04X} A=${r.get(0,0):02X} X=${r.get(1,0):02X} Y=${r.get(2,0):02X}\n")
                log.flush()
        send(s,0xAA,b'',rid=4)
    log.close()

if __name__=='__main__': main()
