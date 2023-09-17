using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface ICsvReadService
    {
        IEnumerable<T> GetCsvData<T>(string cacheKey, string path);
        Task<Dictionary<string, Dictionary<string, List<PortfolioData>>>> GetCsvData();
    }
}
