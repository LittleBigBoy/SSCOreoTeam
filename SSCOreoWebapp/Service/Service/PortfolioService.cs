using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;

namespace SSCOreoWebapp.Service.Service
{
    public class PortfolioService: IPortfolioService
    {
        private readonly ICsvReadService _csvReadService;
        private readonly IConfiguration _configuration;

        public PortfolioService(IConfiguration configuration, ICsvReadService csvReadService)
        {
            _configuration = configuration;
            _csvReadService = csvReadService;
        }

        public async Task GetPortfolioList(string clientName)
        {
            var portfolioHistoryPath = _configuration["AppSettings:PortfolioHistoryFilePath"];
            var sourceDatas =
                 _csvReadService.GetCsvData<PortfolioHistoryModel>("PortfolioHistoryFileData", portfolioHistoryPath);
            var result = new Dictionary<string, Dictionary<string, List<PortfolioData>>>();
            foreach (var record in sourceDatas)
            {
                var data = new PortfolioData()
                    { NAV = record.NAV, Return = record.Return, AsOf = record.AsOf.ToString("yyyy-MM-dd") };
                if (result.ContainsKey(record.Service))
                {
                    var portfolio = result[record.Service];
                    if (portfolio.ContainsKey(record.Portfolio))
                    {
                        portfolio[record.Portfolio].Add(data);
                    }
                    else
                    {
                        portfolio.Add(record.Portfolio, new List<PortfolioData>() { data });
                    }
                }
                else
                {
                    result.Add(record.Service,
                        new Dictionary<string, List<PortfolioData>>
                            { { record.Portfolio, new List<PortfolioData>() { data } } });
                }
            }
        }

        public IEnumerable<PortfolioData> GetPortfolioData(string portfolioName, DateTime? startDate=null, DateTime? endDate=null)
        {
            startDate = startDate ?? new DateTime(1970, 01, 01);
            endDate = endDate ?? DateTime.Now;
            var portfolioHistoryFilePath = _configuration["AppSettings:PortfolioHistoryFilePath"];
            var portfolioHistorySourceData =
                _csvReadService.GetCsvData<PortfolioHistoryModel>("PortfolioHistoryFileData", portfolioHistoryFilePath);
            var portfolioData = portfolioHistorySourceData.Where(p =>
                    p.Portfolio == portfolioName && p.AsOf > startDate && p.AsOf <= endDate)
                .OrderBy(p => p.AsOf).Select(p => new PortfolioData()
                    { NAV = p.NAV, Return = p.Return, AsOf = p.AsOf.ToString("yyyy-MM-dd") });
            return portfolioData;
        }
    }
}
