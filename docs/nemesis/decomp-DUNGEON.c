/* Decompiled DUNGEON.COM - Ghidra Z80 decompiler */

/* === ram:0000 : RST0 === */

void RST0(byte *param_1)

{
  short sVar1;
  
  sVar1 = FUN_ram_0005;
  *(undefined2 *)(FUN_ram_0005 + -2) = 0xf;
  FUN_ram_034b();
  *(undefined2 *)(sVar1 + -2) = 0x12;
  FUN_ram_079e();
  if ((*param_1 & 1) == 0) {
    return;
  }
  *(undefined2 *)(sVar1 + -2) = 0x537;
  func_0x0038(8);
  return;
}



/* === ram:0005 : FUN_ram_0005 === */

void FUN_ram_0005(byte *param_1)

{
  FUN_ram_034b();
  FUN_ram_079e();
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
  return;
}



/* === ram:0018 : RST3 === */

void RST3(short param_1,byte *param_2)

{
  if ((*param_2 & 1) == 0) {
    return;
  }
  func_0x0038(8,param_2 + param_1);
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

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void RST6(char param_1,undefined1 *param_2)

{
  byte bVar1;
  undefined1 uVar2;
  undefined2 uVar3;
  
  enableMaskableInterrupts();
  bVar1 = FUN_ram_0d96(param_1 + '\x01');
  if (bVar1 == 0x38) {
    _DAT_ram_a2f2 = param_2 + 3;
    uVar3 = 0xda3;
    FUN_ram_3b6f();
    FUN_ram_0da4(uVar3,_DAT_ram_80ba,_DAT_ram_80b8);
    return;
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



/* === ram:0129 : FUN_ram_0129 === */

byte FUN_ram_0129(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  undefined2 uVar2;
  
  bVar1 = (byte)((ushort)param_1 >> 8) | (byte)param_1;
  if (bVar1 == 0) {
    return 0;
  }
  uVar2 = param_1;
  FUN_ram_0289(bVar1,param_2);
  bVar1 = (byte)uVar2;
  FUN_ram_026b(param_1);
  return ~bVar1;
}



/* === ram:012c : FUN_ram_012c === */

byte FUN_ram_012c(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  undefined2 uVar2;
  
  uVar2 = param_1;
  FUN_ram_0289(param_2);
  bVar1 = (byte)uVar2;
  FUN_ram_026b(param_1);
  return ~bVar1;
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
  FUN_ram_026b();
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
  FUN_ram_026b();
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



/* === ram:019d : thunk_FUN_ram_0296 === */

/* WARNING: Instruction at (ram,0x03b2) overlaps instruction at (ram,0x03b1)
    */
/* WARNING: Removing unreachable block (ram,0x050d) */

void thunk_FUN_ram_0296(char *param_1,undefined1 *param_2)

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



/* === ram:01bb : FUN_ram_01bb === */

/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_01bb(ushort param_1,undefined2 param_2,undefined2 param_3)

{
  char in_F;
  undefined1 *puVar1;
  undefined2 in_stack_00000000;
  
  if ((char)((ushort)in_stack_00000000 >> 8) == '\x01') {
    return;
  }
  if ((param_1 & 0x7f00) == 0 && -1 < (char)param_1) {
    return;
  }
  puVar1 = (undefined1 *)CONCAT11((char)((ushort)param_2 >> 8),0xc2);
  FUN_ram_04c6((char)param_1 << 1 | (byte)in_stack_00000000 & 1,
               CONCAT11((char)((ushort)param_3 >> 8),(byte)param_3 >> 1 | in_F << 7) + 1);
  *puVar1 = 0;
  puVar1[0x14] = 0;
  FUN_ram_013f(puVar1 + 0x15);
  return;
}



/* === ram:01e5 : FUN_ram_01e5 === */

/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x050d) */

byte FUN_ram_01e5(byte param_1,undefined2 param_2,undefined2 param_3,char param_4)

{
  byte bVar1;
  byte *pbVar2;
  
  if (param_4 == '\0') {
    return param_1;
  }
  bVar1 = (byte)((ushort)param_3 >> 8);
  pbVar2 = (byte *)CONCAT11(bVar1 >> 1,(byte)param_3 >> 1 | bVar1 << 7);
  bVar1 = *pbVar2;
  if (bVar1 == 0) {
    return 0;
  }
  if (DAT_ram_0572 != '\0') {
    if (bVar1 == 0x22) {
      DAT_ram_0572 = '\0';
      *pbVar2 = 0;
    }
    pbVar2[1] = 0x20;
    if ((char)((ushort)param_2 >> 8) == '\0') {
      return 0x20;
    }
    bVar1 = func_0x0038(pbVar2 + 2,param_4);
    return bVar1;
  }
  if (bVar1 < 0x61) {
    return bVar1;
  }
  if (bVar1 < 0x7b) {
    return bVar1 - 0x20;
  }
  return bVar1;
}



/* === ram:01ec : FUN_ram_01ec === */

/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x050d) */

byte FUN_ram_01ec(undefined2 param_1,undefined2 param_2)

{
  char in_F;
  byte bVar1;
  byte *pbVar2;
  
  pbVar2 = (byte *)CONCAT11((char)((ushort)param_2 >> 8),(byte)param_2 >> 1 | in_F << 7);
  bVar1 = *pbVar2;
  if (bVar1 == 0) {
    return 0;
  }
  if (DAT_ram_0572 != '\0') {
    if (bVar1 == 0x22) {
      DAT_ram_0572 = '\0';
      *pbVar2 = 0;
    }
    pbVar2[1] = 0x20;
    if ((char)((ushort)param_1 >> 8) == '\0') {
      return 0x20;
    }
    bVar1 = func_0x0038(pbVar2 + 2);
    return bVar1;
  }
  if (bVar1 < 0x61) {
    return bVar1;
  }
  if (bVar1 < 0x7b) {
    return bVar1 - 0x20;
  }
  return bVar1;
}



/* === ram:01f9 : FUN_ram_01f9 === */

byte FUN_ram_01f9(undefined1 param_1,undefined1 *param_2,byte param_3)

{
  *param_2 = param_1;
  return ~param_3;
}



/* === ram:01fa : FUN_ram_01fa === */

byte FUN_ram_01fa(byte param_1)

{
  return ~param_1;
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



/* === ram:020f : FUN_ram_020f === */

void FUN_ram_020f(void)

{
  return;
}



/* === ram:0229 : FUN_ram_0229 === */

/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x031c) */

void FUN_ram_0229(char param_1)

{
  if (param_1 == '\x01') {
    return;
  }
  uRam06d7 = 0;
  return;
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
  uRam05b0 = 0xd3;
  DAT_ram_05af = 0xc9;
  DAT_ram_05b2 = 0xc9;
  _DAT_ram_058f = 0xcc00;
  _DAT_ram_0593 = 0x10;
  _DAT_ram_0591 = 0x40;
  pcVar4 = (char *)0x400;
  _DAT_ram_0595 = 0x400;
  _DAT_ram_05b5 = param_2;
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



/* === ram:026b : FUN_ram_026b === */

/* WARNING: Instruction at (ram,0x03b2) overlaps instruction at (ram,0x03b1)
    */
/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x04aa) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_026b(undefined1 param_1,undefined1 *param_2)

{
  byte bVar1;
  char cVar2;
  byte *pbVar3;
  char *pcVar4;
  
  DAT_ram_05af = 0xc9;
  DAT_ram_05b2 = 0xc9;
  _DAT_ram_058f = 0xcc00;
  _DAT_ram_0593 = 0x10;
  _DAT_ram_0591 = 0x40;
  pcVar4 = (char *)0x400;
  _DAT_ram_0595 = 0x400;
  DAT_io_0032 = param_1;
  bVar1 = FUN_ram_0005(0xb);
  if ((bVar1 & 1) == 0) {
    cVar2 = FUN_ram_0503(*param_2);
    pbVar3 = param_2 + 2;
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



/* === ram:02e5 : FUN_ram_02e5 === */

/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x050d) */

byte FUN_ram_02e5(undefined2 param_1,byte *param_2,short param_3)

{
  byte bVar1;
  
  bVar1 = *param_2;
  if (bVar1 == 0) {
    return 0;
  }
  if (DAT_ram_0572 != '\0') {
    if (bVar1 == 0x22) {
      DAT_ram_0572 = '\0';
      *param_2 = 0;
    }
    param_2[1] = 0x20;
    if ((char)((ushort)param_1 >> 8) == '\0') {
      return 0x20;
    }
    bVar1 = func_0x0038(param_2 + 2,param_3 + 1);
    return bVar1;
  }
  if (bVar1 < 0x61) {
    return bVar1;
  }
  if (bVar1 < 0x7b) {
    return bVar1 - 0x20;
  }
  return bVar1;
}



/* === ram:0313 : FUN_ram_0313 === */

/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x031c) */

void FUN_ram_0313(void)

{
  func_0x0010();
  return;
}



/* === ram:0337 : FUN_ram_0337 === */

void FUN_ram_0337(undefined2 param_1,byte *param_2)

{
  undefined2 in_stack_00000000;
  
  FUN_ram_0446();
  if ((char)((ushort)in_stack_00000000 >> 8) == '\b') {
    RST0();
    return;
  }
  if ((*param_2 & 1) == 0) {
    return;
  }
  func_0x0038(param_1);
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



/* === ram:0446 : FUN_ram_0446 === */

undefined1 FUN_ram_0446(undefined2 param_1,undefined1 *param_2,undefined2 param_3)

{
  undefined1 uVar1;
  
  *param_2 = (char)((ushort)param_3 >> 8);
  if ((char)((ushort)param_1 >> 8) == -1) {
    uVar1 = func_0x0038();
    return uVar1;
  }
  return (char)param_1;
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



/* === ram:079e : FUN_ram_079e === */

/* WARNING: Instruction at (ram,0x088d) overlaps instruction at (ram,0x088c)
    */
/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x09c3) */
/* WARNING: Removing unreachable block (ram,0x0be2) */
/* WARNING: Removing unreachable block (ram,0x0beb) */
/* WARNING: Removing unreachable block (ram,0x09c9) */
/* WARNING: Removing unreachable block (ram,0x0ad5) */
/* WARNING: Removing unreachable block (ram,0x09cf) */
/* WARNING: Removing unreachable block (ram,0x09d5) */
/* WARNING: Removing unreachable block (ram,0x0ae1) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09e1) */
/* WARNING: Removing unreachable block (ram,0x09e7) */
/* WARNING: Removing unreachable block (ram,0x0c1b) */
/* WARNING: Removing unreachable block (ram,0x09ed) */
/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0aed) */
/* WARNING: Removing unreachable block (ram,0x0af6) */
/* WARNING: Removing unreachable block (ram,0x0af9) */
/* WARNING: Removing unreachable block (ram,0x0bfc) */
/* WARNING: Removing unreachable block (ram,0x0bfe) */
/* WARNING: Removing unreachable block (ram,0x0bff) */
/* WARNING: Removing unreachable block (ram,0x0d18) */
/* WARNING: Removing unreachable block (ram,0x0c06) */
/* WARNING: Removing unreachable block (ram,0x0c08) */
/* WARNING: Removing unreachable block (ram,0x0c0b) */
/* WARNING: Removing unreachable block (ram,0x0c10) */
/* WARNING: Removing unreachable block (ram,0x0d2c) */
/* WARNING: Removing unreachable block (ram,0x0c15) */
/* WARNING: Removing unreachable block (ram,0x0d33) */
/* WARNING: Removing unreachable block (ram,0x0d1d) */
/* WARNING: Removing unreachable block (ram,0x09ab) */
/* WARNING: Removing unreachable block (ram,0x0bd6) */
/* WARNING: Removing unreachable block (ram,0x09b1) */
/* WARNING: Removing unreachable block (ram,0x09b7) */
/* WARNING: Removing unreachable block (ram,0x0bdc) */
/* WARNING: Removing unreachable block (ram,0x09bd) */
/* WARNING: Removing unreachable block (ram,0x0c52) */
/* WARNING: Removing unreachable block (ram,0x089e) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0000 : 0x0a54 */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_079e(code **param_1,code *param_2,code *param_3,undefined2 param_4,undefined1 param_5)

{
  byte in_F;
  byte bVar1;
  undefined1 uVar2;
  code cVar3;
  byte bVar4;
  code cVar5;
  code cVar6;
  code *pcVar7;
  undefined2 uVar8;
  code cVar9;
  undefined1 uVar12;
  code *pcVar10;
  short *psVar11;
  byte bVar13;
  code **ppcVar14;
  undefined2 *puVar15;
  code *pcStack_2;
  
  uVar8 = param_2;
  uVar12 = 9;
  param_2 = (code *)0x91d;
  FUN_ram_6286(uVar8);
  uVar8 = param_2;
  param_2 = (code *)0x7ab;
  uVar2 = FUN_ram_6b8d();
  ppcVar14 = &param_3;
  puVar15 = &param_3;
  cVar6 = SUB21(uVar8,0);
  pcVar10 = (code *)CONCAT11(uVar12,uVar2);
  *pcVar10 = cVar6;
  *pcVar10 = cVar6;
  cVar3 = SUB21(param_1,0);
  if ((in_F >> 6 & 1) == 0) {
LAB_ram_0807:
    uVar2 = SUB21(pcVar10,0);
    pcVar10 = (code *)CONCAT11(uVar2,uVar2);
    ppcVar14 = &param_3;
    if ((in_F & 1) != 0) {
      cVar9 = *pcVar10;
      DAT_ram_3433 = cVar3;
      *pcVar10 = (code)((char)cVar9 + -1);
      bVar1 = in_F & 0x29;
      *pcVar10 = (code)0x37;
      if ((bool)(in_F & 1)) goto LAB_ram_084e;
      cVar5 = SUB21(param_1,0);
      *(code *)CONCAT11(cVar5,uVar2) = cVar6;
      *(code *)CONCAT11(cVar5,uVar2) = cVar5;
      param_2 = (code *)CONCAT11(cVar5,cVar3);
      if ((code)((char)cVar9 + -1) != (code)0x0) {
        _DAT_ram_a2f8 = 0;
        return;
      }
      *(code *)CONCAT11(cVar6,cVar5) = cVar5;
      *(code *)CONCAT11(cVar6,cVar5) = cVar6;
      param_1 = &pcStack_2;
      pcVar10 = RST0 + 2;
      ppcVar14 = param_1;
    }
    pcVar10 = pcVar10 + (short)param_1;
    cVar3 = (code)0x0;
  }
  else {
    pcVar10 = (code *)CONCAT11(uVar12,0x2e);
    if ((in_F >> 6 & 1) != 0) {
      *(code *)CONCAT11(cVar3,cVar3) = cVar3;
      ppcVar14 = &param_3;
      if ((in_F >> 6 & 1) == 0) goto code_r0x084a;
      cVar9 = *(code *)CONCAT11(cVar3,cVar3);
      *(code *)CONCAT11(cVar3,cVar9) = cVar3;
      pcVar10 = (code *)CONCAT11(cVar9,cVar9);
      *pcVar10 = cVar6;
      bVar1 = in_F;
      if ((in_F >> 6 & 1) != 0) {
        *pcVar10 = cVar9;
        cVar9 = *(code *)CONCAT11(*pcVar10,cVar3);
        pcVar10 = (code *)CONCAT11(*pcVar10,cVar9);
        param_1 = (code **)CONCAT11(cVar6,cVar3);
        *pcVar10 = cVar9;
        *pcVar10 = cVar6;
        *pcVar10 = cVar6;
        uVar2 = *(undefined1 *)CONCAT11(cVar9,cVar9);
        *(code *)CONCAT11(cVar9,uVar2) = cVar9;
        if ((in_F >> 6 & 1) == 0) {
          param_1 = (code **)0xe500;
          param_2 = (code *)0x873;
          func_0x1ae1();
          puVar15 = &param_4;
          pcVar10 = (code *)((ushort)param_1 & 0xff00);
LAB_ram_0875:
          uVar8 = param_4;
          psVar11 = (short *)CONCAT11((char)((ushort)pcVar10 >> 8),(char)param_1);
          *(short **)((short)puVar15 + -2) = psVar11;
          **(short **)((short)puVar15 + -2) = *psVar11 * 2;
          param_1[1] = param_1[1] + 1;
          _param_4 = CONCAT12(param_5,uVar8);
          FUN_ram_094a(param_1 + 1);
          return;
        }
        *(code *)CONCAT11(uVar2,uVar2) = cVar6;
        *(code *)CONCAT11(uVar2,uVar2) = cVar6;
        cVar9 = *(code *)CONCAT11(uVar2,cVar3);
        pcVar10 = (code *)CONCAT11(uVar2,cVar9);
        if ((in_F >> 6 & 1) == 0) goto LAB_ram_0875;
        *pcVar10 = cVar9;
        *pcVar10 = cVar6;
        *pcVar10 = cVar6;
        pcVar10 = (code *)(ushort)*(byte *)CONCAT11(cVar9,cVar9);
        goto LAB_ram_0807;
      }
LAB_ram_084e:
      param_2 = (code *)0x854;
      bVar4 = FUN_ram_01f9();
      if ((bVar1 & 1) != 0) {
        param_2 = (code *)_DAT_ram_a2f8;
                    /* WARNING: Subroutine does not return */
        FUN_ram_0190();
      }
      bVar1 = (byte)pcVar10;
      bVar13 = (byte)((ushort)pcVar10 >> 8);
      if (bVar4 != bVar1) {
        if (SBORROW1(bVar4,bVar1)) {
                    /* WARNING: Bad instruction - Truncating control flow here */
          halt_baddata();
        }
LAB_ram_0ba2:
        *(byte *)(param_1 + 1) = bVar1;
        *(byte *)((short)param_1 + 3) = bVar13;
        if ((bVar13 | bVar1) == 0) {
          uVar8 = stack0x0007;
          _param_4 = 0xc00d0;
                    /* WARNING: Subroutine does not return */
          FUN_ram_0190(0,uVar8);
        }
                    /* WARNING: Subroutine does not return */
        param_3 = FUN_ram_0bb3;
        FUN_ram_0190(bVar13 | bVar1);
      }
      if (bVar13 == 0) goto LAB_ram_0ba2;
      bVar4 = 8 < bVar1;
      if (bVar1 == 8) {
        if (bVar13 == 0) goto LAB_ram_0ba2;
LAB_ram_0ab1:
        bVar1 = (byte)param_1;
        param_2 = pcVar10 + -1;
        pcStack_2 = (code *)0xaba;
        pcVar10 = _DAT_ram_80ba;
        FUN_ram_42a3();
        param_2 = pcVar10;
        FUN_ram_0cf3();
        uVar2 = 0;
        pcVar7 = param_2;
        if ((char)((ushort)pcVar10 >> 8) != '\0' || (char)pcVar10 != '\0') {
          _DAT_ram_80b8 = _DAT_ram_80b8 + -1;
          FUN_ram_0bd3();
          return;
        }
      }
      else {
        pcVar7 = RST0;
        param_2 = (code *)0xaab;
        uVar2 = FUN_ram_0204();
        bVar1 = (byte)param_1;
        if ((bVar4 & 1) != 0) {
          pcVar10 = _DAT_ram_80b8;
          goto LAB_ram_0ab1;
        }
      }
      bVar4 = 0xfe < bVar1;
      param_2 = pcVar7;
      FUN_ram_01f9(uVar2,bVar1 + 1);
      pcVar7 = _DAT_ram_80b8;
      if ((bVar4 & 1) != 0) {
        pcStack_2 = _DAT_ram_80ba;
        pcVar10 = _DAT_ram_80ba;
        FUN_ram_4228();
        pcStack_2 = (code *)0xa59;
        FUN_ram_0cf3(pcVar7);
        if ((char)((ushort)pcVar10 >> 8) != '\0' || (char)pcVar10 != '\0') {
          pcVar10 = _DAT_ram_80ba + 1;
          register0x44 = (BADSPACEBASE *)&param_2;
          _DAT_ram_80ba = pcVar10;
          goto code_r0x0b6d;
        }
      }
      pcStack_2 = (code *)0xb6d;
      FUN_ram_170e();
code_r0x0b6d:
      if ((char)((ushort)pcVar10 >> 8) != '\0' || (char)pcVar10 != '\0') {
        *(code **)((short)register0x44 + 2) = pcVar10;
                    /* WARNING: Subroutine does not return */
        *(code **)register0x44 = FUN_ram_0cd6;
        FUN_ram_0190();
      }
      _DAT_ram_a2f2 = pcVar10 + 3;
      *(undefined2 *)((short)register0x44 + 2) = 0xda3;
      *(undefined2 *)register0x44 = 0xc88;
      FUN_ram_3b6f(0);
      uVar8 = *(undefined2 *)((short)register0x44 + 2);
      *(code **)((short)register0x44 + 2) = _DAT_ram_80b8;
      *(code **)register0x44 = _DAT_ram_80ba;
      *(undefined2 *)((short)register0x44 + -2) = 0xc94;
      FUN_ram_0da4(uVar8);
      return;
    }
  }
  *pcVar10 = cVar3;
  pcVar10[1] = cVar3;
  *(undefined1 *)param_1 = 1;
  *(undefined1 *)((short)param_1 + 1) = 0;
code_r0x084a:
                    /* WARNING: Subroutine does not return */
  *(code **)((short)ppcVar14 + -2) = FUN_ram_084d;
  FUN_ram_0190();
}



/* === ram:084d : FUN_ram_084d === */

/* WARNING: Instruction at (ram,0x0ccf) overlaps instruction at (ram,0x0ccd)
    */
/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x09c3) */
/* WARNING: Removing unreachable block (ram,0x0be2) */
/* WARNING: Removing unreachable block (ram,0x0beb) */
/* WARNING: Removing unreachable block (ram,0x09c9) */
/* WARNING: Removing unreachable block (ram,0x0ad5) */
/* WARNING: Removing unreachable block (ram,0x09cf) */
/* WARNING: Removing unreachable block (ram,0x09d5) */
/* WARNING: Removing unreachable block (ram,0x0ae1) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09e1) */
/* WARNING: Removing unreachable block (ram,0x09e7) */
/* WARNING: Removing unreachable block (ram,0x0c1b) */
/* WARNING: Removing unreachable block (ram,0x09ed) */
/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0aed) */
/* WARNING: Removing unreachable block (ram,0x0af6) */
/* WARNING: Removing unreachable block (ram,0x0af9) */
/* WARNING: Removing unreachable block (ram,0x0bfc) */
/* WARNING: Removing unreachable block (ram,0x0bfe) */
/* WARNING: Removing unreachable block (ram,0x0bff) */
/* WARNING: Removing unreachable block (ram,0x0d18) */
/* WARNING: Removing unreachable block (ram,0x0c06) */
/* WARNING: Removing unreachable block (ram,0x0c08) */
/* WARNING: Removing unreachable block (ram,0x0c0b) */
/* WARNING: Removing unreachable block (ram,0x0c10) */
/* WARNING: Removing unreachable block (ram,0x0d2c) */
/* WARNING: Removing unreachable block (ram,0x0c15) */
/* WARNING: Removing unreachable block (ram,0x0d33) */
/* WARNING: Removing unreachable block (ram,0x0d1d) */
/* WARNING: Removing unreachable block (ram,0x09ab) */
/* WARNING: Removing unreachable block (ram,0x0bd6) */
/* WARNING: Removing unreachable block (ram,0x09b1) */
/* WARNING: Removing unreachable block (ram,0x09b7) */
/* WARNING: Removing unreachable block (ram,0x0bdc) */
/* WARNING: Removing unreachable block (ram,0x09bd) */
/* WARNING: Removing unreachable block (ram,0x0c52) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0003 : 0x0ccc */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_084d(undefined1 param_1,undefined1 *param_2,short param_3,undefined2 param_4,
                 undefined1 param_5)

