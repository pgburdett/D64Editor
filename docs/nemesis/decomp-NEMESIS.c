/* Decompiled NEMESIS.COM - Ghidra Z80 decompiler */

/* === ram:0000 : RST0 === */

void RST0(void)

{
  short sVar1;
  
  sVar1 = FUN_ram_0005;
  *(undefined2 *)(FUN_ram_0005 + -2) = 0xf;
  FUN_ram_034b();
  *(undefined2 *)(sVar1 + -2) = 0x12;
  FUN_ram_079e();
  FUN_ram_042b();
  return;
}



/* === ram:0005 : FUN_ram_0005 === */

void FUN_ram_0005(void)

{
  FUN_ram_034b();
  FUN_ram_079e();
  FUN_ram_042b();
  return;
}



/* === ram:0008 : RST1 === */

void RST1(void)

{
  FUN_ram_034b();
  FUN_ram_079e();
  FUN_ram_042b();
  return;
}



/* === ram:0018 : RST3 === */

void RST3(undefined2 param_1,byte *param_2)

{
  if ((*param_2 & 1) == 0) {
    return;
  }
  func_0x0038(8,param_2 + CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1));
  return;
}



/* === ram:0020 : RST4 === */

void RST4(byte *param_1)

{
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
  return;
}



/* === ram:0028 : RST5 === */

void RST5(void)

{
  FUN_ram_0472();
  return;
}



/* === ram:0030 : RST6 === */

void RST6(undefined2 param_1,undefined1 *param_2)

{
  byte bVar1;
  undefined1 uVar2;
  short sVar3;
  
  enableMaskableInterrupts();
  sVar3 = CONCAT11((char)((ushort)param_1 >> 8),(char)param_1 + '\x01');
  bVar1 = FUN_ram_0d96();
  if (bVar1 == 0x38) {
    *(ushort *)(sVar3 + 2) = ((ushort)param_2 & 0xff) + 0x82e7;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  uVar2 = 0;
  if ((bVar1 & 8) != 0) {
    uVar2 = FUN_ram_1182();
  }
  *param_2 = uVar2;
  param_2[1] = uVar2;
  param_2[2] = uVar2;
  return;
}



/* === ram:0066 : NMI_ISR === */

undefined1 NMI_ISR(undefined1 *param_1)

{
  return *param_1;
}



/* === ram:00d6 : FUN_ram_00d6 === */

void FUN_ram_00d6(void)

{
  byte in_F;
  
  if ((bool)(in_F & 1)) {
    return;
  }
  return;
}



/* === ram:011d : FUN_ram_011d === */

byte FUN_ram_011d(byte param_1,byte param_2)

{
  if ((char)param_1 < '\0') {
    return ~param_2;
  }
  return param_1;
}



/* === ram:013f : FUN_ram_013f === */

byte FUN_ram_013f(undefined2 param_1,undefined2 param_2)

{
  char cVar1;
  byte bVar2;
  
  cVar1 = '\0';
  DAT_ram_0571 = 0;
  FUN_ram_025a();
  FUN_ram_025a(param_2,param_1);
  bVar2 = (byte)param_2;
  func_0x026b();
  if ((DAT_ram_0571 & 1) == 0) {
    return DAT_ram_0571 >> 1 | cVar1 << 7;
  }
  return ~bVar2;
}



/* === ram:0147 : FUN_ram_0147 === */

byte FUN_ram_0147(byte param_1)

{
  char in_F;
  
  FUN_ram_025a();
  func_0x026b();
  if ((DAT_ram_0571 & 1) == 0) {
    return DAT_ram_0571 >> 1 | in_F << 7;
  }
  return ~param_1;
}



/* === ram:0190 : FUN_ram_0190 === */

void FUN_ram_0190(void)

{
  return;
}



/* === ram:01af : FUN_ram_01af === */

/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_01af(ushort param_1,short param_2,byte param_3,undefined2 param_4)

{
  byte bVar1;
  undefined2 in_AF;
  ushort uVar2;
  undefined1 *puVar3;
  undefined2 in_stack_00000000;
  
  bVar1 = (byte)((ushort)in_stack_00000000 >> 8);
  uVar2 = CONCAT11(bVar1 >> 1,bVar1) & 0xff01;
  if ((char)((ushort)param_4 >> 8) == '\x01') {
    return;
  }
  if ((param_1 & 0x7f00) == 0 && -1 < (char)param_1) {
    return;
  }
  puVar3 = (undefined1 *)
           CONCAT11((char)((ushort)(param_2 + CONCAT11(~(byte)((ushort)in_AF >> 8),~param_3) + 1) >>
                          8),0xc2);
  FUN_ram_04c6((char)param_1 << 1 | (byte)param_4 & 1,
               CONCAT11((char)(uVar2 >> 8),(byte)in_stack_00000000 >> 1 | (char)uVar2 << 7) + 1);
  *puVar3 = 0;
  puVar3[0x14] = 0;
  FUN_ram_013f(puVar3 + 0x15);
  return;
}



/* === ram:01f9 : FUN_ram_01f9 === */

byte FUN_ram_01f9(undefined1 param_1,undefined1 *param_2,byte param_3)

{
  *param_2 = param_1;
  return ~param_3;
}



/* === ram:0204 : FUN_ram_0204 === */

byte FUN_ram_0204(byte param_1)

{
  return ~param_1;
}



/* === ram:0205 : FUN_ram_0205 === */

byte FUN_ram_0205(byte param_1)

{
  return ~param_1;
}



/* === ram:023f : FUN_ram_023f === */

/* WARNING: Removing unreachable block (ram,0x0249) */

void FUN_ram_023f(undefined1 *param_1,undefined1 *param_2)

{
  *param_2 = *param_1;
  RST0(CONCAT11((char)((ushort)(param_1 + 1) >> 8),(char)(param_1 + 1) + -1),param_2 + 1);
  return;
}



/* === ram:025a : FUN_ram_025a === */

/* WARNING: Instruction at (ram,0x03b2) overlaps instruction at (ram,0x03b1)
    */
/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x04aa) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_025a(short param_1,undefined2 param_2,undefined1 *param_3)

