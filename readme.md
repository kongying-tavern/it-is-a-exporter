# It is a exporter

Yes, it's just a exporter.
**This is a development tool only for developers.**
**If you don't know how to use it, you do not need it. Maybe.**

## How to use

1. Download latest [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).
2. Download [GenshinStudio](https://github.com/Razmoth/GenshinStudio) version v0.16.55+.
3. Use GenshinStudio to build `BLK Map` and **`assets_map.json`(must be json)**.
4. Use `.NET CLI` to install [dotnet-script](https://github.com/filipw/dotnet-script).
5. Clone or download this repository.
6. Tell `./scripts/main.csx` whereis `assets_map.json` and `AssetStudioCLI.exe`(in the directory of GenshinStudio).
7. Use command `dotnet script` to run `./scripts/main.csx` and waiting for finish.
8. See `./data/` for outputs.

## License

[GNU AGPLv3](LICENSE.txt)
