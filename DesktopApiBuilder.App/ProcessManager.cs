using System.Diagnostics;

namespace DesktopApiBuilder.App;

public static class ProcessManager
{
    public static void ExecuteCmdCommands(string[] commands)
    {
        Process cmd = new();

        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;

        cmd.Start();

        foreach (var command in commands) 
        {
            cmd.StandardInput.WriteLine(command);
        }

        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
    }
}
