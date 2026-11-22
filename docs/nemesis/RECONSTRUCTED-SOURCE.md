# Nemesis (SuperSoft, 1985/86) — reconstructed original C source

**Working reconstruction for the C# port.** Reconstructed from literal C source
text embedded in `docs/nemesis/game-files/LEVEL.*` (and `CASTLE.0`, `L0`–`L7`),
from the fragment dumps in `/tmp/opencode/nemesis-source/ALL-SOURCE-FRAGMENTS.txt`,
and cross-checked against the Ghidra/Z80 material in `docs/nemesis/`.

Confidence legend:
- **[CONFIRMED]** — text is present verbatim in one or more LEVEL files.
- **[INFERRED]** — deduced from binary data, compiled `.COM` strings, or code context.
- **[UNCERTAIN]** — plausible reconstruction; missing bytes were overwritten by
  interleaved binary data in every surviving copy.

---

## 0. How the fragments are laid out (important for further work)

* `LEVEL.0` (7168 bytes) holds the tail of `main()`, `init_char()`,
  `get_dungeon()`, `get_class()`, `dis_val_classes()`, `valid()`, `get_ali()`,
  then (from ~offset 4000) `compute_stats()`, `display_stats()`, `get_name()`,
  `get_type()`.  The text begins at byte 546.
* `LEVEL.1` (identical in `LEVEL.2`, `LEVEL.3`) holds the tail of `get_ali()`
  at 2232, then `compute_stats()`, `display_stats()`, `get_name()`,
  `get_type()`, `init_char()` … through 7168.
* Overlapping source regions are at a constant displacement: a string found at
  offset `X` in `LEVEL.1` is at `X + 1616` in `LEVEL.0` (e.g.
  `display_stats()` = 2571 / 4187).  This is how the 2-byte holes in `LEVEL.0`
  (`<= \x00\x00 ? 1`) were filled from `LEVEL.1` (`<= 0 ? 1`).
* The source text is **overwritten by other data at a regular 39-byte stride**
  in `LEVEL.0` (37 good bytes + 2 overwritten bytes, repeating) and by an
  interleaved binary item table in `LEVEL.1` (~offset 3200 onward).  Each file
  therefore preserves different parts of the same text; merge them rather than
  trusting a single file.
* `LEVEL.4`/`5`/`6`/`10` contain item tables plus monster-name lists;
  `LEVEL.8` contains another item-table slice; `LEVEL.7` contains a *different*
  source file — the Z8000 `zstdio` runtime (`fill_buf`, `write_buf`,
  `toupper`, `#asm` stubs) and a small `zstdio.h` test program.
* `PERSON.COM` (offset ~6400–7600) stores literal **class names** and **type
  names** as NUL-separated strings, plus the compiled immediate constants of
  the `xclass`/`xtype` factor tables (pushed with Z80 `LD HL,nn; PUSH HL`
  sequences starting near 0x1E5C in the COM address space).

---

## 1. Recovered C functions

### 1.1 `main()` — PERSON.C **[CONFIRMED tail, header INFERRED]**

The first line is missing (it was overwritten by the level map at the head of
`LEVEL.0`).  `seed` is almost certainly `int`:

```c
main()
{
	int seed;                       /* INFERRED */

	for(seed=0; seed<CSTATS; ++seed)
		PERSON.stat[seed] = 0;

	getdef();
	clr();
	init_rand();
	get_name();
	get_dungeon();
	get_type();
	get_stats();
	get_ali();
	get_class();
	init_char();
	save_char();
}
```

### 1.2 `init_char()` **[CONFIRMED]**

```c
init_char()
{
	int t1,t;

	t = PERSON.stat[CLASS];
	if(t==4 || t==5 || t==9 || t==12)
		PERSON.p_mspell = 0xffff;
	else	PERSON.p_mspell = 0;
	if(t==0 || t==1 || t==7 || t==9 || t==10 || t==12)
		PERSON.p_cspell = 0xFFFF;
	else	PERSON.p_cspell = 0;

	PERSON.p_ac = 10;

	PERSON.stat[LVL] = 0;

	PERSON.stat[DLVL] =
	PERSON.stat[X] =
	PERSON.stat[Y] =
	PERSON.stat[HIPAGE] = -1;

	/* Eldar elf starts at 89 (why not) */
	PERSON.stat[AGE1] = PERSON.stat[TYPE] == 14 ? 89 : 16;
	PERSON.stat[O_HITS] = PERSON.stat[HITS] = 1 + rand() %
		(PERSON.stat[CON] + PERSON.stat[STR]);
	for(t1=0; t1<MAX_ITEMS; ++t1)
		PERSON.p_item[t1].i_type = 255;

	for(t1=0; t1<CARRY_OVER;++t1)
		PERSON.p_time[t1]=0;

	sprintf(PERSON.exp,NSTR,0);
	zfill(PERSON.exp);
}
```

Notes: magic-user classes (mspell) are 4,5,9,12; cleric-spell classes are
0,1,7,9,10,12. `TYPE == 14` is **Grey elf** (confirmed by the type-name table
in `PERSON.COM`), explaining the age 89. Item slot sentinel is `255`.

### 1.3 `get_dungeon()` **[PARTIAL]**

```c
get_dungeon()
{
#ifndef	PERSON2
	char semp[?];                   /* INFERRED */

	/* ... `strcpy(PERSON.p_dungeon, ...)` was overwritten by an embedded
	   class-name table in LEVEL.0 ... */
	do
	{
		tab(?,0);                   /* first arg overwritten */
		ol();
		puts("Enter the dungeon where this character lives: ");
	} while(!(*gets(semp)));
	make_filename(PERSON.p_dungeon,semp);
#endif
}
```

### 1.4 `get_class()` **[CONFIRMED]**

```c
get_class()
{
	int t1, t2;
	char temp[50];

	load_classes();
	dis_val_classes();
	do
	{
		tab(22,0);
		eol();
		puts("Pick a class : ");
		sscanf(gets(temp),"%d",&t1);
	} while(!valid(t1));

	PERSON.stat[CLASS] = t1;
	PERSON.stat[XMULT] = (PERSON.stat[XMULT] *
		xclass[t1].factor[XMULT]) / 200;
	PERSON.stat[AGEC] = (PERSON.stat[AGEC] *
		xclass[t1].factor[AGEC]) / 10;
	PERSON.stat[I_1] |= xclass[t1].factor[I_1];
	PERSON.stat[I_2] |= xclass[t1].factor[I_2];
	strcpy(PERSON.class,xclass[t1].xname);
}
```

### 1.5 `dis_val_classes()` **[CONFIRMED]**

```c
dis_val_classes()
{
	int t1, t2, t3;

	t2 = 0; t3 = 0;
	for(t1=0; t1 <= MAX_CLASS; ++t1)
		if(valid(t1))
		{
			tab(12+t2,20*t3);
			printf("%d)",t1);
			tab(12+t2,20*t3+4);
			puts(xclass[t1].xname);
			if(++t3 == 3)
			{ ++t2; t3 = 0; }
		}
}
```

Three columns of valid classes, 20 columns apart.

### 1.6 `valid(xx)` **[CONFIRMED — note the original bug]**

