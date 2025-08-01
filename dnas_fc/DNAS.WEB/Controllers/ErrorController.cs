using Microsoft.AspNetCore.Mvc;

namespace DNAS.WEB.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