{
  byte in_F;
  byte bVar1;
  undefined1 uVar2;
  short sVar3;
  undefined2 uVar4;
  byte bVar5;
  byte bVar6;
  short *psVar7;
  undefined1 auStackY_84 [128];
  short sStack_2;
  
  *param_2 = param_1;
  sStack_2 = 0x854;
  bVar1 = FUN_ram_01f9();
  if ((in_F & 1) != 0) {
    sStack_2 = _DAT_ram_a2f8;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  bVar5 = (byte)param_3;
  bVar6 = (byte)((ushort)param_3 >> 8);
  if (bVar1 != bVar5) {
    if (SBORROW1(bVar1,bVar5)) {
                    /* WARNING: Bad instruction - Truncating control flow here */
      halt_baddata();
    }
LAB_ram_0ba2:
    param_2[2] = bVar5;
    param_2[3] = bVar6;
    if ((bVar6 | bVar5) == 0) {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(0,stack0x0003);
    }
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar6 | bVar5);
  }
  if (bVar6 == 0) goto LAB_ram_0ba2;
  bVar1 = 8 < bVar5;
  if (bVar5 == 8) {
    if (bVar6 == 0) goto LAB_ram_0ba2;
LAB_ram_0ab1:
    bVar5 = (byte)param_2;
    sStack_2 = param_3 + -1;
    param_3 = _DAT_ram_80ba;
    FUN_ram_42a3(_DAT_ram_80ba);
    sStack_2 = param_3;
    FUN_ram_0cf3();
    uVar2 = 0;
    sVar3 = sStack_2;
    if ((char)((ushort)param_3 >> 8) != '\0' || (char)param_3 != '\0') {
      _DAT_ram_80b8 = _DAT_ram_80b8 + -1;
      FUN_ram_0bd3();
      return;
    }
  }
  else {
    sVar3 = 0;
    sStack_2 = 0xaab;
    uVar2 = FUN_ram_0204();
    bVar5 = (byte)param_2;
    if ((bVar1 & 1) != 0) {
      param_3 = _DAT_ram_80b8;
      goto LAB_ram_0ab1;
    }
  }
  bVar1 = 0xfe < bVar5;
  sStack_2 = sVar3;
  FUN_ram_01f9(uVar2,bVar5 + 1);
  sVar3 = _DAT_ram_80b8;
  if ((bVar1 & 1) != 0) {
    param_3 = _DAT_ram_80ba;
    FUN_ram_4228();
    FUN_ram_0cf3(sVar3);
    psVar7 = &sStack_2;
    if ((char)((ushort)param_3 >> 8) != '\0' || (char)param_3 != '\0') {
      param_3 = _DAT_ram_80ba + 1;
      _DAT_ram_80ba = param_3;
      goto code_r0x0b6d;
    }
  }
  FUN_ram_170e();
  psVar7 = (short *)(auStackY_84 + 0x80);
code_r0x0b6d:
  if ((char)((ushort)param_3 >> 8) != '\0' || (char)param_3 != '\0') {
    psVar7[1] = param_3;
                    /* WARNING: Subroutine does not return */
    *psVar7 = (short)FUN_ram_0cd6;
    FUN_ram_0190();
  }
  _DAT_ram_a2f2 = param_3 + 3;
  psVar7[1] = 0xda3;
  *psVar7 = 0xc88;
  FUN_ram_3b6f(0);
  uVar4 = psVar7[1];
  psVar7[1] = _DAT_ram_80b8;
  *psVar7 = _DAT_ram_80ba;
  psVar7[-1] = 0xc94;
  FUN_ram_0da4(uVar4);
  return;
}



/* === ram:085e : FUN_ram_085e === */

/* WARNING: Instruction at (ram,0x0ccf) overlaps instruction at (ram,0x0ccd)
    */
/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x09c3) */
/* WARNING: Removing unreachable block (ram,0x0be2) */
/* WARNING: Removing unreachable block (ram,0x0beb) */
/* WARNING: Removing unreachable block (ram,0x09c9) */
/* WARNING: Removing unreachable block (ram,0x0ad5) */
/* WARNING: Removing unreachable block (ram,0x09cf) */
/* WARNING: Removing unreachable block (ram,0x09d5) */
/* WARNING: Removing unreachable block (ram,0x0ae1) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09e1) */
/* WARNING: Removing unreachable block (ram,0x09e7) */
/* WARNING: Removing unreachable block (ram,0x0c1b) */
/* WARNING: Removing unreachable block (ram,0x09ed) */
/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0aed) */
/* WARNING: Removing unreachable block (ram,0x0af6) */
/* WARNING: Removing unreachable block (ram,0x0af9) */
/* WARNING: Removing unreachable block (ram,0x0bfc) */
/* WARNING: Removing unreachable block (ram,0x0bfe) */
/* WARNING: Removing unreachable block (ram,0x0bff) */
/* WARNING: Removing unreachable block (ram,0x0d18) */
/* WARNING: Removing unreachable block (ram,0x0c06) */
/* WARNING: Removing unreachable block (ram,0x0c08) */
/* WARNING: Removing unreachable block (ram,0x0c0b) */
/* WARNING: Removing unreachable block (ram,0x0c10) */
/* WARNING: Removing unreachable block (ram,0x0d2c) */
/* WARNING: Removing unreachable block (ram,0x0c15) */
/* WARNING: Removing unreachable block (ram,0x0d33) */
/* WARNING: Removing unreachable block (ram,0x0d1d) */
/* WARNING: Removing unreachable block (ram,0x09ab) */
/* WARNING: Removing unreachable block (ram,0x0bd6) */
/* WARNING: Removing unreachable block (ram,0x09b1) */
/* WARNING: Removing unreachable block (ram,0x0993) */
/* WARNING: Removing unreachable block (ram,0x0999) */
/* WARNING: Removing unreachable block (ram,0x0aa5) */
/* WARNING: Removing unreachable block (ram,0x0aae) */
/* WARNING: Removing unreachable block (ram,0x0987) */
/* WARNING: Removing unreachable block (ram,0x098d) */
/* WARNING: Removing unreachable block (ram,0x099f) */
/* WARNING: Removing unreachable block (ram,0x09a5) */
/* WARNING: Removing unreachable block (ram,0x0ab1) */
/* WARNING: Removing unreachable block (ram,0x0abd) */
/* WARNING: Removing unreachable block (ram,0x0ac6) */
/* WARNING: Removing unreachable block (ram,0x0a42) */
/* WARNING: Removing unreachable block (ram,0x0a48) */
/* WARNING: Removing unreachable block (ram,0x0b69) */
/* WARNING: Removing unreachable block (ram,0x0a5f) */
/* WARNING: Removing unreachable block (ram,0x0b6d) */
/* WARNING: Removing unreachable block (ram,0x09b7) */
/* WARNING: Removing unreachable block (ram,0x0bdc) */
/* WARNING: Removing unreachable block (ram,0x09bd) */
/* WARNING: Removing unreachable block (ram,0x0ac9) */
/* WARNING: Removing unreachable block (ram,0x0c52) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0000 : 0x0c88 */
/* WARNING: Removing unreachable block (ram,0x0c7a) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_085e(short param_1,undefined2 param_2,short param_3,undefined2 param_4)

{
  byte bVar1;
  short sVar2;
  byte bVar4;
  undefined2 uVar3;
  short sVar5;
  undefined2 *puVar6;
  short in_stack_00000000;
  
  puVar6 = &param_3;
  bVar4 = (byte)((ushort)param_2 >> 8) & (byte)((ushort)in_stack_00000000 >> 8);
  bVar1 = (byte)param_2 & (byte)in_stack_00000000;
  sVar5 = CONCAT11(bVar4,bVar1);
  if ((bVar4 | bVar1) != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar4 | bVar1);
  }
  param_1 = param_1 + -1;
  if (bVar1 == 0x52) {
    if (bVar4 == 0) {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190();
    }
  }
  else {
    FUN_ram_42a3((char)((ushort)param_1 >> 8) + 'R');
    sVar2 = param_3;
    param_3 = sVar5;
    FUN_ram_0cf3(sVar2);
    puVar6 = &param_4;
    in_stack_00000000 = param_3;
  }
  bVar1 = (byte)((ushort)sVar5 >> 8) | (byte)sVar5;
  bVar4 = bVar1 ^ bVar1 >> 1;
  bVar4 = bVar4 ^ bVar4 >> 2;
  if (bVar1 == 0) {
    puVar6[-1] = sVar5;
    puVar6[-2] = 0xba1;
    FUN_ram_59ea(0,in_stack_00000000);
  }
  else {
    sVar5 = _DAT_ram_80b8 + 1;
    _DAT_ram_80b8 = sVar5;
    if (((bVar4 ^ bVar4 >> 4) & 1) == 0) {
                    /* WARNING: Bad instruction - Truncating control flow here */
      halt_baddata();
    }
  }
  *(byte *)(param_1 + 2) = (byte)sVar5;
  bVar4 = (byte)((ushort)sVar5 >> 8);
  *(byte *)(param_1 + 3) = bVar4;
  bVar4 = bVar4 | (byte)sVar5;
  if (bVar4 == 0) {
    uVar3 = *(undefined2 *)((short)puVar6 + 3);
                    /* WARNING: Subroutine does not return */
    *(code **)((short)puVar6 + 3) = FUN_ram_0cd0;
    FUN_ram_0190(0,uVar3);
  }
                    /* WARNING: Subroutine does not return */
  *puVar6 = FUN_ram_0bb3;
  FUN_ram_0190(bVar4);
}



/* === ram:08be : FUN_ram_08be === */

/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09d5) */
/* WARNING: Removing unreachable block (ram,0x0ae1) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0c7d) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_08be(undefined1 *param_1,char *param_2)

{
  bool bVar1;
  byte bVar2;
  char cVar3;
  char cVar4;
  char cVar5;
  undefined2 uVar6;
  short sVar7;
  short sVar8;
  char *pcVar9;
  undefined2 *puVar10;
  undefined2 uStack_2;
  
  if ((char)((ushort)param_2 >> 8) == '\0' && (char)param_2 == '\0') {
LAB_ram_0bff:
    cVar3 = *param_2;
    pcVar9 = (char *)CONCAT11((char)((ushort)param_2 >> 8),cVar3);
    if (cVar3 == '\0') {
      pcVar9 = pcVar9 + -0x6071;
      cVar3 = *pcVar9;
    }
    else if ((cVar3 != '\x01') && (cVar3 != '\x02')) {
      if (cVar3 == '\x03') {
        uStack_2 = pcVar9;
        func_0x3b05(0x2306);
        puVar10 = &uStack_2;
      }
      else {
        puVar10 = (undefined2 *)((short)&uStack_2 + 1);
      }
      uVar6 = *(undefined2 *)((short)puVar10 + 2);
                    /* WARNING: Subroutine does not return */
      *(undefined2 *)((short)puVar10 + 2) = 0xd38;
      FUN_ram_0190(uVar6);
    }
    uStack_2 = (char *)CONCAT11(pcVar9[1],cVar3);
    FUN_ram_3a6a();
    uVar6 = uStack_2;
                    /* WARNING: Subroutine does not return */
    uStack_2 = (char *)0xd28;
    FUN_ram_0190(uVar6);
  }
  *param_1 = 0;
  param_1[1] = 0;
  uStack_2 = (char *)0xce1;
  FUN_ram_3b6f();
  sVar7 = _DAT_ram_80bc;
  cVar3 = (char)((ushort)_DAT_ram_80bc >> 8);
  if (_DAT_ram_80bc < 0) {
    sVar8 = 0;
    _DAT_ram_80bc = sVar7;
  }
  else {
    cVar4 = (char)param_1 + '\x01';
    cVar5 = (char)((ushort)param_1 >> 8);
    bVar1 = (char)sVar7 == 'C';
    bVar2 = bVar1 << 6;
    if (bVar1) {
      if (cVar3 == '\0') {
        return;
      }
      cVar3 = '+';
      sVar8 = sVar7;
      _DAT_ram_80bc = sVar7;
    }
    else {
      uStack_2 = (char *)0xaf3;
      _DAT_ram_80bc = sVar7;
      cVar3 = FUN_ram_01e5(0x59);
      if ((bVar2 >> 6 & 1) == 0) {
        param_2 = (char *)(sVar7 + CONCAT11(cVar5 + '\x01',cVar4));
        goto LAB_ram_0bff;
      }
      sVar8 = -1;
    }
  }
  uStack_2 = (char *)0xcdb;
  FUN_ram_42a3(cVar3);
  uVar6 = *(undefined2 *)(sVar8 * 2 + -0x6071);
  FUN_ram_3a6a();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(uVar6);
}



