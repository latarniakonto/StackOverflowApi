using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using StackOverflow.Infrastructure.Clients;

namespace StackOverflow.Infrastructure.Services;

public class TagService : ITagService
{
    private readonly MongoDbContext _dbContext;
    private readonly ITagsClient _tagsClient;

    public TagService(MongoDbContext dbContext, ITagsClient tagsClient)
    {
        _dbContext = dbContext;
        _tagsClient = tagsClient;
    }

    [HttpGet]
    public async Task<IEnumerable<Tag>> GetOrUpdateAllTagsAsync()
    {
        if (_tagsClient == null)
            throw new InvalidDataException("StackOverflow client is missing");

        List<ResponseTag> tags = await _tagsClient.GetDataAsync();
        int totalTagsCount = tags.Sum(t => t.Count);
            
        foreach(ResponseTag tag in tags)
        {
            FilterDefinition<Tag> filter = Builders<Tag>.Filter.Eq(t => t.Name, tag.Name);
            UpdateDefinition<Tag> update = Builders<Tag>.Update
                .Set(t => t.Count, tag.Count)
                .Set(t => t.Weight, (float)tag.Count / totalTagsCount);
            FindOneAndUpdateOptions<Tag, Tag> options = new FindOneAndUpdateOptions<Tag, Tag>
            {
                ReturnDocument = ReturnDocument.After
            };

            Tag dbTag = await _dbContext.Tags.FindOneAndUpdateAsync(filter, update, options);
            if (dbTag != null) continue;

            dbTag = new Tag()
            {
                Id = ObjectId.GenerateNewId(),
                Name = tag.Name,
                Count = tag.Count,
                Weight = (float)tag.Count / totalTagsCount,
            };
            await _dbContext.Tags.InsertOneAsync(dbTag);
        }
        return await _dbContext.Tags.Find(Builders<Tag>.Filter.Empty).ToListAsync();
    }

    [HttpGet]
    public async Task<IEnumerable<Tag>> GetTagsAsync()
    {
        if (_dbContext == null || _dbContext.Tags == null)
            throw new InvalidDataException("Database context is missing");

        return await _dbContext.Tags.Find(Builders<Tag>.Filter.Empty).ToListAsync();
    }
}
