using Microsoft.AspNetCore.Mvc;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAnswer(string q)
        {
            var a = "Thanks for your question, I am learning now!";
            if (q.Contains("return"))
            {
                a = "After analysis, the return of portfolio A will be 10%";
            }
            else if (q.Contains("ratio"))
            {
                a = "After analysis, the ratio of portfolio A meet your requirement";
            }

            return Ok(new { data = a });
        }
    }
}
