using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Service.Service
{
    public class AllocService:IAllocService
    {
        private readonly IConfiguration _configuration;
        private readonly IClientService _clientService;
        private readonly IPortfolioService _portfolioService;
        private readonly ICsvReadService _csvReadService;

        public AllocService(IConfiguration configuration, IClientService clientService, IPortfolioService portfolioService, ICsvReadService csvReadService)
        {
            _configuration = configuration;
            _clientService = clientService;
            _portfolioService = portfolioService;
            _csvReadService = csvReadService;
        }

        public async Task<IEnumerable<ClientPortfoliosResponse>> GetClientPortfolios(string client, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            startDate = startDate ?? new DateTime(1970, 01, 01);
            endDate = endDate ?? DateTime.Now;
            var clientAllocPath = _configuration["AppSettings:ClientAllocFilePath"];
            var clientAllocSourceData = _csvReadService.GetCsvData<ClientAllocModel>("ClientAllocFileData", clientAllocPath);
            var clientAllocs = clientAllocSourceData
                .Where(p => string.Equals(p.Client, client, StringComparison.CurrentCultureIgnoreCase) &&
                            p.AsOf >= startDate && p.AsOf <= endDate);
            var portfolioHistoryFilePath = _configuration["AppSettings:PortfolioHistoryFilePath"];
            var portfolioHistorySourceData =
                _csvReadService.GetCsvData<PortfolioHistoryModel>("PortfolioHistoryFileData", portfolioHistoryFilePath);
            var result = new List<ClientPortfoliosResponse>();
            foreach (var clientAllocModel in clientAllocs)
            {
                var item = new ClientPortfoliosResponse() { PortfolioName = clientAllocModel.Portfolio };
                var portfolioData = portfolioHistorySourceData.Where(p =>
                        p.Portfolio == clientAllocModel.Portfolio && p.AsOf > startDate && p.AsOf <= endDate)
                    .OrderBy(p => p.AsOf).Select(p => new PortfolioData()
                        { NAV = p.NAV, Return = p.Return, AsOf = p.AsOf });
                item.Data = portfolioData.ToList();

                item.PortfolioData = portfolioData.FirstOrDefault().NAV * clientAllocModel.SharesHoldingPercentage;

                result.Add(item);
            }
            return result;
        }
    }
}
