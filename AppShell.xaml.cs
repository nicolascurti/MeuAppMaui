namespace MeuAppMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Permite abrir a tela de configurações pela navegação do Shell.
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
