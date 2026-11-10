using System.Text;
using MeuAppMaui.Models;
using QRCoder;

namespace MeuAppMaui.Services;

/// <summary>Cria payloads pessoais e imagens QR inteiramente no dispositivo.</summary>
public static class QrCodeService
{
    /// <summary>Gera uma imagem PNG de QR Code a partir do conteúdo informado.</summary>
    public static ImageSource CreateImageSource(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var imageBytes = new PngByteQRCode(data).GetGraphic(12);
        return ImageSource.FromStream(() => new MemoryStream(imageBytes, writable: false));
    }

    /// <summary>Formata os dados opcionais de contato como um vCard 3.0.</summary>
    public static string CreateVCard(AppSettings settings)
    {
        var lines = new List<string>
        {
            "BEGIN:VCARD",
            "VERSION:3.0",
            $"FN:{EscapeVCard(settings.ContactName)}"
        };

        if (!string.IsNullOrWhiteSpace(settings.Company))
            lines.Add($"ORG:{EscapeVCard(settings.Company)}");
        if (!string.IsNullOrWhiteSpace(settings.ContactPhone))
            lines.Add($"TEL;TYPE=CELL:{DigitsOnly(settings.ContactPhone)}");
        if (!string.IsNullOrWhiteSpace(settings.ContactEmail))
            lines.Add($"EMAIL:{settings.ContactEmail.Trim()}");

        lines.Add("END:VCARD");
        return string.Join("\r\n", lines);
    }

    /// <summary>Cria a URL internacional do WhatsApp com o número sem pontuação.</summary>
    public static string CreateWhatsAppUrl(string phone) =>
        $"https://wa.me/{DigitsOnly(phone)}";

    /// <summary>Normaliza o endereço do perfil do LinkedIn para uso no QR Code.</summary>
    public static string CreateLinkedInUrl(string url) => url.Trim();

    /// <summary>Escapa separadores e quebras de linha conforme o formato vCard.</summary>
    private static string EscapeVCard(string value) =>
        value.Trim()
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace(";", "\\;", StringComparison.Ordinal)
            .Replace(",", "\\,", StringComparison.Ordinal);

    /// <summary>Remove pontuação de telefone para montar o endereço do WhatsApp.</summary>
    private static string DigitsOnly(string value) =>
        new(value.Where(char.IsDigit).ToArray());
}
