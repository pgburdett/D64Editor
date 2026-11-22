# Nemesis LE — character generation & castle logic analysis

**Status:** partial but substantial. Confirmed facts come from the recovered original C source fragments embedded in the `LEVEL.*` data files and from manual Z80 decoding of `PERSON.COM`/`CASTLE.COM`. The Ghidra C output was useful only as a cross-check; it is badly garbled by overlapping instructions and wrong call signatures.

**Sources:**
- `/tmp/opencode/nemesis-source/LEVEL.0.txt`, `LEVEL.1-4.txt`, `ALL-SOURCE-FRAGMENTS.txt`
- `/home/pat/source/repos/D64Editor/docs/nemesis/disasm-PERSON.txt`
- `/home/pat/source/repos/D64Editor/docs/nemesis/disasm-CASTLE.txt`
- `/home/pat/source/repos/D64Editor/docs/nemesis/game-files/*.COM`, `ITEMS.*`, `CASTLE.0`
- `nemesis-manual-ocr.txt` (supporting only; appendix tables too corrupted for exact values)

---

## 1. Character creation flow (`PERSON.C`, recovered source)

The recovered main flow is:

```c
for (seed = 0; seed < CSTATS; ++seed)
    PERSON.stat[seed] = 0;

getdef();
clr();
init_rand();
get_name();
get_dungeon();
get_type();
get_stats();          /* source also calls this compute_stats() in some fragments */
get_ali();
get_class();
init_char();
save_char();
```

### 1.1 Attribute rolling — CONFIRMED formula

From `compute_stats()` / `get_stats()`:

```c
tot = 0;
t3 = PERSON.stat[TYPE];
for (t1 = 0; t1 < 6; ++t1)
    tot += PERSON.stat[t1] =
        (t2 = roll() + (2 * xtype.factor[t1])) <= 0 ? 1 : t2;

PERSON.stat[SEX] = (rand() % 2) ? M : F;
sprintf(PERSON.gold, NSTR, rand() % (200 - tot));
zfill(PERSON.gold);
```

- Six attributes in index order: `0=STR, 1=INT, 2=WIS, 3=CHA, 4=CON, 5=DEX`.
- Each is `max(1, roll() + 2 * type_modifier)`.
- Starting gold is `rand() % (200 - total_of_six_attributes)`, stored as a zero-filled decimal string.
- **UNCERTAIN:** exact dice expression inside `roll()` was not recovered from the fragments; likely a standard 3d6-style roll, but this is not proven.

### 1.2 Race/type table — CONFIRMED from Z80 table constructor at `0x1b0e`–`0x1e3a`

Fifteen types, 0-based. First six numbers are `xtype.factor[STR..DEX]`; the remaining fields are extra type factors (one is an XP/level multiplier around 174–225).

| # | Name | STR | INT | WIS | CHA | CON | DEX | Other fields (raw) |
|---|------|----:|----:|----:|----:|----:|----:|--------------------|
| 0 | Human | 0 | 0 | 0 | 0 | 0 | 0 | 0, 195, 0, 0, 1 |
| 1 | Elf | 0 | 1 | 0 | 1 | 0 | 0 | 0, 225, 0, 0, 1 |
| 2 | Dwarf | 2 | -1 | 1 | -1 | 1 | -1 | 2, 205, -128, 0, 1 |
| 3 | Halfdwarf | 1 | -1 | 0 | -1 | 1 | 0 | 1, 217, -128, 0, 1 |
| 4 | Halfelf | 0 | 1 | 0 | 0 | 0 | 1 | 0, 217, 0, 0, 1 |
| 5 | Hobbit | -1 | -1 | 0 | 1 | 1 | 1 | -2, 192, 0, 0, 1 |
| 6 | Orc | 1 | -1 | -1 | -1 | 0 | 0 | 0, 182, 0, 0, 1 |
| 7 | Uruk-hai | 2 | -1 | -1 | -1 | 1 | 0 | 0, 200, 0, 0, 1 |
| 8 | Orge | 3 | -1 | -2 | 1 | 1 | 0 | 2, 210, 0, 0, 1 |
| 9 | Pixie | -1 | 0 | -1 | 1 | -1 | 1 | -1, 190, 0, 0, 1 |
| 10 | Goblin | 0 | -1 | -1 | -1 | 0 | 0 | 0, 185, 0, 0, 1 |
| 11 | Hobgoblin | 1 | -2 | -1 | -1 | 1 | 1 | 2, 200, 0, 0, 1 |
| 12 | Kobold | -1 | -2 | -2 | -1 | 0 | 0 | -2, 174, 0, 0, 1 |
| 13 | Ur-vile | 0 | 0 | 1 | -2 | 0 | 1 | 0, 192, 0, 0, 1 |
| 14 | Grey elf | -1 | 2 | 0 | 1 | -1 | 0 | -2, 225, 0, 0, 1 |

