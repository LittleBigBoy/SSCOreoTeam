using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface IClientService
    {
        Task<List<string>> GetClients();
        List<string> GetServiceClients();
        List<ClientServiceResponseModel> GetServicesByClientName(string clientName);
        Task<PredictedNetIncomeModel> GetPredictedNetIncome(string clientName);
    }
}
