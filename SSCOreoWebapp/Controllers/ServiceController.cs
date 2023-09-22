using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;
using SSCOreoWebapp.Service.Service;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly IAllocService _allocService;
        public ServiceController(IAllocService allocService)
        {
            _allocService = allocService;
        }

        [HttpPost("customizeService")]
        public IActionResult GetServiceClients([FromBody] List<ClientServiceResponseModel> data)
        {
            return Ok(_allocService.GetCustomizeService(data));
        }
        [HttpGet]
        public IActionResult GetServices()
        {
            return Ok(_allocService.GetAllServices());
        }

        [HttpGet("client/{clientName}")]
        public async Task<IActionResult> GetServiceScore(string clientName)
        {
            var data = await _allocService.GetServiceScore(clientName);
            return Ok(data);
        }

    }
}