{
  byte in_F;
  byte bVar1;
  char cVar2;
  byte *pbVar3;
  char *pcVar4;
  
  if ((bool)(in_F >> 2 & 1)) {
    return;
  }
  _DAT_ram_0597 = 0x59dc;
  DAT_ram_05ad = 0xdb;
  DAT_ram_05b0 = 0xd3;
  DAT_ram_05af = 0xc9;
  DAT_ram_05b2 = 0xc9;
  _DAT_ram_058f = 0xcc00;
  _DAT_ram_0593 = 0x10;
  _DAT_ram_0591 = 0x40;
  pcVar4 = (char *)0x400;
  _DAT_ram_0595 = 0x400;
  DAT_ram_05b5 = param_2;
  bVar1 = FUN_ram_0005(CONCAT11((char)((ushort)(param_1 + 1) >> 8),0xb));
  if ((bVar1 & 1) == 0) {
    cVar2 = FUN_ram_0503(*param_3);
    pbVar3 = param_3 + 2;
    *pcVar4 = cVar2 + -0x40;
    pcVar4 = pcVar4 + 1;
    FUN_ram_0147();
    bVar1 = *pbVar3;
    if (bVar1 != 0x2e) {
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      RST6();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      return;
    }
  }
  else {
    FUN_ram_0005();
    pbVar3 = (byte *)0x6df;
    if (DAT_ram_0080 == 0) {
      return;
    }
    pcVar4 = (char *)0xc2;
    bVar1 = DAT_ram_0080 | bRam00c2;
  }
  FUN_ram_04c6(bVar1,pbVar3 + 1);
  *pcVar4 = '\0';
  pcVar4[0x14] = '\0';
  FUN_ram_013f(pcVar4 + 0x15);
  return;
}



/* === ram:0289 : FUN_ram_0289 === */

/* WARNING: Instruction at (ram,0x03b2) overlaps instruction at (ram,0x03b1)
    */
/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x04aa) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0289(undefined1 *param_1)

{
  byte bVar1;
  char cVar2;
  byte *pbVar3;
  char *pcVar4;
  
  pcVar4 = (char *)0x400;
  _DAT_ram_0595 = 0x400;
  bVar1 = FUN_ram_0005(0xb);
  if ((bVar1 & 1) == 0) {
    cVar2 = FUN_ram_0503(*param_1);
    pbVar3 = param_1 + 2;
    *pcVar4 = cVar2 + -0x40;
    pcVar4 = pcVar4 + 1;
    FUN_ram_0147();
    bVar1 = *pbVar3;
    if (bVar1 != 0x2e) {
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      RST6();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      return;
    }
  }
  else {
    FUN_ram_0005(1);
    pbVar3 = (byte *)0x6df;
    if (DAT_ram_0080 == 0) {
      return;
    }
    pcVar4 = (char *)0xc2;
    bVar1 = DAT_ram_0080 | bRam00c2;
  }
  FUN_ram_04c6(bVar1,pbVar3 + 1);
  *pcVar4 = '\0';
  pcVar4[0x14] = '\0';
  FUN_ram_013f(pcVar4 + 0x15);
  return;
}



/* === ram:0296 : FUN_ram_0296 === */

/* WARNING: Instruction at (ram,0x03b2) overlaps instruction at (ram,0x03b1)
    */
/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_0296(char *param_1,undefined1 *param_2)

{
  byte in_F;
  char cVar1;
  byte bVar2;
  byte *pbVar3;
  
  if ((bool)(in_F >> 6 & 1)) {
    if ((in_F >> 6 & 1) == 0) {
      func_0x0038();
      return;
    }
    cVar1 = FUN_ram_0503(*param_2);
    pbVar3 = param_2 + 2;
    *param_1 = cVar1 + -0x40;
    param_1 = param_1 + 1;
    FUN_ram_0147();
    bVar2 = *pbVar3;
    if (bVar2 != 0x2e) {
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      RST6();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      return;
    }
  }
  else {
    FUN_ram_0005(1);
    pbVar3 = (byte *)0x6df;
    if (DAT_ram_0080 == 0) {
      return;
    }
    param_1 = (char *)0xc2;
    bVar2 = DAT_ram_0080 | bRam00c2;
  }
  FUN_ram_04c6(bVar2,pbVar3 + 1);
  *param_1 = '\0';
  param_1[0x14] = '\0';
  FUN_ram_013f(param_1 + 0x15);
  return;
}



/* === ram:02cb : FUN_ram_02cb === */

/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_02cb(byte param_1,char *param_2,undefined1 *param_3)

