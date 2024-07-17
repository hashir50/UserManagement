using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers.Api
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
