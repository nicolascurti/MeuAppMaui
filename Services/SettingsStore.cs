using System.Text.Json;
using MeuAppMaui.Models;

namespace MeuAppMaui.Services;

/// <summary>Carrega e salva as preferências do usuário no armazenamento local do MAUI.</summary>
public static class SettingsStore
{
    private const string SettingsKey = "qr_personal_settings_v1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Retorna as configurações locais ou valores vazios na primeira execução.</summary>
    public static AppSettings Load()
    {
        var json = Preferences.Default.Get(SettingsKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
            return new AppSettings();

        var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions)
            ?? throw new JsonException("As configurações salvas estão vazias ou inválidas.");
        settings.ExtraLinks ??= [];
        return settings;
    }

    /// <summary>Persiste as configurações como JSON nas preferências privadas do aplicativo.</summary>
    public static void Save(AppSettings settings)
    {
        Preferences.Default.Set(SettingsKey, JsonSerializer.Serialize(settings, JsonOptions));
    }
}
