using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class EscalasAdminController : Controller
{
    private readonly SaudePedraBelaContext _context;
    private readonly IWebHostEnvironment _env;

    public EscalasAdminController(SaudePedraBelaContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        var escalas = _context.Escalas.ToList();
        return View(escalas);
    }

    public IActionResult ListarArquivos(string pasta)
    {
        var arquivos = _context.Escalas
            .Where(e => e.Categoria == pasta)
            .OrderByDescending(e => e.Ano)
            .ThenByDescending(e => e.Mes)
            .Select(e => new
            {
                nome = e.NomeArquivo,
                caminho = e.CaminhoArquivo,
                mes = e.Mes,
                ano = e.Ano
            })
            .ToList();

        return Json(arquivos);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile arquivo, string categoria, int mes, int ano)
    {

        Console.WriteLine("Categoria recebida: " + categoria);
        if (arquivo != null && arquivo.Length > 0)
        {
            string pastaCategoria = categoria;

            var pasta = Path.Combine(
     _env.WebRootPath,
     "pdf",
     "escalas",
     pastaCategoria,
     ano.ToString(),
     mes.ToString("D2")
 );
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo.FileName);
            var caminho = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var escalas = new Escala
            {
                Categoria = categoria,
                NomeArquivo = arquivo.FileName,
                CaminhoArquivo = $"/pdf/escalas/{pastaCategoria}/{ano}/{mes:D2}/{nomeArquivo}",
                Mes = mes,
                Ano = ano,
                DataUpload = DateTime.Now
            };

            _context.Escalas.Add(escalas);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    public IActionResult Excluir(int id)
    {
        var escala = _context.Escalas.FirstOrDefault(e => e.Id == id);

        if (escala != null)
        {
            var caminho = Path.Combine(_env.WebRootPath, escala.CaminhoArquivo.TrimStart('/'));

            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);

            _context.Escalas.Remove(escala);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}