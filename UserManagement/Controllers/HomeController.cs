using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult ResetPassword(string token)
        {
            ViewBag.token=token;
            return View();
        }
    }
}
