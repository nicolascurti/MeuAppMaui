using System.Text.Json;
using System.Text.RegularExpressions;
using MeuAppMaui.Models;
using MeuAppMaui.Services;

namespace MeuAppMaui;

/// <summary>Permite editar os dados locais que originam os QR Codes pessoais.</summary>
public partial class SettingsPage : ContentPage
{
    private const int MaximumExtraLinks = 5;
    private readonly Entry[] linkTitles = new Entry[MaximumExtraLinks];
    private readonly Entry[] linkUrls = new Entry[MaximumExtraLinks];

    public SettingsPage()
    {
        InitializeComponent();
        BuildExtraLinkFields();
    }

    /// <summary>Carrega os dados locais e avisa se as preferências salvas estiverem inválidas.</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            LoadSettings();
        }
        catch (JsonException)
        {
            await DisplayAlertAsync(
                "Configurações inválidas",
                "Os dados salvos não puderam ser lidos. Preencha novamente e salve para substituí-los.",
                "OK");
        }
    }

    /// <summary>Cria os cinco campos fixos de links adicionais permitidos pelo aplicativo.</summary>
    private void BuildExtraLinkFields()
    {
        for (var index = 0; index < MaximumExtraLinks; index++)
        {
            var title = new Entry
            {
                Placeholder = $"Nome do link {index + 1}",
                MaxLength = 60,
                ReturnType = ReturnType.Next
            };
            SemanticProperties.SetDescription(title, $"Nome do link adicional {index + 1}");

            var url = new Entry
            {
                Placeholder = "https://exemplo.com",
                Keyboard = Keyboard.Url,
                MaxLength = 500,
                ReturnType = index == MaximumExtraLinks - 1 ? ReturnType.Done : ReturnType.Next
            };
            SemanticProperties.SetDescription(url, $"Endereço HTTPS do link adicional {index + 1}");

            linkTitles[index] = title;
            linkUrls[index] = url;
            ExtraLinksContainer.Add(title);
            ExtraLinksContainer.Add(url);
        }
    }

    /// <summary>Preenche os campos com as preferências salvas neste dispositivo.</summary>
    private void LoadSettings()
    {
        var settings = SettingsStore.Load();
        ContactNameEntry.Text = settings.ContactName;
        CompanyEntry.Text = settings.Company;
        ContactPhoneEntry.Text = settings.ContactPhone;
        ContactEmailEntry.Text = settings.ContactEmail;
        WhatsAppEntry.Text = settings.WhatsAppPhone;
        LinkedInEntry.Text = settings.LinkedInUrl;

        for (var index = 0; index < Math.Min(settings.ExtraLinks.Count, MaximumExtraLinks); index++)
        {
            linkTitles[index].Text = settings.ExtraLinks[index].Title;
            linkUrls[index].Text = settings.ExtraLinks[index].Url;
        }
    }

    /// <summary>Valida os campos e salva toda a configuração pessoal localmente.</summary>
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ValidationMessage.IsVisible = false;
        var contactName = (ContactNameEntry.Text ?? string.Empty).Trim();
        var company = (CompanyEntry.Text ?? string.Empty).Trim();
        var contactPhone = (ContactPhoneEntry.Text ?? string.Empty).Trim();
        var contactEmail = (ContactEmailEntry.Text ?? string.Empty).Trim();
        var whatsappPhone = (WhatsAppEntry.Text ?? string.Empty).Trim();
        var linkedInUrl = (LinkedInEntry.Text ?? string.Empty).Trim();

        var hasContactData = contactName.Length > 0 || company.Length > 0 ||
                             contactPhone.Length > 0 || contactEmail.Length > 0;
        if (hasContactData &&
            (contactName.Length == 0 || (contactPhone.Length == 0 && contactEmail.Length == 0)))
        {
            await ShowValidationError("Para gerar o vCard, informe o nome e ao menos um telefone ou e-mail.");
            return;
        }

        if (contactPhone.Length > 0 && !IsValidPhone(contactPhone))
        {
            await ShowValidationError("O telefone do contato deve conter de 8 a 15 dígitos.");
            return;
        }

        if (contactEmail.Length > 0 && !IsValidEmail(contactEmail))
        {
            await ShowValidationError("Informe um endereço de e-mail válido.");
            return;
        }

        if (whatsappPhone.Length > 0 && !IsValidPhone(whatsappPhone))
        {
            await ShowValidationError("O WhatsApp deve conter de 8 a 15 dígitos, incluindo o código do país.");
            return;
        }

        if (linkedInUrl.Length > 0 && !IsValidLinkedInUrl(linkedInUrl))
        {
            await ShowValidationError("Informe uma URL HTTPS válida de linkedin.com.");
            return;
        }

        var extraLinks = new List<ExtraLink>();
        for (var index = 0; index < MaximumExtraLinks; index++)
        {
            var title = (linkTitles[index].Text ?? string.Empty).Trim();
            var url = (linkUrls[index].Text ?? string.Empty).Trim();
            if (title.Length == 0 && url.Length == 0)
                continue;
            if (title.Length == 0 || !IsValidHttpsUrl(url))
            {
                await ShowValidationError($"No link {index + 1}, preencha o nome e uma URL HTTPS válida.");
                return;
            }

            extraLinks.Add(new ExtraLink { Title = title, Url = url });
        }

        SettingsStore.Save(new AppSettings
        {
            ContactName = contactName,
            Company = company,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail,
            WhatsAppPhone = whatsappPhone,
            LinkedInUrl = linkedInUrl,
            ExtraLinks = extraLinks
        });

        await Shell.Current.GoToAsync("..");
    }

    /// <summary>Apresenta uma mensagem junto ao botão sem descartar os dados digitados.</summary>
    private Task ShowValidationError(string message)
    {
        ValidationMessage.Text = message;
        ValidationMessage.IsVisible = true;
        SemanticScreenReader.Announce(message);
        return Task.CompletedTask;
    }

    /// <summary>Valida número de telefone por quantidade de dígitos, ignorando pontuação.</summary>
    private static bool IsValidPhone(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());
        return digits.Length is >= 8 and <= 15 &&
               Regex.IsMatch(value, @"^\+?[\d\s().-]+$", RegexOptions.CultureInvariant);
    }

    /// <summary>Valida um e-mail de forma simples antes de incluí-lo no vCard.</summary>
    private static bool IsValidEmail(string value) =>
        Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant);

    /// <summary>Restringe o perfil a um endereço HTTPS no domínio oficial do LinkedIn.</summary>
    private static bool IsValidLinkedInUrl(string value)
    {
        if (!IsValidHttpsUrl(value) || !Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return false;

        return uri.Host.Equals("linkedin.com", StringComparison.OrdinalIgnoreCase) ||
               uri.Host.Equals("www.linkedin.com", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Valida URLs de links extras sem aceitar esquemas diferentes de HTTPS.</summary>
    private static bool IsValidHttpsUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(uri.Host);

    /// <summary>Retorna à tela principal sem modificar as preferências.</summary>
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
