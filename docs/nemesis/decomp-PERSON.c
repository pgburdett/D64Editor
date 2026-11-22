/* Decompiled PERSON.COM - Ghidra Z80 decompiler */

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

void RST3(byte *param_1)

{
  byte in_F;
  
  if ((bool)(in_F & 1)) {
    return;
  }
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
  return;
}



/* === ram:001d : thunk_FUN_ram_0427 === */

void thunk_FUN_ram_0427(byte *param_1)

{
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
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

/* WARNING: Removing unreachable block (ram,0x0dc4) */
/* WARNING: Removing unreachable block (ram,0x0ed0) */
/* WARNING: Removing unreachable block (ram,0x1059) */
/* WARNING: Removing unreachable block (ram,0x0ed9) */
/* WARNING: Removing unreachable block (ram,0x0dca) */
/* WARNING: Removing unreachable block (ram,0x0eea) */
/* WARNING: Removing unreachable block (ram,0x0dd0) */
/* WARNING: Removing unreachable block (ram,0x0edc) */
/* WARNING: Removing unreachable block (ram,0x0ddc) */
/* WARNING: Removing unreachable block (ram,0x0d91) */
/* WARNING: Removing unreachable block (ram,0x0dbe) */
/* WARNING: Removing unreachable block (ram,0x0ec4) */

void RST6(undefined1 *param_1)

{
  byte bVar1;
  undefined1 uVar2;
  
  enableMaskableInterrupts();
  bVar1 = FUN_ram_0d96();
  if (bVar1 == 0x38) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(FUN_ram_0005);
  }
  uVar2 = 0;
  if ((bVar1 & 8) != 0) {
    uVar2 = FUN_ram_1182();
  }
  *param_1 = uVar2;
  param_1[1] = uVar2;
  param_1[2] = uVar2;
  return;
}



/* === ram:0066 : NMI_ISR === */

undefined1 NMI_ISR(undefined1 *param_1)

{
  return *param_1;
}



/* === ram:0126 : FUN_ram_0126 === */

void FUN_ram_0126(void)

{
  return;
}



/* === ram:012c : FUN_ram_012c === */

byte FUN_ram_012c(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  undefined2 uVar2;
  
  uVar2 = param_1;
  FUN_ram_0289(param_2);
  bVar1 = (byte)uVar2;
  func_0x026b(param_1);
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
  func_0x026b();
  if ((DAT_ram_0571 & 1) == 0) {
    return DAT_ram_0571 >> 1 | cVar1 << 7;
  }
  return ~bVar2;
}



/* === ram:014d : FUN_ram_014d === */

byte FUN_ram_014d(byte param_1)

{
  char in_F;
  
  if ((DAT_ram_0571 & 1) == 0) {
    return DAT_ram_0571 >> 1 | in_F << 7;
  }
  return ~param_1;
}



/* === ram:015c : FUN_ram_015c === */

void FUN_ram_015c(void)

{
  char in_F;
  
  if (in_F < '\0') {
    DAT_ram_0571 = DAT_ram_0571 + '\x01';
    return;
  }
  return;
}



/* === ram:0177 : FUN_ram_0177 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined1 FUN_ram_0177(char param_1,ushort param_2,short param_3)

{
  if (param_1 == '\0' && (byte)param_2 == 0) {
    return 0;
  }
  if ((param_2 & 1) != 0) {
    param_3 = param_3 * 2;
    _DAT_ram_058f = 0xcc00;
    _DAT_ram_0593 = 0x10;
  }
  _DAT_ram_0591 = 0x40;
  _DAT_ram_0595 = 0x400;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005((byte)param_2 >> 1 | (char)(param_2 >> 8) << 7,0xb,param_3);
}



/* === ram:0190 : FUN_ram_0190 === */

void FUN_ram_0190(void)

{
  return;
}



/* === ram:01a3 : FUN_ram_01a3 === */

/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x04e9) */

void FUN_ram_01a3(short param_1,short param_2,undefined1 *param_3)

{
  char cVar1;
  byte bVar2;
  short sVar3;
  byte extraout_F;
  byte bVar4;
  byte extraout_F_00;
  byte extraout_F_01;
  undefined2 in_AF;
  ushort uVar5;
  byte bVar6;
  undefined1 uVar7;
  code cVar8;
  undefined2 uVar9;
  code *pcVar10;
  undefined1 *puVar11;
  undefined2 *puVar12;
  undefined2 *puVar13;
  undefined2 uStack_2;
  
  uStack_2 = in_AF;
                    /* WARNING: Call to offcut address within same function */
  func_0x02dd();
  puVar12 = &uStack_2;
  if (!(bool)(extraout_F & 1)) {
    bVar2 = (byte)((ushort)param_3 >> 8);
    uVar5 = CONCAT11(bVar2 >> 1,bVar2) & 0xff01;
    bVar6 = (byte)((ushort)uStack_2 >> 8);
    bVar4 = (byte)uStack_2;
    cVar1 = bVar6 - 1;
    uStack_2 = CONCAT11(cVar1,bVar4 & 0x29 | (((bVar6 & 0xf) - 1 & 0x10) != 0) << 4 |
                              SBORROW1(bVar6,'\x01') << 2 | 2 | (cVar1 == '\0') << 6 |
                              (cVar1 < '\0') << 7);
    if (cVar1 != '\0') {
      sVar3 = (param_1 + 1) * 2;
      FUN_ram_02a6(CONCAT11((char)((ushort)sVar3 >> 8),(byte)sVar3 | bVar4 & 1),
                   param_2 + CONCAT11(~bVar2,~(byte)param_3) + 1,
                   CONCAT11((char)(uVar5 >> 8),(byte)param_3 >> 1 | (char)uVar5 << 7));
      return;
    }
    return;
  }
  *param_3 = 0;
  pcVar10 = RST0 + 1;
  uVar9 = 0x762;
  DAT_ram_0572 = '\0';
  puVar11 = (undefined1 *)0x6e0;
  if (DAT_ram_06df == ' ') {
    uVar7 = FUN_ram_04f1(RST0);
    puVar13 = &uStack_2;
    if ((bool)(extraout_F_00 & 1)) goto LAB_ram_04e7;
    *puVar11 = uVar7;
    pcVar10 = pcVar10 + 1;
    if ((char)((ushort)uVar9 >> 8) != '\x01') {
      func_0x0038(puVar11 + 1);
      return;
    }
LAB_ram_03de:
    cVar8 = *pcVar10;
    uVar9 = uStack_2;
    puVar13 = (undefined2 *)register0x44;
  }
  else {
    if (DAT_ram_06df == '\0') {
      return;
    }
    uStack_2 = uVar9;
    register0x44 = (BADSPACEBASE *)&uStack_2;
    if (DAT_ram_06df != '\"') goto LAB_ram_03de;
    DAT_ram_0572 = DAT_ram_06df;
    cVar8 = (code)0x22;
    puVar13 = puVar12;
  }
  *(undefined2 *)((short)puVar13 + -2) = 0x3e2;
  FUN_ram_04f1(cVar8,uVar9);
  if ((bool)(extraout_F_01 & 1)) {
    return;
  }
  *(undefined2 *)((short)puVar13 + -2) = 0x4df;
  func_0x0038(pcVar10 + 1);
  *(undefined2 *)((short)puVar13 + -2) = 0x4e0;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e1;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e2;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e3;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e4;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e5;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e6;
  func_0x0038();
  *(undefined2 *)((short)puVar13 + -2) = 0x4e7;
  func_0x0038();
LAB_ram_04e7:
  *(undefined2 *)((short)puVar13 + -2) = 0x4e8;
  func_0x0038();
  return;
}



/* === ram:01af : FUN_ram_01af === */

void FUN_ram_01af(char param_1,short param_2,byte param_3,undefined2 param_4)

{
  byte bVar1;
  undefined2 in_AF;
  ushort uVar2;
  undefined2 in_stack_00000000;
  
  bVar1 = (byte)((ushort)in_stack_00000000 >> 8);
  uVar2 = CONCAT11(bVar1 >> 1,bVar1) & 0xff01;
  if ((char)((ushort)param_4 >> 8) == '\x01') {
    return;
  }
  FUN_ram_02a6(param_1 << 1 | (byte)param_4 & 1,
               param_2 + CONCAT11(~(byte)((ushort)in_AF >> 8),~param_3) + 1,
               CONCAT11((char)(uVar2 >> 8),(byte)in_stack_00000000 >> 1 | (char)uVar2 << 7));
  return;
}



/* === ram:01e5 : FUN_ram_01e5 === */

void FUN_ram_01e5(undefined2 param_1,char param_2)

{
  if (param_2 == '\0') {
    return;
  }
  FUN_ram_02e6((byte)param_1 >> 1 | (char)((ushort)param_1 >> 8) << 7,param_2);
  return;
}



/* === ram:01eb : FUN_ram_01eb === */

void FUN_ram_01eb(byte param_1)

{
  char in_F;
  
  FUN_ram_02e6(param_1 >> 1 | in_F << 7);
  return;
}



/* === ram:01ec : FUN_ram_01ec === */

