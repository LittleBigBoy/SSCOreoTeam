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
    }
}
