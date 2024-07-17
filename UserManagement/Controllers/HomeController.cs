using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
