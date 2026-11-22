# DUNGEON.COM logic analysis (Nemesis)

Status: **partial but actionable**. This report distinguishes code-confirmed facts from inferred behavior. The linear disassembly is treated as primary control-flow truth; Ghidra C output was used only cautiously because several function boundaries/decompilations in the combat and event regions are unreliable.

Inputs used:

- `docs/nemesis/disasm-DUNGEON.txt` — primary linear disassembly.
- `docs/nemesis/decomp-DUNGEON.c` — noisy Ghidra output, used cautiously.
- `docs/nemesis/game-files/DUNGEON.COM` — binary (32,512 bytes).
- `docs/nemesis/nemesis-manual-ocr.txt` — rules, controls, spell names, feature descriptions.
- `strings -n 4 game-files/DUNGEON.COM` / saved xrefs in `/tmp/opencode/dungeon_strings_xref.txt`.
- Source fragments in `/tmp/opencode/nemesis-source/ALL-SOURCE-FRAGMENTS.txt` and `LEVEL.*.txt`, especially character generation and `init_rand()` call order.
- BDOS call inventory in `/tmp/opencode/bdos-out.txt`.

Address convention: **runtime address = file offset + 0x100**.

---

## Confidence legend

- **[confirmed]** — directly visible in disassembly/source fragments or strongly xref-backed.
- **[high]** — clear from code + strings/manual, with only minor ambiguity.
- **[medium]** — plausible reconstruction; helper routine semantics not fully decoded.
- **[low]** — inferred from names/manual or incomplete arithmetic trace; verify before porting.

---

## 1. Program setup and RNG usage

### Confirmed startup/source sequence

The interleaved C source fragments show this setup chain:

```c
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
```

The binary entry path is consistent with this:

- `0x0100`: sets up stack, calls `0x034b`, then `0x079e`, then `JP 0x042b`.
- `0x079e` contains the game setup/loop initialization; loop head appears at `0x07d1`.
- Main command dispatch is anchored at `0x099e`.

### RNG — `rand()` at `0x6d72` **[confirmed algorithm, seed unknown]**

The actual generator is visible in the linear disassembly at `0x6d72`, not in the noisy Ghidra function around `0x6c75`.

State:

- 32-bit LFSR stored little-endian in memory:
  - `0x0597` = low word
  - `0x0599` = high word

Feedback:

- XOR of bits **3** and **6** of the low byte at `0x0597`.

Step:

```c
uint32_t rng_state; /* 0x0597 low word, 0x0599 high word */

uint16_t rand(void)
{
    uint8_t b0 = rng_state & 0xff;
    unsigned fb = ((b0 >> 3) ^ (b0 >> 6)) & 1;
    rng_state = ((rng_state << 1) & 0xffffffffu) | fb;

    /* Return 15-bit value: low byte unchanged, high byte masked to 7 bits. */
    return (uint16_t)(((rng_state >> 8) & 0x7f) << 8 | (rng_state & 0xff));
}
```

Equivalent: `return rng_state & 0x7fff`, implemented bytewise as “high byte AND 0x7F, low byte unchanged”.

Known callers include:

- `0x1a1b`, `0x1a7d`, `0x1aa0`, `0x1abb`
- `0x28e9` — combat/damage-related
- `0x2f54`, `0x2f66` — random event/item check
- `0x71fe`, `0x720b`, `0x7238`, `0x734f` — pit/shaft/flame hazards

**[uncertain]** Exact `init_rand()` seed source was not recovered. The source fragments only prove it is called before character creation. Hazards call `0x6d41(n)` for random-looking delays/pauses; that is a separate delay/random-wait helper, not `rand()` itself.

### Character generation evidence **[confirmed]**

`init_char()` source fragment:

- Spell-user masks by class.
- `p_ac = 10`
- `LVL = 0`
- `DLVL = X = Y = HIPAGE = -1`
- Age `89` if type `14`, otherwise `16`
- `HITS = O_HITS = 1 + rand() % (CON + STR)`
- All item types initialized to `255`
- `p_time[] = 0`
- Experience string zeroed

