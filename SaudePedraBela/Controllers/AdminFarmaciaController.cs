// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

// Importa os Models utilizados neste controller
using SaudePedraBela.Models;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // [LoginFilter] = protege TODAS as actions deste controller
    // Qualquer pessoa que tentar acessar sem estar logada será redirecionada para o login
    [LoginFilter]
    public class AdminFarmaciaController : Controller
    {
        // Variável que representa a conexão com o banco de dados
        // "readonly" significa que só pode ser definida uma vez (no construtor)
        private readonly SaudePedraBelaContext _context;

        // Construtor do controller
        // O ASP.NET injeta automaticamente o contexto do banco de dados aqui
        public AdminFarmaciaController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        // ===================================================
        // ACTION: Index
        // Página principal do admin de farmácia
        // Carrega a configuração de horário/local E os cards de medicamentos
        // ===================================================
        public IActionResult Index()
        {
            // Busca a configuração da farmácia (horário e local)
            // FirstOrDefault = pega o primeiro registro, ou null se não houver nenhum
            var config = _context.FarmaciaConfigs.FirstOrDefault();

            // Busca todos os cards de medicamentos ordenados pelo campo "Ordem"
            var cards = _context.FarmaciaCards.OrderBy(c => c.Ordem).ToList();

            // Passa os cards para a View via ViewBag
            // (config vai direto como Model da View)
            ViewBag.Cards = cards;

            // Retorna a View passando config como Model
            // (Views/AdminFarmacia/Index.cshtml)
            return View(config);
        }

        // ===================================================
        // ACTION: SalvarConfig
        // Salva ou atualiza o horário e local da farmácia
        // Chamada quando o formulário de configuração é enviado
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        public IActionResult SalvarConfig(FarmaciaConfig model)
        {
            // Verifica se já existe uma configuração no banco
            var config = _context.FarmaciaConfigs.FirstOrDefault();

            if (config == null)
                // Se não existe nenhuma configuração ainda, adiciona uma nova
                _context.FarmaciaConfigs.Add(model);
            else
                // Se já existe, apenas atualiza os campos
                config.Horario = model.Horario;
            config.Local = model.Local;

            // Salva as alterações no banco de dados
            _context.SaveChanges();

            // Volta para a página principal do admin de farmácia
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: CriarCard (GET)
        // Apenas retorna a View do formulário de criação de card
        // Não é muito usada pois o formulário fica no modal da Index
        // ===================================================
        public IActionResult CriarCard()
        {
            return View();
        }

        // ===================================================
        // ACTION: CriarCard (POST)
        // Salva um novo card de medicamento no banco
        // Recebe os dados do formulário + o arquivo PDF (se houver)
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        public async Task<IActionResult> CriarCard(FarmaciaCard card, IFormFile arquivoPdf)
        {
            // Verifica se foi enviado um arquivo PDF
            if (arquivoPdf != null && arquivoPdf.Length > 0)
            {
                // Gera um nome único para o arquivo usando GUID
                // Isso evita conflito de nomes caso dois arquivos tenham o mesmo nome
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivoPdf.FileName);

                // Monta o caminho completo onde o arquivo será salvo no servidor
                var caminho = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/pdf/farmacia",
                    nomeArquivo
                );

                // Salva o arquivo fisicamente na pasta wwwroot/pdf/farmacia
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivoPdf.CopyToAsync(stream);
                }

                // Salva o nome do arquivo no card para referenciar depois
                card.Arquivo = nomeArquivo;

                // Garante que o Link fica nulo quando tem PDF
                // (não pode usar PDF e Link ao mesmo tempo)
                card.Link = null;
            }

            // Adiciona o novo card ao banco de dados
            _context.FarmaciaCards.Add(card);

            // Salva as alterações de forma assíncrona (não trava o sistema enquanto salva)
            await _context.SaveChangesAsync();

            // Volta para a página principal do admin de farmácia
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: EditarCard (POST)
        // Atualiza os dados de um card já existente no banco
        // Chamada quando o formulário de edição é enviado
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        public IActionResult EditarCard(FarmaciaCard card)
        {
            // Busca o card no banco pelo Id recebido
            var cardDb = _context.FarmaciaCards.Find(card.Id);

            // Se o card foi encontrado no banco
            if (cardDb != null)
            {
                // Atualiza cada campo com os novos valores do formulário
                cardDb.Titulo = card.Titulo;
                cardDb.Descricao = card.Descricao;

                // Se o novo Link não for nulo usa o novo, senão mantém o que já tinha
                cardDb.Link = card.Link ?? cardDb.Link;
                cardDb.Ordem = card.Ordem;

                // Salva as alterações no banco de dados
                _context.SaveChanges();
            }

            // Volta para a página principal do admin de farmácia
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: ExcluirCard
        // Remove um card de medicamento do banco de dados
        // Recebe o Id do card a ser excluído pela URL
        // Exemplo: /AdminFarmacia/ExcluirCard/3
        // ===================================================
        public IActionResult ExcluirCard(int id)
        {
            // Busca o card no banco pelo Id recebido
            var card = _context.FarmaciaCards.Find(id);

            // Se o card foi encontrado no banco
            if (card != null)
            {
                // Remove o card do banco de dados
                _context.FarmaciaCards.Remove(card);

                // Salva as alterações no banco de dados
                _context.SaveChanges();
            }

            // Volta para a página principal do admin de farmácia
            return RedirectToAction("Index");
        }
    }
}