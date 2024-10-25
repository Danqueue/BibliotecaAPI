using BibliotecaAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace BibliotecaAPI.Repositories
{
    public class BibliotecaRepository
    {
        private readonly string _connectionString;

        public BibliotecaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        
        public async Task<IEnumerable<Livro>> ListarLivros()
        {
            using (var conn = Connection)
            {
                return await conn.QueryAsync<Livro>("SELECT * FROM Livros");
            }
        }

        public async Task AdicionarLivro(Livro livro)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Livros (Titulo, Autor, AnoPublicacao, Genero) VALUES (@Titulo, @Autor, @AnoPublicacao, @Genero)";
                await conn.ExecuteAsync(sql, livro);
            }
        }

        public async Task<Livro> ObterLivroPorId(int id)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Livros WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Livro>(sql, new { Id = id });
            }
        }

        public async Task AtualizarLivro(Livro livro)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Livros SET Titulo = @Titulo, Autor = @Autor, AnoPublicacao = @AnoPublicacao, Genero = @Genero WHERE Id = @Id";
                await conn.ExecuteAsync(sql, livro);
            }
        }

        public async Task ExcluirLivro(int id)
        {
            using (var conn = Connection)
            {
                var sql = "DELETE FROM Livros WHERE Id = @Id";
                await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        
        public async Task<bool> RegistrarEmprestimo(Emprestimo emprestimo)
        {
            using (var conn = Connection)
            {
                var livro = await ObterLivroPorId(emprestimo.LivroId);
                if (livro == null || !livro.Disponivel || livro.Emprestado)
                {
                    return false;
                }

                livro.Disponivel = false;
                livro.Emprestado = true;
                await AtualizarLivro(livro);

                var sql = "INSERT INTO Emprestimos (LivroId, UsuarioId, DataEmprestimo, DataDevolucao) VALUES (@LivroId, @UsuarioId, @DataEmprestimo, @DataDevolucao)";
                emprestimo.DataEmprestimo = DateTime.Now;
                emprestimo.DataDevolucao = emprestimo.DataEmprestimo.AddDays(14); 
                await conn.ExecuteAsync(sql, emprestimo);
                return true;
            }
        }

        public async Task<bool> RegistrarDevolucao(int emprestimoId)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Emprestimos SET DataDevolucao = @DataDevolucao WHERE Id = @Id AND DataDevolucao IS NULL";
                var rowsAffected = await conn.ExecuteAsync(sql, new { DataDevolucao = DateTime.Now, Id = emprestimoId });
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<Emprestimo>> ListarHistoricoEmprestimos(int usuarioId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Emprestimos WHERE UsuarioId = @UsuarioId";
                return await conn.QueryAsync<Emprestimo>(sql, new { UsuarioId = usuarioId });
            }
        }

        
        public async Task CadastrarUsuario(Usuario usuario)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Usuarios (Nome, Email) VALUES (@Nome, @Email)";
                await conn.ExecuteAsync(sql, usuario);
            }
        }

        public async Task<Usuario> ObterUsuarioPorId(int id)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Usuarios WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
            }
        }

        public async Task ExcluirUsuario(int id)
        {
            using (var conn = Connection)
            {
                var sql = "DELETE FROM Usuarios WHERE Id = @Id";
                await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Usuario>> ListarUsuarios()
        {
            using (var conn = Connection)
            {
                return await conn.QueryAsync<Usuario>("SELECT * FROM Usuarios");
            }
        }

        public async Task<IEnumerable<Livro>> ConsultarLivros(string? genero, string? autor, int? ano)
        {
            using (var conn = Connection)
            {
                var query = "SELECT * FROM Livros WHERE (@Genero IS NULL OR Genero = @Genero) AND (@Autor IS NULL OR Autor = @Autor) AND (@Ano IS NULL OR AnoPublicacao = @Ano)";
                return await conn.QueryAsync<Livro>(query, new { Genero = genero, Autor = autor, Ano = ano });
            }
        }
        public async Task<IEnumerable<Usuario>> BuscarUsuarios(string? nome, string? email)
        {
            using (var conn = Connection)
            {
                var query = "SELECT * FROM Usuarios WHERE (@Nome IS NULL OR Nome LIKE CONCAT('%', @Nome, '%')) AND (@Email IS NULL OR Email LIKE CONCAT('%', @Email, '%'))";
                return await conn.QueryAsync<Usuario>(query, new { Nome = nome, Email = email });
            }
        }
        public async Task<bool> VerificarLivroEmprestado(int livroId)
        {
            using (var conn = Connection)
            {
                var query = "SELECT COUNT(*) FROM Emprestimos WHERE LivroId = @LivroId AND DataDevolucao IS NULL";
                var emprestado = await conn.ExecuteScalarAsync<int>(query, new { LivroId = livroId });
                return emprestado > 0;
            }
        }


    }
}
