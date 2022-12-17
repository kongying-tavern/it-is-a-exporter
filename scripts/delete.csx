#load "Utils.csx"
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

static var path = "./data/UI_MapBack_/Texture2D/";

static string fileName = "diff.csv";
static string outputPath = Path.Combine("./output", "diff");

static var set = new ConcurrentDictionary<string, byte>();

Utils.ClearDirectory(outputPath);

await DeleteSameFileBySHA256Async(path);
await CreateFileListSHA256Async();

public static async Task DeleteSameFileBySHA256Async(string directory)
{
    Console.WriteLine($"[Start] - Resources DeleteSameFileBySHA256(\"{directory}\")\n");
    const string pattern = @"\bUI_MapBack_(?<X>-?\d+)_(?<Y>-?\d+)#\d+\.png\b";
    var regex = new Regex(pattern, RegexOptions.IgnoreCase);

    if (Directory.Exists(directory))
    {
        // Get the FileInfo objects for every file in the directory.
        var files = new DirectoryInfo(directory).GetFiles();
        // Initialize a SHA256 hash object.
        using var sha256 = SHA256.Create();
        {
            foreach (var fileInfo in files)
            {
                await Task.Run(() => CompareFileBySHA256Async(regex, fileInfo, directory, sha256));
            }
        }
    }
    else
    {
        Console.WriteLine("The directory specified could not be found.");
    }
}

private static Task CompareFileBySHA256Async(Regex regex, FileInfo fileInfo, string directory, SHA256 sha256)
{
    if (regex.IsMatch(fileInfo.Name))
    {
        var matchResult = regex.Match(fileInfo.Name);
        var groups = matchResult.Groups;
        var fileName = $"UI_MapBack_{groups["X"].Value}_{groups["Y"].Value}.png";
        var fileHash1 = Utils.ComputeHashStringFromFileInfo(new FileInfo(directory + fileName), sha256);
        var fileHash2 = Utils.ComputeHashStringFromFileInfo(fileInfo, sha256);
        if (fileHash1 == fileHash2)
        {
            try { fileInfo.Delete(); }
            finally
            {
                WriteLine($"[Deleted] - File deleted: {fileInfo.Name} has same hash with {fileName}");
                WriteLine($"{fileHash2}: {fileInfo.Name}");
                WriteLine($"{fileHash1}: {fileName}");
            }
        }
        else
        {
            WriteLine($"[Finished] - File not same: {fileInfo.Name} does not has same hash with {fileName}");
            WriteLine($"{fileHash2}: {fileInfo.Name}");
            WriteLine($"{fileHash1}: {fileName}");
            set.TryAdd($"{fileInfo.Name},{fileHash2}", 0);
            set.TryAdd($"{fileName},{fileHash1}", 0);
            File.Copy(fileInfo.FullName, Path.Combine(outputPath, fileInfo.Name), true);
            File.Copy(Path.Combine(path, fileName), Path.Combine(outputPath, fileName), true);
        }
        WriteLine();
    }

    return Task.CompletedTask;
}

public static async Task CreateFileListSHA256Async()
{
    await File.AppendAllLinesAsync(Path.Combine(outputPath, fileName), new[] { "Name,SHA256,VersionState" });
    await File.AppendAllLinesAsync(Path.Combine(outputPath, fileName), set.Keys);
}
