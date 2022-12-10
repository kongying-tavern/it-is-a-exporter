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
    Resources.DeleteDirectory();
    // AssetsMapJSON.Analyzer(assetsMap);
    var taskList = new List<Task>();
    var watch = Stopwatch.StartNew();
    try
    {
        Console.WriteLine("[Start] - Export start.");
        foreach (var assetsName in assetsIndex)
        {
            taskList.Add(Task.Run(() => Resources.ExportAsync(Resources.CreateAssetSourcePathSetParallel(assetsMap, assetsName), assetStudioCLIPath, assetsName)));
        }
        await Task.WhenAll(taskList.ToArray());
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
