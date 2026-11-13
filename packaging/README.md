# Packaging & OS integration

Packaging helpers for the cross-platform release (the app itself is the Avalonia
build in `D64Editor.App`).

## File associations

- **Linux** — `install-associations.sh /path/to/D64Editor.App` installs a
  `.desktop` entry, MIME types (`.d64/.d71/.d81/.d80/.d82/.dnp/.g64/.t64/.lbr`
  → `application/x-*`), and the default-app associations under `~/.local` (no
  root). `install-associations.sh <exe> --uninstall` reverses it.
- **Windows** — import `d64-file-association.reg` (`regedit /s …`) after editing
  the install path to point at your extracted `D64Editor.App.exe`.

## Emulator / OpenCBM (from inside the app)

- **Run → Configure Emulators…** sets a command template; **Run in Emulator…**
  substitutes `{disk}` with the open image and launches it (default `x64sc -autostart "{disk}"`).
- **CBM → OpenCBM Transfer…** detects `cbmcopy`/`cbmctrl` on `PATH` (say "not
  found" otherwise) and runs your command with `{device}/{disk}/{out}` substituted.
  These need the emulator or OpenCBM installed and a real drive to do anything.

## Not included

- Native OS **print** dialog (Avalonia has no reliable cross-platform print);
  "Print Directory…" instead writes a printable text listing.
- Built-in **auto-update** installer — "Check for Updates" only reads a version
  feed URL (configurable in settings).
