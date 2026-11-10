using System.Text.Json;
using MeuAppMaui.Models;
using MeuAppMaui.Services;

namespace MeuAppMaui;

/// <summary>Exibe e permite percorrer os QR Codes pessoais configurados localmente.</summary>
public partial class MainPage : ContentPage
{
    private List<QrEntry> entries = [];
    private int selectedIndex;

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>Atualiza os QR Codes ao retornar das configurações.</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            LoadQrEntries();
        }
        catch (JsonException)
        {
            entries = [];
            QrContent.IsVisible = false;
            EmptyState.IsVisible = true;
            await DisplayAlertAsync(
                "Configurações inválidas",
                "Não foi possível ler os dados salvos. Abra Configurações e salve novamente para corrigir.",
                "OK");
        }
    }

    /// <summary>Carrega as configurações locais e prepara somente os QR Codes disponíveis.</summary>
    private void LoadQrEntries()
    {
        var settings = SettingsStore.Load();
        entries = [];

        if (!string.IsNullOrWhiteSpace(settings.ContactName) &&
            (!string.IsNullOrWhiteSpace(settings.ContactPhone) ||
             !string.IsNullOrWhiteSpace(settings.ContactEmail)))
        {
            entries.Add(new QrEntry(
                "Meu contato",
                "Escaneie para salvar meus dados de contato",
                QrCodeService.CreateVCard(settings)));
        }

        if (!string.IsNullOrWhiteSpace(settings.WhatsAppPhone))
        {
            entries.Add(new QrEntry(
                "WhatsApp",
                "Escaneie para iniciar uma conversa",
                QrCodeService.CreateWhatsAppUrl(settings.WhatsAppPhone)));
        }

        if (!string.IsNullOrWhiteSpace(settings.LinkedInUrl))
        {
            entries.Add(new QrEntry(
                "LinkedIn",
                "Escaneie para visitar meu perfil",
                QrCodeService.CreateLinkedInUrl(settings.LinkedInUrl)));
        }

        entries.AddRange(settings.ExtraLinks
            .Where(link => !string.IsNullOrWhiteSpace(link.Title) &&
                           !string.IsNullOrWhiteSpace(link.Url))
            .Take(5)
            .Select(link => new QrEntry(link.Title.Trim(), "Escaneie para abrir este link", link.Url.Trim())));

        selectedIndex = Math.Clamp(selectedIndex, 0, Math.Max(entries.Count - 1, 0));
        UpdateDisplay();
    }

    /// <summary>Atualiza o estado vazio, a imagem e os controles de navegação.</summary>
    private void UpdateDisplay()
    {
        var hasEntries = entries.Count > 0;
        QrContent.IsVisible = hasEntries;
        EmptyState.IsVisible = !hasEntries;
        if (!hasEntries)
            return;

        var entry = entries[selectedIndex];
        CurrentTitle.Text = entry.Title;
        CurrentDescription.Text = entry.Description;
        QrCounter.Text = $"{selectedIndex + 1} de {entries.Count}";
        SwipeHint.IsVisible = entries.Count > 1;
        QrImage.Source = QrCodeService.CreateImageSource(entry.Payload);
        SemanticProperties.SetDescription(QrImage, $"QR Code de {entry.Title}");
        PreviousButton.IsEnabled = selectedIndex > 0;
        NextButton.IsEnabled = selectedIndex < entries.Count - 1;
    }

    /// <summary>Exibe o QR Code anterior, quando houver.</summary>
    private void OnPreviousClicked(object? sender, EventArgs e)
    {
        ShowPreviousQrCode();
    }

    /// <summary>Exibe o próximo QR Code, quando houver.</summary>
    private void OnNextClicked(object? sender, EventArgs e)
    {
        ShowNextQrCode();
    }

    /// <summary>Move entre os QR Codes conforme a direção do gesto horizontal.</summary>
    private void OnQrSwiped(object? sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left)
            ShowNextQrCode();
        else if (e.Direction == SwipeDirection.Right)
            ShowPreviousQrCode();
    }

    /// <summary>Exibe o QR Code anterior, quando houver.</summary>
    private void ShowPreviousQrCode()
    {
        if (selectedIndex > 0)
        {
            selectedIndex--;
            UpdateDisplay();
        }
    }

    /// <summary>Exibe o próximo QR Code, quando houver.</summary>
    private void ShowNextQrCode()
    {
        if (selectedIndex < entries.Count - 1)
        {
            selectedIndex++;
            UpdateDisplay();
        }
    }

    /// <summary>Abre a tela de edição das configurações pessoais.</summary>
    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