{
  byte in_F;
  byte bVar1;
  undefined1 uVar2;
  undefined1 *puVar3;
  
  if ((in_F >> 6 & 1) == 0) {
    FUN_ram_0005((param_1 - *param_2) - (in_F & 1));
    if (DAT_ram_0080 != 0) {
      puVar3 = (undefined1 *)0xc2;
      FUN_ram_04c6(DAT_ram_0080 | bRam00c2,0x6e0);
      *puVar3 = 0;
      puVar3[0x14] = 0;
      FUN_ram_013f(puVar3 + 0x15);
      return;
    }
  }
  else {
    if ((bool)(in_F >> 6 & 1)) {
      return;
    }
    if (param_1 != 0) {
      bVar1 = param_1 < 0x22;
      if (param_1 == 0x22) {
        uVar2 = 0x22;
        DAT_ram_0572 = 0x22;
      }
      else {
        uVar2 = *param_3;
      }
      FUN_ram_04f1(uVar2);
      if ((bool)(bVar1 & 1)) {
        return;
      }
      func_0x0038(param_3 + 1);
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      func_0x0038();
      return;
    }
  }
  return;
}



/* === ram:0313 : FUN_ram_0313 === */

/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x031c) */

void FUN_ram_0313(void)

{
  func_0x0010();
  return;
}



/* === ram:034b : FUN_ram_034b === */

char FUN_ram_034b(short param_1,byte *param_2,byte *param_3,undefined2 param_4)

{
  byte extraout_F;
  char cVar1;
  byte *pbVar2;
  
  param_1 = param_1 + 1;
  FUN_ram_0472();
  if ((bool)(extraout_F & 1)) {
    if ((*param_3 & 1) == 0) {
      return '\b';
    }
    cVar1 = func_0x0038();
    return cVar1;
  }
  if ((*param_2 & 4) == 0) {
    FUN_ram_0005();
    return (char)((ushort)param_4 >> 8);
  }
  pbVar2 = param_2;
  FUN_ram_0313();
  FUN_ram_051a(param_1);
  cVar1 = FUN_ram_0005(0x10,param_3,pbVar2);
  *param_2 = 0;
  if (cVar1 == -1) {
    return cVar1;
  }
  return cVar1;
}



/* === ram:03c8 : FUN_ram_03c8 === */

void FUN_ram_03c8(byte param_1,undefined2 param_2,undefined1 *param_3,undefined1 *param_4,
                 undefined1 param_5)

{
  byte bVar1;
  undefined1 uVar2;
  char cVar3;
  undefined2 in_stack_00000000;
  
  cVar3 = (char)((ushort)param_2 >> 8);
  bVar1 = param_1 < 0x2a;
  if (param_1 == 0x2a) {
    func_0x0038(0x3f);
    return;
  }
  uVar2 = FUN_ram_04f1(*param_4);
  if (!(bool)(bVar1 & 1)) {
    *param_3 = uVar2;
    param_4 = param_4 + 1;
    bVar1 = bVar1 & 0x29;
    if (cVar3 != '\x01') {
      func_0x0038(param_3 + 1);
      return;
    }
    FUN_ram_04f1(*param_4,in_stack_00000000);
    if ((bool)(bVar1 & 1)) {
      return;
    }
    func_0x0038(param_4 + 1);
    func_0x0038();
    func_0x0038();
    func_0x0038();
    func_0x0038();
    func_0x0038();
    func_0x0038();
    func_0x0038();
    func_0x0038();
    register0x44 = (BADSPACEBASE *)&param_5;
  }
  *(undefined2 *)((short)register0x44 + -2) = 0x4e8;
  func_0x0038();
  return;
}



/* === ram:042b : FUN_ram_042b === */

void FUN_ram_042b(byte *param_1)

{
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
  return;
}



/* === ram:0472 : FUN_ram_0472 === */

void FUN_ram_0472(void)

{
  func_0x0038();
  return;
}



/* === ram:04c6 : FUN_ram_04c6 === */

void FUN_ram_04c6(void)

{
  func_0x0038();
  return;
}



/* === ram:04f1 : FUN_ram_04f1 === */

void FUN_ram_04f1(void)

{
  func_0x0038();
  return;
}



/* === ram:0503 : FUN_ram_0503 === */

void FUN_ram_0503(void)

{
  func_0x0038();
  return;
}



/* === ram:051a : FUN_ram_051a === */

void FUN_ram_051a(void)

{
  func_0x0038();
  return;
}



/* FAILED ram:079e : Low-level Error: Bad instruction count exceeded:
    ...
    Instruction at (ram,0x0827) overlaps instruction at (ram,0x0825)
 */

/* === ram:0cda : FUN_ram_0cda === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0cda(short param_1)

{
  short sVar1;
  ushort uVar2;
  
  sVar1 = *(short *)(param_1 + 6);
  *(short *)(param_1 + 6) = sVar1 + -1;
  if (sVar1 == 0) {
    *(ushort *)(param_1 + 6) = (ushort)*(byte *)(param_1 + 7) * 0x100 + -1;
    FUN_ram_0ee4(0);
    return;
  }
  uVar2 = _DAT_ram_82d0 & 0xff;
  FUN_ram_0f12();
  FUN_ram_0dda(uVar2);
  return;
}



/* === ram:0d06 : FUN_ram_0d06 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0d06(undefined1 *param_1,short *param_2,char param_3)

{
  byte bVar1;
  ushort uVar2;
  short sVar3;
  undefined2 in_stack_00000000;
  
  *param_2 = CONCAT11(*(char *)((short)param_2 + 1),param_3) + -1;
  if (*(char *)((short)param_2 + 1) != '\0' || param_3 != '\0') {
    uVar2 = _DAT_ram_832f & 0xff;
    FUN_ram_023f(8);
    *param_1 = (char)uVar2;
    param_1[1] = (char)(uVar2 >> 8);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  uVar2 = (ushort)*(byte *)((short)param_2 + (short)param_1);
  FUN_ram_1838(0,uVar2);
  sVar3 = uVar2 + 1;
  bVar1 = (byte)((ushort)sVar3 >> 8) | (byte)sVar3;
  if (bVar1 != 0) {
    FUN_ram_0289(bVar1,sVar3);
    FUN_ram_104a();
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(in_stack_00000000);
  }
  FUN_ram_1741();
  _DAT_ram_a2fe = 0;
  func_0x1027();
  FUN_ram_0cda(0xf84);
  FUN_ram_0f79(0xf96);
  return;
}



/* === ram:0d6e : FUN_ram_0d6e === */

