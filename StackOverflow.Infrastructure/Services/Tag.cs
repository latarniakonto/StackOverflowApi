using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StackOverflow.Infrastructure.Services;

public class Tag
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("count")]
    public int Count { get; set; }

    [BsonElement("weight")]
    public float Weight { get; set; }
}

public class ResponseTag
{
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("count")]
    public int Count { get; set; }

    [BsonExtraElements]
    public BsonDocument AdditionalElements { get; set; }
}
