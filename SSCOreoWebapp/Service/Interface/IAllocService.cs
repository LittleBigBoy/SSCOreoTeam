using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface IAllocService
    {
        Task<IEnumerable<ClientPortfoliosResponse>> GetClientPortfolios(string client, string frequence, DateTime? startDate = null, DateTime? endDate = null);
        List<ClientServiceResponseModel> GetCustomizeService(List<ClientServiceResponseModel> serviceModels);
        List<string> GetAllServices();
        Task<IEnumerable<KeyValuePair<double, string>>> GetServiceScore(string clientName);
    }
}
