namespace SaudePedraBela.Models
{
    public class Categorias
    {
        private int id;
        private string nomeCategoria;

        public int Id { get => id; set => id = value; }
        public string NomeCategoria { get => nomeCategoria; set => nomeCategoria = value; }
        public List<Documento> Documentos { get; set; }
    }
}