`compute_stats()` source fragment:

- Six stats: `roll() + 2 * xtype.factor[t1]`, clamped to minimum `1`
- Sex: `rand() % 2`
- Starting gold: `rand() % (200 - tot)`

Display format strings at `0x46e8`, `0x4702`, and `0x4718` confirm the character-sheet labels:

- `Level: %u Hits: %u Expr:`
- `Age: %u AC: %d Gold:`
- `Str/Int/Wis/Cha/Con/Dex`
- `Magic units`, `Cleric units`

Globals strongly associated with the sheet:

- `0x8098`–`0x80a2`: STR/INT/WIS/CHA/CON/DEX **[high]**
- `0x80a4`: experience **[high]**
- `0x80a6`: current HITS/HP **[confirmed by pit/death code]**
- `0x80a8`: magic units **[high]**
- `0x80aa`: cleric units **[high]**
- `0x80bc`: current dungeon level **[confirmed]**

---

## 2. Main game loop

### High-level flow **[confirmed structure]**

Pseudocode for the persistent game loop (anchors `0x079e`, `0x07d1`, `0x099e`):

```text
initialize_game():
    getdef()
    clr()
    init_rand()
    create_character()
    save_character()
    enter_main_loop()

main_loop():
    while true:
        # Pre-command refresh/state update
        clear small local scratch frame
        stage = call_2da3(level)          # level/context bookkeeping
        save stage byte to 0x9fa5
        call_106b()                       # replot/redraw current view
        call_0190()                       # low-level refresh/input helper
        if returned value != 0:
            print diagnostic string (0x0ce1 region)

        if level < 0:
            reset display/position state
            goto command_wait

        call_0ee5(row, col)
        if failed:
            call_0f4b(row, col)
            call_0da4(row, col)           # draw local room/area

        call_156a()                       # update visibility/explored map

command_wait:
        call_2f4c()                       # random item/event check
        key = call_3b92()                 # console input/command read

        switch key:
            U / Ctrl-K (0x0b) -> move_north()
            D / Ctrl-J (0x0a) -> move_south()
            R / Ctrl-L (0x0c) -> move_east()
            L / Ctrl-H (0x08) -> move_west()
            P (0x50)          -> call_106b()          # replot
            H (0x48)          -> call_1087()          # hardcopy
            ESC (0x1b)        -> display_character()
            M (0x4d)          -> magic_spell_prompt()
            C (0x43)          -> cleric_spell_prompt()
            '+' (0x2b)        -> stairs(-1)          # ascend
            '-' (0x2d)        -> stairs(+1)          # descend
            default           -> ignore / re-prompt
```

Notes:

- Before command dispatch, the loop repeatedly refreshes view state using `0x156a`, `0x0da4`, `0x0ee5`, and `0x0f4b`.
- The exact semantics of `0x0190`, `0x0ee5`, and `0x0f4b` are not fully decoded; they appear to be generic 16-bit arithmetic/memory helpers used pervasively, not game rules themselves. **[medium]**
- `CALL 0x2f4c` immediately before input is the random event/item generator. **[confirmed]**

---

## 3. Movement and position handling

### Globals **[confirmed]**

- `0x80b8` = column / X
- `0x80ba` = row / Y
- `0x80bc` = dungeon level

Bounds visible in the dispatch code:

- Rows: `0 .. 0x15` (0–21)
- Columns: `0 .. 0x19` (0–25)

This matches a 26×22 map grid.

### Dispatch details **[confirmed]**

