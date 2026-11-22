# Nemesis (CP/M) game-data format analysis

Status: **work in progress / partially confirmed**. This report is for a C# port.
Source files were **not modified**. All offsets are decimal unless written `0x...`.

## Evidence sources

- `docs/nemesis/game-files/` staged CP/M data set.
- `/tmp/opencode/nemesis-source/ALL-SOURCE-FRAGMENTS.txt` and per-file `LEVEL.*.txt` extracts.
- `/tmp/opencode/level0-full.txt` (LEVEL.0 bytes with non-printables shown as `<XX>`).
- `nemesis-manual-ocr.txt`, `decomp-*.c`, `disasm-*.txt`, `DM.COM` strings.
- `MAST.CAT` and `MCAT505.DQC` were **not found** anywhere in the repo; they are not analyzed.

## 1. Big picture: these are mixed binary + stale C-source files

The `.LEVEL`, `CASTLE.0`, `L0`-`L7`, `ARAGORN.CHR` and even item data files contain
**interleaved C source text** (PERSON.C / character code / Z8000 stdio) surrounded by
binary tables. The source text is *stale bytes from deleted/overwritten CP/M directory
blocks*, not meaningful data. It is useful only because it exposes struct names and
stat constants:

- `PERSON.stat[STR|INT|WIS|CHA|CON|DEX|...]`
- `PERSON.stat[CLASS]`, `[XMULT]`, `[AGEC]`, `[I_1]`, `[I_2]`, `[ALI]`, `[SEX]`
- `PERSON.stat[TYPE]`, `[LVL]`, `[DLVL]`, `[X]`, `[Y]`, `[HIPAGE]`, `[HITS]`, `[O_HITS]`, `[AGE1]`
- `xclass[t].factor[...]`, `xtype.factor[...]`, `xtype[t3].factor[t1]`
- `PERSON.p_item[t].i_type`, `PERSON.exp`, `PERSON.gold`, `PERSON.class`
- `MAX_ITEMS`, `CARRY_OVER`, `CSTATS`, `MAX_NAME`, `NSTR`
- Character initialization rules (`init_char()`) and class validity tests (`valid()`).

This is the most reliable evidence for character/stat indexing; the Z80 decompilations
are raw Ghidra output with almost no usable literals.

## 2. File inventory and sizes (confirmed)

| File(s) | Size | Type |
|---|---:|---|
| `LEVEL.0`-`LEVEL.10` (no 9), `CASTLE.0` | 7168 | binary level blob with stale source |
| `L0`-`L7` | 1792 | map-only blob (1716 map bytes + 76 stale bytes) |
| `MONSTERS.0-1` | 512 | CP/M text monster records |
| `MONSTERS.2-3` | 768 | CP/M text monster records |
| `MONSTERS.4-5` | 896 | CP/M text monster records |
| `MONSTERS.6-7` | 768 | CP/M text monster records |
| `ITEMS.1-2` | 640 | CP/M text item records |
| `ITEMS.3-4` | 896 | CP/M text item records |
| `ITEMS.4-5` | 1792 | CP/M text item records |
| `ITEMS.6-7` | 3584 | CP/M text item records |
| `ITEMS.C-0` | 768 | CP/M text item records |
| `ARAGORN.CHR` | 640 | mixed binary/text character-ish file |

CP/M `0x1a` EOF padding begins at: MONSTERS.0-1 `0x1e4`, MONSTERS.2-3 `0x2de`,
MONSTERS.4-5 `0x331`, MONSTERS.6-7 `0x2e3`; ITEMS.1-2 `0x239`, ITEMS.3-4 `0x31b`,
ITEMS.4-5 `0x6a6`, ITEMS.6-7 `0xd99`, ITEMS.C-0 `0x2c8`.

## 3. LEVEL.N / CASTLE.0 binary layout (partially confirmed)

Representative `LEVEL.0` (7168 bytes):

- `0x0000`: `uint16 LE` count of monster instances (`0x22` = 34 in LEVEL.0/1,
  `0x38` = 56 in LEVEL.2/3, `0x3f` = 63 in LEVEL.4/5, `0x57` = 87 in LEVEL.10),
  followed by 2 padding bytes.
- `0x0004 + 16*i`: monster-instance records, `count` of them.
- Around `0x0222`-`0x0641`: stale C source text; the monster name table starts at
  `0x0642` in LEVEL.0 (`Veteran\0Warrior\0Swordman\0Orc\0Pseudo-Dragon\0Medium\0Seer\0Conjurer\0Acolyte\0Adept\0Priest\0Apprentice\0Footpad\0Robber\0`).
  Name-table offsets seen in records: 0, 8, 16, 25, 29, 43, 50, 55, 64, 72, 78, 85, 96, 104.
- Around `0x0c84` onward in LEVEL.0: **39-byte binary item records**
  (`5 × int16 LE` + 29-byte NUL-padded name). Confirmed records:
  SWORD `(2,10,0,0,-1)`, MACE `(1,10,0,0,-1)`, HELMET `(0,10,0,0,-1)`,
  MAGIC TORCH `(0,5,1,0,-1)`, AXE `(2,10,0,0,-1)`, LEATHER `(0,10,0,0,-1)`,
  SHIELD `(0,10,0,0,-1)`, WAR HAMMER `(2,10,0,0,-1)`, MAIL `(0,10,0,0,-1)`,
  DAGGER `(1,10,0,0,-1)`, PLATE `(0,15,0,0,-1)`, BRICK `(0,32,0,0,-1)`, etc.
  The unused tail of each 29-byte name field contains stale C source bytes.
- `0x1482`-`0x1b35` in LEVEL.0: **map** = 1716 bytes = 572 cells × 3 bytes.
- `0x1b36` onward: stale shared C-source tail (`/* macro define */`, `waitkey()`, ...).

### 3.1 Monster instance record (16 bytes, high confidence)

From `LEVEL.0` record 0: `0a 05 01 05 01 0d 05 08 00 01 00 00 00 00 00 00`,
matching text `4 Veteran 10 5 1 5 1 13 5 8 1 0 0`.

Layout:

```
offset 0..6   : 7 × int8   stats (text fields 4..10)
offset 7..8   : int16 LE   stat 8 (text field 11); may be negative, e.g. -1 -> ff ff
offset 9      : int8       stat 9 (text field 12)
offset 10     : int8       stat 10 (text field 13)
offset 11     : int8       stat 11 (text field 14) in some files; 0 in LEVEL.0 sample
offset 12..13 : uint16     0 / reserved
offset 14..15 : uint16 LE  offset into monster-name string table
```