void FUN_ram_0d6e(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1);
}



/* === ram:0d96 : FUN_ram_0d96 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0d96(byte param_1,undefined2 param_2)

{
  undefined2 uStack0002;
  
  uStack0002 = 0xda1;
  FUN_ram_0d6e(param_1 >> 1 | param_1 << 7,param_2);
  uStack0002 = 0xdab;
  FUN_ram_0dc9(0x82df);
  return;
}



/* === ram:0dc9 : FUN_ram_0dc9 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0dc9(undefined2 param_1,undefined2 param_2)

{
  ushort uVar1;
  undefined2 in_stack_00000000;
  ushort uVar2;
  
  FUN_ram_0d6e();
  uVar1 = _DAT_ram_82d2 & 0xff;
  uVar2 = uVar1;
  FUN_ram_0dc9(param_2);
  if ((char)((ushort)param_1 >> 8) != '\x01') {
    FUN_ram_104a(uVar2);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(in_stack_00000000);
  }
  FUN_ram_104a(uVar2,*(undefined1 *)CONCAT11((char)(uVar1 >> 8),(char)param_1));
  return;
}



/* === ram:0dda : FUN_ram_0dda === */

void FUN_ram_0dda(byte param_1,undefined2 param_2,undefined2 param_3)

{
  undefined1 uVar1;
  undefined1 uVar2;
  
  uVar2 = (undefined1)((ushort)param_2 >> 8);
  uVar1 = (undefined1)param_2;
  func_0x101d(param_1 >> 1 | param_1 << 7);
  *(undefined1 *)CONCAT11(uVar2,uVar1) = (char)param_3;
  ((undefined1 *)CONCAT11(uVar2,uVar1))[1] = (char)((ushort)param_3 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:0e23 : FUN_ram_0e23 === */

/* WARNING: Instruction at (ram,0x0f68) overlaps instruction at (ram,0x0f67)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0e23(byte param_1,byte *param_2)

{
  byte bVar1;
  ushort uVar2;
  short sVar3;
  short sVar4;
  short sStack_2;
  
  sStack_2 = 0xd;
  FUN_ram_0f12(param_1 >> 1 | param_1 << 7);
  sVar4 = _DAT_ram_a2fe;
  if ((char)((ushort)_DAT_ram_a2fe >> 8) == '\0' && (char)_DAT_ram_a2fe == '\0') {
    bVar1 = (byte)param_2;
    *param_2 = bVar1;
  }
  else {
    uVar2 = (ushort)param_2[4];
    FUN_ram_1838(uVar2);
    sVar3 = uVar2 + 1;
    bVar1 = (byte)((ushort)sVar3 >> 8) | (byte)sVar3;
    sStack_2 = sVar4;
    sVar4 = sVar3;
    if (bVar1 == 0) {
      FUN_ram_1741();
      _DAT_ram_a2fe = 0;
      func_0x1027();
      FUN_ram_0cda(0xf84);
      FUN_ram_0f79(0xf96);
      return;
    }
  }
  FUN_ram_0289(bVar1,sVar4);
  FUN_ram_104a();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(sStack_2);
}



/* === ram:0ee4 : FUN_ram_0ee4 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0ee4(byte *param_1,char param_2,char param_3)

{
  byte bVar1;
  byte bVar2;
  
  *param_1 = param_2 + param_3;
  if ((char)((uint3)_DAT_ram_82cd >> 8) == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  bVar2 = *param_1;
  bVar1 = (byte)_DAT_ram_82cd;
  FUN_ram_0f12();
  FUN_ram_1014(bVar1 ^ bVar2);
  return;
}



/* === ram:0f12 : FUN_ram_0f12 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0f12(undefined2 param_1,undefined1 param_2)

{
  byte bVar1;
  char cVar2;
  byte bVar3;
  
  cVar2 = (char)((ushort)param_1 >> 8) + -1;
  if (cVar2 == '\0') {
    return;
  }
  *(undefined1 *)CONCAT11(cVar2,(char)param_1) = param_2;
  if ((char)((uint3)_DAT_ram_82cd >> 8) == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  bVar3 = *(byte *)CONCAT11(cVar2,(char)param_1);
  bVar1 = (byte)_DAT_ram_82cd;
  FUN_ram_0f12();
  FUN_ram_1014(bVar1 ^ bVar3);
  return;
}



/* === ram:0f79 : FUN_ram_0f79 === */

void FUN_ram_0f79(undefined1 param_1,undefined1 *param_2,short param_3)

{
  *param_2 = param_1;
  FUN_ram_0f12(param_3 + 0x30);
  return;
}



/* === ram:1014 : FUN_ram_1014 === */

/* WARNING: Instruction at (ram,0x1142) overlaps instruction at (ram,0x1140)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1014(char param_1,char *param_2,short *param_3,short *param_4)

{
  byte bVar1;
  byte in_F;
  char cVar2;
  short sVar3;
  char cVar4;
  char *pcVar5;
  char cVar6;
  undefined1 uVar7;
  undefined1 uVar8;
  ushort uVar9;
  undefined1 *puVar10;
  byte *pbVar11;
  undefined1 *puVar12;
  undefined1 uStack0000;
  undefined1 auStack_4 [2];
  char *pcStack_2;
  
  *param_3 = *param_3 + 1;
  pcVar5 = (char *)(*(short *)(param_2 + 6) + 1);
  cVar6 = (char)((ushort)pcVar5 >> 8);
  cVar4 = (char)pcVar5;
  *(char **)(param_2 + 6) = pcVar5;
  uVar8 = SUB21(param_2 + 6,0);
  uVar7 = *(undefined1 *)CONCAT11(uVar8,param_1);
  puVar10 = (undefined1 *)CONCAT11(uVar8,uVar7);
  if ((in_F >> 6 & 1) == 0) {
LAB_ram_1100:
    puVar10 = (undefined1 *)CONCAT11((char)((ushort)puVar10 >> 8),(char)param_2);
code_r0x1101:
    *puVar10 = (char)((ushort)puVar10 >> 8);
    if ((in_F >> 6 & 1) == 0) {
      puVar10 = puVar10 + (short)register0x44;
      param_2 = (char *)((ushort)puVar10 & 0xff00);
      puVar12 = puVar10;
      goto LAB_ram_1168;
    }
    if ((in_F >> 6 & 1) == 0) goto LAB_ram_1171;
    uVar8 = SUB21(puVar10,0);
    param_1 = (char)param_2;
    if ((in_F >> 6 & 1) == 0) {
      *param_4 = CONCAT11(uVar8,uVar8) * 2 + _uStack0000;
                    /* WARNING: Subroutine does not return */
      param_4 = (short *)0x1189;
      FUN_ram_0190();
    }
    pbVar11 = (byte *)CONCAT11(uVar8,param_1);
    if ((in_F >> 6 & 1) != 0) {
      *(undefined1 *)CONCAT11(uVar8,*(undefined1 *)CONCAT11(uVar8,param_1)) = uVar8;
      *(undefined1 *)CONCAT11(uVar8,*(undefined1 *)CONCAT11(uVar8,param_1)) =
           *(undefined1 *)CONCAT11(uVar8,param_1);
      pcStack_2 = param_2;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(auStack_4,2);
    }
  }
  else {
    param_1 = (char)param_2;
    *(char *)CONCAT11(param_1,uVar7) = cVar4;
    *(char *)CONCAT11(param_1,uVar7) = param_1;
    *(char *)CONCAT11(uVar7,uVar7) = cVar6;
    cVar2 = *param_2;
    param_2 = (char *)CONCAT11(cVar4,param_1);
    *(char *)CONCAT11(uVar7,cVar2) = cVar4;
    *(char *)CONCAT11(uVar7,cVar2) = cVar6;
    *(char *)CONCAT11(param_1,cVar4) = param_1;
    if ((in_F >> 6 & 1) != 0) {
      uVar9 = CONCAT11(cVar4,cVar4) * 2;
      if ((in_F >> 6 & 1) == 0) goto LAB_ram_10fa;
      puVar10 = &uStack0000 + uVar9;
      if (CARRY2(uVar9,(ushort)&uStack0000)) goto LAB_ram_10fd;
      param_1 = *param_2;
      puVar10 = (undefined1 *)CONCAT11(cVar4,cVar4);
      if ((in_F >> 6 & 1) == 0) {
        param_2 = (char *)0xe506;
        pcStack_2 = (char *)0x112f;
        FUN_ram_10a0(param_1 - cVar4);
        register0x44 = (BADSPACEBASE *)&param_4;
code_r0x1130:
        pcVar5 = *(char **)register0x44;
        *(undefined1 **)register0x44 = puVar10;
LAB_ram_1132:
        *(undefined2 *)((short)register0x44 + -2) = 0x1135;
        FUN_ram_0d06(pcVar5);
        *param_2 = (char)puVar10;
        param_2 = param_2 + 1;
        *param_2 = (char)((ushort)puVar10 >> 8);
        bVar1 = (byte)((ushort)(puVar10 + 1) >> 8) | (byte)(puVar10 + 1);
        if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
          *(undefined2 *)((short)register0x44 + 2) = 0x1252;
          _DAT_ram_9fa1 = param_2;
          pcRam9fa3 = param_2;
          FUN_ram_0190(bVar1);
        }
code_r0x1143:
        FUN_ram_1258(0);
        return;
      }
      param_2 = (char *)CONCAT11(cVar4,cVar4);
      *(char *)CONCAT11(cVar4,cVar4) = cVar6;
      if ((in_F >> 6 & 1) == 0) {
        *pcVar5 = param_1;
        goto code_r0x1143;
      }
      puVar10 = (undefined1 *)CONCAT11(cVar4,*(char *)CONCAT11(cVar4,cVar4));
      if ((in_F >> 6 & 1) == 0) goto LAB_ram_1132;
      puVar10 = (undefined1 *)CONCAT11(cVar4,cVar4);
      if ((in_F >> 6 & 1) != 0) goto LAB_ram_10ed;
      param_2 = (char *)CONCAT11(cVar4 + -1,cVar4);
      if ((char)(cVar4 + -1) == '\0') goto code_r0x1130;
      goto code_r0x1101;
    }
