using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    [LoginFilter]
    public class AdminGestaoController : Controller
    {
        private readonly SaudePedraBelaContext _context;

        public AdminGestaoController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var anos = _context.GestaoAnos
                .Include(a => a.Arquivos)
                .ToList();

            return View(anos);
        }

        [HttpPost]
        public IActionResult CriarAno(string ano)
        {
            var novo = new GestaoAno
            {
                Ano = ano
            };

            _context.GestaoAnos.Add(novo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadArquivo(int anoId, string titulo, IFormFile arquivo)
        {
            if (arquivo != null)
            {
                var nome = Guid.NewGuid() + Path.GetExtension(arquivo.FileName);

                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/gestao", nome);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                var novo = new GestaoArquivo
                {
                    Titulo = titulo,
                    Arquivo = nome,
                    GestaoAnoId = anoId
                };

                _context.GestaoArquivos.Add(novo);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult ExcluirAno(int id)
        {
            var ano = _context.GestaoAnos.Find(id);

            _context.GestaoAnos.Remove(ano);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ExcluirArquivo(int id)
        {
            var arq = _context.GestaoArquivos.Find(id);

            _context.GestaoArquivos.Remove(arq);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}