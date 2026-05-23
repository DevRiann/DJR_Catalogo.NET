using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace DJR_Catalogo.Pages;

public class IndexModel(ILogger<IndexModel> logger, AppDbContext context) : PageModel
{

    // Propriedade que vai receber os dados do formulário HTML via Model Binding
    [BindProperty]
    public Obra NovaObra { get; set; }

    public List<Obra> ListaDeObras { get; set; }

    public void OnGet()
    {
        ListaDeObras = context.Obras.ToList();
    }

    // Método que lida com o envio do formulário (Botão Salvar)
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Tentativa de cadastro com dados inválidos.");
            return Page(); 
        }

        // Salva no SQLite
        context.Obras.Add(NovaObra);
        context.SaveChanges();

        logger.LogInformation($"Novo anime/série salvo com sucesso: {NovaObra.Titulo}");

        return RedirectToPage();
    }

    public IActionResult OnPostDeletar(int id)
    {
        // 1. Busca a obra no banco pelo ID
        var obra = context.Obras.Find(id);

        if (obra != null)
        {
            // 2. Remove a obra do rastreador do EF
            context.Obras.Remove(obra);
            
            // 3. Salva a alteração definitivamente no SQLite
            context.SaveChanges();
        }

        return RedirectToPage();
    }

    public IActionResult OnPostAtualizar(int id, int novaTemporada, int novoEpisodio)
    {
        var obra = context.Obras.Find(id);
        if (obra != null)
        {
            obra.Temporada = novaTemporada;
            obra.UltimoEpisodio = novoEpisodio;

            context.SaveChanges();
        }
        return RedirectToPage();
    }
}
