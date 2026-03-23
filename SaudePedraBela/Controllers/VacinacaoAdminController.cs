// Importações necessárias para o controller de administração de vacinação
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;

// Aplica o filtro de login a todas as actions deste controller,
// exigindo autenticação para acessar qualquer funcionalidade administrativa
[LoginFilter]
public class VacinacaoAdminController : Controller
{
    // Contexto do banco de dados para acesso às tabelas via Entity Framework
    private readonly SaudePedraBelaContext _context;

    // Construtor: recebe o contexto por injeção de dependência
    public VacinacaoAdminController(SaudePedraBelaContext context)
    {
        _context = context;
    }

    // Página principal do painel de administração de vacinação:
    // Carrega todos os dados relacionados (campanhas, calendário, locais e documentos)
    // e os disponibiliza para a View via ViewBag e model principal
    public IActionResult Index()
    {
        var campanhas = _context.CampanhasVacinacao.ToList();
        var calendario = _context.CalendarioVacinal.ToList();
        var locais = _context.LocaisVacinacao.ToList();
        var documentos = _context.DocumentosVacina.ToList();

        ViewBag.Calendario = calendario;
        ViewBag.Locais = locais;
        ViewBag.Documentos = documentos;

        return View(campanhas); // Campanhas enviadas como model principal da View
    }

    // POST: Recebe os dados do formulário e adiciona uma nova campanha de vacinação ao banco
    [HttpPost]
    public IActionResult CriarCampanha(CampanhaVacinacao campanha)
    {
        _context.CampanhasVacinacao.Add(campanha);
        _context.SaveChanges();

        return RedirectToAction("Index"); // Redireciona para o painel após salvar
    }

    // POST: Recebe os dados editados de uma campanha e atualiza os campos no banco
    // Busca a campanha pelo ID e sobrescreve apenas os campos permitidos
    [HttpPost]
    public IActionResult EditarCampanha(CampanhaVacinacao campanha)
    {
        var campanhaDb = _context.CampanhasVacinacao.Find(campanha.Id); // Busca a campanha existente

        if (campanhaDb != null)
        {
            // Atualiza cada campo individualmente para evitar sobrescrita indevida
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

    // POST: Remove uma campanha de vacinação do banco pelo ID informado
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

    // POST: Adiciona um novo item ao calendário vacinal no banco
    [HttpPost]
    public IActionResult CriarCalendario(CalendarioVacinal calendario)
    {
        _context.CalendarioVacinal.Add(calendario);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // POST: Atualiza os dados de um item existente no calendário vacinal
    // Busca pelo ID e sobrescreve os campos de idade e vacinas
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

    // POST: Remove um item do calendário vacinal pelo ID informado
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

    // POST: Adiciona um novo local de vacinação ao banco
    [HttpPost]
    public IActionResult CriarLocal(LocalVacinacao local)
    {
        _context.LocaisVacinacao.Add(local);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // POST: Atualiza os dados de um local de vacinação existente
    // Busca pelo ID e sobrescreve os campos de nome e horário
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

    // POST: Remove um local de vacinação do banco pelo ID informado
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

    // POST: Faz o upload de um arquivo PDF relacionado à vacinação
    // Salva o arquivo fisicamente na pasta wwwroot/pdf/Vacina e registra o documento no banco
    [HttpPost]
    public async Task<IActionResult> UploadPdfVacina(string titulo, IFormFile arquivo)
    {
        if (arquivo != null && arquivo.Length > 0) // Verifica se um arquivo foi enviado
        {
            // Define o caminho físico da pasta de destino
            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/Vacina");

            // Cria a pasta caso ainda não exista
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            // Gera o nome do arquivo substituindo espaços por underscores e adicionando extensão .pdf
            var nomeArquivo = titulo.Replace(" ", "_") + ".pdf";

            var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            // Salva o arquivo no disco de forma assíncrona
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // Cria o registro do documento no banco com título e caminho relativo para acesso via URL
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

    // POST: Exclui um documento PDF de vacinação pelo nome informado
    // Remove o registro do banco e também apaga o arquivo físico do servidor
    [HttpPost]
    public IActionResult ExcluirPdfVacina(string nome)
    {
        // Busca o documento no banco cujo caminho contenha o nome informado
        var documento = _context.DocumentosVacina
            .FirstOrDefault(d => d.Caminho.Contains(nome));

        if (documento != null)
        {
            // Monta o caminho físico completo do arquivo no servidor
            var caminho = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                documento.Caminho.TrimStart('/') // Remove a barra inicial para montar o path corretamente
            );

            // Apaga o arquivo do disco se ele existir
            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);

            // Remove o registro do banco e salva
            _context.DocumentosVacina.Remove(documento);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}