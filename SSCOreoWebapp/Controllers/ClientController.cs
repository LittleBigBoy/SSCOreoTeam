using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IAllocService _allocService;

        public ClientController(IClientService clientService,IAllocService allocService)
        {
            _clientService = clientService;
            _allocService = allocService;
        }
        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            return Ok(await _clientService.GetClients());
        }

        [HttpGet("{client}/portfolios")]
        public async Task<IActionResult> GetPortfolios(string client)
        {
            return Ok(await _allocService.GetClientPortfolios(client));
        }
    }
}