void FUN_ram_01ec(byte param_1)

{
  char in_F;
  
  FUN_ram_02e6(param_1 >> 1 | in_F << 7);
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

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_025a(short param_1,undefined2 param_2)

{
  byte in_F;
  
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
  _DAT_ram_0595 = 0x400;
  _DAT_ram_05b5 = param_2;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005(CONCAT11((char)((ushort)(param_1 + 1) >> 8),0xb));
}



/* === ram:0289 : FUN_ram_0289 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0289(void)

{
  _DAT_ram_0595 = 0x400;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005(0xb);
}



/* === ram:02a6 : FUN_ram_02a6 === */

/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_02a6(undefined2 param_1,undefined2 param_2,short param_3)

{
  byte bVar1;
  byte *pbVar2;
  
  bVar1 = (byte)((ushort)param_1 >> 8);
  if (bVar1 == 0) {
    return;
  }
  pbVar2 = (byte *)CONCAT11((char)((ushort)param_2 >> 8),0xc2);
  FUN_ram_04c6(bVar1 | *pbVar2,param_3 + 1);
  *pbVar2 = 0;
  pbVar2[0x14] = 0;
  FUN_ram_013f(pbVar2 + 0x15);
  return;
}



/* === ram:02cb : FUN_ram_02cb === */

/* WARNING: Removing unreachable block (ram,0x050d) */

void FUN_ram_02cb(byte param_1,short param_2,char *param_3,undefined1 *param_4)

{
  byte in_F;
  byte bVar1;
  undefined1 uVar2;
  
  if ((in_F >> 6 & 1) == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005((param_1 - *param_3) - (in_F & 1),CONCAT11((char)((ushort)(param_2 + 1) >> 8),1));
  }
  if ((bool)(in_F >> 6 & 1)) {
    return;
  }
  if (param_1 == 0) {
    return;
  }
  bVar1 = param_1 < 0x22;
  if (param_1 == 0x22) {
    uVar2 = 0x22;
    DAT_ram_0572 = 0x22;
  }
  else {
    uVar2 = *param_4;
  }
  FUN_ram_04f1(uVar2);
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
  func_0x0038();
  return;
}



/* === ram:02e6 : FUN_ram_02e6 === */

/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x050d) */

byte FUN_ram_02e6(undefined2 param_1,byte *param_2)

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
    bVar1 = func_0x0038(param_2 + 2);
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



/* === ram:034b : FUN_ram_034b === */

void FUN_ram_034b(short param_1,byte *param_2,byte *param_3)

