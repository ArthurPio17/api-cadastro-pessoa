using api_cadastro_pessoa.Data;
using api_cadastro_pessoa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_cadastro_pessoa.Controllers;

/// <summary>
/// Controller para gerenciamento de cadastro de pessoas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PessoasController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Obtém todas as pessoas cadastradas
    /// </summary>
    /// <returns>Lista de pessoas</returns>
    /// <response code="200">Retorna a lista de pessoas</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Pessoa>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pessoa>>> GetAll()
    {
        return Ok(await _context.Pessoas.ToListAsync());
    }

    /// <summary>
    /// Pesquisa pessoas pelo nome
    /// </summary>
    /// <param name="nome">Nome ou parte do nome para pesquisar</param>
    /// <returns>Lista de pessoas encontradas</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     GET /api/pessoas/buscar?nome=João
    ///
    /// </remarks>
    /// <response code="200">Retorna a lista de pessoas encontradas</response>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<Pessoa>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pessoa>>> SearchByName([FromQuery] string nome)
    {
        var pessoas = await _context.Pessoas
            .Where(p => p.NomeCompleto.Contains(nome, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();

        return Ok(pessoas);
    }

    /// <summary>
    /// Obtém os cursos em que uma pessoa está inscrita
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <returns>Lista de cursos da pessoa</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     GET /api/pessoas/1/cursos
    ///
    /// </remarks>
    /// <response code="200">Retorna a lista de cursos da pessoa</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpGet("{id}/cursos")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetCursosDaPessoa(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound("Pessoa não encontrada");

        var cursos = await _context.PessoasCursos
            .Where(pc => pc.PessoaId == id)
            .Include(pc => pc.Curso)
            .Select(pc => new
            {
                pc.Curso.Id,
                pc.Curso.Nome,
                pc.Curso.Horario,
                pc.Curso.CargaHoraria,
                pc.Curso.TempoDeCurso,
                pc.DataInscricao
            })
            .ToListAsync();

        return Ok(new
        {
            Pessoa = pessoa.NomeCompleto,
            QuantidadeCursos = cursos.Count,
            Cursos = cursos
        });
    }

    /// <summary>
    /// Obtém uma pessoa pelo ID
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <returns>Dados da pessoa</returns>
    /// <response code="200">Retorna a pessoa encontrada</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Pessoa), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pessoa>> GetById(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        return Ok(pessoa);
    }

    /// <summary>
    /// Cadastra uma nova pessoa
    /// </summary>
    /// <param name="pessoa">Dados da pessoa</param>
    /// <returns>Pessoa cadastrada</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/pessoas
    ///     {
    ///        "nomeCompleto": "João Silva Santos",
    ///        "email": "joao.silva@email.com",
    ///        "cpf": "123.456.789-00",
    ///        "dataNascimento": "1990-05-15",
    ///        "peso": 75.5,
    ///        "telefone": "(11) 99999-8888"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Pessoa cadastrada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(Pessoa), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pessoa>> Create(Pessoa pessoa)
    {
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = pessoa.Id }, pessoa);
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <param name="pessoaAtualizada">Dados atualizados</param>
    /// <returns>Sem conteúdo</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     PUT /api/pessoas/1
    ///     {
    ///        "nomeCompleto": "João Silva Santos Atualizado",
    ///        "email": "joao.novo@email.com",
    ///        "cpf": "123.456.789-00",
    ///        "dataNascimento": "1990-05-15",
    ///        "peso": 78.0,
    ///        "telefone": "(11) 98888-7777"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Pessoa atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Pessoa pessoaAtualizada)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        pessoa.NomeCompleto = pessoaAtualizada.NomeCompleto;
        pessoa.Email = pessoaAtualizada.Email;
        pessoa.Cpf = pessoaAtualizada.Cpf;
        pessoa.DataNascimento = pessoaAtualizada.DataNascimento;
        pessoa.Peso = pessoaAtualizada.Peso;
        pessoa.Telefone = pessoaAtualizada.Telefone;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove uma pessoa do cadastro
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Pessoa removida com sucesso</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
