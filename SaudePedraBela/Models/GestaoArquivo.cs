namespace SaudePedraBela.Models
{
    public class GestaoArquivo
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Arquivo { get; set; }

        public int GestaoAnoId { get; set; }
        public GestaoAno GestaoAno { get; set; }
    }
}
