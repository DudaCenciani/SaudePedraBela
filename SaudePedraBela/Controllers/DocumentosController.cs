// Importa funções básicas do sistema como datas, listas, etc.
using System;
using System.Collections.Generic;

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

// Importa recursos para criar filtros de action
using Microsoft.AspNetCore.Mvc.Filters;

// Importa recursos para criar listas de seleção (dropdowns)
using Microsoft.AspNetCore.Mvc.Rendering;

// Importa recursos para consultas avançadas no banco (como o Include)
using Microsoft.EntityFrameworkCore;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

// Importa os Models utilizados neste controller
using SaudePedraBela.Models;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;

//Importa o serviço de Dropbox para upload de arquivos
using SaudePedraBela.Services;

namespace SaudePedraBela.Controllers
{
 
    public class DocumentosController : Controller
    {
        // Variável que representa a conexão com o banco de dados
        // "readonly" significa que só pode ser definida uma vez (no construtor)
        private readonly SaudePedraBelaContext _context;
        private readonly DropboxService _dropboxService;

        // Construtor do controller
        // O ASP.NET injeta automaticamente o contexto do banco de dados aqui
        public DocumentosController(SaudePedraBelaContext context, DropboxService dropboxService)
        {
            _context = context;
            _dropboxService = dropboxService;
        }

        // ===================================================
        // ACTION: Index (GET)
        // Lista todos os documentos cadastrados no banco
        // Carrega também a categoria de cada documento (Include)
        // Acessada em: /Documentos
        // ===================================================
        public async Task<IActionResult> Index()
        {
            // Busca todos os documentos do banco
            // Include(d => d.Categorias) = carrega também a categoria vinculada
            // sem o Include, o campo Categoria viria nulo
            var documentos = _context.Documento.Include(d => d.Categorias);

            // Retorna a View com a lista de documentos
            return View(await documentos.ToListAsync());
        }

        // ===================================================
        // ACTION: Details (GET)
        // Exibe os detalhes de um documento específico
        // Acessada em: /Documentos/Details/5
        // Recebe o Id do documento pela URL
        // ===================================================
        public async Task<IActionResult> Details(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca o documento no banco pelo Id recebido
            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Id == id);

            // Se não encontrou o documento retorna erro 404
            if (documento == null)
            {
                return NotFound();
            }

            // Retorna a View com os dados do documento encontrado
            return View(documento);
        }

        // ===================================================
        // ACTION: Create (GET)
        // Exibe o formulário de criação de documento
        // Acessada em: /Documentos/Create
        // ===================================================
        public IActionResult Create()
        {
            // Cria uma lista de categorias para o dropdown do formulário
            // "Id" = valor que será salvo no banco
            // "NomeCategoria" = texto que aparece no dropdown para o usuário
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "NomeCategoria");

