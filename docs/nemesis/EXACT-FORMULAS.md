# Nemesis exact-formula recovery (work in progress)

Status: **partial recovery; do not treat UNCERTAIN entries as final**. Reported formulas are decoded by hand from the original `.COM` bytes. Linear disassemblies are cross-checks only: `CALL 0x0190` consumes a one-byte inline operand, so raw bytes must be decoded with that byte skipped. Ghidra C output was used only as a secondary cross-check where noted, never as control-flow truth.

Primary inputs:

- `docs/nemesis/disasm-DUNGEON.txt`
- `docs/nemesis/disasm-CASTLE.txt`
- `docs/nemesis/game-files/DUNGEON.COM`
- `docs/nemesis/game-files/CASTLE.COM`
- Prior notes: `DUNGEON-LOGIC.md`, `CASTLE-CHARACTER.md`, `DATA-FORMATS.md`

Address convention: **runtime address = file offset + `0x100`**.

## Confidence legend

- **CONFIRMED** — directly visible in hand-decoded disassembly; operands, shifts, and helper semantics were traced.
- **HIGH** — control flow/data tables are clear, but one naming/semantic layer (for example, which character-sheet field a global represents) is inferred from prior corroborated notes.
- **UNCERTAIN** — partially traced or plausible only; needs more disassembly or runtime verification before porting.

---

## Calling convention and helper routines used below

The compiler uses `BC` as a frame pointer. After a callee's `PUSH BC` and local-frame allocation, the first caller argument is at `BC + localsSize + 4`. `CALL 0x0190` takes a **inline 1-byte operand** immediately following the call; it reads `HL = word[BC + inline_u8]`. Raw linear disassembly misaligns immediately after every such call, so all ranges below were re-decoded with the inline byte emitted as `.db`.

Important helpers (DUNGEON.COM; analogous helpers exist in CASTLE.COM):

| Address | Recovered meaning | Confidence |
|---:|---|---|
| `0x0190` | `HL = word[BC + inline_u8]` (inline operand after CALL) | CONFIRMED |
| `0x01e5` | 16-bit unsigned equality, `HL == DE` | CONFIRMED |
| `0x01fa` | signed compare/equality (`HL` vs `DE`; carries on `HL < DE`) | CONFIRMED |
| `0x0204` | signed compare `HL` vs `DE` (`DE` is negated before common compare path) | CONFIRMED |
| `0x0205` | signed compare `HL` vs `DE`; `C` set iff `HL < DE`, `Z` iff equal | CONFIRMED |
| `0x020f` | signed remainder. With entry `HL = divisor`, `DE = dividend`, returns `DE % HL` with the sign of `DE` (C-truncated remainder) | CONFIRMED |
| `0x023f` | signed multiply (`HL *= DE`, sign corrected from `0x0571`) | CONFIRMED |
| `0x025a` | two's-complement magnitude and sign counter at `0x0571` | CONFIRMED |
| `0x026b` | unsigned multiply (`HL *= DE`) | CONFIRMED |
| `0x0289` | unsigned divide. With entry `HL = divisor`, `DE = dividend`, returns `DE / HL` | CONFIRMED |
| `0x02cb` | signed divide. With entry `HL = divisor`, `DE = dividend`, returns trunc-toward-zero `DE / HL` | CONFIRMED |
| `0x02fa` | `HL = -HL` | CONFIRMED |
| `0x6c75` | 48-bit LCG; arg `1` advances state and returns a 15-bit positive value | CONFIRMED |
| `0x6d72` | 15-bit LFSR `rand()` at `0x0597`/`0x0599`, feedback bits 3^6; returns `state & 0x7fff` | CONFIRMED |

### The 48-bit LCG at `0x6c75` — CONFIRMED

