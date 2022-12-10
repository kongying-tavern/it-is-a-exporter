using System.Security.Cryptography;
using System.Threading;

public static class Utils
{
    public static async Task CreateWaitForExitProcessAsync(ProcessStartInfo startInfo)
    {
        TaskCompletionSource<bool> processExitedEventHandler = new();
        using Process process = new();
        {
            try
            {
                process.StartInfo = startInfo;
                //  Note that the Process.Exited event is raised 
                //  even if the value of Process.EnableRaisingEvents is false 
                //  when the process exits during or before the user performs a Process.HasExited check.
                process.EnableRaisingEvents = true;
                process.Exited += (object sender, System.EventArgs e) => processExitedEventHandler.TrySetResult(true);
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred trying to create process :{ex.Message}");
                return;
            }
        }
        // Use SpinWait to wait for process exited.
        SpinWait.SpinUntil(() => processExitedEventHandler.Task.Result);
        // Because of SpinWaitUntil, it is not require Task.Delay().
        // But MAKE SURE the EventHandler currently.
        await Task.WhenAny(processExitedEventHandler.Task);
    }

    public static string ComputeHashStringFromFileInfo(FileInfo fileInfo, SHA256 sha256)
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