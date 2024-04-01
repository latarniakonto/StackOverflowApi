
namespace StackOverflow.Infrastructure.Clients;

public enum ClientType
{
    Tags = 1,
}
public class ClientFactory : IClientFactory
{
    private volatile Dictionary<ClientType, IClient> _apiClients = new Dictionary<ClientType, IClient>();
    private readonly object _lock = new object();

    public IClient GetApiClient(ClientType type)
    {
        if (!_apiClients.ContainsKey(type))
        {
            lock (_lock)
            {
                if (!_apiClients.ContainsKey(type))
                {
                    IClient apiClient;
                    switch (type)
                    {
                        case ClientType.Tags:
                            // apiClient = new TagsClient();
                            break;
                        
                        default: 
                            throw new ArgumentException("Not implemented api client exception");
                    }

                    // _apiClients[type] = apiClient;
                }
            }
        }

        return _apiClients[type];
    }
}