This is a second, independent generator from the LFSR. `CALL 0x6c75` with
argument `1` (many call sites, e.g. `0x2888` inside `damage_roll`) advances it.
The multiplier is a six-byte data table embedded in code bytes at
`0x6d2f`–`0x6d34`: `c5 87 01 0e 9a e0` little-endian = `0xe09a0e0187c5`.
The 48-bit state lives at `0x6d35`–`0x6d3a` and starts as `1` in the `.COM`
image. The step routine (`0x6cbf`–`0x6d2d`) multiplies the state by the
constant, negates the product modulo 2^48, and returns the top 15 bits of the
new state (`state[5] & 0x7f` as the high byte, `state[4]` as the low):

```c
state = (-state * 0xe09a0e0187c5ULL) & 0xffffffffffffULL;
return (state >> 32) & 0x7fff;
```

First five outputs from the boot state `1` (verified by byte-level emulation of
the Z80 multiply loop):

| step | state | return |
|---:|---|---:|
| 1 | `0x1f65f1fe783b` | 8037 |
| 2 | `0x43dfe38b5d99` | 17375 |
| 3 | `0x0303abcc4a43` | 771 |
| 4 | `0x9a6a1c5e8571` | 6762 |
| 5 | `0xc9bd9f73b90b` | 18877 |

The port is `Nemesis/Engine/Lcg48Random.cs`; it drives both `roll()` in
character generation and the exact melee damage roll below.  The combat and
payout formulas from sections 1.1–1.4 are now applied in
`Nemesis/Engine/CombatRules.cs` and `DungeonEngine`.

Important globals:

| Address | Meaning used here | Confidence |
|---:|---|---|
| `0x80a2` | DEX stat (last of STR/INT/WIS/CHA/CON/DEX at `0x8098`–`0x80a2`) | HIGH |
| `0x80a4` | experience | HIGH |
| `0x80a6` | current HP / HITS | CONFIRMED |
| `0x80a8` | magic units | HIGH |
| `0x80aa` | cleric units | HIGH |
| `0x80bc` | current dungeon level | CONFIRMED |
| `0x80d6` | magic-spell level gate mask/value | HIGH |
| `0x80d8` | cleric-spell level gate mask/value | HIGH |
| `0xa2e4` | current monster-instance pointer | CONFIRMED |
| `0xa2e6` / `0xa2e8` | magic / cleric spell tables installed at startup | CONFIRMED |
| `0xa2ea` | game-state word used by the `25ab` formula | HIGH |
| `0xa2ec` / `0xa2ee` | spell-name-table counters used by magic/cleric prompts | HIGH |
| `0x838f` | monster template base | CONFIRMED |

---

# 0. Monster template layout (DM.COM loader)

The `MONSTERS.*` files are plain text: `<level> <name> <11 numeric fields>`.
`DM.COM` parses them with the format string `%d %s %d %d %d %d %d %d %d %d %d %d`
at runtime `0x1ffa`, storing the 11 numbers as consecutive words in a scratch
buffer at `BC+0x8d .. BC+0xa1` (name string at `BC+0x64`, name pointer at
`BC+0xa5`).  Its record builder at `0x1e14`–`0x1f3c` copies each parsed value
into a 16-byte template record at `0x838f + 16*index`:

| Template offset | Source token | Example (`Veteran`) | Width | Used by |
|---:|---|---:|---|---|
| `+0x00` | name pointer | — | word | display |
| `+0x02` | field 1 | 10 | byte | armour class (HIGH) |
| `+0x03` | field 2 | 5 | byte | maximum group size |
| `+0x04` | field 3 | 1 | byte | minimum group size |
| `+0x05` | field 4 | 5 | byte | maximum attack value |
| `+0x06` | field 5 | 1 | byte | minimum attack value |
| `+0x07` | field 6 | 13 | byte | maximum per-monster HP |
| `+0x08` | field 7 | 5 | byte | minimum per-monster HP |
| `+0x09` | field 8 | 8 | **word** | `monsterPower` in melee |
| `+0x0b` | field 9 | 1 | byte | kill-payout base `v` |
| `+0x0c` | field 10 | 0 | word | UNKNOWN |
| `+0x0e` | field 11 | 0 | word | UNKNOWN |

