#load "Utils.csx"
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var path = "./data/UI_MapBack_/Texture2D/";
var teyvatTilePath = Path.Combine("./output", "Teyvat");
const string pattern = @"\bUI_MapBack_(?<X>-?\d+)_(?<Y>-?\d+)\.png\b";

var regex = new Regex(pattern, RegexOptions.IgnoreCase);
var files = new DirectoryInfo(path).GetFiles();

// for the 2D tile map, every tile region is called a "cell".
// the positionX/Y is the "cell index" below.
// tileSize is the real cell size with the size unit (pixel common).
var positionX = new List<int>();
var positionY = new List<int>();
const int tileSize = 1024;

foreach (var fileInfo in files)
{
    if (regex.IsMatch(fileInfo.Name))
    {
        var matchResult = regex.Match(fileInfo.Name);
        var groups = matchResult.Groups;
        // left-handed Cartesian coordinate system.
        // for most renderers, they use 3D coordinate systems for 2D.
        // check the coordinate system of your renderers.

        // this is the way that the top-left to bottom-right 2D index renderer
        // render Unity right-handed tile index.
        // x-axis is the horizontal axis,
        // y-axis is the vertical axis.
        positionY.Add(Convert.ToInt32(groups["X"].Value));
        positionX.Add(Convert.ToInt32(groups["Y"].Value));
    }
}
Console.WriteLine($"{positionX.Min()}, {positionX.Max()}");
Console.WriteLine($"{positionY.Min()}, {positionY.Max()}");

var tileCountX = Math.Abs(positionX.Min()) + Math.Abs(positionX.Max()) + 1;
var tileCountY = Math.Abs(positionY.Min()) + Math.Abs(positionY.Max()) + 1;
var JsonMeta = new Meta()
{
    TileLayerMeta = new TileLayerMeta
    {
        TileMeta = new TileMeta
        {
            BaseUrl = ".",
            TileName = "UI_MapBack_",
            TileRegion = "Teyvat",
            Version = 3.3,
            ExtName = "png"
        },
        TileSize = tileSize
    },
    Zoom = -2,
    MinZoom = -4,
    TileLayerSize = new TileLayerSize
    {
        Width = tileCountX * tileSize,
        Height = tileCountY * tileSize
    },
    TileLayerOriginOffset = new TileLayerOriginOffset
    {
        X = -positionX.Max() * tileSize,
        Y = -positionY.Max() * tileSize
    },
    MapCenter = new MapCenter()
};
var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

string fileName = "TyvetImageMetaData.json";
string outputPath = Path.Combine("./output", "json");

using (FileStream fileStream = File.Create(Path.Combine(outputPath, fileName)))
{
    await JsonSerializer.SerializeAsync(fileStream, JsonMeta, serializeOptions);
    await fileStream.DisposeAsync();
}

Console.WriteLine(File.ReadAllText(Path.Combine(outputPath, fileName)));

var diffPath = Path.Combine("./output", "diff");
Utils.ClearDirectory(teyvatTilePath);
for (var x = positionX.Min(); x <= positionX.Max(); x++)
{
    for (var y = positionY.Min(); y <= positionY.Max(); y++)
    {
        var fileName = $"UI_MapBack_{y}_{x}.png";
        if (!File.Exists(Path.Combine(path, fileName)))
        {
            Console.WriteLine($"CreateFile: UI_MapBack_{y}_{x}.png");
            File.Copy(Path.Combine(path, "UI_MapBack_None.png"), Path.Combine(teyvatTilePath, fileName));
        }
        else if (!File.Exists(Path.Combine(diffPath, fileName)))
        {
            File.Copy(Path.Combine(path, fileName), Path.Combine(teyvatTilePath, fileName));
        }
    }
}

public record Meta
{
    public object TileLayerMeta { get; set; }
    public int Zoom { get; set; }
    public int MinZoom { get; set; }
    public int MaxZoom { get; set; }
    public object TileLayerSize { get; set; }
    public object TileLayerOriginOffset { get; set; }
    public object MapCenter { get; set; }
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
    public object TileMeta { get; set; }
    public int TileSize { get; set; }
    public int MinZoomNative { get; set; }
    public int MaxZoomNative { get; set; }
}
public record TileMeta
{
    public string BaseUrl { get; set; }
    public string TileName { get; set; }
    public int TileLevel { get; set; }
    public string TileRegion { get; set; }
    public double Version { get; set; }
    public string ExtName { get; set; }
}
public record MapCenter
{
    public int X { get; set; }
    public int Y { get; set; }
}