/* === ram:094a : FUN_ram_094a === */

/* WARNING: Instruction at (ram,0x0ccf) overlaps instruction at (ram,0x0ccd)
    */
/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x09c3) */
/* WARNING: Removing unreachable block (ram,0x0be2) */
/* WARNING: Removing unreachable block (ram,0x0beb) */
/* WARNING: Removing unreachable block (ram,0x09c9) */
/* WARNING: Removing unreachable block (ram,0x0ad5) */
/* WARNING: Removing unreachable block (ram,0x09cf) */
/* WARNING: Removing unreachable block (ram,0x09d5) */
/* WARNING: Removing unreachable block (ram,0x0ae1) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09e1) */
/* WARNING: Removing unreachable block (ram,0x09e7) */
/* WARNING: Removing unreachable block (ram,0x0c1b) */
/* WARNING: Removing unreachable block (ram,0x09ed) */
/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0aed) */
/* WARNING: Removing unreachable block (ram,0x0af6) */
/* WARNING: Removing unreachable block (ram,0x0af9) */
/* WARNING: Removing unreachable block (ram,0x0bfc) */
/* WARNING: Removing unreachable block (ram,0x0bfe) */
/* WARNING: Removing unreachable block (ram,0x0bff) */
/* WARNING: Removing unreachable block (ram,0x0d18) */
/* WARNING: Removing unreachable block (ram,0x0c06) */
/* WARNING: Removing unreachable block (ram,0x0c08) */
/* WARNING: Removing unreachable block (ram,0x0c0b) */
/* WARNING: Removing unreachable block (ram,0x0c10) */
/* WARNING: Removing unreachable block (ram,0x0d2c) */
/* WARNING: Removing unreachable block (ram,0x0c15) */
/* WARNING: Removing unreachable block (ram,0x0d33) */
/* WARNING: Removing unreachable block (ram,0x0d1d) */
/* WARNING: Removing unreachable block (ram,0x09ab) */
/* WARNING: Removing unreachable block (ram,0x0bd6) */
/* WARNING: Removing unreachable block (ram,0x09b1) */
/* WARNING: Removing unreachable block (ram,0x0993) */
/* WARNING: Removing unreachable block (ram,0x0999) */
/* WARNING: Removing unreachable block (ram,0x0aa5) */
/* WARNING: Removing unreachable block (ram,0x0aae) */
/* WARNING: Removing unreachable block (ram,0x097b) */
/* WARNING: Removing unreachable block (ram,0x0981) */
/* WARNING: Removing unreachable block (ram,0x0963) */
/* WARNING: Removing unreachable block (ram,0x0969) */
/* WARNING: Removing unreachable block (ram,0x0a75) */
/* WARNING: Removing unreachable block (ram,0x0a7b) */
/* WARNING: Removing unreachable block (ram,0x096f) */
/* WARNING: Removing unreachable block (ram,0x0b3c) */
/* WARNING: Removing unreachable block (ram,0x0b61) */
/* WARNING: Removing unreachable block (ram,0x0975) */
/* WARNING: Removing unreachable block (ram,0x0a81) */
/* WARNING: Removing unreachable block (ram,0x0a82) */
/* WARNING: Removing unreachable block (ram,0x0a8d) */
/* WARNING: Removing unreachable block (ram,0x0a92) */
/* WARNING: Removing unreachable block (ram,0x0b9d) */
/* WARNING: Removing unreachable block (ram,0x0987) */
/* WARNING: Removing unreachable block (ram,0x098d) */
/* WARNING: Removing unreachable block (ram,0x0a99) */
/* WARNING: Removing unreachable block (ram,0xd159) */
/* WARNING: Removing unreachable block (ram,0x099f) */
/* WARNING: Removing unreachable block (ram,0x0ba2) */
/* WARNING: Removing unreachable block (ram,0x0bb0) */
/* WARNING: Removing unreachable block (ram,0x0cc9) */
/* WARNING: Removing unreachable block (ram,0x0ccc) */
/* WARNING: Removing unreachable block (ram,0x09a5) */
/* WARNING: Removing unreachable block (ram,0x0ab1) */
/* WARNING: Removing unreachable block (ram,0x0abd) */
/* WARNING: Removing unreachable block (ram,0x0ac6) */
/* WARNING: Removing unreachable block (ram,0x0a42) */
/* WARNING: Removing unreachable block (ram,0x0a48) */
/* WARNING: Removing unreachable block (ram,0x0b69) */
/* WARNING: Removing unreachable block (ram,0x09b7) */
/* WARNING: Removing unreachable block (ram,0x0bdc) */
/* WARNING: Removing unreachable block (ram,0x09bd) */
/* WARNING: Removing unreachable block (ram,0x0ac9) */
/* WARNING: Removing unreachable block (ram,0x0c52) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0002 : 0x0c88 */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_094a(short param_1,undefined2 param_2,undefined1 param_3)

{
  short sVar1;
  char cVar2;
  
  cVar2 = (char)((ushort)param_1 >> 8);
  if (cVar2 != '\0') {
    if ((char)param_1 != '\v') {
      param_1 = _DAT_ram_80ba + 1;
      register0x44 = (BADSPACEBASE *)&param_3;
      sVar1 = param_1;
      if (param_1 == 0) {
        _DAT_ram_a2f2 = _DAT_ram_80ba + 4;
        _DAT_ram_80ba = param_1;
        FUN_ram_3b6f(0);
        FUN_ram_0da4(param_2);
        return;
      }
      goto LAB_ram_0cd2;
    }
    sVar1 = _DAT_ram_80ba;
    if (cVar2 != '\0') goto LAB_ram_0cd2;
  }
  FUN_ram_1b9d();
  param_1 = 0xcf1;
  FUN_ram_3b6f(1);
  sVar1 = _DAT_ram_80ba;
LAB_ram_0cd2:
  _DAT_ram_80ba = sVar1;
  *(short *)((short)register0x44 + -2) = param_1;
                    /* WARNING: Subroutine does not return */
  *(code **)((short)register0x44 + -4) = FUN_ram_0cd6;
  FUN_ram_0190();
}



/* === ram:0b64 : FUN_ram_0b64 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0b64(undefined1 param_1,undefined1 *param_2)

{
  short sVar1;
  undefined2 uVar2;
  
  *param_2 = param_1;
  sVar1 = -1;
  FUN_ram_170e(0xffff);
  if ((char)((ushort)sVar1 >> 8) != '\0' || (char)sVar1 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0x2b08,sVar1);
  }
  _DAT_ram_a2f2 = sVar1 + 3;
  uVar2 = 0xda3;
  FUN_ram_3b6f(0);
  FUN_ram_0da4(uVar2,_DAT_ram_80ba,_DAT_ram_80b8);
  return;
}



/* === ram:0bb3 : FUN_ram_0bb3 === */

void FUN_ram_0bb3(undefined1 param_1,undefined1 *param_2,undefined2 param_3)

{
  byte bVar1;
  undefined2 uVar2;
  undefined2 in_stack_00000001;
  
  *param_2 = param_1;
  uVar2 = 1;
  FUN_ram_170e(1);
  bVar1 = (byte)((ushort)uVar2 >> 8) | (byte)uVar2;
  if (bVar1 == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0,in_stack_00000001);
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(bVar1,0x2b08,param_3,uVar2);
}



/* === ram:0bd3 : FUN_ram_0bd3 === */

/* WARNING: Removing unreachable block (ram,0x09e7) */
/* WARNING: Removing unreachable block (ram,0x0c1b) */
/* WARNING: Removing unreachable block (ram,0x09ed) */
/* WARNING: Removing unreachable block (ram,0x09ff) */
/* WARNING: Removing unreachable block (ram,0x0a05) */
/* WARNING: Removing unreachable block (ram,0x0c7d) */
/* WARNING: Removing unreachable block (ram,0x09f3) */
/* WARNING: Removing unreachable block (ram,0x0c2f) */
/* WARNING: Removing unreachable block (ram,0x09f9) */
/* WARNING: Removing unreachable block (ram,0x0b05) */
/* WARNING: Removing unreachable block (ram,0x09db) */
/* WARNING: Removing unreachable block (ram,0x09e1) */
/* WARNING: Removing unreachable block (ram,0x0c52) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0bd3(undefined1 param_1,code *param_2,undefined2 param_3)

{
  byte bVar1;
  byte in_F;
  code cVar2;
  char cVar3;
  code **ppcVar4;
  undefined2 uVar5;
  code cVar6;
  char cVar9;
  short sVar7;
  code *pcVar8;
  code **ppcVar10;
  undefined1 *puVar11;
  code *pcStack_2;
  
  pcVar8 = (code *)CONCAT11((char)((ushort)param_3 >> 8),param_1);
  cVar2 = *pcVar8;
  if ((bool)(in_F & 1)) {
    pcStack_2 = (code *)0x9be;
    cVar2 = (code)func_0x3e0b();
  }
  cVar3 = (char)pcVar8;
  cVar6 = SUB21(param_2,0);
  if (cVar2 != cVar6) {
    _DAT_ram_80b8 = param_2 + -1;
    FUN_ram_0bd3();
    return;
  }
  cVar9 = (char)((ushort)param_2 >> 8);
  if (cVar9 == '\0') {
    *param_2 = cVar6;
    *param_2 = SUB21(param_3,0);
    *param_2 = cVar6;
    ppcVar10 = &pcStack_2;
    ppcVar4 = &pcStack_2;
    pcStack_2 = (code *)CONCAT11(cVar6,cVar3);
    sVar7 = 4;
  }
  else {
    if (cVar6 != (code)0x1b) {
      pcStack_2 = (code *)0xad9;
      FUN_ram_106b(CONCAT11(cVar6,cVar3 + '\x01'));
      pcStack_2 = param_2;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190();
    }
    ppcVar10 = (code **)register0x44;
    if (cVar9 == '\0') goto LAB_ram_0bff;
    cVar3 = cVar3 + '\x01';
    cVar9 = '\x1b';
    bVar1 = (cVar3 == '\0') << 6;
    sVar7 = 0xce2;
    pcStack_2 = (code *)0xce2;
    thunk_FUN_ram_3dc0();
    pcVar8 = pcStack_2;
    pcStack_2 = (code *)0xaed;
    FUN_ram_3b92(pcVar8);
    pcStack_2 = (code *)0xaf3;
    FUN_ram_01e5(0x59);
    if ((bVar1 >> 6 & 1) != 0) {
      sVar7 = -1;
      pcStack_2 = FUN_ram_0cd6 + 5;
      FUN_ram_42a3();
      uVar5 = *(undefined2 *)(sVar7 * 2 + -0x6071);
      FUN_ram_3a6a();
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(uVar5);
    }
    ppcVar4 = (code **)CONCAT11(cVar9 + '\x01',cVar3);
  }
  param_2 = (code *)((short)ppcVar4 + sVar7);
LAB_ram_0bff:
  cVar2 = *param_2;
  pcVar8 = (code *)CONCAT11((char)((ushort)param_2 >> 8),cVar2);
  if (cVar2 == (code)0x0) {
    pcVar8 = pcVar8 + -0x6071;
    cVar2 = *pcVar8;
  }
  else if ((cVar2 != (code)0x1) && (cVar2 != (code)0x2)) {
    if (cVar2 == (code)0x3) {
      *(code **)((short)ppcVar10 + -2) = pcVar8;
      *(undefined2 *)((short)ppcVar10 + -4) = 0xd33;
      func_0x3b05(0x2306);
      puVar11 = (undefined1 *)((short)ppcVar10 + -2);
    }
    else {
      puVar11 = (undefined1 *)((short)ppcVar10 + -1);
    }
    uVar5 = *(undefined2 *)(puVar11 + 2);
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)(puVar11 + 2) = 0xd38;
    FUN_ram_0190(uVar5);
  }
  *(ushort *)((short)ppcVar10 + -2) = CONCAT11(pcVar8[1],cVar2);
  *(undefined2 *)((short)ppcVar10 + -4) = 0xd24;
  FUN_ram_3a6a();
  uVar5 = *(undefined2 *)((short)ppcVar10 + -2);
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)ppcVar10 + -2) = 0xd28;
  FUN_ram_0190(uVar5);
}



/* === ram:0cd0 : FUN_ram_0cd0 === */

void FUN_ram_0cd0(short param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1 + -1);
}



/* === ram:0cd6 : FUN_ram_0cd6 === */

void FUN_ram_0cd6(short param_1)

{
  undefined2 uVar1;
  
  FUN_ram_42a3();
  uVar1 = *(undefined2 *)(param_1 * 2 + -0x6071);
  FUN_ram_3a6a();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(uVar1);
}



/* === ram:0cf3 : FUN_ram_0cf3 === */

void FUN_ram_0cf3(short param_1)

{
  undefined2 uVar1;
  
  FUN_ram_4228();
  uVar1 = *(undefined2 *)(param_1 * 2 + -0x6069);
  FUN_ram_3a6a();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(uVar1);
}



/* === ram:0d10 : FUN_ram_0d10 === */

void FUN_ram_0d10(short param_1)

{
  undefined2 uVar1;
  
  FUN_ram_42a3();
  uVar1 = *(undefined2 *)(param_1 * 2 + -0x6071);
  FUN_ram_3a6a();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(uVar1);
}



/* === ram:0d96 : FUN_ram_0d96 === */

void FUN_ram_0d96(void)

{
  undefined2 in_stack_00000000;
  
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(in_stack_00000000);
}



/* === ram:0da4 : FUN_ram_0da4 === */

void FUN_ram_0da4(undefined2 param_1)

{
  undefined2 in_stack_00000000;
  
  if ((char)((ushort)param_1 >> 8) == '\0' && (char)param_1 == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0,0xd1,in_stack_00000000);
  }
  return;
}



/* === ram:106b : FUN_ram_106b === */

void FUN_ram_106b(short param_1,undefined2 param_2)

{
  FUN_ram_3cb2();
  *(undefined1 *)(param_1 + 4) = 0x1a;
  *(undefined1 *)(param_1 + 5) = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_2);
}



/* === ram:1182 : FUN_ram_1182 === */

void FUN_ram_1182(short param_1)

