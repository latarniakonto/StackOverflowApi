
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StackOverflow.Infrastructure.Services;

public class Tag
{
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("count")]
    public int Count { get; set; }

    public float Weight { get; set; }

    [BsonExtraElements]
    public BsonDocument AdditionalElements { get; set; }
}