| Key | Address | Behavior |
|---|---:|---|
| `U` / Ctrl-K `0x0b` | `0x0b08` | If row > 0, call `0x4228(row-1,col)`, then `0x0cf3`, then decrement `0x80ba`. |
| `D` / Ctrl-J `0x0a` | `0x0b3c` | If row < 0x15, call `0x4228(row+1,col)`, then `0x0cf3`, then increment `0x80ba`. |
| `R` / Ctrl-L `0x0c` | `0x0b6f` | If col < 0x19, call `0x42a3(row,col+1)`, then `0x0cf3`, then increment `0x80b8`. |
| `L` / Ctrl-H `0x08` | `0x0ba2` | If col > 0, call `0x42a3(row,col-1)`, then `0x0cf3`, then decrement `0x80b8`. |
| `P` | `0x0bd6` | Replot via `0x106b`. |
| `H` | `0x0bdc` | Hardcopy via `0x1087`. |
| ESC | `0x0bff` | `CALL 0x431e`, then `CALL 0x307a` — display character. |
| `M` | `0x0c08` | `CALL 0x1b9d` — magic spell prompt. |
| `C` | `0x0c1b` | `CALL 0x1d51` — cleric spell prompt. |
| `+` | `0x0c2e` | Stairs lookup, `0x59ea`, `0x170e(-1)` — ascend. |
| `-` | `0x0c7d` | Same lookup, `0x170e(1)` — descend. |

### Movement algorithm **[confirmed control flow]**

```text
move(dir):
    target = current + delta(dir)

    if target outside bounds:
        goto invalid_move   # 0x0cd2

    if vertical_move:
        result = call_4228(target_row, col)   # passability/encounter helper
    else:
        result = call_42a3(row, target_col)   # passability/encounter helper

    result2 = call_0cf3(result)               # post-move event/validation

    if result2 == 0:
        goto invalid_move

    commit target to 0x80ba/0x80b8
    goto after_move                           # 0x0ccf
```

### Passability helpers **[high]**

`0x4228` handles vertical moves; `0x42a3` handles horizontal moves.

Observed pattern in `0x4228`:

```text
4228(row, col):
    compute map pointer = 0x980f + row*0x4e + col*3
    read map cell byte via 0x59ea(...)
    mask byte with 0xff
    call 0x5be8(0, flags_at_0xa2f8)
    if blocked -> return 3
    else       -> return 0
```

`0x42a3` is the symmetric horizontal version.

**[medium]** The map cell byte encodes terrain/passability; `0x59ea` appears to normalize or rotate/map the raw byte. The exact bit assignments were not fully recovered.

---

## 4. Dungeon level loading and map display

### Level transitions **[confirmed]**

`0x170e(delta)`:

```text
change_level(delta):
    set a transition flag at 0xa2fc = -1
    call 3a96()                         # screen/mode reset
    level = level_at_0x80bc + delta
    store back to 0x80bc

    call 439a()                         # transition/cleanup

    if level < 0:
        return failure

    if level > 999 (0x3e7):
        ask confirmation via strings at 0x184f / 0x1867
        if declined -> loop/retry
        else allow/continue

    call 598b(level, template/name args...)   # load level data
    if failed:
        retry from 0x7238-style fallback
    else:
        call 2da3(level)                # update stage context
        store stage byte to 0x9fa5
        call 3a96()
        call 106b()                     # redraw
        return success
```

The `+`/`-` handlers look up a stairs byte from:

```text
0x980f + row*0x4e + col*3
```

then call `0x59ea` and pass `-1` or `+1` to `0x170e`.

Level-transition strings xref’d around `0x14b3`, `0x14f7`, `0x152b`, `0x1889`, `0x18be`, `0x18f2`, and `0x1929`.

### Map buffer layout **[confirmed]**

- Base: `0x980f`
- Row stride: `0x4e` = 78 = 26 cells × 3 bytes
- Column stride: `3`
- Cell pointer: `0x980f + row*78 + col*3`

So each map cell has **3 bytes**, likely glyph/attribute/flags. **[high]**

### Redraw pipeline **[high]**

- `0x106b` — normal replot:
  ```text
  0xa2fe = 0                # hardcopy file handle/state disabled
  call 0x3a96               # reset/prepare screen
  call 0x10bb               # draw character/status header
  call 0x0da4(row, col)     # draw local dungeon view
  ```
