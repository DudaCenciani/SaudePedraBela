// Importa funções básicas do sistema como datas, etc.
using System;

// Importa recursos para trabalhar com arquivos e pastas
using System.IO;

using System.Linq;
using System.Threading.Tasks;

// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para trabalhar com arquivos enviados pelo formulário
using Microsoft.AspNetCore.Http;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

// Importa os Models utilizados neste controller
using SaudePedraBela.Models;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;


public class EscalasAdminController : Controller
{
    // Variável que representa a conexão com o banco de dados
    // "readonly" significa que só pode ser definida uma vez (no construtor)
    private readonly SaudePedraBelaContext _context;

    // IWebHostEnvironment = dá acesso ao caminho físico da pasta wwwroot no servidor
    // Usado para montar o caminho completo onde os arquivos serão salvos
    private readonly IWebHostEnvironment _env;

    // Construtor do controller
    // O ASP.NET injeta automaticamente o contexto do banco E o ambiente do servidor
    public EscalasAdminController(SaudePedraBelaContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ===================================================
    // ACTION: Index (GET)
    // Página principal do admin de escalas
    // Lista todas as escalas cadastradas no banco
    // Acessada em: /EscalasAdmin
    // ===================================================
    public IActionResult Index()
    {
        // Busca todas as escalas do banco e converte para lista
        var escalas = _context.Escalas.ToList();

        // Retorna a View com a lista de escalas
        // (Views/EscalasAdmin/Index.cshtml)
        return View(escalas);
    }

    // ===================================================
    // ACTION: ListarArquivos (GET)
    // Retorna os arquivos de uma categoria específica em formato JSON
    // Chamada pelo JavaScript da tela pública de Escalas via fetch()
    // Exemplo: /EscalasAdmin/ListarArquivos?pasta=clinicos
    // ===================================================
    public IActionResult ListarArquivos(string pasta)
    {
        // Busca no banco apenas as escalas da categoria recebida
        // OrderByDescending = ordena do mais recente para o mais antigo
        // ThenByDescending = dentro do mesmo ano, ordena pelo mês mais recente
        // Select = molda os dados retornados (pega apenas os campos necessários)
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

        // Retorna os dados em formato JSON para o JavaScript consumir
        return Json(arquivos);
    }

    // ===================================================
    // ACTION: Upload (POST)
    // Faz o upload de um arquivo PDF de escala
    // Organiza os arquivos em pastas por categoria/ano/mês
    // Recebe: o arquivo PDF, a categoria, o mês e o ano
    // Exemplo de pasta gerada: wwwroot/pdf/escalas/clinicos/2026/03
    // ===================================================
    [HttpPost] // só responde a envios de formulário (POST)
    public async Task<IActionResult> Upload(IFormFile arquivo, string categoria, int mes, int ano)
    {
        // Log no console para debug — mostra a categoria recebida
        Console.WriteLine("Categoria recebida: " + categoria);

        // Verifica se um arquivo foi enviado
        if (arquivo != null && arquivo.Length > 0)
        {
            // Define o nome da pasta como o nome da categoria
            string pastaCategoria = categoria;

            // Monta o caminho completo da pasta onde o arquivo será salvo
            // Estrutura: wwwroot/pdf/escalas/{categoria}/{ano}/{mes}
            // Exemplo:   wwwroot/pdf/escalas/clinicos/2026/03
            var pasta = Path.Combine(
                _env.WebRootPath,  // caminho físico do wwwroot no servidor
                "pdf",
                "escalas",
                pastaCategoria,
                ano.ToString(),
                mes.ToString("D2") // D2 = sempre 2 dígitos (ex: 3 vira "03")
            );

            // Cria a pasta no servidor se ela ainda não existir
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            // Gera um nome único para o arquivo usando GUID
            // Isso evita conflito caso dois arquivos tenham o mesmo nome
            // Mantém a extensão original do arquivo (.pdf)
            var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo.FileName);

            // Monta o caminho completo do arquivo
            var caminho = Path.Combine(pasta, nomeArquivo);

            // Salva o arquivo fisicamente na pasta gerada
            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // Cria um novo objeto Escala com todos os dados do arquivo
            var escalas = new Escala
            {
                Categoria = categoria,          // ex: "clinicos"
                NomeArquivo = arquivo.FileName,   // nome original do arquivo

                // Caminho relativo usado para acessar o arquivo pelo navegador
                // Exemplo: /pdf/escalas/clinicos/2026/03/guid.pdf
                CaminhoArquivo = $"/pdf/escalas/{pastaCategoria}/{ano}/{mes:D2}/{nomeArquivo}",

                Mes = mes,           // mês da escala
                Ano = ano,           // ano da escala
                DataUpload = DateTime.Now   // data e hora do upload
            };

            // Adiciona o registro da escala ao banco de dados
            _context.Escalas.Add(escalas);

            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();
        }

        // Volta para a página principal do admin de escalas
        return RedirectToAction("Index");
    }

    // ===================================================
    // ACTION: Excluir
    // Remove uma escala do banco E o arquivo físico do servidor
    // Recebe o Id da escala pela URL
    // Exemplo: /EscalasAdmin/Excluir/5
    // ATENÇÃO: diferente de outros controllers, aqui o arquivo
    // físico também é deletado da pasta wwwroot
    // ===================================================
    public IActionResult Excluir(int id)
    {
        // Busca a escala no banco pelo Id recebido
        var escala = _context.Escalas.FirstOrDefault(e => e.Id == id);

        if (escala != null)
        {
            // Monta o caminho físico completo do arquivo no servidor
            // TrimStart('/') = remove a barra inicial do caminho relativo
            // para montar o caminho físico corretamente
            var caminho = Path.Combine(
                _env.WebRootPath,
                escala.CaminhoArquivo.TrimStart('/')
            );

            // Verifica se o arquivo existe fisicamente no servidor
            // e deleta o arquivo da pasta wwwroot
            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);

            // Remove o registro da escala do banco de dados
            _context.Escalas.Remove(escala);

            // Salva as alterações no banco de dados
            _context.SaveChanges();
        }

        // Volta para a página principal do admin de escalas
        return RedirectToAction("Index");
    }
}