using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Controllers
{
    [Route("api/[controller]")]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioService _portfolioService;
        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet("name/{portfolioName}/dataType/{dataType}")]
        public async Task<IActionResult> GetHistoryPortfolio(string portfolioName, string dataType)
        {
            return Ok(_portfolioService.GetPortfolioData(portfolioName));
        }
    }
}
