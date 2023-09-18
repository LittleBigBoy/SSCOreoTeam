using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;
using System.Globalization;

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

        public async Task<IEnumerable<ClientPortfoliosResponse>> GetClientPortfolios(string client, string frequence, DateTime? startDate = null,
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
                        { NAV = p.NAV, Return = p.Return, AsOf = p.AsOf.ToString("yyyy-MM-dd") });
                if (frequence == "Future")
                {
                    var nav = new List<PortfolioData>();
                    var month = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    var navValue = 0d;
                    for (var i = 1; i <= 6; i++)
                    {
                        var yearTemp = year;
                        if (month + i > 12)
                            yearTemp += 1;
                        navValue += 200000 + new Random(1015068).NextDouble();
                        var r = new Random();
                        var model = new PortfolioData()
                        {
                            AsOf = $"{yearTemp}/{month + i}/01",
                            NAV = 1015068.37 + navValue,
                            Return = r.NextDouble() * (4) + (-2d)
                        };
                        nav.Add(model);
                    }
                    
                    item.Data = nav;
                }
                else
                {
                    for (var i = 0; i < portfolioData.Count(); i++)
                    {
                        if (i % int.Parse(frequence) == 0)
                        {
                            item.Data.Add(portfolioData.ElementAt(i));
                        }
                    }
                    
                }
                portfolioData = item.Data;

                item.PortfolioData = portfolioData.LastOrDefault().NAV * clientAllocModel.SharesHoldingPercentage;

                result.Add(item);
            }

            var totalAmount = result.Sum(p => p.PortfolioData);
            result.ForEach(t =>
            {
                t.Percentage = Math.Round(t.PortfolioData * 100 / totalAmount, 2) + "%";
            });
            return result;
        }
    }
}