```c
valid(xx)
int	xx;
{
	int t1;

	for(t1=0; t1<6; ++t1)
		if(PERSON.stat[t1] < xclass[xx].factor[t1])
			return(FALSE);

	/* Test for a ranger being human */
	if((t1 == 9) && (PERSON.stat[TYPE] != 0))
		return(FALSE);
	if((PERSON.stat[ALI] & xclass[xx].factor[ALI] & 0x0007) == 0)
		return(FALSE);
	if((PERSON.stat[SEX] & xclass[xx].factor[SEX] & 0x0018) == 0)
		return(FALSE);

	return(TRUE);
}
```

**Bug preserved verbatim:** after the loop `t1 == 6`, so `if(t1 == 9)` can
never fire.  The author plainly meant `xx == 9` (Ranger).  The C# port should
decide whether to preserve or fix this.  Alignment eligibility uses the low
3 bits (`0x0007`); sex eligibility uses bits `0x08`/`0x10` (`0x0018`).

### 1.7 `get_ali()` **[PARTIAL]**

```c
get_ali()
{
	char c1;

	if(PERSON.stat[TYPE] == 13)
		c1 = C;                     /* Kobold(?) is always Chaotic */
	else do
	{
		tab(22,0);
		eol();
		puts("Enter alignment(L,N,C) : ");
		c1 = toupper(sgetchar());   /* INFERRED from "switch(to…” */
		switch(c1)
		{
			case 'L': c1 = L; break;
			case 'N': c1 = N; break;
			case 'C': c1 = C; break;
			default:  c1 = 0;  break;
		}
	} while(!c1);                   /* UNCERTAIN */
	PERSON.stat[ALI] |= c1;

	/* Tail recovered from LEVEL.1 offset 2232: */
	/* ... } while(toupper(sgetchar()) != 'Y');   — confirmation prompt */
	/* Persisted strings in PERSON.COM: "%s the ", "Lawful ",
	   "Neutral ", "Chaotic ", "Untouchable " */
}
```

Between the prompt and the switch, an embedded item table (`SWORD`, `MACE`,
`HELMET`, `MAGIC TORCH`, …) overwrote several statements. `TYPE == 13` is
**Kobold** per the compiled type-name list (index 13), so Kobolds get `C`.

### 1.8 `compute_stats()` **[CONFIRMED]**

```c
compute_stats()
{
	int t1, t2, t3, tot;

	tot=0;
	t3 = PERSON.stat[TYPE];
	for(t1=0; t1<6; ++t1)
		tot+=PERSON.stat[t1] = (t2 = roll() +
			(2 * xtype.factor[t1])) <= 0 ? 1 : t2;
	PERSON.stat[SEX] = (rand() % 2) ? M : F;
	sprintf(PERSON.gold,NSTR,rand()%(200-tot));
	zfill(PERSON.gold);
}
```

`t3` is assigned but never used in the surviving text — an earlier revision
in `LEVEL.0` reads `xtype[t3].factor[t1]`, so the table may originally have
been indexed by type.  For the port, use `xtype.factor[t1]` (six race
modifiers) and treat `t3` as dead.

### 1.9 `display_stats()` **[CONFIRMED]**

```c
display_stats()
{
	tab(3,0);
	puts(((PERSON.stat[SEX]&M)==M) ? "Male  " : "Female");
	tab(4,0);
	printf("STR  : %d   \n",PERSON.stat[STR]);
	printf("INT  : %d   \n",PERSON.stat[INT]);
	printf("WIS  : %d   \n",PERSON.stat[WIS]);
	printf("CHA  : %d   \n",PERSON.stat[CHA]);
	printf("CON  : %d   \n",PERSON.stat[CON]);
	printf("DEX  : %d   \n",PERSON.stat[DEX]);
	printf("GOLD : %d   \n",atoi(PERSON.gold));
}
```

(`LEVEL.0` has a variant `tab(?,0)` with the first argument overwritten.)

### 1.10 `get_name()` **[CONFIRMED — two minor variants]**

`LEVEL.1` version:

```c
get_name()
{
	char temp_name[50], filename[13], temp[50];
	int t1, t2, t3, fd;

	sleep(20);
	do
	{
		fd = -1;
		tab(0,0); eol();
		puts("What shall this character be called? ");
		sscanf(gets(temp),"%s",temp_name);
		if(valid_name(temp_name,filename))
		{
			tab(2,0);
			puts("This name is in use, would you like");
			puts(" to delete it ? ");
			if(toupper(sgetchar()) == 'Y')
				unlink(filename);
			else
				fd = 0;
			tab(2,0);
			eol();
		}
	} while(((t2 = strlen(temp_name)) == 0)
		|| (t2 > MAX_NAME) || (fd != -1));

	strcpy(PERSON.p_name,temp_name);
}
```

`LEVEL.0` variant adds `clr();` before `sleep(20);`; the `LEVEL.1` text shows
the same function without it.  Use the `LEVEL.1` version unless the GUI port
wants an explicit clear-screen.

### 1.11 `get_type()` **[INCOMPLETE]**

```c
get_type()
{
	int t1;
	char temp[50];

	/* ... :/* macro define * / ... */
	for(i=0; i<4; ++i)
		if(((ch=waitkey()) >= '0' ) ...)     /* overwritten */
	...
	/* *gets(); ... do { tab(2,0); eol(); ... } ... */
}
```

Only a menu-loop fragment survives.  Fifteen type names are confirmed from
`PERSON.COM` (section 4.3) and the selection is stored in `PERSON.stat[TYPE]`.

### 1.12 `fill_buf()`, `write_buf()`, `toupper()` — zstdio runtime **[CONFIRMED]**

From `LEVEL.7` (a separate Z8000 `zstdio.h` source embedded in the level file):

```c
fill_buf(fd)
char	*fd;
{
	int	*p1;

	p1 = *(p1 = &fd[PS]);
	if((p1[AC] = read(fd,&p1[BU],p1[BS])) <= 0)
	{
		errno = EOFERR;
		return ERROR;
	}
	p1[AP] = &p1[BU];
	return TRUE;
}

write_buf(fd)
char	*fd;
{
	int	*p1;

	p1 = *(p1 = &fd[PS]);
	if(write(fd,&p1[BU],p1[BS]) != p1[BS])
		return ERROR;
	p1[AP] = &p1[BU];
	p1[AC] = 0;
	return TRUE;
}

toupper(c)
char	c;
{
	if(c >= 'a' && c <= 'z')
		return c-' ';
	return c;
}
```

`PS`, `BU`, `BS`, `AP`, `AC` are indices into the `FILE`/fd structure
(`PS` = pointer to the per-stream state block, `BU` = buffer, `BS` = buffer
size, `AP` = current pointer, `AC` = byte count).

### 1.13 Z8000 assembly I/O stubs **[CONFIRMED]**

Literal `#asm` block from `LEVEL.7`:

```asm
! Z8000 IO runtime routines
! Version 1.0
!  All routines return rr4 == 0 on success else r5 == error code

zs_diskr  equ ^x36
zs_diskw  equ ^x37
zs_close  equ ^x33

zzzend:
	ldk	r4,#0
	ldk	r5,#0
	ret	c
	ld	r5,r0
	ret

zzz0:
	ld	r1,sr[#10]     ! record_count
	ldl	rr2,sr[#12]    ! record
	ldl	rr4,sr[#16]    ! buffer
	ldb	rl6,sr[#23]    ! fd
	ret

zread:  calr zzz0 ; sc #zs_diskr ; calr zzzend ; ret   ! (formatted below)
zwrite: calr zzz0 ; sc #zs_diskw ; calr zzzend ; ret
zclose: ldb rl6,sr[#7] ; sc #zs_close ; calr zzzend ; ret
zcloseall: sc #zs_closeall ; calr zzzend ; ret
zblock: sc #zs_blockdsk ; calr zzzend ; ret
```

