# Diagnostic disk images

Reference images used by the dynamic-trace and copy-protection investigation. These are test and
reference data only; copyright remains with the original publishers and preservation projects.

## Layout

| Directory | Contents |
|---|---|
| [`russia/`](russia/README.md) | RUSSIA (ID `GM`) preservation master NIB, nibconv G64, preserve/report/logs, write-back verification, and detailed protection analysis. |
| [`arcticfox/`](arcticfox/README.md) | Arctic Fox / EA PRODOS preservation artifacts, faithful SRQ NIB/G64, reproduction experiments, and detailed protection analysis. |
| [`arcticfox/generated/`](arcticfox/generated/README.md) | Arctic Fox capture/reproduction artifacts generated during investigation. |
| Kracker Jax (`kracker_jax_*`, `kjr.d64`) | Kracker Jax NBZ/D64 reference images used for TGREAD/fat-track/archive research. |

## Kracker Jax

| File | Description |
|---|---|
| `kracker_jax_revealed_trilogy_s1*.nbz`, `s2*.nbz` | Original NBZ images from the preservation archive. Side 2 contains `TGREAD`. |
| `kracker_jax_revealed_trilogy_s1*.d64`, `s2*.d64` | D64 versions produced with `--convert`. |
| `kracker_jax_vol1..vol7*.d64` | D64s decoded from the remaining Kracker Jax volumes by `tools/diagnostics/scan_kj.sh` during the "fat track" text search. |
| `kjr.d64` | Kracker Jax "Revealed Trilogy" side 1 decoded from NBZ; contains `KJ DOCS`, `TGREAD`, and loaders. |

## Regenerating decoded D64s

```bash
# NBZ -> D64 (all of them)
for f in *.nbz; do
  dotnet run --project D64Editor.App --no-build -- --convert "$f" "${f%.nbz}.d64"
done

# G64 -> D64
dotnet run --project D64Editor.App --no-build -- \
  --convert 'arcticfox/arcticfox[ea_1986](!).g64' arcticfox/af-g64.d64
```

VICE launchers and binary-monitor clients that use these images are in
`../../tools/diagnostics/README.md`. Drive-code/protection notes are in
`../drive-assembly/README.md`.