Confirmation: `LEVEL.2` last record `13 02 02 0a 05 14 0f ff ff 05 00 00 00 00 91 00`
matches `1 Cockatrice 19 2 2 10 5 20 15 -1 5 0 0`; `ff ff` is the sign-extended `-1`.

The *text* monster files store one line per **monster type**:
`[count] [Name] [f0] [f1] ... [f10]  CR LF`. `count` is the number of consecutive
16-byte instance records on that level. Names use `_` for spaces (`Ogre_Mage`,
`Large_spider`). Fields include negatives (`-1`, `-3`, `-4`).

## 4. MAP / cell encoding (partially confirmed)

`L0`-`L7` are 1792 bytes; the first 1716 bytes are the same 572-cell × 3-byte map
as the map region inside the corresponding `LEVEL.N`. `L0` map is byte-identical to
the LEVEL.0 map except a few cells. `L0`/`L1` have the shared stale source tail at
`0x6b4`; `L7` is mostly zero tail.

Cell is 3 bytes:

```
byte 0: high nibble = 4-direction wall bitmask (N/E/S/W), low nibble mostly 0
byte 1: feature flag bitmask (see below)
byte 2: feature parameter / index (stairway 1..3, teleporter, multiplier, etc.)
```

Observed byte-0 high nibbles in L0: `0` (132×), `3` (96×), `4` (22×), `5` (9×),
`7` (35×), `c` (157×), `d` (40×), `f` (65×); L7 adds `a`,`b`,`e`. This is consistent
with wall bits: `0xf` solid, `0xc`/`0x3` parallel walls, `0xd`/`0x7` dead ends,
`0x0` open room. Low nibble is 0 except rare markers (`0x04`, `0x02`, `0x01`).

Grid shape is **uncertain**: 572 = 26×22 or 22×26; both reshapes produce plausible
walls/corridors. The game's display/DM code was not conclusively decoded for dimensions.

Feature-name order from `DM.COM`/`DUNGEON.COM` strings:
`dark, pit, tport, shaft, a/m, a/c, illsn., fire, melee, spec.`

Likely bit assignment (hypothesis, **not fully confirmed**):
`dark=0x01, pit=0x02, tport=0x04, shaft=0x08, a/m=0x10, a/c=0x20,
illsn.=0x40, fire=0x80`; `melee`/`spec.` may live in another flag byte or in byte 2.
Observed byte-1 values in L7 include `0x02,0x04,0x08,0x10,0x20,0x30,0x38,0x80,0x82,0x90,0xa0,0xb0`,
which fit the low 8 feature bits. Byte-2 values in L7 include `1` (105×), `2` (20×),
`3`,`4`,`8`,`16`,`20`,`24`,`28`,`36`,`40`,`52`,`64`.

## 5. Character/stat source evidence (confirmed from extracted C)

Key code fragments:

```c
for(seed=0; seed<CSTATS; ++seed) PERSON.stat[seed]=0;
...
t = PERSON.stat[CLASS];
if(t==4 || t==5 || t==9 || t==12) PERSON.p_mspell = 0xffff; else PERSON.p_mspell = 0;
if(t==0 || t==1 || t==7 || t==9 || t==10 || t==12) PERSON.p_cspell = 0xFFFF; else PERSON.p_cspell = 0;
PERSON.p_ac = 10;
PERSON.stat[LVL] = 0;
PERSON.stat[DLVL] = PERSON.stat[X] = PERSON.stat[Y] = PERSON.stat[HIPAGE] = -1;
PERSON.stat[AGE1] = PERSON.stat[TYPE]==14 ? 89 : 16;
PERSON.stat[O_HITS] = PERSON.stat[HITS] = 1 + rand() % (PERSON.stat[CON]+PERSON.stat[STR]);
for(t1=0; t1<MAX_ITEMS; ++t1) PERSON.p_item[t1].i_type = 255;
```

`valid()`:

```c
for(t1=0; t1<6; ++t1)
    if(PERSON.stat[t1] < xclass[xx].factor[t1]) return(FALSE);
if((t1==9) && (PERSON.stat[TYPE]!=0)) return(FALSE);
if((PERSON.stat[ALI] & xclass[xx].factor[ALI] & 0x0007)==0) return(FALSE);
if((PERSON.stat[SEX] & xclass[xx].factor[SEX] & 0x0018)==0) return(FALSE);
```

`get_class()`:

```c
PERSON.stat[CLASS] = t1;
PERSON.stat[XMULT] = (PERSON.stat[XMULT]*xclass[t1].factor[XMULT])/200;
PERSON.stat[AGEC]  = (PERSON.stat[AGEC]*xclass[t1].factor[AGEC])/10;
PERSON.stat[I_1]  |= xclass[t1].factor[I_1];
PERSON.stat[I_2]  |= xclass[t1].factor[I_2];
strcpy(PERSON.class, xclass[t1].xname);
```

`compute_stats()`:

```c
tot=0; t3=PERSON.stat[TYPE];
for(t1=0; t1<6; ++t1)
    tot += PERSON.stat[t1] = (t2 = roll() + (2*xtype[t3].factor[t1])) <= 0 ? 1 : t2;
PERSON.stat[SEX] = (rand()%2) ? M : F;
sprintf(PERSON.gold, NSTR, rand()%(200-tot));
```

Stat indices 0..5 are `STR,INT,WIS,CHA,CON,DEX` (from `display_stats()` printf calls).
`ALI` and `SEX` are bitfields: alignment masks `0x0007` (L/N/C), sex masks `0x0018` (M/F).
`XMULT`/`AGEC` are percentage/divisor factors; `I_1`/`I_2` are bit flags.
`PERSON.p_item[t].i_type == 255` is the empty-slot sentinel.

## 6. Text item records (ITEMS.*) — high confidence on column meaning, names uncertain

Each record is CR/LF-terminated ASCII with 12 tokens:

```
[type] [class] [NAME] [ac_or_kind] [ac_bonus] [damage_or_power] [price]
       [flag0] [flag1] [flag2] [flag3] [flag4]
```

Examples:

```
6 6 SWORD 0 0 2 20 0 0 0 0 0
2 1 SWORD_+_1 0 0 4 15 0 0 0 0 0
6 6 SHIELD 3 -1 0 20 0 0 0 0 -1
6 6 LEATHER 1 -1 0 20 0 0 0 0 0
6 6 TORCH 14 0 0 50 1 0 0 0 0
6 6 MAIL 9 -1 0 20 0 0 0 0 0
2 1 PLATE 10 -3 0 25 0 0 0 0 -3
```