Confidence: **CONFIRMED** for the loader offsets and widths, and for the
semantics of fields 2-9 once DUNGEON.COM's creation (`0x7acc`) and turn
(`0x2954`) routines are decoded with `CALL 0x0190` inline operands skipped.
Field 1 remains HIGH; fields 10-11 remain UNKNOWN.  The C# catalogue's
`F1..F11` map 1:1 to source fields 1..11; `F12` is a trailing placeholder that
the original text format does not contain.

The signed-remainder helper at `0x020f` computes `DE % HL` (dividend in DE,
divisor in HL).  The signed-divide helper at `0x023f` computes `DE / HL`.
Mistaking their operand order is what produced the earlier incorrect `10*v`
payout note.

---

# 1. Combat: melee damage / “to hit”

## 1.1 Fight argument `F`

Address range: DUNGEON.COM `0x7dbf` vicinity (argument setup), call site `0x7a13`.

The value passed to the melee routine is computed as:

```c
// word(0xa2f6) is the single argument used by 0x25dc.
int16_t f = (int16_t)((word(0x8098) + word(0x80a2)) / 10);  // signed /10
word(0xa2f6) = f;
```

Then the `F` command calls:

```c
result = melee_attack(word(0xa2f6));   // CALL 0x25dc at 0x7a13
```

Confidence: **HIGH**. The division and store are visible; the exact semantic label “to-hit bonus” is inferred from the call into the damage roll. `0x8098` is STR and `0x80a2` is DEX per prior corroborated sheet work.

## 1.2 Damage roll helper — `0x2853(arg1, arg2)`

Address range: DUNGEON.COM `0x2853`–`0x28b9`.

Hand-decoded stack order: because the caller pushes `arg2` first and `arg1` last, `BC+0x08` is `arg1` and `BC+0x0a` is `arg2`.

```c
uint16_t damage_roll(int16_t arg1, int16_t arg2)
{
    // Sign-corrected arg2 / 2; NOT 2 / arg2.
    int16_t half = signed_div(arg2, 2);
    int16_t sum  = arg1 + half;

    if (sum <= 0)
        return 0;

    uint16_t r = rnd_6c75(1);          // 0..0x7fff
    uint16_t x = signed_rem(r, sum);  // 0x020f: r % sum (r positive here)
    return x > 0 ? x : 0;
}
```

Confidence: **CONFIRMED**. The `LD DE,2; EX DE,HL; CALL 0x02cb` sequence proves the divisor is the literal `2`; the earlier working note `2 / arg2` is superseded.

## 1.3 Melee attack — `0x25dc(f)`

Address range: DUNGEON.COM `0x25dc`–`0x269f`.

```c
#define KILLED 0xffff
#define MISS_OR_ALIVE 0

uint16_t melee_attack(int16_t f)
{
    MONSTER *m = (MONSTER *)word(0xa2e4);
    int16_t type = word(m + 4);
    int16_t monsterPower = word(0x838f + 16 * type + 9);

    // This is the actual damage applied to the monster.
    uint16_t hits = damage_roll(f, monsterPower);   // CALL 0x2853

    word(m + 6) -= hits;                           // 0x02fa + ADD HL,DE
    printf("   You gave it %u hits", hits);        // string 0x26a0

    if (word(m + 6) <= 0) {                        // compare updated HP with 0
        printf("   You killed 1");                 // string 0x26b7
        award_kill_payout(m);                      // CALL 0x28ba
        monster_turn(0xffff);                      // CALL 0x2954
        return KILLED;
    }
    return MISS_OR_ALIVE;
}
```

Confidence:

- Roll, subtraction, print, kill branch, and return values: **CONFIRMED**.
- Meaning of `monster+6` as current monster HP: **HIGH** (consistent with the rest of the combat code and prior notes).
- Meaning of `0x838f + 16*type + 9` as a monster power/damage field: **CONFIRMED** by the DM.COM loader (section 0 shows field 8 is stored there as a word).
- Calling this quantity “to-hit” is misleading in porting terms: the same number is subtracted from monster HP, so it is the damage applied. The original game's displayed phrase is “hits”.

