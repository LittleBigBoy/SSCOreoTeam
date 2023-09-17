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
    }
}