Confirmed patterns:
- Token 1 (`type`) and token 2 (`class`) are small integers.
- Token 3 is a 8.3-ish uppercase name with `_` for spaces (`SWORD_+_1`, `ROD_OF_MIGHT`).
- Token 4 looks like an armor/item-kind code: `0` weapon, `1` leather, `3` shield,
  `4` mace, `7` war hammer, `8` scroll, `9` mail, `10` plate, `14` torch, `15` staff/rod/candle,
  `16` charm/amulet, `17` ring, `21` cape, `22` boots.
- Token 5 is AC modifier (`-1`, `-2`, `-3`, ...) for armor.
- Token 6 is weapon damage / power bonus (`2`,`4`,`5`,`6`,...).
- Token 7 is price/value (`20`,`15`,`10`,`25`,`50`,...).
- Tokens 8-12 are bit flags / special powers. Observed values include `1` (light),
  `2` (healing), `8`, `16`, `32`, `64`, `128`, `256`, `512`, `1024`, `2048`.
- Token 12 often repeats the AC modifier for armor (`-1`, `-3`) — possibly an
  "identified/disclosed" bonus. **Uncertain.**

`LEVEL.10.txt` and `LEVEL.4/5/8.txt` contain long runs of these item records plus
human-readable item-name lists (`Dragonslayer`, `Power Axe`, `Cloak of Godhood`,
`Sword of Godhood`, `Wizards Staff`, `Sphere of Might`, `Ring of Destruction`,
`Vorpal Blade`, ...). These are likely the "artifact" / special-item catalog.

## 7. ARAGORN.CHR (partially confirmed)

640 bytes. Starts:

```
4c 45 56 45 4c 2e 00 74 ...  ("LEVEL.\0t file  |  aragorn\0E a file  |  Hobbit\0SpePeasant\0")
```

then `a0 00 30 30 30 ... 00117 ...` and later mixed stat-like bytes and text fragments.
It appears to be a character save file with a header/name area and `PERSON`-like stats,
but the exact struct offsets were not recovered before wrap-up. **Uncertain.**

## 8. Confirmed vs uncertain summary

Confirmed:
- LEVEL/CASTLE files contain a 16-byte monster-instance table, a monster-name
  string table, 39-byte binary item records, and a 572-cell × 3-byte map.
- L0-L7 are the standalone map blobs (same 1716-byte cell array).
- MONSTERS.*/ITEMS.* are CR/LF ASCII tables with 0x1a CP/M EOF padding.
- C source fragments expose PERSON.stat indices, xclass/xtype factor usage,
  spell-class sets, i_type sentinel 255, and character init rules.

Uncertain:
- Exact grid orientation (26×22 vs 22×26) and exact feature-bit→name mapping.
- Meaning of item text tokens 1/2/4 vs binary 39-byte item struct fields.
- Meaning of item text token 12 and some special-power bit values.
- Exact ARAGORN.CHR field offsets.
- `MAST.CAT` / `MCAT505.DQC` layouts (files unavailable).
- Z80 executable details: only strings and fragments were used; decompilations are unusable.

## 9. Hex dumps (first and last 256 bytes of representative file types)

### LEVEL.0 (7168 bytes)

First 256 bytes:

```
0000: 22 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  "...............
0010: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0020: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0030: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0040: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0050: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0060: 00 00 10 00 0c 03 01 07 03 0f 09 06 00 03 00 00  ................
0070: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0080: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0090: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00a0: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00b0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00c0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00d0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00e0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00f0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
```

Last 256 bytes:

```
1b00: f0 00 00 f0 00 00 f0 04 00 f0 00 00 30 00 00 30  ............0..0
1b10: 00 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00  ..0..0..0..0..0.
1b20: 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00 00  .0..0..0..0..0..
1b30: 30 00 00 f0 00 00 3a 2f 2a 20 6d 61 63 72 6f 20  0.....:/* macro 
1b40: 64 65 66 69 6e 65 20 2a 2f 0d 0a 09 09 09 66 6f  define */.....fo
1b50: 72 28 69 3d 30 3b 20 69 3c 34 3b 20 2b 2b 69 29  r(i=0; i<4; ++i)
1b60: 0d 0a 09 09 09 09 69 66 28 28 28 63 68 3d 77 61  ......if(((ch=wa
1b70: 69 74 6b 65 79 28 29 29 20 3e 3d 20 27 30 27 20  itkey()) >= '0' 
1b80: 29 0d 00 00 00 00 01 01 01 01 02 02 02 02 03 03  )...............
1b90: 03 03 04 04 04 04 05 05 05 05 06 06 06 06 07 07  ................
1ba0: 07 07 08 08 08 08 09 09 09 09 0a 0a 0a 0a 0b 0b  ................
1bb0: 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0c 0c 0c  ................
1bc0: 0c 0d 0d 0d 0d 0e 0e 0e 0e 0f 0f 0f 0f 10 11 11  ................
1bd0: 11 11 12 13 14 2a 67 65 74 73 28 29 3b 0d 0a 0d  .....*gets();...
1be0: 0a 09 64 6f 0d 0a 52 00 0d 0a 09 09 74 61 62 28  ..do..R.....tab(
1bf0: 32 2c 30 29 3b 20 65 6f 6c 28 29 3b 0d 0a 09 09  2,0); eol();....
```

### LEVEL.1 (7168 bytes)

First 256 bytes:

```
0000: 22 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  "...............
0010: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0020: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0030: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0040: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0050: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0060: 00 00 10 00 0c 03 01 07 03 0f 09 06 00 03 00 00  ................
0070: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0080: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0090: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00a0: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00b0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00c0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00d0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00e0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00f0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
```

Last 256 bytes:

```
1b00: 30 00 00 30 00 00 30 00 00 38 00 00 30 00 00 30  0..0..0..8..0..0
1b10: 00 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00  ..0..0..0..0..0.
1b20: 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00 00  .0..0..0..0..0..
1b30: 30 00 00 f0 00 00 3a 2f 2a 20 6d 61 63 72 6f 20  0.....:/* macro 
1b40: 64 65 66 69 6e 65 20 2a 2f 0d 0a 09 09 09 66 6f  define */.....fo
1b50: 72 28 69 3d 30 3b 20 69 3c 34 3b 20 2b 2b 69 29  r(i=0; i<4; ++i)
1b60: 0d 0a 09 09 09 09 69 66 28 28 28 63 68 3d 77 61  ......if(((ch=wa
1b70: 69 74 6b 65 79 28 29 29 20 3e 3d 20 27 30 27 20  itkey()) >= '0' 
1b80: 29 0d 00 00 00 00 00 00 01 01 02 03 03 03 03 03  )...............
1b90: 03 04 04 05 06 06 06 06 06 06 07 07 08 09 09 09  ................
1ba0: 09 09 09 0a 0a 0a 0a 0a 0a 0b 0b 0c 0d 0d 0d 0d  ................
1bb0: 0d 0d 0e 0e 0f 10 10 0b 0b 0b 0b 0b 0b 0c 0c 0c  ................
1bc0: 0c 0d 0d 0d 0d 0e 0e 0e 0e 0f 0f 0f 0f 10 11 11  ................
1bd0: 11 11 12 13 14 1a 1a 1a 1b 1b 1c 1c 1d 1d 1e 1e  ................
1be0: 1e 1e 1f 1f 63 00 34 00 20 20 62 72 65 61 6b 3b  ....c.4.  break;
1bf0: 0d 0a 09 09 09 63 61 73 65 20 27 43 27 3a 20 63  .....case 'C': c
```

### LEVEL.10 (7168 bytes)

First 256 bytes:

```
0000: 57 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  W...-.....d.....
0010: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0020: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0030: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0040: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0050: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0060: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0070: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0080: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
0090: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00a0: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00b0: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00c0: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00d0: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00e0: 00 00 00 00 2d 02 01 c8 96 c8 64 f2 ff 1e 00 00  ....-.....d.....
00f0: 00 00 07 00 32 01 01 2c fa 2c c8 e8 ff 28 00 00  ....2..,.,...(..
```

Last 256 bytes:

```
1b00: b0 00 00 f0 00 00 30 00 00 30 00 00 30 00 00 30  ......0..0..0..0
1b10: 00 00 30 00 00 30 00 00 b0 00 00 30 00 00 30 00  ..0..0.....0..0.
1b20: 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00 00  .0..0..0..0..0..
1b30: 30 00 00 f0 00 00 42 39 45 20 20 74 6f 75 70 70  0.....B9E  toupp
1b40: 65 72 0d 0a 20 34 43 32 38 20 20 7a 7a 7a 65 6e  er.. 4C28  zzzen
1b50: 64 0d 0a 20 34 43 33 32 20 20 7a 7a 7a 30 0d 0a  d.. 4C32  zzz0..
1b60: 20 34 43 34 34 20 20 7a 72 65 61 64 0d 0a 20 34   4C44  zread.. 4
1b70: 43 34 43 20 20 7a 77 72 69 74 65 0d 0a 20 34 43  C4C  zwrite.. 4C
1b80: 35 34 00 00 00 00 00 01 01 01 01 01 01 01 01 01  54..............
1b90: 01 01 01 01 01 01 02 02 03 03 04 04 05 05 06 06  ................
1ba0: 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 08  ................
1bb0: 09 0a 0a 0a 0a 0a 0b 0c 23 24 25 26 27 28 29 29  ........#$%&'())
1bc0: 2a 2a 2b 2b 2c 2d 2d 2e 2e 2f 30 31 17 17 18 18  **++,--../01....
1bd0: 18 18 19 19 19 19 1a 1a 1a 1a 1b 1b 1c 1c 1d 1d  ................
1be0: 1e 1e 1e 1e 1f 1f 35 00 20 20 62 72 65 61 6b 3b  ......5.  break;
1bf0: 0d 0a 09 09 09 63 61 73 65 20 27 43 27 3a 20 63  .....case 'C': c
```

### CASTLE.0 (7168 bytes)

First 256 bytes:

```
0000: 22 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  "...............
0010: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0020: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0030: 00 00 00 00 0a 05 01 05 01 0d 05 08 00 01 00 00  ................
0040: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0050: 00 00 08 00 0b 04 01 06 02 0f 07 07 00 02 00 00  ................
0060: 00 00 10 00 0c 03 01 07 03 0f 09 06 00 03 00 00  ................
0070: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0080: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
0090: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00a0: 00 00 19 00 0a 0a 05 01 01 02 01 0a 00 01 00 00  ................
00b0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00c0: 00 00 1d 00 0a 03 01 03 01 0f 0a 07 00 02 04 00  ................
00d0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00e0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
00f0: 00 00 2b 00 09 05 01 03 01 05 01 0a 00 01 00 00  ..+.............
```

Last 256 bytes:

```
1b00: f0 00 00 f0 00 00 f0 04 00 f0 00 00 30 00 00 30  ............0..0
1b10: 00 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00  ..0..0..0..0..0.
1b20: 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00 00  .0..0..0..0..0..
1b30: 30 00 00 f0 00 00 3a 2f 2a 20 6d 61 63 72 6f 20  0.....:/* macro 
1b40: 64 65 66 69 6e 65 20 2a 2f 0d 0a 09 09 09 66 6f  define */.....fo
1b50: 72 28 69 3d 30 3b 20 69 3c 34 3b 20 2b 2b 69 29  r(i=0; i<4; ++i)
1b60: 0d 0a 09 09 09 09 69 66 28 28 28 63 68 3d 77 61  ......if(((ch=wa
1b70: 69 74 6b 65 79 28 29 29 20 3e 3d 20 27 30 27 20  itkey()) >= '0' 
1b80: 29 0d 00 00 00 00 01 01 01 01 02 02 02 02 03 03  )...............
1b90: 03 03 04 04 04 04 05 05 05 05 06 06 06 06 07 07  ................
1ba0: 07 07 08 08 08 08 09 09 09 09 0a 0a 0a 0a 0b 0b  ................
1bb0: 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0b 0c 0c 0c  ................
1bc0: 0c 0d 0d 0d 0d 0e 0e 0e 0e 0f 0f 0f 0f 10 11 11  ................
1bd0: 11 11 12 13 14 bc 00 c2 c3 f7 9b f7 f7 f7 f7 95  ................
1be0: fa 65 00 f7 9b f7 52 00 00 c2 f8 0e 00 c7 f8 0a  .e....R.........
1bf0: 00 c3 c6 f9 c3 00 c2 c3 c6 f7 f9 c4 00 c2 fd 0c  ................
```

### MONSTERS.0-1 (512 bytes)

First 256 bytes:

```
0000: 34 20 56 65 74 65 72 61 6e 20 31 30 20 35 20 31  4 Veteran 10 5 1
0010: 20 35 20 31 20 31 33 20 35 20 38 20 31 20 30 20   5 1 13 5 8 1 0 
0020: 30 0d 0a 32 20 57 61 72 72 69 6f 72 20 31 31 20  0..2 Warrior 11 
0030: 34 20 31 20 36 20 32 20 31 35 20 37 20 37 20 32  4 1 6 2 15 7 7 2
0040: 20 30 20 30 0d 0a 31 20 53 77 6f 72 64 6d 61 6e   0 0..1 Swordman
0050: 20 31 32 20 33 20 31 20 37 20 33 20 31 35 20 39   12 3 1 7 3 15 9
0060: 20 36 20 33 20 30 20 30 0d 0a 34 20 4f 72 63 20   6 3 0 0..4 Orc 
0070: 31 30 20 31 30 20 35 20 31 20 31 20 32 20 31 20  10 10 5 1 1 2 1 
0080: 31 30 20 31 20 30 20 30 0d 0a 32 20 50 73 65 75  10 1 0 0..2 Pseu
0090: 64 6f 2d 44 72 61 67 6f 6e 20 31 30 20 33 20 31  do-Dragon 10 3 1
00a0: 20 33 20 31 20 31 35 20 31 30 20 37 20 32 20 34   3 1 15 10 7 2 4
00b0: 20 30 0d 0a 34 20 4d 65 64 69 75 6d 20 39 20 35   0..4 Medium 9 5
00c0: 20 31 20 33 20 31 20 35 20 31 20 31 30 20 31 20   1 3 1 5 1 10 1 
00d0: 30 20 30 0d 0a 32 20 53 65 65 72 20 31 30 20 35  0 0..2 Seer 10 5
00e0: 20 31 20 34 20 31 20 37 20 31 20 39 20 32 20 38   1 4 1 7 1 9 2 8
00f0: 20 30 0d 0a 31 20 43 6f 6e 6a 75 72 65 72 20 31   0..1 Conjurer 1
```

Last 256 bytes:

```
0100: 31 20 34 20 31 20 35 20 31 20 38 20 32 20 39 20  1 4 1 5 1 8 2 9 
0110: 33 20 38 20 30 0d 0a 34 20 41 63 6f 6c 79 74 65  3 8 0..4 Acolyte
0120: 20 39 20 35 20 31 20 33 20 31 20 35 20 31 20 31   9 5 1 3 1 5 1 1
0130: 30 20 31 20 30 20 30 0d 0a 32 20 41 64 65 70 74  0 1 0 0..2 Adept
0140: 20 31 30 20 35 20 31 20 34 20 31 20 37 20 31 20   10 5 1 4 1 7 1 
0150: 39 20 32 20 31 36 20 30 0d 0a 31 20 50 72 69 65  9 2 16 0..1 Prie
0160: 73 74 20 31 31 20 34 20 31 20 35 20 31 20 38 20  st 11 4 1 5 1 8 
0170: 32 20 39 20 33 20 31 36 20 30 0d 0a 34 20 41 70  2 9 3 16 0..4 Ap
0180: 70 72 65 6e 74 69 63 65 20 39 20 35 20 31 20 33  prentice 9 5 1 3
0190: 20 31 20 35 20 31 20 31 30 20 31 20 30 20 30 0d   1 5 1 10 1 0 0.
01a0: 0a 32 20 46 6f 6f 74 70 61 64 20 31 30 20 35 20  .2 Footpad 10 5 
01b0: 31 20 34 20 31 20 37 20 31 20 39 20 32 20 30 20  1 4 1 7 1 9 2 0 
01c0: 30 0d 0a 31 20 52 6f 62 62 65 72 20 31 31 20 34  0..1 Robber 11 4
01d0: 20 31 20 35 20 31 20 38 20 32 20 39 20 33 20 30   1 5 1 8 2 9 3 0
01e0: 20 30 0d 0a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a   0..............
01f0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
```

### MONSTERS.4-5 (896 bytes)

First 256 bytes:

```
0000: 34 20 57 65 72 65 63 61 74 20 31 36 20 35 20 32  4 Werecat 16 5 2
0010: 20 37 20 35 20 31 32 20 35 20 35 20 34 20 30 20   7 5 12 5 5 4 0 
0020: 30 0d 0a 34 20 4f 67 72 65 5f 6d 61 67 65 20 31  0..4 Ogre_mage 1
0030: 38 20 32 20 31 20 31 32 20 35 20 32 30 20 31 32  8 2 1 12 5 20 12
0040: 20 34 20 34 20 30 20 30 0d 0a 34 20 57 69 67 68   4 4 0 0..4 Wigh
0050: 74 20 31 34 20 31 20 31 20 32 30 20 31 35 20 35  t 14 1 1 20 15 5
0060: 20 31 20 34 20 34 20 31 20 30 0d 0a 34 20 42 6c   1 4 4 1 0..4 Bl
0070: 69 6e 6b 5f 64 6f 67 20 31 33 20 32 20 31 20 31  ink_dog 13 2 1 1
0080: 32 20 38 20 38 20 34 20 35 20 34 20 30 20 30 0d  2 8 8 4 5 4 0 0.
0090: 0a 34 20 47 68 6f 75 6c 20 31 33 20 33 20 31 20  .4 Ghoul 13 3 1 
00a0: 32 30 20 31 30 20 38 20 33 20 34 20 34 20 31 20  20 10 8 3 4 4 1 
00b0: 30 0d 0a 33 20 42 61 73 69 6c 69 73 6b 20 31 32  0..3 Basilisk 12
00c0: 20 34 20 32 20 35 20 31 20 31 35 20 31 30 20 2d   4 2 5 1 15 10 -
00d0: 31 20 35 20 30 20 30 0d 0a 33 20 43 6f 63 6b 61  1 5 0 0..3 Cocka
00e0: 74 72 69 63 65 20 31 39 20 32 20 32 20 31 30 20  trice 19 2 2 10 
00f0: 35 20 32 30 20 31 35 20 2d 31 20 35 20 30 20 30  5 20 15 -1 5 0 0
```

Last 256 bytes:

