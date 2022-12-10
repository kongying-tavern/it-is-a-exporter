# It is a exporter

Yes, it's just a exporter.

**This is a development tool only for developers.**

**If you don't know how to use it, you do not need it. Maybe.**

## How to use

1. Download latest [.NET 6+ SDK](https://dotnet.microsoft.com/en-us/download).
2. Use `.NET CLI` to install [dotnet-script](https://github.com/filipw/dotnet-script).
3. Download Studio version v0.18.60.
4. Use `AssetStudioCLI.exe` in Studio to build **`assets_map_GI.json`(must be json)**.
5. Clone or download this repository.
6. Tell `./scripts/main.csx` whereis `assets_map_GI.json` and `AssetStudioCLI.exe`(in the directory of Studio).
7. Use command `dotnet script` to run `./scripts/main.csx` and waiting for finish.
8. See `./data/` for outputs.

## Example

```bash
AssetStudioCLI.exe --game GI --map_op AssetMap --map_type JSON --group_assets_type ByType --no_asset_bundle --no_index_object "Input Directory" "Output Directory"
```

```powershell
dotnet script -c Release .\scripts\main.csx -- assets_map_GI.json $env:Studio
```

```powershell
dotnet script -c Release .\scripts\delete.csx
```

```powershell
dotnet script -c Release .\scripts\main.csx -- assets_map_GI.json .\3rd\studio\AssetStudioCLI.exe && dotnet script -c Release .\scripts\delete.csx
```

## License

[GNU AGPLv3](LICENSE.txt)
