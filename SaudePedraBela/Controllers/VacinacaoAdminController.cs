using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using SaudePedraBela.Models;

public class VacinacaoAdminController : Controller
{
    private readonly SaudePedraBelaContext _context;

    public VacinacaoAdminController(SaudePedraBelaContext context)
    {
        _context = context;
    }



    public IActionResult Index()
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
    [HttpPost]
    public IActionResult CriarCampanha(CampanhaVacinacao campanha)
    {
        _context.CampanhasVacinacao.Add(campanha);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // ABRIR tela de edição
    [HttpPost]
    public IActionResult EditarCampanha(CampanhaVacinacao campanha)
    {
        var campanhaDb = _context.CampanhasVacinacao.Find(campanha.Id);

        if (campanhaDb != null)
        {
            campanhaDb.Titulo = campanha.Titulo;
            campanhaDb.DataInicio = campanha.DataInicio;
            campanhaDb.DataFim = campanha.DataFim;
            campanhaDb.Status = campanha.Status;
            campanhaDb.PublicoAlvo = campanha.PublicoAlvo;
            campanhaDb.Observacoes = campanha.Observacoes;

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ExcluirCampanha(int id)
    {
        var campanha = _context.CampanhasVacinacao.Find(id);

        if (campanha != null)
        {
            _context.CampanhasVacinacao.Remove(campanha);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CriarCalendario(CalendarioVacinal calendario)
    {
        _context.CalendarioVacinal.Add(calendario);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditarCalendario(CalendarioVacinal calendario)
    {
        var item = _context.CalendarioVacinal.Find(calendario.Id);

        if (item != null)
        {
            item.Idade = calendario.Idade;
            item.Vacinas = calendario.Vacinas;

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ExcluirCalendario(int id)
    {
        var item = _context.CalendarioVacinal.Find(id);

        if (item != null)
        {
            _context.CalendarioVacinal.Remove(item);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult CriarLocal(LocalVacinacao local)
    {
        _context.LocaisVacinacao.Add(local);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditarLocal(LocalVacinacao local)
    {
        var item = _context.LocaisVacinacao.Find(local.Id);

        if (item != null)
        {
            item.Nome = local.Nome;
            item.Horario = local.Horario;

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ExcluirLocal(int id)
    {
        var local = _context.LocaisVacinacao.Find(id);

        if (local != null)
        {
            _context.LocaisVacinacao.Remove(local);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UploadPdfVacina(string titulo, IFormFile arquivo)
    {
        if (arquivo != null && arquivo.Length > 0)
        {
            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/Vacina");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var nomeArquivo = titulo.Replace(" ", "_") + ".pdf";

            var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // SALVAR NO BANCO
            var documento = new DocumentoVacina
            {
                Titulo = titulo,
                Caminho = "/pdf/Vacina/" + nomeArquivo
            };

            _context.DocumentosVacina.Add(documento);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult ExcluirPdfVacina(string nome)
    {
        var documento = _context.DocumentosVacina
            .FirstOrDefault(d => d.Caminho.Contains(nome));

        if (documento != null)
        {
            var caminho = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                documento.Caminho.TrimStart('/')
            );

            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);

            _context.DocumentosVacina.Remove(documento);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }





}

