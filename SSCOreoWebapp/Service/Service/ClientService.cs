using Microsoft.Extensions.Caching.Memory;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;
using System.Net.Http.Headers;

namespace SSCOreoWebapp.Service.Service
{
    public class ClientService:IClientService
    {
        private readonly ICsvReadService _csvReadService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public ClientService(IConfiguration configuration, ICsvReadService csvReadService, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _csvReadService = csvReadService;
            _memoryCache = memoryCache;
        }

        public async Task<List<string>> GetClients()
        {
            var clientAllocPath = _configuration["AppSettings:ClientAllocFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ClientAllocModel>("ClientAllocFileData", clientAllocPath);
            var client = sourceDatas.Select(p => p.Client).Distinct();
            return client.ToList();
        }

        public List<string> GetServiceClients()
        {
            var clientAllocPath = _configuration["AppSettings:ClientServiceAllocFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ClientAllocAmountModel>("ClientServiceAllocFilePath", clientAllocPath);
            var client = sourceDatas.Select(p => p.Client).Distinct().OrderBy(p=>p);
            return client.ToList();
        }

        public List<ClientServiceResponseModel> GetServicesByClientName(string clientName)
        {
            var clientAllocPath = _configuration["AppSettings:ClientServiceAllocFilePath"];
            var clientAllocDatas = _csvReadService.GetCsvData<ClientAllocAmountModel>("ClientServiceAllocFilePath", clientAllocPath);
            var clientServices = clientAllocDatas.Where(p => p.Client == clientName).Select(p =>
                new ClientServiceAmountMapping { Service = p.Service, AllocPercent = p.AllocPercent, TotalInvestment = p.TotalInvestment}).Distinct();

            var serviceProfilePath = _configuration["AppSettings:ServiceProfileFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ServiceProfileModel>("ServiceProfileFilePath", serviceProfilePath);
            var serviceDatas = new List<ClientServiceResponseModel>();

            var totalPercent = clientServices.Sum(p => p.AllocPercent);
            var factor = 100 / totalPercent;
            foreach (var clientService in clientServices)
            {
                var serviceData = new ClientServiceResponseModel();
                var serviceProfile = sourceDatas.FirstOrDefault(p => p.Service == clientService.Service);
                if(serviceProfile== null) continue;
                var avgAnnualFeeRate = serviceProfile.AvgAnnualFeeRate;
                var profitMargin = serviceProfile.ProfitMargin;
                serviceData.Amount = Math.Round(clientService.TotalInvestment * factor * clientService.AllocPercent, 2);
                serviceData.Percentage = Math.Round(factor * clientService.AllocPercent , 2) + "%";
                serviceData.ServiceName = clientService.Service;
                serviceDatas.Add(serviceData);
            }
            return serviceDatas;
        }

        public async Task<PredictedNetIncomeModel> GetPredictedNetIncome(string clientName)
        {
            var clientProfilePath = _configuration["AppSettings:ClientProfileFilePath"];
            var clientProfileDatas = _csvReadService.GetCsvData<ClientProfileModel>("ClientProfileFilePath", clientProfilePath);
            var clientService = clientProfileDatas.FirstOrDefault(p => p.Client == clientName); 
            return await GetPredictValue(clientService);
        }

        private async Task<PredictedNetIncomeModel> GetPredictValue(ClientProfileModel clientProfile)
        {
            var model = new PredictedNetIncomeModel();
            try
            {
                var client = new HttpClient();
                var requestBody = $@"{{
    ""input_data"":{{
    ""columns"":[
        ""RiskTolerance"",
        ""Region"",
        ""TotalInvestment"",
""YearsOfInvestmentExperience""
        ],
        ""data"":[[""{clientProfile.RiskTolerance}"",""{clientProfile.Region}"",""{clientProfile.TotalInvestment}"",""{clientProfile.YearsOfInvestmentExperience}""]]
        }}
    }}";
                model.AnnualFeeRate = clientProfile.AnnualFeeRate;
                model.TotalServiceFee = clientProfile.TotalServiceFee;
                model.TotalServiceCost = clientProfile.TotalServiceCost;
                model.ProfitMargin = clientProfile.ProfitMargin;
                model.NetIncome = clientProfile.NetIncome;
                var apikey = _configuration["AppSettings:ClientPortfolioModelKey"];
                var endPoint = _configuration["AppSettings:ClientPortfolioModelEndPoint"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
                client.BaseAddress = new Uri(endPoint);
                var content = new StringContent(requestBody);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("azureml-model-deployment", "clientprofileautoallmodels-1");
                HttpResponseMessage reponse = await client.PostAsync("", content);

                if (reponse.IsSuccessStatusCode)
                {
                    string result = await reponse.Content.ReadAsStringAsync();
                    result = result.Replace("[", "").Replace("]", "");
                    model.PredictedNetIncome = Math.Round(double.Parse(result),2);
                }
                else
                {
                    model.PredictedNetIncome = 0;
                }

                return model;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
