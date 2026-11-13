#!/usr/bin/env bash
# Registers D64 Editor and its extensions under ~/.local/share (no root needed).
#
# Usage:
#     ./install-associations.sh /path/to/D64Editor.App
#     ./install-associations.sh /path/to/D64Editor.App --uninstall
set -euo pipefail

APP_EXE="${1:?Usage: $0 /path/to/D64Editor.App [--uninstall]}"
UNINSTALL="${2:-}"

APPS_DIR="$HOME/.local/share/applications"
MIME_DIR="$HOME/.local/share/mime/packages"
DESKTOP="$APPS_DIR/d64editor.desktop"

ext_for_mime() {
    case "$1" in
        d64) echo ".d64 .D64";;
        d71) echo ".d71 .D71";;
        d81) echo ".d81 .D81";;
        d80) echo ".d80 .D80";;
        d82) echo ".d82 .D82";;
        dnp) echo ".dnp .DNP";;
        g64) echo ".g64 .G64";;
        t64) echo ".t64 .T64";;
        lbr) echo ".lbr .LBR";;
        *)  echo "";;
    esac
}

if [[ "$UNINSTALL" == "--uninstall" ]]; then
    rm -f "$DESKTOP"
    rm -f "$MIME_DIR/d64editor.xml"
    update-mime-database "$HOME/.local/share/mime" 2>/dev/null || true
    update-desktop-database "$APPS_DIR" 2>/dev/null || true
    echo "Uninstalled D64 Editor file associations."
    exit 0
fi

mkdir -p "$APPS_DIR" "$MIME_DIR"

# 1) Desktop entry (Exec points at the published executable)
sed "s|Exec=d64editor %f|Exec=${APP_EXE} %f|" "$(dirname "$0")/d64editor.desktop" > "$DESKTOP"
update-desktop-database "$APPS_DIR" 2>/dev/null || true

# 2) MIME registrations (glob → x-d64 etc.), so "Open With D64 Editor" appears.
{
  echo '<?xml version="1.0" encoding="UTF-8"?>'
  echo '<mime-info xmlns="http://www.freedesktop.org/standards/shared-mime-info">'
  for ext in d64 d71 d81 d80 d82 dnp g64 t64 lbr; do
      exts="$(ext_for_mime "$ext")"
      echo "  <mime-type type=\"application/x-${ext}\">"
      for e in $exts; do echo "    <glob pattern=\"*${e}\"/>"; done
      echo "    <comment>Commodore disk image</comment>"
      echo "  </mime-type>"
  done
  echo '</mime-info>'
} > "$MIME_DIR/d64editor.xml"
update-mime-database "$HOME/.local/share/mime" 2>/dev/null || true

# 3) Associate default app with those types.
for ext in d64 d71 d81 d80 d82 dnp g64 t64 lbr; do
    xdg-mime default d64editor.desktop "application/x-${ext}" 2>/dev/null || true
done

echo "Installed D64 Editor file associations (Linux)."
echo "You may need to log out/in (or run: xdg-mime default d64editor.desktop application/x-d64)"
