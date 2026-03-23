// Importações necessárias para o funcionamento do controller
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // Controller principal da aplicação, responsável pelas páginas públicas do site
    public class HomeController : Controller
    {
        // Logger para registrar eventos e erros da aplicação
        private readonly ILogger<HomeController> _logger;

        // Contexto do banco de dados, utilizado para acessar as tabelas via Entity Framework
        private readonly SaudePedraBelaContext _context;

        // Construtor: recebe o logger e o contexto por injeção de dependência
        public HomeController(ILogger<HomeController> logger, SaudePedraBelaContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Página do Plano Municipal:
        // Busca todas as categorias do banco de dados, incluindo os documentos relacionados a cada uma,
        // e envia a lista para a View correspondente
        public IActionResult PlanoMunicipal()
        {
            var categorias = _context.Categorias
                .Include(c => c.Documentos) // Carrega os documentos vinculados a cada categoria
                .ToList();
            return View(categorias);
        }

        // Página inicial do site (Index), apenas renderiza a View sem dados adicionais
        public IActionResult Index()
        {
            return View();
        }

        // Endpoint que retorna, em formato JSON, a lista de arquivos existentes
        // dentro de uma subpasta específica do diretório de escalas em PDF
        public IActionResult ListarArquivos(string pasta)
        {
            // Monta o caminho completo da pasta informada dentro de wwwroot/pdf/escalas
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/escalas", pasta);

            // Se a pasta não existir, retorna uma lista vazia em JSON
            if (!Directory.Exists(caminho))
                return Json(new List<string>());

            // Lista todos os arquivos da pasta e retorna apenas os nomes (sem o caminho completo)
            var arquivos = Directory.GetFiles(caminho)
                .Select(a => Path.GetFileName(a))
                .ToList();

            return Json(arquivos);
        }

        // Página de Locais e Horários, apenas renderiza a View sem dados adicionais
        public IActionResult LocalHorarios()
        {
            return View();
        }

        // Página de Escalas, apenas renderiza a View sem dados adicionais
        public IActionResult Escalas()
        {
            return View();
        }

        // Página da Farmácia:
        // Busca as configurações gerais da farmácia e os cards informativos (ordenados pelo campo Ordem)
        // e os disponibiliza para a View via ViewBag
        public IActionResult Farmacia()
        {
            var config = _context.FarmaciaConfigs.FirstOrDefault(); // Configuração geral (primeiro registro)
            var cards = _context.FarmaciaCards.OrderBy(c => c.Ordem).ToList(); // Cards ordenados

            ViewBag.Config = config;
            ViewBag.Cards = cards;
            return View();
        }

        // Página de Vacinação:
        // Busca campanhas, calendário vacinal, locais de vacinação e documentos relacionados
        // Os dados secundários são enviados via ViewBag, e as campanhas como model principal da View
        public IActionResult Vacinacao()
        {
            var campanhas = _context.CampanhasVacinacao.ToList();
            var calendario = _context.CalendarioVacinal.ToList();
            var locais = _context.LocaisVacinacao.ToList();
            var documentos = _context.DocumentosVacina.ToList();

            ViewBag.Calendario = calendario;
            ViewBag.Locais = locais;
            ViewBag.Documentos = documentos;

            return View(campanhas); // Campanhas enviadas diretamente como model
        }

        // Página de Gestão:
        // Busca os anos de gestão cadastrados no banco, incluindo os arquivos vinculados a cada ano,
        // e envia para a View
        public IActionResult Gestao()
        {
            var anos = _context.GestaoAnos
                .Include(a => a.Arquivos) // Carrega os arquivos relacionados a cada ano
                .ToList();

            return View(anos);
        }

        // Página de Pesquisa, apenas renderiza a View sem dados adicionais
        public IActionResult Pesquisa()
        {
            return View();
        }

        // Página de Privacidade, apenas renderiza a View sem dados adicionais
        public IActionResult Privacy()
        {
            return View();
        }

        // Página de Erro:
        // Desabilita o cache para garantir que erros sempre sejam exibidos com informações atualizadas
        // Captura o ID da requisição atual para exibição no modelo de erro
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Usa o ID da atividade atual ou o identificador da requisição HTTP como fallback
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}