Declared prototypes (from the comment block):
`zread(fd,buffer,record,record_count)`, `zwrite(...)`,
`zcreat(p_ptr,l_ptr,l_count)`, `zopenr/w(file_name,file_type)`,
`zadd(fd,where,record_count)`, `zclose(fd)`, `zcloseall()`, `zblock()`,
`zdelete(filename,file_type)`, `zfileinfo(filename,type,where)`.

The `#asm` block is followed in the same file by an unrelated test program:
`#include "zstdio.h"`, `main()` calling `unlink("test;rick")` → `creat(...)`
→ `close()` → `write()` → `for(t=0;t<200;++t) putchar(getc(fd));`, and
`fatal(s) { puts(s); puts(...); }` — truncated at the end of the file.

---

## 2. Constants and `#define`s

| Symbol | Value | Confidence / evidence |
|---|---|---|
| `STR` | 0 | CONFIRMED (stats[0..5] printed STR,INT,WIS,CHA,CON,DEX) |
| `INT` | 1 | CONFIRMED |
| `WIS` | 2 | CONFIRMED |
| `CHA` | 3 | CONFIRMED |
| `CON` | 4 | CONFIRMED |
| `DEX` | 5 | CONFIRMED |
| `TYPE` | ? | index of race/type; 15 types (0..14) |
| `SEX` | ? | bitfield, masks `0x08`/`0x10` inside `0x0018` |
| `ALI` | ? | bitfield, L/N/C in low 3 bits `0x0007` |
| `CLASS` | ? | index of class; 15 classes (0..14) |
| `LVL`, `DLVL` | ? | level / dungeon level |
| `X`, `Y` | ? | dungeon coordinates |
| `HIPAGE` | ? | hi-res display page |
| `AGE1` | ? | age |
| `O_HITS`, `HITS` | ? | original/current hit points |
| `XMULT`, `AGEC` | ? | experience multiplier / age-change factor |
| `I_1`, `I_2` | ? | item bitmasks / inventory flags |
| `M`, `F` | `0x08`, `0x10` (or swapped) | INFERRED from `0x0018` mask |
| `L`, `N`, `C` | `0x01`, `0x02`, `0x04` | INFERRED from `0x0007` mask |
| `MAX_CLASS` | 14 | INFERRED (15 names in PERSON.COM) |
| `MAX_TYPE` | 14 | INFERRED (15 names in PERSON.COM) |
| `CSTATS` | ≥ 20, exact ? | used as loop bound for stat array |
| `MAX_ITEMS` | ? | inventory slots; empty slot `i_type == 255` |
| `CARRY_OVER` | ? | number of `p_time[]` counters |
| `MAX_NAME` | ? | max length of character name; `filename[13]` |
| `NSTR` | ? | printf format for gold/exp (likely `"%d"`) |
| `TRUE`, `FALSE` | 1, 0 | conventional |
| `ERROR` | -1 | conventional C/CP/M runtime |
| `EOFERR`, `INVFD` | runtime errno codes | from `fill_buf()` |
| `PS`,`BU`,`BS`,`AP`,`AC` | FILE-structure field offsets | from zstdio runtime |

`XMULT` is divided by 200, `AGEC` by 10 in `get_class()` — these factors are
stored in the class table and are therefore percentage-like.

---

## 3. `PERSON.stat[]` layout

Confirmed numeric positions:

| Index | Name | Meaning |
|---|---|---|
| 0 | `STR` | strength |
| 1 | `INT` | intelligence |
| 2 | `WIS` | wisdom |
| 3 | `CHA` | charisma |
| 4 | `CON` | constitution |
| 5 | `DEX` | dexterity |

Named slots appearing elsewhere (relative order **not recoverable** from the
fragments; only indices 0–5 are pinned down):

| Name | Meaning / use |
|---|---|
| `TYPE` | race/creature type index (0..14) |
| `SEX` | male/female bit (`M`/`F`) |
| `ALI` | alignment bits L/N/C |
| `CLASS` | class index (0..14) |
| `LVL` | character level, initialised 0 |
| `DLVL` | dungeon level, initialised -1 |
| `X`, `Y` | position, initialised -1 |
| `HIPAGE` | hi-res page, initialised -1 |
| `AGE1` | age; 89 for Grey elf (TYPE 14), else 16 |
| `O_HITS`, `HITS` | hit points: `1 + rand() % (CON + STR)` |
| `XMULT` | experience multiplier (scaled ×200) |
| `AGEC` | age-change factor (scaled ×10) |
| `I_1`, `I_2` | class-granted item/ability masks (OR-ed) |

`CSTATS` is the total number of `int` slots zeroed at the start of `main()`.

---

## 4. Struct layouts

Approximate reconstructions — field sizes are **[INFERRED]** from usage:

```c
typedef struct {
	int  stat[CSTATS];          /* indices 0..5 STR..DEX, see section 3 */
	char p_name[?];             /* MAX_NAME+1 */
	char p_dungeon[?];          /* filename mapped via make_filename() */
	char class[?];              /* xname of selected class */
	char type[?];               /* printed with "%s the ..." in get_ali */
	char gold[NSTR+1];          /* numeric string, zfill()'d */
	char exp[NSTR+1];           /* numeric string, zfill()'d */
	int  p_mspell;              /* 0xffff for classes 4,5,9,12 */
	int  p_cspell;              /* 0xffff for classes 0,1,7,9,10,12 */
	int  p_ac;                  /* 10 at start */
	ITEM p_item[MAX_ITEMS];
	int  p_time[CARRY_OVER];
} PERSON;

typedef struct {
	int  i_type;                /* 255 = empty slot */
	int  i_??;                  /* see item-table field legend */
	char i_name[?];
	int  i_ac;
	int  i_bonus;
	int  i_damage;
	int  i_price;
	int  i_flags[5];
} ITEM;                         /* field names are suggestive only */

typedef struct {
	char *xname;
	int   factor[CSTATS];       /* indexed by STR..DEX, XMULT, AGEC,
	                               I_1, I_2, ALI, SEX */
} XCLASS;

typedef struct {
	char *tname;                /* INFERRED */
	int   factor[CSTATS];       /* only factor[0..5] read by compute_stats */
} XTYPE;
```

Global instances: `PERSON PERSON;` (or a global named `PERSON`), `XCLASS xclass[MAX_CLASS+1];`,
`XTYPE xtype;`.  `load_classes()` fills `xclass[]` from a data file at runtime.

---

## 5. Data tables

### 5.1 Item table line format **[CONFIRMED from ITEMS.* files]**

Each line has **11 numeric fields plus a name**:

```
<freq1> <freq2> <NAME_WITH_UNDERSCORES> <type> <ac> <damage> <price> <f1> <f2> <f3> <f4> <f5>
```

Example: `6 6 SWORD 0 0 2 20 0 0 0 0 0` → freq1=6, freq2=6, name=SWORD,
type=0, ac=0, damage=2, price=20, then five flag/power fields (all 0).

