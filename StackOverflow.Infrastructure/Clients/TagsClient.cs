using System.IO.Compression;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Infrastructure.Clients;

public class TagsClient : IClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private TagsResponse? _data;

    public class TagsResponse
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

    public async Task GetDataAsync()
    {
        await FetchDataFromApi();
    }

    private async Task FetchDataFromApi()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/2.3/tags?order=desc&sort=popular&site=stackoverflow");
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

                    _data = BsonSerializer.Deserialize<TagsResponse>(document);
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
