#load "AssetsMapJSON.csx"
#load "Utils.csx"
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public static class Resources
{
    public static ConcurrentDictionary<string, byte> CreateAssetSourcePathSetParallel(AssetsMapJSON.Object[] assetsMap, string assetsName)
    {
        var set = new ConcurrentDictionary<string, byte>();
        Parallel.ForEach(assetsMap, assets =>
        {
            if (assets.Name.StartsWith(assetsName))
            {
                set.TryAdd(assets.Source, 0);
            }
        });
        return set;
    }

    /// <summary>
    /// Export assets.
    /// </summary>
    /// <param name="assetSourcePathSet">assets path set</param>
    /// <param name="assetStudioCLIPath">assetStudioCil file path</param>
    /// <param name="assetsName">assets name</param>
    public static async Task ExportAsync(ConcurrentDictionary<string, byte> assetSourcePathSet, string assetStudioCLIPath, string assetsName)
    {
        var outputPath = Path.Combine("./data/", assetsName);
        const string game = "GI";
        const string types = "Texture2D";

        await Parallel.ForEachAsync(assetSourcePathSet.Keys, async (inputPath, _) =>
      {
          var args = $"\"{inputPath}\" {outputPath} --names {assetsName} --game {game} --types {types}";
          Console.WriteLine($"[Start] - Call AssetStudioCLI with args: {args}");
          var watch = Stopwatch.StartNew();
          try
          {
              await CallAssetStudioCLI(assetStudioCLIPath, args);
          }
          catch (Exception e)
          {
              Console.WriteLine(e.Message);
          }
          finally
          {
              watch.Stop();
              Console.WriteLine($"[Finished][{watch.ElapsedMilliseconds}ms] - Call AssetStudioCLI with args: {args}");
          }
      });
    }

    private static async Task CallAssetStudioCLI(string assetStudioCLIPath, string args)
    {
        ProcessStartInfo startInfo = new()
        {
            Arguments = args,
            CreateNoWindow = true,
            FileName = assetStudioCLIPath,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        await Utils.CreateWaitForExitProcessAsync(startInfo);
    }
    public static void DeleteDirectory()
    {
        try
        {
            Console.WriteLine("[Start] - Resources Delete.");
            Directory.Delete("./data", true);
        }
        catch (Exception e)
        {
            Console.WriteLine("Resources Delete Failed: {0}", e.Message);
        }
        finally
        {
            Console.WriteLine("[Finished] - Resources Delete.");
        }
    }
}