- `0x1087` — hardcopy path:
  ```text
  save old display state
  0xa2fe = output file handle
  call 0x432f               # create/open HARDCOPY
  call 0x10bb
  call 0x4372               # write/close HARDCOPY
  restore state
  call 0x0da4(row, col)
  ```
- `0x0da4(row,col)` draws the visible room/neighborhood. It probes north/south/east/west via `0x4228`/`0x42a3`, then uses lookup tables at `0x9f8f`, `0x9f97`, etc. with `0x3a6a` to resolve display characters.
- `0x156a` updates the current cell and visibility/exploration flags:
  ```text
  cell = 0x980f + row*0x4e + col*3
  store cell pointer in local frame
  clear flag words at cell+4 and cell+6
  set pointer at cell+2 to 0x8347
  loop over neighbors (B up to 0x10):
      combine flag words from neighbor tables
      if nonzero: propagate/update visibility
  ```

### ESC character display **[confirmed action, display details medium]**

ESC dispatch at `0x0bff` calls:

- `0x431e` — `CALL 0x3a96`, `CALL 0x453d`, `CALL 0x4774(0)`
- `0x307a` — additional character display/refresh helper

This is a “display character” command; exact screen layout is not fully decoded.

### Map feature tags from strings **[confirmed strings, glyph mapping uncertain]**

Feature tags found around `0x49d9`–`0x4a08`:

- `dark`
- `tport`
- `shaft`
- `illsn.`
- `fire`
- `melee`
- `spec.`

These likely label the map legend / feature overlay, but exact glyph-to-feature mapping was not recovered.

---

## 5. Combat

Combat code is the least completely reconstructed area. Several helpers (`0x0190`, `0x020f`, `0x023f`, `0x6c75`) are multiprecision/arithmetic routines, and Ghidra output in this region is unusable. The control flow and string xrefs are clear; exact arithmetic formulas are **partially inferred**.

### Combat entry / command prompt **[confirmed]**

`0x79d1` prints:

```text
"Enter attack command: "   @ 0x7ab8
```

Then reads a command via `0x3b92` and dispatches:

| Input | Address | Meaning |
|---|---:|---|
| `F` | `0x79e8` | Fight / melee attack. Calls `0x25dc(...)`; on failure advances a pointer at `0xa2f2` by 5. |
| `M` | `0x7a39` | Magic spell: pushes `2`, calls `0x1b9d`, then battle callback `0x7e68(0x7cad, 0)`. |
| `C` | `0x7a59` | Cleric spell: pushes `2`, calls `0x1d51`, then battle callback `0x7e68(0x7cad, 0)`. |
| `U` | `0x7a79` | Use item (partially traced). |
| other | `0x7aac` | loop/re-prompt |

### Observed combat strings **[confirmed]**

- `0x265f`: `"   You gave it %u hits"`
- `0x26b7`: `"   You killed 1"`
- `0x274e`: `"slept"`
- `0x27a7`: `"killed"`
- `0x2819`: `"  You cured %d hits."`
- `0x2c42`: `"You %s 1"`
- `0x2c5c`: death message
- `0x7ab8`: `"Enter attack command: "`

### Combat algorithm — working model **[medium/low formulas]**

```text
combat_loop(monster):
    while monster_alive and player_alive:
        cmd = prompt("Enter attack command: ")

        if cmd == 'F':
            if attempt_physical_attack(monster) succeeds:
                damage = roll_physical_damage()
                apply damage to monster HP
                print "You gave it %u hits"
            else:
                monster gets a counterattack / round advances

        elif cmd == 'M':
            spell = magic_spell_prompt(context=2)   # 0x1b9d
            if spell != cancel:
                resolve spell effect via battle callback 0x7e68(0x7cad, 0)

        elif cmd == 'C':
            spell = cleric_spell_prompt(context=2)  # 0x1d51
            if spell != cancel:
                resolve spell effect via battle callback 0x7e68(0x7cad, 0)

        elif cmd == 'U':
            use_item()

    if monster dead:
        print "You killed 1"
        award experience (uses 0x80a4, level 0x80bc, monster table near 0x838f)
        return victory

    if player HP (0x80a6) <= 0:
        print death message
        call death/cleanup 0x2c4b
        return defeat
```

