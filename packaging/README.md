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
  launches it. `{disk}` = autostart target, `{image}` = disk path for mounting.
  By default it mounts drive 8 and autostarts (e.g. `x64sc -8 "{image}" -autostart "{disk}"`;
  `-8 <image>` is injected when needed). No selection → autostart the disk/`*`
  (first file); one selected file → autostart `disk:NAME` (normalized); a
  **$0801 BASIC** program adds `-autostartprgmode 1` (LOAD ,8,1 style) when
  absent. Paths are shell-quoted safely.
- **CBM → OpenCBM Transfer…** opens the structured `d64copy` dialog
  (`OpenCbmWindow`): D64/D71 selection, device number, transfer mode and a live
  command preview. Arguments are passed via `ProcessStartInfo.ArgumentList`
  (no shell). This needs OpenCBM installed and a real drive to do anything.

## Not included

- Native OS **print** dialog (Avalonia has no reliable cross-platform print);
  "Print Directory…" instead writes a printable text listing.
- Built-in **auto-update** installer — "Check for Updates" only reads a version
  feed URL (configurable in settings).