## 1.4 Monster creation — `0x7acc`

Address range: DUNGEON.COM `0x7acc`–`0x7c8a`.  This routine allocates a 10-byte
live monster record and fills it from the loaded template table.  `word(0x838d)`
is the number of records loaded from `MONSTERS.N-M` for the current dungeon
level by `0x598b`.

```c
MONSTER *m = alloc(10);
if (!m) { error("..."); return; }

// Live record layout:
//   +0 next pointer, +2 remaining group size, +4 template index,
//   +6 current HP, +8 current attack value
int type = rnd_6c75(1) % word(0x838d);
m->type = type;

int f2 = byte(0x838f + 16*type + 3);   // field 2, group max
int f3 = byte(0x838f + 16*type + 4);   // field 3, group min
m->group = f3 + rnd_6c75(1) % (f2 - f3);

int f6 = byte(0x838f + 16*type + 7);   // field 6, HP max
int f7 = byte(0x838f + 16*type + 8);   // field 7, HP min
m->hp = f7 + rnd_6c75(1) % (f6 - f7);

int f4 = byte(0x838f + 16*type + 5);   // field 4, attack max
int f5 = byte(0x838f + 16*type + 6);   // field 5, attack min
m->attack = f5 + rnd_6c75(1) % (f4 - f5);
```

All four random draws come from the 48-bit LCG in exactly the order shown.
Confidence: **CONFIRMED** (every offset and draw is visible once the inline
operands after `CALL 0x0190` are skipped).  With the boot LCG state the first
four outputs are `8037, 17375, 771, 6762`; for 14 records and the `Veteran`
template this yields type `1`, group `4`, HP `8`, attack `3`.

## 1.5 Kill payout — `0x28ba(typeOrMonsterPtr)`

Address range: DUNGEON.COM `0x28ba`–`0x2947`.

```c
uint16_t v = byte(0x838f + 16 * type + 11);  // template +0x0b, field 9
uint16_t r = rand();                          // 0x6d72, 0..0x7fff
uint16_t payout = v + signed_rem(r, signed_div(10, v));

write_decimal_padded(word(0x8082), payout, "%010u");  // CALL 0x62ab/0x5a3c/0x5b5b
write_decimal_padded(word(0x808d), payout, "%010u");
```

`0x023f` is signed `DE / HL`; with `HL = v`, `DE = 10` it computes `10 / v`.
`0x020f` is signed `DE % HL`; with `HL = 10 / v`, `DE = rand()` it takes the
remainder.  For `v > 10` the divisor is zero and the payout is just `v`.

Confidence:

- Formula `v + rand() % (10 / v)`: **CONFIRMED**.  The earlier `10 * v` reading
  was wrong because the linear disassembly did not skip the inline operand after
  `CALL 0x0190`.
- Destination buffers `0x8082`/`0x808d`: **CONFIRMED** as addresses written.
- Which buffer is gold versus experience: **UNCERTAIN**; both receive the same formatted payout in this path.

## 1.6 Monster-turn helper — `0x2954(arg)`

Address range: DUNGEON.COM `0x2954`–`0x2a62`.

The `0xffff` argument is passed from the kill path at `0x268a`. The routine:

1. Decrements the word at `currentMonster + 2` (remaining group size); if it
   becomes zero, returns `0xffff` immediately (group exhausted).
2. Otherwise draws a fresh per-monster HP into `+6` and a fresh attack value
   into `+8`, in that order, using the same bounded-LCG idiom as creation:

```c
int f7 = byte(0x838f + 16*type + 8);   // HP min
int f6 = byte(0x838f + 16*type + 7);   // HP max
m->hp = f7 + rnd_6c75(1) % (f6 - f7);

int f5 = byte(0x838f + 16*type + 6);   // attack min
int f4 = byte(0x838f + 16*type + 5);   // attack max
m->attack = f5 + rnd_6c75(1) % (f4 - f5);
return 0;
```