Decoded meanings (partly **[INFERRED]**):
1. `freq1` — relative frequency / quantity in the treasure table.
2. `freq2` — secondary frequency (level band / drop weight).
3. `name` — 8.3-ish token converted to display text by replacing `_` with spaces.
4. `type` — item-type id: 0 SWORD, 1 LEATHER, 2 ELIXIR, 3 SHIELD, 4 MACE,
   5 (variant), 6 HELMET, 7 WAR_HAMMER, 8 SCROLL, 9 MAIL, 10 PLATE, 11 DAGGER,
   14 TORCH, 15 STAFF/ROD/POINTED_STICK, 16 CHARM/LOCKET, 17 RING, 18 AXE,
   19 GLOVES, 20 BRICK, 21 CAPE, 22 BOOTS.
5. `ac` — armour-class modifier (negative = better armour).
6. `damage` — weapon damage bonus.
7. `price` — shop value (often 5/10/15/20/25/50).
8. `f1` — first power/effect bitmask (e.g. 256 levitation, 512 fear,
   1024 cloud-kill, 64 restoration, 128 dexedrine, 2048 knowledge).
9. `f2` — second power/effect (e.g. 18 = staff-of-life, 2 = cure/healing,
   8 magic mail, 10 shield/gloves/boots).
10. `f3`–`f5` — additional effect/curse fields; `-1` is frequently a
    "no extra / cursed" marker (e.g. SHIELD `… 0 0 0 0 -1`).

`+N` / `-N` in the source text (e.g. `ROD_OF_MIGHT 15 -1 +1 10 2 …`) are
decimal modifiers written with an explicit sign; parse them with `atoi`.

#### ITEMS.1-2
```
6 6 SWORD 0 0 2 20 0 0 0 0 0
2 1 SWORD_+_1 0 0 4 15 0 0 0 0 0
1 1 SWORD_+_2 0 0 5 10 0 0 0 0 0
6 6 SHIELD 3 -1 0 20 0 0 0 0 -1
2 1 SHIELD_+_1 3 -2 0 15 0 0 0 0 -1
1 1 SHIELD_+_2 3 -3 0 10 0 0 0 0 -1
6 6 LEATHER 1 -1 0 20 0 0 0 0 0
2 1 LEATHER_+_1 1 -2 0 15 0 0 0 0 0
1 1 LEATHER_+_2 1 -3 0 10 0 0 0 0 0
6 6 TORCH 14 0 0 50 1 0 0 0 0
6 6 MACE 4 0 2 20 0 0 0 0 0
2 1 MACE_+_1 4 0 4 15 0 0 0 0 0
1 1 MACE_+_2 4 0 5 10 0 0 0 0 0
6 6 MAIL 9 -1 0 20 0 0 0 0 0
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 0
1 1 MAIL_+_2 9 -3 0 10 0 0 0 0 0
2 1 PLATE 10 -3 0 25 0 0 0 0 -3

```

#### ITEMS.3-4
```
4 1 SWORD_+_2 0 0 5 15 0 0 0 0 0
1 1 SWORD_+_3 0 0 6 10 0 0 0 0 0
2 1 SHIELD_+_1 3 -2 0 15 0 0 0 0 -1
2 1 SHIELD_+_2 3 -3 0 10 0 0 0 0 -1
2 1 LEATHER_+_1 1 -2 0 15 0 0 0 0 0
2 1 LEATHER_+_2 1 -3 0 15 0 0 0 0 0
1 1 LEATHER_+_3 1 -4 0 10 0 0 0 0 0
4 1 TORCH 14 0 0 50 1 0 0 0 0
2 1 MACE_+_1 4 0 4 15 0 0 0 0 0
2 1 MACE_+_2 4 0 5 10 0 0 0 0 0
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 -1
2 1 MAIL_+_2 9 -3 0 10 0 0 0 0 -1
4 1 PLATE_+_1 10 -4 0 25 0 0 0 0 -3
2 1 PLATE_+_2 10 -5 0 25 0 0 0 0 -3
1 1 PLATE_+_3 10 -6 0 25 0 0 0 0 -3
1 1 STAFF_OF_LIFE 15 0 5 10 0 18 0 0 0
4 1 HELMET 6 -1 0 25 0 0 0 0 -1
2 1 HELMET_+_1 6 -2 0 20 0 0 0 0 -2
1 1 HELMET_+_2 6 -3 0 15 0 0 0 0 -3
10 10 POINTED_STICK 15 0 0 50 0 0 0 0 -1
10 10 LOUD_BOOTS 22 0 0 50 0 0 0 0 -5
4 1 SOFT_BOOTS 22 0 0 50 0 0 0 0 5

```

#### ITEMS.4-5
```
2 2 ELIXIR_OF_HEALING 2 0 0 0 1 0 0 0 0
6 6 WATER_BOTTLE 2 0 0 0 0 0 0 0 0
5 2 SWORD_+_1 0 0 3 10 0 0 0 0 -1
4 2 SWORD_-_1 0 0 -1 10 0 0 0 0 -1
1 1 MAGIC_SWORD_+_1 0 0 3 5 2 0 0 0 -1
5 5 SWORD 0 0 2 10 0 0 0 0 0 -1
1 1 SWORD_+_2 2 0 4 10 0 0 0 0 -1
5 2 MACE_+_1 4 0 3 10 0 0 0 0 -1
4 2 MACE_-_1 4 1 -1 10 0  0 0 0 -1
1 1 MAGIC_MACE_+_1 4 0 3 5 0 4 0 0 -1
1 1 MACE_+_2 4 0 4 10 0 0 0 0 -1
5 5 SHIELD 3 0 0 -1 0 10 0 0 -1
4 2 SHIELD_+_1 3 -2 0 10 0 0 0 0 -1
2 1 SHIELD_+_2 3 -3 0 10 0 0 0 0 -1
2 1 SHIELS_-_1 3 1 0 10 0 0 0 0 -1
1 1 MAGIC_SHIELD_+_1 3 -2 0 5 0 13 0 0 -1
5 5 MAIL 9 -1 0 10 0 0 0 0 -1
4 3 MAIL_+_1 9 -2 0 10 0 0 0 0 -1
3 1 MAIL_+_2 9 -3 0 10 0 0 0 0 -1
2 1 MAIL_-_1 9 1 0 10 0 0 0 0 -1
1 1 MAGIC_MAIL 9 0 0 -1 0 5 0 8 -1
4 2 MAIL 9 0 0 -2 0 10 0 0 -2
4 4 DAGGER 11 1 1 10 0 0 0 0 -1
5 10 POINTED_STICK 15 0 1 0 0 0 0
4 2 GLOVES 19 0 0 -1 0 10 0 0 0 0 -1
4 1 PLATE 10 -3 0 15 0 0 0 0 -1
4 5 BRICK 20 0 0 32 0 0 0 0 -1
4 5 HANKY 8 0 0 3 0 0  0 0 0
2 1 MACE_+_2 4 0 5 10 0 0 0 0 0
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 -1
2 1 MAIL_+_2 9 -3 0 10 0 0 0 0 -1
4 1 PLATE_+_1 10 -4 0 25 0 0 0 0 -3
2 1 PLATE_+_2 10 -5 0 25 0 0 0 0 -3
1 1 PLATE_+_3 10 -6 0 25 0 0 0 0 -3
4 1 HELMET 6 -1 0 25 0 0 0 0 -1
1 1 SCROLL_OF_CURE 8 0 0 5 0 2 0 0 0
1 1 SCROLL_OF_PROTECT 8 -1 0 5 8 0 0 0 0 0
1 1 SCROLL_OF_FEAR 8 0 0 5 512 0 0 0 0
1 1 CELLOPHANE 8 0 0 10 0 0 0 0 -1
1 1 SCROLL_OF_3 8 0 0 5 135 0 0 0 0
2 2 INK_WELL 2 0 0 0 0 0 0 0 0
1 1 ELIXIR_OF_RESTORATION 2 0 0 5 0 64 0 0 0 
1 1 ELVEN_BOOTS 22 0 0 0 10 0 0 0 0 2
1 1 MAGIC_SWORD_+3 0 -1 4 10 2 0 0 0 -4
1 1 RUSTY_BLADE 0 0 -1 10 0 0 0 0 0 
1 1 DAGGER+2 11 1 2 10 4 0 0 0 -3
2 2 PUTTY_KNIFE 11 0 0 10 0 0 0 0 0 

```

