using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Web.Controllers
{
    public class AdminController : Controller
    {
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}