LAB_ram_10ed:
    param_1 = *param_2;
    pcVar5 = (char *)((ushort)pcVar5 & 0xff);
    uVar9 = 0x20;
    DAT_ram_2020 = cVar4;
LAB_ram_10fa:
    puVar10 = (undefined1 *)CONCAT11((char)param_2,(char)uVar9);
    *puVar10 = (char)pcVar5;
LAB_ram_10fd:
    puVar10 = (undefined1 *)CONCAT11((char)puVar10,(char)puVar10);
    puVar12 = (undefined1 *)register0x44;
    if ((in_F >> 6 & 1) != 0) goto LAB_ram_1100;
LAB_ram_1168:
    sVar3 = CONCAT11((char)((ushort)param_2 >> 8),(char)puVar10);
    pcVar5 = (char *)(sVar3 + 4);
    register0x44 = (BADSPACEBASE *)(puVar12 + -2);
    *(char **)(puVar12 + -2) = pcVar5;
    param_1 = *pcVar5;
    pbVar11 = (byte *)(sVar3 + 5);
  }
  puVar10 = (undefined1 *)((ushort)*pbVar11 << 8);
LAB_ram_1171:
  *(ushort *)((short)register0x44 + -2) = CONCAT11((char)((ushort)puVar10 >> 8),param_1);
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)register0x44 + -4) = 0x1176;
  FUN_ram_0190();
}



