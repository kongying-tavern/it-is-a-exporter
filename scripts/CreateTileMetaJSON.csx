using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var path = "./data/UI_MapBack_/Texture2D/";
const string pattern = @"\bUI_MapBack_(?<X>-?\d+)_(?<Y>-?\d+)\.png\b";
var regex = new Regex(pattern, RegexOptions.IgnoreCase);
var files = new DirectoryInfo(path).GetFiles();
var positionX = new List<int>();
var positionY = new List<int>();
const int tileSize = 1024;
foreach (var fileInfo in files)
{
    if (regex.IsMatch(fileInfo.Name))
    {
        var matchResult = regex.Match(fileInfo.Name);
        var groups = matchResult.Groups;
        positionX.Add(Convert.ToInt32(groups["X"].Value));
        positionY.Add(Convert.ToInt32(groups["Y"].Value));
    }
}
Console.WriteLine($"{positionX.Min()}, {positionX.Max()}");
Console.WriteLine($"{positionY.Min()}, {positionY.Max()}");

var JsonMeta = new Meta()
{
    TileLayerSize = new TileLayerSize
    {
        Height = (Math.Abs(positionX.Min()) + Math.Abs(positionX.Max()) + 1) * tileSize,
        Width = (Math.Abs(positionY.Min()) + Math.Abs(positionY.Max()) + 1) * tileSize
    },
    TileLayerOriginOffset = new TileLayerOriginOffset
    {
        Y = -positionX.Max() * tileSize,
        X = -positionY.Max() * tileSize
    }
};
var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};
var jsonString = JsonSerializer.Serialize(JsonMeta, serializeOptions);

Console.WriteLine(jsonString);

public record Meta
{
    public object TileLayerSize { get; set; }
    public object TileLayerOriginOffset { get; set; }
}
public record TileLayerOriginOffset
{
    public int X { get; set; }
    public int Y { get; set; }
}
public record TileLayerSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}
public record TileLayerMeta
{
    public record TileMeta
    {
        public string BaseUrl { get; set; }
    }
}