Notes:
- String addresses: Human `0x1e54`, Elf `0x1e5a`, Dwarf `0x1e5e`, Halfdwarf `0x1e64`, Halfelf `0x1e6e`, Hobbit `0x1e76`, Orc `0x1e7d`, Uruk-hai `0x1e81`, Orge `0x1e8a`, Pixie `0x1e8f`, Goblin `0x1e95`, Hobgoblin `0x1e9c`, Kobold `0x1ea6`, Ur-vile `0x1ead`, Grey elf `0x1eb5`.
- `get_type()` prompts `"Pick a type : "` (`0x146c`) and displays the list; the exact input loop was not fully reconstructed.
- `init_char()` comment: `/* Eldar elf starts at 89 (why not) */` — type 14 (Grey elf) starts at age 89, all others at 16.

### 1.3 Class table — CONFIRMED from Z80 table constructor at `0x158f`–`0x19df`

Fifteen classes, 0-based. First six numbers are minimum STR/INT/WIS/CHA/CON/DEX. The 7th is the alignment mask (`1=Lawful, 2=Neutral, 4=Chaotic`; `7` = any). Later fields include experience/age/item flags.

| # | Name | STR | INT | WIS | CHA | CON | DEX | Align mask | Later raw fields |
|---|------|----:|----:|----:|----:|----:|----:|-----------:|------------------|
| 0 | Cleric | 0 | 0 | 12 | 0 | 0 | 0 | 1 (L) | 108, 24, 100, 100, 100 |
| 1 | Demondim | 0 | 0 | 12 | 0 | 0 | 0 | 4 (C) | 108, 24, 100, 100, 100 |
| 2 | Featheror | 0 | 0 | 0 | 19 | 9 | 0 | 6 (N+C) | 95, 16, 100, 100, 100 |
| 3 | Hirebrand | 12 | 0 | 0 | 0 | 0 | 0 | 7 (any) | 100, 24, 100, 100, 100 |
| 4 | Mage | 0 | 12 | 0 | 0 | 0 | 0 | 7 | 110, 24, 100, 100, 100 |
| 5 | Minstrel | 9 | 12 | 12 | 15 | 0 | 9 | 7 | 112, 24, 100, 100, 100 |
| 6 | Ninja | 15 | 12 | 0 | 0 | 0 | 15 | 6 | 95, 24, 100, 100, 100 |
| 7 | Paladin | 12 | 9 | 13 | 17 | 9 | 0 | 1 (L) | 112, 8, 100, 100, 100 |
| 8 | Peasant | 0 | 0 | 0 | 0 | 0 | 0 | 7 | 90, 24, 100, 100, 100 |
| 9 | Ranger | 13 | 13 | 14 | 0 | 14 | 15 | 7 | 118, 24, 100, 100, 100 |
| 10 | Raver | 12 | 9 | 13 | 17 | 9 | 0 | 4 (C) | 112, 8, 100, 100, 100 |
| 11 | Rogue | 0 | 0 | 0 | 0 | 0 | 12 | 6 | 95, 24, 100, 100, 100 |
| 12 | Sage | 9 | 14 | 14 | 0 | 0 | 12 | 7 | 115, 24, 100, 100, 100 |
| 13 | Samurai | 15 | 0 | 15 | 0 | 9 | 15 | 7 | 112, 24, 100, 100, 100 |
| 14 | Valkyrie | 12 | 9 | 13 | 19 | 9 | 0 | 7 | 115, 16, 100, 100, 100 |

String addresses `0x1a00`–`0x1a68` in order: Cleric, Demondim, Featheror, Hirebrand, Mage, Minstrel, Ninja, Paladin, Peasant, Ranger, Raver, Rogue, Sage, Samurai, Valkyrie.

