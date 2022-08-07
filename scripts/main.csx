#load "class.csx"

var fileName = "assets_map.json";

var assetStudioCLIPath = @"AssetStudioCLI.exe";

var set = await AssetsMapJSON.DeserializeFromFileAsync(fileName);

Resources.Export(set, assetStudioCLIPath);
