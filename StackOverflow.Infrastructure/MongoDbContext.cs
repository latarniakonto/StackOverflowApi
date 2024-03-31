using MongoDB.Driver;

namespace StackOverflow.Infrastructure;

public class MongoDbContext
{
    public IMongoCollection<Services.Tag> Tags { get; set; }

    public MongoDbContext(IMongoDatabase database)
    {
        Tags = database.GetCollection<Services.Tag>("Tags");
    }
}
