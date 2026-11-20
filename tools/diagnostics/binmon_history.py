#!/usr/bin/env python3
"""Sample VICE drive-8 CPU history without stopping execution."""
import socket, struct, time

HOST, PORT = '127.0.0.1', 6502
STX, API = 0x02, 0x02

def send(s, cmd, body=b'', rid=1):
    s.sendall(bytes([STX,API])+struct.pack('<I',len(body))+struct.pack('<I',rid)+bytes([cmd])+body)

def frames(s, timeout=0.4):
    s.settimeout(timeout); out=b''
    try:
        while True:
            c=s.recv(1<<20)
            if not c: break
            out+=c
    except socket.timeout: pass
    r=[]
    while len(out)>=12:
        if out[0]!=STX: out=out[1:]; continue
        ln=struct.unpack_from('<I',out,2)[0]
        if len(out)<12+ln: break
        r.append(out[:12+ln]); out=out[12+ln:]
    return r

def parse_history(frame):
    if len(frame)<14 or frame[6]!=0x86: return []
    b=frame[12:]
    if len(b)<4: return []
    count=struct.unpack_from('<I',b,0)[0]; pos=4; out=[]
    for _ in range(count):
        if pos>=len(b): break
        size=b[pos]; pos+=1
        end=pos+size
        if end>len(b): break
        rc=struct.unpack_from('<H',b,pos)[0]; pos+=2
        regs={}
        for _ in range(rc):
            if pos>=end: break
            rs=b[pos]; pos+=1
            rid=b[pos] if rs>=1 else 0
            val=int.from_bytes(b[pos+1:pos+rs],'little') if rs>=3 else 0
            regs[rid]=val
            pos+=rs
        if pos+9>end: break
        pos+=8                 # CPU clock
        il=b[pos]; pos+=1
        ib=b[pos:pos+il]; pos+=il
        out.append((regs.get(3,0), ib.hex(' ')))
    return out

def main():
    s=socket.create_connection((HOST,PORT),timeout=3)
    text=b'LOAD"*",8,1\r'
    send(s,0x72,bytes([len(text)])+text,rid=1)
    send(s,0xAA,b'',rid=2)
    log=open('/tmp/opencode/g64-cpu-history.log','w')
    deadline=time.time()+90
    while time.time()<deadline:
        for f in frames(s):
            for pc, ins in parse_history(f):
                if 0x0300 <= pc <= 0x07FF:
                    log.write(f"${pc:04X} {ins}\n")
        log.flush()
        send(s,0x86,bytes([1])+struct.pack('<I',200),rid=3)  # history drive 8
        time.sleep(0.2)
    log.close()

if __name__=='__main__': main()
