using System.IO.Compression;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Infrastructure.Clients;

public class TagsClient : ITagsClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly TagsResponse _data;

    private class TagsResponse
    {
        [BsonElement("items")]
        public List<Tag> Items { get; set; } = new List<Tag>();

        [BsonElement("has_more")]
        public bool HasMore { get; set; } = false;

        [BsonExtraElements]
        public BsonDocument AdditionalElements { get; set; }
    }

    public TagsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _data = new TagsResponse();
    }

    public TagsClient()
    {
        _httpClient= new HttpClient { BaseAddress = new Uri("https://api.stackexchange.com/")};
        _data = new TagsResponse();
    }

    public async Task<List<Tag>> GetDataAsync()
    {
        int page = 1;
        while (_data.Items.Count < 1000)
        {
            await FetchDataFromApi(page++);
        }

        return _data.Items;
    }

    private async Task FetchDataFromApi(int page)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/2.3/tags?order=desc&sort=popular&site=stackoverflow&pagesize=100&page={page}");
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                using (Stream compressedStream = await response.Content.ReadAsStreamAsync())
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzipStream, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    BsonDocument document = BsonDocument.Parse(json);
                    TagsResponse tags = new TagsResponse();
                    tags = BsonSerializer.Deserialize<TagsResponse>(document);

                    _data.Items.AddRange(tags.Items);
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Data fetch exception");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
