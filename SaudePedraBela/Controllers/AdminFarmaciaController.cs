using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using SaudePedraBela.Models;

namespace SaudePedraBela.Controllers
{
    public class AdminFarmaciaController : Controller
    {
        private readonly SaudePedraBelaContext _context;

        public AdminFarmaciaController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var config = _context.FarmaciaConfigs.FirstOrDefault();
            var cards = _context.FarmaciaCards.OrderBy(c => c.Ordem).ToList();

            ViewBag.Cards = cards;

            return View(config);
        }
        [HttpPost]
        public IActionResult SalvarConfig(FarmaciaConfig model)
        {
            var config = _context.FarmaciaConfigs.FirstOrDefault();

            if (config == null)
                _context.FarmaciaConfigs.Add(model);
            else
                _context.Entry(config).CurrentValues.SetValues(model);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult CriarCard()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CriarCard(FarmaciaCard card)
        {
            _context.FarmaciaCards.Add(card);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
