using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Infrastructure.Clients;

public interface ITagsClient
{
    Task<List<ResponseTag>> GetDataAsync();
}