{
  byte in_F;
  
  param_1 = param_1 + 1;
  FUN_ram_0472();
  if ((bool)(in_F & 1)) {
    if ((*param_3 & 1) == 0) {
      return;
    }
    func_0x0038(8);
    return;
  }
  if ((*param_2 & 4) == 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(0);
  }
  FUN_ram_0313(param_2);
  FUN_ram_051a(param_1);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005(0x10,param_3,param_2);
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



/* === ram:0427 : FUN_ram_0427 === */

void FUN_ram_0427(byte *param_1)

{
  if ((*param_1 & 1) == 0) {
    return;
  }
  func_0x0038(8);
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



/* === ram:051a : FUN_ram_051a === */

void FUN_ram_051a(void)

{
  func_0x0038();
  return;
}



/* === ram:079e : FUN_ram_079e === */

void FUN_ram_079e(undefined1 param_1,short param_2)

{
  (&DAT_ram_010f)[param_2] = 0xff;
  (&DAT_ram_0110)[param_2] = 0xff;
  (&DAT_ram_0110)[param_2] = param_1;
  (&DAT_ram_0111)[param_2] = param_1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:0d96 : FUN_ram_0d96 === */

/* WARNING: Removing unreachable block (ram,0x0dd0) */
/* WARNING: Removing unreachable block (ram,0x0dd6) */
/* WARNING: Removing unreachable block (ram,0x0ef5) */
/* WARNING: Removing unreachable block (ram,0x0dc4) */
/* WARNING: Removing unreachable block (ram,0x0ed0) */
/* WARNING: Removing unreachable block (ram,0x1059) */
/* WARNING: Removing unreachable block (ram,0x0ed9) */
/* WARNING: Removing unreachable block (ram,0x0edc) */
/* WARNING: Removing unreachable block (ram,0x0dca) */
/* WARNING: Removing unreachable block (ram,0x0eea) */
/* WARNING: Removing unreachable block (ram,0x0ef6) */
/* WARNING: Removing unreachable block (ram,0x0ddc) */

void FUN_ram_0d96(undefined1 *param_1)

{
  byte bVar1;
  undefined2 in_stack_00000000;
  
  FUN_ram_26e6(in_stack_00000000);
  bVar1 = (char)DAT_ram_0115 + 0x92;
  FUN_ram_2a8f();
  FUN_ram_015c();
  if ((bVar1 & 7) == 1) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  *(undefined1 *)((bVar1 & 7) + 1) = 0;
  *param_1 = 0;
  param_1[1] = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:1182 : FUN_ram_1182 === */

/* WARNING: Instruction at (ram,0x128f) overlaps instruction at (ram,0x128e)
    */
/* WARNING: Removing unreachable block (ram,0x128e) */
/* WARNING: Removing unreachable block (ram,0x128a) */
/* WARNING: Removing unreachable block (ram,0x127b) */
/* WARNING: Removing unreachable block (ram,0x1281) */
/* WARNING: Removing unreachable block (ram,0x1338) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0000 : 0x1293 */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_1182(short param_1)

{
  undefined1 uVar1;
  undefined1 *puVar2;
  char cVar3;
  char cVar4;
  char cVar7;
  undefined1 *puVar5;
  short sVar6;
  short in_stack_00000000;
  undefined1 *puVar8;
  undefined1 auStack_34 [48];
  undefined2 uStack_4;
  short sStack_2;
  
  FUN_ram_26e6(in_stack_00000000);
  sStack_2 = 0x118e;
  sVar6 = param_1;
  FUN_ram_2ba0();
  cVar3 = (char)sVar6;
  *(char *)(param_1 + 0x73) = cVar3;
  cVar4 = (char)((ushort)sVar6 >> 8);
  *(char *)(param_1 + 0x74) = cVar4;
  if (cVar4 != '\0' || cVar3 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  cVar7 = (char)((ushort)(param_1 + 0x74) >> 8);
  cVar4 = cVar7 + -1;
  *(char *)CONCAT11(cVar4,cVar7) = cVar3;
  uVar1 = (undefined1)param_1;
  *(char *)CONCAT11(cVar4,uVar1) = cVar3;
  if (cVar4 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  *(char *)CONCAT11(uVar1,uVar1) = cVar3;
  cVar4 = *(char *)CONCAT11(uVar1,uVar1);
  *(char *)CONCAT11(uVar1,cVar4) = cVar3;
  if (cVar4 == -1) {
    uRamff00 = 0;
    RST0 = (code)0x0;
    puVar2 = auStack_34;
    FUN_ram_1ad2();
    FUN_ram_147e();
    FUN_ram_266f(0x16);
    FUN_ram_26e6(0);
    func_0x2a48();
    puVar5 = puVar2 + 2;
    puVar8 = puVar5;
    FUN_ram_3566(0x146c,0x147b);
    FUN_ram_2a74(puVar8,puVar5);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(puVar2);
  }
  func_0x026b(-(char)((ushort)param_1 >> 8),0x1111);
  sVar6 = DAT_ram_0115 + 0x300;
  sStack_2 = DAT_ram_0115 + 0xa7;
  uStack_4 = 0x12a1;
  FUN_ram_2a00();
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(sVar6 + in_stack_00000000);
}



/* === ram:147e : FUN_ram_147e === */

void FUN_ram_147e(undefined2 param_1,undefined1 param_2)

{
  FUN_ram_1ebe(0x1a00,0,0,0xc,param_1);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(&param_2);
}



/* === ram:1ad2 : FUN_ram_1ad2 === */

void FUN_ram_1ad2(undefined1 param_1)

{
  FUN_ram_20a6(0x1e6e,0,1,0,0,0,1,0,0xd9);
  FUN_ram_20a6(&param_1,0x1e76,0xffff,0xffff,0,1,1,1,0xfffe,0xc0);
  FUN_ram_20a6(&param_1,0x1e7d,1,0xffff,0xffff,0xffff,0,0,0,0xb6);
  FUN_ram_20a6(&param_1,0x1e81,2,0xffff,0xffff,0xffff,1,0,0,200);
  FUN_ram_20a6(&param_1,0x1e8a,3,0xffff,0xfffe,1,1,0,2,0xd2);
  FUN_ram_20a6(&param_1,0x1e8f,0xffff,0,0xffff,1,0xffff,1,0xffff,0xbe);
  FUN_ram_20a6(&param_1,0x1e95,0,0xffff,0xffff,0xffff,0,0,0,0xb9);
  FUN_ram_20a6(&param_1,0x1e9c,1,0xfffe,0xffff,0xffff,1,1,2,200);
  FUN_ram_20a6(&param_1,0x1ea6,0xffff,0xfffe,0xfffe,0xffff,0,0,0xfffe,0xae);
  FUN_ram_20a6(&param_1,0x1ead,0,0,1,0xfffe,0,1,0,0xc0);
  FUN_ram_20a6(&param_1,0x1eb5,0xffff,2,0,1,0xffff,0,0xfffe,0xe1);
  *(short *)(DAT_ram_0115 + 0x738) = *(short *)(DAT_ram_0115 + 0x738) + -1;
  return;
}



/* === ram:1ebe : FUN_ram_1ebe === */

void FUN_ram_1ebe(short param_1,short param_2)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1 + param_2);
}



/* === ram:20a6 : FUN_ram_20a6 === */

void FUN_ram_20a6(short param_1,short param_2)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1 + param_2);
}



/* === ram:266f : FUN_ram_266f === */

void FUN_ram_266f(undefined2 param_1)

{
  undefined1 uVar1;
  undefined1 uVar2;
  
  uVar2 = (undefined1)((ushort)param_1 >> 8);
  uVar1 = (undefined1)param_1;
  func_0x386b();
  FUN_ram_277f(*(undefined1 *)CONCAT11(uVar2,uVar1),8);
  return;
}



/* === ram:26e6 : FUN_ram_26e6 === */

void FUN_ram_26e6(undefined2 param_1)

{
  FUN_ram_27eb(param_1);
  return;
}



/* === ram:277f : FUN_ram_277f === */

void FUN_ram_277f(byte param_1,short *param_2,byte param_3)

{
  undefined1 *puVar1;
  
  if ((param_1 | param_3) == 0) {
    return;
  }
  puVar1 = (undefined1 *)param_2[1];
  param_2[1] = (short)(puVar1 + 1);
  func_0x386b(param_1 | param_3,param_2 + 1,*puVar1);
  *param_2 = *param_2 + -1;
  FUN_ram_287a();
  return;
}



/* === ram:27eb : FUN_ram_27eb === */

/* WARNING: Instruction at (ram,0x31f5) overlaps instruction at (ram,0x31f3)
    */
/* WARNING: Removing unreachable block (ram,0x3448) */
/* WARNING: Removing unreachable block (ram,0x344b) */
/* WARNING: Removing unreachable block (ram,0x2c0a) */
/* WARNING: Removing unreachable block (ram,0x2d5e) */
/* WARNING: Removing unreachable block (ram,0x2c6e) */
/* WARNING: Removing unreachable block (ram,0x2e9b) */
/* WARNING: Removing unreachable block (ram,0x2c42) */
/* WARNING: Removing unreachable block (ram,0x2d9a) */
/* WARNING: Removing unreachable block (ram,0x2c47) */
/* WARNING: Removing unreachable block (ram,0x2da4) */
/* WARNING: Removing unreachable block (ram,0x2c4c) */
/* WARNING: Removing unreachable block (ram,0x2dac) */
/* WARNING: Removing unreachable block (ram,0x2db7) */
/* WARNING: Removing unreachable block (ram,0x2dcc) */
/* WARNING: Removing unreachable block (ram,0x2eb7) */
/* WARNING: Removing unreachable block (ram,0x2ec4) */
/* WARNING: Removing unreachable block (ram,0x326f) */
/* WARNING: Removing unreachable block (ram,0x2c51) */
/* WARNING: Removing unreachable block (ram,0x2c56) */
/* WARNING: Removing unreachable block (ram,0x2e23) */
/* WARNING: Removing unreachable block (ram,0x2f38) */
/* WARNING: Removing unreachable block (ram,0x2c5b) */
/* WARNING: Removing unreachable block (ram,0x2f3b) */
/* WARNING: Removing unreachable block (ram,0x2df2) */
/* WARNING: Removing unreachable block (ram,0x2f07) */
/* WARNING: Removing unreachable block (ram,0x301c) */
/* WARNING: Removing unreachable block (ram,0x302d) */
/* WARNING: Removing unreachable block (ram,0x3152) */
/* WARNING: Removing unreachable block (ram,0x2f0f) */
/* WARNING: Removing unreachable block (ram,0x2df9) */
/* WARNING: Removing unreachable block (ram,0x2edf) */
/* WARNING: Removing unreachable block (ram,0x2fec) */
/* WARNING: Removing unreachable block (ram,0x2ff9) */
/* WARNING: Removing unreachable block (ram,0x3103) */
/* WARNING: Removing unreachable block (ram,0x2ee9) */
/* WARNING: Removing unreachable block (ram,0x31f3) */
/* WARNING: Removing unreachable block (ram,0x2e26) */
/* WARNING: Removing unreachable block (ram,0x2f12) */
/* WARNING: Removing unreachable block (ram,0x3279) */
/* WARNING: Removing unreachable block (ram,0x2d32) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined1 FUN_ram_27eb(char *param_1,short param_2,undefined2 param_3,undefined2 param_4)

{
  short sVar1;
  uint uVar2;
  undefined2 *puVar3;
  byte bVar4;
  undefined1 uVar5;
  undefined1 *puVar6;
  char *pcVar7;
  code *pcVar8;
  char cVar9;
  code cVar10;
  short *psVar11;
  ushort uVar12;
  char *pcVar13;
  byte bVar16;
  char cVar17;
  code *pcVar14;
  code *pcVar15;
  undefined2 *puVar18;
  undefined1 *puVar19;
  undefined1 uStack0002;
  char *in_stack_00000000;
  
  psVar11 = (short *)(param_1 + param_2);
  sVar1 = *psVar11;
  *psVar11 = sVar1 + -1;
  bVar4 = (byte)((ushort)sVar1 >> 8) | (byte)sVar1;
  if (bVar4 != 0) {
    FUN_ram_0177(bVar4);
    uVar12 = (ushort)param_1 >> 8;
    FUN_ram_023f(8);
    *param_1 = (char)uVar12;
    param_1[1] = (char)(uVar12 >> 8);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  pcVar13 = (char *)*psVar11;
  *psVar11 = (short)(pcVar13 + 1);
  cVar9 = *pcVar13;
  *pcVar7 = cVar9;
  if (cVar9 != '\0') {
    if (*pcVar7 == ' ') {
      *(short *)(param_1 + 9) = *(short *)(param_1 + 9) + 1;
      uVar5 = FUN_ram_2afd();
      return uVar5;
    }
    pcVar7 = (char *)(CONCAT11((char)((ushort)pcVar13 >> 8),cVar9) + -1);
    cVar9 = *pcVar7;
    if (cVar9 == '\0') {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190();
    }
    *(undefined1 *)(CONCAT11((char)((ushort)pcVar7 >> 8),cVar9) + 1) = 0;
    pcVar7 = *(char **)(param_1 + 0x95);
    *(char **)(param_1 + 0x95) = pcVar7 + 2;
    cVar9 = pcVar7[1];
    param_1[2] = *pcVar7;
    param_1[3] = cVar9;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  pcVar7[1] = -1;
  *(short *)(param_1 + 9) = *(short *)(param_1 + 9) + 1;
  pcVar13 = *(char **)(param_1 + 9);
  *(char **)(param_1 + 9) = pcVar13 + 1;
  cVar9 = *pcVar13;
  pcVar13 = param_1 + 2;
  *pcVar13 = cVar9;
  uVar2 = (uint)in_stack_00000000 >> 0x10;
  FUN_ram_32b1(0);
  bVar16 = (byte)((ushort)pcVar13 >> 8);
  bVar4 = bVar16 | (byte)pcVar13;
  if (bVar4 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar4);
  }
  if (*pcVar13 == '%') {
    *(char **)(param_1 + 0x8d) = param_1 + 6;
    param_1[0x93] = '\x06';
    param_1[0x94] = '\0';
    param_1[0x8f] = '\0';
    param_1[0x90] = '\0';
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  puVar19 = &uStack0002;
  if (cVar9 == '*') {
    *param_1 = *param_1 + '\x01';
    pcVar13 = *(char **)(param_1 + 6);
    *(char **)(param_1 + 6) = pcVar13 + 1;
    param_1[1] = *pcVar13;
    pcVar8 = (code *)(ushort)(byte)param_1[1];
    _uStack0002 = (undefined1 *)((uint)in_stack_00000000 >> 0x10);
    pcVar15 = pcVar8;
    FUN_ram_2ab4();
    puVar19 = &uStack0002;
    puVar18 = (undefined2 *)&uStack0002;
    cVar9 = (char)pcVar15;
    if (cVar9 == 'X') {
      if (*pcVar15 != SUB41(uVar2,0)) {
        puVar6 = &uStack0002;
        bVar4 = (undefined1 *)0xfffb < &uStack0002;
        FUN_ram_01f9(0x30);
        uVar5 = FUN_ram_01af();
        puVar19 = &uStack0002;
        if ((bool)(bVar4 & 1)) {
          return uVar5;
        }
        pcVar15 = (code *)(ushort)(byte)puVar6[4];
        goto LAB_ram_31d1;
      }
      pcVar15 = FUN_ram_0005 + 1;
      puVar3 = &param_3;
    }
    else {
      puVar3 = (undefined2 *)&uStack0002;
      if (cVar9 != 'O') {
        if (cVar9 != 'D') {
          pcVar14 = pcVar15;
          if (cVar9 != 'U') {
            if (cVar9 != 'S') {
              if (cVar9 != 'C') {
                    /* WARNING: Subroutine does not return */
                FUN_ram_0190(pcVar8);
              }
              pcVar7 = (char *)(((ushort)pcVar15 & 0xff) +
                               CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1));
              *pcVar7 = *pcVar7 + -0x37;
              uVar5 = FUN_ram_34b4();
              return uVar5;
            }
            *param_1 = 'S';
            puVar18 = &param_3;
            *_uStack0002 = 0x53;
            _uStack0002[1] = (char)((ushort)pcVar15 >> 8);
            pcVar14 = (code *)(param_1 + 3);
            *pcVar14 = (code)((char)*pcVar14 + '\x01');
            pcVar8 = pcVar15;
          }
          if ((char)pcVar14 == '\0') {
            param_1[3] = param_1[3] + '\x01';
                    /* WARNING: Subroutine does not return */
            puVar18[-1] = 0x30e1;
            FUN_ram_0190(pcVar8);
          }
          bVar4 = CARRY2((ushort)pcVar14,(ushort)param_1);
          pcVar15 = pcVar14 + (short)param_1;
          *puVar18 = 0x31f5;
          FUN_ram_01ec((char)pcVar14);
          if ((bVar4 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
            *puVar18 = 0x31fb;
            FUN_ram_0190();
          }
          if ((char)((ushort)pcVar15 >> 8) != '\0' || (char)pcVar15 != '\0') {
                    /* WARNING: Subroutine does not return */
            *puVar18 = 0x3338;
            FUN_ram_0190();
          }
          *pcVar15 = (code)0x22;
          _DAT_ram_36d0 = pcVar15;
          _DAT_ram_36d2 = pcVar15;
          return 0x36;
        }
        goto LAB_ram_31d1;
      }
    }
    puVar19 = (undefined1 *)puVar3;
    pcVar15 = pcVar15 + (short)param_1;
    *(short *)pcVar15 = *(short *)pcVar15 + 1;
  }
  else {
    cVar17 = *(char *)(CONCAT11(bVar16,*pcVar13) + 1);
    param_1[4] = cVar9;
    param_1[5] = cVar17;
    pcVar15 = *(code **)(param_1 + 0x12);
    *(code **)(param_1 + 0x12) = pcVar15 + 1;
    cVar10 = *pcVar15;
    pcVar15 = (code *)(param_1 + 1);
    *pcVar15 = cVar10;
    if (cVar10 != (code)0x0) {
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(cVar10);
    }
  }
