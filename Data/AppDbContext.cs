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

    /// <summary>
    /// Tabela de cursos
    /// </summary>
    public DbSet<Curso> Cursos => Set<Curso>();

    /// <summary>
    /// Tabela de relação pessoa-curso
    /// </summary>
    public DbSet<PessoaCurso> PessoasCursos => Set<PessoaCurso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da chave composta para PessoaCurso
        modelBuilder.Entity<PessoaCurso>()
            .HasKey(pc => new { pc.PessoaId, pc.CursoId });

        modelBuilder.Entity<PessoaCurso>()
            .HasOne(pc => pc.Pessoa)
            .WithMany(p => p.PessoasCursos)
            .HasForeignKey(pc => pc.PessoaId);

        modelBuilder.Entity<PessoaCurso>()
            .HasOne(pc => pc.Curso)
            .WithMany(c => c.PessoasCursos)
            .HasForeignKey(pc => pc.CursoId);
    }
}
