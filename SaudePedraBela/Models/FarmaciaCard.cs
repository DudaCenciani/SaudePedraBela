namespace SaudePedraBela.Models
{
    public class FarmaciaCard
    {
        public int Id { get; set; }

        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Link { get; set; }

        public string? Arquivo { get; set; } // nome do PDF salvo

        public int? Ordem { get; set; } // pra ordenar na tela
    }
}
