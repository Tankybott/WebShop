using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {

        // Redirect "/" or /User/Home/Index to the product browser
        public IActionResult Index()
        {
            return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
