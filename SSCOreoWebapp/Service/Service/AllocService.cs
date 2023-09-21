using Microsoft.AspNetCore.Mvc;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;
using System.Globalization;
using System.Net.Http.Headers;

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
           // await GetPredictValue();
            startDate = startDate ?? new DateTime(1970, 01, 01);
            endDate = endDate ?? DateTime.Now;
            var clientAllocPath = _configuration["AppSettings:ClientAllocFilePath"];
            var clientAllocSourceData = _csvReadService.GetCsvData<ClientAllocModel>("ClientAllocFileData", clientAllocPath);
            var clientAllocs = clientAllocSourceData
                .Where(p => string.Equals(p.Client, client, StringComparison.CurrentCultureIgnoreCase) &&
                            p.AsOf >= startDate && p.AsOf <= endDate);
            
            var result = new List<ClientPortfoliosResponse>();
            foreach (var clientAllocModel in clientAllocs)
            {
                var item = new ClientPortfoliosResponse() { PortfolioName = clientAllocModel.Portfolio };
                var portfolioData = _portfolioService.GetPortfolioData(clientAllocModel.Portfolio, startDate.Value, endDate.Value);
                if (frequence == "Prediction 6 month")
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

        private async Task GetPredictValue()
        {
            try
            {
                var client = new HttpClient();
                var requestBody = @"{
    ""input_data"":{
    ""columns"":[
        ""Service"",
        ""Portfolio"",
        ""AsOf""
        ],
        ""index"":[],
        ""data"":[[""Equilties I"",""Value Oriented"",""2823/9/1""]]
        }
    }";
                const string apikey = "BWWg6NSyVSIifaqJEue79lTGDk1IA00C";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
                client.BaseAddress = new Uri("https://oreo-ml-2023-port-prediction.eastasia.inference.ml.azure.com/score");
                var content = new StringContent(requestBody);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("azureml-model-deployment", "portfoliopredictionmodel-1");
                HttpResponseMessage reponse = await client.PostAsync("", content);

                if (reponse.IsSuccessStatusCode)
                {
                    string result = await reponse.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine(reponse.StatusCode);
                    Console.WriteLine(reponse.Headers.ToString());
                    string reponsecontent = await reponse.Content.ReadAsStringAsync();
                    Console.WriteLine(reponsecontent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