{
  byte in_F;
  undefined2 in_stack_00000000;
  
  if ((in_F & 1) == 0) {
    *(undefined1 *)(param_1 + 2) = 1;
    *(undefined1 *)(param_1 + 3) = 0;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(in_stack_00000000);
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:14b5 : FUN_ram_14b5 === */

/* WARNING: Instruction at (ram,0x18f3) overlaps instruction at (ram,0x18f2)
    */
/* WARNING: Removing unreachable block (ram,0x18f0) */
/* WARNING: Removing unreachable block (ram,0x1180) */
/* WARNING: Removing unreachable block (ram,0x18bb) */
/* WARNING: Removing unreachable block (ram,0x18ee) */
/* WARNING: Removing unreachable block (ram,0x18be) */
/* WARNING: Removing unreachable block (ram,0x18c0) */
/* WARNING: Removing unreachable block (ram,0x18c2) */
/* WARNING: Removing unreachable block (ram,0x18c4) */
/* WARNING: Removing unreachable block (ram,0x18c7) */
/* WARNING: Removing unreachable block (ram,0x19e9) */
/* WARNING: Removing unreachable block (ram,0x18dd) */
/* WARNING: Removing unreachable block (ram,0x18e9) */
/* WARNING: Removing unreachable block (ram,0x19f0) */
/* WARNING: Removing unreachable block (ram,0x18f7) */
/* WARNING: Removing unreachable block (ram,0x18f3) */
/* WARNING: Removing unreachable block (ram,0x18f2) */
/* WARNING: Removing unreachable block (ram,0x18f6) */
/* WARNING: Removing unreachable block (ram,0x18f9) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_14b5(short param_1,short param_2)

{
  byte in_F;
  undefined1 uVar1;
  undefined1 uVar2;
  byte bVar4;
  byte *pbVar3;
  byte bVar5;
  undefined1 auStack_d [11];
  short sStack_2;
  
  if ((in_F & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    sStack_2 = 0x14bb;
    FUN_ram_0190();
  }
  sStack_2 = 0x162f;
  _DAT_ram_80bc = param_2;
  FUN_ram_439a();
  bVar4 = (byte)((ushort)_DAT_ram_80bc >> 8);
  if (_DAT_ram_80bc < 0) {
    return;
  }
  uVar1 = (undefined1)_DAT_ram_80bc;
  *(undefined1 *)(param_1 + 4) = uVar1;
  pbVar3 = (byte *)(param_1 + 5);
  *pbVar3 = bVar4;
  bVar5 = (byte)((ushort)pbVar3 >> 8);
  *pbVar3 = bVar5;
  if (bVar4 == 0) {
    *(byte *)CONCAT11(bVar5,*(undefined1 *)((ushort)bVar5 << 8)) = bVar5;
    uVar2 = *(undefined1 *)CONCAT11(bVar5,(char)param_1);
    *(undefined1 *)CONCAT11(bVar5,uVar2) = uVar2;
    *(undefined1 *)CONCAT11(uVar2,uVar2) = uVar1;
    *(undefined1 *)CONCAT11(uVar2,*(undefined1 *)CONCAT11(uVar2,uVar2)) = uVar2;
    sStack_2 = param_1;
    FUN_ram_62c4(auStack_d);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0x19b3);
  }
  return;
}



/* FAILED ram:170e : Low-level Error: Bad instruction count exceeded:
    ...
    Instruction at (ram,0x18c3) overlaps instruction at (ram,0x18c2)
 */

/* === ram:1a20 : FUN_ram_1a20 === */

void FUN_ram_1a20(char *param_1,short param_2)

{
  byte bVar1;
  byte bVar2;
  char cVar3;
  char cVar4;
  short sVar5;
  undefined2 in_stack_00000000;
  
  *param_1 = (char)param_2;
  cVar3 = (char)((ushort)param_2 >> 8);
  param_1[1] = cVar3;
  cVar4 = param_1[6];
  if ((cVar4 == '\x01') || (cVar4 == '\x03')) {
    sVar5 = *(short *)(CONCAT11((char)((ushort)(param_1 + 6) >> 8),cVar4) + param_2);
    FUN_ram_61f7(sVar5);
    cVar4 = '\0';
    if ((char)((ushort)sVar5 >> 8) != '\0' || (char)sVar5 != '\0') {
      bVar1 = (byte)((ushort)(sVar5 + -1) >> 8) | (byte)(sVar5 + -1);
      bVar2 = (bVar1 == 0) << 6;
      thunk_FUN_ram_0296(bVar1,0x2b1e,in_stack_00000000);
      if ((bVar2 >> 6 & 1) != 0) {
        func_0x3b05(0xf);
        FUN_ram_1d84(0);
        return;
      }
      if ((bVar2 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
        thunk_FUN_ram_0190(&DAT_ram_a2f8);
      }
      FUN_ram_1ee7(0);
      return;
    }
  }
  else if (cVar4 != '\x02') {
    if (cVar4 == '\x04') {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(CONCAT11(cVar3,(char)param_2 + '\x01'));
    }
    if (cVar4 == '\x05') {
      FUN_ram_1d33();
      return;
    }
    FUN_ram_1b8e();
    return;
  }
  FUN_ram_1c69(cVar4);
  return;
}



/* === ram:1afa : FUN_ram_1afa === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1afa(short param_1)

{
  ushort uVar1;
  short in_stack_00000000;
  
  if ((char)((ushort)param_1 >> 8) != '\0' || (char)param_1 != '\0') {
    uVar1 = (ushort)*(byte *)(param_1 + 2);
    func_0x02fa();
    _DAT_ram_80a8 = uVar1 + in_stack_00000000;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_3b6f(0x2b);
  FUN_ram_1d33(0,0x1d4f);
  return;
}



/* === ram:1b8e : FUN_ram_1b8e === */

void FUN_ram_1b8e(short param_1,short param_2)

{
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(0x80d6,*(undefined1 *)(param_1 + param_2));
}



/* === ram:1b9d : FUN_ram_1b9d === */

/* WARNING: Removing unreachable block (ram,0x1cab) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1b9d(short param_1,short param_2)

{
  byte bVar1;
  
  bVar1 = (byte)((ushort)param_1 >> 8) | (byte)param_1;
  if (bVar1 != 0) {
    func_0x02fa(bVar1);
    _DAT_ram_80aa = param_1 + param_2;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_1d33(0);
  return;
}



/* === ram:1bfe : FUN_ram_1bfe === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1bfe(short param_1)

{
  ushort uVar1;
  short in_stack_00000000;
  
  FUN_ram_026b(5);
  uVar1 = (ushort)*(byte *)(param_1 + -0x5d6e);
  func_0x02fa();
  _DAT_ram_80a8 = uVar1 + in_stack_00000000;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:1c69 : FUN_ram_1c69 === */

void FUN_ram_1c69(undefined2 param_1)

{
  func_0x3b05(0xf);
  FUN_ram_1d84(param_1);
  return;
}



/* === ram:1d33 : FUN_ram_1d33 === */

void FUN_ram_1d33(char param_1,undefined2 param_2,short param_3)

{
  FUN_ram_026b(param_1 - (char)((ushort)param_2 >> 8),0x1014);
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(0x80d8,*(undefined1 *)(param_3 + -0x5dbc));
}



/* === ram:1d84 : FUN_ram_1d84 === */

void FUN_ram_1d84(short param_1)

{
  FUN_ram_026b(5);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(CONCAT11((char)((ushort)(param_1 + -0x5dbd) >> 8),*(undefined1 *)(param_1 + -0x5dbd))
              );
}



/* === ram:1dbd : FUN_ram_1dbd === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1dbd(short param_1)

{
  ushort uVar1;
  short in_stack_00000000;
  
  uVar1 = (ushort)*(byte *)(param_1 + 2);
  func_0x02fa();
  _DAT_ram_80aa = uVar1 + in_stack_00000000;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:1ee7 : FUN_ram_1ee7 === */

/* WARNING: Instruction at (ram,0x25d9) overlaps instruction at (ram,0x25d8)
    */
/* WARNING: Removing unreachable block (ram,0x25ad) */
/* WARNING: Removing unreachable block (ram,0x25a8) */
/* WARNING: Removing unreachable block (ram,0x2616) */
/* WARNING: Removing unreachable block (ram,0x25b3) */
/* WARNING: Removing unreachable block (ram,0x25d9) */
/* WARNING: Removing unreachable block (ram,0x25bf) */
/* WARNING: Removing unreachable block (ram,0x25f7) */
/* WARNING: Removing unreachable block (ram,0x25c7) */
/* WARNING: Removing unreachable block (ram,0x25d5) */
/* WARNING: Removing unreachable block (ram,0x25dc) */
/* WARNING: Removing unreachable block (ram,0x25b9) */
/* WARNING: Removing unreachable block (ram,0x25bb) */
/* WARNING: Removing unreachable block (ram,0x262a) */
/* WARNING: Removing unreachable block (ram,0x25b0) */
/* WARNING: Removing unreachable block (ram,0x25a4) */
/* WARNING: Removing unreachable block (ram,0x260f) */
/* WARNING: Removing unreachable block (ram,0x24bd) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1ee7(short param_1,undefined2 param_2,undefined2 param_3,short param_4,
                 undefined1 param_5,undefined1 param_6)

{
  byte in_F;
  char cVar1;
  byte bVar2;
  char cVar3;
  ushort uVar4;
  short sVar5;
  undefined2 uVar6;
  undefined1 *puVar7;
  
  if ((bool)(in_F & 1)) {
    func_0xd125();
  }
  if ((bool)(in_F >> 2 & 1)) {
    return;
  }
  if ((char)((ushort)param_2 >> 8) == '\0' && (char)param_2 == '\0') {
    FUN_ram_25a2(0);
    return;
  }
  uVar4 = (ushort)FUN_ram_0005;
  FUN_ram_023f(param_3);
  func_0x02fa();
  puVar7 = &param_5;
  *(ushort *)(param_1 + 4) = uVar4 + param_4;
  sVar5 = uVar4 + param_4 + 2;
  cVar1 = (sVar5 < 0) << 7;
  cVar3 = (char)((ushort)param_1 >> 8);
  bVar2 = FUN_ram_01bb((char)((ushort)sVar5 >> 8));
  if (cVar1 < '\0') {
    bVar2 = (bVar2 & (byte)RST0) + cVar3;
    uVar6 = 0;
  }
  else {
    puVar7 = &param_6;
    uVar6 = _DAT_ram_80a6;
  }
  *(undefined2 *)(puVar7 + -2) = uVar6;
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)(puVar7 + -4) = 0x2731;
  FUN_ram_0190(bVar2);
}



/* === ram:2272 : FUN_ram_2272 === */

/* WARNING: Removing unreachable block (ram,0x2287) */
/* WARNING: Removing unreachable block (ram,0x228d) */
/* WARNING: Removing unreachable block (ram,0x2399) */
/* WARNING: Removing unreachable block (ram,0x2293) */
/* WARNING: Removing unreachable block (ram,0x2299) */

void FUN_ram_2272(short param_1,short param_2)

{
  byte in_F;
  
  if (!(bool)(in_F >> 6 & 1)) {
    if ((char)param_1 != '\r') {
      FUN_ram_27ca(0xd100);
      FUN_ram_25a2(param_1 + param_2);
      return;
    }
    if ((char)((ushort)param_1 >> 8) != '\0') {
      FUN_ram_25a2(0);
      return;
    }
  }
  return;
}



/* === ram:22f9 : FUN_ram_22f9 === */

void FUN_ram_22f9(undefined2 param_1)

{
  FUN_ram_27ca();
  FUN_ram_25a2(param_1);
  return;
}



/* === ram:2313 : FUN_ram_2313 === */

void FUN_ram_2313(undefined2 param_1)

{
  FUN_ram_25dc();
  FUN_ram_25a2(param_1);
  return;
}



/* === ram:23b2 : FUN_ram_23b2 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_23b2(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  byte bVar3;
  undefined2 uVar2;
  
  bVar3 = (byte)((ushort)param_2 >> 8);
  bVar1 = bVar3 | (byte)param_2;
  if (bVar1 != 0) {
    uVar2 = _DAT_ram_80a2;
    FUN_ram_023f((bVar1 & bVar3) + (char)((ushort)param_1 >> 8),param_2);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(uVar2);
  }
  FUN_ram_25a2(0);
  return;
}



/* === ram:23fc : FUN_ram_23fc === */

void FUN_ram_23fc(undefined2 param_1)

{
  FUN_ram_2770();
  FUN_ram_25a2(param_1);
  return;
}



/* === ram:25a2 : FUN_ram_25a2 === */

/* WARNING: Instruction at (ram,0x25d9) overlaps instruction at (ram,0x25d8)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_25a2(code param_1,code *param_2,code *param_3,short param_4,short param_5,
                 undefined1 param_6)

{
  byte in_F;
  byte bVar1;
  code *pcVar2;
  char cVar3;
  undefined2 uVar4;
  code *pcVar5;
  ushort uVar6;
  code *pcVar7;
  undefined2 in_stack_00000000;
  undefined1 auStack_8 [6];
  code *pcStack_2;
  
  if ((in_F >> 6 & 1) == 0) {
LAB_ram_25fd:
    uVar6 = (ushort)(byte)param_3[0xb];
    FUN_ram_023f(in_stack_00000000);
    func_0x02fa();
    register0x44 = (BADSPACEBASE *)&param_6;
    param_4 = uVar6 + param_5;
    param_2[4] = SUB21(param_4,0);
    pcVar5 = param_2 + 5;
  }
  else {
    param_3 = (code *)CONCAT11((char)((ushort)param_3 >> 8),param_1);
    *param_3 = param_1;
    if ((in_F >> 6 & 1) == 0) {
      param_2 = (code *)CONCAT11((char)((ushort)param_2 >> 8) + '\x01',(char)param_2);
      goto LAB_ram_25fd;
    }
    halt();
    pcVar5 = (code *)CONCAT11(param_1,param_1);
    if ((in_F >> 6 & 1) != 0) {
      *pcVar5 = param_1;
      pcVar2 = param_2;
      pcVar7 = (code *)register0x44;
      if ((in_F >> 6 & 1) != 0) {
        *pcVar5 = param_1;
        if ((in_F >> 6 & 1) == 0) goto LAB_ram_261b;
        pcVar5 = (code *)CONCAT11(param_1,(char)param_2);
        *pcVar5 = param_1;
        *pcVar5 = SUB21(param_4,0);
        if ((in_F >> 6 & 1) == 0) {
          pcVar2 = (code *)0xe500;
          goto code_r0x25dc;
        }
        if ((in_F >> 6 & 1) == 0) goto LAB_ram_2614;
        *(code *)CONCAT11(param_1,param_1) = param_1;
        if ((in_F >> 6 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
          FUN_ram_020f(in_stack_00000000);
        }
        if ((in_F >> 6 & 1) == 0) {
          param_3 = (code *)(CONCAT11(param_1,param_1) * 4 + -0x7c71);
          goto LAB_ram_25fd;
        }
        pcVar2 = (code *)auStack_8;
        param_4 = _DAT_ram_a2e4;
        pcVar5 = pcVar2;
        pcVar7 = pcVar2;
        pcStack_2 = param_2;
      }
      *pcVar5 = SUB21(param_4,0);
      pcVar5[1] = SUB21((ushort)param_4 >> 8,0);
      pcVar5 = RST0 + 1;
      register0x44 = (BADSPACEBASE *)(pcVar7 + -2);
      *(undefined2 *)(pcVar7 + -2) = 1;
code_r0x25dc:
      *(undefined2 *)((short)register0x44 + -2) = 0x25df;
      FUN_ram_6c75(pcVar2);
      uVar4 = *(undefined2 *)register0x44;
      *(code **)register0x44 = pcVar5;
      *(undefined2 *)((short)register0x44 + -2) = _DAT_ram_80a4;
      *(undefined2 *)((short)register0x44 + -4) = _DAT_ram_80bc;
                    /* WARNING: Subroutine does not return */
      *(undefined2 *)((short)register0x44 + -6) = 0x25ec;
      FUN_ram_0190(uVar4);
    }
    *param_2 = param_1;
  }
LAB_ram_2614:
  *pcVar5 = SUB21((ushort)param_4 >> 8,0);
  param_1 = SUB21((ushort)(param_4 + 2) >> 8,0);
  in_F = (param_4 + 2 < 0) << 7;
LAB_ram_261b:
  cVar3 = (char)((ushort)param_2 >> 8);
  *(undefined2 *)((short)register0x44 + -2) = 0x261e;
  bVar1 = FUN_ram_01bb(param_1);
  if ((char)in_F < '\0') {
    bVar1 = (bVar1 & (byte)RST0) + cVar3;
    uVar4 = 0;
  }
  else {
    register0x44 = (BADSPACEBASE *)((short)register0x44 + 2);
    uVar4 = _DAT_ram_80a6;
  }
  *(undefined2 *)((short)register0x44 + -2) = uVar4;
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)register0x44 + -4) = 0x2731;
  FUN_ram_0190(bVar1);
}



/* === ram:25dc : FUN_ram_25dc === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_25dc(void)

{
  undefined2 in_stack_00000000;
  
  FUN_ram_6c75();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(in_stack_00000000,_DAT_ram_80bc,_DAT_ram_80a4);
}



/* === ram:2770 : FUN_ram_2770 === */

void FUN_ram_2770(ushort param_1,ushort param_2,undefined2 param_3)

