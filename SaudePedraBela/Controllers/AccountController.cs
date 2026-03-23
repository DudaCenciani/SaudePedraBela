// Importa funções para trabalhar com listas e consultas
using System.Linq;

// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa o contexto do banco de dados do projeto
using SaudePedraBela.Data;

namespace SaudePedraBela.Controllers
{
    // Controller responsável por tudo relacionado a login e logout
    // Não tem [LoginFilter] porque precisa ser acessível sem estar logado
    public class AccountController : Controller
    {
        // Variável que representa a conexão com o banco de dados
        // "readonly" significa que só pode ser definida uma vez (no construtor)
        private readonly SaudePedraBelaContext _context;

        // Construtor do controller
        // O ASP.NET injeta automaticamente o contexto do banco aqui
        public AccountController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        // [AllowAnonymous] = permite acesso sem estar logado
        // [HttpGet] = essa action responde quando o usuário ACESSA a página (abre no navegador)
        // Recebe "returnUrl" = a URL que o usuário tentou acessar antes de ser redirecionado ao login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            // Salva a returnUrl para usar depois que o login for feito
            ViewBag.ReturnUrl = returnUrl;

            // Retorna a View da página de login (Views/Account/Login.cshtml)
            return View();
        }

        // [AllowAnonymous] = permite acesso sem estar logado
        // [HttpPost] = essa action responde quando o usuário ENVIA o formulário de login
        // Recebe os dados digitados no formulário: usuário, senha e returnUrl
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string usuarioLogin, string senha, string returnUrl)
        {
            // Busca no banco de dados um usuário onde o login E a senha batem com o que foi digitado
            // FirstOrDefault = retorna o primeiro encontrado, ou null se não achar nenhum
            var usuario = _context.Usuario
                .FirstOrDefault(u => u.UsuarioLogin1 == usuarioLogin && u.Senha1 == senha);

            // Se encontrou o usuário (login e senha corretos)
            if (usuario != null)
            {
                // Salva o nome do usuário logado na Session
                // A Session é como uma "memória temporária" que dura enquanto o navegador estiver aberto
                // É isso que o LoginFilter verifica para saber se está logado
                HttpContext.Session.SetString("UsuarioLogado", usuario.UsuarioLogin1);

                // Se o usuário tentou acessar uma página antes de logar
                // redireciona de volta para aquela página
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Se não tinha returnUrl, vai direto para o painel de administração
                return RedirectToAction("Index", "Administrar");
            }

            // Se chegou aqui, o usuário ou senha estão errados
            // Salva a mensagem de erro para mostrar na tela
            ViewBag.Erro = "Usuário ou senha inválidos";

            // Retorna a mesma View de login com a mensagem de erro
            return View();
        }

        // Action de logout — encerra a sessão do usuário
        public IActionResult Logout()
        {
            // Limpa TUDO que está salvo na Session
            // Após isso o LoginFilter vai barrar o acesso às páginas protegidas
            HttpContext.Session.Clear();

            // Redireciona para a página de login
            return RedirectToAction("Login");
        }
    }
}