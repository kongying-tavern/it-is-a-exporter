using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;

public static class AssetsMapJSON
{
    public class Object
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string Type { get; set; }
    }
    public static async Task<Object[]> DeserializeFromFileAsync(string filePath)
    {
        Object[] assetsMap = null;
        var watch = Stopwatch.StartNew();
        try
        {
            Console.WriteLine($"[Start] - DeserializeAsync: {filePath}");
            using var fileStream = File.OpenRead(filePath);
            {
                assetsMap = await JsonSerializer.DeserializeAsync<Object[]>(fileStream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            watch.Stop();
            Console.WriteLine($"[Finished][{watch.ElapsedMilliseconds}ms] - DeserializeAsync: {filePath}");
        }
        return assetsMap;
    }
    public static void Analyzer(Object[] assetsMap)
    {
        var assetsTypeSet = new ConcurrentDictionary<string, int>();
        var assetsSourcePathSet = new ConcurrentDictionary<string, int>();
        var assetsNameSet = new ConcurrentDictionary<string, int>();
        Parallel.ForEach(assetsMap, assets =>
        {
            assetsTypeSet.TryAdd(assets.Type, 0);
            assetsSourcePathSet.TryAdd(assets.SourcePath, 0);
            assetsNameSet.TryAdd(assets.Name, 0);
        });
        Console.WriteLine($"Type: {assetsTypeSet.Count}");
        Console.WriteLine($"File: {assetsSourcePathSet.Count}");
        Console.WriteLine($"Name: {assetsNameSet.Count}");
    }
}