{
  short sVar1;
  byte bVar2;
  char cVar3;
  char *pcVar4;
  undefined2 uVar5;
  undefined1 uVar6;
  undefined2 uVar7;
  
  bVar2 = CARRY2(param_2,param_1);
  *(undefined1 *)(param_2 + param_1) = (char)param_3;
  ((undefined1 *)(param_2 + param_1))[1] = (char)((ushort)param_3 >> 8);
  pcVar4 = (char *)0x0;
  FUN_ram_0205();
  if (!(bool)(bVar2 & 1)) {
    uVar5 = 1;
    uVar7 = 1;
    FUN_ram_6c75(FUN_ram_0005);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(uVar7,uVar5);
  }
  cVar3 = *pcVar4;
  if (cVar3 == '\0') {
    sVar1 = CONCAT11((char)((ushort)pcVar4 >> 8),cVar3) * 4;
    uVar6 = *(undefined1 *)(sVar1 + -0x7c67);
    *(undefined1 *)(param_1 + 2) = *(undefined1 *)(sVar1 + -0x7c68);
    *(undefined1 *)(param_1 + 3) = uVar6;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_4429(cVar3);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:27ca : FUN_ram_27ca === */

void FUN_ram_27ca(short param_1,short *param_2)

{
  *(undefined1 *)(param_1 + 0xb) = *(undefined1 *)(*param_2 * 0x10 + -0x7c66);
  *(undefined1 *)(param_1 + 0xc) = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:28aa : FUN_ram_28aa === */

void FUN_ram_28aa(short param_1)

{
  undefined1 *puVar1;
  undefined2 uVar2;
  undefined2 uVar3;
  
  puVar1 = (undefined1 *)(param_1 * 8 + -0x7c69);
  uVar2 = 1;
  uVar3 = 1;
  FUN_ram_6c75(CONCAT11((char)((ushort)puVar1 >> 8),*puVar1));
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(uVar3,uVar2);
}



/* === ram:2954 : FUN_ram_2954 === */

void FUN_ram_2954(undefined2 param_1)

{
  func_0x3b05(0);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1);
}



/* === ram:2da3 : FUN_ram_2da3 === */

void FUN_ram_2da3(undefined1 param_1,undefined1 *param_2,short param_3)

{
  *param_2 = param_1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_3 + 5);
}



/* === ram:2e5e : FUN_ram_2e5e === */

/* WARNING: Instruction at (ram,0x30ca) overlaps instruction at (ram,0x30c9)
    */
/* WARNING: This function may have set the stack pointer */
/* WARNING: Removing unreachable block (ram,0x3160) */
/* WARNING: Removing unreachable block (ram,0x2fa9) */
/* WARNING: Removing unreachable block (ram,0x30de) */
/* WARNING: Removing unreachable block (ram,0x2faf) */
/* WARNING: Removing unreachable block (ram,0x30bb) */
/* WARNING: Removing unreachable block (ram,0x2f9d) */
/* WARNING: Removing unreachable block (ram,0x2fa3) */
/* WARNING: Removing unreachable block (ram,0x30af) */
/* WARNING: Removing unreachable block (ram,0x2fc1) */
/* WARNING: Removing unreachable block (ram,0x2fc7) */
/* WARNING: Removing unreachable block (ram,0x2fb5) */
/* WARNING: Removing unreachable block (ram,0x2fbb) */
/* WARNING: Removing unreachable block (ram,0x30c7) */
/* WARNING: Removing unreachable block (ram,0x2fe3) */
/* WARNING: Removing unreachable block (ram,0x30ea) */
/* WARNING: Removing unreachable block (ram,0x2fe7) */
/* WARNING: Removing unreachable block (ram,0x2fd9) */
/* WARNING: Removing unreachable block (ram,0x311c) */
/* WARNING: Removing unreachable block (ram,0x311f) */
/* WARNING: Removing unreachable block (ram,0x3239) */
/* WARNING: Removing unreachable block (ram,0x3131) */
/* WARNING: Removing unreachable block (ram,0x2fd5) */
/* WARNING: Removing unreachable block (ram,0x30dc) */
/* WARNING: Removing unreachable block (ram,0x30df) */
/* WARNING: Removing unreachable block (ram,0x30e4) */
/* WARNING: Removing unreachable block (ram,0x30e7) */
/* WARNING: Removing unreachable block (ram,0x30eb) */
/* WARNING: Removing unreachable block (ram,0x30ec) */
/* WARNING: Removing unreachable block (ram,0x3164) */
/* WARNING: Removing unreachable block (ram,0x30f0) */
/* WARNING: Removing unreachable block (ram,0x3157) */
/* WARNING: Removing unreachable block (ram,0x30f3) */
/* WARNING: Removing unreachable block (ram,0x30f9) */
/* WARNING: Removing unreachable block (ram,0x310b) */
/* WARNING: Removing unreachable block (ram,0x3162) */
/* WARNING: Removing unreachable block (ram,0x3165) */
/* WARNING: Removing unreachable block (ram,0x2fdc) */
/* WARNING: Removing unreachable block (ram,0x3145) */
/* WARNING: Removing unreachable block (ram,0x3148) */
/* WARNING: Heritage AFTER dead removal. Example location: r0x90cd : 0x30b1 */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */
/* WARNING: Restarted to delay deadcode elimination for space: ram */

void FUN_ram_2e5e(undefined2 param_1,char *param_2)

{
  char cVar2;
  undefined2 uVar1;
  
  cVar2 = (char)((ushort)param_2 >> 8);
  if (cVar2 != '\0' || (char)param_2 != '\0') {
    FUN_ram_431e();
    func_0x3b05(0xf);
                    /* WARNING: Subroutine does not return */
    thunk_FUN_ram_0190(0);
  }
  *param_2 = cVar2;
  RST0 = SUB21((ushort)param_1 >> 8,0);
  *(code *)CONCAT11(RST0,RST0) = RST0;
  uVar1 = 0x17;
  func_0x3b05(0x17);
  FUN_ram_3b92();
  if ((char)uVar1 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  if ((char)((ushort)uVar1 >> 8) != '\0') {
    _DAT_ram_90cb = 0x80d0;
    _DAT_ram_90c9 = 0x30b0;
    FUN_ram_5d4c(0xe500);
    uVar1 = _DAT_ram_90cd;
                    /* WARNING: Subroutine does not return */
    _DAT_ram_90cd = 0x30b5;
    FUN_ram_0190(uVar1);
  }
  func_0x0038();
  func_0x0038();
  FUN_ram_31cf();
  return;
}



/* === ram:311b : FUN_ram_311b === */

void FUN_ram_311b(short param_1)

{
  char cVar1;
  short sVar2;
  undefined1 uVar3;
  undefined1 *in_stack_00000000;
  
  FUN_ram_026b(0x25);
  cVar1 = *(char *)(param_1 + -0x7ef6);
  sVar2 = CONCAT11((char)((ushort)(param_1 + -0x7ef6) >> 8),cVar1);
  if (cVar1 == -1) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_026b();
  uVar3 = *(undefined1 *)(sVar2 + -0x6fea);
  *in_stack_00000000 = *(undefined1 *)(sVar2 + -0x6feb);
  in_stack_00000000[1] = uVar3;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:3131 : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:31c1 : FUN_ram_31c1 === */

void FUN_ram_31c1(undefined1 param_1,undefined2 param_2)

{
  undefined1 *in_stack_00000000;
  
  *in_stack_00000000 = param_1;
  in_stack_00000000[1] = (char)((ushort)param_2 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:31cf : FUN_ram_31cf === */

void FUN_ram_31cf(undefined1 param_1,undefined1 *param_2,undefined2 param_3,undefined1 param_4)

{
  *param_2 = param_1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(CONCAT11((char)((ushort)param_3 >> 8),param_4) + -0x7f14);
}



/* === ram:3a6a : FUN_ram_3a6a === */

char FUN_ram_3a6a(char param_1,char *param_2)

{
  byte in_F;
  
  return param_1 + *param_2 + (in_F & 1);
}



/* === ram:3a96 : FUN_ram_3a96 === */

/* WARNING: Instruction at (ram,0x3d08) overlaps instruction at (ram,0x3d06)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_3a96(short param_1)

{
  undefined1 *puVar1;
  undefined2 uVar2;
  byte bVar3;
  undefined2 *puVar4;
  ushort uVar5;
  short sVar6;
  undefined1 auStack_7ffe [32760];
  ushort auStack_6 [2];
  undefined1 uStack_1;
  
  puVar4 = (undefined2 *)(param_1 + (short)register0x44);
  puVar1 = &LAB_ram_3c8b_1;
  *(undefined1 *)puVar4 = 0x8c;
  *(undefined1 *)((short)puVar4 + 1) = 0x3c;
  *(undefined1 *)((short)puVar4 + 3) = 0x8c;
  *(undefined1 *)((short)puVar4 + 4) = 0x3c;
  bVar3 = **(byte **)((short)puVar4 + 3);
  sVar6 = CONCAT11(*(undefined1 *)((short)puVar4 + 4),bVar3);
  if (bVar3 == 0x58) {
    bVar3 = *(byte *)(*(short *)((short)puVar4 + 3) + 1);
    sVar6 = CONCAT11((char)((ushort)(*(short *)((short)puVar4 + 3) + 1) >> 8),bVar3);
    if (bVar3 == 0x58) {
                    /* WARNING: Subroutine does not return */
      *(undefined2 *)((short)puVar4 + -2) = 0x3acc;
      FUN_ram_0190();
    }
  }
  bVar3 = bVar3 | (byte)sVar6;
  if (bVar3 == 0) {
    puVar4 = (undefined2 *)((short)puVar4 + -1);
    bVar3 = 0;
  }
  else {
    *(undefined2 *)((short)puVar4 + -2) = _DAT_ram_a2fe;
    uVar5 = (ushort)*(byte *)((short)puVar4 + 4);
    *(ushort *)((short)puVar4 + -4) = uVar5;
    *(undefined2 *)((short)puVar4 + -6) = 0x3be4;
    FUN_ram_630c(bVar3);
    puVar1 = *(undefined1 **)((short)puVar4 + -2);
    sVar6 = uVar5 + 1;
    bVar3 = (byte)((ushort)sVar6 >> 8) | (byte)sVar6;
    if (bVar3 == 0) {
      *(undefined2 *)((short)puVar4 + -2) = 0x3bef;
      FUN_ram_4372();
      _DAT_ram_a2fe = 0;
      *(undefined2 *)((short)puVar4 + -2) = 0x3d24;
      *(undefined2 *)((short)puVar4 + -4) = 0x3bfc;
      FUN_ram_3b6f();
      uVar2 = *(undefined2 *)((short)puVar4 + -2);
      *(undefined2 *)((short)puVar4 + -2) = 0x3d36;
      *(undefined2 *)((short)puVar4 + -4) = 0x3c04;
      FUN_ram_3a6a(uVar2);
      FUN_ram_3d19(*(undefined2 *)((short)puVar4 + -2));
      return;
    }
  }
  puVar4[-1] = sVar6;
  puVar4[-2] = 0x3d09;
  func_0x3b05(bVar3,puVar1);
  uVar2 = *puVar4;
                    /* WARNING: Subroutine does not return */
  *puVar4 = 0x3d0e;
  thunk_FUN_ram_0190(uVar2);
}



/* === ram:3b6f : FUN_ram_3b6f === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_3b6f(byte param_1,short param_2,undefined2 param_3)

{
  byte bVar1;
  undefined1 *puVar2;
  ushort uVar3;
  short sVar4;
  undefined2 in_stack_00000000;
  undefined2 uVar5;
  undefined2 uVar6;
  
  puVar2 = (undefined1 *)(param_2 + 2);
  *puVar2 = (char)param_3;
  uVar3 = (ushort)*(byte *)(param_2 + 4);
  FUN_ram_630c(param_1 & (byte)((ushort)param_3 >> 8),uVar3);
  sVar4 = uVar3 + 1;
  bVar1 = (byte)((ushort)sVar4 >> 8) | (byte)sVar4;
  if (bVar1 != 0) {
    func_0x3b05(bVar1,puVar2,sVar4);
                    /* WARNING: Subroutine does not return */
    thunk_FUN_ram_0190(in_stack_00000000);
  }
  FUN_ram_4372();
  _DAT_ram_a2fe = 0;
  uVar5 = 0x3d24;
  FUN_ram_3b6f();
  uVar6 = 0x3d36;
  FUN_ram_3a6a(uVar5);
  FUN_ram_3d19(uVar6);
  return;
}



/* === ram:3b92 : FUN_ram_3b92 === */

void FUN_ram_3b92(short param_1)

{
  undefined2 uVar1;
  undefined2 auStack_6 [3];
  
  param_1 = param_1 + (short)register0x44;
  *(undefined2 *)(param_1 + -2) = 0;
  *(undefined2 *)(param_1 + -4) = 0;
  *(undefined2 *)(param_1 + -6) = 0x3ba2;
  func_0x3b05((char)param_1);
  uVar1 = *(undefined2 *)(param_1 + -2);
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)(param_1 + -2) = 0x3ba7;
  thunk_FUN_ram_0190(uVar1);
}



/* === ram:3c8f : thunk_FUN_ram_3dc0 === */

void thunk_FUN_ram_3dc0(void)

{
  return;
}



/* === ram:3cb2 : FUN_ram_3cb2 === */

void FUN_ram_3cb2(undefined1 *param_1,undefined2 param_2)

{
  *param_1 = (char)((ushort)param_2 >> 8);
  param_1[-1] = (char)param_2;
  FUN_ram_3d9b();
  return;
}



/* === ram:3d19 : FUN_ram_3d19 === */

void FUN_ram_3d19(void)

{
  undefined2 in_stack_00000001;
  
  FUN_ram_3a6a(in_stack_00000001);
  return;
}



/* === ram:3d65 : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:3d9b : FUN_ram_3d9b === */

void FUN_ram_3d9b(short param_1,short param_2)

{
  undefined1 *puVar1;
  undefined1 uVar2;
  
  puVar1 = *(undefined1 **)(param_2 + param_1);
  *(undefined1 **)(param_2 + param_1) = puVar1 + 1;
  uVar2 = *(undefined1 *)(param_1 + 2);
  *puVar1 = uVar2;
  FUN_ram_3cb2();
  FUN_ram_3e3f(uVar2);
  return;
}



/* === ram:3dc0 : FUN_ram_3dc0 === */

void FUN_ram_3dc0(void)

{
  return;
}



/* === ram:3dc9 : FUN_ram_3dc9 === */

void FUN_ram_3dc9(short param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(*(undefined1 *)(param_1 + 0x26));
}



/* === ram:3e3f : FUN_ram_3e3f === */

void FUN_ram_3e3f(short param_1,byte *param_2)

{
  *(ushort *)(param_1 + 2) = *param_2 + 0x82e7;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:3eb8 : FUN_ram_3eb8 === */

void FUN_ram_3eb8(char param_1)

{
  undefined2 in_stack_00000000;
  
  FUN_ram_3f9f(param_1 + '\x01',in_stack_00000000);
  return;
}



/* === ram:3ec1 : FUN_ram_3ec1 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_3ec1(byte param_1,undefined1 *param_2,undefined2 param_3)

{
  short sVar1;
  byte bVar2;
  undefined2 uVar3;
  undefined1 uVar4;
  undefined1 uVar5;
  ushort uVar6;
  undefined1 auStack_4 [2];
  undefined2 uStack_2;
  
  if (param_1 == 2) {
    sVar1 = *(short *)(param_2 + 6);
    *(short *)(param_2 + 6) = sVar1 + -1;
    param_1 = 0;
    param_3 = 0;
    if (sVar1 != 0) {
      uVar6 = _DAT_ram_832f & 0xff;
      uStack_2 = 0x3ee2;
      FUN_ram_023f(8);
      *param_2 = (char)uVar6;
      param_2[1] = (char)(uVar6 >> 8);
                    /* WARNING: Subroutine does not return */
      uStack_2 = 0x3eeb;
      FUN_ram_0190();
    }
  }
  bVar2 = param_1 < (byte)((ushort)param_2 >> 8);
  uVar3 = 0x1104;
  uStack_2 = 0x4009;
  FUN_ram_01e5(uRam1104);
  if ((bool)(bVar2 & 1)) {
    uVar4 = (undefined1)((ushort)uVar3 >> 8);
    uVar5 = (undefined1)((ushort)param_3 >> 8);
    *(undefined1 *)CONCAT11((char)uVar3,uVar4) = uVar5;
    *(char *)CONCAT11((char)param_3,uVar4) = (char)param_3;
    *(undefined1 *)CONCAT11(uVar4,uVar4) = uVar5;
    uStack_2 = uVar3;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(auStack_4);
  }
                    /* WARNING: Subroutine does not return */
  uStack_2 = 0x400f;
  FUN_ram_0190();
}



/* === ram:3f33 : FUN_ram_3f33 === */

