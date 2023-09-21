using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IConversationService _conversationService;
        public ChatController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnswer(string q)
        {
            var answer = await _conversationService.GetAnswerAsync(q);
            return Ok(new { data = answer });
        }


    }
}
