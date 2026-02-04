using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_cadastro_pessoa.Models;

/// <summary>
/// Representa uma pessoa no sistema de cadastro
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador único da pessoa
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo da pessoa
    /// </summary>
    /// <example>João Silva Santos</example>
    [Required(ErrorMessage = "O nome completo é obrigatório")]
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail
    /// </summary>
    /// <example>joao.silva@email.com</example>
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// CPF da pessoa (apenas números ou formatado)
    /// </summary>
    /// <example>123.456.789-00</example>
    [Required(ErrorMessage = "O CPF é obrigatório")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Data de nascimento
    /// </summary>
    /// <example>1990-05-15</example>
    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Peso em quilogramas
    /// </summary>
    /// <example>75.5</example>
    [Range(0.1, 500, ErrorMessage = "O peso deve estar entre 0.1 e 500 kg")]
    public double Peso { get; set; }

    /// <summary>
    /// Número de telefone
    /// </summary>
    /// <example>(11) 99999-8888</example>
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    public string Telefone { get; set; } = string.Empty;
}
