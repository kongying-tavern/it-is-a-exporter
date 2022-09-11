#load "AssetsMapJSON.csx"
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
public static class Resources
{
    public static HashSet<string> CreateAssetSourcePathHashSet(AssetsMapJSON.Object[] assetsMap, string assetsName)
    {
        var set = new HashSet<string>();
        Parallel.ForEach(assetsMap, assets =>
        {
            if (assets.Name.StartsWith(assetsName))
            {
                set.Add(assets.SourcePath);
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
        foreach (var path in assetSourcePathHashSet)
        {
            var args = $"\"{path}\" ./data/{assetsName} --filter {assetsName} --game GI --type Texture2D";
            var watch = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Start] - Export with args: {args}");
                using Process process = new();
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = assetStudioCLIPath;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.Arguments = args;
                    process.Start();
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
        };
    }
    public static void Delete()
    {
        try
        {
            Console.WriteLine($"[Start] - Resources Delete.");
            Directory.Delete("./data", true);
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
    public static void DeleteSameFileBySHA256(string directory)
    {
        do
        {
            foreach (var process in Process.GetProcessesByName("AssetStudioCLI"))
            {
                Console.WriteLine($"Waiting for process ID: {process.Id}, Name: {process.ProcessName} to exit.");
            }
        } while (Process.GetProcessesByName("AssetStudioCLI") is null);
        Thread.Sleep(15000);
        Console.WriteLine($"[Start] - Resources DeleteSameFileBySHA256(\"{directory}\")\n");
        var pattern = @"\bUI_MapBack_(?<X>-?\d+)_(?<Y>-?\d+)#\d+\.png\b";
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
                    if (regex.IsMatch(fileInfo.Name))
                    {
                        var matchResult = regex.Match(fileInfo.Name);
                        var groups = matchResult.Groups;
                        var fileName = $"UI_MapBack_{groups["X"].Value}_{groups["Y"].Value}.png";
                        var fileHash1 = ComputeHashStringFromFileInfo(new FileInfo($"{directory}{fileName}"), sha256);
                        var fileHash2 = ComputeHashStringFromFileInfo(fileInfo, sha256);
                        if (fileHash1 == fileHash2)
                        {
                            try { fileInfo.Delete(); }
                            finally
                            {
                                Console.WriteLine($"[Deleted] - File deleted: {fileInfo.Name} has same hash with {fileName}");
                                Console.WriteLine($"{fileHash2}: {fileInfo.Name}");
                                Console.WriteLine($"{fileHash1}: {fileName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Finished] - File not same: {fileInfo.Name} does not has same hash with {fileName}");
                            Console.WriteLine($"{fileHash2}: {fileInfo.Name}");
                            Console.WriteLine($"{fileHash1}: {fileName}");
                        }
                        Console.WriteLine();
                    }
                };
            }
        }
        else
        {
            Console.WriteLine("The directory specified could not be found.");
        }
    }

    // private static string ByteArrayToString(byte[] array)
    // {
    //     var stringBuilder = new StringBuilder(array.Length);
    //     for (int i = 0; i < array.Length - 1; i++)
    //     {
    //         stringBuilder.Append($"{array[i]:X2}");
    //     }
    //     return stringBuilder.ToString();
    // }
    private static string ComputeHashStringFromFileInfo(FileInfo fileInfo, SHA256 sha256)
    {
        string hashString = string.Empty;
        using var fileStream = fileInfo.Open(FileMode.Open);
        {
            try
            {
                // Create a fileStream for the file.
                // Be sure it's positioned to the beginning of the stream.
                fileStream.Position = 0;
                // Compute the hash of the fileStream.
                var bytes = sha256.ComputeHash(fileStream);
                // PrintByteArray(bytes);
                // https://github.com/PowerShell/PowerShell/blob/7dc4587014bfa22919c933607bf564f0ba53db2e/src/Microsoft.PowerShell.Commands.Utility/commands/utility/GetHash.cs#L172
                hashString = BitConverter.ToString(bytes).Replace("-", string.Empty);
            }
            catch (IOException e)
            {
                Console.WriteLine($"I/O Exception: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Access Exception: {e.Message}");
            }
        }
        return hashString;
    }
}