LAB_ram_31d1:
  *(undefined2 *)(puVar19 + -2) = 0x31d9;
  FUN_ram_0204((ushort)pcVar15 & 0xff,0x39);
  *(undefined2 *)(puVar19 + -2) = 0x31dc;
  uVar5 = FUN_ram_01af();
  return uVar5;
}



/* === ram:287a : FUN_ram_287a === */

void FUN_ram_287a(byte param_1,byte param_2)

{
  byte bVar1;
  char cVar2;
  short sVar3;
  ushort uVar4;
  short sVar5;
  ushort uVar6;
  
  sVar3 = -0x2efa;
  bVar1 = 0;
  uVar4 = (ushort)(param_1 ^ param_2);
  uVar6 = uVar4;
  cVar2 = func_0x386b();
  sVar5 = uVar4 + sVar3;
  FUN_ram_2c3f(cVar2 + (char)uVar6 + (bVar1 & 1),sVar3);
  func_0x2a48(sVar5,sVar3);
  return;
}



/* === ram:2a00 : FUN_ram_2a00 === */

/* WARNING: Instruction at (ram,0x31f5) overlaps instruction at (ram,0x31f3)
    */
/* WARNING: Removing unreachable block (ram,0x3448) */
/* WARNING: Removing unreachable block (ram,0x344b) */
/* WARNING: Removing unreachable block (ram,0x2c0a) */
/* WARNING: Removing unreachable block (ram,0x2d5e) */
/* WARNING: Removing unreachable block (ram,0x2c6e) */
/* WARNING: Removing unreachable block (ram,0x2e9b) */
/* WARNING: Removing unreachable block (ram,0x2c42) */
/* WARNING: Removing unreachable block (ram,0x2d9a) */
/* WARNING: Removing unreachable block (ram,0x2c47) */
/* WARNING: Removing unreachable block (ram,0x2da4) */
/* WARNING: Removing unreachable block (ram,0x2c4c) */
/* WARNING: Removing unreachable block (ram,0x2dac) */
/* WARNING: Removing unreachable block (ram,0x2db7) */
/* WARNING: Removing unreachable block (ram,0x2dcc) */
/* WARNING: Removing unreachable block (ram,0x2eb7) */
/* WARNING: Removing unreachable block (ram,0x2ec4) */
/* WARNING: Removing unreachable block (ram,0x326f) */
/* WARNING: Removing unreachable block (ram,0x2c51) */
/* WARNING: Removing unreachable block (ram,0x2df2) */
/* WARNING: Removing unreachable block (ram,0x2f07) */
/* WARNING: Removing unreachable block (ram,0x301c) */
/* WARNING: Removing unreachable block (ram,0x302d) */
/* WARNING: Removing unreachable block (ram,0x3152) */
/* WARNING: Removing unreachable block (ram,0x2f0f) */
/* WARNING: Removing unreachable block (ram,0x2df9) */
/* WARNING: Removing unreachable block (ram,0x2edf) */
/* WARNING: Removing unreachable block (ram,0x2fec) */
/* WARNING: Removing unreachable block (ram,0x2ff9) */
/* WARNING: Removing unreachable block (ram,0x3103) */
/* WARNING: Removing unreachable block (ram,0x2ee9) */
/* WARNING: Removing unreachable block (ram,0x2c56) */
/* WARNING: Removing unreachable block (ram,0x2e23) */
/* WARNING: Removing unreachable block (ram,0x2f38) */
/* WARNING: Removing unreachable block (ram,0x2c5b) */
/* WARNING: Removing unreachable block (ram,0x2f3b) */
/* WARNING: Removing unreachable block (ram,0x306e) */
/* WARNING: Removing unreachable block (ram,0x3092) */
/* WARNING: Removing unreachable block (ram,0x2f57) */
/* WARNING: Removing unreachable block (ram,0x309c) */
/* WARNING: Removing unreachable block (ram,0x31b1) */
/* WARNING: Removing unreachable block (ram,0x32dc) */
/* WARNING: Removing unreachable block (ram,0x31cc) */
/* WARNING: Removing unreachable block (ram,0x30a3) */
/* WARNING: Removing unreachable block (ram,0x2f80) */
/* WARNING: Removing unreachable block (ram,0x30a6) */
/* WARNING: Removing unreachable block (ram,0x2f85) */
/* WARNING: Removing unreachable block (ram,0x30ae) */
/* WARNING: Removing unreachable block (ram,0x31d1) */
/* WARNING: Removing unreachable block (ram,0x2f8a) */
/* WARNING: Removing unreachable block (ram,0x2f8f) */
/* WARNING: Removing unreachable block (ram,0x2f94) */
/* WARNING: Removing unreachable block (ram,0x2f99) */
/* WARNING: Removing unreachable block (ram,0x31f3) */
/* WARNING: Removing unreachable block (ram,0x3160) */
/* WARNING: Removing unreachable block (ram,0x2fab) */
/* WARNING: Removing unreachable block (ram,0x30d4) */
/* WARNING: Removing unreachable block (ram,0x30d9) */
/* WARNING: Removing unreachable block (ram,0x31f0) */
/* WARNING: Removing unreachable block (ram,0x3330) */
/* WARNING: Removing unreachable block (ram,0x3335) */
/* WARNING: Removing unreachable block (ram,0x3443) */
/* WARNING: Removing unreachable block (ram,0x354c) */
/* WARNING: Removing unreachable block (ram,0x31f6) */
/* WARNING: Removing unreachable block (ram,0x322b) */
/* WARNING: Removing unreachable block (ram,0x31f8) */
/* WARNING: Removing unreachable block (ram,0x2d32) */
/* WARNING: Removing unreachable block (ram,0x2e26) */
/* WARNING: Removing unreachable block (ram,0x2f12) */
/* WARNING: Removing unreachable block (ram,0x3279) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined1 FUN_ram_2a00(short param_1,short param_2,undefined1 param_3)

{
  undefined1 *puVar1;
  undefined1 uVar2;
  short sVar3;
  char cVar4;
  undefined1 *in_stack_00000000;
  
  cVar4 = *(char *)(param_2 + param_1);
  sVar3 = CONCAT11((char)((ushort)(param_2 + param_1) >> 8),cVar4);
  *(char *)(param_1 + 2) = cVar4;
  if (cVar4 == '\t') {
    register0x44 = (BADSPACEBASE *)&param_3;
    *in_stack_00000000 = (char)(char *)(param_1 + 2);
    sVar3 = *(short *)(param_1 + 8) + 1;
    *(short *)(param_1 + 8) = sVar3;
  }
  else if (*(char *)(param_1 + 2) == ' ') {
    *(short *)(param_1 + 9) = *(short *)(param_1 + 9) + 1;
    uVar2 = FUN_ram_2afd();
    return uVar2;
  }
  cVar4 = *(char *)(sVar3 + -1);
  if (cVar4 == '\0') {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)((short)register0x44 + -2) = 0x2b32;
    FUN_ram_0190();
  }
  *(undefined1 *)(CONCAT11((char)((ushort)(sVar3 + -1) >> 8),cVar4) + 1) = 0;
  puVar1 = *(undefined1 **)(param_1 + 0x95);
  *(undefined1 **)(param_1 + 0x95) = puVar1 + 2;
  uVar2 = puVar1[1];
  *(undefined1 *)(param_1 + 2) = *puVar1;
  *(undefined1 *)(param_1 + 3) = uVar2;
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)register0x44 + -2) = 0x2d54;
  FUN_ram_0190();
}



/* === ram:2a74 : FUN_ram_2a74 === */

