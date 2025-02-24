using Microsoft.AspNetCore.Mvc;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? orderId) 
        {
            return View();
        }
    }
}
