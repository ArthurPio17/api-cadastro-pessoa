using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_cadastro_pessoa.Models;

/// <summary>
/// Representa um curso no sistema
/// </summary>
public class Curso
{
    /// <summary>
    /// Identificador único do curso
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Nome do curso
    /// </summary>
    /// <example>Desenvolvimento Web com .NET</example>
    [Required(ErrorMessage = "O nome do curso é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Horário das aulas
    /// </summary>
    /// <example>19:00 - 22:00</example>
    [Required(ErrorMessage = "O horário é obrigatório")]
    public string Horario { get; set; } = string.Empty;

    /// <summary>
    /// Carga horária total do curso em horas
    /// </summary>
    /// <example>120</example>
    [Range(1, 10000, ErrorMessage = "A carga horária deve estar entre 1 e 10000 horas")]
    public int CargaHoraria { get; set; }

    /// <summary>
    /// Duração do curso (ex: 6 meses, 1 ano)
    /// </summary>
    /// <example>6 meses</example>
    [Required(ErrorMessage = "O tempo de curso é obrigatório")]
    public string TempoDeCurso { get; set; } = string.Empty;

    /// <summary>
    /// Pessoas inscritas no curso
    /// </summary>
    [JsonIgnore]
    public ICollection<PessoaCurso> PessoasCursos { get; set; } = [];

    /// <summary>
    /// Quantidade de pessoas inscritas no curso
    /// </summary>
    public int QuantidadePessoasInscritas => PessoasCursos?.Count ?? 0;
}
