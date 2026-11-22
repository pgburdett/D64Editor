/* Decompiled CASTLE.COM - Ghidra Z80 decompiler */

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

/* WARNING: Control flow encountered bad instruction data */

void RST6(char param_1,undefined1 *param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  enableMaskableInterrupts();
  bVar1 = func_0x0d96(param_1 + '\x01');
  if (bVar1 == 0x38) {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
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



/* === ram:0126 : FUN_ram_0126 === */

void FUN_ram_0126(void)

{
  return;
}



/* === ram:0129 : FUN_ram_0129 === */

void FUN_ram_0129(undefined2 param_1,undefined2 param_2)

{
  byte bVar1;
  
  bVar1 = (byte)((ushort)param_1 >> 8) | (byte)param_1;
  if (bVar1 == 0) {
    return;
  }
  FUN_ram_0289(bVar1,param_2);
  func_0x026b(param_1);
  return;
}



/* === ram:012c : FUN_ram_012c === */

void FUN_ram_012c(undefined2 param_1,undefined2 param_2)

{
  FUN_ram_0289(param_2);
  func_0x026b(param_1);
  return;
}



/* === ram:013f : FUN_ram_013f === */

void FUN_ram_013f(undefined2 param_1,undefined2 param_2)

{
  DAT_ram_0571 = 0;
  FUN_ram_025a();
  FUN_ram_025a(param_2,param_1);
  func_0x026b();
  return;
}



/* === ram:0147 : FUN_ram_0147 === */

void FUN_ram_0147(void)

{
  FUN_ram_025a();
  func_0x026b();
  return;
}



/* === ram:0190 : FUN_ram_0190 === */

void FUN_ram_0190(void)

{
  return;
}



/* === ram:019d : thunk_FUN_ram_0296 === */

void thunk_FUN_ram_0296(char *param_1,undefined1 *param_2)

{
  byte in_F;
  char cVar1;
  char *pcVar2;
  
  if (!(bool)(in_F >> 6 & 1)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(1);
  }
  if ((in_F >> 6 & 1) == 0) {
    func_0x0038();
    return;
  }
  cVar1 = FUN_ram_0503(*param_2);
  pcVar2 = param_2 + 2;
  *param_1 = cVar1 + -0x40;
  param_1 = param_1 + 1;
  FUN_ram_0147();
  if (*pcVar2 == '.') {
    FUN_ram_04c6(pcVar2 + 1);
    *param_1 = '\0';
    param_1[0x14] = '\0';
    FUN_ram_013f(param_1 + 0x15);
    return;
  }
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
    if ((bool)(extraout_F_00 & 1)) {
      func_0x0038();
      return;
    }
    *puVar11 = uVar7;
    pcVar10 = pcVar10 + 1;
    if ((char)((ushort)uVar9 >> 8) != '\x01') {
      func_0x0038(puVar11 + 1);
      return;
    }
  }
  else {
    if (DAT_ram_06df == '\0') {
      return;
    }
    uStack_2 = uVar9;
    register0x44 = (BADSPACEBASE *)&uStack_2;
    if (DAT_ram_06df == '\"') {
      DAT_ram_0572 = DAT_ram_06df;
      cVar8 = (code)0x22;
      register0x44 = (BADSPACEBASE *)puVar12;
      goto LAB_ram_03df;
    }
  }
  cVar8 = *pcVar10;
  uVar9 = uStack_2;
LAB_ram_03df:
  *(undefined2 *)((short)register0x44 + -2) = 0x3e2;
  FUN_ram_04f1(cVar8,uVar9);
  if (!(bool)(extraout_F_01 & 1)) {
    FUN_ram_04de(pcVar10 + 1);
    return;
  }
  return;
}



/* === ram:01a9 : FUN_ram_01a9 === */

/* WARNING: Removing unreachable block (ram,0x050d) */
/* WARNING: Removing unreachable block (ram,0x04e9) */

void FUN_ram_01a9(short param_1,short param_2,undefined1 *param_3,undefined1 param_4)

{
  byte bVar1;
  short sVar2;
  byte in_F;
  byte extraout_F;
  byte extraout_F_00;
  ushort uVar3;
  undefined1 uVar4;
  code cVar5;
  undefined2 uVar6;
  code *pcVar7;
  undefined1 *puVar8;
  undefined2 in_stack_00000000;
  
  if (!(bool)(in_F & 1)) {
    bVar1 = (byte)((ushort)param_3 >> 8);
    uVar3 = CONCAT11(bVar1 >> 1,bVar1) & 0xff01;
    if ((char)((ushort)in_stack_00000000 >> 8) == '\x01') {
      return;
    }
    sVar2 = (param_1 + 1) * 2;
    FUN_ram_02a6(CONCAT11((char)((ushort)sVar2 >> 8),(byte)sVar2 | (byte)in_stack_00000000 & 1),
                 param_2 + CONCAT11(~bVar1,~(byte)param_3) + 1,
                 CONCAT11((char)(uVar3 >> 8),(byte)param_3 >> 1 | (char)uVar3 << 7));
    return;
  }
  *param_3 = 0;
  pcVar7 = RST0 + 1;
  uVar6 = 0x762;
  DAT_ram_0572 = '\0';
  puVar8 = (undefined1 *)0x6e0;
  if (DAT_ram_06df == ' ') {
    uVar4 = FUN_ram_04f1(RST0);
    if ((bool)(extraout_F & 1)) {
      func_0x0038();
      return;
    }
    *puVar8 = uVar4;
    pcVar7 = pcVar7 + 1;
    if ((char)((ushort)uVar6 >> 8) != '\x01') {
      func_0x0038(puVar8 + 1);
      return;
    }
    register0x44 = (BADSPACEBASE *)&param_4;
  }
  else {
    if (DAT_ram_06df == '\0') {
      return;
    }
    in_stack_00000000 = uVar6;
    if (DAT_ram_06df == '\"') {
      DAT_ram_0572 = DAT_ram_06df;
      cVar5 = (code)0x22;
      goto LAB_ram_03df;
    }
  }
  cVar5 = *pcVar7;
LAB_ram_03df:
  *(undefined2 *)((short)register0x44 + -2) = 0x3e2;
  FUN_ram_04f1(cVar5,in_stack_00000000);
  if ((bool)(extraout_F_00 & 1)) {
    return;
  }
  FUN_ram_04de(pcVar7 + 1);
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



/* === ram:01ec : FUN_ram_01ec === */

void FUN_ram_01ec(byte param_1)

{
  char in_F;
  
  FUN_ram_02e6(param_1 >> 1 | in_F << 7);
  return;
}



/* === ram:01f3 : FUN_ram_01f3 === */

/* WARNING: Instruction at (ram,0x03e6) overlaps instruction at (ram,0x03e4)
    */
/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x03cf) */
/* WARNING: Removing unreachable block (ram,0x04e7) */
/* WARNING: Removing unreachable block (ram,0x03d6) */
/* WARNING: Removing unreachable block (ram,0x03dd) */
/* WARNING: Removing unreachable block (ram,0x03e2) */
/* WARNING: Removing unreachable block (ram,0x03e3) */
/* WARNING: Removing unreachable block (ram,0x04c7) */
/* WARNING: Removing unreachable block (ram,0x0403) */
/* WARNING: Removing unreachable block (ram,0x0405) */
/* WARNING: Removing unreachable block (ram,0x0406) */
/* WARNING: Removing unreachable block (ram,0x0409) */
/* WARNING: Removing unreachable block (ram,0x0408) */

char FUN_ram_01f3(char param_1,undefined2 param_2,short param_3,char param_4)

{
  undefined1 *puVar1;
  char cVar2;
  
  if (param_4 == '\0') {
    return param_1;
  }
  puVar1 = (undefined1 *)(param_3 * 2);
  if (param_1 == '\"') {
    DAT_ram_0572 = 0;
    puVar1[-1] = 0;
  }
  *puVar1 = 0x20;
  if ((char)((ushort)param_2 >> 8) == '\0') {
    return ' ';
  }
  cVar2 = func_0x0038(0x20,puVar1 + 1);
  return cVar2;
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



/* === ram:0296 : FUN_ram_0296 === */

void FUN_ram_0296(char *param_1,undefined1 *param_2)

{
  byte in_F;
  char cVar1;
  char *pcVar2;
  
  if (!(bool)(in_F >> 6 & 1)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(1);
  }
  if ((in_F >> 6 & 1) == 0) {
    func_0x0038();
    return;
  }
  cVar1 = FUN_ram_0503(*param_2);
  pcVar2 = param_2 + 2;
  *param_1 = cVar1 + -0x40;
  param_1 = param_1 + 1;
  FUN_ram_0147();
  if (*pcVar2 == '.') {
    FUN_ram_04c6(pcVar2 + 1);
    *param_1 = '\0';
    param_1[0x14] = '\0';
    FUN_ram_013f(param_1 + 0x15);
    return;
  }
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



/* === ram:02e6 : FUN_ram_02e6 === */

/* WARNING: Instruction at (ram,0x03e6) overlaps instruction at (ram,0x03e4)
    */
/* WARNING: Removing unreachable block (ram,0x0308) */
/* WARNING: Removing unreachable block (ram,0x03cf) */
/* WARNING: Removing unreachable block (ram,0x04e7) */
/* WARNING: Removing unreachable block (ram,0x03d6) */
/* WARNING: Removing unreachable block (ram,0x03dd) */
/* WARNING: Removing unreachable block (ram,0x03e2) */
/* WARNING: Removing unreachable block (ram,0x03e3) */
/* WARNING: Removing unreachable block (ram,0x04c7) */
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
    bVar1 = func_0x0038(0x20,param_2 + 2);
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



/* === ram:04de : FUN_ram_04de === */

void FUN_ram_04de(void)

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
    Instruction at (ram,0x0992) overlaps instruction at (ram,0x0990)
 */

/* === ram:0d26 : FUN_ram_0d26 === */

void FUN_ram_0d26(char param_1,undefined2 param_2)

{
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(param_1 - (char)((ushort)param_2 >> 8),0xe502,0xf2e);
}



/* === ram:0d69 : FUN_ram_0d69 === */

void FUN_ram_0d69(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(0x80d2,param_1);
}



/* === ram:0d87 : FUN_ram_0d87 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_0d87(short param_1)

{
  short sVar1;
  short in_stack_00000000;
  
  sVar1 = *(short *)(param_1 + -0x7f16);
  func_0x02fa();
  _DAT_ram_80da = sVar1 + in_stack_00000000;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:0da8 : FUN_ram_0da8 === */

void FUN_ram_0da8(undefined1 param_1,undefined1 *param_2,undefined2 param_3)

{
  undefined1 *puVar1;
  
  *param_2 = param_1;
  FUN_ram_38a7(param_2 + 6,0xf59);
  puVar1 = param_2 + 6;
  FUN_ram_33fe(param_3);
  FUN_ram_343d(puVar1,&DAT_ram_8082);
  FUN_ram_0d26(param_2 + 6);
  return;
}



/* FAILED ram:0ed0 : Low-level Error: Bad instruction count exceeded:
    ...
    Instruction at (ram,0x1126) overlaps instruction at (ram,0x1125)
 */

/* FAILED ram:0f38 : Low-level Error: Bad instruction count exceeded:
    ...
    Instruction at (ram,0x1126) overlaps instruction at (ram,0x1125)
 */

/* FAILED ram:0f74 : Low-level Error: Bad instruction count exceeded:
    ...
    Instruction at (ram,0x1126) overlaps instruction at (ram,0x1125)
 */

/* === ram:1166 : FUN_ram_1166 === */

void FUN_ram_1166(undefined2 param_1)

{
                    /* WARNING: Subroutine does not return */
  thunk_FUN_ram_0190(0x12e4,param_1);
}



/* === ram:1182 : FUN_ram_1182 === */

/* WARNING: Instruction at (ram,0x14ea) overlaps instruction at (ram,0x14e8)
    */
/* WARNING: Removing unreachable block (ram,0x144a) */
/* WARNING: Removing unreachable block (ram,0x143f) */
/* WARNING: Removing unreachable block (ram,0x14b4) */
/* WARNING: Removing unreachable block (ram,0x1442) */
/* WARNING: Removing unreachable block (ram,0x14ac) */
/* WARNING: Removing unreachable block (ram,0x1448) */
/* WARNING: Removing unreachable block (ram,0x144b) */
/* WARNING: Removing unreachable block (ram,0x14b6) */
/* WARNING: Removing unreachable block (ram,0x144e) */
/* WARNING: Removing unreachable block (ram,0x1453) */
/* WARNING: Removing unreachable block (ram,0x1458) */
/* WARNING: Removing unreachable block (ram,0x145d) */
/* WARNING: Removing unreachable block (ram,0x1479) */
/* WARNING: Removing unreachable block (ram,0x1497) */
/* WARNING: Removing unreachable block (ram,0x1498) */
/* WARNING: Removing unreachable block (ram,0x1464) */
/* WARNING: Removing unreachable block (ram,0x1469) */
/* WARNING: Removing unreachable block (ram,0x146a) */
/* WARNING: Removing unreachable block (ram,0x146e) */
/* WARNING: Removing unreachable block (ram,0x14ea) */
/* WARNING: Removing unreachable block (ram,0x14c3) */
/* WARNING: Heritage AFTER dead removal. Example location: s0x0004 : 0x1504 */
/* WARNING: Removing unreachable block (ram,0x14e7) */
/* WARNING: Removing unreachable block (ram,0x14e8) */
/* WARNING: Restarted to delay deadcode elimination for space: stack */

void FUN_ram_1182(short param_1,undefined2 param_2,short *param_3)

{
  byte extraout_F;
  byte extraout_F_00;
  undefined1 uVar1;
  byte bVar2;
  byte bVar4;
  short *psVar3;
  byte bVar5;
  char cVar6;
  short sVar7;
  short sVar8;
  undefined2 in_stack_00000000;
  
  FUN_ram_20c1();
  FUN_ram_2242(in_stack_00000000);
  FUN_ram_37ed();
  FUN_ram_01e5(0x59);
  if ((extraout_F >> 6 & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_38a7();
  sVar7 = param_1 + 0xb;
  FUN_ram_33fe(param_2);
  sVar8 = param_1 + 0xb;
  FUN_ram_383a(sVar7);
  FUN_ram_383a(sVar8);
  sVar8 = param_1 + 0xb;
  sVar7 = param_1;
  FUN_ram_320d(&DAT_ram_8082);
  if ((char)((ushort)sVar7 >> 8) != '\0' || (char)sVar7 != '\0') {
    FUN_ram_20c1(sVar8);
    *(undefined1 *)(param_1 + 2) = 1;
    *(undefined1 *)(param_1 + 3) = 0;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190((undefined1 *)(param_1 + 3),0x1542);
  }
  bVar2 = (byte)param_1;
  halt();
  bVar5 = (byte)((ushort)sVar8 >> 8);
  *(undefined1 *)(param_1 << 8) = 0;
  bVar4 = (byte)((ushort)param_1 >> 8);
  halt();
  *(byte *)CONCAT11(bVar2,bVar2) = bVar2;
  psVar3 = (short *)CONCAT11(bVar4,bVar4);
  *(undefined1 *)(param_1 << 8) = 0;
  *(byte *)CONCAT11(bVar4,bVar4) = bVar2;
  *(byte *)CONCAT11(bVar4,bVar4) = bVar4;
  *(byte *)CONCAT11(bVar4,bVar4) = bVar2;
  cVar6 = *(char *)CONCAT11(bVar4,bVar4);
  *(byte *)CONCAT11(bVar4,cVar6) = bVar4;
  cVar6 = cVar6 + -1;
  *(byte *)CONCAT11(cVar6,bVar4) = bVar4;
  if (cVar6 == '\0') {
    *(byte *)(ushort)bVar4 = bVar2;
    *(byte *)(ushort)bVar4 = bVar5;
    *(byte *)CONCAT11(*(undefined1 *)CONCAT11(bVar4,*(undefined1 *)CONCAT11(bVar4,bVar4)),bVar4) =
         bVar5;
    *(byte *)CONCAT11(bVar4,bVar4) = bVar4;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  bVar2 = bVar4 & bVar2;
  sVar7 = CONCAT11(bVar4 & bVar5,bVar2);
  if ((bVar4 & bVar5) != 0 || bVar2 != 0) {
    psVar3[3] = psVar3[3] + 1000;
    *psVar3 = *psVar3 + 1;
    psVar3[1] = psVar3[1] * 2;
    FUN_ram_397a();
    FUN_ram_023f(500);
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  psVar3 = (short *)CONCAT11(0x7c,bVar4);
  uVar1 = FUN_ram_01a3(bVar2);
  if (!(bool)(extraout_F_00 >> 6 & 1)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  *param_3 = CONCAT11(*(undefined1 *)(sVar7 + 1),uVar1) + 1000;
  *psVar3 = *psVar3 + 1;
  psVar3[1] = psVar3[1] * 2;
  func_0x0038();
  return;
}



/* === ram:1353 : FUN_ram_1353 === */

/* WARNING: Instruction at (ram,0x14ea) overlaps instruction at (ram,0x14e8)
    */
/* WARNING: Removing unreachable block (ram,0x147a) */
/* WARNING: Removing unreachable block (ram,0x1471) */
/* WARNING: Removing unreachable block (ram,0x1469) */
/* WARNING: Removing unreachable block (ram,0x146e) */
/* WARNING: Removing unreachable block (ram,0x14ea) */
/* WARNING: Removing unreachable block (ram,0x1475) */
/* WARNING: Removing unreachable block (ram,0x14e7) */
/* WARNING: Removing unreachable block (ram,0x14e8) */
/* WARNING: Removing unreachable block (ram,0x14ec) */
/* WARNING: Removing unreachable block (ram,0x1608) */
/* WARNING: Removing unreachable block (ram,0x1611) */
/* WARNING: Removing unreachable block (ram,0x1718) */
/* WARNING: Removing unreachable block (ram,0x14f6) */
/* WARNING: Removing unreachable block (ram,0x147f) */
/* WARNING: Removing unreachable block (ram,0x14fa) */
/* WARNING: Removing unreachable block (ram,0x1509) */
/* WARNING: Removing unreachable block (ram,0x148b) */
/* WARNING: Removing unreachable block (ram,0x1492) */
/* WARNING: Removing unreachable block (ram,0x1495) */
/* WARNING: Removing unreachable block (ram,0x14c3) */

void FUN_ram_1353(short param_1,short param_2)

{
  char cVar1;
  undefined1 *puVar2;
  
  cVar1 = *(char *)(param_2 + -0x7ef6);
  puVar2 = (undefined1 *)CONCAT11((char)((ushort)(param_2 + -0x7ef6) >> 8),cVar1);
  if (cVar1 == -1) {
    *puVar2 = 0xff;
    puVar2[1] = 0xff;
    *(undefined1 *)(param_1 + 2) = 1;
    puVar2 = (undefined1 *)(param_1 + 3);
    *puVar2 = 0;
  }
  else {
    puVar2 = (undefined1 *)CONCAT11(0x20,(char)((ushort)param_1 >> 8));
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(puVar2);
}



/* === ram:13a6 : FUN_ram_13a6 === */

void FUN_ram_13a6(short param_1,short param_2)

{
  if (*(short *)(param_2 + -0x7f12) == -1) {
    RST0 = (code)0x0;
    RST0 = 0;
    *(undefined1 *)(param_1 + 2) = 1;
    *(undefined1 *)(param_1 + 3) = 0;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:14a8 : FUN_ram_14a8 === */

void FUN_ram_14a8(char param_1)

{
  func_0x026b(param_1 + '\x01',0x25);
  return;
}



/* === ram:14d5 : FUN_ram_14d5 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_14d5(short param_1,undefined1 *param_2)

{
  byte in_F;
  undefined2 uVar1;
  
  uVar1 = _DAT_ram_a2ee;
  FUN_ram_01ec();
  if ((in_F & 1) != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  FUN_ram_020f();
  *param_2 = (char)((ushort)uVar1 >> 8);
  param_2[-1] = (char)uVar1;
  *(short *)(param_1 + 2) = *(short *)(param_1 + 2) * 2;
  func_0x0038(param_1 + 3);
  return;
}



/* === ram:1614 : FUN_ram_1614 === */

/* WARNING: Removing unreachable block (ram,0x1616) */

void FUN_ram_1614(undefined1 param_1,undefined1 param_2)

{
  char cVar1;
  byte bVar2;
  byte in_F;
  short sVar3;
  undefined2 uVar4;
  undefined1 *puVar5;
  
  sVar3 = CONCAT11(0xc3,param_2);
  cVar1 = BCDadjust(param_1,in_F & 1,in_F >> 4 & 1);
  bVar2 = BCDadjustCarry(cVar1,in_F & 1,in_F >> 4 & 1);
  hasEvenParity(cVar1);
  uVar4 = 1;
  FUN_ram_4754(cVar1 << 1 | bVar2 & 1,1);
  puVar5 = (undefined1 *)0x14;
  FUN_ram_020f();
  *puVar5 = (char)((ushort)uVar4 >> 8);
  puVar5[-1] = (char)uVar4;
  *(short *)(sVar3 + 2) = *(short *)(sVar3 + 2) * 2;
  func_0x0038(sVar3 + 3);
  return;
}



/* === ram:1733 : FUN_ram_1733 === */

void FUN_ram_1733(undefined1 param_1,short param_2)

{
  short *in_stack_00000000;
  
  *in_stack_00000000 = CONCAT11(*(undefined1 *)(param_2 + 1),param_1) * 2;
  func_0x0038();
  return;
}



/* === ram:1ad1 : FUN_ram_1ad1 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1ad1(short param_1)

{
  if (*(char *)(param_1 + -0x7ef6) != -1) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  _DAT_ram_802c = 0x1e64;
  DAT_ram_8045 = 2;
  _DAT_ram_802e = 0x1e69;
  DAT_ram_8046 = 4;
  return;
}



/* === ram:1b56 : FUN_ram_1b56 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1b56(short param_1)

{
  byte bVar1;
  short sVar2;
  
  sVar2 = *(short *)(param_1 + -0x7f12) + 1;
  bVar1 = (byte)((ushort)sVar2 >> 8) | (byte)sVar2;
  if (bVar1 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar1);
  }
  _DAT_ram_801e = sVar2;
  DAT_ram_803e = 6;
  _DAT_ram_8020 = 0x1e40;
  DAT_ram_803f = 0;
  _DAT_ram_8022 = 0x1e46;
  DAT_ram_8040 = 6;
  _DAT_ram_8024 = 0x1e4d;
  DAT_ram_8041 = 6;
  _DAT_ram_8026 = 0x1e52;
  DAT_ram_8042 = 0;
  _DAT_ram_8028 = 0x1e56;
  DAT_ram_8043 = 5;
  _DAT_ram_802a = 0x1e5d;
  DAT_ram_8044 = 6;
  _DAT_ram_802c = 0x1e64;
  DAT_ram_8045 = 2;
  _DAT_ram_802e = 0x1e69;
  DAT_ram_8046 = 4;
  return;
}



/* === ram:1b9c : FUN_ram_1b9c === */

/* WARNING: Instruction at (ram,0x1bba) overlaps instruction at (ram,0x1bb9)
    */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_1b9c(short *param_1,undefined2 *param_2)

{
  byte bVar1;
  
  FUN_ram_20c1(*param_2);
  bVar1 = (short *)0xfffd < param_1;
  param_1[1] = param_1[1] + 1;
  FUN_ram_0205(4);
  if ((bVar1 & 1) == 0) {
    _DAT_ram_802c = 0x1e64;
    DAT_ram_8045 = 2;
    _DAT_ram_802e = 0x1e69;
    DAT_ram_8046 = 4;
    return;
  }
  do {
    param_1[2] = param_1[2] + 1;
    *(undefined1 *)(param_1 + 1) = 0;
    *(undefined1 *)((short)param_1 + 3) = 0;
    *param_1 = *param_1 + 1;
    param_1 = (short *)CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1);
  } while( true );
}



/* === ram:20c1 : FUN_ram_20c1 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_20c1(undefined2 param_1,undefined2 param_2)

{
  FUN_ram_24c0(param_1);
  FUN_ram_2386(param_2);
  FUN_ram_23e1(0x82df);
  return;
}



/* === ram:20ed : FUN_ram_20ed === */

/* WARNING: Removing unreachable block (ram,0x220e) */
/* WARNING: Removing unreachable block (ram,0x2225) */
/* WARNING: Removing unreachable block (ram,0x233f) */
/* WARNING: Removing unreachable block (ram,0x2344) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_20ed(undefined1 *param_1)

{
  ushort uVar1;
  ushort uVar2;
  undefined1 in_stack_00000000;
  
  if ((char)_DAT_ram_82d8 == '\0') {
    *_DAT_ram_82d8 = in_stack_00000000;
    DAT_ram_9fa6 = 0;
    FUN_ram_25a5(*param_1);
    return;
  }
  FUN_ram_2386();
  uVar1 = _DAT_ram_82d2 & 0xff;
  uVar2 = uVar1;
  FUN_ram_23e1(0x82d7);
  FUN_ram_20c1(uVar1);
  FUN_ram_2352(uVar2);
  return;
}



/* === ram:215c : FUN_ram_215c === */

void FUN_ram_215c(undefined1 *param_1)

{
  FUN_ram_2262(*param_1);
  return;
}



/* === ram:2174 : FUN_ram_2174 === */

void FUN_ram_2174(void)

{
  return;
}



/* === ram:21de : FUN_ram_21de === */

void FUN_ram_21de(void)

{
  return;
}



/* === ram:2242 : FUN_ram_2242 === */

void FUN_ram_2242(undefined2 param_1,short param_2)

{
  FUN_ram_48cf(4,*(undefined1 *)
                  (param_2 + CONCAT11((char)((ushort)param_1 >> 8) + '\x01',(char)param_1)));
  return;
}



/* === ram:2262 : FUN_ram_2262 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_2262(code param_1,code *param_2,code *param_3,ushort param_4)

{
  short sVar1;
  byte in_F;
  byte bVar2;
  undefined1 uVar3;
  code cVar5;
  ushort uVar4;
  undefined1 uVar7;
  code *pcVar6;
  undefined1 auStack_6 [2];
  undefined2 uStack_4;
  code *pcStack_2;
  
  *param_3 = param_1;
  cVar5 = SUB21(param_4 >> 8,0);
  *param_3 = cVar5;
  uVar3 = SUB21(param_2,0);
  uVar7 = (undefined1)((ushort)param_3 >> 8);
  *(undefined1 *)CONCAT11(uVar7,uVar3) = uVar7;
  pcVar6 = (code *)CONCAT11(uVar3,uVar3);
  if ((in_F >> 6 & 1) == 0) {
LAB_ram_22ce:
    *(short *)pcVar6 = *(short *)pcVar6 + -1;
    FUN_ram_23ae();
    return;
  }
  *pcVar6 = cVar5;
  *pcVar6 = cVar5;
  *(code *)CONCAT11(uVar3,param_1) = cVar5;
  uVar4 = param_4 & 0xff;
  *(char *)((short)param_2 << 8) = (char)param_4;
  pcVar6 = RST0;
  if ((in_F >> 6 & 1) == 0) {
    pcStack_2 = (code *)(ushort)(byte)RST0;
    uStack_4 = 0x22cb;
    FUN_ram_22eb();
    pcVar6 = param_2;
    goto LAB_ram_22ce;
  }
  if ((in_F >> 6 & 1) == 0) {
    pcStack_2 = FUN_ram_22eb;
    RST1();
code_r0x22eb:
    bVar2 = (byte)pcVar6;
    if (bVar2 != 1) goto LAB_ram_2410;
    pcVar6 = FUN_ram_0005 + 1;
  }
  else {
    pcVar6 = (code *)(ushort)(byte)param_1;
    if ((in_F >> 6 & 1) == 0) goto code_r0x22eb;
    *(char *)CONCAT11(param_1,param_1) = (char)((ushort)param_2 >> 8);
    pcVar6 = (code *)CONCAT11(param_1,param_1);
    *pcVar6 = param_1;
    if ((in_F >> 6 & 1) != 0) {
      *(undefined1 *)CONCAT11((char)param_4,param_1) = 0;
      pcStack_2 = param_2;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(auStack_6);
    }
    param_2 = (code *)((ushort)param_2 & 0xff);
  }
  pcVar6 = pcVar6 + (short)param_2;
  sVar1 = *(short *)pcVar6;
  *(short *)pcVar6 = sVar1 + -1;
  bVar2 = 0;
  uVar4 = 0;
  if (sVar1 != 0) {
    pcStack_2 = (code *)(_DAT_ram_82d0 & 0xff);
    uStack_4 = 0x230c;
    FUN_ram_22eb();
    FUN_ram_23f2(pcStack_2);
    return;
  }
LAB_ram_2410:
  pcStack_2 = (code *)CONCAT11(bVar2 ^ (byte)(uVar4 >> 8),(byte)pcVar6 ^ (byte)uVar4);
  uStack_4 = 0x2419;
  FUN_ram_22eb();
  FUN_ram_2527(pcStack_2);
  return;
}



/* === ram:22e0 : FUN_ram_22e0 === */

void FUN_ram_22e0(void)

{
  return;
}



/* === ram:22eb : FUN_ram_22eb === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_22eb(short param_1,short *param_2,undefined2 param_3)

{
  short sVar1;
  byte bVar2;
  ushort uVar3;
  
  bVar2 = (byte)param_2;
  if (bVar2 == 1) {
    sVar1 = *(short *)(param_1 + 6);
    param_2 = (short *)(param_1 + 6);
    *param_2 = sVar1 + -1;
    bVar2 = 0;
    param_3 = 0;
    if (sVar1 != 0) {
      uVar3 = _DAT_ram_82d0 & 0xff;
      FUN_ram_22eb();
      FUN_ram_23f2(uVar3);
      return;
    }
  }
  FUN_ram_22eb();
  FUN_ram_2527(CONCAT11(bVar2 ^ (byte)((ushort)param_3 >> 8),(byte)param_2 ^ (byte)param_3));
  return;
}



/* === ram:2352 : FUN_ram_2352 === */

void FUN_ram_2352(void)

{
  return;
}



/* === ram:2386 : FUN_ram_2386 === */

void FUN_ram_2386(short param_1,undefined2 param_2)

{
  *(char *)(param_1 + 0x50) = (char)param_2;
  *(char *)(param_1 + 0x51) = (char)((ushort)param_2 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190(0);
}



/* === ram:23ae : FUN_ram_23ae === */

void FUN_ram_23ae(undefined2 param_1)

{
  FUN_ram_20c1(param_1);
  return;
}



/* === ram:23e1 : FUN_ram_23e1 === */

void FUN_ram_23e1(char *param_1,char param_2)

{
  char in_stack_00000000;
  
  *param_1 = param_2 + in_stack_00000000;
  FUN_ram_24fc();
  return;
}



/* === ram:23f2 : FUN_ram_23f2 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_23f2(byte *param_1,char param_2)

{
  byte bVar1;
  byte bVar2;
  char in_stack_00000000;
  
  *param_1 = param_2 + in_stack_00000000;
  if ((char)((uint3)_DAT_ram_82cd >> 8) == '\0') {
    func_0x026b(0xa2,5);
    return;
  }
  bVar2 = *param_1;
  bVar1 = (byte)_DAT_ram_82cd;
  FUN_ram_22eb();
  FUN_ram_2527(bVar1 ^ bVar2);
  return;
}



/* === ram:243b : FUN_ram_243b === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_243b(void)

{
  ushort uVar1;
  
  uVar1 = _DAT_ram_9fa6;
  if ((char)_DAT_ram_9fa6 == '\0') {
    func_0x026b(0);
    return;
  }
  _DAT_ram_9fa6 = _DAT_ram_9fa6 & 0xff00;
  FUN_ram_25a5(uVar1 & 0xff);
  return;
}



/* === ram:24c0 : FUN_ram_24c0 === */

void FUN_ram_24c0(void)

{
  FUN_ram_21de(0xd1d1);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:24fc : FUN_ram_24fc === */

void FUN_ram_24fc(undefined2 param_1,short param_2)

{
  FUN_ram_0229(param_1);
  FUN_ram_22eb(param_2 + 0x30);
  return;
}



/* === ram:2527 : FUN_ram_2527 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_2527(byte param_1,short param_2,short param_3)

{
  *(undefined1 *)(param_2 + param_3) = 0xea;
  ((undefined1 *)(param_2 + param_3))[1] = 0x29;
  func_0x026b(param_1 & (byte)((ushort)param_3 >> 8),_DAT_ram_a2ee,5);
  return;
}



/* === ram:25a5 : FUN_ram_25a5 === */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_ram_25a5(byte param_1,short param_2,undefined2 param_3)

{
  _DAT_ram_a2ee = param_2 + 1;
  func_0x026b(param_1 & (byte)((ushort)param_3 >> 8),0xee2a,param_2,5);
  return;
}



/* === ram:25ae : FUN_ram_25ae === */

void FUN_ram_25ae(void)

{
  func_0x026b(5);
  return;
}



/* === ram:31ba : FUN_ram_31ba === */

void FUN_ram_31ba(undefined2 param_1)

{
  undefined1 *in_stack_00000000;
  
  *in_stack_00000000 = (char)param_1;
  in_stack_00000000[1] = (char)((ushort)param_1 >> 8);
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:320d : FUN_ram_320d === */

void FUN_ram_320d(undefined1 *param_1,undefined2 param_2)

{
  *param_1 = (char)((ushort)param_2 >> 8);
  return;
}



/* === ram:3310 : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:33fe : FUN_ram_33fe === */

void FUN_ram_33fe(undefined1 param_1,undefined1 *param_2,short param_3)

{
  short in_stack_00000000;
  
  *param_2 = param_1;
  FUN_ram_023f(0x10);
  func_0x02fa();
  *(short *)(param_2 + 6) = param_3 + in_stack_00000000;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:343d : FUN_ram_343d === */

void FUN_ram_343d(undefined1 *param_1,undefined2 param_2)

{
  *param_1 = (char)((ushort)param_2 >> 8);
  return;
}



/* === ram:3540 : FUN_ram_3540 === */

void FUN_ram_3540(char *param_1)

{
  char *in_stack_00000000;
  
  *in_stack_00000000 = *param_1 + '\n';
  return;
}



/* === ram:36a7 : FUN_ram_36a7 === */

/* WARNING: Removing unreachable block (ram,0x37e8) */
/* WARNING: Removing unreachable block (ram,0x37f3) */
/* WARNING: Removing unreachable block (ram,0x3903) */

void FUN_ram_36a7(short param_1,short param_2)

{
  undefined1 *puVar1;
  short in_stack_00000000;
  
  if (*(char *)(param_2 + in_stack_00000000) == '\0') {
    puVar1 = *(undefined1 **)(*(short *)(param_1 * 2) + 4);
    *(undefined1 **)(*(short *)(param_1 * 2) + 4) = puVar1 + 1;
    *puVar1 = *(undefined1 *)(param_1 + 4);
    return;
  }
  FUN_ram_4022(*(char *)(param_2 + in_stack_00000000) - (char)((ushort)param_1 >> 8),0xe504);
  return;
}



/* === ram:37ed : FUN_ram_37ed === */

void FUN_ram_37ed(short param_1,short param_2)

{
  undefined2 uStack_2;
  
  if ((char)param_2 != ' ') {
                    /* WARNING: Subroutine does not return */
    *(undefined2 *)(param_2 + (short)register0x44 + -2) = 0x390a;
    FUN_ram_0190(param_2 + (short)register0x44);
  }
  *(short *)(param_1 + 9) = *(short *)(param_1 + 9) + 1;
  FUN_ram_38d7();
  return;
}



/* === ram:383a : FUN_ram_383a === */

void FUN_ram_383a(short *param_1,short param_2)

{
  undefined1 *puVar1;
  short sVar2;
  short sVar3;
  short sVar4;
  undefined2 in_stack_00000000;
  
  if ((char)((ushort)param_2 >> 8) != '\0' || (char)param_2 != '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(in_stack_00000000);
  }
  sVar3 = (ushort)*(byte *)(param_2 + 1) * 0x100;
  sVar2 = *(short *)(sVar3 + 2);
  *(short *)(sVar3 + 2) = sVar2 + -1;
  if (sVar2 == 0) {
    sVar4 = 0x80;
    func_0x02cb(*(undefined2 *)(sVar3 + 4));
    sVar2 = 8;
    func_0x02fa();
    *param_1 = sVar4 + sVar2;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  puVar1 = *(undefined1 **)(param_1[3] + 4);
  *(undefined1 **)(param_1[3] + 4) = puVar1 + 1;
  *puVar1 = (char)param_1[2];
  return;
}



/* === ram:384c : FUN_ram_384c === */

void FUN_ram_384c(undefined1 param_1,short *param_2,short param_3)

{
  short in_stack_00000000;
  
  *(undefined1 *)param_2 = param_1;
  *param_2 = (ushort)*(byte *)(param_3 + (short)param_2) + in_stack_00000000 + -0x30;
  FUN_ram_3920();
  return;
}



/* === ram:3882 : thunk_FUN_ram_0190 === */

void thunk_FUN_ram_0190(void)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:38a7 : FUN_ram_38a7 === */

void FUN_ram_38a7(undefined2 param_1)

{
  DAT_ram_eb07 = (undefined1)((ushort)param_1 >> 8);
  DAT_ram_eb06 = (undefined1)param_1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0190();
}



/* === ram:38d7 : FUN_ram_38d7 === */

void FUN_ram_38d7(short param_1,undefined1 *param_2)

{
  char *pcVar1;
  undefined1 *puVar2;
  short sVar3;
  undefined1 *in_stack_00000000;
  
  *in_stack_00000000 = *param_2;
  pcVar1 = *(char **)(param_1 + 8);
  *(short *)(param_1 + 8) = (short)(pcVar1 + 1);
  if (*pcVar1 == '\0') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190((short *)(param_1 + 8));
  }
  sVar3 = *(short *)(param_1 + 6);
  *(undefined1 *)(sVar3 + 2) = 0xff;
  *(undefined1 *)(sVar3 + 3) = 3;
  *(short *)(*(short *)(param_1 + 6) + 4) = *(short *)(param_1 + 6) + 6;
  puVar2 = *(undefined1 **)(*(short *)(param_1 + 6) + 4);
  *(undefined1 **)(*(short *)(param_1 + 6) + 4) = puVar2 + 1;
  *puVar2 = *(undefined1 *)(param_1 + 4);
  return;
}



/* === ram:3920 : FUN_ram_3920 === */

void FUN_ram_3920(short param_1,undefined1 param_2)

{
  byte bVar1;
  short sVar2;
  undefined1 *puVar3;
  
  puVar3 = (undefined1 *)(param_1 + -1);
  bVar1 = (byte)((ushort)puVar3 >> 8) | (byte)puVar3;
  if (bVar1 != 0) {
    sVar2 = CONCAT11(*puVar3,param_2) + 1;
    *(short *)(param_1 + -2) = sVar2;
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190(bVar1,param_1 + -2 + sVar2);
  }
  FUN_ram_4969(5,DAT_ram_2b0a);
  return;
}



/* === ram:397a : FUN_ram_397a === */

void FUN_ram_397a(undefined1 param_1,short param_2,undefined2 param_3)

{
  undefined1 *puVar1;
  short sVar2;
  
  sVar2 = CONCAT11((char)((ushort)param_3 >> 8),param_1);
  puVar1 = *(undefined1 **)(sVar2 + 4);
  *(undefined1 **)(sVar2 + 4) = puVar1 + 1;
  *puVar1 = *(undefined1 *)(param_2 + 4);
  return;
}



/* === ram:39ff : FUN_ram_39ff === */

void FUN_ram_39ff(short param_1,short param_2,short param_3)

{
  undefined1 *puVar1;
  
  puVar1 = *(undefined1 **)(param_2 + param_3);
  *(undefined1 **)(param_2 + param_3) = puVar1 + 1;
  *puVar1 = *(undefined1 *)(param_1 + 4);
  return;
}



/* === ram:4022 : FUN_ram_4022 === */

/* WARNING: Removing unreachable block (ram,0x4209) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_4022(char param_1,short *param_2,short *param_3,char *param_4,undefined2 param_5)

{
  byte bVar1;
  undefined1 *puVar2;
  byte *pbVar3;
  byte in_F;
  char cVar4;
  undefined1 uVar5;
  byte bVar6;
  byte bVar7;
  undefined2 uVar8;
  undefined2 *puVar9;
  short *psVar10;
  short sVar11;
  undefined1 *puVar12;
  char *pcVar13;
  char *in_stack_00000000;
  
  cVar4 = (char)((ushort)param_3 >> 8);
  if ((bool)(in_F >> 6 & 1)) {
    if (cVar4 != '\0' || (char)param_3 != '\0') {
      puVar12 = *(undefined1 **)((short)param_2 + 0x8d);
      *(undefined1 **)((short)param_2 + 0x8d) = puVar12 + 1;
      puVar2 = (undefined1 *)param_2[1];
      param_2[1] = (short)(puVar2 + 1);
      *puVar12 = *puVar2;
      *(short *)((short)param_2 + 0x93) = *(short *)((short)param_2 + 0x93) + -1;
      *(short *)((short)param_2 + 0x91) = *(short *)((short)param_2 + 0x91) + -1;
      puVar12 = *(undefined1 **)((short)param_2 + 0x9b);
      *(undefined1 **)((short)param_2 + 0x9b) = puVar12 + 1;
      *puVar12 = (char)*param_2;
      puVar9 = (undefined2 *)(puVar12 + (short)param_2);
      puVar12 = (undefined1 *)*puVar9;
      *puVar9 = puVar12 + 1;
      *puVar12 = 0x2d;
                    /* WARNING: Subroutine does not return */
      FUN_ram_0190(puVar9);
    }
    FUN_ram_0204(0);
    bVar7 = FUN_ram_01af();
    return bVar7;
  }
  if (param_1 == 'U') {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0190();
  }
  if (param_1 == 'X') {
    param_4 = (char *)((short)param_3 + (short)param_2);
    *(char *)((short)param_2 + 0x8d) = (char)param_4;
    param_3 = param_2 + 0x47;
  }
  else if (param_1 != 'O') {
    if (param_1 != 'C') {
      if (param_1 == 'S') {
        register0x44 = (BADSPACEBASE *)&param_5;
        param_4 = in_stack_00000000;
      }
      if (param_1 == 'S' && (cVar4 == '\0' && (char)param_3 == '\0')) {
        *(undefined1 *)param_3 = 0;
        *(undefined1 *)((short)param_3 + 1) = 0;
                    /* WARNING: Subroutine does not return */
        *(undefined2 *)((short)register0x44 + -2) = 0x443a;
        FUN_ram_0190(param_4);
      }
      *(short **)((short)register0x44 + -2) = param_2 + 2;
      psVar10 = *(short **)((short)register0x44 + -2);
      *(char *)psVar10 = (char)param_2[2] + -0x37;
      pbVar3 = (byte *)*psVar10;
      *psVar10 = (short)(pbVar3 + 1);
      sVar11 = (ushort)*pbVar3 + *(short *)register0x44 + -0x30;
      puVar12 = (undefined1 *)((short)param_2 + 1);
      bVar7 = (byte)((ushort)sVar11 >> 8);
      *param_2 = sVar11;
      if (puVar12 == (undefined1 *)0x0) {
        *(undefined2 *)register0x44 = 0x4547;
        FUN_ram_0126(0,0x6c,&DAT_ram_05a3);
        puVar12 = &DAT_ram_0081;
        DAT_ram_0081 = 0;
        *(char ***)register0x44 = &DAT_ram_05a1;
        if ((char)DAT_ram_05a1 == '\0' && DAT_ram_05a1._1_1_ == '\0') {
          cVar4 = -*DAT_ram_05a1;
          *(char **)((short)register0x44 + -2) = DAT_ram_05a1;
          *(undefined2 *)((short)register0x44 + -4) = 0x4681;
          FUN_ram_20c1();
          uVar8 = *(short *)((short)register0x44 + -2);
          *(undefined2 *)((short)register0x44 + -2) = 0x5a97;
                    /* WARNING: Subroutine does not return */
          *(undefined2 *)((short)register0x44 + -4) = 0x468b;
          FUN_ram_0005(CONCAT11(cVar4,0xb),uVar8);
        }
        pcVar13 = DAT_ram_05a1 + -1;
        *(undefined2 *)((short)register0x44 + -2) = 0x4560;
        uVar5 = FUN_ram_466f(0x20);
        *puVar12 = uVar5;
        if (pcVar13[1] != '\0') {
          *(undefined2 *)((short)register0x44 + -2) = 0x465e;
          bVar7 = func_0x0038(puVar12 + 1);
          return bVar7;
        }
        bVar6 = 0;
      }
      else {
        bVar6 = (byte)puVar12 >> 7;
        _DAT_ram_0599 = CONCAT11((char)((ushort)puVar12 >> 8),(byte)puVar12 << 1);
        bVar1 = bVar7 << 1;
        _DAT_ram_0597 = CONCAT11(bVar1 | bVar6,(char)sVar11 << 1 | bVar7 >> 7);
        bVar6 = bVar1 & 0x7f | bVar6;
      }
      return bVar6;
    }
    register0x44 = (BADSPACEBASE *)&param_5;
    cVar4 = (char)param_4;
    *in_stack_00000000 = cVar4;
    if (cVar4 == '\0') {
      *(char *)param_2 = (char)*param_2;
                    /* WARNING: Subroutine does not return */
      param_5 = 0x43fa;
      FUN_ram_0190(0);
    }
    psVar10 = (short *)((short)param_2 + 0x9b);
    *psVar10 = *(short *)((short)param_2 + 0x9b) + 1;
    goto LAB_ram_42c1;
  }
  *(char *)param_3 = (char)((ushort)param_4 >> 8);
  cVar4 = (char)param_2[0x48];
  psVar10 = (short *)CONCAT11((char)((ushort)(param_2 + 0x48) >> 8),cVar4);
  if (cVar4 == '\0') {
    bVar7 = (short *)0xff6e < param_2;
    sVar11 = *(short *)((short)param_2 + 0x91);
    *(short *)((short)param_2 + 0x91) = sVar11 + -1;
    psVar10 = (short *)0x0;
    cVar4 = FUN_ram_0205(0,0,sVar11);
    if ((bVar7 & 1) != 0) {
      puVar12 = *(undefined1 **)((short)param_2 + 0x9b);
      *(undefined2 *)((short)param_2 + 0x9bU) = puVar12 + 1;
      *puVar12 = 0x20;
      sVar11 = CONCAT11((char)(undefined2 *)((short)param_2 + 0x9bU),0xca);
      bVar7 = (*(char *)(sVar11 + 4) == '\t') << 6;
      thunk_FUN_ram_0296();
      if ((bool)(bVar7 >> 6 & 1)) {
                    /* WARNING: Subroutine does not return */
        FUN_ram_0190();
      }
      bVar7 = thunk_FUN_ram_0296(*(undefined1 *)(sVar11 + 4));
      return bVar7;
    }
  }
LAB_ram_42c1:
                    /* WARNING: Subroutine does not return */
  *(undefined2 *)((short)register0x44 + -2) = 0x42c6;
  FUN_ram_0190(cVar4,psVar10);
}



/* === ram:466f : FUN_ram_466f === */

void FUN_ram_466f(undefined2 param_1)

{
  uRam4818 = param_1;
  return;
}



/* === ram:46ce : FUN_ram_46ce === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Instruction at (ram,0x47bd) overlaps instruction at (ram,0x47bc)
    */
/* WARNING: Removing unreachable block (ram,0x46e4) */
/* WARNING: Removing unreachable block (ram,0x47bd) */
/* WARNING: Removing unreachable block (ram,0x46e9) */
/* WARNING: Removing unreachable block (ram,0x46ee) */
/* WARNING: Removing unreachable block (ram,0x47f8) */
/* WARNING: Removing unreachable block (ram,0x47ff) */
/* WARNING: Removing unreachable block (ram,0x4805) */
/* WARNING: Removing unreachable block (ram,0x481d) */
/* WARNING: Removing unreachable block (ram,0x494a) */
/* WARNING: Removing unreachable block (ram,0x4960) */
/* WARNING: Removing unreachable block (ram,0x4a61) */
/* WARNING: Removing unreachable block (ram,0x47b5) */
/* WARNING: Removing unreachable block (ram,0x47bb) */
/* WARNING: Removing unreachable block (ram,0x48c7) */
/* WARNING: Removing unreachable block (ram,0x48c9) */
/* WARNING: Removing unreachable block (ram,0x48ca) */
/* WARNING: Removing unreachable block (ram,0x47c1) */
/* WARNING: Removing unreachable block (ram,0x47c5) */
/* WARNING: Removing unreachable block (ram,0x011d) */
/* WARNING: Removing unreachable block (ram,0x011e) */
/* WARNING: Removing unreachable block (ram,0x011f) */
/* WARNING: Removing unreachable block (ram,0x4703) */

undefined1 FUN_ram_46ce(byte param_1,short param_2)

{
  code *UNRECOVERED_JUMPTABLE;
  ushort in_AF;
  undefined1 uVar1;
  undefined1 auStack_2 [2];
  
  if ((in_AF & 1) == 0) {
    param_1 = (byte)(in_AF >> 8) & param_1;
  }
  else {
    register0x44 = (BADSPACEBASE *)auStack_2;
    param_1 = cRam4814 + bRam481a;
    param_2 = 0x481b;
    bRam481a = param_1;
  }
  *(ushort *)((short)register0x44 + -2) = param_2 + (ushort)(byte)(param_1 * '\x03');
  UNRECOVERED_JUMPTABLE = *(code **)((short)register0x44 + -2);
  *(undefined2 *)((short)register0x44 + -2) = 0x48ee;
                    /* WARNING: Could not recover jumptable at 0x47ed. Too many branches */
                    /* WARNING: Treating indirect jump as call */
  uVar1 = (*UNRECOVERED_JUMPTABLE)(DAT_ram_05a1);
  return uVar1;
}



/* === ram:4754 : FUN_ram_4754 === */

/* WARNING: Removing unreachable block (ram,0x4765) */

void FUN_ram_4754(undefined2 param_1)

{
  DAT_ram_0080 = *DAT_ram_05a1;
  DAT_ram_0575 = DAT_ram_0575 + 1;
  FUN_ram_4915(DAT_ram_0080 & (byte)((ushort)param_1 >> 8));
  return;
}



/* === ram:4820 : FUN_ram_4820 === */

void FUN_ram_4820(undefined2 param_1,undefined2 param_2)

{
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005(0x1a,param_2,param_1);
}



/* === ram:48cf : FUN_ram_48cf === */

/* WARNING: Control flow encountered bad instruction data */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

byte FUN_ram_48cf(short param_1,short param_2)

{
  byte in_F;
  byte bVar1;
  short sVar2;
  
  param_1 = param_1 + 1;
  bVar1 = FUN_ram_012c(DAT_ram_059f);
  if ((bool)(in_F & 1)) {
    if ((char)bVar1 < '\0') {
      return ~(byte)param_2;
    }
    return bVar1;
  }
  sVar2 = _DAT_ram_059f;
  func_0x4a3d(_DAT_ram_059f,param_1);
  if ((char)DAT_ram_05a3 == '\0') {
                    /* WARNING: Bad instruction - Truncating control flow here */
    halt_baddata();
  }
  sVar2 = DAT_ram_05a1 + sVar2;
  DAT_ram_0571 = ((byte)((ushort)sVar2 >> 8) & 0x3f) << 1 | (byte)sVar2 >> 7;
  if (DAT_ram_0571 != *(byte *)(param_2 + 0xc)) {
                    /* WARNING: Subroutine does not return */
    FUN_ram_0005(0x10,0xc,param_2,(byte *)(param_2 + 0xc),sVar2);
  }
                    /* WARNING: Bad instruction - Truncating control flow here */
  halt_baddata();
}



/* === ram:4915 : FUN_ram_4915 === */

void FUN_ram_4915(undefined1 param_1,undefined1 *param_2,undefined2 param_3)

{
  *param_2 = param_1;
                    /* WARNING: Subroutine does not return */
  FUN_ram_0005(0x700f,param_3);
}



/* === ram:4969 : FUN_ram_4969 === */

/* WARNING: Control flow encountered bad instruction data */

void FUN_ram_4969(undefined2 param_1,undefined2 param_2)

{
  undefined1 uVar1;
  
  uVar1 = (undefined1)((ushort)param_1 >> 8);
  func_0x4f98(uVar1,CONCAT11(uVar1,(char)((ushort)param_2 >> 8)) + 4);
                    /* WARNING: Bad instruction - Truncating control flow here */
  halt_baddata();
}