void FUN_ram_2a74(short *param_1,short param_2)

{
  short in_stack_00000000;
  
  *param_1 = (ushort)*(byte *)(param_2 + (short)param_1) + in_stack_00000000 + -0x30;
  FUN_ram_2b46();
  return;
}



/* === ram:2a8f : FUN_ram_2a8f === */

void FUN_ram_2a8f(short param_1,undefined1 *param_2)

{
  undefined2 in_stack_00000000;
  
  FUN_ram_023f();
  *param_2 = (char)in_stack_00000000;
  param_2[1] = (char)((ushort)in_stack_00000000 >> 8);
  *(undefined1 *)(param_1 + 0x93) = 6;
  *(undefined1 *)(param_1 + 0x94) = 0;
  *(undefined1 *)(param_1 + 0x8f) = 0;
  *(undefined1 *)(param_1 + 0x90) = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:2ab4 : FUN_ram_2ab4 === */

/* WARNING: Removing unreachable block (ram,0x1902) */

void FUN_ram_2ab4(short *param_1,undefined1 *param_2,undefined1 param_3,undefined2 param_4)

{
  short sVar1;
  undefined1 uVar4;
  char *pcVar2;
  char *pcVar3;
  char cVar5;
  ushort uVar6;
  undefined1 *in_stack_00000000;
  
  uVar4 = *param_2;
  pcVar2 = (char *)CONCAT11(uVar4,param_3);
  pcVar3 = pcVar2 + 1;
  *param_2 = (char)((ushort)pcVar3 >> 8);
  param_2[-1] = (char)pcVar3;
  cVar5 = *pcVar2;
  if (cVar5 != '\0') {
    sVar1 = *param_1;
    *param_1 = sVar1 + 1;
    *(char *)param_1 = (char)(sVar1 + 1);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  sVar1 = CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1);
  uVar6 = (ushort)*(byte *)CONCAT11(uVar4,cVar5);
  FUN_ram_32b1(0,param_2 + -1);
  if ((char)uVar6 != '\0') {
    uVar6 = sVar1 + 4;
    func_0x3384();
  }
  DAT_ram_eb20 = (undefined1)uVar6;
  *in_stack_00000000 = DAT_ram_eb20;
  DAT_ram_eb21 = (undefined1)(uVar6 >> 8);
  in_stack_00000000[1] = DAT_ram_eb21;
  DAT_ram_eb17 = 0;
  pcVar2 = (char *)CONCAT11(DAT_ram_eb1b,DAT_ram_eb1a);
  pcVar3 = pcVar2 + 1;
  DAT_ram_eb1b = (undefined1)((ushort)pcVar3 >> 8);
  DAT_ram_eb1a = SUB21(pcVar3,0);
  DAT_ram_eb15 = *pcVar2;
  if (DAT_ram_eb15 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_337b(0,cRameb14 + '\x01',param_4);
  return;
}



/* === ram:2ae6 : FUN_ram_2ae6 === */

void FUN_ram_2ae6(char param_1,undefined2 param_2,undefined2 param_3)

{
  DAT_ram_eb07 = (undefined1)((ushort)param_3 >> 8);
  DAT_ram_eb06 = (undefined1)param_3;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1 - (char)((ushort)param_2 >> 8));
}



/* === ram:2afd : FUN_ram_2afd === */

void FUN_ram_2afd(short *param_1)

{
  *param_1 = *param_1 + 1;
  FUN_ram_2bef();
  return;
}



/* === ram:2b46 : FUN_ram_2b46 === */

void FUN_ram_2b46(undefined2 param_1,undefined1 param_2)

{
  undefined1 *puVar1;
  short sVar2;
  undefined1 uVar3;
  
  sVar2 = CONCAT11((char)((ushort)param_1 >> 8),param_2);
  puVar1 = *(undefined1 **)(sVar2 + 0x9d);
  *(undefined1 **)(sVar2 + 0x9d) = puVar1 + 2;
  uVar3 = puVar1[1];
  *(undefined1 *)(sVar2 + 4) = *puVar1;
  *(undefined1 *)(sVar2 + 5) = uVar3;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:2ba0 : FUN_ram_2ba0 === */

void FUN_ram_2ba0(short param_1,short param_2)

{
  *(undefined1 *)(param_2 + 1) = 0;
  *(undefined1 *)(param_1 + 0x8f) = 0;
  *(undefined1 *)(param_1 + 0x90) = 0;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:2bef : FUN_ram_2bef === */

/* WARNING: Instruction at (ram,0x31f5) overlaps instruction at (ram,0x31f3)
    */
/* WARNING: Removing unreachable block (ram,0x344b) */
/* WARNING: Removing unreachable block (ram,0x3448) */
/* WARNING: Removing unreachable block (ram,0x2e26) */
/* WARNING: Removing unreachable block (ram,0x31f3) */
/* WARNING: Removing unreachable block (ram,0x2d32) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

char FUN_ram_2bef(code *param_1,undefined1 *param_2,undefined2 param_3,undefined2 param_4,
                 undefined2 param_5)

{
  undefined1 *puVar1;
  short sVar2;
  undefined2 *puVar3;
  byte bVar4;
  char cVar5;
  undefined1 *puVar6;
  code *pcVar7;
  undefined2 uVar8;
  code cVar9;
  char cVar10;
  code *pcVar11;
  code *pcVar12;
  char *pcVar13;
  undefined1 uStack0000;
  
  *param_2 = (char)param_3;
  param_2[1] = (char)((ushort)param_3 >> 8);
  pcVar11 = *(code **)(param_1 + 4);
  *(code **)(param_1 + 4) = pcVar11 + 1;
  cVar9 = *pcVar11;
  *param_1 = cVar9;
  if (cVar9 != (code)0x2e) {
    param_1[1] = (code)0x0;
    pcVar11 = *(code **)(param_1 + 0x95);
    *(code **)(param_1 + 0x95) = pcVar11 + 2;
    cVar9 = pcVar11[1];
    param_1[2] = *pcVar11;
    param_1[3] = cVar9;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  pcVar11 = param_1 + 4;
                    /* WARNING: Call to offcut address within same function */
  func_0x3384(pcVar11);
  param_1[0x93] = SUB21(pcVar11,0);
  param_1[0x94] = SUB21((ushort)pcVar11 >> 8,0);
  param_1[0x8f] = (code)((char)param_1[0x8f] + '\x01');
  pcVar11 = *(code **)(param_1 + 4);
  *(code **)(param_1 + 4) = pcVar11 + 1;
  *param_1 = *pcVar11;
  pcVar11 = (code *)(ushort)(byte)*param_1;
  pcVar12 = pcVar11;
  FUN_ram_2ab4();
  cVar10 = (char)pcVar11;
  cVar5 = (char)pcVar12;
  if (cVar10 == 'D') {
    bVar4 = (byte)((ushort)pcVar11 >> 8) | 0x44;
    if (bVar4 != 0) {
      puVar6 = *(undefined1 **)(param_1 + 0x8d);
      *(undefined1 **)(param_1 + 0x8d) = puVar6 + 1;
      puVar1 = *(undefined1 **)(param_1 + 2);
      *(undefined1 **)(param_1 + 2) = puVar1 + 1;
      *puVar6 = *puVar1;
      *(short *)(param_1 + 0x93) = *(short *)(param_1 + 0x93) + -1;
      *(short *)(param_1 + 0x91) = *(short *)(param_1 + 0x91) + -1;
      pcVar11 = *(code **)(param_1 + 0x9b);
      *(code **)(param_1 + 0x9b) = pcVar11 + 1;
      *pcVar11 = *param_1;
      pcVar11 = pcVar11 + (short)param_1;
      puVar6 = *(undefined1 **)pcVar11;
      *(undefined1 **)pcVar11 = puVar6 + 1;
      *puVar6 = 0x2d;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(bVar4,pcVar11);
    }
    param_1 = (code *)0xeb14;
    DAT_ram_eb21 = (undefined1)((ushort)pcVar12 >> 8);
    DAT_ram_eb17 = 0;
    pcVar12 = (code *)CONCAT11(DAT_ram_eb1b,DAT_ram_eb1a);
    pcVar11 = pcVar12 + 1;
    DAT_ram_eb20 = cVar5;
    goto LAB_ram_2eb7;
  }
  if (cVar10 == 'U') {
    param_1 = (code *)0x368d;
    pcVar11 = FUN_ram_0005 + 1;
code_r0x2da4:
    pcVar12 = pcVar11 + (short)param_1;
    param_1[0x8d] = SUB21(pcVar12,0);
    pcVar11 = param_1 + 0x8e;
LAB_ram_2dac:
    *pcVar11 = SUB21((ushort)pcVar12 >> 8,0);
    cVar9 = param_1[0x90];
    pcVar11 = (code *)CONCAT11((char)((ushort)(param_1 + 0x90) >> 8),cVar9);
    if (cVar9 == (code)0x0) {
      bVar4 = (code *)0xff6e < param_1;
      sVar2 = *(short *)(param_1 + 0x91);
      *(short *)(param_1 + 0x91) = sVar2 + -1;
      pcVar11 = RST0;
      FUN_ram_0205(0,sVar2);
      if ((bVar4 & 1) != 0) {
        puVar6 = *(undefined1 **)(param_1 + 0x9b);
        pcVar12 = param_1 + 0x9b;
        *(undefined1 **)pcVar12 = puVar6 + 1;
        *puVar6 = 0x20;
        pcVar11 = (code *)CONCAT11(DAT_ram_eb1b,DAT_ram_eb1a);
LAB_ram_2eb7:
        DAT_ram_eb1b = (undefined1)((ushort)pcVar11 >> 8);
        DAT_ram_eb1a = SUB21(pcVar11,0);
        cVar9 = *pcVar12;
        param_1[1] = cVar9;
        if (cVar9 != (code)0x0) {
                    /* WARNING: Subroutine does not return */
          FUN_ram_0190();
        }
        cVar5 = FUN_ram_337b(0,(char)*param_1 + '\x01',_uStack0000);
        return cVar5;
      }
    }
  }
  else {
    if (cVar10 == 'X') goto code_r0x2da4;
    if (cVar10 == 'O') goto LAB_ram_2dac;
    if (cVar10 != 'C') {
      if (cVar10 == 'S') {
        pcVar11 = param_1;
      }
      *pcVar11 = (code)0x0;
      pcVar11 = *(code **)(param_1 + 6);
      *(code **)(param_1 + 6) = pcVar11 + 1;
      cVar9 = *pcVar11;
      param_1[1] = cVar9;
      if (cVar9 == (code)0x2a) {
        *param_1 = (code)((char)*param_1 + '\x01');
        pcVar11 = *(code **)(param_1 + 6);
        *(code **)(param_1 + 6) = pcVar11 + 1;
        param_1[1] = *pcVar11;
        pcVar12 = (code *)(ushort)(byte)param_1[1];
        pcVar7 = pcVar12;
        FUN_ram_2ab4();
        cVar5 = (char)pcVar12;
        if (cVar5 == 'X') {
          if (*pcVar12 != SUB21(_uStack0000,0)) {
            bVar4 = (undefined1 *)0xfffb < &uStack0000;
            puVar6 = (undefined1 *)register0x44;
            FUN_ram_01f9(0x30);
            cVar5 = FUN_ram_01af();
            if ((bool)(bVar4 & 1)) {
              return cVar5;
            }
            pcVar12 = (code *)(ushort)(byte)puVar6[4];
            goto LAB_ram_31d1;
          }
          pcVar12 = FUN_ram_0005 + 1;
          register0x44 = (BADSPACEBASE *)&param_4;
        }
        else if (cVar5 != 'O') {
          if (cVar5 == 'D') goto LAB_ram_31d1;
          pcVar11 = pcVar12;
          if (cVar5 != 'U') {
            if (cVar5 != 'S') {
              if (cVar5 != 'C') {
                    /* WARNING: Subroutine does not return */
                FUN_ram_0190(pcVar7);
              }
              pcVar13 = (char *)(((ushort)pcVar12 & 0xff) +
                                CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1));
              *pcVar13 = *pcVar13 + -0x37;
              cVar5 = FUN_ram_34b4();
              return cVar5;
            }
            *param_1 = (code)0x53;
            register0x44 = (BADSPACEBASE *)&param_4;
            *_uStack0000 = 'S';
            _uStack0000[1] = (char)((ushort)pcVar12 >> 8);
            pcVar11 = param_1 + 3;
            *pcVar11 = (code)((char)*pcVar11 + '\x01');
            pcVar7 = pcVar12;
          }
          goto LAB_ram_30d4;
        }
        pcVar12 = pcVar12 + (short)param_1;
        *(short *)pcVar12 = *(short *)pcVar12 + 1;
      }
      else {
        param_1[4] = cVar9;
        param_1[5] = param_1[2];
        pcVar11 = *(code **)(param_1 + 0x12);
        *(code **)(param_1 + 0x12) = pcVar11 + 1;
        cVar9 = *pcVar11;
        pcVar12 = param_1 + 1;
        *pcVar12 = cVar9;
        if (cVar9 != (code)0x0) {
                    /* WARNING: Subroutine does not return */
          FUN_ram_0190(cVar9,CONCAT11((char)((ushort)(param_1 + 1) >> 8),param_1[1]));
        }
      }
LAB_ram_31d1:
      *(undefined2 *)((short)register0x44 + -2) = 0x31d9;
      FUN_ram_0204((ushort)pcVar12 & 0xff,0x39);
      *(undefined2 *)((short)register0x44 + -2) = 0x31dc;
      cVar5 = FUN_ram_01af();
      return cVar5;
    }
    register0x44 = (BADSPACEBASE *)&param_4;
    *_uStack0000 = cVar5;
    bVar4 = (cVar5 == '\0') << 6;
    if (cVar5 == '\0') {
      FUN_ram_01e5(0);
      cVar9 = SUB21(_uStack0000,0);
      if (!(bool)(bVar4 >> 6 & 1)) {
        return cVar5;
      }
      FUN_ram_3454();
      param_1[1] = cVar9;
      if (cVar9 == (code)0xff) {
        param_5 = CONCAT11(param_1[1],0xff);
                    /* WARNING: Subroutine does not return */
        param_4 = 0x3158;
        FUN_ram_0190();
      }
                    /* WARNING: Subroutine does not return */
      param_5 = 0x3030;
      FUN_ram_0190();
    }
    pcVar11 = param_1 + 0x9b;
    *(short *)pcVar11 = *(short *)(param_1 + 0x9b) + 1;
  }
  *(code **)((short)register0x44 + -2) = pcVar11;
  *(undefined2 *)((short)register0x44 + -4) = 0x2ee3;
  FUN_ram_33e1();
  pcVar7 = *(code **)((short)register0x44 + -2);
  if ((char)((ushort)pcVar11 >> 8) == '\0' && (char)pcVar11 == '\0') {
    uVar8 = *(undefined2 *)register0x44;
    *(code **)register0x44 = pcVar11;
    *(undefined2 *)((short)register0x44 + -2) = 0x2ff1;
    FUN_ram_3454(0,uVar8);
    if (pcVar11 == (code *)0xffff) {
      return (char)*(undefined2 *)((short)register0x44 + 2);
    }
    puVar3 = *(undefined2 **)(param_1 + 0xc);
    *(undefined2 **)(param_1 + 0xc) = puVar3 + 1;
    *(undefined2 *)((short)register0x44 + 2) = *puVar3;
    *(undefined2 *)register0x44 = 0x311a;
    func_0x38c9(param_1 + 0xc);
    register0x44 = (BADSPACEBASE *)((short)register0x44 + 6);
    param_1[3] = (code)((char)param_1[3] + '\x01');
    pcVar7 = (code *)(*(short *)(param_1 + 0x12) + 1);
    pcVar11 = param_1 + 0x12;
    *(code **)pcVar11 = pcVar7;
  }
LAB_ram_30d4:
  if ((char)pcVar11 == '\0') {
    param_1[3] = (code)((char)param_1[3] + '\x01');
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)((short)register0x44 + -2) = 0x30e1;
    FUN_ram_0190(pcVar7);
  }
  bVar4 = CARRY2((ushort)pcVar11,(ushort)param_1);
  param_1 = pcVar11 + (short)param_1;
  *(undefined2 *)register0x44 = 0x31f5;
  FUN_ram_01ec((char)pcVar11);
  if ((bVar4 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)register0x44 = 0x31fb;
    FUN_ram_0190();
  }
  if ((char)((ushort)param_1 >> 8) != '\0' || (char)param_1 != '\0') {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)register0x44 = 0x3338;
    FUN_ram_0190();
  }
  *param_1 = (code)0x22;
  _DAT_ram_36d0 = param_1;
  _DAT_ram_36d2 = param_1;
  return '6';
}



