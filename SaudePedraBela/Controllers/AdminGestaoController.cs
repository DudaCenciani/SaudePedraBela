// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa recursos para consultas avançadas no banco (como o Include)
using Microsoft.EntityFrameworkCore;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

// Importa os Models utilizados neste controller
using SaudePedraBela.Models;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // [LoginFilter] = protege TODAS as actions deste controller
    // Qualquer pessoa que tentar acessar sem estar logada será redirecionada para o login
    [LoginFilter]
    public class AdminGestaoController : Controller
    {
        // Variável que representa a conexão com o banco de dados
        // "readonly" significa que só pode ser definida uma vez (no construtor)
        private readonly SaudePedraBelaContext _context;

        // Construtor do controller
        // O ASP.NET injeta automaticamente o contexto do banco de dados aqui
        public AdminGestaoController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        // ===================================================
        // ACTION: Index
        // Página principal do admin de gestão
        // Carrega todos os anos cadastrados junto com seus arquivos
        // ===================================================
        public IActionResult Index()
        {
            // Busca todos os anos de gestão do banco
            // Include(a => a.Arquivos) = carrega também os arquivos de cada ano
            // sem o Include, a lista de arquivos viria vazia
            var anos = _context.GestaoAnos
                .Include(a => a.Arquivos)
                .ToList();

            // Retorna a View passando a lista de anos como Model
            // (Views/AdminGestao/Index.cshtml)
            return View(anos);
        }

        // ===================================================
        // ACTION: CriarAno (POST)
        // Cria um novo ano de gestão no banco de dados
        // Chamada quando o formulário de novo ano é enviado
        // Exemplo: cadastrar o ano "2026"
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        public IActionResult CriarAno(string ano)
        {
            // Cria um novo objeto GestaoAno com o ano recebido do formulário
            var novo = new GestaoAno
            {
                Ano = ano
            };

            // Adiciona o novo ano ao banco de dados
            _context.GestaoAnos.Add(novo);

            // Salva as alterações no banco de dados
            _context.SaveChanges();

            // Volta para a página principal do admin de gestão
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: UploadArquivo (POST)
        // Faz o upload de um arquivo PDF e vincula a um ano de gestão
        // Recebe: o Id do ano, o título do arquivo e o arquivo PDF
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        public async Task<IActionResult> UploadArquivo(int anoId, string titulo, IFormFile arquivo)
        {
            // Verifica se um arquivo foi enviado
            if (arquivo != null)
            {
                // Gera um nome único para o arquivo usando GUID
                // Isso evita conflito de nomes caso dois arquivos tenham o mesmo nome
                // Mantém a extensão original do arquivo (.pdf)
                var nome = Guid.NewGuid() + Path.GetExtension(arquivo.FileName);

                // Monta o caminho completo onde o arquivo será salvo no servidor
                var caminho = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/pdf/gestao",
                    nome
                );

                // Salva o arquivo fisicamente na pasta wwwroot/pdf/gestao
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                // Cria um novo objeto GestaoArquivo com os dados do arquivo
                var novo = new GestaoArquivo
                {
                    Titulo = titulo,  // título digitado no formulário
                    Arquivo = nome,    // nome do arquivo gerado no servidor
                    GestaoAnoId = anoId  // vincula o arquivo ao ano correto
                };

                // Adiciona o arquivo ao banco de dados
                _context.GestaoArquivos.Add(novo);

                // Salva as alterações no banco de dados
                _context.SaveChanges();
            }

            // Volta para a página principal do admin de gestão
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: ExcluirAno
        // Remove um ano de gestão do banco de dados
        // Recebe o Id do ano pela URL
        // Exemplo: /AdminGestao/ExcluirAno/3
        // ATENÇÃO: ao excluir o ano, os arquivos vinculados
        // também são excluídos (depende da configuração do banco)
        // ===================================================
        public IActionResult ExcluirAno(int id)
        {
            // Busca o ano no banco pelo Id recebido
            var ano = _context.GestaoAnos.Find(id);

            // Remove o ano do banco de dados
            _context.GestaoAnos.Remove(ano);

            // Salva as alterações no banco de dados
            _context.SaveChanges();

            // Volta para a página principal do admin de gestão
            return RedirectToAction("Index");
        }

        // ===================================================
        // ACTION: ExcluirArquivo
        // Remove um arquivo PDF de gestão do banco de dados
        // Recebe o Id do arquivo pela URL
        // Exemplo: /AdminGestao/ExcluirArquivo/5
        // ATENÇÃO: remove apenas o registro do banco,
        // o arquivo físico na pasta wwwroot/pdf/gestao permanece
        // ===================================================
        public IActionResult ExcluirArquivo(int id)
        {
            // Busca o arquivo no banco pelo Id recebido
            var arq = _context.GestaoArquivos.Find(id);

            // Remove o arquivo do banco de dados
            _context.GestaoArquivos.Remove(arq);

            // Salva as alterações no banco de dados
            _context.SaveChanges();

            // Volta para a página principal do admin de gestão
            return RedirectToAction("Index");
        }
    }
}