            // Retorna a View com o formulário vazio
            return View();
        }

        // ===================================================
        // ACTION: Create (POST)
        // Salva um novo documento no banco de dados
        // Chamada quando o formulário de criação é enviado
        // Recebe os dados do formulário + o arquivo PDF
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF (formulários falsos)
        public async Task<IActionResult> Create(Documento documento, IFormFile arquivoPdf)
        {
            // Verifica se um arquivo PDF foi enviado
            if (arquivoPdf != null && arquivoPdf.Length > 0)
            {
                // Pega apenas o nome do arquivo (sem o caminho completo)
                var nomeArquivo = Path.GetFileName(arquivoPdf.FileName);

                using (var stream = arquivoPdf.OpenReadStream())
                {
                    //Define a pasta no Dropbox onde será salvo o arquivo
                    string linkDropbox = await _dropboxService.UploadArquivo("/documentos", nomeArquivo, stream);
                    //Salva o link do dropbox no BD ao invés do caminho local
                    documento.CaminhoDocumento = linkDropbox;
                }

                /* CODIGO ANTIGO PARA SALVAR O ARQUIVO LOCALMENTE 
                    // Monta o caminho completo onde o arquivo será salvo no servidor
                    var caminho = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/documentos",
                        nomeArquivo
                    );

                // Salva o arquivo fisicamente na pasta wwwroot/documentos
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivoPdf.CopyToAsync(stream);
                }

                // Salva o caminho relativo do arquivo no documento
                // Esse caminho é usado para exibir/baixar o PDF no site
                documento.CaminhoDocumento = "/documentos/" + nomeArquivo;
            }*/
            }
                // Adiciona o documento ao banco de dados
                _context.Add(documento);

                // Salva as alterações de forma assíncrona
                await _context.SaveChangesAsync();

                // Redireciona para a lista de documentos
                return RedirectToAction(nameof(Index));
        }

        // ===================================================
        // ACTION: Edit (GET)
        // Exibe o formulário de edição com os dados atuais do documento
        // Acessada em: /Documentos/Edit/5
        // Recebe o Id do documento pela URL
        // ===================================================
        public async Task<IActionResult> Edit(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca o documento no banco pelo Id
            var documento = await _context.Documento.FindAsync(id);

            // Se não encontrou o documento retorna erro 404
            if (documento == null)
            {
                return NotFound();
            }

            // Retorna a View com os dados atuais do documento
            return View(documento);
        }

        // ===================================================
        // ACTION: ListaPublica (GET)
        // Retorna todos os documentos agrupados por categoria em formato JSON
        // É chamada pelo modal de documentos do site público via fetch()
        // NÃO é protegida pelo LoginFilter pois precisa ser acessível
        // por qualquer visitante do site
        // ===================================================
        [HttpGet]
        [IgnoreAntiforgeryToken] // não exige token de segurança pois é uma API pública
        public async Task<IActionResult> ListaPublica()
        {
            // Busca todas as categorias com seus documentos vinculados
            // Select = molda os dados retornados (pega apenas os campos necessários)
            var categorias = await _context.Categorias
                .Include(c => c.Documentos)
                .Select(c => new
                {
                    nomeCategoria = c.NomeCategoria,
                    documentos = c.Documentos.Select(d => new
                    {
                        nomeDocumento = d.NomeDocumento,
                        caminhoDocumento = d.CaminhoDocumento
                    })
                })
                .ToListAsync();

            // Retorna os dados em formato JSON para o JavaScript do site consumir
            return Json(categorias);
        }

        // ===================================================
        // ACTION: Edit (POST)
        // Salva as alterações de um documento no banco de dados
        // Chamada quando o formulário de edição é enviado
        // [Bind] = define quais campos do formulário serão aceitos
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeDocumento,CaminhoDocumento,IdCategoria")] Documento documento)
        {
            // Verifica se o Id da URL bate com o Id do formulário
            if (id != documento.Id)
            {
                return NotFound();
            }

            // Verifica se os dados recebidos são válidos
            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza o documento no banco de dados
                    _context.Update(documento);

                    // Salva as alterações de forma assíncrona
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Ocorre quando dois usuários editam o mesmo registro ao mesmo tempo
                    if (!DocumentoExists(documento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redireciona para a lista de documentos
                return RedirectToAction(nameof(Index));
            }

            // Se os dados não são válidos retorna o formulário com os erros
            return View(documento);
        }

        // ===================================================
        // ACTION: Delete (GET)
        // Exibe a página de confirmação de exclusão
        // Acessada em: /Documentos/Delete/5
        // Recebe o Id do documento pela URL
        // ===================================================
        public async Task<IActionResult> Delete(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca o documento no banco pelo Id
            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Id == id);

            // Se não encontrou o documento retorna erro 404
            if (documento == null)
            {
                return NotFound();
            }

            // Retorna a View de confirmação com os dados do documento
            return View(documento);
        }

        // ===================================================
        // ACTION: DeleteConfirmed (POST)
        // Exclui definitivamente o documento do banco de dados
        // Chamada quando o usuário confirma a exclusão
        // [ActionName("Delete")] = usa a rota /Delete mas chama este método
        // ===================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca o documento no banco pelo Id
            var documento = await _context.Documento.FindAsync(id);

            // Se o documento foi encontrado, remove do banco
            // ATENÇÃO: remove apenas o registro do banco
            // o arquivo físico na pasta wwwroot/documentos permanece
            if (documento != null)
            {
                //Remove o arquivo no DropBox (talvez seja necessário tratar a string do link salvo no BD)
                try
                {
                    await _dropboxService.DeletarArquivo($"/documentos/{documento.NomeDocumento}");
                }
                catch
                {
                    //local para tratar erros durante a exclusão do arquivo
                }

                _context.Documento.Remove(documento);
            }

            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Redireciona para a lista de documentos
            return RedirectToAction(nameof(Index));
        }

        // ===================================================
        // MÉTODO AUXILIAR: DocumentoExists
        // Verifica se um documento existe no banco pelo Id
        // Usado internamente pelo Edit para verificar concorrência
        // Retorna true se existe, false se não existe
        // ===================================================
        private bool DocumentoExists(int id)
        {
            return _context.Documento.Any(e => e.Id == id);
        }
    }
}