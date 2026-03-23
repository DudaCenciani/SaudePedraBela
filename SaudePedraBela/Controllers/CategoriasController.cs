// Importa funções básicas do sistema como datas, listas, etc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa recursos para criar listas de seleção (dropdowns)
using Microsoft.AspNetCore.Mvc.Rendering;

// Importa recursos para consultas avançadas no banco (como o Include e FirstOrDefaultAsync)
using Microsoft.EntityFrameworkCore;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

// Importa os Models utilizados neste controller
using SaudePedraBela.Models;

// Importa recursos para trabalhar com arquivos enviados pelo formulário
using Microsoft.AspNetCore.Http;

// Importa recursos para criar filtros de action
using Microsoft.AspNetCore.Mvc.Filters;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // [LoginFilter] = protege TODAS as actions deste controller
    // Qualquer pessoa que tentar acessar /Categorias sem estar logada
    // será redirecionada automaticamente para a página de login
    [LoginFilter]
    public class CategoriasController : Controller
    {
        // Variável que representa a conexão com o banco de dados
        // "readonly" significa que só pode ser definida uma vez (no construtor)
        private readonly SaudePedraBelaContext _context;

        // Construtor do controller
        // O ASP.NET injeta automaticamente o contexto do banco de dados aqui
        public CategoriasController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        // ===================================================
        // ACTION: Index (GET)
        // Lista todas as categorias cadastradas no banco
        // Acessada em: /Categorias
        // ===================================================
        public async Task<IActionResult> Index()
        {
            // Busca todas as categorias do banco de forma assíncrona
            // e retorna para a View como lista
            return View(await _context.Categorias.ToListAsync());
        }

        // ===================================================
        // ACTION: Details (GET)
        // Exibe os detalhes de uma categoria específica
        // Acessada em: /Categorias/Details/5
        // Recebe o Id da categoria pela URL
        // ===================================================
        public async Task<IActionResult> Details(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca a categoria no banco pelo Id recebido
            // FirstOrDefaultAsync = retorna a primeira encontrada ou null
            var categorias = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            // Se não encontrou a categoria retorna erro 404
            if (categorias == null)
            {
                return NotFound();
            }

            // Retorna a View com os dados da categoria encontrada
            return View(categorias);
        }

        // ===================================================
        // ACTION: Create (GET)
        // Apenas exibe o formulário de criação de categoria
        // Acessada em: /Categorias/Create
        // ===================================================
        public IActionResult Create()
        {
            // Retorna a View com o formulário vazio
            return View();
        }

        // ===================================================
        // ACTION: Create (POST)
        // Salva uma nova categoria no banco de dados
        // Chamada quando o formulário de criação é enviado
        // [Bind] = define quais campos do formulário serão aceitos
        // (proteção contra envio de campos não autorizados)
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF (formulários falsos)
        public async Task<IActionResult> Create([Bind("Id,NomeCategoria")] Categorias categorias)
        {
            // Verifica se os dados recebidos são válidos
            // (campos obrigatórios preenchidos, tamanhos corretos, etc.)
            if (ModelState.IsValid)
            {
                // Adiciona a nova categoria ao banco de dados
                _context.Add(categorias);

                // Salva as alterações de forma assíncrona
                await _context.SaveChangesAsync();

                // Redireciona para a lista de categorias
                return RedirectToAction(nameof(Index));
            }

            // Se os dados não são válidos, retorna o formulário
            // com os erros de validação para o usuário corrigir
            return View(categorias);
        }

        // ===================================================
        // ACTION: Edit (GET)
        // Exibe o formulário de edição com os dados atuais da categoria
        // Acessada em: /Categorias/Edit/5
        // Recebe o Id da categoria pela URL
        // ===================================================
        public async Task<IActionResult> Edit(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca a categoria no banco pelo Id
            var categorias = await _context.Categorias.FindAsync(id);

            // Se não encontrou a categoria retorna erro 404
            if (categorias == null)
            {
                return NotFound();
            }

            // Retorna a View com os dados atuais da categoria
            // para o usuário editar
            return View(categorias);
        }

        // ===================================================
        // ACTION: Edit (POST)
        // Salva as alterações de uma categoria no banco de dados
        // Chamada quando o formulário de edição é enviado
        // ===================================================
        [HttpPost] // só responde a envios de formulário (POST)
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeCategoria")] Categorias categorias)
        {
            // Verifica se o Id da URL bate com o Id do formulário
            // (proteção contra alteração de registro errado)
            if (id != categorias.Id)
            {
                return NotFound();
            }

            // Verifica se os dados recebidos são válidos
            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza a categoria no banco de dados
                    _context.Update(categorias);

                    // Salva as alterações de forma assíncrona
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // DbUpdateConcurrencyException ocorre quando dois usuários
                    // tentam editar o mesmo registro ao mesmo tempo

                    // Verifica se a categoria ainda existe no banco
                    if (!CategoriasExists(categorias.Id))
                    {
                        // Se não existe mais retorna erro 404
                        return NotFound();
                    }
                    else
                    {
                        // Se existe mas houve outro erro, lança a exceção
                        throw;
                    }
                }

                // Redireciona para a lista de categorias
                return RedirectToAction(nameof(Index));
            }

            // Se os dados não são válidos, retorna o formulário
            // com os erros de validação para o usuário corrigir
            return View(categorias);
        }

        // ===================================================
        // ACTION: Delete (GET)
        // Exibe a página de confirmação de exclusão
        // Acessada em: /Categorias/Delete/5
        // Recebe o Id da categoria pela URL
        // ===================================================
        public async Task<IActionResult> Delete(int? id)
        {
            // Se não recebeu nenhum Id retorna erro 404
            if (id == null)
            {
                return NotFound();
            }

            // Busca a categoria no banco pelo Id
            var categorias = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            // Se não encontrou a categoria retorna erro 404
            if (categorias == null)
            {
                return NotFound();
            }

            // Retorna a View de confirmação com os dados da categoria
            // para o usuário confirmar a exclusão
            return View(categorias);
        }

        // ===================================================
        // ACTION: DeleteConfirmed (POST)
        // Exclui definitivamente a categoria do banco de dados
        // Chamada quando o usuário confirma a exclusão
        // [ActionName("Delete")] = usa a rota /Delete mas chama este método
        // (necessário pois não pode ter dois métodos Delete com a mesma assinatura)
        // ===================================================
        [HttpPost, ActionName("Delete")] // POST em /Categorias/Delete/5
        [ValidateAntiForgeryToken] // proteção contra ataques CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca a categoria no banco pelo Id
            var categorias = await _context.Categorias.FindAsync(id);

            // Se a categoria foi encontrada, remove do banco
            if (categorias != null)
            {
                _context.Categorias.Remove(categorias);
            }

            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Redireciona para a lista de categorias
            return RedirectToAction(nameof(Index));
        }

        // ===================================================
        // MÉTODO AUXILIAR: CategoriasExists
        // Verifica se uma categoria existe no banco pelo Id
        // Usado internamente pelo Edit para verificar concorrência
        // Retorna true se existe, false se não existe
        // ===================================================
        private bool CategoriasExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}