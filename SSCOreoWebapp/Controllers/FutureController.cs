using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class FutureController : Controller
    {
        [HttpGet("NAV")]
        public async Task<IActionResult> GetFutureNav()
        {
            var nav = new List<FutureModel>();
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            for (var i = 1; i <= 6; i++)
            {
                if (month + i > 12)
                    year += 1;
                var rand = new Random(100).NextDouble();
                var model = new FutureModel()
                {
                    Date = $"{year}/{month + i}/01",
                    Value = (1015068.37 + rand).ToString(CultureInfo.CurrentCulture)
                };
                nav.Add(model);
            }

            return Ok(nav);
        }
    }
}
