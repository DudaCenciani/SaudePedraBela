// Importações necessárias para o controller de usuários
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // Aplica o filtro de login a todas as actions deste controller
    [LoginFilter]
    public class UsuariosController : Controller
    {
        // Contexto do banco de dados para acesso às tabelas via Entity Framework
        private readonly SaudePedraBelaContext _context;

        // Construtor: recebe o contexto por injeção de dependência
        public UsuariosController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        // Lista todos os usuários cadastrados no banco e envia para a View
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // Método executado automaticamente antes de qualquer action deste controller
        // Garante controle de acesso: se não há usuários, força a criação do primeiro;
        // se já existem, exige que o usuário esteja logado na sessão
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool existeUsuario = _context.Usuario.Any(); // Verifica se existe ao menos um usuário no banco
            var action = context.RouteData.Values["action"]?.ToString(); // Obtém o nome da action atual

            if (!existeUsuario)
            {
                // Se não existe nenhum usuário cadastrado, redireciona para a criação do primeiro,
                // bloqueando o acesso a qualquer outra página
                if (action != "Create")
                {
                    context.Result = RedirectToAction("Create");
                }
            }
            else
            {
                // Se já existem usuários, verifica se há uma sessão ativa (usuário logado)
                // Caso contrário, redireciona para a página de login
                if (HttpContext.Session.GetString("UsuarioLogado") == null)
                {
                    context.Result = RedirectToAction("Login", "Account");
                }
            }

            base.OnActionExecuting(context); // Chama a implementação base do método
        }

        // GET: Usuarios/Details/5
        // Exibe os detalhes de um usuário específico com base no ID informado
        public async Task<IActionResult> Details(int? id)
        {
            // Retorna erro 404 se nenhum ID foi informado
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário no banco pelo ID
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);

            // Retorna erro 404 se o usuário não foi encontrado
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        // Exibe o formulário para criação de um novo usuário
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // Recebe os dados do formulário e salva o novo usuário no banco
        // [Bind] limita os campos aceitos para evitar ataques de overposting
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra ataques CSRF
        public async Task<IActionResult> Create([Bind("Id,UsuarioLogin1,Senha1")] Usuario usuario)
        {
            // Só salva se os dados passarem nas validações do modelo
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redireciona para a listagem após salvar
            }
            return View(usuario); // Retorna o formulário com os erros caso a validação falhe
        }

        // GET: Usuarios/Edit/5
        // Exibe o formulário de edição preenchido com os dados do usuário informado
        public async Task<IActionResult> Edit(int? id)
        {
            // Retorna 404 se nenhum ID foi informado
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário pelo ID
            var usuario = await _context.Usuario.FindAsync(id);

            // Retorna 404 se o usuário não foi encontrado
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Recebe os dados editados e atualiza o usuário no banco
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra ataques CSRF
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioLogin1,Senha1")] Usuario usuario)
        {
            // Verifica se o ID da rota corresponde ao ID do usuário recebido
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario); // Marca o usuário como modificado no contexto
                    await _context.SaveChangesAsync(); // Persiste as alterações no banco
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Trata conflito de concorrência: se o usuário não existe mais, retorna 404;
                    // caso contrário, relança a exceção para tratamento superior
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redireciona para a listagem após editar
            }
            return View(usuario); // Retorna o formulário com erros se a validação falhar
        }

        // GET: Usuarios/Delete/5
        // Exibe a página de confirmação de exclusão com os dados do usuário
        public async Task<IActionResult> Delete(int? id)
        {
            // Retorna 404 se nenhum ID foi informado
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário pelo ID
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);

            // Retorna 404 se o usuário não foi encontrado
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        // Confirma a exclusão e remove o usuário do banco de dados
        [HttpPost, ActionName("Delete")] // Mapeia esta action para a rota "Delete" via POST
        [ValidateAntiForgeryToken] // Proteção contra ataques CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id); // Busca o usuário pelo ID

            // Remove o usuário do contexto apenas se ele existir
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync(); // Persiste a exclusão no banco
            return RedirectToAction(nameof(Index)); // Redireciona para a listagem após excluir
        }

        // Método auxiliar privado que verifica se um usuário com o ID informado existe no banco
        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}