### Formula evidence and caveats

Confirmed anchors:

- `0x28ba` reads the payout base from `0x838f + ...`, calls `rand()`, divides `10` by that base via `0x023f`, then takes `rand() % (10 / base)` via `0x020f` and adds the base. Superseded by the operand-aware decoding in `EXACT-FORMULAS.md` section 1.5. **[confirmed]**
- `0x25ab` combines experience (`0x80a4`) and a stat/global (`0x80a2`, likely DEX) and adds `100` (`0x64`). This looks like an experience/level threshold calculation, not direct melee damage. **[medium]**
- `0x26c7` on kill also reads `0x80a4` and level `0x80bc`, indexing a table near `0x838f`; consistent with **experience award**. **[high]**
- `0x2b64` prints `"You %s 1"` and calls `0x6d41(1)`, likely a short delay/animation after a kill. **[medium]**
- `0x80a6` is current HP: pit damage subtracts/adds it, and `<= 0` calls `0x2c4b`. **[confirmed]**

Working (but **unverified**) porting model:

```text
to_hit  ≈ rand() + DEX/level/weapon bonuses >= monster_AC
damage  ≈ max(0, (rand() % 10) + STR/weapon/spell bonus)
healing ≈ rand-based roll clamped to max HP
EXP     ≈ monster-table value scaled by level, added to 0x80a4
```

Do not treat these formulas as byte-exact yet; the helper routines must be decoded before a faithful port can rely on them.

---

## 6. Spells

### Prompt dispatch **[confirmed]**

- Magic prompt: `0x1b9d`
  - String `"What magic spell: "` at `0x1d3c`, xref `0x1bdb`.
  - String table setup region around `0x5029`–`0x53a4`.
- Cleric prompt: `0x1d51`
  - String `"What cleric spell: "` at `0x1ef0`, xref `0x1d8f`.
  - String table setup region around `0x5457`–`0x58c5`.
- Generic/combat spell prompt: `0x7594`
  - String `"What spell: "` at `0x774b`, xref `0x7594`.

Both prompt routines:

1. Optionally seed a local argument/context at `+0x14`.
2. Print the prompt via `0x3a6a`.
3. Read a line via `0x3e2d`.
4. Match input against spell names in a table.
5. Return a spell index in `HL`; `0` means cancel/fail. **[high]**

### Spell lists **[confirmed names; effects partly manual-inferred]**

Cleric spells:

| # | Name | Likely effect / evidence |
|---:|---|---|
| 1 | `LIGHT` | Light/visibility. **[medium]** |
| 2 | `CURE` | Healing; combat string `"You cured %d hits."` at `0x2819`. **[high]** |
| 3 | `DAMAGE` | Direct damage. **[high]** |
| 4 | `PROTECT` | Defensive buff. **[medium]** |
| 5 | `HEAL` | Healing. **[high]** |
| 6 | `CLEANSE` | Remove adverse condition/poison. **[low/medium]** |
| 7 | `RESTORE` | Restore stats/condition. **[low/medium]** |
| 8 | `ATTACK` | Offensive combat spell. **[high]** |
| 9 | `DISPELL` | Dispel magic/effects. **[medium]** |
| 10 | `HOLY WORD` | Powerful offensive/turning spell. **[medium]** |
| 11 | `SILENCE` | Prevent casting/noise; combat string association uncertain. **[medium]** |
| 12 | `DISCLOSE` | Identify an item; manual describes DISCLOSING items. **[high]** |

Magic spells:

| # | Name | Likely effect / evidence |
|---:|---|---|
| 1 | `LIGHT` | Light/visibility. **[medium]** |
| 2 | `DAMAGE` | Direct damage. **[high]** |
| 3 | `SLEEP` | Sleep status; combat string `"slept"` at `0x274e`. **[high]** |
| 4 | `SHIELD` | Defensive buff. **[medium]** |
| 5 | `STRENGTH` | STR buff. **[medium]** |
| 6 | `FIREBALL` | Area/direct damage. **[high]** |
| 7 | `LIGHTNING` | Direct damage. **[high]** |
| 8 | `SPEED` | Speed buff. **[medium]** |
| 9 | `LEVITATE` | Movement/levitation. **[medium]** |
| 10 | `FEAR` | Fear status. **[medium]** |
| 11 | `CLOUDKILL` | High-damage attack. **[high by name]** |
| 12 | `POWER WORD` | High-damage attack. **[high by name]** |
| 13 | `TELEPORT` | Move player. **[high by name]** |
| 14 | `DISCLOSE` | Identify item. **[high]** |
| 15 | `FIRE PROTECTION` | Protect against flame/fire. **[high by name]** |

**[uncertain]** Exact spell index-to-cost, level requirement, duration, and damage dice tables were not transcribed. The string-table regions are identified but not fully decoded.

### Spell effects visible in code/strings **[confirmed associations]**

- `"slept"` xref `0x274e` — SLEEP-like effect resolves in combat.
- `"killed"` xref `0x27a7` — lethal spell/attack result.
- `"You cured %d hits."` xref `0x2819` — healing/cure effect.
- Combat spell path passes context `2` to `0x1b9d`/`0x1d51`, then calls `0x7e68(0x7cad, 0)` to resolve.

---

## 7. Doors, traps, treasure, gold, pits, shafts, flame

### Random event / item generation **[confirmed]**

`0x2f4c`:

```text
maybe_event():
    if rand() % 20 != 0:       # LD DE,0x14; compare with rand()
        return

    value = rand()
    store via 0x9f73
    read map/feature byte from 0x900f + ... + 0x25
    if byte == 0:
        return

    print "You have found: "            # 0x3048, xref 0x2fa3
    print item name from table at 0x8002 + index*2
    print " Do you wish to keep the item?" # 0x3059, xref 0x2fce
    read answer
    if yes:
        add item
    else:
        leave/reject item
```

Manual confirms:

- If carrying the maximum 10 items, the game displays the character and asks which item to drop.
- Dropped items are unrecoverable.
- Generic lowercase names in the dungeon may be special; `DISCLOSE` reveals true identity.

### Doors / passability **[high]**

Doors are handled through the same map-cell/passability helpers as movement:

- `0x4228` / `0x42a3` read the cell byte and call `0x5be8` with a flags word at `0xa2f8`.
- A return of `3` appears to mean blocked. **[medium]**

Exact door-opening key/state behavior was not recovered.

### Pits **[confirmed messages, damage formula partial]**

- `0x711a`: `"You have passed over a pit"` — safe pass-over.
- `0x7125`: `"You have fallen into a pit: "` — damage path.

Damage path:

```text
pit_fall():
    print "You have fallen into a pit: "
    roll random damage via 0x6c75/rand path
    read HP at 0x80a6
    subtract/add delta, store back
    call 0x6d41(2)                     # pause
    print result string at 0x71f3
    if HP <= 0:
        call 0x2c4b                    # death
```

### Shafts **[confirmed control flow]**

- `0x7257`: prints shaft message at `0x72c3`.
- Increments `0x80bc` (level).
- Loads new level via `0x598b(...)`.
- Calls `0x2da3(level)`, updates `0x9fa5`, replots via `0x106b`.

### Flame **[confirmed message]**

- `0x730f`: `"You have been engulfed by flame !!"`
- Nearby hazard code at `0x734f` uses RNG and adjusts `0x80a6` (HP).

### Treasure / gold **[mixed]**

- Gold pickup formula was **not** recovered.
- Gold is displayed in the character sheet (`"Age: %u AC: %d Gold: "`) and initialized randomly in `compute_stats()`.
- Treasure/item discovery at `0x2f4c` is confirmed.
- **[low]** Gold piles may use the same `rand() % 20` event path, but this was not verified.