/* === ram:104a : FUN_ram_104a === */

void FUN_ram_104a(undefined1 param_1)

{
  *(undefined1 *)CONCAT11(0xc3,param_1) = param_1;
  return;
}



/* === ram:1085 : FUN_ram_1085 === */

void FUN_ram_1085(short param_1)

{
  short sVar1;
  
  FUN_ram_011d = param_1;
  sVar1 = param_1;
  FUN_ram_2174();
  param_1 = param_1 + 1;
  FUN_ram_2275(sVar1);
  FUN_ram_22de(param_1,10);
  return;
}



/* === ram:10a0 : FUN_ram_10a0 === */

/* WARNING: Instruction at (ram,0x1142) overlaps instruction at (ram,0x1140)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_10a0(code param_1,code *param_2,undefined1 param_3,code *param_4,short *param_5)

{
  byte in_F;
  byte bVar1;
  code cVar2;
  code cVar3;
  short sVar4;
  code cVar5;
  code cVar6;
  undefined1 uVar7;
  undefined1 uVar8;
  code *pcVar9;
  ushort uVar10;
  undefined1 *puVar11;
  code *pcVar12;
  byte *pbVar13;
  undefined1 *puVar14;
  undefined1 uStack0000;
  undefined1 auStack_4 [2];
  code *pcStack_2;
  
  cVar5 = SUB21(param_4,0);
  *(code *)CONCAT11(param_3,param_3) = cVar5;
  cVar3 = SUB21(param_2,0);
  pcVar12 = (code *)CONCAT11(param_3,cVar3);
  *pcVar12 = cVar5;
  bVar1 = in_F;
  if ((in_F >> 6 & 1) == 0) {
LAB_ram_1108:
    if ((bVar1 >> 6 & 1) == 0) {
      *param_5 = (short)pcVar12 * 2 + _uStack0000;
                    /* WARNING: Subroutine does not return */
      param_5 = (short *)0x1189;
      FUN_ram_0190();
    }
    uVar7 = (undefined1)((ushort)pcVar12 >> 8);
    pbVar13 = (byte *)CONCAT11(uVar7,param_1);
    if ((bVar1 >> 6 & 1) != 0) {
      *(undefined1 *)CONCAT11(uVar7,*(undefined1 *)CONCAT11(uVar7,param_1)) = uVar7;
      uVar8 = *(undefined1 *)CONCAT11(uVar7,(char)param_2);
      *(undefined1 *)CONCAT11(uVar7,uVar8) = uVar8;
      pcStack_2 = param_2;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(auStack_4,2);
    }
  }
  else {
    pcVar9 = (code *)CONCAT11(param_3,*pcVar12);
    if ((in_F >> 6 & 1) == 0) {
LAB_ram_10ef:
      uVar10 = (ushort)pcVar9 & 0xff;
      *(code *)CONCAT11((char)pcVar9,(char)pcVar9) = cVar5;
      cVar3 = *param_2;
      param_4 = (code *)((ushort)param_4 & 0xff);
LAB_ram_10fa:
      puVar11 = (undefined1 *)CONCAT11((char)param_2,(char)uVar10);
      *puVar11 = (char)param_4;
LAB_ram_10fd:
      puVar11 = (undefined1 *)CONCAT11((char)puVar11,(char)puVar11);
      param_1 = cVar3;
      puVar14 = (undefined1 *)register0x44;
      if ((bVar1 >> 6 & 1) != 0) {
LAB_ram_1100:
        puVar11 = (undefined1 *)CONCAT11((char)((ushort)puVar11 >> 8),(char)param_2);
        goto code_r0x1101;
      }
    }
    else {
      *pcVar9 = *pcVar12;
      uVar7 = *(undefined1 *)CONCAT11(*pcVar9,param_1);
      puVar11 = (undefined1 *)CONCAT11(*pcVar9,uVar7);
      if ((in_F >> 6 & 1) == 0) goto LAB_ram_1100;
      *(code *)CONCAT11(cVar3,uVar7) = cVar5;
      *(code *)CONCAT11(cVar3,uVar7) = cVar3;
      cVar6 = SUB21((ushort)param_4 >> 8,0);
      *(code *)CONCAT11(uVar7,uVar7) = cVar6;
      cVar2 = *param_2;
      param_2 = (code *)CONCAT11(cVar5,cVar3);
      *(code *)CONCAT11(uVar7,cVar2) = cVar5;
      *(code *)CONCAT11(uVar7,cVar2) = cVar6;
      *(code *)CONCAT11(cVar3,cVar5) = cVar3;
      if ((in_F >> 6 & 1) == 0) {
LAB_ram_10ed:
        pcVar9 = RST4;
        goto LAB_ram_10ef;
      }
      bVar1 = in_F & 0xec;
      uVar10 = CONCAT11(cVar5,cVar5) * 2;
      if ((in_F >> 6 & 1) == 0) goto LAB_ram_10fa;
      bVar1 = in_F & 0xec;
      puVar11 = &uStack0000 + uVar10;
      if (CARRY2(uVar10,(ushort)&uStack0000)) goto LAB_ram_10fd;
      param_1 = *param_2;
      puVar11 = (undefined1 *)CONCAT11(cVar5,cVar5);
      if ((in_F >> 6 & 1) == 0) {
        param_2 = (code *)0xe506;
        pcStack_2 = (code *)0x112f;
        FUN_ram_10a0((char)param_1 - (char)cVar5);
        register0x44 = (BADSPACEBASE *)&param_5;
code_r0x1130:
        param_4 = *(code **)register0x44;
        *(undefined1 **)register0x44 = puVar11;
LAB_ram_1132:
        *(undefined2 *)((short)register0x44 + -2) = 0x1135;
        FUN_ram_0d06(param_4);
        *param_2 = SUB21(puVar11,0);
        param_2 = param_2 + 1;
        *param_2 = SUB21((ushort)puVar11 >> 8,0);
        bVar1 = (byte)((ushort)(puVar11 + 1) >> 8) | (byte)(puVar11 + 1);
        if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
          *(undefined2 *)((short)register0x44 + 2) = 0x1252;
          _DAT_ram_9fa1 = param_2;
          pcRam9fa3 = param_2;
          FUN_ram_0190(bVar1);
        }
code_r0x1143:
        FUN_ram_1258(0);
        return;
      }
      param_2 = (code *)CONCAT11(cVar5,cVar5);
      *(code *)CONCAT11(cVar5,cVar5) = cVar6;
      if ((in_F >> 6 & 1) == 0) {
        *param_4 = param_1;
        goto code_r0x1143;
      }
      puVar11 = (undefined1 *)CONCAT11(cVar5,*(code *)CONCAT11(cVar5,cVar5));
      if ((in_F >> 6 & 1) == 0) goto LAB_ram_1132;
      puVar11 = (undefined1 *)CONCAT11(cVar5,cVar5);
      if ((in_F >> 6 & 1) != 0) goto LAB_ram_10ed;
      param_2 = (code *)CONCAT11((char)cVar5 + -1,cVar5);
      if ((char)((char)cVar5 + -1) == '\0') goto code_r0x1130;
code_r0x1101:
      *puVar11 = (char)((ushort)puVar11 >> 8);
      if ((bVar1 >> 6 & 1) != 0) {
        if ((bVar1 >> 6 & 1) == 0) goto LAB_ram_1171;
        pcVar12 = (code *)CONCAT11((char)puVar11,(char)puVar11);
        param_1 = SUB21(param_2,0);
        goto LAB_ram_1108;
      }
      puVar11 = puVar11 + (short)register0x44;
      param_2 = (code *)((ushort)puVar11 & 0xff00);
      puVar14 = puVar11;
    }
    sVar4 = CONCAT11((char)((ushort)param_2 >> 8),(char)puVar11);
    pcVar12 = (code *)(sVar4 + 4);
    register0x44 = (BADSPACEBASE *)(puVar14 + -2);
    *(code **)(puVar14 + -2) = pcVar12;
    param_1 = *pcVar12;
    pbVar13 = (byte *)(sVar4 + 5);
  }
  puVar11 = (undefined1 *)((ushort)*pbVar13 << 8);