#### ITEMS.6-7
```
4 2 SWORD_-_1 0 0 -1 10 0 0 -1
1 1 MAGIC_SWORD_+_1 0 0 3 5 2 0 -1
1 1 MAGIC_SWORD_+3 0 -1 4 10 2 0 0 0 -4
2 2 SWORD_+_4 0 0 6 10 0 0 0 0 -3
1 1 MAGIC_SWORD_+_1 0 0 3 5 2 0 -1
1 1 SWORD_+_2 2 0 4 10 0 0 -1
1 1 SWORD_+_3 0 0 6 10 0 0 0 0 0
1 1 SHIELD_-_2 3 1 0 10 0 0 0 0 -1
1 1 MAGIC_SHIELD_+_1 3 -2 0 5 0 13 -1
2 1 SHIELD_+_2 3 -3 0 10 0 0 0 0 -1
1 1 SHIELD_-_2 3 1 0 10 0 0 0 0 -1
2 1 SHIELD_+_1 3 -2 0 15 0 0 0 0 -1
2 1 MACE_+_2 4 0 5 10 0 0 0 0 0
1 1 MAGIC_MACE_+_1 4 0 3 5 0 4 -1
1 1 MACE_+_4 4 0 6 10 0 0 -1
2 1 MACE_+_1 4 0 4 15 0 0 0 0 0
2 1 MAIL_-_1 9 1 0 10 0 0 -1
1 1 MAGIC_MAIL 9 -1 0 5 0 8 -1
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 -1
1 1 MAGIC_MAIL 9 -1 0 5 0 8 -1
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 -1
2 1 MAIL_+_2 9 -3 0 10 0 0 0 0 -1
1 2 PLATE-3 10 5 0 25 0 0 0 0 -3
2 1 PLATE_+_1 10 -4 0 25 0 0 0 0 -3
2 1 PLATE_+_2 10 -5 0 25 0 0 0 0 -3
1 1 PLATE_+_3 10 -6 0 25 0 0 0 0 -3
2 2 ROD_OF_MIGHT 15 -1 +1 10 2 0 0 0 0
2 2 CRAYON 15 0 0 0 0 0 0 0 0 
2 2 ROD_OF_MIGHT 15 -1 8 10 2 0 0 0 0
1 1 ROD_OF_WEAKNESS 15 -1 -3 10 0 0 0 0 -1
1 1 STAFF_OF_LIFE 15 0 5 10 0 18 0 0 0
3 3 PYROTECHNIC_STAFF 15 0 6 5 68 0 0 0 0 
1 1 STAFF_OF_LIFE 15 0 5 10 0 18 0 0 0
2 2 ROD_OF_MIGHT 15 -1 +1 10 2 0 0 0 0
2 2 CRAYON 15 0 0 0 0 0 0 0 0 
2 2 ROD_OF_MIGHT 15 -1 +1 10 2 0 0 0 0
1 1 ROD_OF_WEAKNESS 15 -1 -3 10 0 0 0 0 -1
1 1 RING_OF_LEVITATE 17 0 0 5 256 0 0 0 0
1 1 RING_OF_PROTECT 17 -1 0 5 8 0 0 0 0
1 1 RING_OF_MIGHT 17 -1 2 5 0 0 0 0 0
1 1 RING_OF_DAMAGE 17 -1 2 5 2 0 0 0 0
1 1 RING_OF_SLUMBER 17 0 0 5 4 0 0 0 0
1 1 RING_RING 17 0 0 5 0 0 0 0 -3
1 1 RING_OF_2 17 -1 0 5 19 0 0 0 0
1 1 RING_OF_3 17 0 0 5 42 0 0 0 0
4 1 SOFT_BOOTS 22 0 0 50 0 0 0 0 5
2 2 CHARM_BRACLET 16 +1 0 10 0 0 0 0 -2
2 2 CRAYON 15 0 0 0 0 0 0 0 0 
1 1 SCROLL_OF_LIGHT 8 0 0 5 1 0 0 0 0
1 1 SCROLL_OF_FEAR 8 0 0 5 512 0 0 0 0
1 1 SCROLL_OF_2 8 0 0 5 266 0 0 0 0
1 1 ELIXIR_OF_RESTORATION 2 0 0 5 0 64 0 0 0 
1 1 CAPE_OF_DEFENSE 21 -4 0 10 264 0 0 0 3
1 1 TOME_OF_KNOWLEGE 5 0 0 2 0 2048 0 0 -1
1 1 DEXEDRINE 2 0 0 0 128 0 0 0 0
2 2 WAR_HAMMER_+_4 7 0 6 10 0 0 0 0 -2
2 2 CANDLE 15 0 0 0 0 0 0 0 0 
2 2 CRAYON 15 0 0 0 0 0 0 0 0 
1 1 CLOUD_KILL_SOLUTION 2 0 0 0 1024 0 0 0 0 
2 2 BROOCH_OF_LEVITATE 16 0 0 5 256 0 0 0 0 
1 1 LOCKET_OF_FIREBALLS 16 -1 +1 5 32 0 0 0 0 
2 2 LOCKET_OF_HEALING 16 0 0 5 0 2 0 0 0
2 2 CHARM_BRACLET 16 +1 0 10 0 0 0 0 -2
1 1 SCROLL_OF_LIGHT 8 0 0 5 1 0 0 0 0
1 1 SCROLL_OF_CURE 8 0 0 5 0 2 0 0 0
1 1 SCROLL_OF_DAMAGE 8 0 0 5 2 0 0 0 0
1 1 SCROLL_OF_PROTECT 8 -1 0 5 8 0 0 0 0 0
1 1 SCROLL_OF_FEAR 8 0 0 5 512 0 0 0 0
1 1 CELLOPHANE 8 0 0 10 0 0 0 0 -1
1 1 SCROLL_OF_2 8 0 0 5 266 0 0 0 0
1 1 SCROLL_OF_3 8 0 0 5 135 0 0 0 0
1 1 SCROLL_OF_4 8 -1 0 5 15 0 0 0 0
2 2 NEWS_PAPERS 8 +1 0 10 0 0 0 0 -2
3 3 BEER_TAB 17 +1 0 10 0 0 0 0 
2 2 INK_WELL 2 0 0 0 0 0 0 0 0
1 1 ELIXIR_OF_RESTORATION 2 0 0 5 0 64 0 0 0 
1 1 ELVEN_BOOTS 22 0 0 0 10 0 0 0 0 2
1 1 RUSTY_BLADE 0 0 -1 10 0 0 0 0 0 
1 1 DAGGER+2 11 1 2 10 4 0 0 0 -3
2 2 PUTTY_KNIFE 11 0 0 10 0 0 0 0 0 
4 2 HELMET 6 0 0 -1 0 10 0 0 0 0 -1
4 1 MAGIC_TORCH 14 0 0 5 1 0 0 0 -1
4 2 WAR_HAMMER 7 0 2 10 0 0 0 0 -1
4 2 GLOVES 19 0 0 -1 0 10 0 0 0 0 -1
4 5 BRICK 20 0 0 32 0 0 0 0 -1
1 1 TACK_HAMMER 7 1 0 10 0 0 0 0 -1
6 6 WATER_BOTTLE 2 0 0 0 0 0 0
4 1 TORCH 14 0 0 50 1 0 0 0 0
4 1 HELMET 6 -1 0 25 0 0 0 0 -1
2 1 HELMET_+_1 6 -2 0 20 0 0 0 0 -2
1 1 HELMET_+_2 6 -3 0 15 0 0 0 0 -3
1 1 LOUD_BOOTS 22 0 0 50 0 0 0 0 -5
1 1 SOFT_BOOTS 22 0 0 50 0 0 0 0 5

```

