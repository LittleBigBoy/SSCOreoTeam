﻿using Microsoft.AspNetCore.Mvc;
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

        public List<ClientServiceResponseModel> GetCustomizeService(List<ClientServiceResponseModel> serviceModels)
        {
            var result = new List<ClientServiceResponseModel>();
            var serviceProfilePath = _configuration["AppSettings:ServiceProfileFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ServiceProfileModel>("ServiceProfileFilePath", serviceProfilePath);
            serviceModels.ForEach(t =>
            {
                var serviceProfile = new ClientServiceResponseModel();
                var service = sourceDatas.FirstOrDefault(p => p.Service == t.ServiceName);
                if (service == null)
                {
                    return;
                }

                serviceProfile.ServiceName = t.ServiceName;
                serviceProfile.Amount = Math.Round(t.Amount * service.AvgAnnualFeeRate * service.ProfitMargin / 10000, 2);
                result.Add(serviceProfile);
            });
            var totalAmount = result.Sum(p => p.Amount);
            result.ForEach(t =>
            {
                t.Percentage = Math.Round(t.Amount * 100 / totalAmount, 2) + "%";
            });
            return result;
        }

        public List<string> GetAllServices()
        {
            var serviceProfilePath = _configuration["AppSettings:ServiceProfileFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ServiceProfileModel>("ServiceProfileFilePath", serviceProfilePath);
            return sourceDatas.Select(p => p.Service).Distinct().ToList();
        }

        public async Task<IEnumerable<KeyValuePair<double,string>>> GetServiceScore(string clientName)
        {

            var clientProfilePath = _configuration["AppSettings:ClientProfileFilePath"];
            var clientProfileDatas = _csvReadService.GetCsvData<ClientProfileModel>("ClientProfileFilePath", clientProfilePath);
            var clientService = clientProfileDatas.FirstOrDefault(p => p.Client == clientName);
            var serviceScore = await GetPredictValue(clientService);

            var serviceProfilePath = _configuration["AppSettings:ServiceProfileFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ServiceProfileModel>("ServiceProfileFilePath", serviceProfilePath);
            var serviceDatas = new List<ClientServiceResponseModel>();
            var scoreServicePairs = sourceDatas
                .Select(p => new KeyValuePair<double, string>(Math.Abs(p.Score - serviceScore), p.Service))
                .OrderBy(p => p.Key).Take(8).ToList();

            return scoreServicePairs;
        }

        private async Task<double> GetPredictValue(ClientProfileModel clientProfile)
        {
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
                var apikey = _configuration["AppSettings:ServiceModelKey"];
                var endPoint = _configuration["AppSettings:ServiceModelEndPoint"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
                client.BaseAddress = new Uri(endPoint);
                var content = new StringContent(requestBody);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("azureml-model-deployment", "clientserviceallocmodel-1");
                HttpResponseMessage reponse = await client.PostAsync("", content);

                if (reponse.IsSuccessStatusCode)
                {
                    string result = await reponse.Content.ReadAsStringAsync();
                    result = result.Replace("[", "").Replace("]", "");
                    return double.Parse(result);
                }
                else
                {
                    string reponsecontent = await reponse.Content.ReadAsStringAsync();
                    return 0;
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