**Alignment encoding:** `L=1, N=2, C=4` (bit flags). `get_ali()` forces `C` when `TYPE == 13` (Ur-vile). Otherwise prompts `"Enter alignment(L,N,C) : "` (`0xf20`) and displays `Lawful`, `Neutral`, `Chaotic`, `Untouchable`.

**Sex encoding:** `PERSON.stat[SEX] = (rand() % 2) ? M : F`. Display: `(stat[SEX] & M) == M ? "Male" : "Female"`. Exact numeric values of `M`/`F` not recovered; class sex mask uses `0x18` (bits 3 and 4), so likely `M=8, F=16`.

### 1.4 Class selection / validation — CONFIRMED source

```c
get_class() {
    load_classes();
    dis_val_classes();
    do {
        tab(22,0); eol();
        puts("Pick a class : ");       /* 0xbdc */
        sscanf(gets(temp), "%d", &t1);
    } while (!valid(t1));

    PERSON.stat[CLASS] = t1;
    PERSON.stat[XMULT] = (PERSON.stat[XMULT] * xclass[t1].factor[XMULT]) / 200;
    PERSON.stat[AGEC]  = (PERSON.stat[AGEC]  * xclass[t1].factor[AGEC])  / 10;
    PERSON.stat[I_1]  |= xclass[t1].factor[I_1];
    PERSON.stat[I_2]  |= xclass[t1].factor[I_2];
    strcpy(PERSON.class, xclass[t1].xname);
}

valid(xx) {
    for (t1 = 0; t1 < 6; ++t1)
        if (PERSON.stat[t1] < xclass[xx].factor[t1]) return FALSE;

    /* Test for a ranger being human */
    if ((t1 == 9) && (PERSON.stat[TYPE] != 0)) return FALSE;   /* BUG: t1 is 6 here, so dead code */
    if ((PERSON.stat[ALI] & xclass[xx].factor[ALI] & 0x0007) == 0) return FALSE;
    if ((PERSON.stat[SEX] & xclass[xx].factor[SEX] & 0x0018) == 0) return FALSE;
    return TRUE;
}
```

`dis_val_classes()` lists all valid classes in a 3-column grid as `"%d) %s"`.

### 1.5 Character initialization — CONFIRMED source

```c
init_char() {
    t = PERSON.stat[CLASS];

    if (t == 4 || t == 5 || t == 9 || t == 12)   /* Mage, Minstrel, Ranger, Sage */
        PERSON.p_mspell = 0xffff;
    else
        PERSON.p_mspell = 0;

    if (t == 0 || t == 1 || t == 7 || t == 9 || t == 10 || t == 12)
        PERSON.p_cspell = 0xFFFF;               /* Cleric, Demondim, Paladin, Ranger, Raver, Sage */
    else
        PERSON.p_cspell = 0;

    PERSON.p_ac = 10;
    PERSON.stat[LVL] = 0;
    PERSON.stat[DLVL] = PERSON.stat[X] = PERSON.stat[Y] = PERSON.stat[HIPAGE] = -1;

    /* Eldar elf starts at 89 (why not) */
    PERSON.stat[AGE1] = PERSON.stat[TYPE] == 14 ? 89 : 16;

    PERSON.stat[O_HITS] = PERSON.stat[HITS] = 1 + rand() % (PERSON.stat[CON] + PERSON.stat[STR]);

    for (t1 = 0; t1 < MAX_ITEMS; ++t1)
        PERSON.p_item[t1].i_type = 255;
    for (t1 = 0; t1 < CARRY_OVER; ++t1)
        PERSON.p_time[t1] = 0;

    sprintf(PERSON.exp, NSTR, 0);
    zfill(PERSON.exp);
}
```

### 1.6 Name validation — CONFIRMED source + Z80 xrefs

```c
get_name() {
    char temp_name[50], filename[13], temp[50];
    int t1, t2, t3, fd;

    clr();
    sleep(20);
    do {
        fd = -1;
        tab(0,0); eol();
        puts("What shall this character be called? ");  /* 0x12cc */
        sscanf(gets(temp), "%s", temp_name);
        if (valid_name(temp_name, filename)) {          /* name already exists */
            tab(2,0);
            puts("This name is in use, would you like");
            puts(" to delete it ? ");                    /* 0x12f5, 0x1319 */
            if (toupper(sgetchar()) == 'Y')
                unlink(filename);
            else
                fd = 0;
            tab(2,0); eol();
        }
    } while (((t2 = strlen(temp_name)) == 0) || (t2 > MAX_NAME) || (fd != -1));

    strcpy(PERSON.p_name, temp_name);
}
```

