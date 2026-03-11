using System;

namespace SeuProjeto.Models
{
    public class CampanhaVacinacao
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string PublicoAlvo { get; set; }

        public bool Ativa { get; set; }
    }
}