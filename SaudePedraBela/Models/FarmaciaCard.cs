namespace SaudePedraBela.Models
{
    public class FarmaciaCard
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Link { get; set; }

        public int Ordem { get; set; } // pra ordenar na tela
    }
}
