using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface IAllocService
    {
        Task<IEnumerable<ClientPortfoliosResponse>> GetClientPortfolios(string client, DateTime? startDate = null, DateTime? endDate = null);
    }
}
