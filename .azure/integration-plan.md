# Plano de integração — MeuAppMaui

- Backend: nenhum. Aplicativo nativo .NET MAUI na raiz do repositório; não iniciar serviço.
- Comando de compilação: `dotnet build MeuAppMaui.csproj -f net10.0-ios`; Android requer JDK configurado.
- Frontend web: nenhum. A interface é MAUI (`MainPage.xaml`, `SettingsPage.xaml`); não há cliente web, mocks ou API seam.
- API: nenhuma rota HTTP; os QR Codes são gerados localmente pelo serviço `Services/QrCodeService.cs`.
- Dados: `Preferences.Default`, serializados por `Services/SettingsStore.cs`; sem banco, ferramenta de migração ou variáveis de conexão. Não criar banco, migrations ou seed data.
- Tipos: `Models/AppSettings.cs` (`AppSettings`, `ExtraLink`, `QrEntry`).
- Serviços: armazenamento local e QRCoder são essenciais. Serviços Azure, conta, backend e rede não são necessários.
- Verificação: compilar os targets MAUI viáveis e conferir o fluxo local de configuração, validação, limite de cinco links e navegação entre QR Codes.
