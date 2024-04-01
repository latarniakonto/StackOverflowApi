using Microsoft.AspNetCore.Builder;
using StackOverflow.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace StackOverflow.Infrastructure;

public static class MongoDbInitializer
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            MongoDbContext dbContext = serviceScope.ServiceProvider.GetService<MongoDbContext>();

            if(dbContext == null || dbContext.Tags == null)
                throw new InvalidDataException("MongoDbContext service is missing");

            if (await dbContext.Tags.EstimatedDocumentCountAsync() >= 1000) return;

            await dbContext.Tags.DeleteManyAsync(FilterDefinition<Services.Tag>.Empty);

            ITagsClient client = serviceScope.ServiceProvider.GetService<ITagsClient>();

            List<Services.Tag> tags = await client.GetDataAsync();
            int totalTagsCount = tags.Sum(t => t.Count);
            
            foreach(Services.Tag tag in tags)
            {
                Services.Tag dbTag = new Services.Tag()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = tag.Name,
                    Count = tag.Count,
                    Weight = (float)tag.Count / totalTagsCount,
                    AdditionalElements = tag.AdditionalElements
                };
                await dbContext.Tags.InsertOneAsync(dbTag);
            }
        }
    }
}