The byte at `+8` is also tested before the refresh (`CALL 0x4429` when
non-zero), which is consistent with a special-attack/status flag; that branch
remains UNCERTAIN.  Confidence: **CONFIRMED** for the group decrement and both
stat refreshes; **UNCERTAIN** for the special-attack branch.

## 1.7 Other combat helper — `0x25ab(arg)`

Address range: DUNGEON.COM `0x25ab`–`0x25d9`.

```c
// r is positive 15-bit from 0x6c75(1)
uint16_t r = rnd_6c75(1);
int32_t product = (int32_t)word(0x80a2) * (int32_t)word(0x80a4); // DEX * EXP
int32_t scaled  = product * arg;
uint16_t result = 100 + signed_rem(r, scaled);
```

Confidence: **HIGH**. The multiply order and `+100` are visible. This is not the melee damage roll; it is a separate scaled random value called from item/effect paths around `0x1fb2` and `0x2566`.

---

# 2. Spell costs and gating

## 2.1 Spell record layout

Both spell tables use 5-byte records:

```c
struct SpellRecord {
    uint16_t namePtr;   // +0..+1
    uint8_t  cost;      // +2
    uint8_t  flags;     // +3  (context/use mask)
    uint8_t  level;     // +4
};
```

Tables:

- Cleric base `0xa240`, installed from setup region `0x5014`–`0x58c5`.
- Magic base `0xa290`, installed from setup region `0x5014`–`0x590a`.

Confidence: **CONFIRMED** for the 5-byte stride and field offsets (`+2`, `+3`, `+4` are written directly by setup code and read by the prompt routines).

## 2.2 Magic prompt — `0x1b9d(context)`

Address range: DUNGEON.COM `0x1b9d`–`0x1d3b`.

Key decoded sequence:

```c
int magic_prompt(int context)
{
    // ... prompt/read/name match against table 0xa290, stride 5 ...
    int idx = matchedIndex;

    if (idx == word(0xa2ec)) return 0;                 // 0x01fa equality gate
    if (!level_check(word(0x80d6), table[idx].level)) return 0; // CALL 0x5be8
    if (word(0x80a8) < table[idx].cost) return 0;                // signed compare
    if ((table[idx].flags & context) == 0) return 0;

    word(0x80a8) -= table[idx].cost;                    // 0x02fa + ADD
    call_1f06(table[idx].level);                        // effect setup
    return idx;
}
```

Confidence: **CONFIRMED** for cost subtraction, compare order, and flag mask test; **HIGH** for interpreting `level` as a level gate and `0x80d6` as the corresponding character value.

## 2.3 Cleric prompt — `0x1d51(context)`

Address range: DUNGEON.COM `0x1d51`–`0x1ef0`.

Parallel structure to magic prompt, with these substitutions:

```c
table   = 0xa240;
counter = word(0xa2ee);
gate    = word(0x80d8);
units   = word(0x80aa);
```

```c
if (idx == counter) return 0;
if (!level_check(gate, table[idx].level)) return 0;
if (units < table[idx].cost) return 0;
if ((table[idx].flags & context) == 0) return 0;
units -= table[idx].cost;
call_1f06(table[idx].level);
return idx;
```

Confidence: **CONFIRMED** for field offsets and units subtraction; **HIGH** for semantic naming.

## 2.4 Recovered spell tables

Indexes are 0-based as stored in the tables/prompt return values. Prior notes used 1-based display numbers; subtract 1 to map them here.

### Cleric table at `0xa240`

| idx | name | cost | flags | level |
|---:|---|---:|---:|---:|
| 0 | LIGHT | 2 | 1 | 0 |
| 1 | CURE | 4 | 1 | 1 |
| 2 | DAMAGE | 4 | 2 | 2 |
| 3 | PROTECT | 8 | 1 | 3 |
| 4 | HEAL | 8 | 1 | 4 |
| 5 | CLEANSE | 8 | 1 | 5 |
| 6 | RESTORE | 10 | 1 | 6 |
| 7 | ATTACK | 10 | 2 | 7 |
| 8 | DISPELL | 10 | 2 | 8 |
| 9 | HOLY WORD | 20 | 2 | 9 |
| 10 | SILENCE | 15 | 1 | 10 |
| 11 | DISCLOSE | 10 | 1 | 11 |