#### ITEMS.C-0
```
4 3 SWORD 0 0 2 10 0 0 0 0 -1
4 3 MACE 4 0 1 10 0 0 0 0 -1
4 2 HELMET 6 -1 0 10 0 0 0 0 -1
4 1 MAGIC_TORCH 14 0 0 5 1 0 0 0 -1
4 3 AXE 18 0 2 10 0 0 0 0 -1
4 3 BOOTS 22 0 0 0 0 10 0 0 1
4 4 LEATHER 1 -1 0 10 0 0 0 0 -1
4 2 SHIELD 3 -1 0 10 0 0 0 0 -1
4 2 WAR_HAMMER 7 0 2 10 0 0 0 0 -1
4 2 MAIL 9 -2 0 10 0 0 0 0 -1
4 4 DAGGER 11 1 1 10 0 0 0 0 -1
15 10 POINTED_STICK 15 0 1 1 0 0 0 0 0
4 2 GLOVES 19 0 0 -1 0 10 0 0 0 0 -1
4 1 PLATE 10 -3 0 15 0 0 0 0 -1
4 5 BRICK 20 0 0 32 0 0 0 0 -1
4 5 HANKY 8 0 0 3 0 0  0 0 0
1 1 LEATHER_-_2 1 2 0 30 0 0 0 0 -1
4 1 BEENIE 6 2 0 15 0 0 0 0 0 
1 1 BUTTER_KNIFE 11 2 0 15 0 0 0 0 0
1 1 TACK_HAMMER 7 1 0 10 0 0 0 0 -1
1 1 SHIELD_-_2 3 1 0 10 0 0 0 0 -1

```

#### Supplemental item lines embedded in `LEVEL.8`, `LEVEL.4`, `LEVEL.10`
```
1 1 SWORD_+_2 2 0 4 10 0 0 -1
1 1 SWORD_+_3 0 0 6 10 0 0 0 0 0
1 1 SHIELD_-_2 3 1 0 10 0 0 0 0 -1
1 1 MAGIC_SHIELD_+_1 3 -2 0 5 0 13 -1
2 1 SHIELD_+_2 3 -3 0 10 0 0 0 0 -1
2 1 SHIELD_+_1 3 -2 0 15 0 0 0 0 -1
2 1 MACE_+_2 4 0 5 10 0 0 0 0 0
1 1 MAGIC_MACE_+_1 4 0 3 5 0 4 -1
1 1 MACE_+_4 4 0 6 10 0 0 -1
2 1 MACE_+_1 4 0 4 15 0 0 0 0 0
2 1 MAIL_-_1 9 1 0 10 0 0 -1
1 1 MAGIC_MAIL 9 -1 0 5 0 8 -1
2 1 MAIL_+_1 9 -2 0 15 0 0 0 0 -1
2 1 MAIL_+_2 9 -3 0 10 0 0 0 0 -1
1 2 PLATE-3 10 5 0 25 0 0 0 0 -3
2 1 PLATE_+_1 10 -4 0 25 0 0 0 0 -3
2 1 PLATE_+_2 10 -5 0 25 0 0 0 0 -3
1 1 STAFF_OF_LIFE 15 0 5 10 0 18 0 0 0
3 3 PYROTECHNIC_STAFF 15 0 6 5 68 0 0 0 0
2 2 ROD_OF_MIGHT 15 -1 +1 10 2 0 0 0 0
2 2 CRAYON 15 0 0 0 0 0 0 0 0
1 1 ROD_OF_WEAKNESS 15 -1 -3 10 0 0 0 0 -1
1 1 RING_OF_LEVITATE 17 0 0 5 256 0 0 0 0
1 1 RING_OF_PROTECT 17 -1 0 5 8 0 0 0 0
1 1 RING_OF_MIGHT 17 -1 2 5 0 0 0 0 0
1 1 RING_OF_DAMAGE 17 -1 2 5 2 0 0 0 0
1 1 RING_OF_SLUMBER 17 0 0 5 4 0 0 0 0
1 1 RING_RING 17 0 0 5 0 0 0 0 -3
1 1 RING_OF_2 17 -1 0 5 19 0 0 0 0
1 1 RING_OF_3 17 0 0 5 42 0 0 0 0
4 1 SOFT_BOOTS 22 0 0 50 0 0 0 0 5
2 2 CHARM_BRACLET 16 +1 0 10 0 0 0 0 -2
1 1 SCROLL_OF_LIGHT 8 0 0 5 1 0 0 0 0
1 1 SCROLL_OF_FEAR 8 0 0 5 512 0 0 0 0
1 1 SCROLL_OF_2 8 0 0 5 266 0 0 0 0
1 1 ELIXIR_OF_RESTORATION 2 0 0 5 0 64 0 0 0
1 1 CAPE_OF_DEFENSE 21 -4 0 10 264 0 0 0 3
1 1 TOME_OF_KNOWLEGE 5 0 0 2 0 2048 0 0 -1
1 1 DEXEDRINE 2 0 0 0 128 0 0 0 0
2 2 WAR_HAMMER_+_4 7 0 6 10 0 0 0 0 -2
2 2 CANDLE 15 0 0 0 0 0 0 0 0
1 1 CLOUD_KILL_SOLUTION 2 0 0 0 1024 0 0 0 0
2 2 BROOCH_OF_LEVITATE 16 0 0 5 256 0 0 0 0
1 1 LOCKET_OF_FIREBALLS 16 -1 +1 5 32 0 0 0 0
2 2 LOCKET_OF_HEALING 16 0 0 5 0 2 0 0 0
1 1 SCROLL_OF_CURE 8 0 0 5 0 2 0 0 0
1 5 HANKY 8 0 0 3 0 0  0 0 0
1 1 MACE_+_2 4 0 5 10 0 0 0 0 0
```

### 5.2 Class factor table `xclass[].factor[]`

**Numeric factors not recovered as text.**  What is confirmed:
* 15 classes. Names from `PERSON.COM` (offset ~6400), in order:
  `Cleric, Demondim, Featheror, Hirebrand, Mage, Minstrel, Ninja, Paladin,
  Peasant, Ranger, Raver, Rogue, Sage, Samurai, Valkyrie`.
  (Earlier level data also shows `Veteran, Warrior, Swordman, Pseudo-Dragon,
  Medium, Seer, Conjurer, Acolyte, Adept, Priest, Apprentice, Footpad,
  Robber, Magician, Theurgist` — these are level-denominator/monster lists,
  **not** the class table.)
