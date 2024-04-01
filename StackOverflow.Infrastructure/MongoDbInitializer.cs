using Microsoft.AspNetCore.Builder;
using StackOverflow.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace StackOverflow.Infrastructure;

public static class MongoDbInitializer
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetService<MongoDbContext>();
            var client = serviceScope.ServiceProvider.GetService<ITagsClient>();

            if(dbContext == null || dbContext.Tags == null)
                throw new InvalidDataException("MongoDbContext service is missing");

           await client.GetDataAsync();
        }
    }
}
