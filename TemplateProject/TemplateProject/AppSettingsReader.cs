using System.IO;
using System.Text.Json;

namespace TemplateProject;

public class AppSettingsReader
{
    internal static AppSettings ReadAppSettings(string path = "appsettings.json")
    {
        var appSettingsText = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path));
    }
}

internal record DbSettings(string ConnectionString);

internal record AppSettings(DbSettings DbSettings);