- `valid_name()` maps a character name to a host filename; the Z80 check uses `BC+0x32` and a filesystem call at `0x237a`.
- `'Y'` response is `0x59`; uppercase via `toupper()`.

---

## 2. Character file (`.CHR`) field map

The character record is **640 bytes**. Source-level fields confirmed from the recovered C:

| Source field | Meaning | Confirmed/uncertain details |
|---|---|---|
| `PERSON.stat[0..5]` | STR, INT, WIS, CHA, CON, DEX | 6 × 16-bit ints; produced by `compute_stats()` |
| `PERSON.stat[SEX]` | M/F flag | set randomly; mask `0x18` |
| `PERSON.stat[ALI]` | alignment bits | `1=L,2=N,4=C`; forced C for Ur-vile |
| `PERSON.stat[TYPE]` | race/type index 0–14 | see type table |
| `PERSON.stat[CLASS]` | class index 0–14 | see class table |
| `PERSON.stat[XMULT]` | experience multiplier | modified by `xclass.factor[XMULT] / 200` |
| `PERSON.stat[AGEC]` | age change/clock factor | modified by `xclass.factor[AGEC] / 10` |
| `PERSON.stat[I_1]`, `I_2` | item/ability flag words | OR'ed with class factors |
| `PERSON.stat[LVL]` | level | initialized 0 |
| `PERSON.stat[DLVL]` | deepest dungeon level | initialized -1 |
| `PERSON.stat[X]`, `Y` | dungeon coordinates | initialized -1 |
| `PERSON.stat[HIPAGE]` | current hi-page | initialized -1 |
| `PERSON.stat[AGE1]` | age counter | 89 for type 14, else 16 |
| `PERSON.stat[HITS]`, `O_HITS` | current/max hit points | `1 + rand() % (CON + STR)` |
| `PERSON.p_name[]` | character name | from `get_name()` |
| `PERSON.class[]` | class name string | copied from `xclass[t].xname` |
| `PERSON.type[]` | type name string | displayed in stats |
| `PERSON.p_dungeon[]` | dungeon name | from `get_dungeon()` (`make_filename`) |
| `PERSON.p_dungeoVeteran[]` | veteran flag/name | appears in source fragment, meaning partial |
| `PERSON.gold[]` | gold, zero-filled ASCII | `rand() % (200 - tot)` |
| `PERSON.exp[]` | experience, zero-filled ASCII | initialized `"0"` |
| `PERSON.p_mspell` | magic-unit pool | `0xffff` = enabled for Mage/Minstrel/Ranger/Sage |
| `PERSON.p_cspell` | cleric-unit pool | `0xffff` = enabled for Cleric/Demondim/Paladin/Ranger/Raver/Sage |
| `PERSON.p_ac` | armor class | initialized 10 |
| `PERSON.p_item[MAX_ITEMS]` | inventory | each `i_type = 255` (empty) at creation |
| `PERSON.p_time[CARRY_OVER]` | carry-over time array | zeroed at creation |

