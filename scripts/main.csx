#load "AssetsMapJSON.csx"
#load "AssetsIndex.csx"
#load "Resources.csx"

if (Args.Count == 2)
{
    var fileName = Args[0];
    var assetStudioCLIPath = Args[1];
    var assetsIndex = new HashSet<string>() {
        AssetsIndex.UI_MapBack_,
        AssetsIndex.UI_MapBack_TheChasm_,
        AssetsIndex.UI_MapBack_AbyssalPalace_,
        AssetsIndex.UI_Map_GoldenAppleIsles_,
        AssetsIndex.UI_Map_DreamIsland_,
        AssetsIndex.UI_Map_HomeWorld_,
        AssetsIndex.UI_Map_AbyssalPalace_,
        AssetsIndex.UI_EmotionIcon,
        AssetsIndex.UI_EmotionTagIcon,
        AssetsIndex.UI_Gcg_CardFace_
    };

    var assetsMap = await AssetsMapJSON.DeserializeFromFileAsync(fileName);
    Resources.Delete();
    // AssetsMapJSON.Analyzer(assetsMap);
    var watch = Stopwatch.StartNew();
    try
    {
        Console.WriteLine("[Start] - Export start.");
        Parallel.ForEach(assetsIndex, assetsName => Resources.Export(Resources.CreateAssetSourcePathHashSet(assetsMap, assetsName), assetStudioCLIPath, assetsName));
    }
    finally
    {
        Console.WriteLine();
        Console.WriteLine($"[Finished][{watch.ElapsedMilliseconds}ms] - Total export time {watch.ElapsedMilliseconds}ms.");
    }
}
else
{
    Console.WriteLine("Usage: ");
    Console.WriteLine("dotnet script <script name> -- <assets_map.json path> <assetStudioCLIPath>");
    Console.WriteLine("Example: ");
    Console.WriteLine(@"dotnet script .\scripts\main.csx -- assets_map.json assetStudioCLIPath");
}
