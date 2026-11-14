PETSCII/CBM64 fonts live here (e.g. `C64_Pro_Mono-STYLE.ttf`, bundled). The
app uses them for PETSCII text and, together with `Assets/chargen.bin` (the
VICE C64 character ROM), for exact ROM-bitmap rendering.

See `D64Editor.App/Fonts.cs` for how the font is referenced — to swap fonts,
update the file and the embedded family name (`fc-scan <font> | grep family`)
and keep `PetsciiFontAvailable` set. The global charset (upper/graphics vs
lower/upper) is toggled via **View → PETSCII Lower/Upper Case** in the app.
