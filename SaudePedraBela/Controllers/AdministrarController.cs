using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SaudePedraBela.Filters;


namespace SaudePedraBela.Controllers
{
    [LoginFilter]
    public class AdministrarController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }
    }
}