**Raw byte offsets — PARTIAL / UNCERTAIN.** Z80 code uses a global base pointer at `0x0115` plus offsets:
- `+0x300` = start of the in-memory `xclass`/`xtype` table arena (15 records × `0x48` bytes); `+0x738` = table count. This is runtime scratch, not the saved 640-byte record.
- Name/type/class strings are written near `+0x73`, `+0x92`; other touched offsets include `+0xa7`, `+0xe5`, `+0xe9`, `+0xed`, `+0xef`, `+0x107`.
- The `ARAGORN.CHR` sample is a genuine 640-byte saved character and **is** a usable layout oracle (the earlier "placeholder/garbage" assessment was wrong). Confirmed directly from its bytes:

  | Offset | Width | Value / meaning |
  |---|---|---|
  | `0x12` | ASCIIZ | character name (`"aragorn"`) |
  | `0x27` | ASCIIZ | type name (`"Hobbit"`) |
  | `0x31` | ASCIIZ | class name (`"Peasant"`) |
  | `0x3b` | 10 chars | gold, zero-padded decimal (`"0000000117"`) |
  | `0x46` | 10 chars | experience, zero-padded decimal (`"0000000000"`) |
  | `0x50..0x5b` | 6 × BE16 | STR=12, INT=10, WIS=11, CHA=12, CON=8, DEX=15 |
  | `0x5c` | BE16 | level (0) |
  | `0x5e` | BE16 | age (16) |
  | `0x64` | BE16 | current HITS (5) |
  | `0x66` | BE16 | O_HITS / maximum HITS (8) |
  | `0x89` | LE16 | per-slot identified bits; bit N = slot N (all zero in sample) |
  | `0x8b` | LE16 | second flag word, tested/cleared by the pawn path; meaning UNCERTAIN |
  | `0xa3 + slot*0x25 + 0x20` | byte | inventory item type; `0xff` = empty slot |

  The inventory records are 37 bytes each.  In `ARAGORN.CHR` their non-type
  bytes happen to contain a CP/M setup script, but every slot's `+0x20` byte is
  `0xff`, confirming both the slot stride and the empty-slot marker.

**Save/load direction — CONFIRMED:** `NEMESIS.COM 0x093f` reads five 128-byte
records into runtime `0x8047` via `0x2090`; `CASTLE.COM 0x1b12` writes the same
buffer back via `0x4820` on exit.  The runtime globals map to file offsets by
subtracting `0x8047`, so `0x80d0` is `.CHR` offset `0x89` and `0x80ea` is
offset `0xa3`.  The C# reader/writer therefore persists identified bits and
inventory item types there while keeping all other bytes in `RawData` for
byte-exact round-trips.  The exact meaning of the `0x8b` flag word and of the
remaining non-type inventory bytes is still UNCERTAIN.

---

## 3. `CASTLE.COM` — main menu and branches

### 3.1 Main loop / dispatch — CONFIRMED from strings + xrefs

Strings at `0x97a` onward:

```
Welcome to the castle
        Please select
        A) Armory: Items bought and sold
        B) Hospital: Wounds healed
        C) Inn: Wine, Wenches and Rest
        D) The Archives
        E) Entrance to Dungeon
        F) Display Character
        G) Exit Nemesis
```

Pseudocode:

```
load CASTLE.0 or print "Error can not load castle.0."
loop:
    clear screen; print "Welcome to the castle" + A-G menu
    ch = toupper(getchar())
    switch ch:
        'A' -> armory()
        'B' -> hospital()
        'C' -> inn()
        'D' -> archives()
        'E' -> check dungeon/character; chain to DUNGEON.COM; print "Enter through NEMESIS."
        'F' -> display_character()
        'G' -> save character (or "Unable to save character!!" at 0x1b81); exit
```

String xrefs found: main menu `LD HL,0x10e2 @ 0x0f6a`? (hospital menu), `LD HL,0x12af @ 0x1204` (inn), `LD HL,0x14d2 @ 0x1332` (archives), `"Enter through NEMESIS." @ 0x7e1`, `"castle." @ 0x955/0x970`, `"dungeon" @ 0xa78`, `"armory" @ 0xb01`.

### 3.2 Armory / Pawn Shop

Strings:

```
Welcome to the Armory
A) Sell items.
B) Buy items.
What is your choice ?
You have  gold pieces.
What item would you like to buy ?
Sorry you can not afford that item.

Welcome to the Pawn Shop
You have  gold pieces
Which item do you wish to sell ?
I'll give you %u gold pieces for that item
```

Item data records recovered from `ITEMS.C-0` give item type, level, name, and a numeric field likely price/weight. Examples:

```
4 3 SWORD 0 0 2 10 0 0 0 0 -1
4 3 MACE 4 0 1 10 0 0 0 0 -1
4 1 MAGIC_TORCH 14 0 0 5 1 0 0 0 -1
4 4 LEATHER 1 -1 0 10 0 0 0 0 -1
4 2 SHIELD 3 -1 0 10 0 0 0 0 -1
4 2 WAR_HAMMER 7 0 2 10 0 0 0 0 -1
4 2 MAIL 9 -2 0 10 0 0 0 0 -1
4 4 DAGGER 11 1 1 10 0 0 0 0 -1
4 1 PLATE 10 -3 0 15 0 0 0 0 -1
```

