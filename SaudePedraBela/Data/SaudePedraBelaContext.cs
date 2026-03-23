// Importações necessárias para o contexto do banco de dados
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Models;

namespace SaudePedraBela.Data
{
    // Classe de contexto do banco de dados da aplicação
    // Herda de DbContext (Entity Framework Core) e centraliza o acesso a todas as tabelas do sistema
    public class SaudePedraBelaContext : DbContext
    {
        // Construtor: recebe as opções de configuração do contexto (string de conexão, provedor, etc.)
        // e repassa para a classe base DbContext
        public SaudePedraBelaContext(DbContextOptions<SaudePedraBelaContext> options)
            : base(options)
        {
        }

        // Tabela de usuários do sistema (utilizada para autenticação/login)
        public DbSet<SaudePedraBela.Models.Usuario> Usuario { get; set; } = default!;

        // Tabela de categorias (utilizada no Plano Municipal para agrupar documentos)
        public DbSet<SaudePedraBela.Models.Categorias> Categorias { get; set; } = default!;

        // Tabela de documentos vinculados às categorias do Plano Municipal
        public DbSet<SaudePedraBela.Models.Documento> Documento { get; set; } = default!;

        // Tabela de campanhas de vacinação (ex: campanha de gripe, sarampo, etc.)
        public DbSet<CampanhaVacinacao> CampanhasVacinacao { get; set; }

        // Tabela do calendário vacinal (vacinas recomendadas por faixa etária)
        public DbSet<CalendarioVacinal> CalendarioVacinal { get; set; }

        // Tabela de locais de vacinação (nome e horário de atendimento)
        public DbSet<LocalVacinacao> LocaisVacinacao { get; set; }

        // Tabela de escalas de profissionais de saúde
        public DbSet<Escala> Escalas { get; set; }

        // Tabela de documentos PDF relacionados à vacinação
        public DbSet<DocumentoVacina> DocumentosVacina { get; set; }

        // Tabela de configurações gerais da página da Farmácia
        public DbSet<FarmaciaConfig> FarmaciaConfigs { get; set; }

        // Tabela de cards informativos exibidos na página da Farmácia
        public DbSet<FarmaciaCard> FarmaciaCards { get; set; }

        // Tabela de anos de gestão (utilizada na página de Gestão)
        public DbSet<GestaoAno> GestaoAnos { get; set; }

        // Tabela de arquivos vinculados a cada ano de gestão
        public DbSet<GestaoArquivo> GestaoArquivos { get; set; }
    }
}