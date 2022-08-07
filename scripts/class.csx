using System.Text.Json;
using System.Text.Json.Serialization;

public static class AssetsMapJSON
{
    public class Object
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string Type { get; set; }
    }
    public static class MapIndex
    {
        public static readonly string UI_MapBack_ = "UI_MapBack_";//大世界
        public static readonly string UI_MapBack_AbyssalPalace_ = "UI_MapBack_AbyssalPalace_";//渊下宫
        public static readonly string UI_MapBack_TheChasm_ = "UI_MapBack_TheChasm_";//地下矿区
        public static readonly string UI_Map_GoldenAppleIsles_ = "UI_Map_GoldenAppleIsles_";//1.6
        public static readonly string UI_Map_DreamIsland_ = "UI_Map_DreamIsland_";//2.8
        public static readonly string UI_Map_AbyssalPalace_ = "UI_Map_AbyssalPalace_";//渊下宫(活动)
        public static readonly string UI_Map_HomeWorld_ = "UI_Map_HomeWorld_";//家园
    }
    public static async Task<HashSet<string>> DeserializeFromFileAsync(string filePath)
    {
        var set = new HashSet<string>();
        try
        {
            using (
                var openStream = File.OpenRead(filePath)
            )
            {
                var assetsMap = await JsonSerializer.DeserializeAsync<Object[]>(openStream);
                foreach (var asset in assetsMap)
                {
                    if (asset.Name.StartsWith("UI_Map"))
                    {
                        set.Add(asset.SourcePath);
                    }
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return set;
    }
}
public static class Resources
{
    public static void Export(HashSet<string> collection, string assetStudioCLIPath)

    {

        Parallel.ForEach(collection, path =>
                    {
                        try
                        {
                            using Process myProcess = new();
                            {
                                myProcess.StartInfo.UseShellExecute = false;
                                myProcess.StartInfo.FileName = assetStudioCLIPath;
                                myProcess.StartInfo.CreateNoWindow = true;
                                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                myProcess.StartInfo.Arguments = $"\"{path}\" ./data/ -t Texture2D";
                                myProcess.Start();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
    }
}