void FUN_ram_3f33(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:3f8e : FUN_ram_3f8e === */

void FUN_ram_3f8e(undefined1 *param_1,undefined2 param_2,undefined1 param_3)

{
  *(undefined1 *)CONCAT11((char)((ushort)param_2 >> 8),(char)param_1) = param_3;
  DAT_ram_9fa6 = 0;
  FUN_ram_40e4(*param_1);
  return;
}



/* === ram:3f9f : FUN_ram_3f9f === */

void FUN_ram_3f9f(char *param_1,undefined2 param_2)

{
  char cVar1;
  undefined2 uVar2;
  undefined2 uVar3;
  
  uVar2 = 2;
  func_0x6f12(2);
  if ((char)((ushort)uVar2 >> 8) == '\0' && (char)uVar2 == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0,0xe508,param_2);
  }
  uVar3 = 0;
  uVar2 = 3;
  func_0x6f12(3);
  FUN_ram_64c4(uVar3,uVar2);
  cVar1 = (char)uVar2;
  *param_1 = cVar1;
  if (cVar1 != '\r') {
    func_0x7f94(cVar1 - (char)((ushort)param_1 >> 8));
    func_0x7f94();
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  *param_1 = '\n';
  DAT_ram_9fa7 = *param_1;
  FUN_ram_40e4();
  return;
}



/* === ram:3fe8 : FUN_ram_3fe8 === */

void FUN_ram_3fe8(void)

{
  return;
}



/* === ram:40e4 : FUN_ram_40e4 === */

void FUN_ram_40e4(undefined2 param_1,undefined2 param_2)

{
  FUN_ram_6dcc(param_1);
  func_0x7f94(param_2);
  func_0x7f94();
  func_0x7f94();
  func_0x7f94();
  func_0x7f94();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:4228 : FUN_ram_4228 === */

void FUN_ram_4228(undefined2 param_1)

{
  FUN_ram_4774(param_1);
  return;
}



/* === ram:42a3 : FUN_ram_42a3 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_42a3(byte param_1,undefined2 param_2,undefined2 param_3,undefined2 param_4)

{
  undefined1 uVar1;
  char cVar2;
  short sVar3;
  
  cVar2 = (char)((ushort)param_2 >> 8);
  uVar1 = (undefined1)param_2;
  FUN_ram_023f((param_1 & (byte)((ushort)param_4 >> 8)) + cVar2,3);
  sVar3 = _DAT_ram_80bc + 1;
  FUN_ram_02cb(param_3);
  _DAT_ram_a2ea = sVar3;
  *(undefined1 *)CONCAT11(cVar2,uVar1) = 0;
  ((undefined1 *)CONCAT11(cVar2,uVar1))[1] = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:431e : FUN_ram_431e === */

void FUN_ram_431e(void)

{
  return;
}



/* === ram:4372 : FUN_ram_4372 === */

void FUN_ram_4372(short param_1,short param_2)

{
  *(short *)(param_2 + param_1) = *(short *)(param_2 + param_1) + 1;
  FUN_ram_445b();
  return;
}



/* === ram:439a : FUN_ram_439a === */

void FUN_ram_439a(undefined1 *param_1)

{
  undefined2 in_stack_00000000;
  
  *param_1 = 0x45;
  FUN_ram_6286(param_1);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(in_stack_00000000);
}



/* === ram:4414 : FUN_ram_4414 === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_4414(short param_1,undefined1 param_2)

{
  undefined2 uVar1;
  byte in_F;
  undefined2 in_stack_00000000;
  
  if ((in_F & 1) != 0) {
    FUN_ram_3cb2(0x20);
    *(short *)(param_1 + 4) = *(short *)(param_1 + 4) + 1;
    FUN_ram_450a();
    return;
  }
  if ((bool)(in_F & 1)) {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  FUN_ram_6286(0x4702,_DAT_ram_80ca);
  FUN_ram_5b08(in_stack_00000000);
  FUN_ram_3cb2(0x8082);
  FUN_ram_6286(10,0x4718,_DAT_ram_8098,_DAT_ram_809a,_DAT_ram_809c,_DAT_ram_809e,_DAT_ram_80a0);
  func_0x3b05(&param_2,0xb);
  uVar1 = _DAT_ram_80a8;
  FUN_ram_6286(0,0x474f);
  func_0x3b05(uVar1,0xc);
  FUN_ram_6286(0,0x4761);
  return;
}



/* === ram:4429 : FUN_ram_4429 === */

void FUN_ram_4429(undefined1 *param_1,undefined1 param_2)

{
  *param_1 = param_2;
  FUN_ram_450a();
  return;
}



/* === ram:445b : FUN_ram_445b === */

/* WARNING: Instruction at (ram,0x4598) overlaps instruction at (ram,0x4596)
    */
/* WARNING: Removing unreachable block (ram,0x447e) */
/* WARNING: Removing unreachable block (ram,0x458a) */
/* WARNING: Removing unreachable block (ram,0x4484) */
/* WARNING: Removing unreachable block (ram,0x4472) */
/* WARNING: Removing unreachable block (ram,0x457e) */
/* WARNING: Removing unreachable block (ram,0x4478) */
/* WARNING: Removing unreachable block (ram,0x4598) */
/* WARNING: Removing unreachable block (ram,0x448a) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_445b(byte param_1)

{
  undefined1 uVar1;
  undefined2 uVar2;
  ushort uVar3;
  
  uVar3 = (ushort)(param_1 & 7);
  if ((param_1 & 7) == 1) {
    uVar1 = 0;
  }
  else {
    *(undefined2 *)(uVar3 - 2) = 0;
    *(undefined2 *)(uVar3 - 4) = 0xb;
    *(undefined2 *)(uVar3 - 6) = 0x457f;
    func_0x3b05(uVar3);
    uVar2 = *(undefined2 *)(uVar3 - 2);
    *(undefined2 *)(uVar3 - 2) = _DAT_ram_80a8;
    *(undefined2 *)(uVar3 - 4) = 0x474f;
    *(undefined2 *)(uVar3 - 6) = 0x458c;
    uVar1 = FUN_ram_6286(uVar2);
    register0x44 = (BADSPACEBASE *)(uVar3 - 2);
  }
  uVar2 = *(undefined2 *)register0x44;
  *(undefined2 *)register0x44 = 0;
  *(undefined2 *)((short)register0x44 + -2) = 0xc;
  *(undefined2 *)((short)register0x44 + -4) = 0x4599;
  func_0x3b05(uVar1,uVar2);
  uVar2 = *(undefined2 *)register0x44;
  *(undefined2 *)register0x44 = _DAT_ram_80aa;
  *(undefined2 *)((short)register0x44 + -2) = 0x4761;
  *(undefined2 *)((short)register0x44 + -4) = 0x45a6;
  FUN_ram_6286(uVar2);
  return;
}



/* === ram:450a : FUN_ram_450a === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_450a(void)

{
  undefined1 uStack0000;
  undefined2 uVar1;
  undefined2 uVar2;
  undefined2 uVar3;
  
  uVar3 = _DAT_ram_80a6;
  FUN_ram_6286(0x46e8,_DAT_ram_80a4);
  uVar1 = 0x808d;
  FUN_ram_5b08(uVar3);
  uVar2 = 10;
  FUN_ram_3cb2(uVar1);
  uVar3 = _DAT_ram_80da;
  FUN_ram_6286(uVar2,0x4702,_DAT_ram_80ca);
  uVar1 = 0x8082;
  FUN_ram_5b08(uVar3);
  uVar3 = 10;
  FUN_ram_3cb2(uVar1);
  FUN_ram_6286(uVar3,0x4718,_DAT_ram_8098,_DAT_ram_809a,_DAT_ram_809c,_DAT_ram_809e,_DAT_ram_80a0,
               _DAT_ram_80a2);
  uVar1 = 0;
  func_0x3b05(&uStack0000,0xb);
  uVar3 = _DAT_ram_80a8;
  FUN_ram_6286(uVar1,0x474f);
  uVar1 = 0;
  func_0x3b05(uVar3,0xc);
  FUN_ram_6286(uVar1,0x4761,_DAT_ram_80aa);
  return;
}



/* === ram:4774 : FUN_ram_4774 === */

/* WARNING: Instruction at (ram,0x4792) overlaps instruction at (ram,0x4791)
    */

void FUN_ram_4774(short *param_1,undefined2 *param_2)

{
  byte bVar1;
  short sVar2;
  
  FUN_ram_3a6a(*param_2);
  bVar1 = (short *)0xfffd < param_1;
  param_1[1] = param_1[1] + 1;
  sVar2 = 4;
  FUN_ram_0205();
  if ((bVar1 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(sVar2 + 2);
  }
  do {
    param_1[2] = param_1[2] + 1;
    *(undefined1 *)(param_1 + 1) = 0;
    *(undefined1 *)((short)param_1 + 3) = 0;
    *param_1 = *param_1 + 1;
    param_1 = (short *)CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1);
  } while( true );
}



/* === ram:4c59 : FUN_ram_4c59 === */

void FUN_ram_4c59(undefined1 *param_1,undefined2 param_2)

{
  *param_1 = (char)param_2;
  param_1[1] = (char)((ushort)param_2 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:4e0b : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:598b : FUN_ram_598b === */

void FUN_ram_598b(undefined1 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  *(short *)(param_1 + 8) = *(short *)(param_1 + 8) + 9;
  *(short *)(param_1 + 10) = *(short *)(param_1 + 10) + 9;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:59cf : FUN_ram_59cf === */

void FUN_ram_59cf(undefined1 *param_1,short param_2)

{
  short sVar1;
  short *psVar2;
  
  sVar1 = *(short *)(param_1 + 8);
  psVar2 = (short *)(param_1 + 8);
  *psVar2 = sVar1 + -1;
  FUN_ram_5e6f(sVar1,param_2 + 1);
  *param_1 = (char)psVar2;
  param_1[1] = (char)((ushort)psVar2 >> 8);
  *(short *)(param_1 + 2) = *(short *)(param_1 + 2) + -1;
  FUN_ram_5ab6();
  return;
}



/* === ram:59ea : FUN_ram_59ea === */

void FUN_ram_59ea(short param_1)

{
  *(short *)(param_1 + 2) = *(short *)(param_1 + 2) + -1;
  FUN_ram_5ab6();
  return;
}



/* === ram:5a3c : FUN_ram_5a3c === */

void FUN_ram_5a3c(undefined1 param_1,undefined2 param_2)

{
  FUN_ram_3cb2(param_1,param_2);
  FUN_ram_5b52(0x30);
  return;
}



/* === ram:5ab6 : FUN_ram_5ab6 === */

void FUN_ram_5ab6(undefined1 *param_1,short *param_2,undefined1 param_3)

{
  short sVar1;
  short *psVar2;
  
  sVar1 = CONCAT11(*(undefined1 *)((short)param_2 + 1),param_3);
  *param_2 = sVar1 + -1;
  FUN_ram_5ec0(sVar1);
  *param_1 = (char)param_2;
  param_1[1] = (char)((ushort)param_2 >> 8);
  sVar1 = *(short *)(param_1 + 2) + -1;
  psVar2 = (short *)(param_1 + 2);
  *psVar2 = sVar1;
  func_0x02fa();
  *(undefined1 **)(param_1 + 6) = (undefined1 *)((short)psVar2 + sVar1);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:5b08 : FUN_ram_5b08 === */

void FUN_ram_5b08(undefined2 param_1)

{
  undefined1 *in_stack_00000000;
  
  *in_stack_00000000 = (char)param_1;
  in_stack_00000000[1] = (char)((ushort)param_1 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:5b52 : FUN_ram_5b52 === */

byte FUN_ram_5b52(byte param_1,byte param_2)

{
  return param_1 & param_2;
}



/* === ram:5b5b : FUN_ram_5b5b === */

void FUN_ram_5b5b(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1,&param_1);
}



/* === ram:5be8 : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:5d4c : FUN_ram_5d4c === */

void FUN_ram_5d4c(undefined1 param_1,undefined2 param_2,undefined2 param_3,undefined1 *param_4)

{
  byte in_F;
  byte bVar1;
  undefined1 uVar2;
  undefined1 uVar3;
  undefined1 uVar5;
  ushort uVar4;
  undefined1 uVar6;
  undefined1 *puVar7;
  undefined2 uVar8;
  ushort in_stack_00000000;
  undefined2 uStack_2;
  
  uVar3 = (undefined1)param_3;
  *(undefined1 *)CONCAT11(uVar3,param_1) = param_1;
  uVar6 = *(undefined1 *)CONCAT11(uVar3,param_1);
  *(undefined1 *)CONCAT11(uVar3,uVar6) = uVar3;
  uVar5 = (undefined1)((ushort)param_3 >> 8);
  *(undefined1 *)CONCAT11(uVar6,uVar6) = uVar5;
  if ((in_F >> 6 & 1) == 0) {
    return;
  }
  if ((in_F >> 6 & 1) == 0) {
    return;
  }
  uVar2 = (undefined1)param_2;
  *(undefined1 *)CONCAT11(uVar6,uVar2) = uVar3;
  uStack_2 = CONCAT11(uVar6,uVar3);
  if ((in_F >> 6 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  puVar7 = (undefined1 *)CONCAT11(uVar2,uVar3);
  if ((in_F >> 6 & 1) == 0) {
    uStack_2 = CONCAT11(uVar2,*puVar7);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(0xe504);
  }
  *puVar7 = uVar5;
  *puVar7 = uVar5;
  *(undefined1 *)CONCAT11(uVar2,param_1) = uVar5;
  if ((in_F >> 6 & 1) != 0) {
    uStack_2 = param_2;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(&uStack_2);
  }
  puVar7 = (undefined1 *)CONCAT11(uVar2,param_1) + -0x30;
  uVar4 = in_stack_00000000 & 0xff;
  func_0x02fa();
  bVar1 = CARRY2((ushort)puVar7,uVar4);
  *param_4 = (char)(puVar7 + uVar4);
  uVar4 = (ushort)(puVar7 + uVar4) & 0xff;
  uVar8 = 0x30;
  FUN_ram_01fa(0x30);
  if ((bVar1 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_02cb(uVar4,uVar8);
  return;
}



/* === ram:5e6f : FUN_ram_5e6f === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_5e6f(byte param_1,short param_2,byte param_3,undefined2 param_4)

{
  byte bVar1;
  undefined1 uVar2;
  short sVar3;
  byte *in_stack_00000000;
  
  *in_stack_00000000 = param_3 | (byte)param_4;
  in_stack_00000000[1] = param_1 | (byte)((ushort)param_4 >> 8);
  uVar2 = DAT_ram_9fa0;
  bVar1 = DAT_ram_9f9f;
  sVar3 = CONCAT11(DAT_ram_9fa0,DAT_ram_9f9f);
  *(byte *)(param_2 + 2) = DAT_ram_9f9f;
  *(undefined1 *)(param_2 + 3) = uVar2;
  *(byte *)(sVar3 + 2) = bVar1 | 1;
  *(undefined1 *)(sVar3 + 3) = uVar2;
  _DAT_ram_9fa1 = CONCAT11(DAT_ram_9fa0,DAT_ram_9f9f) + 2;
  _DAT_ram_9fa3 = _DAT_ram_9fa1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:5ec0 : FUN_ram_5ec0 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_5ec0(short param_1,short param_2,undefined2 param_3)

{
  undefined2 uVar1;
  
  *(undefined1 *)(param_2 + param_1) = (char)param_3;
  ((undefined1 *)(param_2 + param_1))[1] = (char)((ushort)param_3 >> 8);
  uVar1 = _DAT_ram_9fa1;
  *(char *)(param_1 + 0xc) = (char)_DAT_ram_9fa1;
  *(char *)(param_1 + 0xd) = (char)((ushort)uVar1 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:6140 : FUN_ram_6140 === */

void FUN_ram_6140(short param_1)

{
  byte bVar1;
  char cVar2;
  ushort uVar3;
  short in_stack_00000000;
  
  cVar2 = *(char *)(param_1 + in_stack_00000000);
  if (cVar2 == '\0') {
    FUN_ram_627d(0,0);
    return;
  }
  uVar3 = CONCAT11((char)((ushort)(param_1 + in_stack_00000000) >> 8),cVar2) - 3;
  bVar1 = (byte)(uVar3 >> 8) | (byte)uVar3;
  if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar1,uVar3 & 0xff);
  }
  FUN_ram_6fe0(4);
  return;
}



/* === ram:61f7 : FUN_ram_61f7 === */

void FUN_ram_61f7(char param_1)

{
  byte in_F;
  
  if ((bool)(in_F >> 7)) {
    param_1 = func_0xc362();
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1 + -0x62);
}



/* === ram:627d : FUN_ram_627d === */

void FUN_ram_627d(short param_1,short param_2)

{
  undefined1 *puVar1;
  char *pcVar2;
  short sVar3;
  byte in_F;
  byte extraout_F;
  short *psVar4;
  
  if (!(bool)(in_F >> 6 & 1)) {
    puVar1 = *(undefined1 **)(*(short *)(param_1 + 6) + 4);
    *(undefined1 **)(*(short *)(param_1 + 6) + 4) = puVar1 + 1;
    *puVar1 = *(undefined1 *)(param_1 + 4);
    return;
  }
  psVar4 = (short *)(param_2 + param_1);
  pcVar2 = (char *)*psVar4;
  *psVar4 = (short)(pcVar2 + 1);
  if (*pcVar2 == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(psVar4);
  }
  FUN_ram_01e5(0x400);
  if ((extraout_F >> 6 & 1) == 0) {
    sVar3 = *(short *)(param_1 + 6);
    *(undefined1 *)(sVar3 + 2) = 0;
    *(undefined1 *)(sVar3 + 3) = 4;
    *(short *)(*(short *)(param_1 + 6) + 4) = *(short *)(param_1 + 6) + 6;
    FUN_ram_66d1(0);
    return;
  }
  FUN_ram_66d1(0);
  return;
}



/* === ram:6286 : FUN_ram_6286 === */

void FUN_ram_6286(undefined1 param_1,short param_2,undefined1 *param_3)

{
  undefined1 *puVar1;
  
  puVar1 = *(undefined1 **)(CONCAT11(*param_3,param_1) + 4);
  *(undefined1 **)(CONCAT11(*param_3,param_1) + 4) = puVar1 + 1;
  *puVar1 = *(undefined1 *)(param_2 + 4);
  return;
}



/* === ram:62ab : FUN_ram_62ab === */

void FUN_ram_62ab(undefined1 param_1,short param_2,short param_3,undefined2 param_4)

{
  byte in_F;
  byte bVar1;
  undefined2 uVar2;
  
  bVar1 = in_F & 0xec;
  uVar2 = **(undefined2 **)(param_2 + 6);
  FUN_ram_6dcc(uVar2,CONCAT11(*(undefined1 *)(param_3 + 1),param_1) + 6);
  FUN_ram_01e5(8);
  if ((bool)(bVar1 >> 6 & 1)) {
    FUN_ram_6a2e();
    bVar1 = (byte)((ushort)uVar2 >> 8) | (byte)uVar2;
    if (bVar1 != 0) {
      param_4 = 0xffe0;
    }
    FUN_ram_66d1(bVar1,0xffff,param_4);
    return;
  }
  return;
}



/* === ram:62c4 : FUN_ram_62c4 === */

void FUN_ram_62c4(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  byte in_F;
  undefined2 uStack0002;
  
  uStack0002 = 0x62cc;
  FUN_ram_01e5(8);
  if ((bool)(in_F >> 6 & 1)) {
    uStack0002 = 0x63d7;
    FUN_ram_6a2e();
    bVar1 = (byte)((ushort)param_1 >> 8) | (byte)param_1;
    if (bVar1 != 0) {
      param_2 = 0xffe0;
    }
    FUN_ram_66d1(bVar1,0xffff,param_2);
    return;
  }
  return;
}



/* === ram:630c : FUN_ram_630c === */

void FUN_ram_630c(short param_1,short param_2,short param_3)

{
  undefined1 *puVar1;
  
  puVar1 = *(undefined1 **)(param_2 + param_3);
  *(undefined1 **)(param_2 + param_3) = puVar1 + 1;
  *puVar1 = *(undefined1 *)(param_1 + 4);
  return;
}



/* === ram:64c4 : FUN_ram_64c4 === */

void FUN_ram_64c4(short param_1)

{
  undefined2 in_stack_00000000;
  
  FUN_ram_6dcc(**(undefined2 **)(param_1 + 6),*(short *)(param_1 + 6) + 6);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(in_stack_00000000);
}



/* === ram:66d1 : FUN_ram_66d1 === */

/* WARNING: Instruction at (ram,0x6cfc) overlaps instruction at (ram,0x6cfb)
    */
/* WARNING: Removing unreachable block (ram,0x6c0a) */
/* WARNING: Removing unreachable block (ram,0x6cd6) */
/* WARNING: Removing unreachable block (ram,0x6c0f) */
/* WARNING: Removing unreachable block (ram,0x6c24) */
/* WARNING: Removing unreachable block (ram,0x6d19) */
/* WARNING: Removing unreachable block (ram,0x6bd3) */
/* WARNING: Removing unreachable block (ram,0x6bed) */
/* WARNING: Removing unreachable block (ram,0x6bf2) */
/* WARNING: Removing unreachable block (ram,0x6cfc) */
/* WARNING: Removing unreachable block (ram,0x6c05) */
/* WARNING: Removing unreachable block (ram,0x6d11) */
/* WARNING: Removing unreachable block (ram,0x6d1a) */
/* WARNING: Removing unreachable block (ram,0x6d24) */
/* WARNING: Removing unreachable block (ram,0x68de) */
/* WARNING: Removing unreachable block (ram,0x6953) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_66d1(byte *param_1,short param_2,undefined2 param_3,byte *param_4)

{
  undefined1 *puVar1;
  byte extraout_F;
  byte extraout_F_00;
  byte extraout_F_01;
  byte extraout_F_02;
  char cVar2;
  byte *pbVar3;
  short sVar4;
  byte bVar5;
  undefined1 *puVar6;
  byte *pbVar7;
  byte *in_stack_00000000;
  
  *(char *)(param_2 + 1) = (char)((ushort)param_3 >> 8);
  param_1[0x8f] = param_1[0x8f] + 1;
  pbVar3 = *(byte **)(param_1 + 4);
  *(byte **)(param_1 + 4) = pbVar3 + 1;
  *param_1 = *pbVar3;
  bVar5 = *param_1;
  pbVar3 = (byte *)(ushort)bVar5;
  pbVar7 = pbVar3;
  FUN_ram_64c4();
  cVar2 = (char)pbVar7;
  if (cVar2 == 'D') {
    puVar6 = *(undefined1 **)(param_1 + 0x8d);
    *(undefined1 **)(param_1 + 0x8d) = puVar6 + 1;
    puVar1 = *(undefined1 **)(param_1 + 2);
    *(undefined1 **)(param_1 + 2) = puVar1 + 1;
    *puVar6 = *puVar1;
    *(short *)(param_1 + 0x93) = *(short *)(param_1 + 0x93) + -1;
    *(short *)(param_1 + 0x91) = *(short *)(param_1 + 0x91) + -1;
    pbVar3 = *(byte **)(param_1 + 0x9b);
    *(byte **)(param_1 + 0x9b) = pbVar3 + 1;
    *pbVar3 = *param_1;
    pbVar3 = pbVar3 + (short)param_1;
    puVar6 = *(undefined1 **)pbVar3;
    *(undefined1 **)pbVar3 = puVar6 + 1;
    *puVar6 = 0x2d;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(pbVar3);
  }
  if (cVar2 == 'U') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190((ushort)param_1 & 0xff);
  }
  if (cVar2 == 'X') {
    pbVar3 = pbVar7 + (short)param_1;
    param_1[0x8d] = (byte)pbVar3;
    pbVar7 = param_1 + 0x8e;
  }
  else if (cVar2 != 'O') {
    if (cVar2 != 'C') {
      if (cVar2 == 'S') {
        pbVar7 = (byte *)(((ushort)pbVar7 & 0xff) + 1);
      }
      bVar5 = FUN_ram_6af8((ushort)pbVar7 & 0xff);
      return bVar5;
    }
    *in_stack_00000000 = bVar5;
    if (bVar5 == 0) {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190();
    }
    pbVar3 = (byte *)(*(short *)(param_1 + 0x9b) + 1);
    pbVar7 = param_1 + 0x9b;
    *(byte **)pbVar7 = pbVar3;
    goto LAB_ram_6996;
  }
  *pbVar7 = (byte)((ushort)pbVar3 >> 8);
  bVar5 = param_1[0x90];
  pbVar7 = (byte *)CONCAT11((char)((ushort)(param_1 + 0x90) >> 8),bVar5);
  if (bVar5 == 0) {
    pbVar3 = *(byte **)(param_1 + 0x91);
    *(byte **)(param_1 + 0x91) = pbVar3 + -1;
    pbVar7 = (byte *)0x0;
    FUN_ram_0205(0,pbVar3);
    if ((extraout_F & 1) != 0) {
      puVar6 = *(undefined1 **)(param_1 + 0x9b);
      *(undefined1 **)(param_1 + 0x9b) = puVar6 + 1;
      *puVar6 = 0x20;
      FUN_ram_01ec(param_1 + 0x9b);
      if ((extraout_F_00 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
        FUN_ram_0190();
      }
      *puVar6 = 0x4d;
      cVar2 = FUN_ram_0005();
      if (cVar2 == -1) {
        bVar5 = 0xff;
      }
      else {
        DAT_ram_6d35 = DAT_ram_6d35 | 1;
        DAT_ram_6d3b = 0;
        param_4 = &DAT_ram_6d3c;
        FUN_ram_0337();
        bVar5 = FUN_ram_0129(DAT_ram_059f);
        if ((!(bool)(extraout_F_01 & 1)) && (bVar5 = *param_4 & 4, (*param_4 & 4) != 0)) {
          FUN_ram_012c(DAT_ram_059f);
          DAT_ram_0575 = (undefined1 *)0x0;
          DAT_ram_0573 = param_4;
          FUN_ram_0005(0x80);
          puVar6 = DAT_ram_05a1;
          if ((char)((ushort)DAT_ram_05a3 >> 8) == '\0' && (char)DAT_ram_05a3 == '\0') {
            return 0;
          }
          DAT_ram_0080 = *DAT_ram_05a1;
          sVar4 = 0x81;
          DAT_ram_05a1[1] = (char)(DAT_ram_05a1 + 1);
          DAT_ram_0575 = puVar6 + 2;
          bVar5 = FUN_ram_6fd9();
          if ((extraout_F_02 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
            thunk_FUN_ram_0190();
          }
          _DAT_ram_05b3 = sVar4 + 1;
          return bVar5;
        }
      }
      if ((char)bVar5 < '\0') {
        return ~(byte)param_4;
      }
      return bVar5;
    }
  }
LAB_ram_6996:
  bVar5 = FUN_ram_6aa2(pbVar7 + (short)pbVar3);
  return bVar5;
}



/* === ram:6a2e : FUN_ram_6a2e === */

void FUN_ram_6a2e(void)

{
  return;
}



/* === ram:6aa2 : FUN_ram_6aa2 === */

/* WARNING: Instruction at (ram,0x6cfc) overlaps instruction at (ram,0x6cfb)
    */
/* WARNING: Removing unreachable block (ram,0x6c0a) */
/* WARNING: Removing unreachable block (ram,0x6cd6) */
/* WARNING: Removing unreachable block (ram,0x6c0f) */
/* WARNING: Removing unreachable block (ram,0x6c24) */
/* WARNING: Removing unreachable block (ram,0x6d19) */
/* WARNING: Removing unreachable block (ram,0x6bd3) */
/* WARNING: Removing unreachable block (ram,0x6bed) */
/* WARNING: Removing unreachable block (ram,0x6bf2) */
/* WARNING: Removing unreachable block (ram,0x6cfc) */
/* WARNING: Removing unreachable block (ram,0x6c05) */
/* WARNING: Removing unreachable block (ram,0x6d11) */
/* WARNING: Removing unreachable block (ram,0x6d1a) */
/* WARNING: Removing unreachable block (ram,0x6d24) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_6aa2(byte *param_1)

{
  undefined1 *puVar1;
  byte extraout_F;
  byte extraout_F_00;
  char cVar2;
  byte bVar3;
  undefined1 uVar4;
  undefined2 uVar5;
  short sVar6;
  short in_stack_00000000;
  undefined2 uStack0002;
  
  *(undefined1 *)(in_stack_00000000 + 9) = 0x43;
  *(undefined1 *)(in_stack_00000000 + 10) = 0x4f;
  *(undefined1 *)(in_stack_00000000 + 0xb) = 0x4d;
  uVar4 = 0xf;
  uStack0002 = 0x6ab5;
  cVar2 = FUN_ram_0005();
  if (cVar2 == -1) {
    bVar3 = 0xff;
  }
  else {
    DAT_ram_6d35 = DAT_ram_6d35 | 1;
    DAT_ram_6d3b = 0;
    param_1 = &DAT_ram_6d3c;
    uVar5 = CONCAT11(5,uVar4);
    uStack0002 = 0x6ccf;
    FUN_ram_0337();
    uStack0002 = 0x6cd5;
    bVar3 = FUN_ram_0129(DAT_ram_059f);
    if ((!(bool)(extraout_F & 1)) && (bVar3 = *param_1 & 4, (*param_1 & 4) != 0)) {
      uStack0002 = uVar5;
      FUN_ram_012c(DAT_ram_059f);
      DAT_ram_0575 = (undefined1 *)0x0;
      DAT_ram_0573 = param_1;
      FUN_ram_0005(0x80);
      puVar1 = DAT_ram_05a1;
      if ((char)((ushort)DAT_ram_05a3 >> 8) == '\0' && (char)DAT_ram_05a3 == '\0') {
        return 0;
      }
      DAT_ram_0080 = *DAT_ram_05a1;
      sVar6 = 0x81;
      DAT_ram_05a1[1] = (char)(DAT_ram_05a1 + 1);
      DAT_ram_0575 = puVar1 + 2;
      bVar3 = FUN_ram_6fd9();
      if ((extraout_F_00 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
        thunk_FUN_ram_0190();
      }
      _DAT_ram_05b3 = sVar6 + 1;
      return bVar3;
    }
  }
  if ((char)bVar3 < '\0') {
    return ~(byte)param_1;
  }
  return bVar3;
}



/* === ram:6af8 : FUN_ram_6af8 === */

/* WARNING: Instruction at (ram,0x6cfc) overlaps instruction at (ram,0x6cfb)
    */
/* WARNING: Removing unreachable block (ram,0x6d11) */
/* WARNING: Removing unreachable block (ram,0x011d) */
/* WARNING: Removing unreachable block (ram,0x011e) */
/* WARNING: Removing unreachable block (ram,0x011f) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

char FUN_ram_6af8(undefined2 param_1,short *param_2,undefined1 *param_3)

{
  char cVar1;
  byte extraout_F;
  undefined1 uVar2;
  char cVar3;
  char cVar5;
  undefined2 uVar4;
  short sVar6;
  undefined1 *puVar7;
  short **ppsVar8;
  short **ppsVar9;
  short *psStack_2;
  
  ppsVar9 = &psStack_2;
  sVar6 = *param_2;
  psStack_2 = param_2;
  if ((char)*param_2 == '\0' && *(char *)((short)param_2 + 1) == '\0') {
LAB_ram_6c1f:
    cVar3 = (char)((ushort)param_1 >> 8) + -1;
    if (cVar3 == '\0') {
      return DAT_ram_6d39;
    }
    *(undefined2 *)((short)ppsVar9 + -2) = 0x6d1d;
    cVar3 = FUN_ram_0005(CONCAT11(cVar3,(char)param_1),sVar6,
                         CONCAT11((char)((ushort)(param_3 + 1) >> 8) + -1,(char)(param_3 + 1)));
    if (cVar3 == '\0') {
      DAT_ram_0575 = (undefined1 *)((short)DAT_ram_0575 + 1);
      DAT_ram_05a3 = DAT_ram_05a3 + -1;
      return '\0';
    }
  }
  else {
    sVar6 = sVar6 + -1;
                    /* WARNING: Call to offcut address within same function */
    uVar2 = func_0x6c16(0x20);
    ppsVar8 = &psStack_2;
    *param_3 = uVar2;
    param_3 = param_3 + 1;
    cVar5 = (char)((ushort)param_1 >> 8);
    cVar3 = (char)param_1;
    if (*(char *)(sVar6 + 1) == '\0') {
      *(char *)(psStack_2 + 1) = DAT_ram_6d35 + (char)psStack_2[1];
      puVar7 = (undefined1 *)((short)psStack_2 + 3);
      param_3 = (undefined1 *)0x6d36;
      cVar1 = cVar3 + -1;
      cVar3 = '\0';
      ppsVar8 = (short **)register0x44;
      if (cVar1 == '\0') goto code_r0x6c05;
      *puVar7 = (char)puVar7;
      uVar4 = CONCAT11(cVar5,cVar1);
    }
    else {
      uVar2 = (undefined1)((ushort)(sVar6 + 1) >> 8);
      puVar7 = (undefined1 *)CONCAT11(uVar2,uVar2);
code_r0x6c05:
      register0x44 = (BADSPACEBASE *)((short)ppsVar8 + 2);
      uVar4 = CONCAT11(cVar5,cVar3);
      if (cVar5 == '\0') {
        register0x44 = (BADSPACEBASE *)((short)ppsVar8 + 4);
        ppsVar9 = (short **)((short)ppsVar8 + 4);
        if ((char)*(undefined2 *)((short)ppsVar8 + 2) == '\x01') {
          param_1 = 0x600;
          param_3 = &DAT_ram_6d3b;
          DAT_ram_6d35 = ~DAT_ram_6d3b + 1;
          sVar6 = 0x6d36;
          goto LAB_ram_6c1f;
        }
        param_3 = (undefined1 *)CONCAT11((char)((ushort)param_3 >> 8),(char)param_3 + -1);
        uVar4 = 0xe77e;
      }
    }
    *(undefined2 *)((short)register0x44 + -2) = uVar4;
    *(undefined2 *)((short)register0x44 + -4) = 0x6ce5;
    FUN_ram_012c(DAT_ram_059f,param_3);
    DAT_ram_0575 = (undefined1 *)0x0;
    *(undefined2 *)((short)register0x44 + -4) = 0x6cf6;
    DAT_ram_0573 = puVar7;
    FUN_ram_0005(0x80);
    puVar7 = DAT_ram_05a1;
    cVar3 = '\0';
    if ((char)((ushort)DAT_ram_05a3 >> 8) != '\0' || (char)DAT_ram_05a3 != '\0') {
      DAT_ram_0080 = *DAT_ram_05a1;
      sVar6 = 0x81;
      DAT_ram_05a1[1] = (char)(DAT_ram_05a1 + 1);
      DAT_ram_0575 = puVar7 + 2;
      puVar7 = (undefined1 *)
               ((short)register0x44 +
               CONCAT11(~(byte)((ushort)DAT_ram_0575 >> 8),~(byte)DAT_ram_0575) + -1);
      *(undefined2 *)((short)register0x44 + -4) = 0x6ecb;
      cVar3 = FUN_ram_6fd9();
      if ((extraout_F & 1) == 0) {
        *(undefined1 **)((short)register0x44 + -4) = puVar7;
        *(undefined1 **)((short)register0x44 + -6) = &DAT_ram_a2f8;
                    /* WARNING: Subroutine does not return */
        *(undefined2 *)((short)register0x44 + -8) = 0x6fdd;
        thunk_FUN_ram_0190();
      }
      _DAT_ram_05b3 = sVar6 + 1;
      return cVar3;
    }
  }
  return cVar3;
}



/* === ram:6b8d : FUN_ram_6b8d === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_6b8d(void)

{
  _DAT_ram_6d39 = DAT_ram_05a5;
  return;
}



/* === ram:6c75 : FUN_ram_6c75 === */

/* WARNING: Removing unreachable block (ram,0x6f89) */
/* WARNING: Removing unreachable block (ram,0x6f8f) */
/* WARNING: Removing unreachable block (ram,0x7090) */
/* WARNING: Removing unreachable block (ram,0x719c) */
/* WARNING: Removing unreachable block (ram,0x7099) */
/* WARNING: Removing unreachable block (ram,0x721a) */
/* WARNING: Removing unreachable block (ram,0x71b8) */
/* WARNING: Removing unreachable block (ram,0x722d) */
/* WARNING: Removing unreachable block (ram,0x722e) */
/* WARNING: Removing unreachable block (ram,0x71ba) */
/* WARNING: Removing unreachable block (ram,0x71c8) */
/* WARNING: Removing unreachable block (ram,0x71cd) */
/* WARNING: Removing unreachable block (ram,0x7239) */
/* WARNING: Removing unreachable block (ram,0x723a) */
/* WARNING: Removing unreachable block (ram,0x71d5) */
/* WARNING: Removing unreachable block (ram,0x71da) */
/* WARNING: Removing unreachable block (ram,0x724f) */
/* WARNING: Removing unreachable block (ram,0x71dc) */
/* WARNING: Removing unreachable block (ram,0x71e8) */
/* WARNING: Removing unreachable block (ram,0x7233) */
/* WARNING: Removing unreachable block (ram,0x723b) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_6c75(ushort param_1,undefined1 *param_2)

{
  byte bVar1;
  char cVar2;
  byte bVar3;
  byte bVar4;
  byte in_stack_00000000;
  
  bVar1 = (byte)param_1 & 0x48;
  if (((param_1 & 0x48) != 0) && (((bVar1 >> 3 ^ bVar1 >> 6) & 1) != 0)) {
    bVar4 = (byte)(DAT_ram_0599 >> 8);
    bVar1 = (byte)DAT_ram_0599 >> 7;
    DAT_ram_0599 = CONCAT11(bVar4 << 1,(byte)DAT_ram_0599 << 1 | bVar4 >> 7) | 0x100;
    bVar3 = (byte)(param_1 >> 8);
    bVar4 = bVar3 << 1;
    _DAT_ram_0597 = CONCAT11(bVar4 | bVar1,(byte)param_1 << 1 | bVar3 >> 7);
    return bVar4 & 0x7f | bVar1;
  }
  *param_2 = DAT_ram_0571;
  cVar2 = FUN_ram_0005();
  if (cVar2 == -1) {
    *param_2 = 0xff;
    DAT_ram_0577 = 0;
    return 0;
  }
  *(byte *)(param_1 + 0x20) = in_stack_00000000 & 0x7f;
  return in_stack_00000000 & 0x7f;
}



/* === ram:6d41 : FUN_ram_6d41 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_6d41(char param_1)

{
  short sVar1;
  byte **ppbVar2;
  byte bVar3;
  char cVar4;
  byte bVar5;
  byte bVar6;
  undefined2 uVar7;
  byte *pbVar8;
  byte bVar9;
  code *pcVar10;
  byte bVar11;
  code *pcVar12;
  undefined1 uVar14;
  byte *pbVar13;
  code **ppcVar15;
  byte **ppbVar16;
  short in_stack_00000000;
  code *pcStack_4;
  byte *local_2;
  
  uVar7 = 0xe5c5;
  local_2 = _DAT_ram_059f;
  pcStack_4 = (code *)0x6d4c;
  pbVar13 = _DAT_ram_059f;
  FUN_ram_706c(param_1 + -1);
  cVar4 = (char)((ushort)uVar7 >> 8);
  if ((char)DAT_ram_05a3 != '\0') {
    DAT_ram_0571 = ((byte)((ushort)(DAT_ram_05a1 + (short)pbVar13) >> 8) & 0x3f) << 1 |
                   (byte)(DAT_ram_05a1 + (short)pbVar13) >> 7;
    pcVar10 = FUN_ram_0005 + 7;
    local_2 = (byte *)(in_stack_00000000 + 0xc);
    if (DAT_ram_0571 == *local_2) {
      pcStack_4 = (code *)0x6e8f;
      cVar4 = FUN_ram_0005(CONCAT11(cVar4,1));
      sVar1 = FUN_ram_0005;
      if (cVar4 == '\x03') {
        *(undefined2 *)(FUN_ram_0005 + -2) = 0xf;
        FUN_ram_034b();
        *(undefined2 *)(sVar1 + -2) = 0x12;
        FUN_ram_079e();
        if (((byte)*pcVar10 & 1) == 0) {
          return 8;
        }
        *(undefined2 *)(sVar1 + -2) = 0x537;
        bVar5 = func_0x0038(8);
        return bVar5;
      }
      if (cVar4 == '\x1a') {
        return 0x1a;
      }
      if (cVar4 == '\r') {
        pcStack_4 = (code *)0x6ea9;
        bVar5 = FUN_ram_0005(2,10);
        return bVar5;
      }
      if (SBORROW1(cVar4,'\r')) {
        local_2 = (byte *)0x6faf;
        cVar4 = FUN_ram_2272(CONCAT11(0xff,cVar4));
      }
      _DAT_ram_836f = 0x72f9;
      _DAT_ram_8376 = 0x7306;
      _DAT_ram_837d = 0x730e;
      _DAT_ram_8384 = 0x73a0;
      _DAT_ram_838b = 0x73cc;
      return cVar4 + (char)pcVar10;
    }
    cVar4 = FUN_ram_0005(0xc);
    if (cVar4 != -1) {
      DAT_ram_0577 = 0;
      return 0;
    }
    bVar5 = 0xff;
    pbVar13 = local_2;
LAB_ram_011d:
    if ((char)bVar5 < '\0') {
      return ~(byte)pbVar13;
    }
    return bVar5;
  }
  pbVar8 = (byte *)CONCAT11(cVar4 + -1,(char)uVar7);
  local_2 = (byte *)0x6e5d;
  pbVar13 = DAT_ram_05a1;
  FUN_ram_0129(0);
  if (((char)DAT_ram_05a1 != '\0') && ((char)DAT_ram_05a1 != '\x01')) {
    bVar5 = (char)DAT_ram_05a1 - 2;
    if (bVar5 == 0) {
      *pbVar13 = 7;
      return DAT_ram_0571;
    }
    goto LAB_ram_011d;
  }
  bVar6 = pbVar13[0x20];
  bVar9 = pbVar13[0xc] * -0x80;
  bVar5 = bVar6 & bVar9;
  bVar3 = (bVar5 == 0) << 6 | ((char)bVar5 < '\0') << 7;
  if ((char)bVar5 < '\0') {
    return bVar5;
  }
  pcStack_4 = (code *)0x7096;
  local_2 = pbVar8;
  FUN_ram_0204(bVar5,CONCAT11(pbVar13[0xc] >> 1,bVar9 + bVar6),0);
  if ((bool)(bVar3 & 1)) {
    pcStack_4 = (code *)0x719d;
    bVar5 = func_0x0038();
    return bVar5;
  }
  pcStack_4 = (code *)0x709c;
  func_0x2c4b();
  pcStack_4 = RST0;
  FUN_ram_6d41();
  pcVar10 = pcStack_4;
  pcStack_4 = (code *)0x71fc;
  bVar5 = thunk_FUN_ram_3dc0(pcVar10);
  pcVar10 = pcStack_4;
  ppcVar15 = (code **)&local_2;
  ppbVar16 = &local_2;
  *(byte *)(ushort)bVar5 = bVar5;
  pcVar12 = (code *)(ushort)*(byte *)(ushort)bVar5;
  if ((bVar3 >> 6 & 1) == 0) {
    pcStack_4 = pcVar12;
    FUN_ram_6d41(pcVar10);
    pcVar10 = pcStack_4;
    pcStack_4 = (code *)0x739f;
    cVar4 = FUN_ram_3b6f(pcVar10);
    pcVar10 = pcStack_4;
    ppcVar15 = &pcStack_4;
    pcStack_4 = _DAT_ram_80b8;
    pcVar12 = _DAT_ram_80ba;
code_r0x722e:
    *(code **)((short)ppcVar15 + -2) = pcVar12;
    *(undefined2 *)((short)ppcVar15 + -4) = 0x7232;
    bVar5 = FUN_ram_0da4(cVar4,pcVar10);
    ppbVar2 = (byte **)ppcVar15;
  }
  else {
    if ((bVar3 >> 6 & 1) == 0) {
      cVar4 = bVar5 + bVar9;
      goto code_r0x722e;
    }
    uVar14 = *(undefined1 *)CONCAT11(bVar6,bVar9);
    *(undefined1 *)CONCAT11(uVar14,bVar9) = uVar14;
    bVar11 = (byte)((ushort)pcStack_4 >> 8);
    uVar7 = CONCAT11(bVar11,bVar6);
    pbVar13 = (byte *)CONCAT11(uVar14,bVar5);
    *pbVar13 = bVar5;
    if ((bVar3 >> 6 & 1) == 0) {
LAB_ram_71e8:
      pcStack_4 = (code *)CONCAT11((char)pbVar13,(char)((ushort)pbVar13 >> 8));
      bVar5 = FUN_ram_7404(0x199);
      return bVar5;
    }
    *(byte *)CONCAT11(bVar6,bVar5) = bVar11;
    ppbVar2 = &local_2;
    if ((bVar3 >> 6 & 1) != 0) {
      pcVar10 = (code *)CONCAT11(bVar5,*(undefined1 *)CONCAT11(bVar6,bVar6));
      if ((bVar3 >> 6 & 1) == 0) {
        if ((bool)(bVar3 >> 7)) {
          return bVar5;
        }
        bVar5 = bVar5 & bVar11;
      }
      else {
        *(byte *)CONCAT11(bVar5,bVar5) = bVar5;
        pcVar10 = (code *)CONCAT11(bVar5,*(byte *)CONCAT11(bVar5,bVar5));
        ppbVar16 = &local_2;
        if ((bVar3 >> 6 & 1) != 0) {
          if ((bVar3 >> 6 & 1) == 0) {
                    /* WARNING: Call to offcut address within same function */
            pcStack_4 = (code *)0x7252;
            func_0x6d72();
            pcStack_4 = pcVar10;
            FUN_ram_023f(_DAT_ram_80bc + 1,5);
            pcVar10 = pcStack_4;
                    /* WARNING: Subroutine does not return */
            pcStack_4 = FUN_ram_7261;
            FUN_ram_020f(pcVar10);
          }
          uVar14 = *(undefined1 *)CONCAT11(bVar6,bVar9);
          *(undefined1 *)CONCAT11(uVar14,bVar9) = uVar14;
          pbVar13 = (byte *)CONCAT11(uVar14,10);
          goto LAB_ram_71e8;
        }
      }
      goto LAB_ram_723b;
    }
  }
  ppbVar16 = ppbVar2;
  uVar7 = *ppbVar16;
  *ppbVar16 = (byte *)0x7;
  pcVar10 = (code *)&DAT_ram_a2f8;
LAB_ram_723b:
  *(code **)((short)ppbVar16 + -2) = pcVar10;
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)ppbVar16 + -4) = 0x723f;
  thunk_FUN_ram_0190(bVar5,uVar7);
}