### Magic table at `0xa290`

| idx | name | cost | flags | level |
|---:|---|---:|---:|---:|
| 0 | LIGHT | 2 | 1 | 0 |
| 1 | DAMAGE | 4 | 2 | 1 |
| 2 | SLEEP | 4 | 2 | 2 |
| 3 | SHIELD | 6 | 1 | 3 |
| 4 | STRENGTH | 8 | 1 | 4 |
| 5 | FIREBALL | 10 | 2 | 5 |
| 6 | LIGHTNING | 10 | 2 | 6 |
| 7 | SPEED | 14 | 1 | 7 |
| 8 | LEVITATE | 16 | 1 | 8 |
| 9 | FEAR | 15 | 2 | 9 |
| 10 | CLOUDKILL | 20 | 2 | 10 |
| 11 | POWER WORD | 25 | 2 | 11 |
| 12 | TELEPORT | 20 | 1 | 12 |
| 13 | DISCLOSE | 10 | 1 | 13 |
| 14 | FIRE PROTECTION | 10 | 1 | 14 |

Confidence: **HIGH**. Names were read from the pointer targets in the binary; costs/flags/levels were decoded from the setup writes. A mechanical re-parse during this pass produced a few off-by-one artifacts around records 6–11; the table above uses the prior hand-decoded values, which are internally consistent with the prompt checks and the 12/15 spell counts.

## 2.5 Combat spell effects partially decoded

Address range: DUNGEON.COM `0x7e68` dispatcher; effect vector base `0x7cad`; callback `0x21c5`.

```c
// Call sites such as 0x779e push 0x7cad then 0/1.
// Last pushed is first arg.
battle_callback(int mode, uint16_t *vec)  // 0x7e68
{
    table = (mode == 0) ? word(0xa2e8) : word(0xa2e6); // 0: cleric, 1: magic
    // uses vec[0] etc.; vec[0] = 0x21c5 at 0x7cad
}
```

`0x21c5` calls `0x2770` and jumps `0x22a5`. `0x2770` is associated with the `"killed"` string at `0x27c3`.

Cure effect at `0x27ca`:

```c
int cure(int arg)
{
    int r = rnd_6c75(1) % arg;             // signed remainder helper
    int maxGain = word(0x80be) - word(0x80a6);
    if (r > maxGain) r = maxGain;
    printf("  You cured %d hits.", r);    // string at 0x283e
    word(0x80a6) += r;
    return 0xffff;
}
```

Confidence: `0x27ca` formula **CONFIRMED**; dispatcher mapping **HIGH** for mode/table selection but **UNCERTAIN** for the complete vector-to-spell-ID map.

---

# 3. CASTLE shop pricing

All ranges in this section are CASTLE.COM runtime addresses.

## 3.1 Armory item table basics

Confirmed from disassembly around `0x0d90`, `0x1595`, and `0x1733`:

- Item stride: `0x25` (37 bytes), base `0x80ea`.
- Availability byte: `byte[0x80ea + 37*i + 0x20] == 0xff` means item is not sold/available.
- Shop selection checks `arg < 10` around `0x0da6` (armory stock band).
- `0x1595(itemIndex)` reads `word[0x80ea + 37*i + 8]` into a local; this is the item's price/value word used by buy/sell paths.

Confidence: **CONFIRMED** for stride/offsets; **HIGH** for interpreting `+8` as price.

## 3.2 Armory buy price

Address range: armory buy path `0x0b4b`–`0x0d26` (string xrefs: `"Welcome to the Armory"` `0x0b01`, `"What item would you like to buy ?"` `0x0cd4`, `"Sorry you can not afford that item."` `0x0cfa`).

Recovered control flow:

```c
int price = word(item + 8);       // 0x80ea + 37*i + 8
if (gold() < price) {
    print("Sorry you can not afford that item.");
    return;
}
gold() -= price;
add_item_to_inventory(i);
```