* `factor[0..5]` are minimum STR/INT/WIS/CHA/CON/DEX thresholds (see `valid()`).
* `factor[ALI] & 0x0007` and `factor[SEX] & 0x0018` are eligibility masks.
* `factor[XMULT]` is a percentage divisor of 200; `factor[AGEC]` of 10.
* `factor[I_1]`, `factor[I_2]` are OR-ed into the character.
* The compiled constants are present in `PERSON.COM` as Z80
  `LD HL,<imm16>; PUSH HL` sequences beginning around file offset 0x1E5C
  (e.g. `21 54 1E E5 CD A6 20 EB ... 21 18 00 39 F9 EB C1 C9`), followed by
  further 16-bit immediate pushes.  Decoding those sequences is the remaining
  step needed to recover the exact numeric table for the port.

### 5.3 Type factor table `xtype.factor[]`

**Numeric factors not recovered as text.**  15 types, names from
`PERSON.COM` (offset ~7508), in order:
`Human, Elf, Dwarf, Halfdwarf, Halfelf, Hobbit, Orc, Uruk-hai, Orge, Pixie,
Goblin, Hobgoblin, Kobold, Ur-vile, Grey elf`.

`compute_stats()` uses `factor[0..5]` as additive race modifiers:
`stat[i] = max(1, roll() + 2*factor[i])`.  A "roll" therefore probably returns
3d6 (3..18); the exact dice routine is missing (section 7).

### 5.4 Monster tables

Lines are: `<level> <name> <12 numeric fields>` (level = first number; the
name is followed by 12 values, likely hit dice, AC, speed, damage, etc.).
Recovered verbatim:

#### MONSTERS.0-1
```
4 Veteran 10 5 1 5 1 13 5 8 1 0 0
2 Warrior 11 4 1 6 2 15 7 7 2 0 0
1 Swordman 12 3 1 7 3 15 9 6 3 0 0
4 Orc 10 10 5 1 1 2 1 10 1 0 0
2 Pseudo-Dragon 10 3 1 3 1 15 10 7 2 4 0
4 Medium 9 5 1 3 1 5 1 10 1 0 0
2 Seer 10 5 1 4 1 7 1 9 2 8 0
1 Conjurer 11 4 1 5 1 8 2 9 3 8 0
4 Acolyte 9 5 1 3 1 5 1 10 1 0 0
2 Adept 10 5 1 4 1 7 1 9 2 16 0
1 Priest 11 4 1 5 1 8 2 9 3 16 0
4 Apprentice 9 5 1 3 1 5 1 10 1 0 0
2 Footpad 10 5 1 4 1 7 1 9 2 0 0
1 Robber 11 4 1 5 1 8 2 9 3 0 0

```

#### MONSTERS.2-3
```
3 Wererat 15 4 1 7  3 10 5  5 3 0 0 
4 Wereboar 14 4 1 7 3 9 5 6 2 0 0 
2 Werecat 16 5 2 7 5 12 5 5 4 0 0 
3 Orc 12 5 2 8 2 10 5 6 3 0 0 
4 Gnome 12 5 2 7 2 8 5 6 2 0 0 
3 Hobglobin 15 4 1 10 5 20 10 4 3 0 0 
2 Ogre_Mage 18 2 1 12 5 20 12 4 4 8 0 
3 Ogre 13 3 1 10 4 15 10 5 3 0 0 
4 Seer 15 3 1 8 3 10 8 6 2 8  0 
3 Adept 15 3 1 8 5 12 8 6 3 16 0
4 Skeleton 10 5 2 2 1 20 15 4 2 3 0 
3 Large_spider 10 1 1 12 8 8 4 7 3 0 0 
2 Wight 14 1 1 20 15 5 1 4 4 1 0 
3 Large_ant 12 10 2 12 8 8 4 7 3 0 0 
2 Blink_dog 13 2 1 12 8 8 4 5 4 0 0 
2 Ghoul 13 3 1 20 10 8 3 4 4 1 0
3 Gnoll 16 3 1 10 4 15 10 5 3 0 0 
4 Goblin 14 3 1 7 3 9 5 5 2 0 0 
1 Basilisk 12 4 2 5 1 15 10 -1 5 0 0 
1 Cockatrice 19 2 2 10 5 20 15 -1 5 0 0 

```

#### MONSTERS.4-5
```
4 Werecat 16 5 2 7 5 12 5 5 4 0 0
4 Ogre_mage 18 2 1 12 5 20 12 4 4 0 0
4 Wight 14 1 1 20 15 5 1 4 4 1 0
4 Blink_dog 13 2 1 12 8 8 4 5 4 0 0
4 Ghoul 13 3 1 20 10 8 3 4 4 1 0
3 Basilisk 12 4 2 5 1 15 10 -1 5 0 0
3 Cockatrice 19 2 2 10 5 20 15 -1 5 0 0
3 Werewolf 17 6 2 10 5 15 7 5 5 0 0
2 Manticore 17 5 2 15 5 20 5 3 6 0 0
2 Mummy 12 1 1 10 7 30 20 5 6 9 0
2 Minotaur 17 5 3 15 5 15 5 4 6 0 0
4 Hero 12 3 1 10 7 20 10 3 4 0 0
3 Swashbuckler 12 3 1 15 8 25 10 2 5 0 0
2 Magician 17 5 3 20 10 50 30 0 6 8 0
3 Theurgist 15 5 3 15 10 40 10 2 5 8 0
4 Priest 13 4 3 15 8 25 10 4 4 16 0
4 Robber 15 1 1 10 8 20 10 6 4 0 0
3 Troll 17 6 2 10 5 15 7 5 5 0 0
2 Wraith 15 1 1 20 15 6 2 4 6 1 0
1 Vicar 17 5 3 20 15 50 30 0 7 16 0
1 Wyvern 14 2 1 20 15 50 30 -3 7 4 0
1 Succubi 15 3 1 18 15 30 25 -1 7 25 0

```

#### MONSTERS.6-7
```
4 Manticore 17 5 2 15 5 20 5 3 6 0 0
4 Mummy 12 1 1 10 7 30 20 5 6 9 0
4 Minotaur 17 5 3 15 5 15 5 4 6 0 0
4 Magician 17 5 3 20 10 50 30 0 6 8 0
4 Wraith 15 1 1 20 15 6 2 4 6 1 0
2 Vicar 17 5 3 20 15 50 30 0 7 16 0
2 Wyvern 14 2 1 20 15 50 30 -3 7 4 0
2 Succubi 15 3 1 18 15 30 25 -1 7 25 0
4 Fighter 17 4 1 20 10 30 25 -1 6 0 0
3 Myrmidon 18 4 1 20 15 35 25 -3 7 0 0
3 Enchanter 19 4 2 20 15 30 20 -1 7 8 0
3 Bishop 19 4 2 20 15 30 20 -1 7 16 0
3 Burglar 17 4 2 15 10 40 20 2 7 0 0
2 Huge_spider 20 1 1 25 15 40 25 3 8 0 0
2 Giant_wasp 15 1 1 40 30 20 15 3 8 0 0
1 Will_o_wisp 20 1 1 50 30 10 5 -2 9 1 0
1 Titan 23 1 1 50 40 45 30 -4 9 0 0
1 Chimera 20 2 2 30 20 40 30 -2 8 0 0
1 Hill_giant 20 5 1 30 20 45 35 -2 8 0 0

```

