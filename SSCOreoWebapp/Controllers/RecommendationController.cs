using Microsoft.AspNetCore.Mvc;

namespace SSCOreoWebapp.Controllers
{
    public class RecommendationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
