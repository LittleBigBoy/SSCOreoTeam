using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class PortfolioController : Controller
    {
        private readonly  ICsvReadService _csvReadService;
        public PortfolioController(ICsvReadService csvReadService)
        {
            _csvReadService = csvReadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoryPortfolio()
        {
            return Ok(await _csvReadService.GetCsvData());
        }
    }
}
