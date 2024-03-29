using System.IO;
using System.Text.Json;

namespace CsharpToHtmlWinApp;

public record Settings(string? CsprojPath, bool CopyOnLoad)
{
    private const string SettingFileName = "settings.json";
    private static string GetAppPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CsharpToHtmlWinApp");

    public static Settings? Load()
    {
        var folder = GetAppPath();
        var path = Path.Combine(folder, SettingFileName);

        if (!File.Exists(path)) return null;

        using var f = File.OpenRead(path);
        return JsonSerializer.Deserialize<Settings>(f);
    }

    public static void Save(Settings settings)
    {
        var folder = GetAppPath();
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, SettingFileName);

        using var f = File.Create(path);
        JsonSerializer.Serialize(f, settings);
    }
}