`LEVEL.1`–`LEVEL.3`, `LEVEL.4`–`LEVEL.6`/`10` and `LEVEL.8` each carry their
own monster-name slice embedded in the overlaid source; the `MONSTERS.*`
files above are the cleanest transcription.

---

## 6. Random number generation

#### 6.1 `rand()` — **CONFIRMED from machine code**

`DUNGEON.COM` 0x353e (`disasm-DUNGEON.txt`, runtime address = file offset + 0x100)
holds the complete generator; the identical routine is at 0x353e in `PERSON.COM`.
The state is a 32-bit LFSR kept as two little-endian words at 0x0597 (low) and
0x0599 (high):

```c
/* runtime addresses 0x353e-0x3565 */
unsigned rand(void) {
    unsigned fb = ((*(byte*)0x0597 & 0x08) != 0) ^ ((*(byte*)0x0597 & 0x40) != 0);
    unsigned state = ((*(unsigned*)0x0599 << 16) | *(unsigned*)0x0597);
    state = (state << 1) | fb;
    *(unsigned*)0x0597 = (unsigned)state;
    *(unsigned*)0x0599 = state >> 16;
    return (*(unsigned*)0x0597) & 0x7fff;          /* high byte masked 0x7f */
}
```

`AND 0x48` followed by `JP z` / `JP pe` / `SCF` implements XOR of bits 3 and 6
of the low byte.  `RLA` through both words implements the 32-bit shift.  The
`0x7f` mask on the returned high byte is why `rand()` never exceeds 0x7fff.
The C# port (`Nemesis/Engine/LfsrRandom.cs`) is a bit-exact translation;
`System.Random` must **not** be substituted.

#### 6.2 Seed / `init_rand()` — **CONFIRMED (default seed), HIGH (interactive path)**

- The `.COM` image initialises 0x0597..0x059a to `0xff` (`disasm-PERSON.txt`
  lines 762-765).  At 0x035f, `LD HL,0x59dc / LD (0x0597),HL` overwrites only
  the low word, so the boot state is **`0xffff59dc`**.
- `init_rand()` at 0x36da reads a decimal seed from the console (`CALL 0x030a`,
  typically `atoi`).  If the parsed value is nonzero it is stored into both
  words (0x36e2-0x36e8).  If zero, the program prompts again, converts the
  entered string through the 6-byte bignum routine at 0x3658-0x36c7, masks the
  top byte with `0x7f` (0x36bd-0x36c1) and stores that (0x3704-0x3707).
  Pressing Enter therefore leaves the compiled default `0xffff59dc` intact.

The port exposes the seed explicitly (`new <name> ... [seed]`) and defaults the
`GameEngine` RNG to `0xffff59dc` for the no-seed path.

#### 6.3 `roll()` — **CONFIRMED: 4d5**

`roll()` is used only by `compute_stats()` (stat value =
`max(1, roll() + 2 * xtype.factor[i])`).  No definition survives in any
embedded C fragment, but its compiled body *does* survive in `PERSON.COM` at
runtime addresses `0x1a71`–`0x1ad1`, in the same runtime-library region as the
48-bit LCG at `0x360e`:

```c
int roll(void) {
    int n = 4, sum = 0;
    while (n > 0) {
        sum += lcg_rand() % 5 + 1;   /* CALL 0x360e, DE % 5, then +1 */
        --n;
    }
    return sum;                       /* range 4..20 */
}
```

The four LCG calls per stat are visible at `0x1aa3`; the loop counter is
initialised to 4 at `0x1a7b` and decremented at `0x1abb`; the result is the
accumulator at `BC+2`, returned via `0x1ac2`.  The multiplier and state are the
same as `DUNGEON.COM`'s LCG (`0xe09a0e0187c5`, boot state 1).  The port's
`Lcg48Random.RollStats()` is a direct translation.

The earlier 3d6 placeholder was disproved twice: an exhaustive 2^32 LFSR-seed
search found no match for `ARAGORN.CHR`, and the disassembly now shows 4d5
directly.  With the default LCG state the six rolls are `9,15,8,8,12,11`,
giving Hobbit stats `7,13,8,10,14,13` — not the shipped character's
`12,10,11,12,8,15`, so that character was either rolled after a custom
`init_rand()` seed or edited.  A matching LCG seed (536812) exists, but with
only one hit in 2^96 candidate states it is not evidence of the original seed.

| Call | Site | Purpose |
|---|---|---|
| `init_rand()` | `main()` before character creation | seed RNG (0x36da) |
| `roll()` | `compute_stats()` | stat dice; clamped so `roll()+2*factor >= 1` |
| `rand()` | `compute_stats()` | sex (`rand()%2`), gold (`rand()%(200-tot)`) |
| `rand()` | `init_char()` | initial hits (`1 + rand() % (CON+STR)`) |

---

## 7. Missing / incomplete sections (checklist for the port)

Definitions **not present** in any C fragment, but now recovered from machine
code: `roll()` (4d5, section 6.3), `rand()` and the default seed
(sections 6.1-6.2).

Definitions still missing entirely:

- `get_stats()` — presumably calls `compute_stats()`/`display_stats()` and a
  re-roll confirmation loop (`LEVEL.1` offset 2232 tail:
  `while(toupper(sgetchar()) != 'Y')`).
- `save_char()` — writes `PERSON` to `<name>.CHR`.
- `getdef()` — load/set defaults.
- `load_classes()` — populate `xclass[]` from data file.
- `valid_name()`, `make_filename()` — character-name validation, 8.3 mapping.
- `clr()`, `tab()`, `ol()`, `eol()`, `puts()`, `sprintf()`, `zfill()`,
  `waitkey()`, `sgetchar()`, `sleep()`, `getc()` — screen/console helpers.
- Full `get_dungeon()`, full `get_ali()`, full `get_type()` (section 1).
- Numeric `xclass[].factor[]` and `xtype.factor[]` arrays (section 5.2/5.3).
- Exact values of `CSTATS`, `MAX_ITEMS`, `CARRY_OVER`, `MAX_NAME`, `NSTR`,
  and the numeric ordering of the non-0..5 `stat[]` indices.
- Item/monster table loading routines and level-map decoding code.
- Complete `zstdio` test program and the remainder of the `#asm` stub block
  (`zdelete`, `zfileinfo`, …) — cut off at the end of `LEVEL.7`.

Cross-check aids: `docs/nemesis/decomp-*.c` (Ghidra) and `disasm-*.txt` are
available for the four `.COM` overlays; the `xclass`/`xtype` constants and the
RNG bodies can be recovered from them if byte-exact behaviour is required.

---

## 8. Porting notes

1. Keep the six stat indices as an enum `STR=0,INT=1,WIS=2,CHA=3,CON=4,DEX=5`;
   store all other named slots in explicit fields rather than guessing indices.
2. Preserve the `valid()` ranger bug as a documented compatibility flag.
3. Treat item table names as `_`-separated tokens; display with spaces.
4. Parse signed fields (`-1`, `+1`) with `int.Parse`/`atoi` equivalence; `255`
   is the empty-item sentinel and `-1` the "unset position" sentinel.
5. `gold` and `exp` are zero-padded decimal *strings*, not integers — `zfill()`
   is applied after `sprintf()` (likely width `NSTR`).
6. The class/type name lists in section 5.2/5.3 are literal `.COM` data and are
   the best available source for UI-facing names.
