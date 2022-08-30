#load "AssetsMapJSON.csx"
public static class Resources
{
    public static HashSet<string> CreateAssetSourcePathHashSet(AssetsMapJSON.Object[] assetsMap, string assetsName)
    {
        var set = new HashSet<string>();
        Parallel.ForEach(assetsMap, asset =>
        {
            if (asset.Name.StartsWith(assetsName))
            {
                set.Add(asset.SourcePath);
            }
        });
        return set;
    }
    /// <summary>
    /// Export assets.
    /// </summary>
    /// <param name="assetSourcePathHashSet">assets path set</param>
    /// <param name="assetStudioCLIPath">assetStudioCil file path</param>
    /// <param name="assetsName">assets name</param>
    public static void Export(HashSet<string> assetSourcePathHashSet, string assetStudioCLIPath, string assetsName)
    {
        Parallel.ForEach(assetSourcePathHashSet, path =>
        {
            var args = $"\"{path}\" ./data/{assetsName} -f {assetsName} -g GI -t Texture2D";
            var watch = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Start] - Export with args: {args}");
                using Process myProcess = new();
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = assetStudioCLIPath;
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    myProcess.StartInfo.Arguments = args;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                watch.Stop();
                Console.WriteLine($"[Finished][{watch.ElapsedMilliseconds}ms] - Export with args: {args}");
            }
        });
    }
    public static void Delete()
    {
        try
        {
            Console.WriteLine($"[Start] - Resources Delete.");
            Directory.Delete("./data",true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Resources Delete Failed: {0}", e.Message);
        }
        finally
        {
            Console.WriteLine($"[Finished] - Resources Delete.");
        }
    }
}