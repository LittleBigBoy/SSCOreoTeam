﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("serviceClients")]
        public IActionResult GetServiceClients()
        {
            return Ok(_clientService.GetServiceClients());
        }
        [HttpGet("{clientName}/services")]
        public IActionResult GetServicesByClientName(string clientName)
        {
            return Ok(_clientService.GetServicesByClientName(clientName));
        }

        [HttpGet("{client}/frequence/{frequence}/portfolios")]
        public async Task<IActionResult> GetPortfolios(string client, string frequence)
        {
            return Ok(await _allocService.GetClientPortfolios(client, frequence));
        }
        [HttpGet("{clientName}/PredictedNetIncome")]
        public async Task<IActionResult> GetPredictedNetIncome(string clientName)
        {
            return Ok(await _clientService.GetPredictedNetIncome(clientName));
        }
    }
}