```
0280: 20 31 37 20 36 20 32 20 31 30 20 35 20 31 35 20   17 6 2 10 5 15 
0290: 37 20 35 20 35 20 30 20 30 0d 0a 32 20 57 72 61  7 5 5 0 0..2 Wra
02a0: 69 74 68 20 31 35 20 31 20 31 20 32 30 20 31 35  ith 15 1 1 20 15
02b0: 20 36 20 32 20 34 20 36 20 31 20 30 0d 0a 31 20   6 2 4 6 1 0..1 
02c0: 56 69 63 61 72 20 31 37 20 35 20 33 20 32 30 20  Vicar 17 5 3 20 
02d0: 31 35 20 35 30 20 33 30 20 30 20 37 20 31 36 20  15 50 30 0 7 16 
02e0: 30 0d 0a 31 20 57 79 76 65 72 6e 20 31 34 20 32  0..1 Wyvern 14 2
02f0: 20 31 20 32 30 20 31 35 20 35 30 20 33 30 20 2d   1 20 15 50 30 -
0300: 33 20 37 20 34 20 30 0d 0a 31 20 53 75 63 63 75  3 7 4 0..1 Succu
0310: 62 69 20 31 35 20 33 20 31 20 31 38 20 31 35 20  bi 15 3 1 18 15 
0320: 33 30 20 32 35 20 2d 31 20 37 20 32 35 20 30 0d  30 25 -1 7 25 0.
0330: 0a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0340: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0350: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0360: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0370: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
```

### ITEMS.1-2 (640 bytes)

First 256 bytes:

```
0000: 36 20 36 20 53 57 4f 52 44 20 30 20 30 20 32 20  6 6 SWORD 0 0 2 
0010: 32 30 20 30 20 30 20 30 20 30 20 30 0d 0a 32 20  20 0 0 0 0 0..2 
0020: 31 20 53 57 4f 52 44 5f 2b 5f 31 20 30 20 30 20  1 SWORD_+_1 0 0 
0030: 34 20 31 35 20 30 20 30 20 30 20 30 20 30 0d 0a  4 15 0 0 0 0 0..
0040: 31 20 31 20 53 57 4f 52 44 5f 2b 5f 32 20 30 20  1 1 SWORD_+_2 0 
0050: 30 20 35 20 31 30 20 30 20 30 20 30 20 30 20 30  0 5 10 0 0 0 0 0
0060: 0d 0a 36 20 36 20 53 48 49 45 4c 44 20 33 20 2d  ..6 6 SHIELD 3 -
0070: 31 20 30 20 32 30 20 30 20 30 20 30 20 30 20 2d  1 0 20 0 0 0 0 -
0080: 31 0d 0a 32 20 31 20 53 48 49 45 4c 44 5f 2b 5f  1..2 1 SHIELD_+_
0090: 31 20 33 20 2d 32 20 30 20 31 35 20 30 20 30 20  1 3 -2 0 15 0 0 
00a0: 30 20 30 20 2d 31 0d 0a 31 20 31 20 53 48 49 45  0 0 -1..1 1 SHIE
00b0: 4c 44 5f 2b 5f 32 20 33 20 2d 33 20 30 20 31 30  LD_+_2 3 -3 0 10
00c0: 20 30 20 30 20 30 20 30 20 2d 31 0d 0a 36 20 36   0 0 0 0 -1..6 6
00d0: 20 4c 45 41 54 48 45 52 20 31 20 2d 31 20 30 20   LEATHER 1 -1 0 
00e0: 32 30 20 30 20 30 20 30 20 30 20 30 0d 0a 32 20  20 0 0 0 0 0..2 
00f0: 31 20 4c 45 41 54 48 45 52 5f 2b 5f 31 20 31 20  1 LEATHER_+_1 1 
```

Last 256 bytes:

```
0180: 20 34 20 30 20 34 20 31 35 20 30 20 30 20 30 20   4 0 4 15 0 0 0 
0190: 30 20 30 0d 0a 31 20 31 20 4d 41 43 45 5f 2b 5f  0 0..1 1 MACE_+_
01a0: 32 20 34 20 30 20 35 20 31 30 20 30 20 30 20 30  2 4 0 5 10 0 0 0
01b0: 20 30 20 30 0d 0a 36 20 36 20 4d 41 49 4c 20 39   0 0..6 6 MAIL 9
01c0: 20 2d 31 20 30 20 32 30 20 30 20 30 20 30 20 30   -1 0 20 0 0 0 0
01d0: 20 30 0d 0a 32 20 31 20 4d 41 49 4c 5f 2b 5f 31   0..2 1 MAIL_+_1
01e0: 20 39 20 2d 32 20 30 20 31 35 20 30 20 30 20 30   9 -2 0 15 0 0 0
01f0: 20 30 20 30 0d 0a 31 20 31 20 4d 41 49 4c 5f 2b   0 0..1 1 MAIL_+
0200: 5f 32 20 39 20 2d 33 20 30 20 31 30 20 30 20 30  _2 9 -3 0 10 0 0
0210: 20 30 20 30 20 30 0d 0a 32 20 31 20 50 4c 41 54   0 0 0..2 1 PLAT
0220: 45 20 31 30 20 2d 33 20 30 20 32 35 20 30 20 30  E 10 -3 0 25 0 0
0230: 20 30 20 30 20 2d 33 0d 0a 1a 1a 1a 1a 1a 1a 1a   0 0 -3.........
0240: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0250: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0260: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0270: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
```

### ITEMS.6-7 (3584 bytes)

First 256 bytes:

```
0000: 34 20 32 20 53 57 4f 52 44 5f 2d 5f 31 20 30 20  4 2 SWORD_-_1 0 
0010: 30 20 2d 31 20 31 30 20 30 20 30 20 2d 31 0d 0a  0 -1 10 0 0 -1..
0020: 31 20 31 20 4d 41 47 49 43 5f 53 57 4f 52 44 5f  1 1 MAGIC_SWORD_
0030: 2b 5f 31 20 30 20 30 20 33 20 35 20 32 20 30 20  +_1 0 0 3 5 2 0 
0040: 2d 31 0d 0a 31 20 31 20 4d 41 47 49 43 5f 53 57  -1..1 1 MAGIC_SW
0050: 4f 52 44 5f 2b 33 20 30 20 2d 31 20 34 20 31 30  ORD_+3 0 -1 4 10
0060: 20 32 20 30 20 30 20 30 20 2d 34 0d 0a 32 20 32   2 0 0 0 -4..2 2
0070: 20 53 57 4f 52 44 5f 2b 5f 34 20 30 20 30 20 36   SWORD_+_4 0 0 6
0080: 20 31 30 20 30 20 30 20 30 20 30 20 2d 33 0d 0a   10 0 0 0 0 -3..
0090: 31 20 31 20 4d 41 47 49 43 5f 53 57 4f 52 44 5f  1 1 MAGIC_SWORD_
00a0: 2b 5f 31 20 30 20 30 20 33 20 35 20 32 20 30 20  +_1 0 0 3 5 2 0 
00b0: 2d 31 0d 0a 31 20 31 20 53 57 4f 52 44 5f 2b 5f  -1..1 1 SWORD_+_
00c0: 32 20 32 20 30 20 34 20 31 30 20 30 20 30 20 2d  2 2 0 4 10 0 0 -
00d0: 31 0d 0a 31 20 31 20 53 57 4f 52 44 5f 2b 5f 33  1..1 1 SWORD_+_3
00e0: 20 30 20 30 20 36 20 31 30 20 30 20 30 20 30 20   0 0 6 10 0 0 0 
00f0: 30 20 30 0d 0a 31 20 31 20 53 48 49 45 4c 44 5f  0 0..1 1 SHIELD_
```

