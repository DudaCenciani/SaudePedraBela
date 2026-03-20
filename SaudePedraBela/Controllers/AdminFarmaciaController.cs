using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters; // ADICIONE ESSE USING

namespace SaudePedraBela.Controllers
{
    [LoginFilter]
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
                config.Horario = model.Horario;
                config.Local = model.Local;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult CriarCard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarCard(FarmaciaCard card, IFormFile arquivoPdf)
        {
            if (arquivoPdf != null && arquivoPdf.Length > 0)
            {
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivoPdf.FileName);

                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/farmacia", nomeArquivo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivoPdf.CopyToAsync(stream);
                }

                card.Arquivo = nomeArquivo;
                card.Link = null; // garante que não usa os dois
            }

            _context.FarmaciaCards.Add(card);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // EDITAR CARD
        [HttpPost]
        public IActionResult EditarCard(FarmaciaCard card)
        {
            var cardDb = _context.FarmaciaCards.Find(card.Id);

            if (cardDb != null)
            {
                cardDb.Titulo = card.Titulo;
                cardDb.Descricao = card.Descricao;
                cardDb.Link = card.Link ?? cardDb.Link;
                cardDb.Ordem = card.Ordem;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // EXCLUIR CARD

        public IActionResult ExcluirCard(int id)
        {
            var card = _context.FarmaciaCards.Find(id);

            if (card != null)
            {
                _context.FarmaciaCards.Remove(card);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
