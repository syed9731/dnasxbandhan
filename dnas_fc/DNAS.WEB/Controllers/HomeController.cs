using DNAS.Domian.DTO.FYI;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNAS.WEB.Controllers
{
    public class HomeController() : Controller
    {        
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Save(FyiModel Request)
        {
            return Redirect("/Login");
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
