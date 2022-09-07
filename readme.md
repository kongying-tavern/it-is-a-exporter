# It is a exporter

Yes, it's just a exporter.

**This is a development tool only for developers.**

**If you don't know how to use it, you do not need it. Maybe.**

## How to use

1. Download latest [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).
2. Use `.NET CLI` to install [dotnet-script](https://github.com/filipw/dotnet-script).
3. Download [HoYoStudio](https://github.com/Razmoth/HoYoStudio) version v0.16.69.
4. Use `AssetStudioCLI.exe` in HoYoStudio to build **`assets_map.json`(must be json)**.
5. Clone or download this repository.
6. Tell `./scripts/main.csx` whereis `assets_map.json` and `AssetStudioCLI.exe`(in the directory of HoYoStudio).
7. Use command `dotnet script` to run `./scripts/main.csx` and waiting for finish.
8. See `./data/` for outputs.

## Example

```bash
AssetStudioCLI.exe --game GI --map_op AssetMap --map_type JSON --group_assets_type ByType --no_asset_bundle --no_index_object "Genshin Impact Directory" "Outputs"
```

```powershell
dotnet script .\scripts\main.csx -- assets_map_GI.json $env:HoYoStudio
```

```powershell
 dotnet script .\scripts\delete.csx
```

## License

[GNU AGPLv3](LICENSE.txt)
