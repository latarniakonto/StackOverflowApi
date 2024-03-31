
namespace StackOverflow.Infrastructure.Clients;

public interface IClientFactory
{
    public IClient GetApiClient(ClientType type);
}
