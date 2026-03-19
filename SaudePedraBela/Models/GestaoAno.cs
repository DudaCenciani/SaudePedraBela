namespace SaudePedraBela.Models
{
    public class GestaoAno
    {
        public int Id { get; set; }

        public string Ano { get; set; }

        public List<GestaoArquivo> Arquivos { get; set; }
    }
}
