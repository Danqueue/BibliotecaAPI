using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly BibliotecaRepository _bibliotecaRepository;

        public LivrosController(BibliotecaRepository bibliotecaRepository)
        {
            _bibliotecaRepository = bibliotecaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarLivros()
        {
            var livros = await _bibliotecaRepository.ListarLivros();
            return Ok(livros);
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarLivro([FromBody] Livro livro)
        {
            await _bibliotecaRepository.AdicionarLivro(livro);
            return CreatedAtAction(nameof(ListarLivros), new { id = livro.Id }, livro);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarLivro(int id, [FromBody] Livro livro)
        {
            if (id != livro.Id)
            {
                return BadRequest();
            }

            await _bibliotecaRepository.AtualizarLivro(livro);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirLivro(int id)
        {
            var livro = await _bibliotecaRepository.ObterLivroPorId(id);
            if (livro == null)
            {
                return NotFound();
            }

            var emprestado = await _bibliotecaRepository.VerificarLivroEmprestado(id);
            if (emprestado)
            {
                return BadRequest("O livro está emprestado e não pode ser excluído.");
            }

            await _bibliotecaRepository.ExcluirLivro(id);
            return NoContent();
        }

        [HttpPost("emprestimo")]
        public async Task<IActionResult> RegistrarEmprestimo([FromBody] Emprestimo emprestimo)
        {
            var resultado = await _bibliotecaRepository.RegistrarEmprestimo(emprestimo);
            if (!resultado)
            {
                return BadRequest("Livro não disponível para empréstimo.");
            }
            return Created("Empréstimo registrado com sucesso", emprestimo);
        }

        [HttpPut("devolucao/{id}")]
        public async Task<IActionResult> RegistrarDevolucao(int id)
        {
            var resultado = await _bibliotecaRepository.RegistrarDevolucao(id);
            if (!resultado)
            {
                return NotFound("Empréstimo não encontrado ou já devolvido.");
            }
            return NoContent();
        }

        [HttpGet("historico/{usuarioId}")]
        public async Task<IActionResult> ConsultarHistoricoEmprestimos(int usuarioId)
        {
            var historico = await _bibliotecaRepository.ListarHistoricoEmprestimos(usuarioId);
            return Ok(historico);
        }

        

        [HttpPost("usuario")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest("Usuário não pode ser nulo.");
            }

            await _bibliotecaRepository.CadastrarUsuario(usuario);
            return CreatedAtAction(nameof(ListarUsuarios), new { id = usuario.Id }, usuario);
        }

        


        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _bibliotecaRepository.ListarUsuarios();
            return Ok(usuarios);
        }
        [HttpGet("consultar")]
        public async Task<IActionResult> ConsultarLivros([FromQuery] string? genero, [FromQuery] string? autor, [FromQuery] int? ano)
        {
            var livros = await _bibliotecaRepository.ConsultarLivros(genero, autor, ano);
            return Ok(livros);
        }
        [HttpGet("usuarios/consultar")]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string? nome, [FromQuery] string? email)
        {
            var usuarios = await _bibliotecaRepository.BuscarUsuarios(nome, email);
            return Ok(usuarios);
        }

    }
}
