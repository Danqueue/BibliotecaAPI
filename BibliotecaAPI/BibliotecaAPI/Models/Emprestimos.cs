namespace BibliotecaAPI.Models
{
    public class Emprestimo
    {
        public int Id { get; set; }
        public int LivroId { get; set; }
        public int Livro { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucao { get; set; }
    }
}