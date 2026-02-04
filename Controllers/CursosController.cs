using api_cadastro_pessoa.Data;
using api_cadastro_pessoa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_cadastro_pessoa.Controllers;

/// <summary>
/// Controller para gerenciamento de cursos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CursosController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Obtém todos os cursos
    /// </summary>
    /// <returns>Lista de cursos</returns>
    /// <response code="200">Retorna a lista de cursos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Curso>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Curso>>> GetAll()
    {
        var cursos = await _context.Cursos
            .Include(c => c.PessoasCursos)
            .ToListAsync();
        return Ok(cursos);
    }

    /// <summary>
    /// Obtém um curso pelo ID
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Dados do curso</returns>
    /// <response code="200">Retorna o curso encontrado</response>
    /// <response code="404">Curso não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Curso), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Curso>> GetById(int id)
    {
        var curso = await _context.Cursos
            .Include(c => c.PessoasCursos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso is null)
            return NotFound();

        return Ok(curso);
    }

    /// <summary>
    /// Obtém todas as pessoas inscritas em um curso
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Lista de pessoas inscritas</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     GET /api/cursos/1/pessoas
    ///
    /// </remarks>
    /// <response code="200">Retorna a lista de pessoas inscritas</response>
    /// <response code="404">Curso não encontrado</response>
    [HttpGet("{id}/pessoas")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPessoasDoCurso(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
            return NotFound("Curso não encontrado");

        var pessoas = await _context.PessoasCursos
            .Where(pc => pc.CursoId == id)
            .Include(pc => pc.Pessoa)
            .Select(pc => new
            {
                pc.Pessoa.Id,
                pc.Pessoa.NomeCompleto,
                pc.Pessoa.Email,
                pc.DataInscricao
            })
            .ToListAsync();

        return Ok(new
        {
            Curso = curso.Nome,
            QuantidadeInscritos = pessoas.Count,
            Pessoas = pessoas
        });
    }

    /// <summary>
    /// Cadastra um novo curso
    /// </summary>
    /// <param name="curso">Dados do curso</param>
    /// <returns>Curso cadastrado</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/cursos
    ///     {
    ///        "nome": "Desenvolvimento Web com .NET",
    ///        "horario": "19:00 - 22:00",
    ///        "cargaHoraria": 120,
    ///        "tempoDeCurso": "6 meses"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Curso cadastrado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(Curso), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Curso>> Create(Curso curso)
    {
        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = curso.Id }, curso);
    }

    /// <summary>
    /// Inscreve uma pessoa em um curso
    /// </summary>
    /// <param name="cursoId">ID do curso</param>
    /// <param name="pessoaId">ID da pessoa</param>
    /// <returns>Confirmação da inscrição</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/cursos/1/pessoas/1
    ///
    /// </remarks>
    /// <response code="200">Pessoa inscrita com sucesso</response>
    /// <response code="400">Pessoa já está inscrita neste curso</response>
    /// <response code="404">Curso ou pessoa não encontrado</response>
    [HttpPost("{cursoId}/pessoas/{pessoaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InscreverPessoa(int cursoId, int pessoaId)
    {
        var curso = await _context.Cursos.FindAsync(cursoId);
        if (curso is null)
            return NotFound("Curso não encontrado");

        var pessoa = await _context.Pessoas.FindAsync(pessoaId);
        if (pessoa is null)
            return NotFound("Pessoa não encontrada");

        var inscricaoExistente = await _context.PessoasCursos
            .AnyAsync(pc => pc.CursoId == cursoId && pc.PessoaId == pessoaId);

        if (inscricaoExistente)
            return BadRequest("Pessoa já está inscrita neste curso");

        var pessoaCurso = new PessoaCurso
        {
            PessoaId = pessoaId,
            CursoId = cursoId,
            DataInscricao = DateTime.Now
        };

        _context.PessoasCursos.Add(pessoaCurso);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Mensagem = "Pessoa inscrita com sucesso",
            Pessoa = pessoa.NomeCompleto,
            Curso = curso.Nome,
            DataInscricao = pessoaCurso.DataInscricao
        });
    }

    /// <summary>
    /// Remove uma pessoa de um curso
    /// </summary>
    /// <param name="cursoId">ID do curso</param>
    /// <param name="pessoaId">ID da pessoa</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Pessoa removida do curso com sucesso</response>
    /// <response code="404">Inscrição não encontrada</response>
    [HttpDelete("{cursoId}/pessoas/{pessoaId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverPessoa(int cursoId, int pessoaId)
    {
        var inscricao = await _context.PessoasCursos
            .FirstOrDefaultAsync(pc => pc.CursoId == cursoId && pc.PessoaId == pessoaId);

        if (inscricao is null)
            return NotFound("Inscrição não encontrada");

        _context.PessoasCursos.Remove(inscricao);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Atualiza os dados de um curso
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <param name="cursoAtualizado">Dados atualizados</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Curso atualizado com sucesso</response>
    /// <response code="404">Curso não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Curso cursoAtualizado)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
            return NotFound();

        curso.Nome = cursoAtualizado.Nome;
        curso.Horario = cursoAtualizado.Horario;
        curso.CargaHoraria = cursoAtualizado.CargaHoraria;
        curso.TempoDeCurso = cursoAtualizado.TempoDeCurso;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove um curso
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Curso removido com sucesso</response>
    /// <response code="404">Curso não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);

        if (curso is null)
            return NotFound();

        _context.Cursos.Remove(curso);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
