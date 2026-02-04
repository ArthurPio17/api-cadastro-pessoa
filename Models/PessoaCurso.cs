using System.Text.Json.Serialization;

namespace api_cadastro_pessoa.Models;

/// <summary>
/// Representa a relação entre Pessoa e Curso (tabela de junção)
/// </summary>
public class PessoaCurso
{
    /// <summary>
    /// ID da pessoa
    /// </summary>
    public int PessoaId { get; set; }

    /// <summary>
    /// Pessoa inscrita
    /// </summary>
    [JsonIgnore]
    public Pessoa Pessoa { get; set; } = null!;

    /// <summary>
    /// ID do curso
    /// </summary>
    public int CursoId { get; set; }

    /// <summary>
    /// Curso em que está inscrito
    /// </summary>
    [JsonIgnore]
    public Curso Curso { get; set; } = null!;

    /// <summary>
    /// Data de inscrição
    /// </summary>
    /// <example>2024-01-15</example>
    public DateTime DataInscricao { get; set; } = DateTime.Now;
}