/* === ram:2c3f : FUN_ram_2c3f === */

/* WARNING: Instruction at (ram,0x31f5) overlaps instruction at (ram,0x31f3)
    */
/* WARNING: Removing unreachable block (ram,0x344b) */
/* WARNING: Removing unreachable block (ram,0x3448) */
/* WARNING: Removing unreachable block (ram,0x31f3) */
/* WARNING: Removing unreachable block (ram,0x2e26) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

char FUN_ram_2c3f(char param_1,code *param_2,code *param_3,code *param_4,undefined2 param_5,
                 undefined2 param_6)

{
  undefined1 *puVar1;
  short sVar2;
  undefined2 *puVar3;
  byte in_F;
  byte bVar4;
  char cVar5;
  undefined1 *puVar6;
  code *pcVar7;
  undefined2 uVar8;
  code cVar9;
  code *pcVar10;
  code *pcVar11;
  char *pcVar12;
  undefined1 uStack0000;
  
  cVar5 = (char)param_4;
  if ((bool)(in_F >> 6 & 1)) {
    bVar4 = (byte)((ushort)param_3 >> 8) | (byte)param_3;
    if (bVar4 != 0) {
      puVar6 = *(undefined1 **)(param_2 + 0x8d);
      *(undefined1 **)(param_2 + 0x8d) = puVar6 + 1;
      puVar1 = *(undefined1 **)(param_2 + 2);
      *(undefined1 **)(param_2 + 2) = puVar1 + 1;
      *puVar6 = *puVar1;
      *(short *)(param_2 + 0x93) = *(short *)(param_2 + 0x93) + -1;
      *(short *)(param_2 + 0x91) = *(short *)(param_2 + 0x91) + -1;
      pcVar11 = *(code **)(param_2 + 0x9b);
      *(code **)(param_2 + 0x9b) = pcVar11 + 1;
      *pcVar11 = *param_2;
      pcVar11 = pcVar11 + (short)param_2;
      puVar6 = *(undefined1 **)pcVar11;
      *(undefined1 **)pcVar11 = puVar6 + 1;
      *puVar6 = 0x2d;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(bVar4,pcVar11);
    }
    param_2 = (code *)0xeb14;
    DAT_ram_eb21 = (undefined1)((ushort)param_4 >> 8);
    DAT_ram_eb17 = 0;
    pcVar10 = (code *)CONCAT11(DAT_ram_eb1b,DAT_ram_eb1a);
    pcVar11 = pcVar10 + 1;
    DAT_ram_eb20 = cVar5;
    goto LAB_ram_2eb7;
  }
  if (param_1 == 'U') {
    param_2 = (code *)0x368d;
    param_3 = FUN_ram_0005 + 1;
code_r0x2da4:
    param_4 = param_3 + (short)param_2;
    param_2[0x8d] = SUB21(param_4,0);
    param_3 = param_2 + 0x8e;
LAB_ram_2dac:
    *param_3 = SUB21((ushort)param_4 >> 8,0);
    cVar9 = param_2[0x90];
    pcVar11 = (code *)CONCAT11((char)((ushort)(param_2 + 0x90) >> 8),cVar9);
    if (cVar9 == (code)0x0) {
      bVar4 = (code *)0xff6e < param_2;
      sVar2 = *(short *)(param_2 + 0x91);
      *(short *)(param_2 + 0x91) = sVar2 + -1;
      pcVar11 = RST0;
      FUN_ram_0205(0,sVar2);
      if ((bVar4 & 1) != 0) {
        puVar6 = *(undefined1 **)(param_2 + 0x9b);
        pcVar10 = param_2 + 0x9b;
        *(undefined1 **)pcVar10 = puVar6 + 1;
        *puVar6 = 0x20;
        pcVar11 = (code *)CONCAT11(DAT_ram_eb1b,DAT_ram_eb1a);
LAB_ram_2eb7:
        DAT_ram_eb1b = (undefined1)((ushort)pcVar11 >> 8);
        DAT_ram_eb1a = SUB21(pcVar11,0);
        cVar9 = *pcVar10;
        param_2[1] = cVar9;
        if (cVar9 != (code)0x0) {
                    /* WARNING: Subroutine does not return */
          FUN_ram_0190();
        }
        cVar5 = FUN_ram_337b(0,(char)*param_2 + '\x01',_uStack0000);
        return cVar5;
      }
    }
  }
  else {
    if (param_1 == 'X') goto code_r0x2da4;
    if (param_1 == 'O') goto LAB_ram_2dac;
    if (param_1 != 'C') {
      if (param_1 == 'S') {
        param_3 = param_2;
      }
      *param_3 = (code)0x0;
      pcVar11 = *(code **)(param_2 + 6);
      *(code **)(param_2 + 6) = pcVar11 + 1;
      cVar9 = *pcVar11;
      param_2[1] = cVar9;
      if (cVar9 == (code)0x2a) {
        *param_2 = (code)((char)*param_2 + '\x01');
        pcVar11 = *(code **)(param_2 + 6);
        *(code **)(param_2 + 6) = pcVar11 + 1;
        param_2[1] = *pcVar11;
        pcVar10 = (code *)(ushort)(byte)param_2[1];
        pcVar7 = pcVar10;
        FUN_ram_2ab4();
        cVar5 = (char)pcVar10;
        if (cVar5 == 'X') {
          if (*pcVar10 != SUB21(_uStack0000,0)) {
            bVar4 = (undefined1 *)0xfffb < &uStack0000;
            puVar6 = (undefined1 *)register0x44;
            FUN_ram_01f9(0x30);
            cVar5 = FUN_ram_01af();
            if ((bool)(bVar4 & 1)) {
              return cVar5;
            }
            pcVar10 = (code *)(ushort)(byte)puVar6[4];
            goto LAB_ram_31d1;
          }
          pcVar10 = FUN_ram_0005 + 1;
          register0x44 = (BADSPACEBASE *)&param_5;
        }
        else if (cVar5 != 'O') {
          if (cVar5 == 'D') goto LAB_ram_31d1;
          pcVar11 = pcVar10;
          if (cVar5 != 'U') {
            if (cVar5 != 'S') {
              if (cVar5 != 'C') {
                    /* WARNING: Subroutine does not return */
                FUN_ram_0190(pcVar7);
              }
              pcVar12 = (char *)(((ushort)pcVar10 & 0xff) +
                                CONCAT11((char)((ushort)param_2 >> 8) + '\x01',(char)param_2));
              *pcVar12 = *pcVar12 + -0x37;
              cVar5 = FUN_ram_34b4();
              return cVar5;
            }
            *param_2 = (code)0x53;
            register0x44 = (BADSPACEBASE *)&param_5;
            *_uStack0000 = 'S';
            _uStack0000[1] = (char)((ushort)pcVar10 >> 8);
            pcVar11 = param_2 + 3;
            *pcVar11 = (code)((char)*pcVar11 + '\x01');
            pcVar7 = pcVar10;
          }
          goto LAB_ram_30d4;
        }
        pcVar10 = pcVar10 + (short)param_2;
        *(short *)pcVar10 = *(short *)pcVar10 + 1;
      }
      else {
        param_2[4] = cVar9;
        param_2[5] = param_2[2];
        pcVar11 = *(code **)(param_2 + 0x12);
        *(code **)(param_2 + 0x12) = pcVar11 + 1;
        cVar9 = *pcVar11;
        pcVar10 = param_2 + 1;
        *pcVar10 = cVar9;
        if (cVar9 != (code)0x0) {
                    /* WARNING: Subroutine does not return */
          FUN_ram_0190(cVar9,CONCAT11((char)((ushort)(param_2 + 1) >> 8),param_2[1]));
        }
      }
LAB_ram_31d1:
      *(undefined2 *)((short)register0x44 + -2) = 0x31d9;
      FUN_ram_0204((ushort)pcVar10 & 0xff,0x39);
      *(undefined2 *)((short)register0x44 + -2) = 0x31dc;
      cVar5 = FUN_ram_01af();
      return cVar5;
    }
    register0x44 = (BADSPACEBASE *)&param_5;
    *_uStack0000 = cVar5;
    bVar4 = (cVar5 == '\0') << 6;
    if (cVar5 == '\0') {
      FUN_ram_01e5(0);
      cVar9 = SUB21(_uStack0000,0);
      if (!(bool)(bVar4 >> 6 & 1)) {
        return cVar5;
      }
      FUN_ram_3454();
      param_2[1] = cVar9;
      if (cVar9 == (code)0xff) {
        param_6 = CONCAT11(param_2[1],0xff);
                    /* WARNING: Subroutine does not return */
        param_5 = 0x3158;
        FUN_ram_0190();
      }
                    /* WARNING: Subroutine does not return */
      param_6 = 0x3030;
      FUN_ram_0190();
    }
    pcVar11 = param_2 + 0x9b;
    *(short *)pcVar11 = *(short *)(param_2 + 0x9b) + 1;
  }
  *(code **)((short)register0x44 + -2) = pcVar11;
  *(undefined2 *)((short)register0x44 + -4) = 0x2ee3;
  FUN_ram_33e1();
  pcVar7 = *(code **)((short)register0x44 + -2);
  if ((char)((ushort)pcVar11 >> 8) == '\0' && (char)pcVar11 == '\0') {
    uVar8 = *(undefined2 *)register0x44;
    *(code **)register0x44 = pcVar11;
    *(undefined2 *)((short)register0x44 + -2) = 0x2ff1;
    FUN_ram_3454(0,uVar8);
    if (pcVar11 == (code *)0xffff) {
      return (char)*(undefined2 *)((short)register0x44 + 2);
    }
    puVar3 = *(undefined2 **)(param_2 + 0xc);
    *(undefined2 **)(param_2 + 0xc) = puVar3 + 1;
    *(undefined2 *)((short)register0x44 + 2) = *puVar3;
    *(undefined2 *)register0x44 = 0x311a;
    func_0x38c9(param_2 + 0xc);
    register0x44 = (BADSPACEBASE *)((short)register0x44 + 6);
    param_2[3] = (code)((char)param_2[3] + '\x01');
    pcVar7 = (code *)(*(short *)(param_2 + 0x12) + 1);
    pcVar11 = param_2 + 0x12;
    *(code **)pcVar11 = pcVar7;
  }
