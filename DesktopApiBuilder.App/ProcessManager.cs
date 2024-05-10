using System.Diagnostics;

namespace DesktopApiBuilder.App;

public static class ProcessManager
{
    public static async Task ExecuteCmdCommands(string[] commands, CancellationToken ct)
    {
        Process cmd = new();

        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;

        cmd.Start();

        foreach (var command in commands)
        {
            await cmd.StandardInput.WriteLineAsync(command);
        }

        await cmd.StandardInput.FlushAsync(ct);
        cmd.StandardInput.Close();
        await cmd.WaitForExitAsync(ct);
    }
}
