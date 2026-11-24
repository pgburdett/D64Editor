# Diagnostic disk images

Reference images used by the dynamic-trace and copy-protection investigation. These are test and
reference data only; copyright remains with the original publishers and preservation projects.
They are included so the traces in `../drive-assembly/README.md` and
`../../tools/diagnostics/README.md` are reproducible without fetching an external archive.

## Arctic Fox (Electronic Arts / Dynamix, 1986)

| File | Description |
|---|---|
| `arcticfox[ea_1986](!).g64` | Preservation G64 from the C64 Preservation Project. Raw GCR, 84 half-track slots, no half-tracks present. Attached to emulated drive 8 with `-drive8truedrive` by `tools/diagnostics/launch-x64sc-g64.sh`. |
| `arcticfox[ea_1986](!).nbz` | Original MNIB-1541-RAW NBZ (LZ77-compressed) from the same project. |
| `af-g64.d64` | D64 decoded from the preservation G64 with `--convert`; 106 sectors undecodable because of protected areas. |
| `arcticfox.d64` | D64 decoded from the NBZ with `--convert`. |
| `kjr.d64` | Kracker Jax "Revealed Trilogy" side 1 decoded from NBZ; contains `KJ DOCS`, `TGREAD`, and loaders. |

## Kracker Jax

| File | Description |
|---|---|
| `kracker_jax_revealed_trilogy_s1*.nbz`, `s2*.nbz` | Original NBZ images from the preservation archive. Side 2 contains `TGREAD`. |
| `kracker_jax_revealed_trilogy_s1*.d64`, `s2*.d64` | D64 versions produced with `--convert`. |
| `kracker_jax_vol1..vol7*.d64` | D64s decoded from the remaining Kracker Jax volumes by `tools/diagnostics/scan_kj.sh` during the "fat track" text search. |

## Regenerating decoded D64s

```bash
# NBZ -> D64 (all of them)
for f in *.nbz; do
  dotnet run --project D64Editor.App --no-build -- --convert "$f" "${f%.nbz}.d64"
done

# G64 -> D64
dotnet run --project D64Editor.App --no-build -- \
  --convert 'arcticfox[ea_1986](!).g64' af-g64.d64
```

Generated and captured images are documented in `generated/README.md`. VICE launchers and
binary-monitor clients that use these images are in `../../tools/diagnostics/README.md`.
