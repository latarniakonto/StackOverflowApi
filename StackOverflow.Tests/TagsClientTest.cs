using Moq;
using System.IO.Compression;
using System.Text;
using MongoDB.Bson;
using StackOverflow.Infrastructure.Clients;
using System.Net;
using StackOverflow.Infrastructure.Services;
using Moq.Protected;

namespace StackOverflow.Tests;

public class TagsClientTests
{
    [Fact]
    public async Task GetDataAsync_ReturnsListOfTags()
    {
        // Arrange
        var sampleResponse = new StackOverflowResponse();
        Random random = new Random();
        for (int i = 0; i < 1000; i++)
        {
            var tag = new ResponseTag()
            {
                Name = "Tag" + i.ToString(),
                Count = random.Next(0, 11)
            };
            sampleResponse.Items.Add(tag);
        }

        string json = sampleResponse.ToJson();
        byte[] compressedData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                byte[] jsonData = Encoding.UTF8.GetBytes(json);
                gzipStream.Write(jsonData, 0, jsonData.Length);
            }
            compressedData = memoryStream.ToArray();
        }
        Mock<HttpMessageHandler> mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync
            (
                new HttpResponseMessage
                { 
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(compressedData))
                    {
                        Headers = { ContentEncoding = { "gzip" } }
                    }
                }
            );
        HttpClient mockHttpClient =  new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://example.com") };


        var client = new TagsClient(mockHttpClient);

        // Act
        var result = await client.GetDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000, result.Count);
        Assert.Equal("Tag1", result[1].Name);
    }

    [Fact]
    public async Task GetDataAsync_ReturnsListOfTags_NoMoreTags()
    {
        // Arrange
        var sampleResponse = new StackOverflowResponse();
        sampleResponse.Items = [new ResponseTag() {Name = "Tag0", Count = 10}];
        sampleResponse.HasMore = false;

        string json = sampleResponse.ToJson();
        byte[] compressedData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                byte[] jsonData = Encoding.UTF8.GetBytes(json);
                gzipStream.Write(jsonData, 0, jsonData.Length);
            }
            compressedData = memoryStream.ToArray();
        }
        Mock<HttpMessageHandler> mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync
            (
                new HttpResponseMessage
                { 
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(compressedData))
                    {
                        Headers = { ContentEncoding = { "gzip" } }
                    }
                }
            );
        HttpClient mockHttpClient =  new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://example.com") };


        var client = new TagsClient(mockHttpClient);

        // Act
        var result = await client.GetDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tag0", result[0].Name);
    }

    [Fact]
    public async Task GetDataAsync_BadRequest()
    {
        // Arrange
        Mock<HttpMessageHandler> mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync
            (
                new HttpResponseMessage
                { 
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("")
                }
            );
        HttpClient mockHttpClient =  new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://example.com") };


        var client = new TagsClient(mockHttpClient);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetDataAsync());
    }

            [Fact]
    public async Task GetDataAsync_UnsupportedContentEncoding()
    {
        Mock<HttpMessageHandler> mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync
            (
                new HttpResponseMessage
                { 
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(new byte[3]{0, 0, 0}))
                    {
                        Headers = { ContentEncoding = { "deflate" } }
                    }
                }
            );
        HttpClient mockHttpClient =  new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://example.com") };

        var client = new TagsClient(mockHttpClient);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => client.GetDataAsync());
    }
}