- **CONFIRMED:** sell price is printed via `%u` (`"I'll give you %u gold pieces..."`), so it is computed and displayed, not a fixed table string.
- **CONFIRMED (see `EXACT-FORMULAS.md` 3.2/3.3):** buy price and the identified
  pawn base use the composite `500*|AC| + 100*damage + price` item value;
  identified sales add `LFSR % 20`, while unidentified sales use `LCG48 % 70`.
  The identified flag is the `0x80d0` bit array indexed by inventory slot
  (0..9), set when an item is bought, cleared when it is sold, and toggled by
  the archives disclosure path.  Only the time-seeded LCG's initial value remains
  environment-specific. The save buffer starts at runtime `0x8047`, so the
  identified bit word maps to `.CHR` offset `0x89` and is persisted; the C#
  port serializes `Character.ItemIdentified` there.

### 3.3 Hospital

Strings:

```
Welcome to the Hospital
You have  gold pieces
You are in perfect physical shape
Your wounds will take %u days to heal
At a rate of %u gold per day
Please enter the length of your stay --
As you are unable to pay your bill
 we will have to break your legs !!
```

Behavior inferred:
- If at full HP: `"You are in perfect physical shape"`.
- Otherwise compute days needed and a per-day gold rate; display both.
- Player enters stay length; if gold insufficient, refuses and prints the leg-breaking message.
- **UNCERTAIN:** the exact rate formula (e.g. `rate = f(level)`) and healing-per-day amount.

### 3.4 Inn

Strings:

```
Welcome to the Inn
Restoring magic and cleric unit
...
```

Manual context: the Inn restores HP more slowly than the Hospital and is the **only** place that restores spent Cleric and Magic units. Cost/restoration formulas not recovered before wrap-up.

### 3.5 Archives

Strings include `"How many gold pieces do you wish to donate ? "`.

**CONFIRMED:** Archives disclose unidentified items either by spending a DISCLOSE magic/cleric spell or by donating gold. The gold path at `0x1332` requires the donation to exactly equal `item_value(slot) / 2` for the selected inventory slot A-J, then toggles that slot's `0x80d0` bit (XOR). The final debit of the donation from the gold string remains UNCERTAIN.

### 3.6 Dungeon entrance

- Validates character/dungeon, then chains to `DUNGEON.COM`.
- Prints `"Enter through NEMESIS."` (`0x7e1`) in the castle context.
- `"Error can not load castle.0."` if the castle data file is missing.

### 3.7 Display character

Strings at `0x2035`+:

```
Level: %u Hits: %u Expr:
Age: %u AC: %d Gold:
Str/Int/Wis/Cha/Con/Dex
Magic units
Cleric units
```

Displays all fields from the character record (level, hits, experience, age, AC, gold, six attributes, spell pools, inventory indirectly).

### 3.8 Exit

- Saves character back to disk.
- Error string: `"Unable to save character!!"` at `0x1b81`.
- Failure likely due to read-only disk / file-create error (matching `PERSON.COM` strings `"We have encountered a file create error !!"` and `"We have encountered a disk write error !!!"`).

---

## 4. Summary of confirmed vs uncertain

**Confirmed:**
- Full creation call sequence.
- Attribute formula `max(1, roll() + 2*type_mod)` and gold formula `rand() % (200 - total)`.
- All 15 race/type attribute modifiers.
- All 15 class minimum requirements and alignment masks.
- Class validation logic, alignment/sex encoding, XP/age modifier application.
- `init_char()` spell pools, AC, level/coords/age/HP initialization.
- Name validation loop and delete-on-collision behavior.
- Castle A-G menu structure and all menu/shop/hospital/inn/archive/error strings.
- Dungeon chain and exit-save behavior at a high level.

**Uncertain / not completed:**
- Exact 640-byte on-disk field offsets beyond the decoded gold/EXP/stats/level/age/HP/magic/cleric, identified-bit (`0x89`), and inventory type (`0xa3 + slot*0x25 + 0x20`) fields.
- Inn rest cost and restoration amounts.
- The final gold debit after an Archives donation, the meaning of the required `inventory[slot] + 4` word, and the second flag word at `.CHR` offset `0x8b`.
- Some class constructor “later raw fields” (108/24/100...) are almost certainly XP/age/item flags but were not decoded to named constants.

No source files were modified; only this report was written.
