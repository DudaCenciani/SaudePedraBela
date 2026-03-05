using System.ComponentModel.DataAnnotations.Schema;

namespace SaudePedraBela.Models
{
    public class Documento
    {
        private int id;
        private string nomeDocumento;
        private string caminhoDocumento;
        private int idCategoria;
        public int Id { get => id; set => id = value; }
        public string NomeDocumento { get => nomeDocumento; set => nomeDocumento = value; }
        public string CaminhoDocumento { get => caminhoDocumento; set => caminhoDocumento = value; }
        public int IdCategoria { get => idCategoria; set => idCategoria = value; }

        [ForeignKey("IdCategoria")]
        public Categorias Categorias { get; set; }
    }
}