LAB_ram_30d4:
  if ((char)pcVar11 == '\0') {
    param_2[3] = (code)((char)param_2[3] + '\x01');
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)((short)register0x44 + -2) = 0x30e1;
    FUN_ram_0190(pcVar7);
  }
  bVar4 = CARRY2((ushort)pcVar11,(ushort)param_2);
  param_2 = pcVar11 + (short)param_2;
  *(undefined2 *)register0x44 = 0x31f5;
  FUN_ram_01ec((char)pcVar11);
  if ((bVar4 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)register0x44 = 0x31fb;
    FUN_ram_0190();
  }
  if ((char)((ushort)param_2 >> 8) != '\0' || (char)param_2 != '\0') {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)register0x44 = 0x3338;
    FUN_ram_0190();
  }
  *param_2 = (code)0x22;
  _DAT_ram_36d0 = param_2;
  _DAT_ram_36d2 = param_2;
  return '6';
}



/* === ram:3282 : FUN_ram_3282 === */

void FUN_ram_3282(void)

{
  return;
}



/* === ram:32b1 : FUN_ram_32b1 === */

void FUN_ram_32b1(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(param_1);
}



/* === ram:337b : FUN_ram_337b === */

void FUN_ram_337b(undefined2 param_1,short param_2)

{
  char *pcVar1;
  
  pcVar1 = (char *)(param_2 + CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1));
  *pcVar1 = *pcVar1 + -0x37;
  FUN_ram_34b4();
  return;
}