Last 256 bytes:

```
0d00: 30 20 2d 31 0d 0a 32 20 31 20 48 45 4c 4d 45 54  0 -1..2 1 HELMET
0d10: 5f 2b 5f 31 20 36 20 2d 32 20 30 20 32 30 20 30  _+_1 6 -2 0 20 0
0d20: 20 30 20 30 20 30 20 2d 32 0d 0a 31 20 31 20 48   0 0 0 -2..1 1 H
0d30: 45 4c 4d 45 54 5f 2b 5f 32 20 36 20 2d 33 20 30  ELMET_+_2 6 -3 0
0d40: 20 31 35 20 30 20 30 20 30 20 30 20 2d 33 0d 0a   15 0 0 0 0 -3..
0d50: 31 20 31 20 4c 4f 55 44 5f 42 4f 4f 54 53 20 32  1 1 LOUD_BOOTS 2
0d60: 32 20 30 20 30 20 35 30 20 30 20 30 20 30 20 30  2 0 0 50 0 0 0 0
0d70: 20 2d 35 0d 0a 31 20 31 20 53 4f 46 54 5f 42 4f   -5..1 1 SOFT_BO
0d80: 4f 54 53 20 32 32 20 30 20 30 20 35 30 20 30 20  OTS 22 0 0 50 0 
0d90: 30 20 30 20 30 20 35 0d 0a 1a 1a 1a 1a 1a 1a 1a  0 0 0 5.........
0da0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0db0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0dc0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0dd0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0de0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
0df0: 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a 1a  ................
```

### L0 (1792 bytes)

First 256 bytes:

```
0000: 50 00 00 70 00 00 70 00 00 70 00 00 70 00 00 70  P..p..p..p..p..p
0010: 00 00 70 00 00 70 00 00 70 00 00 70 00 00 d0 00  ..p..p..p..p....
0020: 00 c0 00 00 d0 00 00 d0 00 00 d0 00 00 d0 00 00  ................
0030: d0 00 00 d0 00 00 d0 00 00 d0 00 00 d0 00 00 d0  ................
0040: 00 00 d0 00 00 d0 00 00 d0 00 00 d0 02 04 d0 00  ................
0050: 00 d0 00 00 40 00 00 40 00 00 40 00 00 40 00 00  ....@..@..@..@..
0060: 40 00 00 40 00 00 40 00 00 c0 00 00 c0 00 00 00  @..@..@.........
0070: 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
0080: 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00  ..........0.....
0090: f0 00 00 00 00 00 40 00 00 f0 00 00 d0 00 00 d0  ......@.........
00a0: 00 00 f0 00 00 f0 00 00 f0 00 00 00 00 00 f0 00  ................
00b0: 00 f0 00 00 f0 00 00 f0 00 00 c0 00 00 40 00 00  .............@..
00c0: 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  P...............
00d0: 00 00 00 00 00 f0 00 00 70 00 00 c0 00 00 00 00  ........p.......
00e0: 01 c0 00 01 40 00 00 f0 00 00 d0 00 00 d0 00 00  ....@...........
00f0: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
```

Last 256 bytes:

```
0600: c0 00 00 c0 00 00 00 00 00 c0 00 00 c0 00 00 00  ................
0610: 00 00 c0 00 00 c0 00 00 c0 00 00 c0 00 00 00 00  ................
0620: 00 00 00 00 10 00 00 10 00 00 10 00 00 10 00 00  ................
0630: 10 00 00 10 00 00 c0 00 00 c0 00 00 c0 00 00 30  ...............0
0640: 00 00 70 00 00 f0 00 00 30 00 00 30 00 00 f0 00  ..p.....0..0....
0650: 00 f0 00 00 30 00 00 f0 00 00 30 00 00 30 00 00  ....0.....0..0..
0660: f0 00 00 c0 00 00 f0 02 10 70 00 00 f0 01 00 f0  .........p......
0670: 00 00 f0 00 00 f0 00 00 f0 00 00 f0 00 00 f0 00  ................
0680: 00 f0 00 00 f0 04 00 f0 00 00 30 00 00 30 00 00  ..........0..0..
0690: 30 00 00 30 00 00 30 00 00 30 00 00 30 00 00 30  0..0..0..0..0..0
06a0: 00 00 30 00 00 30 00 00 30 00 00 30 00 00 30 00  ..0..0..0..0..0.
06b0: 00 f0 00 00 3a 2f 2a 20 6d 61 63 72 6f 20 64 65  ....:/* macro de
06c0: 66 69 6e 65 20 2a 2f 0d 0a 09 09 09 66 6f 72 28  fine */.....for(
06d0: 69 3d 30 3b 20 69 3c 34 3b 20 2b 2b 69 29 0d 0a  i=0; i<4; ++i)..
06e0: 09 09 09 09 69 66 28 28 28 63 68 3d 77 61 69 74  ....if(((ch=wait
06f0: 6b 65 79 28 29 29 20 3e 3d 20 27 30 27 20 29 0d  key()) >= '0' ).
```

### L7 (1792 bytes)

First 256 bytes:

