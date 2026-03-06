using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;

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
            return View();
        }

        public IActionResult Vacinacao()
        {
            return View();
        }

        public IActionResult Gestao()
        {
            return View();
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