A separate helper at `0x18ec` initializes a transaction record and compares a value against `word(0x9f73)` before committing; this looks like the buy-confirmation record rather than a different price formula.

Confidence: **HIGH** that the price is the item-table word at `+8`; **UNCERTAIN** whether any class/level multiplier is applied before the affordability test. I did not fully decode every branch in `0x0b4b`–`0x0d26`, so do not port a multiplier assumption from this report.

## 3.3 Armory / pawn sell price

Address range: pawn-shop path `0x0edc`–`0x0f6a` (strings `"Welcome to the Pawn Shop"` `0x0edc`, `"Which item do you wish to sell ?"` `0x0f0c`, `"I'll give you %u gold pieces for that item"` `0x0f2e`).

Path evidence:

```c
// 0x0eb3/0x0ebe/0x0ecb call helpers 0x38a7, 0x33fe, 0x343d
// on locals at +6 and on 0x8082 gold string buffer.
sell_value = item_price_from_inventory(item);
printf("I'll give you %u gold pieces for that item", sell_value);
gold += sell_value;
```

Confidence: **UNCERTAIN**. The string path and helper calls are located, but the exact arithmetic inside `0x38a7`/`0x33fe`/`0x343d` was not decoded in this pass.

## 3.4 Hospital pricing and healing

Address range: `0x0f5f`–`0x10e2` (strings `"Welcome to the Hospital"` `0x10e2`, `"You are in perfect physical shape"` `0x1111`, `"Your wounds will take %u days to heal"` `0x1133`, `"At a rate of %u gold per day"` `0x1159`).

Decoded core:

```c
if (word(0x80be) == word(0x80a6)) {       // max HP == current HP
    print("You are in perfect physical shape");
    return;
}

missing = word(0x80be) - word(0x80a6);    // 0x02fa + ADD at 0x0fa4..0x0faf
printf("Your wounds will take %u days to heal", days);
printf("At a rate of %u gold per day", rate);
```

Additional decoded helper at `0x1ab5`:

```c
// word(0x80cc) is a day counter; word(0x80ca) is gold.
word(0x80cc) += word(arg + 4);           // accumulate days
word(0x80ca) += word(0x80cc) / 0x16d;    // 365 days per year rollover
word(0x80cc) %= 0x16d;
```

Confidence:

- Full-HP early-out and `missing HP` calculation: **CONFIRMED**.
- Day-count rollover `/365` and `%365`: **CONFIRMED**.
- Exact `days` formula and `rate` formula: **UNCERTAIN**. The string xrefs and input prompt at `0x1176` (`"Please enter the length of your stay -- "`) are confirmed, but the arithmetic preceding the two `%u` prints was not fully decoded.

## 3.5 Inn pricing and restoration

Address range: `0x1204`–`0x1332` (strings `"Welcome to the Inn"` `0x12af`, `"Restoring magic and cleric units "` `0x12c2`, `"It will take you %u days to heal."` `0x12e4`, `"Do you wish to stay that long ?"` `0x1306`).

Decoded core:

```c
word(0x80a8) = word(0x80c0);   // magic units saved/restored
word(0x80aa) = word(0x80c2);   // cleric units saved/restored

missing = -(word(0x80a6)) + word(0x80be);   // 0x02fa + ADD at 0x122d..0x1238
stayCost = signed_mul(missing, 3);          // DE=3, CALL 0x023f at 0x1239..0x123f

if (stayCost == 0) {
    // no stay needed
    goto 0x12a6;
}
printf("It will take you %u days to heal.", stayCost);
// ... prompt "Do you wish to stay that long ?" at 0x1306 ...
```

Confidence:

- Copies of `0x80c0/0x80c2` into magic/cleric units: **CONFIRMED**.
- `missing HP * 3`: **CONFIRMED** as the value computed at `0x122d`–`0x123f`.
- Calling that value “days” is **HIGH** because the adjacent string says days; calling it “gold cost” is **UNCERTAIN**. The same value is used in the stay-confirmation flow, but the final debit path was not fully traced.

