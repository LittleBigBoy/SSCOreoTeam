using Microsoft.Extensions.Caching.Memory;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;

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
            var clientAllocPath = _configuration["AppSettings:ClientAllocAmountFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ClientAllocAmountModel>("ClientAllocAmountFilePath", clientAllocPath);
            var client = sourceDatas.Select(p => p.Client).Distinct();
            return client.ToList();
        }

        public List<ClientServiceResponseModel> GetServicesByClientName(string clientName)
        {
            var clientAllocPath = _configuration["AppSettings:ClientAllocAmountFilePath"];
            var clientAllocDatas = _csvReadService.GetCsvData<ClientAllocAmountModel>("ClientAllocAmountFilePath", clientAllocPath);
            var clientServices = clientAllocDatas.Where(p => p.Client == clientName).Select(p =>
                new ClientServiceAmountMapping { Service = p.Service, AllocAmount = p.AllocAmount }).Distinct();

            var serviceProfilePath = _configuration["AppSettings:ServiceProfileFilePath"];
            var sourceDatas = _csvReadService.GetCsvData<ServiceProfileModel>("ServiceProfileFilePath", serviceProfilePath);
            var serviceDatas = new List<ClientServiceResponseModel>();
            //
            foreach (var clientService in clientServices)
            {
                var serviceData = new ClientServiceResponseModel();
                var serviceProfile = sourceDatas.FirstOrDefault(p => p.Service == clientService.Service);
                if(serviceProfile== null) continue;
                var avgAnnualFeeRate = serviceProfile.AvgAnnualFeeRate;
                var profitMargin = serviceProfile.ProfitMargin;
                serviceData.Amount = Math.Round(clientService.AllocAmount * avgAnnualFeeRate * profitMargin / 10000, 2);
                serviceData.ServiceName = clientService.Service;
                serviceDatas.Add(serviceData);
            }
            var totalAmount = serviceDatas.Sum(p => p.Amount);
            serviceDatas.ForEach(t => t.Percentage = Math.Round(t.Amount * 100 / totalAmount, 2) + "%");
            return serviceDatas;
        }
    }
}
