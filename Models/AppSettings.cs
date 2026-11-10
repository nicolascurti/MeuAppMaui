namespace MeuAppMaui.Models;

/// <summary>Representa as informações pessoais salvas somente neste dispositivo.</summary>
public sealed class AppSettings
{
    public string ContactName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string WhatsAppPhone { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public List<ExtraLink> ExtraLinks { get; set; } = [];
}

/// <summary>Um link adicional nomeado para exibição e geração de QR Code.</summary>
public sealed class ExtraLink
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

/// <summary>Descreve um QR Code disponível na tela principal.</summary>
public sealed record QrEntry(string Title, string Description, string Payload);