LAB_ram_1171:
  *(ushort *)((short)register0x44 + -2) = CONCAT11((char)((ushort)puVar11 >> 8),param_1);
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)register0x44 + -4) = 0x1176;
  FUN_ram_0190();
}



/* === ram:1182 : FUN_ram_1182 === */

void FUN_ram_1182(undefined2 param_1)

{
  undefined1 *in_stack_00000000;
  
  *in_stack_00000000 = (char)param_1;
  in_stack_00000000[1] = (char)((ushort)param_1 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:1258 : FUN_ram_1258 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1258(short param_1,undefined2 param_2,undefined2 param_3)

{
  undefined2 uVar1;
  
  FUN_ram_02cb(param_2);
  *(char *)(param_1 + 4) = (char)param_3;
  *(char *)(param_1 + 5) = (char)((ushort)param_3 >> 8);
  uVar1 = _DAT_ram_9fa1;
  *(char *)(param_1 + 0xc) = (char)_DAT_ram_9fa1;
  *(char *)(param_1 + 0xd) = (char)((ushort)uVar1 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:1741 : FUN_ram_1741 === */

void FUN_ram_1741(undefined2 param_1)

{
  byte bVar1;
  
  bVar1 = (byte)((ushort)param_1 >> 8) | (byte)param_1;
  if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar1,CONCAT11(0x2b,(byte)param_1));
  }
  FUN_ram_0f12(DAT_ram_2b0a);
  return;
}



/* === ram:1838 : FUN_ram_1838 === */

void FUN_ram_1838(short param_1,short param_2,short param_3)

{
  undefined1 *puVar1;
  
  puVar1 = *(undefined1 **)(param_2 + param_3);
  *(undefined1 **)(param_2 + param_3) = puVar1 + 1;
  *puVar1 = *(undefined1 *)(param_1 + 4);
  return;
}



/* === ram:1e1a : FUN_ram_1e1a === */

void FUN_ram_1e1a(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1);
}



/* === ram:1eb5 : FUN_ram_1eb5 === */

void FUN_ram_1eb5(void)

{
  func_0x0038();
  return;
}



/* === ram:1f4a : FUN_ram_1f4a === */

void FUN_ram_1f4a(undefined2 param_1,short param_2)

{
  undefined2 in_stack_00000000;
  
  if ((char)((ushort)param_1 >> 8) != '\x01') {
    *(char *)(param_2 + 1) = (char)param_1;
    return;
  }
  if (DAT_ram_0100 == '*') {
                    /* WARNING: Could not recover jumptable at 0x206e. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    (*(code *)(FUN_ram_0005 + (ushort)(byte)((char)((ushort)in_stack_00000000 >> 8) + 0x54)))
              (DAT_ram_05a1);
    return;
  }
  *(undefined2 *)(DAT_ram_0101 + -2) = 0;
  FUN_ram_00d6();
  return;
}



/* === ram:2109 : FUN_ram_2109 === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x2123) */
/* WARNING: Removing unreachable block (ram,0x23fa) */
/* WARNING: Removing unreachable block (ram,0x21d8) */
/* WARNING: Removing unreachable block (ram,0x21da) */
/* WARNING: Removing unreachable block (ram,0x22eb) */
/* WARNING: Removing unreachable block (ram,0x22f6) */
/* WARNING: Removing unreachable block (ram,0x21e9) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_2109(byte param_1,undefined2 param_2,undefined1 *param_3,undefined2 param_4)

{
  byte bVar1;
  byte bVar2;
  char cVar3;
  undefined2 uVar4;
  short sVar5;
  undefined1 *puVar6;
  short sVar7;
  byte *pbVar8;
  char in_stack_00000000;
  
  if (in_stack_00000000 != '\x01') {
    bVar2 = (byte)((ushort)param_2 >> 8);
    if (bVar2 < param_1) {
      return bVar2;
    }
    if (bVar2 == param_1) {
      return (byte)param_2;
    }
    return bVar2;
  }
  bVar2 = ~bRam223a + 1;
  puVar6 = &DAT_ram_2235;
  uVar4 = 0x500;
  bRam2234 = bVar2;
  if ((byte)~bRam223a != 0xff) {
    sVar5 = 0x2234;
    uVar4 = 0x500;
    sVar7 = CONCAT11(~(byte)((ushort)DAT_ram_05b5 >> 8),~(byte)DAT_ram_05b5);
    bVar1 = CARRY2(sVar7 + 1,(ushort)&param_3);
    puVar6 = (undefined1 *)((short)&param_3 + sVar7 + 1);
                    /* WARNING: Call to offcut address within same function */
    bVar2 = func_0x2339();
    if ((bVar1 & 1) != 0) {
      _DAT_ram_05b3 = sVar5 + 1;
      return bVar2;
    }
  }
  *puVar6 = (char)uVar4;
  sVar5 = (short)param_3;
  pbVar8 = (byte *)((short)param_3 + 0xc);
  if (bVar2 == *pbVar8) {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  param_3 = puVar6;
  cVar3 = FUN_ram_0005(CONCAT11((char)((ushort)uVar4 >> 8) + -1,0x10),0xc,sVar5);
  if (cVar3 != -1) {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  bVar2 = FUN_ram_011d(param_4,pbVar8,param_3);
  return bVar2;
}



/* === ram:214d : FUN_ram_214d === */

void FUN_ram_214d(void)

{
  return;
}



/* === ram:2174 : FUN_ram_2174 === */

void FUN_ram_2174(void)

{
  return;
}



/* === ram:2275 : FUN_ram_2275 === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x2298) */
/* WARNING: Removing unreachable block (ram,0x22ab) */
/* WARNING: Removing unreachable block (ram,0x2394) */

undefined1 FUN_ram_2275(void)

{
  FUN_ram_0005(0x80);
  if ((char)((ushort)DAT_ram_05a3 >> 8) != '\0' || (char)DAT_ram_05a3 != '\0') {
    DAT_ram_0080 = *DAT_ram_05a1;
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  return 0;
}



/* === ram:22de : FUN_ram_22de === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_22de(short param_1,undefined1 *param_2,undefined1 *param_3)

{
  char in_stack_00000000;
  
  *param_3 = *param_2;
  if ((char *)(param_1 + -1) != (char *)0x0) {
    *(char *)(param_1 + -1) = (char)_DAT_ram_82ca + in_stack_00000000;
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  return;
}



/* === ram:237d : FUN_ram_237d === */

/* WARNING: Control flow encountered bad instruction data */

byte FUN_ram_237d(short param_1,short param_2)

{
  byte bVar1;
  
  bVar1 = ((char *)(param_1 + param_2))[0x14] & *(char *)(param_1 + param_2) << 7;
  if (-1 < (char)bVar1) {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  return bVar1;
}