/* === ram:6db7 : FUN_ram_6db7 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_6db7(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  undefined2 uVar2;
  undefined2 uVar3;
  undefined2 in_stack_00000000;
  short sVar4;
  
  DAT_ram_0575 = 0;
  bVar1 = (byte)((ushort)DAT_ram_05a3 >> 8) | (byte)DAT_ram_05a3;
  if (bVar1 == 0) {
    DAT_ram_0575 = 0;
    return 0;
  }
  uVar3 = DAT_ram_05a1;
  FUN_ram_0005(bVar1,CONCAT11((char)((ushort)param_1 >> 8) + -1,0x1a),param_2);
  sVar4 = DAT_ram_0573;
  bVar1 = FUN_ram_0005(0x14);
  if (bVar1 != 2) {
    if (bVar1 == 1) {
      bVar1 = *(byte *)(sVar4 + 0x20);
      if (bVar1 == 0x80) {
        *(byte *)(sVar4 + 0x20) = 0;
        return 0x80;
      }
      return bVar1;
    }
    uVar2 = CONCAT11((char)((ushort)in_stack_00000000 >> 8) + -1,(char)in_stack_00000000);
    FUN_ram_6db7(-(bVar1 == 0),uVar3);
    bVar1 = FUN_ram_0005(0x16,0x5c);
    if (bVar1 != 0xff) {
      bVar1 = FUN_ram_6f36(uVar2,_DAT_ram_059f,1);
      return bVar1;
    }
  }
  if ((char)bVar1 < '\0') {
    return ~(byte)uVar3;
  }
  return bVar1;
}



/* === ram:6dcc : FUN_ram_6dcc === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_6dcc(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  undefined1 uVar3;
  undefined2 uVar2;
  undefined2 in_stack_00000000;
  short sVar4;
  
  uVar3 = (undefined1)((ushort)param_1 >> 8);
  FUN_ram_0005();
  sVar4 = DAT_ram_0573;
  bVar1 = FUN_ram_0005(CONCAT11(uVar3,0x14));
  if (bVar1 != 2) {
    if (bVar1 == 1) {
      bVar1 = *(byte *)(sVar4 + 0x20);
      if (bVar1 == 0x80) {
        *(byte *)(sVar4 + 0x20) = 0;
        return 0x80;
      }
      return bVar1;
    }
    uVar2 = CONCAT11((char)((ushort)in_stack_00000000 >> 8) + -1,(char)in_stack_00000000);
    FUN_ram_6db7(-(bVar1 == 0),param_2);
    bVar1 = FUN_ram_0005(0x16,0x5c);
    if (bVar1 != 0xff) {
      bVar1 = FUN_ram_6f36(uVar2,_DAT_ram_059f,1);
      return bVar1;
    }
  }
  if ((char)bVar1 < '\0') {
    return ~(byte)param_2;
  }
  return bVar1;
}



/* === ram:6f36 : FUN_ram_6f36 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

char FUN_ram_6f36(char param_1,undefined1 *param_2,undefined1 *param_3,undefined1 *param_4,
                 undefined2 param_5)

{
  short sVar1;
  short sVar2;
  undefined2 in_stack_00000000;
  
  if (SCARRY1(param_1,'\x01')) {
    sVar1 = _DAT_ram_80ba;
    FUN_ram_026b(*param_2,0x4e,param_3);
    sVar1 = sVar1 + -0x67f1;
    sVar2 = _DAT_ram_80b8;
    FUN_ram_026b(3,sVar1);
    FUN_ram_59ea(sVar2 + sVar1);
    FUN_ram_023f(in_stack_00000000);
                    /* WARNING: Subroutine does not return */
    FUN_ram_020f(param_5);
  }
  do {
    *param_4 = *param_3;
    param_4 = param_4 + 1;
    param_3 = param_3 + 1;
    param_2 = param_2 + -1;
  } while (param_2 != (undefined1 *)0x0);
  return param_1 + '\x01';
}



