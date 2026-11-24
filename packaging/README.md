# Packaging and OS integration

D64Editor itself is the Avalonia build in `D64Editor.App`. This directory contains optional helpers
for desktop integration and file associations.

## Linux file associations

- `d64editor.desktop` — desktop entry for launching the app.
- `install-associations.sh /path/to/D64Editor.App` — installs the `.desktop` entry, MIME types,
  and default-app associations under `~/.local` (no root required). Extensions covered:
  `.d64`, `.d71`, `.d81`, `.d80`, `.d82`, `.dnp`, `.g64`, `.t64`, `.lbr` → `application/x-*`.
- `install-associations.sh <exe> --uninstall` — reverses the installation.

## Windows file associations

`d64-file-association.reg` registers the same disk-image extensions with the extracted
`D64Editor.App.exe`. Edit the install path in the `.reg` file before importing it with:

```text
regedit /s d64-file-association.reg
```

## Emulator integration (inside the app)

`Run → Configure Emulators…` sets a command template; `Run in Emulator…` launches it. Available
placeholders:

| Placeholder | Meaning |
|---|---|
| `{disk}` | Autostart target (selected file, or disk/`*`). |
| `{image}` | Disk image path for mounting. |
| `{appdir}` | Application directory. |
| `{app}` | Application executable. |
| `{cwd}` | Current working directory. |

Substitution handles whole quoted tokens and substrings inside quotes, e.g.
`" {appdir}/file.crt"`. The default behavior mounts drive 8 and autostarts:

```text
x64sc -8 "{image}" -autostart "{disk}"
```

- No selection → autostart the disk/`*` (first file).
- One selected file → autostart `disk:NAME`, normalized for VICE.
- A `$0801` BASIC program adds `-autostartprgmode 1` when absent.
- `.crt` cartridges use `-cartcrt` (not `-cartepyx`).
- Paths are shell-quoted safely.

## OpenCBM transfer (inside the app)

`CBM → OpenCBM Transfer…` opens the structured `d64copy` dialog (`OpenCbmWindow`). It provides:

- D64 vs D71 (`-2`) format selection.
- Device number and transfer mode (original, serial1, serial2, parallel).
- Live command preview and PATH tool detection.
- Confirmation before the destructive image → drive direction.

Arguments are passed with `ProcessStartInfo.ArgumentList`, so no shell quoting is involved. Reads
are non-destructive. OpenCBM and a supported cable/drive are required.

## Not included

- Native OS print dialog. Avalonia has no reliable cross-platform print API; "Print Directory…"
  writes a printable text listing and opens it in the default handler.
- Built-in updater/installer. "Check for Updates" only reads the configurable release-feed URL
  and opens the releases page.
