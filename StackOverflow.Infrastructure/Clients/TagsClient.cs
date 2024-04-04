using System.IO.Compression;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Infrastructure.Clients;

public class StackOverflowResponse
{
    [BsonElement("items")]
    public List<ResponseTag> Items { get; set; } = new List<ResponseTag>();

    [BsonElement("has_more")]
    public bool HasMore { get; set; } = false;

    [BsonExtraElements]
    public BsonDocument AdditionalElements { get; set; }
}

public class TagsClient : ITagsClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly StackOverflowResponse _response;

    public TagsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _response = new StackOverflowResponse();
    }

    public TagsClient()
    {
        _httpClient= new HttpClient { BaseAddress = new Uri("https://api.stackexchange.com/")};
        _response = new StackOverflowResponse();
    }

    public async Task<List<ResponseTag>> GetDataAsync()
    {
        int page = 1;
        bool hasMore = true;
        while (hasMore && _response.Items.Count < 1000)
        {
            hasMore = await FetchDataFromApi(page++);
        }

        return _response.Items;
    }

    private async Task<bool> FetchDataFromApi(int page)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/2.3/tags?order=desc&sort=popular&site=stackoverflow&pagesize=100&page={page}");
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Data fetch exception");

        if (!response.Content.Headers.ContentEncoding.Contains("gzip"))
            throw new InvalidDataException("Unsupported content encoding");

        using (Stream compressedStream = await response.Content.ReadAsStreamAsync())
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (StreamReader reader = new StreamReader(gzipStream, Encoding.UTF8))
        {
            string json = await reader.ReadToEndAsync();
            BsonDocument document = BsonDocument.Parse(json);
            StackOverflowResponse tags = BsonSerializer.Deserialize<StackOverflowResponse>(document);

            foreach (var tag in tags.Items)
            {
                if (_response.Items.Any(t => t.Name == tag.Name)) continue;

                _response.Items.Add(tag);
            }
            return tags.HasMore;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