/* === ram:6fd9 : FUN_ram_6fd9 === */

void FUN_ram_6fd9(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(param_1);
}



/* === ram:6fe0 : FUN_ram_6fe0 === */

/* WARNING: Instruction at (ram,0x72be) overlaps instruction at (ram,0x72bd)
    */
/* WARNING: Removing unreachable block (ram,0x72fd) */
/* WARNING: Removing unreachable block (ram,0x70f5) */
/* WARNING: Removing unreachable block (ram,0x7160) */
/* WARNING: Removing unreachable block (ram,0x70ec) */
/* WARNING: Removing unreachable block (ram,0x715e) */
/* WARNING: Removing unreachable block (ram,0x7162) */
/* WARNING: Removing unreachable block (ram,0x7285) */
/* WARNING: Removing unreachable block (ram,0x7182) */
/* WARNING: Removing unreachable block (ram,0x7288) */
/* WARNING: Removing unreachable block (ram,0x72f0) */
/* WARNING: Removing unreachable block (ram,0x728b) */
/* WARNING: Removing unreachable block (ram,0x7294) */
/* WARNING: Removing unreachable block (ram,0x7297) */
/* WARNING: Removing unreachable block (ram,0x72be) */
/* WARNING: Removing unreachable block (ram,0x29a2) */
/* WARNING: Removing unreachable block (ram,0x729d) */
/* WARNING: Removing unreachable block (ram,0x72c1) */
/* WARNING: Removing unreachable block (ram,0x72f8) */
/* WARNING: Removing unreachable block (ram,0x70ee) */
/* WARNING: Removing unreachable block (ram,0x711a) */
/* WARNING: Removing unreachable block (ram,0x70f8) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_6fe0(byte param_1,byte param_2)

{
  undefined2 in_stack_00000000;
  
  if ((param_1 | param_2) != 0) {
    func_0x3b05(param_1 | param_2);
                    /* WARNING: Subroutine does not return */
    thunk_FUN_ram_0190(in_stack_00000000);
  }
  return;
}



/* === ram:706c : FUN_ram_706c === */

void FUN_ram_706c(undefined1 *param_1,undefined2 param_2)

{
  *param_1 = (char)param_2;
  param_1[1] = (char)((ushort)param_2 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:7261 : FUN_ram_7261 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_7261(ushort param_1)

{
  byte bVar1;
  ushort in_stack_00000000;
  
  func_0x02fa();
  bVar1 = CARRY2(param_1,in_stack_00000000);
  _DAT_ram_80a6 = param_1 + in_stack_00000000;
  FUN_ram_0204(0);
  if ((bool)(bVar1 & 1)) {
    FUN_ram_026b(0x25);
                    /* WARNING: Subroutine does not return */
    thunk_FUN_ram_0190();
  }
  func_0x2c4b();
  return;
}



/* === ram:7404 : FUN_ram_7404 === */

void FUN_ram_7404(void)

{
  byte in_F;
  
  FUN_ram_01f9();
  if ((in_F & 1) == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:7758 : thunk_FUN_ram_77ff === */

void thunk_FUN_ram_77ff(char param_1,undefined2 param_2)

{
  byte in_F;
  
  if ((in_F & 1) != 0) {
    return;
  }
  if (param_1 == (char)((ushort)param_2 >> 8)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_7aac();
  return;
}



/* === ram:77ff : FUN_ram_77ff === */

void FUN_ram_77ff(char param_1,undefined2 param_2)

{
  byte in_F;
  
  if ((in_F & 1) != 0) {
    return;
  }
  if (param_1 == (char)((ushort)param_2 >> 8)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_7aac();
  return;
}



/* === ram:78ad : FUN_ram_78ad === */

void FUN_ram_78ad(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_4c59(0,0x7cad,0,param_1);
}



/* === ram:7a70 : FUN_ram_7a70 === */

void FUN_ram_7a70(short param_1,short *param_2)

{
  ushort in_stack_00000000;
  
  *param_2 = param_1 + (in_stack_00000000 & 0xff);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:7aac : FUN_ram_7aac === */

void FUN_ram_7aac(short param_1,short param_2)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(CONCAT11((char)((ushort)(param_1 + param_2) >> 8),*(undefined1 *)(param_1 + param_2))
              );
}



/* === ram:7e68 : FUN_ram_7e68 === */

/* WARNING: Control flow encountered bad instruction data */

void FUN_ram_7e68(void)

{
                    /* WARNING: Bad instruction - Truncating control flow here */
  halt_baddata();
}


