using api_cadastro_pessoa.Models;
using Microsoft.EntityFrameworkCore;

namespace api_cadastro_pessoa.Data;

/// <summary>
/// Contexto do banco de dados da aplicação
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Tabela de pessoas
    /// </summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
}
