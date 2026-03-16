using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Models;

namespace SaudePedraBela.Data
{
    public class SaudePedraBelaContext : DbContext
    {
        public SaudePedraBelaContext (DbContextOptions<SaudePedraBelaContext> options)
            : base(options)
        {
        }

        public DbSet<SaudePedraBela.Models.Usuario> Usuario { get; set; } = default!;
        public DbSet<SaudePedraBela.Models.Categorias> Categorias { get; set; } = default!;
        public DbSet<SaudePedraBela.Models.Documento> Documento { get; set; } = default!;

        public DbSet<CampanhaVacinacao> CampanhasVacinacao { get; set; }

        public DbSet<CalendarioVacinal> CalendarioVacinal { get; set; }

        public DbSet<LocalVacinacao> LocaisVacinacao { get; set; }

        public DbSet<Escala> Escalas { get; set; }

        public DbSet<DocumentoVacina> DocumentosVacina { get; set; }

    }
}