/* === ram:33e1 : FUN_ram_33e1 === */

void FUN_ram_33e1(short param_1)

{
  byte bVar1;
  byte bVar2;
  short sVar3;
  undefined2 uVar4;
  ushort uVar5;
  ushort auStack_4 [2];
  
  param_1 = param_1 + (short)register0x44;
  uVar5 = (ushort)*(byte *)(param_1 + 4);
  *(ushort *)(param_1 + -2) = uVar5;
  *(undefined2 *)(param_1 + -4) = 0x33f1;
  sVar3 = param_1;
  FUN_ram_350f();
  uVar4 = *(undefined2 *)(param_1 + -2);
  bVar1 = (byte)(uVar5 >> 8) | (byte)uVar5;
  bVar2 = (bVar1 == 0) << 6;
  *(undefined2 *)(param_1 + -2) = 0x33f7;
  FUN_ram_01a3(bVar1,uVar4);
  if ((bVar2 >> 6 & 1) == 0) {
    return;
  }
  uVar5 = (ushort)*(byte *)(sVar3 + 4);
  *(ushort *)(param_1 + -2) = uVar5;
  *(undefined2 *)(param_1 + -4) = 0x3405;
  FUN_ram_3282();
  uVar4 = *(undefined2 *)(param_1 + -2);
  *(undefined2 *)(param_1 + -2) = 0x340b;
  FUN_ram_01a3((byte)(uVar5 >> 8) | (byte)uVar5,uVar4);
  return;
}



/* === ram:3454 : FUN_ram_3454 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_3454(undefined1 param_1,undefined2 param_2,undefined2 param_3)

{
  byte bVar1;
  byte in_F;
  byte bVar2;
  
  bVar2 = (byte)((ushort)param_3 >> 8);
  DAT_ram_0599 = CONCAT11((char)((ushort)param_2 >> 8),param_1);
  bVar1 = bVar2 << 1;
  _DAT_ram_0597 = CONCAT11(bVar1 | in_F & 1,(char)param_3 << 1 | bVar2 >> 7);
  return bVar1 & 0x7f | in_F & 1;
}



/* === ram:34b4 : FUN_ram_34b4 === */

void FUN_ram_34b4(char param_1,undefined2 param_2)

{
  if (param_1 == '\r') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(2,0xd,10,param_2);
  }
  return;
}



/* === ram:350f : FUN_ram_350f === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Instruction at (ram,0x3695) overlaps instruction at (ram,0x3694)
    */
/* WARNING: Removing unreachable block (ram,0x3682) */
/* WARNING: Removing unreachable block (ram,0x368b) */
/* WARNING: Removing unreachable block (ram,0x3783) */
/* WARNING: Removing unreachable block (ram,0x3786) */
/* WARNING: Removing unreachable block (ram,0x37e5) */
/* WARNING: Removing unreachable block (ram,0x36ac) */
/* WARNING: Removing unreachable block (ram,0x3776) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_350f(short param_1,undefined1 param_2)

{
  byte bVar1;
  char cVar2;
  char cVar3;
  byte bVar4;
  
  bVar4 = (byte)((ushort)DAT_ram_059f >> 8);
  bVar1 = bVar4 & (byte)DAT_ram_059f;
  if (bVar1 == 0xff) {
    _DAT_ram_36ce = DAT_ram_05a1;
    _DAT_ram_36d0 = DAT_ram_05a3;
    _DAT_ram_36d2 = DAT_ram_05a5;
    return 0xff;
  }
  cVar3 = *DAT_ram_059f;
  *(undefined1 *)CONCAT11(bVar4,cVar3) = param_2;
  cVar3 = cVar3 + '\x01';
  cVar2 = (char)((ushort)(param_1 + 1) >> 8);
  if (cVar3 == '\0') {
    *(char *)CONCAT11(bVar4,*(undefined1 *)CONCAT11(bVar4,cVar3)) = cVar2;
    return ~s_We_have_encountered_a_file_creat_ram_2417[9];
  }
  cVar2 = cVar2 + -1;
  if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(CONCAT11(cVar2,0x16),0x5c);
  }
  _DAT_ram_0575 = _DAT_ram_0575 + 1;
  DAT_ram_05a3 = DAT_ram_05a3 + -1;
  *(undefined1 *)CONCAT11(cVar2,(char)(param_1 + 1)) = 0;
                    /* WARNING: Bad instruction - Truncating control flow here */
  halt_baddata();
}



/* === ram:3566 : FUN_ram_3566 === */

/* WARNING: Instruction at (ram,0x3677) overlaps instruction at (ram,0x3675)
    */
/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Removing unreachable block (ram,0x368b) */
/* WARNING: Removing unreachable block (ram,0x359e) */
/* WARNING: Removing unreachable block (ram,0x3677) */
/* WARNING: Removing unreachable block (ram,0x35a3) */
/* WARNING: Removing unreachable block (ram,0x35a8) */
/* WARNING: Removing unreachable block (ram,0x36b2) */
/* WARNING: Removing unreachable block (ram,0x36b8) */
/* WARNING: Removing unreachable block (ram,0x011d) */
/* WARNING: Removing unreachable block (ram,0x011e) */
/* WARNING: Removing unreachable block (ram,0x011f) */
/* WARNING: Removing unreachable block (ram,0x3670) */
/* WARNING: Removing unreachable block (ram,0x367b) */
/* WARNING: Removing unreachable block (ram,0x35bd) */
/* WARNING: Removing unreachable block (ram,0x3586) */
/* WARNING: Removing unreachable block (ram,0x358b) */
/* WARNING: Removing unreachable block (ram,0x37ac) */
/* WARNING: Removing unreachable block (ram,0x369e) */
/* WARNING: Removing unreachable block (ram,0x369f) */
/* WARNING: Removing unreachable block (ram,0x3885) */
/* WARNING: Removing unreachable block (ram,0x3776) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined1 FUN_ram_3566(undefined2 param_1,short param_2)

{
  char cVar1;
  byte bVar2;
  undefined1 uVar3;
  
  cVar1 = (char)((ushort)param_1 >> 8) + -1;
  if (cVar1 != '\0') {
    _DAT_ram_0575 = param_2 + 1;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(CONCAT11(cVar1,0x1a),0x80);
  }
  bVar2 = DAT_ram_36d4 >> 7;
  DAT_ram_36d4 = DAT_ram_36d4 * '\x02';
  bRam0080 = bRam36d5;
  if (bRam36d5 < 0xfb && !CARRY1(bRam36d5 + 5,bVar2)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005();
  }
  uVar3 = func_0x0038(0x36d7);
  return uVar3;
}


