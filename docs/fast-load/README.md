# Epyx FastLoad cartridge reference fixture

## What this directory contains

`Epyx_FastLoad_1984_v2-b11b3b.crt` is an 8,272-byte Commodore 64 cartridge image identified by
`file(1)` as:

```
Commodore 64 cartridge: "EPYX FASTLOAD", Epyx Fastload, packet length 0x2010
```

SHA-256:

```
4cff9d1beb8ae0931fed2a66bc4389ede75f93981c2be434e9aed233dc9e71d7
```

The image was moved here from the repository root so that all non-product reference media lives
under `docs/`. The moved bytes are identical to the previously committed root-level file; this is
a pure relocation, not a re-capture or modification. No D64Editor Core/App code, CLI verb, build
target, or embedded resource references the cartridge. The only prior source-tree reference was
the fixture list in `AGENTS.md`, which now points here.

## Why it is kept

Epyx FastLoad is a commercial C64 cartridge with a fast LOAD routine and utility commands. It is
useful here only as a local reproduction/cross-check fixture:

- It gives a known-good commercial fast loader for VICE comparisons against the custom drive-code
  loaders seen on RUSSIA and EA PRODOS/Arctic Fox reference disks.
- It helps separate "the disk image is unreadable" from "the host-side emulator/console path or
  accelerator interaction is wrong".
- It is not part of D64Editor's product behavior and is not embedded in the application.

This fixture does **not** make high-level OpenCBM capable of emulating arbitrary bit-banged drive
code. The RUSSIA check uses the drive DOS `U1` command (no `M-E`), while EA PRODOS/PirateSlayer
uploads code to drive RAM and reads raw GCR from VIA2; those paths must still be analyzed
separately. See `docs/drive-assembly/README.md` and
`docs/disk-images/arcticfox/README.md`.

## Local VICE usage

From a source checkout, pass the CRT with VICE's `-cartcrt` option. D64Editor also uses
`-cartcrt` when the selected emulator target is a `.crt` file.

```bash
# Boot a disk with the cartridge inserted.
x64sc -cartcrt docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt -8 disk.d64

# Autostart a PRG while the cartridge is inserted.
x64sc -cartcrt docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt \
  -8 disk.d64 -autostart program.prg -autostartprgmode 1
```

In D64Editor's `Run → Configure Emulators…` template, an equivalent development-checkout command
is:

```
x64sc -cartcrt "{cwd}/docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt" -8 "{image}" -autostart "{disk}"
```

`{cwd}`, `{image}`, and `{disk}` are documented in the root `README.md`. Path substitution is
shell-quoted; do not add a second quoting layer around the template argument.

On this machine's Xwayland session, VICE GTK3 startup can be flaky. Use the software-renderer
settings documented in `docs/TOOLS.md` §4 if the window fails to appear.

## Verification

The relocation can be checked against the previous committed object:

```bash
cd /home/pat/source/repos/D64Editor
git show HEAD~1:Epyx_FastLoad_1984_v2-b11b3b.crt | sha256sum
sha256sum docs/fast-load/Epyx_FastLoad_1984_v2-b11b3b.crt
# both: 4cff9d1beb8ae0931fed2a66bc4389ede75f93981c2be434e9aed233dc9e71d7
```

`HEAD~1` is only correct for the commit that performed the move; use the pre-move commit hash if
inspecting this later.

## Scope and rights

This is a binary artifact from a commercial product. It is retained as a local development fixture
only; copyright/trademark rights remain with their respective holders. Treat it as reference data,
not as source of truth for 1541 format behavior. No D64Editor code imports, extracts, or modifies
the CRT at runtime.

Cross-references: `AGENTS.md`, `README.md` (Emulator setup), `docs/README.md`, `docs/CODE.md`
(fixture assets), `docs/TOOLS.md` (VICE workflows and safety), `docs/drive-assembly/README.md`.