## 3.6 Archives donation

Address range: `0x1332`–`0x14d2` (strings `"Welcome to the Game Master's Archive"` `0x14d2`, `"How many gold pieces do you wish to donate ?"` `0x150e`, `"You don't have that much gold."` `0x1543`/`0x1542`).

Decoded fragments:

- Donation is read as a decimal string via helper at `0x1b9c`.
- Gold availability is checked and the error string printed when insufficient.
- Helper `0x1595(index)` indexes `word[0x80ea + 37*i + 8]`, i.e. the same item price word used by the armory.
- At `0x1465`–`0x1497`, a value is divided by `2` using signed divide `0x02cb` and compared against an item price before an inventory/availability update.

Confidence: **UNCERTAIN** for the exact donation threshold. The `/2` is confirmed; the operand provenance and the comparison direction were not fully resolved.

---

# 4. Explicitly unresolved items

These remain open and must not be guessed in a faithful port:

1. **Melee terminology** — whether the original source called `0x2853`'s result “to hit”, “damage”, or “hits” internally. Code truth: the value is subtracted from monster HP.
2. **`0x2954` special-attack branch** — what the non-zero byte at `monster+8` triggers via `0x4429`; the group decrement and both stat refreshes are now CONFIRMED.
3. **`0x28ba` payout buffers** — which of `0x8082`/`0x808d` is gold versus experience (the formula itself is CONFIRMED).
4. **Spell effect vector map** — complete mapping from `0x7cad` vector words to spell IDs; only `vec[0] = 0x21c5` was decoded.
5. **Spell level gate internals** — exact semantics of `0x5be8` against `0x80d6`/`0x80d8`.
6. **Context mask values** — why combat passes `2` yet several healing spells have flags `1`; needs runtime or source-fragment confirmation.
7. **Armory buy modifiers** — any class/level multiplier on `word[item+8]`.
8. **Pawn sell formula** — exact arithmetic in `0x38a7`/`0x33fe`/`0x343d` path.
9. **Hospital `days` and `rate`** — exact formulas before the two `%u` prints.
10. **Inn debit** — whether `missing HP * 3` is days, gold, or both, and where the final gold subtraction occurs.
11. **Archives donation threshold** — operand provenance for the confirmed `/2`.
12. **RNG seed used for the shipped `ARAGORN.CHR`** — the compiled default LFSR
    seed (`0xffff59dc`) and the interactive `init_rand()` path are known, but
    the seed actually entered for that saved character is not recorded.
    `roll()` itself is now confirmed (4d5, section 5).

---

## 5. PERSON.COM `roll()` — CONFIRMED 4d5

Address range: PERSON.COM `0x1a71`–`0x1ad1` (same runtime-library region as the
LCG entry at `0x360e`).

```c
int roll(void) {           /* stat dice in compute_stats() */
    int n = 4, sum = 0;
    while (n > 0) {
        sum += lcg_rand() % 5 + 1;   /* CALL 0x360e at 0x1aa3 */
        --n;
    }
    return sum;                       /* 4..20 */
}
```

Evidence: counter `4` stored at `0x1a7b`, four LCG calls with `DE % 5` plus one
at `0x1aa7`–`0x1aae`, accumulator at `BC+2` returned at `0x1ac2`. The LCG is the
same 48-bit generator as `DUNGEON.COM` (multiplier `0xe09a0e0187c5`, boot state
1). Ported as `Lcg48Random.RollStats()`; `CharacterGenerator` now uses it for
the six stats while the LFSR still drives sex/gold/HP.

With the boot state the six rolls are `9,15,8,8,12,11`, so the earlier 3d6
placeholder was wrong; `ARAGORN.CHR`'s stats require a custom seed or editing.

---

## Method note

Every arithmetic claim above was decoded from the original `.COM` bytes with an operand-aware pass that skips the one-byte inline operand after each `CALL 0x0190`; the old linear disassemblies are misaligned after those calls. Ghidra output was not used to resolve any disputed operand order. Source files are updated only where a formula is marked CONFIRMED.
