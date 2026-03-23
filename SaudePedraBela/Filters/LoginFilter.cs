// Importações necessárias para criar o filtro de autenticação
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SaudePedraBela.Filters
{
    // Filtro de autenticação reutilizável aplicado como atributo nos controllers
    // Ao decorar um controller ou action com [LoginFilter], todo acesso será verificado automaticamente
    public class LoginFilter : ActionFilterAttribute
    {
        // Método executado automaticamente antes de qualquer action protegida pelo filtro
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Verifica se existe uma sessão ativa com o usuário logado
            var session = context.HttpContext.Session.GetString("UsuarioLogado");

            // Se a sessão estiver vazia ou nula, o usuário não está autenticado:
            // interrompe a execução e redireciona para a página de login
            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context); // Chama a implementação base do filtro
        }
    }
}