using System;

namespace SaudePedraBela.Models
{
    public class CampanhaVacinacao
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Status { get; set; } // Em andamento, Disponível

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string PublicoAlvo { get; set; }

        public string? Observacoes { get; set; }
    }

}