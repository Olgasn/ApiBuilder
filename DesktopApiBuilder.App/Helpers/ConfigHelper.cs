using DesktopApiBuilder.App.Data.Models;
using System.Text.Json;

namespace DesktopApiBuilder.App.Helpers;

public static class ConfigHelper
{
    public static SolutionConfig? GetSolutionConfig(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SolutionConfig>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
