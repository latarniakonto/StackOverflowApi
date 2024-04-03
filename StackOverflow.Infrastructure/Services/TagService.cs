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

        IEnumerable<Tag> tags = await _tagsClient.GetDataAsync();
        int totalTagsCount = tags.Sum(t => t.Count);
            
        foreach(Tag tag in tags)
        {
            FilterDefinition<Tag> filter = Builders<Tag>.Filter.Eq(t => t.Name, tag.Name);
            UpdateDefinition<Tag> update = Builders<Tag>.Update
                .Set(t => t.Count, tag.Count)
                .Set(t => t.Weight, tag.Weight / totalTagsCount);
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
                AdditionalElements = tag.AdditionalElements
            };
            await _dbContext.Tags.InsertOneAsync(dbTag);
        }
        return tags;
    }

    [HttpGet]
    public async Task<IEnumerable<Tag>> GetTagsAsync(int page, int pageSize, string nameSortOrder, string weightSortOrder)
    {
        if (_dbContext == null || _dbContext.Tags == null)
            throw new InvalidDataException("Database context is missing");

        SortDefinition<Tag> nameSortDefinition = nameSortOrder.ToLower() switch
        {
            "asc" => Builders<Tag>.Sort.Ascending(t => t.Name),
            "desc" => Builders<Tag>.Sort.Descending(t => t.Name),
            _ => Builders<Tag>.Sort.Ascending(t => t.Name)
        };

        SortDefinition<Tag> weightSortDefinition = weightSortOrder.ToLower() switch
        {
            "asc" => Builders<Tag>.Sort.Ascending(t => t.Weight),
            "desc" => Builders<Tag>.Sort.Descending(t => t.Weight),
            _ => Builders<Tag>.Sort.Ascending(t => t.Weight)
        };

        return await _dbContext.Tags
            .Find(Builders<Tag>.Filter.Empty)
            .Sort(nameSortDefinition)
            .Sort(weightSortDefinition)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
}
