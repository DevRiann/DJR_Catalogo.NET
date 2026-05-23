using System.Security.Cryptography.X509Certificates;
using DJR_Catalogo.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;


namespace DJR_Catalogo.Pages
{
    public class ContatoModel : PageModel
    {
        public void OnGet()
        {}

        [BindProperty]
        public string nome {get; set;}

        [BindProperty]
        public int idade {get; set;}

        [BindProperty]
        public string email {get; set;}

        [BindProperty]
        public string curso {get; set;}

        [BindProperty]
        public string observacao {get; set;}

        private readonly IConfiguration _configuration;

        // Construtor: o .NET injeta a configuração automaticamente aqui
        public ContatoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Buscando as credenciais que guardamos no appsettings.json
            string clientId = _configuration["GoogleGmail:ClientId"];
            string clientSecret = _configuration["GoogleGmail:ClientSecret"];

            // Pasta onde o .NET vai salvar o token de acesso para não pedir login toda hora
            string pastaToken = Path.Combine(Directory.GetCurrentDirectory(), "GmailTokenStore");

            // 2. Solicitando a autorização do usuário (OAuth 2.0)
            UserCredential credencial;
            credencial = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
                new[] { GmailService.Scope.GmailSend }, // Permissão apenas para ENVIAR e-mails
                "user",
                CancellationToken.None,
                new FileDataStore(pastaToken, true),
                new LocalServerCodeReceiver()
            );

            // 3. Inicializando o serviço da API do Gmail
            var servico = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credencial,
                ApplicationName = "MeuFormularioRazor",
            });

            // 4. Estruturando a mensagem no formato que o Google exige (padrão MIME/Base64)
            string textoEmail = $"From: riannmeira14@gmail.com\r\n" +
                                $"To: riannmeira14@gmail.com\r\n" + // Coloque seu e-mail de destino aqui
                                $"Subject: Feedback - Catalogo.NET\r\n\r\n" +
                                $"Nome: {nome}\nE-mail: {email}\nCurso: {curso}\nObservações: {observacao}";

            // O Google exige que o texto do e-mail seja convertido em uma string Base64 segura para a web
            var novaMensagem = new Message();
            var bytesMensagem = System.Text.Encoding.UTF8.GetBytes(textoEmail);
            novaMensagem.Raw = System.Convert.ToBase64String(bytesMensagem)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            // 5. Envia o e-mail de verdade usando a API!
            await servico.Users.Messages.Send(novaMensagem, "me").ExecuteAsync();

            return RedirectToPage();
        }
    }
}