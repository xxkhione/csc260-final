using Microsoft.AspNetCore.Mvc;

namespace UserData.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
