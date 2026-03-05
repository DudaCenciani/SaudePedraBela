using Microsoft.AspNetCore.Mvc;
using SaudePedraBela.Data;
using System.Linq;

namespace SaudePedraBela.Controllers
{
    public class AccountController : Controller
    {
        private readonly SaudePedraBelaContext _context;

        public AccountController(SaudePedraBelaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuarioLogin, string senha, string returnUrl)
        {
            var usuario = _context.Usuario
                .FirstOrDefault(u => u.UsuarioLogin1 == usuarioLogin && u.Senha1 == senha);

            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioLogado", usuario.UsuarioLogin1);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Administrar");
            }

            ViewBag.Erro = "Usuário ou senha inválidos";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}