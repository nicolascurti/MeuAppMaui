# Plano do projeto — MeuAppMaui

## Objetivo

Transformar o template .NET MAUI em um aplicativo simples, em português do Brasil, para configurar e exibir QR Codes pessoais.

## Requisitos e comportamento

- Manter o aplicativo nativo com .NET MAUI e suportar iOS 16 ou superior e Android 13 ou superior.
- Usar a logo fornecida no cabeçalho e sua paleta verde-escura, verde e branco.
- Exibir na tela principal um QR Code por vez, com navegação entre os códigos disponíveis e identificação do tipo de link.
- Gerar QR Codes para:
  - contato vCard com nome, empresa, telefone e e-mail;
  - conversa de WhatsApp com o número configurado;
  - perfil do LinkedIn configurado;
  - até cinco links adicionais configuráveis.
- Exibir uma ação persistente para abrir as configurações.
- Permitir editar os dados do vCard, número do WhatsApp, perfil LinkedIn e os links; validar dados e URLs antes de salvar.
- Persistir configurações localmente no dispositivo, sem conta, backend ou envio de dados.
- Se ainda não houver conteúdo configurado, apresentar orientação para preencher as configurações em vez de um QR Code vazio.

## Abordagem de implementação

- Substituir a tela de contador e a navegação de template pelas telas principal e de configurações, com textos, rótulos acessíveis e mensagens em pt-BR.
- Criar modelos e serviços pequenos para persistência local e criação dos payloads vCard/URL e das imagens QR.
- Usar uma biblioteca .NET compatível com MAUI para gerar QR Codes localmente; não depender de rede para exibição.
- Registrar a logo como imagem MAUI e aplicar cores consistentes aos controles e fundos.
- Atualizar os mínimos declarados no projeto para iOS 16 e Android API 33.

## Validação

- Restaurar e compilar os targets MAUI disponíveis no ambiente.
- Conferir validação e persistência de configuração, o limite de cinco links, a criação dos payloads e a atualização do QR selecionado.
- Revisar a interface em XAML para rolagem em telas pequenas, acessibilidade e ausência de textos do template.
