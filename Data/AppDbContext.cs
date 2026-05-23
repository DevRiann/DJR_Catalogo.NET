using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // O DbSet representa a tabela no banco de dados
    public DbSet<Obra> Obras { get; set; }
}