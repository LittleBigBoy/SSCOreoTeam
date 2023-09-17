namespace SSCOreoWebapp.Service.Interface
{
    public interface IClientService
    {
        Task<List<string>> GetClients();
    }
}
