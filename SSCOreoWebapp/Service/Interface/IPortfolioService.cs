using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface IPortfolioService
    {
        Task GetPortfolioList(string clientName);
        IEnumerable<PortfolioData> GetPortfolioData(string portfolioName, DateTime? startDate = null, DateTime? endDate = null);
    }
}