---

## 8. Key BDOS calls and higher-level meaning

`CALL 0x0005` is the CP/M BDOS trap. Function number is in `C`.

| Call site | C | CP/M BDOS function | Higher-level meaning |
|---:|---:|---|---|
| `0x0391` | 11 | Console status | Check for input during startup/test path. |
| `0x039b` | 1 | Console input | Read char if status says ready. |
| `0x0463` | 16 | Close file | Close hardcopy/output file. |
| `0x0569` | 26 | Set DMA | Set DMA buffer to `0x80` for file I/O. |
| `0x6bb2` | 15 | Open file | Open character/dungeon file; code writes `COM` into FCB type at `+9`. |
| `0x6c52` | 26 | Set DMA | Set DMA to `0x100`, likely loader/overlay buffer. |
| `0x6c59` | 20 | Read sequential | Read record into current DMA. |
| `0x6c67` | 26 | Set DMA | Restore DMA to `0x80`. |
| `0x6ca9` | 9 | Print string | Print error/message string near `0x5a97`. |
| `0x6d5a` | 11 | Console status | Input polling with delay loop. |
| `0x6d66` | 1 | Console input | Read char; Ctrl-C jumps to `0x0000` warm boot. |
| `0x6dc4` | 19 | Delete file | Delete generated/temp file. |
| `0x6df3` | 26 | Set DMA | Set DMA to `0x80` before read. |
| `0x6e1a` | 21 | Write sequential | Write HARDCOPY/file record. |
| `0x6e71` | 16 | Close file | Close after write. |
| `0x6e87` | 15 | Open file | Reopen for read/verify. |
| `0x6ecc` | 26 | Set DMA | Set DMA before another read. |
| `0x6ed6` | 20 | Read sequential | Read file record. |
| `0x6f4e` | 15 | Open file | Open file path in character I/O. |
| `0x6f8c` | 1 | Console input | Read char; Ctrl-C resets, `0x1a` is EOF, CR triggers newline. |
| `0x6fa6` | 2 | Console output | Emit newline/character after CR. |
| `0x6fec` | dynamic | Dynamic BDOS call | `C` loaded from variable `0x059f`; exact function depends on runtime state. **[uncertain]** |
| `0x7004` | 22 | Make file | Create character/HARDCOPY file. |

Overall: DUNGEON is a conventional CP/M 2.2 program using BDOS for console I/O and simple sequential ASCII file I/O. Character and dungeon files use `.COM`-style FCB type setup at `0x6baa`. Level data is loaded through the `0x598b` helper, which wraps filename construction + file open/read. **[high]**

---

## 9. Practical notes for a C# port

1. Implement the RNG exactly first. Every random gameplay decision depends on the 32-bit LFSR above; replacing it with `System.Random` will diverge immediately.
2. Preserve the 26×22 cell grid with 3 bytes per cell. The map pointer formula is reliable.
3. Treat `0x80b8`, `0x80ba`, and `0x80bc` as authoritative X/Y/level state.
4. Recreate command dispatch before trying to perfect combat formulas; movement and UI flow are well understood.
5. Combat/spell arithmetic needs another focused pass on helper routines `0x0190`, `0x020f`, `0x023f`, `0x026b`, `0x02cb`, `0x02fa`, and `0x6c75`. Ghidra output for these is not trustworthy.
6. Hazard behavior (pit/shaft/flame) is a good next fidelity target because strings and state transitions are clear.

---

## 10. Explicitly unresolved items

- `init_rand()` seed source.
- Exact `roll()` implementation used in character generation.
- Exact melee to-hit and damage formulas.
- Exact spell cost/level/duration/damage tables.
- Exact bit layout of the 3-byte map cell.
- Door open/close state machine.
- Gold pickup formula.
- Precise visual layout of the ESC “display character” screen.
- Meaning of dynamic BDOS call at `0x6fec`.

This report should therefore be used as a **control-flow and behavior map**, not yet as a byte-exact rules specification for the unresolved areas.