```
0000: 00 00 01 30 00 00 30 00 00 30 00 00 30 00 00 30  ...0..0..0..0..0
0010: 00 01 30 00 00 30 00 00 30 02 08 30 00 00 30 00  ..0..0..0..0..0.
0020: 00 c0 00 00 00 00 00 10 00 00 10 00 00 10 00 00  ................
0030: 10 00 00 10 00 00 10 00 00 10 00 00 10 00 00 10  ................
0040: 00 00 10 00 00 10 00 00 10 00 00 d0 00 00 c0 00  ................
0050: 00 00 00 01 30 00 01 30 00 01 30 00 01 30 00 01  ....0..0..0..0..
0060: 30 00 01 30 00 01 30 00 01 30 00 01 c0 00 01 c0  0..0..0..0......
0070: 80 00 c0 00 00 f0 80 00 f0 02 14 f0 80 00 f0 00  ................
0080: 01 f0 10 00 f0 00 01 f0 80 00 d0 08 00 c0 00 00  ................
0090: f0 00 01 f0 00 01 f0 00 01 f0 04 00 c0 00 00 c0  ................
00a0: 10 00 00 00 01 30 00 01 30 00 01 30 00 01 30 00  .....0..0..0..0.
00b0: 01 30 00 01 30 00 01 c0 00 01 c0 00 01 c0 00 00  .0..0...........
00c0: c0 00 00 d0 00 00 d0 00 00 d0 00 00 d0 00 00 d0  ................
00d0: 00 02 d0 00 00 d0 00 00 d0 00 02 c0 00 00 00 00  ................
00e0: 00 00 00 00 00 00 00 c0 00 00 c0 00 00 c0 20 00  .............. .
00f0: c0 00 01 00 10 00 30 00 01 30 00 01 30 00 01 30  ......0..0..0..0
```

Last 256 bytes:

```
0600: c0 00 00 c0 00 00 c0 00 00 c0 00 00 c0 00 00 c0  ................
0610: 00 00 c0 00 00 c0 00 00 e0 00 01 e0 80 00 e0 02  ................
0620: 28 e0 00 00 e0 00 00 e0 30 00 e0 00 00 00 00 00  (.......0.......
0630: 00 00 00 00 00 00 00 00 00 c0 00 00 c0 00 00 c0  ................
0640: 00 00 c0 00 00 c0 00 00 c0 00 00 c0 00 00 c0 00  ................
0650: 01 c0 00 00 c0 00 00 c0 00 00 c0 00 00 c0 00 00  ................
0660: c0 00 00 c0 00 00 30 00 00 30 00 00 30 00 00 30  ......0..0..0..0
0670: 00 00 30 00 00 30 00 00 b0 00 00 30 00 00 30 00  ..0..0.....0..0.
0680: 00 30 00 00 f0 00 00 30 00 00 f0 00 00 f0 00 02  .0.....0........
0690: f0 00 00 f0 00 00 30 00 00 f0 00 00 30 00 00 f0  ......0.....0...
06a0: 00 00 30 00 00 f0 00 00 30 00 00 f0 00 00 b0 10  ..0.....0.......
06b0: 01 f0 00 00 80 20 cd c9 20 8d 80 20 cd c9 20 8d  ..... .. .. .. .
06c0: 80 20 cd c9 20 8d 80 20 cd c9 20 8d 80 20 cd c9  . .. .. .. .. ..
06d0: 20 8d 80 20 cd c9 20 8d 80 20 cd c9 20 8d 80 20   .. .. .. .. .. 
06e0: cd c9 20 8d 80 20 cd c9 20 8d 80 20 cd c9 20 8d  .. .. .. .. .. .
06f0: 80 20 cd c9 20 8d 80 20 cd c9 20 8d 80 20 cd c9  . .. .. .. .. ..
```

### ARAGORN.CHR (640 bytes)

First 256 bytes:

```
0000: 4c 45 56 45 4c 2e 00 74 20 66 69 6c 65 20 20 7c  LEVEL..t file  |
0010: 20 20 61 72 61 67 6f 72 6e 00 45 20 61 20 66 69    aragorn.E a fi
0020: 6c 65 20 20 7c 20 20 48 6f 62 62 69 74 00 53 70  le  |  Hobbit.Sp
0030: 65 50 65 61 73 61 6e 74 00 a0 00 30 30 30 30 30  ePeasant...00000
0040: 30 30 31 31 37 00 30 30 30 30 30 30 30 30 30 30  00117.0000000000
0050: 00 0c 00 0a 00 0b 00 0c 00 08 00 0f 00 00 00 10  ................
0060: 00 00 00 00 00 05 00 08 00 56 00 11 00 10 08 33  .........V.....3
0070: 00 ff ff ff ff ff ff 10 00 00 00 00 00 00 00 00  ................
0080: 00 00 00 10 00 00 00 0a 00 00 00 00 00 ff ff 00  ................
0090: 00 00 00 0a 00 00 00 00 00 00 00 00 00 00 00 00  ................
00a0: 00 00 00 6d 65 73 73 61 67 65 20 77 69 6c 6c 20  ...message will 
00b0: 63 6c 65 61 72 20 77 68 65 6e 20 61 20 6b 00 63  clear when a k.c
00c0: 6f 70 79 ff 62 3a 3d 70 65 72 73 6f 6e 2e 63 6f  opy.b:=person.co
00d0: 6d 0d 0a 63 6f 70 79 20 62 3a 3d 74 65 72 6d 64  m..copy b:=termd
00e0: 65 66 2e 63 6f 6d 0d 0a ff 6f 70 79 20 62 3a 3d  ef.com...opy b:=
00f0: 64 6d 2e 63 6f 6d 0d 0a 63 6f 70 79 20 62 3a 3d  dm.com..copy b:=
```

Last 256 bytes:

```
0180: 54 4f 4c 44 20 54 4f 2e 0f 8f cd 26 60 d8 3e ff  TOLD TO....&`.>.
0190: 32 ae 5e c9 cd 9d 0d 0f 0f 2a 2a 2a 2a 2a 2a 2a  2.^......*******
01a0: 2a ff 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  *.**************
01b0: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
01c0: 2a 2a 2a 2a 2a 2a ff 2a 2a 2a 2a 2a 2a 2a 2a 2a  ******.*********
01d0: 2a 2a 2a 2a 2a 2a 2a 0f 2a 2a 2a 2a 2a 2a 2a 2a  *******.********
01e0: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a ff 2a 2a 2a 2a  ***********.****
01f0: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
0200: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
0210: ff 2a 2a 2a 2a 2a 0f 59 4f 55 20 4d 41 59 20 4e  .*****.YOU MAY N
0220: 4f 57 20 52 45 50 4c 41 43 45 20 54 48 45 20 44  OW REPLACE THE D
0230: 49 53 4b 53 20 59 4f 55 20 52 45 4d 4f 56 45 44  ISKS YOU REMOVED
0240: 20 45 41 52 4c 45 52 2e 0f 2a 2a 2a 2a 2a 2a 2a   EARLER..*******
0250: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
0260: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
0270: 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a  ****************
```

