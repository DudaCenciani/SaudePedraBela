// Importa recursos de autorização do ASP.NET
using Microsoft.AspNetCore.Authorization;

// Importa recursos para trabalhar com arquivos enviados pelo formulário
using Microsoft.AspNetCore.Http;

// Importa recursos para criar controllers MVC
using Microsoft.AspNetCore.Mvc;

// Importa recursos para criar filtros de action
using Microsoft.AspNetCore.Mvc.Filters;

// Importa o filtro de login criado para proteger as páginas admin
using SaudePedraBela.Filters;

namespace SaudePedraBela.Controllers
{
    // [LoginFilter] = protege TODAS as actions deste controller
    // Qualquer pessoa que tentar acessar /Administrar sem estar logada
    // será redirecionada automaticamente para a página de login
    [LoginFilter]
    public class AdministrarController : Controller
    {
        // ===================================================
        // ACTION: Index
        // Página principal do painel administrativo
        // É a primeira tela que o usuário vê após fazer login
        // Exibe todos os módulos disponíveis para administrar
        // (Documentos, Categorias, Usuários, Escalas, Vacinação, Farmácia, Gestão)
        // ===================================================
        public IActionResult Index()
        {
            // Simplesmente retorna a View do painel
            // Não precisa buscar nada no banco pois
            // os cards de navegação são estáticos na View
            // (Views/Administrar/Index.cshtml)
            return View();
        }
    }
}