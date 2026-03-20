using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;


namespace SaudePedraBela.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SaudePedraBelaContext _context;

        public HomeController(ILogger<HomeController> logger, SaudePedraBelaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult PlanoMunicipal()
        {
            var categorias = _context.Categorias
                .Include(c => c.Documentos)
                .ToList();

            return View(categorias);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListarArquivos(string pasta)
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/escalas", pasta);

            if (!Directory.Exists(caminho))
                return Json(new List<string>());

            var arquivos = Directory.GetFiles(caminho)
                .Select(a => Path.GetFileName(a))
                .ToList();

            return Json(arquivos);
        }

        // NOVAS PÁGINAS

        public IActionResult LocalHorarios()
        {
            return View();
        }

        public IActionResult Escalas()
        {
            return View();
        }

        public IActionResult Farmacia()
        {
            var config = _context.FarmaciaConfigs.FirstOrDefault();
            var cards = _context.FarmaciaCards.OrderBy(c => c.Ordem).ToList();

            ViewBag.Config = config;
            ViewBag.Cards = cards;

            return View();
        }

        public IActionResult Vacinacao()
        {
            var campanhas = _context.CampanhasVacinacao.ToList();
            var calendario = _context.CalendarioVacinal.ToList();
            var locais = _context.LocaisVacinacao.ToList();
            var documentos = _context.DocumentosVacina.ToList();

            ViewBag.Calendario = calendario;
            ViewBag.Locais = locais;
            ViewBag.Documentos = documentos;

            return View(campanhas);
        }

        public IActionResult Gestao()
        {
            var anos = _context.GestaoAnos
                .Include(a => a.Arquivos)
                .ToList();

         

            return View(anos);
        }

        public IActionResult Pesquisa()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}