using SSCOreoWebapp.Models;

namespace SSCOreoWebapp.Service.Interface
{
    public interface IConversationService
    {
        Task<AnswerResponseModel> GetAnswerAsync(string question);
    }
}
